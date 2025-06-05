using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class PaymentForm : Form
    {
        private decimal totalAmount;
        private RadioButton rbCash = new RadioButton();
        private RadioButton rbCard = new RadioButton();
        private TextBox txtCashReceived = new TextBox();
        private Label lblChange = new Label();
        private Button btnProcess = new Button();

       
        public decimal AmountPaid { get; private set; }
        public decimal ChangeGiven { get; private set; }

        public PaymentForm(decimal total)
        {
            totalAmount = total;
            InitializeCustomComponent();
           
        }

        private void InitializeCustomComponent()
        {
            this.Size = new Size(400, 300);
            this.Text = "Payment Processing";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false; 
            this.AcceptButton = btnProcess; 

            Label lblTotal = new Label
            {
                Text = $"Total Amount: {totalAmount:C}",
                Location = new Point(50, 30),
                AutoSize = true, 
                Font = new Font("Arial", 12, FontStyle.Bold)
            };

            GroupBox gbPaymentMethod = new GroupBox
            {
                Text = "Payment Method",
                Location = new Point(50, 70),
                Size = new Size(300, 70) 
            };

            rbCash = new RadioButton
            {
                Text = "Cash",
                Location = new Point(20, 30),
                Size = new Size(80, 23),
                Checked = true
            };
            rbCash.CheckedChanged += PaymentMethod_CheckedChanged;

            rbCard = new RadioButton
            {
                Text = "Card",
                Location = new Point(120, 30),
                Size = new Size(80, 23)
            };
            rbCard.CheckedChanged += PaymentMethod_CheckedChanged;

            gbPaymentMethod.Controls.AddRange(new Control[] { rbCash, rbCard });

            Label lblCashReceivedTitle = new Label
            {
                Text = "Cash Received:",
                Location = new Point(50, 155),
                Size = new Size(100, 23)
            };

            txtCashReceived = new TextBox
            {
                Location = new Point(160, 155), 
                Size = new Size(100, 23)
            };
            txtCashReceived.TextChanged += TxtCashReceived_TextChanged;
            txtCashReceived.KeyPress += TxtCashReceived_KeyPress;

            lblChange = new Label
            {
                Text = "Change: " + (0m).ToString("C"), 
                Location = new Point(50, 190),
                AutoSize = true, 
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            btnProcess = new Button
            {
                Text = "Process Payment",
                Location = new Point(this.ClientSize.Width / 2 - 60, 220), 
                Size = new Size(120, 30),
                BackColor = Color.LightGreen,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            btnProcess.Click += BtnProcess_Click;

            this.Controls.AddRange(new Control[] { lblTotal, gbPaymentMethod, lblCashReceivedTitle, txtCashReceived, lblChange, btnProcess });

            
            PaymentMethod_CheckedChanged(null, EventArgs.Empty);
        }

        private void PaymentMethod_CheckedChanged(object? sender, EventArgs e)
        {
            bool isCash = rbCash.Checked;
            txtCashReceived.Enabled = isCash;
            lblChange.Visible = isCash;
            Label? cashReceivedTextLabel = this.Controls 
                                   .OfType<Label>()
                                   .FirstOrDefault(l => l.Text == "Cash Received:");
            if (cashReceivedTextLabel != null)
            {
                cashReceivedTextLabel.Visible = isCash;
            }


            if (!isCash)
            {
                txtCashReceived.Text = string.Empty; 
                lblChange.Text = "Change: " + (0m).ToString("C");
            }
            else
            {
                TxtCashReceived_TextChanged(null, EventArgs.Empty); 
            }
        }

        private void TxtCashReceived_TextChanged(object? sender, EventArgs e)
        {
            if (rbCash.Checked)
            {
                if (decimal.TryParse(txtCashReceived.Text, NumberStyles.Currency, CultureInfo.CurrentCulture, out decimal cashReceived) ||
                    decimal.TryParse(txtCashReceived.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out cashReceived))
                {
                    decimal change = cashReceived - totalAmount;
                    lblChange.Text = $"Change: {Math.Max(0, change):C}";
                }
                else if (string.IsNullOrWhiteSpace(txtCashReceived.Text))
                {
                    lblChange.Text = "Change: " + (0m).ToString("C");
                }
                else
                {
                    lblChange.Text = "Change: Invalid"; 
                }
            }
        }

        private void TxtCashReceived_KeyPress(object? sender, KeyPressEventArgs e)
        {
            
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            
            if ((e.KeyChar == '.') && ((sender as TextBox)?.Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void BtnProcess_Click(object? sender, EventArgs e)
        {
            if (rbCash.Checked)
            {
                if (!decimal.TryParse(txtCashReceived.Text, NumberStyles.Currency, CultureInfo.CurrentCulture, out decimal cashReceived) &&
                    !decimal.TryParse(txtCashReceived.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out cashReceived))
                {
                    MessageBox.Show("Please enter a valid cash amount.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cashReceived < totalAmount)
                {
                    MessageBox.Show("Insufficient cash amount received.", "Payment Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                AmountPaid = cashReceived;
                ChangeGiven = cashReceived - totalAmount;

               
                string receiptMsg = $"RECEIPT (Cash)\n\nTotal: {totalAmount:C}\nCash Received: {AmountPaid:C}\nChange: {ChangeGiven:C}\n\nThank you for your purchase!";
                MessageBox.Show(receiptMsg, "Transaction Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else // Card
            {
                AmountPaid = totalAmount;
                ChangeGiven = 0;

               
                string receiptMsg = $"RECEIPT (Card)\n\nTotal: {totalAmount:C}\nPayment Method: Card\nAmount Charged: {AmountPaid:C}\n\nThank you for your purchase!";
                MessageBox.Show(receiptMsg, "Transaction Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}