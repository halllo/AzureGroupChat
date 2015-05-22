using System.ComponentModel;
using System.Windows;
using ManuelNaujoks.VSChat;

namespace WPFClient
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			Loaded += (s, e) =>
			{
				chat.GetRelativeCodePosition = callback => callback(new RelativeCodePosition { SolutionFile = @"C:\s.sln", File = @"C:\test.txt", Line = 2 });
			};
		}
		protected override void OnClosing(CancelEventArgs e)
		{
			chat.Closing();

			base.OnClosing(e);
		}
	}
}
