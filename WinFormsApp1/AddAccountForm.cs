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
    public partial class AddAccountForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private ComboBox cmbRole;
        private Button btnSave;
        private Button btnCancel;

        public AddAccountForm()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            this.Text = "Add New Account";
            this.Size = new Size(350, 250);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Username
            Label lblUsername = new Label() { Text = "Username:", Location = new Point(20, 30), Size = new Size(80, 23) };
            txtUsername = new TextBox() { Location = new Point(110, 27), Size = new Size(200, 23) };

            // Password
            Label lblPassword = new Label() { Text = "Password:", Location = new Point(20, 70), Size = new Size(80, 23) };
            txtPassword = new TextBox() { Location = new Point(110, 67), Size = new Size(200, 23), UseSystemPasswordChar = true };

            // Role
            Label lblRole = new Label() { Text = "Role:", Location = new Point(20, 110), Size = new Size(80, 23) };
            cmbRole = new ComboBox() { Location = new Point(110, 107), Size = new Size(200, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRole.Items.AddRange(new string[] { "Cashier", "Manager" });

            // Buttons
            btnSave = new Button() { Text = "Save", Location = new Point(155, 160), Size = new Size(75, 30) };
            btnCancel = new Button() { Text = "Cancel", Location = new Point(235, 160), Size = new Size(75, 30) };

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.AddRange(new Control[] { lblUsername, txtUsername, lblPassword, txtPassword, lblRole, cmbRole, btnSave, btnCancel });
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text) ||
                cmbRole.SelectedItem == null)
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "INSERT INTO Users (Username, Password, Role) VALUES (@username, @password, @role)";
                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", txtUsername.Text.Trim());
                        command.Parameters.AddWithValue("@password", txtPassword.Text.Trim());
                        command.Parameters.AddWithValue("@role", cmbRole.SelectedItem.ToString());
                        command.ExecuteNonQuery();
                    }
                }
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding account: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

