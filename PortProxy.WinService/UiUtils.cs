using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortProxy.WinService
{
	using System.Drawing;
	using System.Security.Principal;
	using System.Windows.Forms;

	static class UiUtils
	{
		private static float _dpiX;
		private static float _dpiY;
		private static float _scaleX;
		public static int ScaleInt { get; }

		private static float _scaleY;
		private static int _scaleIntY;

		static UiUtils()
		{
			using (var bmp = new Bitmap(1, 1))
			using (var g = Graphics.FromImage(bmp))
			{
				_dpiX = g.DpiX;
				_dpiY = g.DpiY;
				_scaleX = _dpiX / 96F;
				_scaleY = _dpiY / 96F;
				ScaleInt = _scaleX > 1.9F ? 2 : 1;
				_scaleIntY = _scaleY > 1.9F ? 2 : 1;
			}
		}

		/// <summary>
		/// 判断当前运行身份是否是管理员
		/// </summary>
		/// <returns></returns>
		public static bool IsAdmin() => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

		public static void SetImage(this Button button, Image img1x, Image image2x)
		{
			button.Image = ScaleInt == 2 ? image2x : img1x;
			button.TextImageRelation = TextImageRelation.ImageBeforeText;
			button.TextAlign = ContentAlignment.MiddleRight;
		}
	}
}
