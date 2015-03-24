using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace ManuelNaujoks.VSChat
{
	[Guid("173cbcde-e728-442c-82ee-1c29ae3e00af")]
	public class MyToolWindow : ToolWindowPane
	{
		public MyToolWindow()
			: base(null)
		{
			this.Caption = Resources.ToolWindowTitle;
			this.BitmapResourceID = 301;
			this.BitmapIndex = 1;

			base.Content = new MyControl();
		}
	}
}
