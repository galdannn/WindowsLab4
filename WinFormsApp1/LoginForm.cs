using System;
using System.Data;

using Microsoft.Data.Sqlite;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class LoginForm : Form
    {

        public LoginForm()
        {
            InitializeComponent();
            SetAcceptButton();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            User? user = AuthenticateUser(username, password);
            if (user != null)
            {
                this.Hide();
                MainForm mainForm = new MainForm(user);
                mainForm.FormClosed += (s, args) => this.Close();
                mainForm.Show();
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private User? AuthenticateUser(string username, string password)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "SELECT Id, Username, Role FROM Users WHERE Username = @username AND Password = @password";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = reader.GetInt32("Id"),
                                Username = reader.GetString("Username"),
                                Role = reader.GetString("Role")
                            };
                        }
                    }
                }
            }
            return null;
        }
        private void SetAcceptButton()
        {
            this.AcceptButton = btnLogin;
        }
    }
    }

