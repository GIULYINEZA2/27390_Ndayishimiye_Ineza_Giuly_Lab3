
using LabTestSystem.Controllers;
using LabTestSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LabTestSystem.Views
{
    public partial class RequestForm : Form
    {
        private readonly RequestController _controller = AppServices.Requests;

        private Label lblTitle;
        private Label lblExamineeNo;
        private TextBox txtExamineeNo;
        private Label lblFullName;
        private TextBox txtName;
        private Label lblTests;
        private CheckedListBox clbTests;
        private Button btnSubmitRequest;
        private Button btnClear;
        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnRefresh;
        private Button btnDelete;
        private DataGridView dgvRequests;
        private Button btnClose;

        public RequestForm()
        {
            InitializeComponent();
        }

        private void RequestForm_Load(object sender, EventArgs e)
        {
            clbTests.Items.Clear();

            foreach (string test in RequestController.TestNames)
            {
                clbTests.Items.Add(test);
            }

            RefreshGrid();
        }

        // Display requests in the DataGridView
        private void ShowTable(DataTable table)
        {
            dgvRequests.DataSource = table;

            if (dgvRequests.Columns["SectionID"] != null)
            {
                dgvRequests.Columns["SectionID"].Visible = false;
            }

            dgvRequests.ClearSelection();
        }

        // Reload all requests
        private void RefreshGrid()
        {
            try
            {
                ShowTable(_controller.GetRequests());
            }
            catch (Exception ex)
            {
                Dialogs.Error(ex);
            }
        }

        // Automatically find an examinee when leaving the number textbox
        private void txtExamineeNo_Leave(object sender, EventArgs e)
        {
            try
            {
                Examinee found = _controller.FindExaminee(
                    txtExamineeNo.Text.Trim()
                );

                if (found != null)
                {
                    txtName.Text = found.FullName;
                    txtName.ReadOnly = true;
                }
                else
                {
                    txtName.Clear();
                    txtName.ReadOnly = false;
                }
            }
            catch (Exception ex)
            {
                Dialogs.Error(ex);
            }
        }

        // Submit a new test request
        private void btnSubmitRequest_Click(object sender, EventArgs e)
        {
            List<string> tests = new List<string>();

            foreach (object item in clbTests.CheckedItems)
            {
                tests.Add(item.ToString());
            }

            if (string.IsNullOrWhiteSpace(txtExamineeNo.Text))
            {
                Dialogs.Warn("Please enter the examinee number.");
                txtExamineeNo.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                Dialogs.Warn("Please enter the examinee's full name.");
                txtName.Focus();
                return;
            }

            if (tests.Count == 0)
            {
                Dialogs.Warn("Please select at least one test.");
                return;
            }

            try
            {
                List<string> created = _controller.SubmitRequest(
                    txtExamineeNo.Text.Trim(),
                    txtName.Text.Trim(),
                    tests
                );

                StringBuilder message = new StringBuilder(
                    "Request submitted successfully. Status: REQUESTED"
                );

                message.AppendLine();
                message.AppendLine();

                foreach (string line in created)
                {
                    message.AppendLine(line);
                }

                Dialogs.Info(message.ToString());

                ClearForm();
                RefreshGrid();
            }
            catch (Exception ex)
            {
                Dialogs.Error(ex);
            }
        }

        // Clear all input fields
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            txtExamineeNo.Clear();
            txtName.Clear();

            txtName.ReadOnly = false;

            for (int i = 0; i < clbTests.Items.Count; i++)
            {
                clbTests.SetItemChecked(i, false);
            }

            txtExamineeNo.Focus();
        }

        // Search requests
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ShowTable(
                    _controller.SearchRequests(txtSearch.Text.Trim())
                );
            }
            catch (Exception ex)
            {
                Dialogs.Error(ex);
            }
        }

        // Refresh the requests table
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            RefreshGrid();
        }

        // Delete a selected request
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvRequests.CurrentRow == null)
            {
                Dialogs.Warn("Please select a request from the table first.");
                return;
            }

            if (!dgvRequests.Columns.Contains("RequestID"))
            {
                Dialogs.Warn("RequestID column was not found in the table.");
                return;
            }

            object requestValue =
                dgvRequests.CurrentRow.Cells["RequestID"].Value;

            if (requestValue == null || requestValue == DBNull.Value)
            {
                Dialogs.Warn("The selected request has an invalid ID.");
                return;
            }

            int requestID = Convert.ToInt32(requestValue);

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this request?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result != DialogResult.Yes)
            {
                return;
            }

            try
            {
                _controller.RemoveRequest(requestID);

                Dialogs.Info("Request deleted successfully.");

                RefreshGrid();
            }
            catch (Exception ex)
            {
                Dialogs.Error(ex);
            }
        }

        // Close the form
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        // Initialize the form and its controls
        private void InitializeComponent()
        {
            lblTitle = new Label();
            lblExamineeNo = new Label();
            txtExamineeNo = new TextBox();
            lblFullName = new Label();
            txtName = new TextBox();
            lblTests = new Label();
            clbTests = new CheckedListBox();
            btnSubmitRequest = new Button();
            btnClear = new Button();
            txtSearch = new TextBox();
            btnSearch = new Button();
            btnRefresh = new Button();
            btnDelete = new Button();
            dgvRequests = new DataGridView();
            btnClose = new Button();

            ((System.ComponentModel.ISupportInitialize)dgvRequests).BeginInit();

            SuspendLayout();

            // Form title
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
            lblTitle.Size = new Size(501, 41);
            lblTitle.Text = "Submit and Manage Test Requests";

            // Examinee number label
            lblExamineeNo.AutoSize = true;
            lblExamineeNo.Location = new Point(20, 68);
            lblExamineeNo.Name = "lblExamineeNo";
            lblExamineeNo.Size = new Size(131, 20);
            lblExamineeNo.Text = "Examinee Number";

            // Examinee number textbox
            txtExamineeNo.Location = new Point(170, 65);
            txtExamineeNo.Name = "txtExamineeNo";
            txtExamineeNo.Size = new Size(200, 27);
            txtExamineeNo.Leave += txtExamineeNo_Leave;

            // Full name label
            lblFullName.AutoSize = true;
            lblFullName.Location = new Point(20, 108);
            lblFullName.Name = "lblFullName";
            lblFullName.Size = new Size(76, 20);
            lblFullName.Text = "Full Name";

            // Full name textbox
            txtName.Location = new Point(170, 105);
            txtName.Name = "txtName";
            txtName.Size = new Size(300, 27);

            // Tests label
            lblTests.AutoSize = true;
            lblTests.Location = new Point(500, 68);
            lblTests.Name = "lblTests";
            lblTests.Size = new Size(170, 20);
            lblTests.Text = "Tests (tick one or more)";

            // Checked tests list
            clbTests.CheckOnClick = true;
            clbTests.FormattingEnabled = true;
            clbTests.Location = new Point(500, 94);
            clbTests.Name = "clbTests";
            clbTests.Size = new Size(380, 84);

            // Submit button
            btnSubmitRequest.Location = new Point(20, 148);
            btnSubmitRequest.Name = "btnSubmitRequest";
            btnSubmitRequest.Size = new Size(200, 36);
            btnSubmitRequest.Text = "Submit Request";
            btnSubmitRequest.UseVisualStyleBackColor = true;
            btnSubmitRequest.Click += btnSubmitRequest_Click;

            // Clear button
            btnClear.Location = new Point(230, 148);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(100, 36);
            btnClear.Text = "Clear";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;

            // Search textbox
            txtSearch.Location = new Point(20, 205);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(260, 27);

            // Search button
            btnSearch.Location = new Point(290, 202);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(90, 30);
            btnSearch.Text = "Search";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click;

            // Refresh button
            btnRefresh.Location = new Point(390, 202);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(90, 30);
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;

            // Delete button
            btnDelete.Location = new Point(740, 202);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(140, 30);
            btnDelete.Text = "Delete Request";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;

            // Requests DataGridView
            dgvRequests.AllowUserToAddRows = false;
            dgvRequests.AllowUserToDeleteRows = false;
            dgvRequests.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dgvRequests.BackgroundColor = Color.White;
            dgvRequests.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRequests.Location = new Point(20, 242);
            dgvRequests.MultiSelect = false;
            dgvRequests.Name = "dgvRequests";
            dgvRequests.ReadOnly = true;
            dgvRequests.RowHeadersVisible = false;
            dgvRequests.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvRequests.Size = new Size(860, 340);

            // Close button
            btnClose.Location = new Point(780, 595);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(100, 32);
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;

            // RequestForm settings
            AcceptButton = btnSubmitRequest;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(900, 640);

            Controls.Add(btnClose);
            Controls.Add(dgvRequests);
            Controls.Add(btnDelete);
            Controls.Add(btnRefresh);
            Controls.Add(btnSearch);
            Controls.Add(txtSearch);
            Controls.Add(btnClear);
            Controls.Add(btnSubmitRequest);
            Controls.Add(clbTests);
            Controls.Add(lblTests);
            Controls.Add(txtName);
            Controls.Add(lblFullName);
            Controls.Add(txtExamineeNo);
            Controls.Add(lblExamineeNo);
            Controls.Add(lblTitle);

            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "RequestForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Test Requests";

            Load += RequestForm_Load;

            ((System.ComponentModel.ISupportInitialize)dgvRequests).EndInit();

            ResumeLayout(false);
            PerformLayout();
        }
    }
}
