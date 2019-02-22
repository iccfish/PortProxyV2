using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PortProxy.WinService
{
	using System.Diagnostics;
	using System.Security.Principal;
	using System.ServiceProcess;

	using ProxyServer;

	public partial class InstallService : Form
	{
		public InstallService()
		{
			InitializeComponent();

			radTypeClient.Click += (sender, args) => txtServiceName.Text = "portproxy-client";
			radTypeServer.Click += (sender, args) => txtServiceName.Text = "portproxy-server";
			if (!UiUtils.IsAdmin())
			{
				btnInstallService.Image1X = Properties.Resources.shield;
				btnInstallService.Image2X = Properties.Resources.shield_2x;
				btnDeleteService.Image1X = Properties.Resources.shield;
				btnDeleteService.Image2X = Properties.Resources.shield_2x;
			}
		}

		private async void BtnRunTest_Click(object sender, EventArgs e)
		{
			var cfg = BuildConfig();
			var error = cfg.CheckForConfigurationError();
			if (!error.IsNullOrEmpty())
			{
				MessageBox.Show(this, error, "配置错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			cfg.GuiMode = true;

			var psi = new ProcessStartInfo(System.Reflection.Assembly.GetEntryAssembly().Location, cfg.GetCmdLine());
			await StartProcessAsync(psi, cfg.LocalServer.Contains("*"));
		}

		private async void BtnInstallService_Click(object sender, EventArgs e)
		{
			var cfg = BuildConfig();
			var error = cfg.CheckForConfigurationError();
			if (!error.IsNullOrEmpty())
			{
				MessageBox.Show(this, error, "配置错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}
			if (txtServiceName.TextLength == 0)
			{
				MessageBox.Show(this, "请输入服务名", "配置错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}
			if (ServiceController.GetServices().Any(x => x.ServiceName == txtServiceName.Text))
			{
				MessageBox.Show(this, "同名服务已安装", "配置错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			cfg.ServiceMode = true;

			var binary = System.Reflection.Assembly.GetEntryAssembly().Location;
			var arguments = cfg.GetCmdLine();

			var psi = new ProcessStartInfo("sc.exe", $"create {txtServiceName.Text} type= own start= auto binPath= \"{binary} {arguments}\"");
			if (!await StartProcessAsync(psi, true))
				return;
			psi = new ProcessStartInfo("net.exe", $"start {txtServiceName.Text}");
			if (!await StartProcessAsync(psi, true))
				return;

			if (!IsServiceInstalled(txtServiceName.Text))
			{
				MessageBox.Show(this, "服务安装失败", "失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			else
			{
				MessageBox.Show(this, "服务安装成功", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		ServerConfig BuildConfig()
		{
			return new ServerConfig()
			{
				Local = radTypeClient.Checked,
				LocalServer = txtLocalStatPrefix.Text,
				Port = (int)localPort.Value,
				RemoteServer = txtUpServer.Text,
				RemoteServerPort = (int)upServerPort.Value
			};
		}

		private async void BtnDeleteService_Click(object sender, EventArgs e)
		{
			if (!IsServiceInstalled(txtServiceName.Text))
			{
				MessageBox.Show(this, $"未找到服务 “{txtServiceName.Text}”", "配置错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			var psi = new ProcessStartInfo("net.exe", $"stop {txtServiceName.Text}");
			if (!await StartProcessAsync(psi, true))
				return;
			psi = new ProcessStartInfo("sc.exe", $"delete {txtServiceName.Text}");
			if (!await StartProcessAsync(psi, true))
				return;

			if (!IsServiceInstalled(txtServiceName.Text))
			{
				MessageBox.Show(this, "服务已删除", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			else
			{
				MessageBox.Show(this, "服务删除失败", "失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		bool IsServiceInstalled(string serviceName) => ServiceController.GetServices().Any(x => x.ServiceName == serviceName);

		async Task<bool> StartProcessAsync(ProcessStartInfo psi, bool needAdmin)
		{
			if (needAdmin && !UiUtils.IsAdmin())
			{
				psi.Verb = "runas";
			}

			var error = await Task<string>.Factory.StartNew(() =>
			  {
				  try
				  {
					  var p = Process.Start(psi);
					  p.WaitForExit();
					  return null;
				  }
				  catch (Win32Exception ex) when (ex.NativeErrorCode == 1223)
				  {
					  return string.Empty;
				  }
				  catch (Exception exception)
				  {
					  return exception.Message;
				  }
			  });

			if (!error.IsNullOrEmpty())
			{
				MessageBox.Show(this, error, "错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}

			return error == null;
		}
	}
}
