using System;
using System.Drawing;
using System.Windows.Forms;
using LabTestSystem.Controllers;

namespace LabTestSystem.Views;

public partial class TechnicianLoginForm : Form
{
    private readonly TechnicianController _controller = AppServices.Technicians;

    public TechnicianLoginForm()
    {
        InitializeComponent();
    }

    private void btnLogin_Click(object sender, EventArgs e)
    {
        try
        {
            _controller.Login(txtUsername.Text, txtPassword.Text);
            DialogResult = DialogResult.OK;   // closes this form and tells MainForm it worked
        }
        catch (Exception ex)
        {
            Dialogs.Error(ex);
            txtPassword.Clear();
            txtPassword.Focus();
        }
    }

    private void InitializeComponent()
    {
        lblTitle = new Label();
        lblUsername = new Label();
        txtUsername = new TextBox();
        lblPassword = new Label();
        txtPassword = new TextBox();
        btnLogin = new Button();
        btnCancel = new Button();
        SuspendLayout();
        // 
        // lblTitle
        // 
        lblTitle.AutoSize = true;
        lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
        lblTitle.ForeColor = Color.FromArgb(15, 118, 110);
        lblTitle.Location = new Point(25, 15);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(230, 41);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "Technician Login";
        // 
        // lblUsername
        // 
        lblUsername.AutoSize = true;
        lblUsername.Location = new Point(25, 75);
        lblUsername.Name = "lblUsername";
        lblUsername.Size = new Size(75, 20);
        lblUsername.TabIndex = 1;
        lblUsername.Text = "Username";
        // 
        // txtUsername
        // 
        txtUsername.Location = new Point(25, 98);
        txtUsername.Name = "txtUsername";
        txtUsername.Size = new Size(350, 27);
        txtUsername.TabIndex = 2;
        // 
        // lblPassword
        // 
        lblPassword.AutoSize = true;
        lblPassword.Location = new Point(25, 140);
        lblPassword.Name = "lblPassword";
        lblPassword.Size = new Size(70, 20);
        lblPassword.TabIndex = 3;
        lblPassword.Text = "Password";
        // 
        // txtPassword
        // 
        txtPassword.Location = new Point(25, 163);
        txtPassword.Name = "txtPassword";
        txtPassword.Size = new Size(350, 27);
        txtPassword.TabIndex = 4;
        txtPassword.UseSystemPasswordChar = true;
        // 
        // btnLogin
        // 
        btnLogin.Location = new Point(25, 215);
        btnLogin.Name = "btnLogin";
        btnLogin.Size = new Size(350, 42);
        btnLogin.TabIndex = 5;
        btnLogin.Text = "Login";
        btnLogin.UseVisualStyleBackColor = true;
        btnLogin.Click += btnLogin_Click;
        // 
        // btnCancel
        // 
        btnCancel.DialogResult = DialogResult.Cancel;
        btnCancel.Location = new Point(25, 267);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(350, 36);
        btnCancel.TabIndex = 6;
        btnCancel.Text = "Cancel";
        btnCancel.UseVisualStyleBackColor = true;
        // 
        // TechnicianLoginForm
        // 
        AcceptButton = btnLogin;
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(400, 320);
        Controls.Add(btnCancel);
        Controls.Add(btnLogin);
        Controls.Add(txtPassword);
        Controls.Add(lblPassword);
        Controls.Add(txtUsername);
        Controls.Add(lblUsername);
        Controls.Add(lblTitle);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        Name = "TechnicianLoginForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Technician Login";
        ResumeLayout(false);
        PerformLayout();
    }

    private Label lblTitle;
    private Label lblUsername;
    private TextBox txtUsername;
    private Label lblPassword;
    private TextBox txtPassword;
    private Button btnLogin;
    private Button btnCancel;
}