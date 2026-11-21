 using System;
using System.Drawing;
using System.Windows.Forms;

namespace LabTestSystem.Views;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
    }

    private void btnExaminees_Click(object sender, EventArgs e)
    {
        using var form = new ExamineeForm();
        form.ShowDialog(this);
    }

    private void btnRequests_Click(object sender, EventArgs e)
    {
        using var form = new RequestForm();
        form.ShowDialog(this);
    }

    private void btnResults_Click(object sender, EventArgs e)
    {
        using var form = new ResultsForm();
        form.ShowDialog(this);
    }

    private void btnTechnician_Click(object sender, EventArgs e)
    {
        using (var login = new TechnicianLoginForm())
        {
            if (login.ShowDialog(this) != DialogResult.OK) return;
        }

        using var form = new TechnicianForm();
        form.ShowDialog(this);
    }

    private void btnExit_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void InitializeComponent()
    {
        lblTitle = new Label();
        lblSubtitle = new Label();
        btnExaminees = new Button();
        btnRequests = new Button();
        btnResults = new Button();
        btnTechnician = new Button();
        btnExit = new Button();
        SuspendLayout();
        // 
        // lblTitle
        // 
        lblTitle.AutoSize = true;
        lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
        lblTitle.ForeColor = Color.FromArgb(15, 118, 110);
        lblTitle.Location = new Point(30, 20);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(200, 41);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "Lab Test System";
        // 
        // lblSubtitle
        // 
        lblSubtitle.AutoSize = true;
        lblSubtitle.Location = new Point(32, 68);
        lblSubtitle.Name = "lblSubtitle";
        lblSubtitle.Size = new Size(350, 20);
        lblSubtitle.TabIndex = 1;
        lblSubtitle.Text = "Digital laboratory requests, specimen tracking and results.";
        // 
        // btnExaminees
        // 
        btnExaminees.Location = new Point(30, 110);
        btnExaminees.Name = "btnExaminees";
        btnExaminees.Size = new Size(380, 48);
        btnExaminees.TabIndex = 2;
        btnExaminees.Text = "Manage Examinees";
        btnExaminees.UseVisualStyleBackColor = true;
        btnExaminees.Click += btnExaminees_Click;
        // 
        // btnRequests
        // 
        btnRequests.Location = new Point(30, 170);
        btnRequests.Name = "btnRequests";
        btnRequests.Size = new Size(380, 48);
        btnRequests.TabIndex = 3;
        btnRequests.Text = "Submit and Manage Test Requests";
        btnRequests.UseVisualStyleBackColor = true;
        btnRequests.Click += btnRequests_Click;
        // 
        // btnResults
        // 
        btnResults.Location = new Point(30, 230);
        btnResults.Name = "btnResults";
        btnResults.Size = new Size(380, 48);
        btnResults.TabIndex = 4;
        btnResults.Text = "View My Requests and Results";
        btnResults.UseVisualStyleBackColor = true;
        btnResults.Click += btnResults_Click;
        // 
        // btnTechnician
        // 
        btnTechnician.Location = new Point(30, 290);
        btnTechnician.Name = "btnTechnician";
        btnTechnician.Size = new Size(380, 48);
        btnTechnician.TabIndex = 5;
        btnTechnician.Text = "Technician Login";
        btnTechnician.UseVisualStyleBackColor = true;
        btnTechnician.Click += btnTechnician_Click;
        // 
        // btnExit
        // 
        btnExit.Location = new Point(30, 370);
        btnExit.Name = "btnExit";
        btnExit.Size = new Size(380, 40);
        btnExit.TabIndex = 6;
        btnExit.Text = "Exit";
        btnExit.UseVisualStyleBackColor = true;
        btnExit.Click += btnExit_Click;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(440, 440);
        Controls.Add(btnExit);
        Controls.Add(btnTechnician);
        Controls.Add(btnResults);
        Controls.Add(btnRequests);
        Controls.Add(btnExaminees);
        Controls.Add(lblSubtitle);
        Controls.Add(lblTitle);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Lab Test Request and Results Management System";
        ResumeLayout(false);
        PerformLayout();
    }

    private Label lblTitle;
    private Label lblSubtitle;
    private Button btnExaminees;
    private Button btnRequests;
    private Button btnResults;
    private Button btnTechnician;
    private Button btnExit;
}