using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;

namespace WinFormsApp1
{
    public partial class UpdateEmployeeForm : Form
    {
        private ComboBox cmbEmployee;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private ComboBox cmbRole;
        private Button btnUpdate;
        private Button btnCancel;
        private User selectedUser;

        public UpdateEmployeeForm()
        {
            InitializeComponent();
            SetupForm();
            LoadEmployees();
        }

        private void SetupForm()
        {
            this.Text = "Update Employee Information";
            this.Size = new Size(400, 280);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Employee selection
            Label lblEmployee = new Label() { Text = "Select Employee:", Location = new Point(20, 30), Size = new Size(100, 23) };
            cmbEmployee = new ComboBox() { Location = new Point(130, 27), Size = new Size(220, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbEmployee.SelectedIndexChanged += CmbEmployee_SelectedIndexChanged;

            // Username
            Label lblUsername = new Label() { Text = "Username:", Location = new Point(20, 70), Size = new Size(80, 23) };
            txtUsername = new TextBox() { Location = new Point(130, 67), Size = new Size(220, 23) };

            // Password
            Label lblPassword = new Label() { Text = "New Password:", Location = new Point(20, 110), Size = new Size(100, 23) };
            txtPassword = new TextBox() { Location = new Point(130, 107), Size = new Size(220, 23), UseSystemPasswordChar = true };

            // Role
            Label lblRole = new Label() { Text = "Role:", Location = new Point(20, 150), Size = new Size(80, 23) };
            cmbRole = new ComboBox() { Location = new Point(130, 147), Size = new Size(220, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRole.Items.AddRange(new string[] { "Cashier", "Manager" });

            // Buttons
            btnUpdate = new Button() { Text = "Update", Location = new Point(195, 200), Size = new Size(75, 30) };
            btnCancel = new Button() { Text = "Cancel", Location = new Point(275, 200), Size = new Size(75, 30) };

            btnUpdate.Click += BtnUpdate_Click;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.AddRange(new Control[] { lblEmployee, cmbEmployee, lblUsername, txtUsername, lblPassword, txtPassword, lblRole, cmbRole, btnUpdate, btnCancel });
        }

        private void LoadEmployees()
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT Id, Username, Role FROM Users ORDER BY Username";
                    using (var command = new SqliteCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var user = new User
                                {
                                    Id = reader.GetInt32("Id"),
                                    Username = reader.GetString("Username"),
                                    Role = reader.GetString("Role")
                                };
                                cmbEmployee.Items.Add(user);
                            }
                        }
                    }
                }
                cmbEmployee.DisplayMember = "Username";
                cmbEmployee.ValueMember = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading employees: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CmbEmployee_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbEmployee.SelectedItem != null)
            {
                selectedUser = (User)cmbEmployee.SelectedItem;
                txtUsername.Text = selectedUser.Username;
                cmbRole.SelectedItem = selectedUser.Role;
                txtPassword.Text = ""; // Don't show existing password
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedUser == null || string.IsNullOrWhiteSpace(txtUsername.Text) || cmbRole.SelectedItem == null)
            {
                MessageBox.Show("Please select an employee and fill in required fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "UPDATE Users SET Username = @username, Role = @role";

                    // Only update password if a new one is provided
                    if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                    {
                        query += ", Password = @password";
                    }

                    query += " WHERE Id = @id";

                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", txtUsername.Text.Trim());
                        command.Parameters.AddWithValue("@role", cmbRole.SelectedItem.ToString());
                        command.Parameters.AddWithValue("@id", selectedUser.Id);

                        if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                        {
                            command.Parameters.AddWithValue("@password", txtPassword.Text.Trim());
                        }

                        command.ExecuteNonQuery();
                    }
                }
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating employee: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
