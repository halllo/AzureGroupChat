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
				chat.GetRelativeCodePosition = callback => callback(new RelativeCodePosition(solutionFile: @"C:\s.sln", file: @"C:\test.txt", line: 2));
				chat.GoToFileAndLine = shortcut => MessageBox.Show("goto " + shortcut);
			};
		}
		protected override void OnClosing(CancelEventArgs e)
		{
			chat.Closing();

			base.OnClosing(e);
		}
	}
}
