using System.ComponentModel;
using System.Windows;
using ManuelNaujoks.VSChat;

namespace WPFClient
{
	public partial class MainWindow : Window
	{
		protected override void OnClosing(CancelEventArgs e)
		{
			var chat = (MyControl)base.Content;
			chat.Closing();

			base.OnClosing(e);
		}
	}
}
