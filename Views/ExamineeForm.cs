
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using LabTestSystem.Controllers;
using LabTestSystem.Models;

namespace LabTestSystem.Views
{
    public partial class ExamineeForm : Form
    {
        private readonly ExamineeController _controller;
        private int _selectedExamineeID = 0;

        private Label lblTitle;
        private Label lblExamineeNo;
        private TextBox txtExamineeNo;
        private Label lblFullName;
        private TextBox txtFullName;
        private Button btnSave;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnClear;
        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnRefresh;
        private Button btnClose;
        private DataGridView dgvExaminees;

        public ExamineeForm()
        {
            InitializeComponent();

            var repository = new ExamineeRepository(
                AppConfig.ConnectionString
            );

            _controller = new ExamineeController(repository);
        }

        private void ExamineeForm_Load(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private Examinee ReadExamineeFromForm()
        {
            return new Examinee
            {
                ExamineeID = _selectedExamineeID,
                ExamineeNo = txtExamineeNo.Text.Trim(),
                FullName = txtFullName.Text.Trim()
            };
        }

        // Refresh the examinees table
        private void RefreshGrid()
        {
            try
            {
                dgvExaminees.DataSource = _controller.GetExaminees();
                dgvExaminees.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Database error: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Save examinee
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtExamineeNo.Text))
            {
                MessageBox.Show("Enter the examinee number.");
                txtExamineeNo.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Enter the full name.");
                txtFullName.Focus();
                return;
            }

            try
            {
                _controller.AddExaminee(ReadExamineeFromForm());

                MessageBox.Show("Examinee saved successfully.");

                RefreshGrid();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Save",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        // Select examinee from table
        private void dgvExaminees_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = dgvExaminees.Rows[e.RowIndex];

            _selectedExamineeID = Convert.ToInt32(
                row.Cells["ExamineeID"].Value
            );

            txtExamineeNo.Text = Convert.ToString(
                row.Cells["ExamineeNo"].Value
            );

            txtFullName.Text = Convert.ToString(
                row.Cells["FullName"].Value
            );
        }

        // Update examinee
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (_selectedExamineeID <= 0)
            {
                MessageBox.Show("Select an examinee first.");
                return;
            }

            try
            {
                _controller.EditExaminee(ReadExamineeFromForm());

                MessageBox.Show("Examinee updated successfully.");

                RefreshGrid();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Update",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        // Delete examinee
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedExamineeID <= 0)
            {
                MessageBox.Show("Select an examinee first.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this examinee?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result != DialogResult.Yes)
                return;

            try
            {
                _controller.RemoveExaminee(_selectedExamineeID);

                MessageBox.Show("Examinee deleted successfully.");

                RefreshGrid();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Delete",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Search examinees
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                dgvExaminees.DataSource =
                    _controller.SearchExaminees(txtSearch.Text.Trim());

                dgvExaminees.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Database error: " + ex.Message,
                    "Search",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Refresh button
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            RefreshGrid();
        }

        // Clear button
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            _selectedExamineeID = 0;

            txtExamineeNo.Clear();
            txtFullName.Clear();

            dgvExaminees.ClearSelection();

            txtExamineeNo.Focus();
        }

        // Close button
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        // Initialize form controls
        private void InitializeComponent()
        {
            lblTitle = new Label();
            lblExamineeNo = new Label();
            txtExamineeNo = new TextBox();
            lblFullName = new Label();
            txtFullName = new TextBox();

            btnSave = new Button();
            btnUpdate = new Button();
            btnDelete = new Button();
            btnClear = new Button();

            txtSearch = new TextBox();
            btnSearch = new Button();
            btnRefresh = new Button();
            btnClose = new Button();

            dgvExaminees = new DataGridView();

            ((System.ComponentModel.ISupportInitialize)dgvExaminees).BeginInit();

            SuspendLayout();

            // Title
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font(
                "Segoe UI",
                18F,
                FontStyle.Bold,
                GraphicsUnit.Point
            );
            lblTitle.ForeColor = Color.FromArgb(15, 118, 110);
            lblTitle.Location = new Point(20, 10);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(230, 41);
            lblTitle.Text = "Manage Examinees";

            // Examinee number label
            lblExamineeNo.AutoSize = true;
            lblExamineeNo.Location = new Point(20, 68);
            lblExamineeNo.Name = "lblExamineeNo";
            lblExamineeNo.Size = new Size(131, 20);
            lblExamineeNo.Text = "Examinee Number";

            // Examinee number textbox
            txtExamineeNo.Location = new Point(170, 65);
            txtExamineeNo.Name = "txtExamineeNo";
            txtExamineeNo.Size = new Size(220, 27);

            // Full name label
            lblFullName.AutoSize = true;
            lblFullName.Location = new Point(20, 108);
            lblFullName.Name = "lblFullName";
            lblFullName.Size = new Size(76, 20);
            lblFullName.Text = "Full Name";

            // Full name textbox
            txtFullName.Location = new Point(170, 105);
            txtFullName.Name = "txtFullName";
            txtFullName.Size = new Size(320, 27);

            // Save button
            btnSave.Location = new Point(20, 150);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(110, 36);
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;

            // Update button
            btnUpdate.Location = new Point(140, 150);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(110, 36);
            btnUpdate.Text = "Update";
            btnUpdate.UseVisualStyleBackColor = true;
            btnUpdate.Click += btnUpdate_Click;

            // Delete button
            btnDelete.Location = new Point(260, 150);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(110, 36);
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;

            // Clear button
            btnClear.Location = new Point(380, 150);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(110, 36);
            btnClear.Text = "Clear";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;

            // Search textbox
            txtSearch.Location = new Point(20, 210);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(300, 27);

            // Search button
            btnSearch.Location = new Point(330, 207);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(90, 32);
            btnSearch.Text = "Search";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click;

            // Refresh button
            btnRefresh.Location = new Point(430, 207);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(90, 32);
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;

            // Close button (top-right, like TechnicianForm Logout)
            btnClose.Location = new Point(580, 14);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(100, 34);
            btnClose.TabIndex = 13;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;

            // Examinees DataGridView
            dgvExaminees.AllowUserToAddRows = false;
            dgvExaminees.AllowUserToDeleteRows = false;
            dgvExaminees.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dgvExaminees.BackgroundColor = Color.White;
            dgvExaminees.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvExaminees.Location = new Point(20, 252);
            dgvExaminees.MultiSelect = false;
            dgvExaminees.Name = "dgvExaminees";
            dgvExaminees.ReadOnly = true;
            dgvExaminees.RowHeadersVisible = false;
            dgvExaminees.RowHeadersWidth = 51;
            dgvExaminees.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvExaminees.Size = new Size(660, 290);
            dgvExaminees.CellClick += dgvExaminees_CellClick;

            // Form settings
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 560);

            Controls.Add(btnClose);
            Controls.Add(dgvExaminees);
            Controls.Add(btnRefresh);
            Controls.Add(btnSearch);
            Controls.Add(txtSearch);
            Controls.Add(btnClear);
            Controls.Add(btnDelete);
            Controls.Add(btnUpdate);
            Controls.Add(btnSave);
            Controls.Add(txtFullName);
            Controls.Add(lblFullName);
            Controls.Add(txtExamineeNo);
            Controls.Add(lblExamineeNo);
            Controls.Add(lblTitle);

            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "ExamineeForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Manage Examinees";

            Load += ExamineeForm_Load;

            ((System.ComponentModel.ISupportInitialize)dgvExaminees).EndInit();

            ResumeLayout(false);
            PerformLayout();
        }
    }
}
