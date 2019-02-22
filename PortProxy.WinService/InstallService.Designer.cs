namespace PortProxy.WinService
{
	using Controls;

	partial class InstallService
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
			this.label1 = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.txtServiceName = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.txtLocalStatPrefix = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.upServerPort = new System.Windows.Forms.NumericUpDown();
			this.label4 = new System.Windows.Forms.Label();
			this.localPort = new System.Windows.Forms.NumericUpDown();
			this.panel2 = new System.Windows.Forms.Panel();
			this.radTypeServer = new System.Windows.Forms.RadioButton();
			this.radTypeClient = new System.Windows.Forms.RadioButton();
			this.txtUpServer = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.btnRunTest = new PortProxy.WinService.Controls.HdapiButton();
			this.btnDeleteService = new PortProxy.WinService.Controls.HdapiButton();
			this.btnInstallService = new PortProxy.WinService.Controls.HdapiButton();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.upServerPort)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.localPort)).BeginInit();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(0, 290);
			this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(278, 41);
			this.label1.TabIndex = 1;
			this.label1.Text = "上级服务器地址";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.txtServiceName);
			this.panel1.Controls.Add(this.label6);
			this.panel1.Controls.Add(this.txtLocalStatPrefix);
			this.panel1.Controls.Add(this.label5);
			this.panel1.Controls.Add(this.upServerPort);
			this.panel1.Controls.Add(this.label4);
			this.panel1.Controls.Add(this.localPort);
			this.panel1.Controls.Add(this.panel2);
			this.panel1.Controls.Add(this.txtUpServer);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.label3);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.btnRunTest);
			this.panel1.Controls.Add(this.btnDeleteService);
			this.panel1.Controls.Add(this.btnInstallService);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Margin = new System.Windows.Forms.Padding(6);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1066, 653);
			this.panel1.TabIndex = 0;
			// 
			// txtServiceName
			// 
			this.txtServiceName.Location = new System.Drawing.Point(287, 136);
			this.txtServiceName.Name = "txtServiceName";
			this.txtServiceName.Size = new System.Drawing.Size(699, 50);
			this.txtServiceName.TabIndex = 1;
			this.txtServiceName.Text = "portproxy-client";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(0, 139);
			this.label6.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(278, 41);
			this.label6.TabIndex = 8;
			this.label6.Text = "服务名";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtLocalStatPrefix
			// 
			this.txtLocalStatPrefix.Location = new System.Drawing.Point(287, 439);
			this.txtLocalStatPrefix.Name = "txtLocalStatPrefix";
			this.txtLocalStatPrefix.Size = new System.Drawing.Size(699, 50);
			this.txtLocalStatPrefix.TabIndex = 5;
			this.txtLocalStatPrefix.Text = "127.0.0.1:7788";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(0, 442);
			this.label5.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(278, 41);
			this.label5.TabIndex = 6;
			this.label5.Text = "状态统计前缀";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// upServerPort
			// 
			this.upServerPort.Location = new System.Drawing.Point(287, 360);
			this.upServerPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
			this.upServerPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.upServerPort.Name = "upServerPort";
			this.upServerPort.Size = new System.Drawing.Size(163, 50);
			this.upServerPort.TabIndex = 4;
			this.upServerPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.upServerPort.Value = new decimal(new int[] {
            8888,
            0,
            0,
            0});
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(0, 365);
			this.label4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(278, 41);
			this.label4.TabIndex = 5;
			this.label4.Text = "上级服务器端口";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// localPort
			// 
			this.localPort.Location = new System.Drawing.Point(287, 215);
			this.localPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
			this.localPort.Minimum = new decimal(new int[] {
            1025,
            0,
            0,
            0});
			this.localPort.Name = "localPort";
			this.localPort.Size = new System.Drawing.Size(163, 50);
			this.localPort.TabIndex = 2;
			this.localPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.localPort.Value = new decimal(new int[] {
            8888,
            0,
            0,
            0});
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.radTypeServer);
			this.panel2.Controls.Add(this.radTypeClient);
			this.panel2.Location = new System.Drawing.Point(287, 31);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(699, 81);
			this.panel2.TabIndex = 0;
			// 
			// radTypeServer
			// 
			this.radTypeServer.AutoSize = true;
			this.radTypeServer.Location = new System.Drawing.Point(197, 14);
			this.radTypeServer.Name = "radTypeServer";
			this.radTypeServer.Size = new System.Drawing.Size(177, 45);
			this.radTypeServer.TabIndex = 1;
			this.radTypeServer.Text = "服务器端";
			this.radTypeServer.UseVisualStyleBackColor = true;
			// 
			// radTypeClient
			// 
			this.radTypeClient.AutoSize = true;
			this.radTypeClient.Checked = true;
			this.radTypeClient.Location = new System.Drawing.Point(13, 16);
			this.radTypeClient.Name = "radTypeClient";
			this.radTypeClient.Size = new System.Drawing.Size(145, 45);
			this.radTypeClient.TabIndex = 0;
			this.radTypeClient.TabStop = true;
			this.radTypeClient.Text = "客户端";
			this.radTypeClient.UseVisualStyleBackColor = true;
			// 
			// txtUpServer
			// 
			this.txtUpServer.Location = new System.Drawing.Point(287, 287);
			this.txtUpServer.Name = "txtUpServer";
			this.txtUpServer.Size = new System.Drawing.Size(699, 50);
			this.txtUpServer.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(0, 51);
			this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(278, 41);
			this.label2.TabIndex = 1;
			this.label2.Text = "安装类型";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(0, 220);
			this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(278, 41);
			this.label3.TabIndex = 1;
			this.label3.Text = "本地端口";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// btnRunTest
			// 
			this.btnRunTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnRunTest.Image1X = null;
			this.btnRunTest.Image2X = null;
			this.btnRunTest.Location = new System.Drawing.Point(43, 540);
			this.btnRunTest.Margin = new System.Windows.Forms.Padding(6);
			this.btnRunTest.Name = "btnRunTest";
			this.btnRunTest.Size = new System.Drawing.Size(299, 70);
			this.btnRunTest.TabIndex = 6;
			this.btnRunTest.Text = "直接运行测试";
			this.btnRunTest.UseVisualStyleBackColor = true;
			this.btnRunTest.Click += new System.EventHandler(this.BtnRunTest_Click);
			// 
			// btnDeleteService
			// 
			this.btnDeleteService.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnDeleteService.Image1X = null;
			this.btnDeleteService.Image2X = null;
			this.btnDeleteService.Location = new System.Drawing.Point(687, 540);
			this.btnDeleteService.Margin = new System.Windows.Forms.Padding(6);
			this.btnDeleteService.Name = "btnDeleteService";
			this.btnDeleteService.Size = new System.Drawing.Size(299, 70);
			this.btnDeleteService.TabIndex = 7;
			this.btnDeleteService.Text = "删除系统服务";
			this.btnDeleteService.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.btnDeleteService.UseVisualStyleBackColor = true;
			this.btnDeleteService.Click += new System.EventHandler(this.BtnDeleteService_Click);
			// 
			// btnInstallService
			// 
			this.btnInstallService.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnInstallService.Image1X = null;
			this.btnInstallService.Image2X = null;
			this.btnInstallService.Location = new System.Drawing.Point(365, 540);
			this.btnInstallService.Margin = new System.Windows.Forms.Padding(6);
			this.btnInstallService.Name = "btnInstallService";
			this.btnInstallService.Size = new System.Drawing.Size(299, 70);
			this.btnInstallService.TabIndex = 7;
			this.btnInstallService.Text = "安装系统服务";
			this.btnInstallService.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.btnInstallService.UseVisualStyleBackColor = true;
			this.btnInstallService.Click += new System.EventHandler(this.BtnInstallService_Click);
			// 
			// InstallService
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1066, 653);
			this.Controls.Add(this.panel1);
			this.Margin = new System.Windows.Forms.Padding(6);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "InstallService";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "PortProxy部署";
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.upServerPort)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.localPort)).EndInit();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private HdapiButton btnInstallService;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.NumericUpDown upServerPort;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown localPort;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.RadioButton radTypeServer;
		private System.Windows.Forms.RadioButton radTypeClient;
		private System.Windows.Forms.TextBox txtUpServer;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private HdapiButton btnRunTest;
		private System.Windows.Forms.TextBox txtLocalStatPrefix;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox txtServiceName;
		private System.Windows.Forms.Label label6;
		private HdapiButton btnDeleteService;
	}
}