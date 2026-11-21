using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using LabTestSystem.Controllers;
using LabTestSystem.Models;

namespace LabTestSystem.Views;

public partial class ResultsForm : Form
{
    private readonly RequestController _controller = AppServices.Requests;

    public ResultsForm()
    {
        InitializeComponent();
    }

    private void btnFind_Click(object sender, EventArgs e)
    {
        try
        {
            dgvRequests.DataSource = _controller.GetRequestsOf(txtExamineeNo.Text);

            foreach (string column in new[] { "RequestID", "SectionID", "ExamineeNo", "FullName", "Workstation" })
            {
                if (dgvRequests.Columns[column] != null)
                    dgvRequests.Columns[column].Visible = false;
            }
            dgvRequests.ClearSelection();

            ShowMessage(dgvRequests.Rows.Count == 0
                ? "No requests were found for this number."
                : "Select a request to see its status or report.");
        }
        catch (Exception ex) { Dialogs.Error(ex); }
    }

    private void dgvRequests_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;

        DataGridViewRow row = dgvRequests.Rows[e.RowIndex];
        int id = Convert.ToInt32(row.Cells["RequestID"].Value);
        string barcode = Convert.ToString(row.Cells["Barcode"].Value);
        string status = Convert.ToString(row.Cells["Status"].Value);

        try
        {
            if (!Convert.ToBoolean(row.Cells["ResultsReleased"].Value))
            {
                string text = "The results of " + barcode + " are not released yet." +
                              Environment.NewLine + "Current status: " + status;
                if (status == StatusRules.Rejected)
                    text += Environment.NewLine + "Reason: " + Convert.ToString(row.Cells["Notes"].Value);
                ShowMessage(text);
                return;
            }

            LabRequest r = _controller.GetRequest(id);
            txtReport.Text =
                "LAB TEST REPORT" + Environment.NewLine + Environment.NewLine +
                "Request:   " + barcode + Environment.NewLine +
                "Examinee:  " + row.Cells["FullName"].Value + " (" + row.Cells["ExamineeNo"].Value + ")" + Environment.NewLine +
                "Section:   " + row.Cells["SectionName"].Value + Environment.NewLine +
                "Tests:     " + r.TestNames + Environment.NewLine + Environment.NewLine +
                "FINDINGS" + Environment.NewLine + r.Results;

            UI.ShowImage(picBarCode, BarcodeRenderer.Render(barcode));
            UI.ShowImage(picSig, r.Signature == null ? null : ToImage(r.Signature));
        }
        catch (Exception ex) { Dialogs.Error(ex); }
    }

    private void ShowMessage(string text)
    {
        txtReport.Text = text;
        UI.ShowImage(picBarCode, null);
        UI.ShowImage(picSig, null);
    }

    private static Image ToImage(byte[] png)
    {
        try
        {
            using var stream = new MemoryStream(png);
            using var loaded = Image.FromStream(stream);
            return new Bitmap(loaded);
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void InitializeComponent()
    {
        lblTitle = new Label();
        lblExamineeNo = new Label();
        txtExamineeNo = new TextBox();
        btnFind = new Button();
        dgvRequests = new DataGridView();
        txtReport = new TextBox();
        lblBarcode = new Label();
        picBarCode = new PictureBox();
        lblSignature = new Label();
        picSig = new PictureBox();
        btnClose = new Button();
        ((System.ComponentModel.ISupportInitialize)dgvRequests).BeginInit();
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
        lblTitle.Size = new Size(320, 41);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "My Requests and Results";
        // 
        // lblExamineeNo
        // 
        lblExamineeNo.AutoSize = true;
        lblExamineeNo.Location = new Point(20, 68);
        lblExamineeNo.Name = "lblExamineeNo";
        lblExamineeNo.Size = new Size(131, 20);
        lblExamineeNo.TabIndex = 1;
        lblExamineeNo.Text = "Examinee Number";
        // 
        // txtExamineeNo
        // 
        txtExamineeNo.Location = new Point(170, 65);
        txtExamineeNo.Name = "txtExamineeNo";
        txtExamineeNo.Size = new Size(200, 27);
        txtExamineeNo.TabIndex = 2;
        // 
        // btnFind
        // 
        btnFind.Location = new Point(385, 61);
        btnFind.Name = "btnFind";
        btnFind.Size = new Size(170, 34);
        btnFind.TabIndex = 3;
        btnFind.Text = "Find My Requests";
        btnFind.UseVisualStyleBackColor = true;
        btnFind.Click += btnFind_Click;
        // 
        // dgvRequests
        // 
        dgvRequests.AllowUserToAddRows = false;
        dgvRequests.AllowUserToDeleteRows = false;
        dgvRequests.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvRequests.BackgroundColor = Color.White;
        dgvRequests.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvRequests.Location = new Point(20, 108);
        dgvRequests.MultiSelect = false;
        dgvRequests.Name = "dgvRequests";
        dgvRequests.ReadOnly = true;
        dgvRequests.RowHeadersVisible = false;
        dgvRequests.RowHeadersWidth = 51;
        dgvRequests.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvRequests.Size = new Size(400, 400);
        dgvRequests.TabIndex = 4;
        dgvRequests.CellClick += dgvRequests_CellClick;
        // 
        // txtReport
        // 
        txtReport.Location = new Point(435, 108);
        txtReport.Multiline = true;
        txtReport.Name = "txtReport";
        txtReport.ReadOnly = true;
        txtReport.ScrollBars = ScrollBars.Vertical;
        txtReport.Size = new Size(405, 220);
        txtReport.TabIndex = 5;
        // 
        // lblBarcode
        // 
        lblBarcode.AutoSize = true;
        lblBarcode.Location = new Point(435, 334);
        lblBarcode.Name = "lblBarcode";
        lblBarcode.Size = new Size(61, 20);
        lblBarcode.TabIndex = 6;
        lblBarcode.Text = "Barcode";
        // 
        // picBarCode
        // 
        picBarCode.BackColor = Color.White;
        picBarCode.BorderStyle = BorderStyle.FixedSingle;
        picBarCode.Location = new Point(435, 356);
        picBarCode.Name = "picBarCode";
        picBarCode.Size = new Size(405, 60);
        picBarCode.SizeMode = PictureBoxSizeMode.Zoom;
        picBarCode.TabIndex = 7;
        picBarCode.TabStop = false;
        // 
        // lblSignature
        // 
        lblSignature.AutoSize = true;
        lblSignature.Location = new Point(435, 422);
        lblSignature.Name = "lblSignature";
        lblSignature.Size = new Size(147, 20);
        lblSignature.TabIndex = 8;
        lblSignature.Text = "Technician signature";
        // 
        // picSig
        // 
        picSig.BackColor = Color.White;
        picSig.BorderStyle = BorderStyle.FixedSingle;
        picSig.Location = new Point(435, 444);
        picSig.Name = "picSig";
        picSig.Size = new Size(405, 64);
        picSig.SizeMode = PictureBoxSizeMode.Zoom;
        picSig.TabIndex = 9;
        picSig.TabStop = false;
        // 
        // btnClose
        // 
        btnClose.Location = new Point(740, 516);
        btnClose.Name = "btnClose";
        btnClose.Size = new Size(100, 32);
        btnClose.TabIndex = 10;
        btnClose.Text = "Close";
        btnClose.UseVisualStyleBackColor = true;
        btnClose.Click += btnClose_Click;
        // 
        // ResultsForm
        // 
        AcceptButton = btnFind;
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(860, 560);
        Controls.Add(btnClose);
        Controls.Add(picSig);
        Controls.Add(lblSignature);
        Controls.Add(picBarCode);
        Controls.Add(lblBarcode);
        Controls.Add(txtReport);
        Controls.Add(dgvRequests);
        Controls.Add(btnFind);
        Controls.Add(txtExamineeNo);
        Controls.Add(lblExamineeNo);
        Controls.Add(lblTitle);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        Name = "ResultsForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "My Requests and Results";
        ((System.ComponentModel.ISupportInitialize)dgvRequests).EndInit();
        ((System.ComponentModel.ISupportInitialize)picBarCode).EndInit();
        ((System.ComponentModel.ISupportInitialize)picSig).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private Label lblTitle;
    private Label lblExamineeNo;
    private TextBox txtExamineeNo;
    private Button btnFind;
    private DataGridView dgvRequests;
    private TextBox txtReport;
    private Label lblBarcode;
    private PictureBox picBarCode;
    private Label lblSignature;
    private PictureBox picSig;
    private Button btnClose;
}