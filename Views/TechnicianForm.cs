using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using LabTestSystem.Controllers;
using LabTestSystem.Models;

namespace LabTestSystem.Views;

public partial class TechnicianForm : Form
{
    private readonly TechnicianController _controller = AppServices.Technicians;

    private DataTable _open = new DataTable();
    private LabRequest _selected;
    private Bitmap _signature;
    private bool _signed;
    private bool _drawing;
    private Point _lastPoint;

    public TechnicianForm()
    {
        InitializeComponent();
    }

    private void TechnicianForm_Load(object sender, EventArgs e)
    {
        try
        {
            lblWho.Text = "Technician: " + _controller.CurrentTechnician.FullName;

            cmbWorkstation.DisplayMember = "Display";
            cmbWorkstation.ValueMember = "WorkstationID";
            cmbWorkstation.DataSource = _controller.GetWorkstations();

            SetRequest(null);
            ReloadTree();
        }
        catch (Exception ex) { Dialogs.Error(ex); }
    }

    private void TechnicianForm_FormClosed(object sender, FormClosedEventArgs e)
    {
        _controller.Logout();
    }

    // ---------- Lab Explorer tree: Section > Workstation > Requests ----------

    private void ReloadTree()
    {
        try
        {
            int selectedId = _selected == null ? -1 : _selected.RequestID;
            _open = _controller.GetOpenRequests();

            treeViewLab.BeginUpdate();
            treeViewLab.Nodes.Clear();
            TreeNode toSelect = null;

            foreach (var section in _open.AsEnumerable().GroupBy(r => r.Field<string>("SectionName")))
            {
                var sectionNode = new TreeNode(section.Key);

                foreach (var station in section.GroupBy(r => r.Field<string>("Workstation")))
                {
                    string label = station.Key == "" ? "Waiting for sampling" : station.Key;
                    var stationNode = new TreeNode(label + " (" + station.Count() + ")");

                    foreach (DataRow r in station)
                    {
                        int id = Convert.ToInt32(r["RequestID"]);
                        var node = new TreeNode(r["Barcode"] + "  " + r["FullName"] + "  [" + r["Status"] + "]");
                        node.Tag = id;
                        stationNode.Nodes.Add(node);
                        if (id == selectedId) toSelect = node;
                    }
                    sectionNode.Nodes.Add(stationNode);
                }
                treeViewLab.Nodes.Add(sectionNode);
            }

            treeViewLab.ExpandAll();
            treeViewLab.EndUpdate();
            if (toSelect != null) treeViewLab.SelectedNode = toSelect;
        }
        catch (Exception ex) { Dialogs.Error(ex); }
    }

    private void treeViewLab_AfterSelect(object sender, TreeViewEventArgs e)
    {
        if (!(e.Node.Tag is int id)) return;   // section and workstation nodes hold no request
        try { SetRequest(_controller.GetRequest(id)); }
        catch (Exception ex) { Dialogs.Error(ex); }
    }

    // ---------- Showing one request ----------

    private void SetRequest(LabRequest r)
    {
        _selected = r;
        ResetSignature();
        txtNotes.Clear();
        txtResults.Clear();
        UI.ShowImage(picBarCode, null);

        bool has = r != null;
        rdSampled.Enabled = has;
        rdProcessed.Enabled = has;
        rdRejected.Enabled = has;
        btnUpdateStatus.Enabled = has && r.Status != StatusRules.Processed && r.Status != StatusRules.Rejected;

        if (!has)
        {
            txtDetails.Text = "Select a request in the tree.";
            rdSampled.Checked = false;
            rdProcessed.Checked = false;
            rdRejected.Checked = false;
            EnableResults(false);
            return;
        }

        DataRow[] rows = _open.Select("RequestID=" + r.RequestID);
        string who = rows.Length > 0 ? rows[0]["FullName"] + " (" + rows[0]["ExamineeNo"] + ")" : "";

        txtDetails.Text = r.Barcode + Environment.NewLine +
            "Examinee: " + who + Environment.NewLine +
            "Tests: " + r.TestNames + Environment.NewLine +
            "Status: " + r.Status + (r.ResultsReleased ? "  (results released)" : "");

        rdSampled.Checked = r.Status == StatusRules.Sampled;
        rdProcessed.Checked = r.Status == StatusRules.Processed;
        rdRejected.Checked = r.Status == StatusRules.Rejected;

        txtResults.Text = r.Results;
        EnableResults(r.Status == StatusRules.Processed && !r.ResultsReleased);
        if (r.Status == StatusRules.Processed)
            UI.ShowImage(picBarCode, BarcodeRenderer.Render(r.Barcode));
    }

    private void EnableResults(bool enabled)
    {
        txtResults.ReadOnly = !enabled;
        picSig.Enabled = enabled;
        btnClearSignature.Enabled = enabled;
        btnSubmitResult.Enabled = enabled;
        btnReleaseResults.Enabled = enabled;
    }

    // ---------- Status change ----------

    private void btnUpdateStatus_Click(object sender, EventArgs e)
    {
        if (_selected == null)
        {
            Dialogs.Warn("Select a request first.");
            return;
        }

        string target = _selected.Status;
        if (rdSampled.Checked) target = StatusRules.Sampled;
        else if (rdProcessed.Checked) target = StatusRules.Processed;
        else if (rdRejected.Checked) target = StatusRules.Rejected;

        try
        {
            int station = Convert.ToInt32(cmbWorkstation.SelectedValue);
            _controller.ChangeStatus(_selected.RequestID, target, station, txtNotes.Text);

            int id = _selected.RequestID;
            ReloadTree();
            SetRequest(_controller.GetRequest(id));
            Dialogs.Info("Status updated to " + target + ".");
        }
        catch (Exception ex) { Dialogs.Error(ex); }
    }

    // ---------- Results ----------

    private void btnSubmitResult_Click(object sender, EventArgs e)
    {
        DoSaveResults(false);
    }

    private void btnReleaseResults_Click(object sender, EventArgs e)
    {
        DoSaveResults(true);
    }

    private void DoSaveResults(bool release)
    {
        if (_selected == null) return;

        try
        {
            _controller.SaveResults(_selected.RequestID, txtResults.Text, SignatureBytes(), release);

            int id = _selected.RequestID;
            ReloadTree();
            SetRequest(_controller.GetRequest(id));
            Dialogs.Info(release
                ? "Results released. The examinee can now see the report."
                : "Findings saved. The examinee cannot see them until you release them.");
        }
        catch (Exception ex) { Dialogs.Error(ex); }
    }

    // ---------- Signature drawn with the mouse ----------

    private void ResetSignature()
    {
        if (_signature != null) _signature.Dispose();
        _signature = new Bitmap(picSig.Width - 2, picSig.Height - 2);
        using (Graphics g = Graphics.FromImage(_signature))
        {
            g.Clear(Color.White);
        }
        _signed = false;
        UI.ShowImage(picSig, (Image)_signature.Clone());
    }

    private byte[] SignatureBytes()
    {
        if (!_signed) return null;
        using var stream = new MemoryStream();
        _signature.Save(stream, ImageFormat.Png);
        return stream.ToArray();
    }

    private void picSig_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left) return;
        _drawing = true;
        _lastPoint = e.Location;
    }

    private void picSig_MouseMove(object sender, MouseEventArgs e)
    {
        if (!_drawing) return;

        using (Graphics g = Graphics.FromImage(_signature))
        using (var pen = new Pen(Color.FromArgb(20, 30, 90), 2.2F))
        {
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawLine(pen, _lastPoint, e.Location);
        }
        _lastPoint = e.Location;
        _signed = true;
        UI.ShowImage(picSig, (Image)_signature.Clone());
    }

    private void picSig_MouseUp(object sender, MouseEventArgs e)
    {
        _drawing = false;
    }

    private void btnClearSignature_Click(object sender, EventArgs e)
    {
        ResetSignature();
    }

    // ---------- Toolbar ----------

    private void btnRefresh_Click(object sender, EventArgs e)
    {
        ReloadTree();
    }

    private void btnLogout_Click(object sender, EventArgs e)
    {
        Close();
    }

    // ---------- Layout (controls and events) ----------

    private void InitializeComponent()
    {
        lblTitle = new Label();
        lblWho = new Label();
        btnRefresh = new Button();
        btnLogout = new Button();
        grpStation = new GroupBox();
        cmbWorkstation = new ComboBox();
        grpTree = new GroupBox();
        treeViewLab = new TreeView();
        grpRequest = new GroupBox();
        txtDetails = new TextBox();
        rdSampled = new RadioButton();
        rdProcessed = new RadioButton();
        rdRejected = new RadioButton();
        lblNotes = new Label();
        txtNotes = new TextBox();
        btnUpdateStatus = new Button();
        grpResults = new GroupBox();
        txtResults = new TextBox();
        lblBarcode = new Label();
        picBarCode = new PictureBox();
        lblSign = new Label();
        picSig = new PictureBox();
        btnClearSignature = new Button();
        btnSubmitResult = new Button();
        btnReleaseResults = new Button();
        grpStation.SuspendLayout();
        grpTree.SuspendLayout();
        grpRequest.SuspendLayout();
        grpResults.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)picBarCode).BeginInit();
        ((System.ComponentModel.ISupportInitialize)picSig).BeginInit();
        SuspendLayout();
        // 
        // lblTitle
        // 
        lblTitle.AutoSize = true;
        lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
        lblTitle.ForeColor = Color.FromArgb(15, 118, 110);
        lblTitle.Location = new Point(20, 10);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(160, 41);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "Lab Explorer";
        // 
        // lblWho
        // 
        lblWho.Location = new Point(20, 52);
        lblWho.Name = "lblWho";
        lblWho.Size = new Size(500, 20);
        lblWho.TabIndex = 1;
        // 
        // btnRefresh
        // 
        btnRefresh.Location = new Point(770, 14);
        btnRefresh.Name = "btnRefresh";
        btnRefresh.Size = new Size(100, 34);
        btnRefresh.TabIndex = 2;
        btnRefresh.Text = "Refresh";
        btnRefresh.UseVisualStyleBackColor = true;
        btnRefresh.Click += btnRefresh_Click;
        // 
        // btnLogout
        // 
        btnLogout.Location = new Point(880, 14);
        btnLogout.Name = "btnLogout";
        btnLogout.Size = new Size(100, 34);
        btnLogout.TabIndex = 3;
        btnLogout.Text = "Logout";
        btnLogout.UseVisualStyleBackColor = true;
        btnLogout.Click += btnLogout_Click;
        // 
        // grpStation
        // 
        grpStation.Controls.Add(cmbWorkstation);
        grpStation.Location = new Point(20, 78);
        grpStation.Name = "grpStation";
        grpStation.Size = new Size(310, 62);
        grpStation.TabIndex = 4;
        grpStation.TabStop = false;
        grpStation.Text = "Working at workstation";
        // 
        // cmbWorkstation
        // 
        cmbWorkstation.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbWorkstation.FormattingEnabled = true;
        cmbWorkstation.Location = new Point(10, 26);
        cmbWorkstation.Name = "cmbWorkstation";
        cmbWorkstation.Size = new Size(290, 28);
        cmbWorkstation.TabIndex = 0;
        // 
        // grpTree
        // 
        grpTree.Controls.Add(treeViewLab);
        grpTree.Location = new Point(20, 148);
        grpTree.Name = "grpTree";
        grpTree.Size = new Size(310, 516);
        grpTree.TabIndex = 5;
        grpTree.TabStop = false;
        grpTree.Text = "Section > Workstation > Requests";
        // 
        // treeViewLab
        // 
        treeViewLab.Dock = DockStyle.Fill;
        treeViewLab.HideSelection = false;
        treeViewLab.Location = new Point(3, 23);
        treeViewLab.Name = "treeViewLab";
        treeViewLab.Size = new Size(304, 490);
        treeViewLab.TabIndex = 0;
        treeViewLab.AfterSelect += treeViewLab_AfterSelect;
        // 
        // grpRequest
        // 
        grpRequest.Controls.Add(btnUpdateStatus);
        grpRequest.Controls.Add(txtNotes);
        grpRequest.Controls.Add(lblNotes);
        grpRequest.Controls.Add(rdRejected);
        grpRequest.Controls.Add(rdProcessed);
        grpRequest.Controls.Add(rdSampled);
        grpRequest.Controls.Add(txtDetails);
        grpRequest.Location = new Point(345, 78);
        grpRequest.Name = "grpRequest";
        grpRequest.Size = new Size(635, 255);
        grpRequest.TabIndex = 6;
        grpRequest.TabStop = false;
        grpRequest.Text = "Selected request";
        // 
        // txtDetails
        // 
        txtDetails.Location = new Point(10, 26);
        txtDetails.Multiline = true;
        txtDetails.Name = "txtDetails";
        txtDetails.ReadOnly = true;
        txtDetails.Size = new Size(615, 96);
        txtDetails.TabIndex = 0;
        // 
        // rdSampled
        // 
        rdSampled.AutoSize = true;
        rdSampled.Location = new Point(10, 130);
        rdSampled.Name = "rdSampled";
        rdSampled.Size = new Size(90, 24);
        rdSampled.TabIndex = 1;
        rdSampled.Text = "SAMPLED";
        // 
        // rdProcessed
        // 
        rdProcessed.AutoSize = true;
        rdProcessed.Location = new Point(120, 130);
        rdProcessed.Name = "rdProcessed";
        rdProcessed.Size = new Size(106, 24);
        rdProcessed.TabIndex = 2;
        rdProcessed.Text = "PROCESSED";
        // 
        // rdRejected
        // 
        rdRejected.AutoSize = true;
        rdRejected.Location = new Point(240, 130);
        rdRejected.Name = "rdRejected";
        rdRejected.Size = new Size(97, 24);
        rdRejected.TabIndex = 3;
        rdRejected.Text = "REJECTED";
        // 
        // lblNotes
        // 
        lblNotes.AutoSize = true;
        lblNotes.Location = new Point(10, 162);
        lblNotes.Name = "lblNotes";
        lblNotes.Size = new Size(320, 20);
        lblNotes.TabIndex = 4;
        lblNotes.Text = "Rejection reason (required when REJECTED)";
        // 
        // txtNotes
        // 
        txtNotes.Location = new Point(10, 185);
        txtNotes.Name = "txtNotes";
        txtNotes.Size = new Size(615, 27);
        txtNotes.TabIndex = 5;
        // 
        // btnUpdateStatus
        // 
        btnUpdateStatus.Location = new Point(10, 217);
        btnUpdateStatus.Name = "btnUpdateStatus";
        btnUpdateStatus.Size = new Size(180, 32);
        btnUpdateStatus.TabIndex = 6;
        btnUpdateStatus.Text = "Update Status";
        btnUpdateStatus.UseVisualStyleBackColor = true;
        btnUpdateStatus.Click += btnUpdateStatus_Click;
        // 
        // grpResults
        // 
        grpResults.Controls.Add(btnReleaseResults);
        grpResults.Controls.Add(btnSubmitResult);
        grpResults.Controls.Add(btnClearSignature);
        grpResults.Controls.Add(picSig);
        grpResults.Controls.Add(lblSign);
        grpResults.Controls.Add(picBarCode);
        grpResults.Controls.Add(lblBarcode);
        grpResults.Controls.Add(txtResults);
        grpResults.Location = new Point(345, 338);
        grpResults.Name = "grpResults";
        grpResults.Size = new Size(635, 326);
        grpResults.TabIndex = 7;
        grpResults.TabStop = false;
        grpResults.Text = "Results (only for PROCESSED requests)";
        // 
        // txtResults
        // 
        txtResults.Location = new Point(10, 26);
        txtResults.Multiline = true;
        txtResults.Name = "txtResults";
        txtResults.ScrollBars = ScrollBars.Vertical;
        txtResults.Size = new Size(360, 211);
        txtResults.TabIndex = 0;
        // 
        // lblBarcode
        // 
        lblBarcode.AutoSize = true;
        lblBarcode.Location = new Point(385, 24);
        lblBarcode.Name = "lblBarcode";
        lblBarcode.Size = new Size(61, 20);
        lblBarcode.TabIndex = 1;
        lblBarcode.Text = "Barcode";
        // 
        // picBarCode
        // 
        picBarCode.BackColor = Color.White;
        picBarCode.BorderStyle = BorderStyle.FixedSingle;
        picBarCode.Location = new Point(385, 46);
        picBarCode.Name = "picBarCode";
        picBarCode.Size = new Size(240, 66);
        picBarCode.SizeMode = PictureBoxSizeMode.Zoom;
        picBarCode.TabIndex = 2;
        picBarCode.TabStop = false;
        // 
        // lblSign
        // 
        lblSign.AutoSize = true;
        lblSign.Location = new Point(385, 118);
        lblSign.Name = "lblSign";
        lblSign.Size = new Size(185, 20);
        lblSign.TabIndex = 3;
        lblSign.Text = "Sign below with the mouse";
        // 
        // picSig
        // 
        picSig.BackColor = Color.White;
        picSig.BorderStyle = BorderStyle.FixedSingle;
        picSig.Cursor = Cursors.Cross;
        picSig.Location = new Point(385, 140);
        picSig.Name = "picSig";
        picSig.Size = new Size(240, 97);
        picSig.SizeMode = PictureBoxSizeMode.Normal;
        picSig.TabIndex = 4;
        picSig.TabStop = false;
        picSig.MouseDown += picSig_MouseDown;
        picSig.MouseMove += picSig_MouseMove;
        picSig.MouseUp += picSig_MouseUp;
        // 
        // btnClearSignature
        // 
        btnClearSignature.Location = new Point(385, 243);
        btnClearSignature.Name = "btnClearSignature";
        btnClearSignature.Size = new Size(240, 28);
        btnClearSignature.TabIndex = 5;
        btnClearSignature.Text = "Clear Signature";
        btnClearSignature.UseVisualStyleBackColor = true;
        btnClearSignature.Click += btnClearSignature_Click;
        // 
        // btnSubmitResult
        // 
        btnSubmitResult.Location = new Point(10, 280);
        btnSubmitResult.Name = "btnSubmitResult";
        btnSubmitResult.Size = new Size(260, 36);
        btnSubmitResult.TabIndex = 6;
        btnSubmitResult.Text = "Save Findings";
        btnSubmitResult.UseVisualStyleBackColor = true;
        btnSubmitResult.Click += btnSubmitResult_Click;
        // 
        // btnReleaseResults
        // 
        btnReleaseResults.Location = new Point(280, 280);
        btnReleaseResults.Name = "btnReleaseResults";
        btnReleaseResults.Size = new Size(345, 36);
        btnReleaseResults.TabIndex = 7;
        btnReleaseResults.Text = "Release Results to Examinee";
        btnReleaseResults.UseVisualStyleBackColor = true;
        btnReleaseResults.Click += btnReleaseResults_Click;
        // 
        // TechnicianForm
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 680);
        Controls.Add(grpResults);
        Controls.Add(grpRequest);
        Controls.Add(grpTree);
        Controls.Add(grpStation);
        Controls.Add(btnLogout);
        Controls.Add(btnRefresh);
        Controls.Add(lblWho);
        Controls.Add(lblTitle);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        Name = "TechnicianForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Technician - Lab Explorer";
        FormClosed += TechnicianForm_FormClosed;
        Load += TechnicianForm_Load;
        grpStation.ResumeLayout(false);
        grpTree.ResumeLayout(false);
        grpRequest.ResumeLayout(false);
        grpRequest.PerformLayout();
        grpResults.ResumeLayout(false);
        grpResults.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)picBarCode).EndInit();
        ((System.ComponentModel.ISupportInitialize)picSig).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private Label lblTitle;
    private Label lblWho;
    private Button btnRefresh;
    private Button btnLogout;
    private GroupBox grpStation;
    private ComboBox cmbWorkstation;
    private GroupBox grpTree;
    private TreeView treeViewLab;
    private GroupBox grpRequest;
    private TextBox txtDetails;
    private RadioButton rdSampled;
    private RadioButton rdProcessed;
    private RadioButton rdRejected;
    private Label lblNotes;
    private TextBox txtNotes;
    private Button btnUpdateStatus;
    private GroupBox grpResults;
    private TextBox txtResults;
    private Label lblBarcode;
    private PictureBox picBarCode;
    private Label lblSign;
    private PictureBox picSig;
    private Button btnClearSignature;
    private Button btnSubmitResult;
    private Button btnReleaseResults;
}