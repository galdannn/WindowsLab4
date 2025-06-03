namespace WinFormsApp1
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            cartflowLayoutPanel = new FlowLayoutPanel();
            InfoPanel = new Panel();
            lblsubtotal = new Label();
            lblUPrice = new Label();
            lblItemName = new Label();
            lblQty = new Label();
            flowLayoutPanel2 = new FlowLayoutPanel();
            button1 = new Button();
            panel1 = new Panel();
            lblTotal = new Label();
            flowLayoutPanel3 = new FlowLayoutPanel();
            txtSearch = new TextBox();
            btnUser = new Button();
            label1 = new Label();
            lblTime = new Label();
            menuStrip = new MenuStrip();
            productsToolStripMenuItem = new ToolStripMenuItem();
            categoriesStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItemToolStripMenuItem = new ToolStripMenuItem();
            srchbtn = new Button();
            timer = new System.Windows.Forms.Timer(components);
            panel3 = new Panel();
            InfoPanel.SuspendLayout();
            panel1.SuspendLayout();
            menuStrip.SuspendLayout();
            panel3.SuspendLayout();
            SuspendLayout();
            // 
            // cartflowLayoutPanel
            // 
            cartflowLayoutPanel.AutoScroll = true;
            cartflowLayoutPanel.AutoSize = true;
            cartflowLayoutPanel.FlowDirection = FlowDirection.TopDown;
            cartflowLayoutPanel.Location = new Point(8, 134);
            cartflowLayoutPanel.Name = "cartflowLayoutPanel";
            cartflowLayoutPanel.Size = new Size(487, 339);
            cartflowLayoutPanel.TabIndex = 0;
            cartflowLayoutPanel.WrapContents = false;
            // 
            // InfoPanel
            // 
            InfoPanel.Controls.Add(lblsubtotal);
            InfoPanel.Controls.Add(lblUPrice);
            InfoPanel.Controls.Add(lblItemName);
            InfoPanel.Controls.Add(lblQty);
            InfoPanel.Location = new Point(8, 86);
            InfoPanel.Name = "InfoPanel";
            InfoPanel.Size = new Size(481, 46);
            InfoPanel.TabIndex = 0;
            // 
            // lblsubtotal
            // 
            lblsubtotal.AutoSize = true;
            lblsubtotal.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblsubtotal.Location = new Point(323, 11);
            lblsubtotal.Name = "lblsubtotal";
            lblsubtotal.Size = new Size(48, 21);
            lblsubtotal.TabIndex = 3;
            lblsubtotal.Text = "Total";
            // 
            // lblUPrice
            // 
            lblUPrice.AutoSize = true;
            lblUPrice.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblUPrice.Location = new Point(224, 11);
            lblUPrice.Name = "lblUPrice";
            lblUPrice.Size = new Size(67, 21);
            lblUPrice.TabIndex = 2;
            lblUPrice.Text = "U/Price";
            // 
            // lblItemName
            // 
            lblItemName.AutoSize = true;
            lblItemName.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblItemName.Location = new Point(6, 11);
            lblItemName.Name = "lblItemName";
            lblItemName.Size = new Size(95, 21);
            lblItemName.TabIndex = 0;
            lblItemName.Text = "Item Name";
            // 
            // lblQty
            // 
            lblQty.AutoSize = true;
            lblQty.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblQty.Location = new Point(128, 11);
            lblQty.Name = "lblQty";
            lblQty.Size = new Size(77, 21);
            lblQty.TabIndex = 1;
            lblQty.Text = "Quantity";
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.AutoScroll = true;
            flowLayoutPanel2.Location = new Point(497, 134);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new Size(487, 257);
            flowLayoutPanel2.TabIndex = 1;
            // 
            // button1
            // 
            button1.BackColor = Color.FromArgb(34, 197, 94);
            button1.Font = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.ForeColor = Color.White;
            button1.Location = new Point(3, 557);
            button1.Name = "button1";
            button1.Size = new Size(487, 106);
            button1.TabIndex = 2;
            button1.Text = "Pay";
            button1.UseVisualStyleBackColor = false;
            button1.Click += BtnPay_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(lblTotal);
            panel1.Location = new Point(3, 479);
            panel1.Name = "panel1";
            panel1.Size = new Size(487, 72);
            panel1.TabIndex = 3;
            // 
            // lblTotal
            // 
            lblTotal.Font = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotal.Location = new Point(0, 0);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(484, 72);
            lblTotal.TabIndex = 0;
            lblTotal.Text = "Total: $0.00";
            lblTotal.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // flowLayoutPanel3
            // 
            flowLayoutPanel3.AutoScroll = true;
            flowLayoutPanel3.Location = new Point(497, 397);
            flowLayoutPanel3.Name = "flowLayoutPanel3";
            flowLayoutPanel3.Size = new Size(487, 266);
            flowLayoutPanel3.TabIndex = 4;
            // 
            // txtSearch
            // 
            txtSearch.Location = new Point(497, 97);
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderText = "Search products";
            txtSearch.Size = new Size(428, 23);
            txtSearch.TabIndex = 5;
            txtSearch.KeyPress += TxtProductId_KeyPress;
            // 
            // btnUser
            // 
            btnUser.BackColor = Color.LightSkyBlue;
            btnUser.Location = new Point(863, 0);
            btnUser.Name = "btnUser";
            btnUser.Size = new Size(121, 37);
            btnUser.TabIndex = 6;
            btnUser.Text = "UserBtn";
            btnUser.UseVisualStyleBackColor = false;
            btnUser.Click += BtnUser_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(3, 5);
            label1.Name = "label1";
            label1.Size = new Size(145, 17);
            label1.TabIndex = 7;
            label1.Text = "RETAIL SUPERMARKET";
            // 
            // lblTime
            // 
            lblTime.AutoSize = true;
            lblTime.Location = new Point(12, 22);
            lblTime.Name = "lblTime";
            lblTime.Size = new Size(10, 15);
            lblTime.TabIndex = 8;
            lblTime.Text = " ";
            // 
            // menuStrip
            // 
            menuStrip.ImageScalingSize = new Size(20, 20);
            menuStrip.Items.AddRange(new ToolStripItem[] { productsToolStripMenuItem, categoriesStripMenuItem, helpToolStripMenuItemToolStripMenuItem });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(984, 24);
            menuStrip.TabIndex = 9;
            menuStrip.Text = "menuStrip";
            // 
            // productsToolStripMenuItem
            // 
            productsToolStripMenuItem.Name = "productsToolStripMenuItem";
            productsToolStripMenuItem.Size = new Size(66, 20);
            productsToolStripMenuItem.Text = "Products";
            productsToolStripMenuItem.Click += productsToolStripMenuItem_Click;
            // 
            // categoriesStripMenuItem
            // 
            categoriesStripMenuItem.Name = "categoriesStripMenuItem";
            categoriesStripMenuItem.Size = new Size(75, 20);
            categoriesStripMenuItem.Text = "Categories";
            categoriesStripMenuItem.Click += categoriesToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItemToolStripMenuItem
            // 
            helpToolStripMenuItemToolStripMenuItem.Name = "helpToolStripMenuItemToolStripMenuItem";
            helpToolStripMenuItemToolStripMenuItem.Size = new Size(44, 20);
            helpToolStripMenuItemToolStripMenuItem.Text = "Help";
            helpToolStripMenuItemToolStripMenuItem.Click += helpToolStripMenuItem_Click;
            // 
            // srchbtn
            // 
            srchbtn.Image = Properties.Resources.Ionic_Ionicons_Search_24;
            srchbtn.Location = new Point(932, 94);
            srchbtn.Name = "srchbtn";
            srchbtn.Size = new Size(35, 28);
            srchbtn.TabIndex = 10;
            srchbtn.UseVisualStyleBackColor = true;
            srchbtn.Click += btnSearch_Click;
            // 
            // timer
            // 
            timer.Enabled = true;
            timer.Interval = 1000;
            timer.Tick += timerClock_Tick;
            // 
            // panel3
            // 
            panel3.Controls.Add(label1);
            panel3.Controls.Add(lblTime);
            panel3.Controls.Add(btnUser);
            panel3.Location = new Point(0, 32);
            panel3.Name = "panel3";
            panel3.Size = new Size(984, 56);
            panel3.TabIndex = 11;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(984, 661);
            Controls.Add(InfoPanel);
            Controls.Add(srchbtn);
            Controls.Add(txtSearch);
            Controls.Add(panel3);
            Controls.Add(flowLayoutPanel3);
            Controls.Add(panel1);
            Controls.Add(button1);
            Controls.Add(flowLayoutPanel2);
            Controls.Add(cartflowLayoutPanel);
            Controls.Add(menuStrip);
            MainMenuStrip = menuStrip;
            Name = "MainForm";
            InfoPanel.ResumeLayout(false);
            InfoPanel.PerformLayout();
            panel1.ResumeLayout(false);
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private FlowLayoutPanel cartflowLayoutPanel;
        private FlowLayoutPanel flowLayoutPanel2;
        private Button button1;
        private Panel panel1;
        private FlowLayoutPanel flowLayoutPanel3;
        private TextBox txtSearch;
        private Button btnUser;
        private Label label1;
        private Label lblTime;
        private MenuStrip menuStrip;
        private ToolStripMenuItem productsToolStripMenuItem;
        private ToolStripMenuItem categoriesStripMenuItem;
        private Label lblTotal;
        private ToolStripMenuItem helpToolStripMenuItemToolStripMenuItem;
        private Button srchbtn;
        private Panel InfoPanel;
        private Label lblsubtotal;
        private Label lblUPrice;
        private Label lblItemName;
        private Label lblQty;
        private System.Windows.Forms.Timer timer;
        private Panel panel3;
    }
}