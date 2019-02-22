using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortProxy.WinService.Controls
{
	using System.Drawing;
	using System.Windows.Forms;

	internal class HdapiButton : Button
	{
		private Image _image1X;

		public Image Image1X
		{
			get => _image1X;
			set
			{
				if (UiUtils.ScaleInt == 1)
					Image = value;
				_image1X = value;
			}
		}

		private Image _image2X;

		public Image Image2X
		{
			get => _image2X;
			set
			{
				if (UiUtils.ScaleInt == 2)
					Image = value;
				_image2X = value;
			}
		}
		
		private bool ShouldSerializeImage() => false;
	}
}
