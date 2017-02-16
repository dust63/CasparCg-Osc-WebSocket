namespace OscWebSocketTester
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;




        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.VersionLabel = new System.Windows.Forms.ToolStripMenuItem();
            this.ipTextBox = new InfluenceTheme.InfluenceControls.InfluenceIpBox();
            this.influenceTheme1 = new InfluenceTheme.InfluenceControls.InfluenceTheme();
            this.portTextBox = new InfluenceTheme.InfluenceControls.InfluenceTextBoxValidation();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnValidate = new InfluenceTheme.InfluenceControls.InfluenceButton();
            this.urlTextBox = new InfluenceTheme.InfluenceControls.InfluenceTextBoxValidation();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnCancel = new InfluenceTheme.InfluenceControls.InfluenceButton();
            this.contextMenuStrip1.SuspendLayout();
            this.influenceTheme1.SuspendLayout();
            this.SuspendLayout();
            // 
            // trayIcon
            // 
            this.trayIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.trayIcon.BalloonTipText = "Started";
            this.trayIcon.BalloonTipTitle = "Caspar Osc WebHub";
            this.trayIcon.ContextMenuStrip = this.contextMenuStrip1;
            this.trayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("trayIcon.Icon")));
            this.trayIcon.Text = "Caspar Osc WebHub";
            this.trayIcon.Visible = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem,
            this.configurationToolStripMenuItem,
            this.VersionLabel});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(149, 70);
            // 
            // VersionLabel
            // 
            this.VersionLabel.Enabled = false;
            this.VersionLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VersionLabel.Name = "VersionLabel";
            this.VersionLabel.Size = new System.Drawing.Size(148, 22);
            this.VersionLabel.Text = "Version";
            // 
            // ipTextBox
            // 
            this.ipTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.ipTextBox.ForeColor = System.Drawing.Color.White;
            this.ipTextBox.InvalidIcon = null;
            this.ipTextBox.Location = new System.Drawing.Point(12, 105);
            this.ipTextBox.MaxLength = 32767;
            this.ipTextBox.MultiLine = false;
            this.ipTextBox.Name = "ipTextBox";
            this.ipTextBox.ReadOnly = false;
            this.ipTextBox.Size = new System.Drawing.Size(237, 25);
            this.ipTextBox.TabIndex = 1;
            this.ipTextBox.TextAlignment = System.Windows.Forms.HorizontalAlignment.Left;
            this.ipTextBox.UseSystemPasswordChar = false;
            this.ipTextBox.ValidIcon = global::OscWebSocketTester.Properties.Resources.success;
            this.ipTextBox.ValidValue = false;
            this.ipTextBox.TextChanged += new System.EventHandler(this.ipTextBox_TextChanged);
            // 
            // influenceTheme1
            // 
            this.influenceTheme1.CloseButtonExitsApp = false;
            this.influenceTheme1.Controls.Add(this.btnCancel);
            this.influenceTheme1.Controls.Add(this.btnValidate);
            this.influenceTheme1.Controls.Add(this.label3);
            this.influenceTheme1.Controls.Add(this.label2);
            this.influenceTheme1.Controls.Add(this.label1);
            this.influenceTheme1.Controls.Add(this.portTextBox);
            this.influenceTheme1.Controls.Add(this.urlTextBox);
            this.influenceTheme1.Controls.Add(this.ipTextBox);
            this.influenceTheme1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.influenceTheme1.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.influenceTheme1.Image = null;
            this.influenceTheme1.Location = new System.Drawing.Point(0, 0);
            this.influenceTheme1.MaximumSize = new System.Drawing.Size(390, 189);
            this.influenceTheme1.MinimizeButton = false;
            this.influenceTheme1.Name = "influenceTheme1";
            this.influenceTheme1.Size = new System.Drawing.Size(390, 189);
            this.influenceTheme1.TabIndex = 2;
            this.influenceTheme1.Text = "Caspar Osc WebHub";
            // 
            // portTextBox
            // 
            this.portTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.portTextBox.ForeColor = System.Drawing.Color.White;
            this.portTextBox.InvalidIcon = null;
            this.portTextBox.Location = new System.Drawing.Point(255, 105);
            this.portTextBox.MaxLength = 32767;
            this.portTextBox.MultiLine = false;
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.ReadOnly = false;
            this.portTextBox.Size = new System.Drawing.Size(123, 25);
            this.portTextBox.TabIndex = 2;
            this.portTextBox.TextAlignment = System.Windows.Forms.HorizontalAlignment.Left;
            this.portTextBox.UseSystemPasswordChar = false;
            this.portTextBox.ValidIcon = global::OscWebSocketTester.Properties.Resources.success;
            this.portTextBox.ValidValue = false;
            this.portTextBox.TextChanged += new System.EventHandler(this.urlTextBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.Silver;
            this.label1.Location = new System.Drawing.Point(12, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Web Socket Server Url:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.Color.Silver;
            this.label2.Location = new System.Drawing.Point(12, 89);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "CasparCG Osc Ip:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.ForeColor = System.Drawing.Color.Silver;
            this.label3.Location = new System.Drawing.Point(255, 89);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(123, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "CasparCG Osc Port:";
            // 
            // btnValidate
            // 
            this.btnValidate.BackColor = System.Drawing.Color.Transparent;
            this.btnValidate.Font = new System.Drawing.Font("Verdana", 9F);
            this.btnValidate.ForeColor = System.Drawing.Color.White;
            this.btnValidate.Location = new System.Drawing.Point(76, 149);
            this.btnValidate.Name = "btnValidate";
            this.btnValidate.Selected = false;
            this.btnValidate.SelectedColorLine = System.Drawing.Color.Empty;
            this.btnValidate.SelectedLineHeight = 0;
            this.btnValidate.Size = new System.Drawing.Size(115, 30);
            this.btnValidate.TabIndex = 4;
            this.btnValidate.Text = "Ok";
            this.btnValidate.TextAlignment = System.Drawing.StringAlignment.Center;
            this.btnValidate.Click += new System.EventHandler(this.btnValidate_Click);
            // 
            // urlTextBox
            // 
            this.urlTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.urlTextBox.ForeColor = System.Drawing.Color.White;
            this.urlTextBox.InvalidIcon = null;
            this.urlTextBox.Location = new System.Drawing.Point(12, 55);
            this.urlTextBox.MaxLength = 32767;
            this.urlTextBox.MultiLine = false;
            this.urlTextBox.Name = "urlTextBox";
            this.urlTextBox.ReadOnly = false;
            this.urlTextBox.Size = new System.Drawing.Size(366, 25);
            this.urlTextBox.TabIndex = 2;
            this.urlTextBox.TextAlignment = System.Windows.Forms.HorizontalAlignment.Left;
            this.urlTextBox.UseSystemPasswordChar = false;
            this.urlTextBox.ValidIcon = global::OscWebSocketTester.Properties.Resources.success;
            this.urlTextBox.ValidValue = false;
            this.urlTextBox.TextChanged += new System.EventHandler(this.urlTextBox_TextChanged);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Image = global::OscWebSocketTester.Properties.Resources.exit;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // configurationToolStripMenuItem
            // 
            this.configurationToolStripMenuItem.Image = global::OscWebSocketTester.Properties.Resources.settings_16px;
            this.configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
            this.configurationToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.configurationToolStripMenuItem.Text = "Configuration";
            this.configurationToolStripMenuItem.Click += new System.EventHandler(this.configurationToolStripMenuItem_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.Font = new System.Drawing.Font("Verdana", 9F);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(198, 149);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Selected = false;
            this.btnCancel.SelectedColorLine = System.Drawing.Color.Empty;
            this.btnCancel.SelectedLineHeight = 0;
            this.btnCancel.Size = new System.Drawing.Size(115, 30);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextAlignment = System.Drawing.StringAlignment.Center;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(390, 189);
            this.Controls.Add(this.influenceTheme1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form1";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Caspar Osc WebHub";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.contextMenuStrip1.ResumeLayout(false);
            this.influenceTheme1.ResumeLayout(false);
            this.influenceTheme1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon trayIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem VersionLabel;
        private System.Windows.Forms.ToolStripMenuItem configurationToolStripMenuItem;
        private InfluenceTheme.InfluenceControls.InfluenceIpBox ipTextBox;
        private InfluenceTheme.InfluenceControls.InfluenceTheme influenceTheme1;
        private InfluenceTheme.InfluenceControls.InfluenceButton btnValidate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private InfluenceTheme.InfluenceControls.InfluenceTextBoxValidation portTextBox;
        private InfluenceTheme.InfluenceControls.InfluenceTextBoxValidation urlTextBox;
        private InfluenceTheme.InfluenceControls.InfluenceButton btnCancel;

    }
}

