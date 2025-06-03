namespace WinFormsApp1
{
    partial class CartItemControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CartItemControl));
            nameLabel = new Label();
            priceLabel = new Label();
            totalLabel = new Label();
            qtyPanel = new Panel();
            plusBtn = new Button();
            qtyLabel = new Label();
            minusBtn = new Button();
            delBtn = new Button();
            qtyPanel.SuspendLayout();
            SuspendLayout();
            // 
            // nameLabel
            // 
            nameLabel.AutoEllipsis = true;
            nameLabel.Location = new Point(6, 13);
            nameLabel.Name = "nameLabel";
            nameLabel.Size = new Size(95, 15);
            nameLabel.TabIndex = 0;
            nameLabel.Text = "Name";
            nameLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // priceLabel
            // 
            priceLabel.AutoEllipsis = true;
            priceLabel.Location = new Point(221, 13);
            priceLabel.Name = "priceLabel";
            priceLabel.Size = new Size(92, 15);
            priceLabel.TabIndex = 1;
            priceLabel.Text = "Price";
            priceLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // totalLabel
            // 
            totalLabel.Location = new Point(319, 13);
            totalLabel.Name = "totalLabel";
            totalLabel.Size = new Size(100, 15);
            totalLabel.TabIndex = 2;
            totalLabel.Text = "Total";
            totalLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // qtyPanel
            // 
            qtyPanel.BackColor = SystemColors.ActiveBorder;
            qtyPanel.Controls.Add(plusBtn);
            qtyPanel.Controls.Add(qtyLabel);
            qtyPanel.Controls.Add(minusBtn);
            qtyPanel.Location = new Point(107, 3);
            qtyPanel.Margin = new Padding(4, 3, 4, 3);
            qtyPanel.Name = "qtyPanel";
            qtyPanel.Size = new Size(104, 46);
            qtyPanel.TabIndex = 3;
            // 
            // plusBtn
            // 
            plusBtn.Location = new Point(69, 6);
            plusBtn.Margin = new Padding(4, 3, 4, 3);
            plusBtn.Name = "plusBtn";
            plusBtn.Size = new Size(31, 33);
            plusBtn.TabIndex = 2;
            plusBtn.Tag = "item";
            plusBtn.Text = "+";
            plusBtn.UseVisualStyleBackColor = true;
            plusBtn.Click += plusBtn_Click;
            // 
            // qtyLabel
            // 
            qtyLabel.AutoSize = true;
            qtyLabel.Location = new Point(39, 12);
            qtyLabel.Margin = new Padding(4, 0, 4, 0);
            qtyLabel.Name = "qtyLabel";
            qtyLabel.Size = new Size(26, 15);
            qtyLabel.TabIndex = 1;
            qtyLabel.Text = "Qty";
            qtyLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // minusBtn
            // 
            minusBtn.Location = new Point(4, 6);
            minusBtn.Margin = new Padding(4, 3, 4, 3);
            minusBtn.Name = "minusBtn";
            minusBtn.Size = new Size(31, 33);
            minusBtn.TabIndex = 0;
            minusBtn.Tag = "item";
            minusBtn.Text = "-";
            minusBtn.UseVisualStyleBackColor = true;
            minusBtn.Click += minusBtn_Click;
            // 
            // delBtn
            // 
            delBtn.BackColor = Color.WhiteSmoke;
            delBtn.BackgroundImage = (Image)resources.GetObject("delBtn.BackgroundImage");
            delBtn.BackgroundImageLayout = ImageLayout.Stretch;
            delBtn.Location = new Point(427, 10);
            delBtn.Margin = new Padding(4, 3, 4, 3);
            delBtn.Name = "delBtn";
            delBtn.Size = new Size(38, 33);
            delBtn.TabIndex = 4;
            delBtn.Tag = "item";
            delBtn.UseVisualStyleBackColor = false;
            delBtn.Click += delBtn_Click;
            // 
            // CartItemControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(delBtn);
            Controls.Add(qtyPanel);
            Controls.Add(totalLabel);
            Controls.Add(priceLabel);
            Controls.Add(nameLabel);
            Margin = new Padding(4, 3, 4, 3);
            Name = "CartItemControl";
            Size = new Size(471, 54);
            qtyPanel.ResumeLayout(false);
            qtyPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Label priceLabel;
        private System.Windows.Forms.Label totalLabel;
        private System.Windows.Forms.Panel qtyPanel;
        private System.Windows.Forms.Button plusBtn;
        private System.Windows.Forms.Label qtyLabel;
        private System.Windows.Forms.Button minusBtn;
        private System.Windows.Forms.Button delBtn;
    }
}
