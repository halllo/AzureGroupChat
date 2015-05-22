using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using Microsoft.AspNet.SignalR.Client;

namespace ManuelNaujoks.VSChat
{
	public partial class MyControl
	{
		public MyControl()
		{
			InitializeComponent();
		}











		public String GroupName { get; set; }
		public String UserName { get; set; }
		public IHubProxy HubProxy { get; set; }
		public HubConnection Connection { get; set; }
		//const string ServerURI = "http://localhost:8080/signalr";
		//const string ServerURI = "http://localhost:57403/signalr";
		//const string ServerURI = "https://localhost:44300/signalr";
		const string ServerURI = "https://manuelnaujoksintegratedchat.azurewebsites.net/signalr";

		void ButtonSend_Click(object sender, RoutedEventArgs e)
		{
			HubProxy.Invoke("Send", UserName, GroupName, TextBoxMessage.Text);
			TextBoxMessage.Text = String.Empty;
			TextBoxMessage.Focus();
		}

		public Action<Action<RelativeCodePosition>> GetRelativeCodePosition { get; set; }
		void ButtonBase_SendShortcut(object sender, RoutedEventArgs e)
		{
			if (GetRelativeCodePosition != null) GetRelativeCodePosition(p =>
			{
				var selectionStart = TextBoxMessage.SelectionStart;
				var shortcut = " @" + p.Shortcut + " ";
				TextBoxMessage.Text = TextBoxMessage.Text.Insert(selectionStart, shortcut);
				TextBoxMessage.SelectionStart = selectionStart + shortcut.Length;
				TextBoxMessage.Focus();
			});
		}

		async Task ConnectAsync()
		{
			Connection = new HubConnection(ServerURI);
			Connection.Closed += Connection_Closed;
			HubProxy = Connection.CreateHubProxy("MyHub");
			HubProxy.On<string, string>("AddMessage", (userName, message) =>
			{
				var inlines = message.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).SelectMany(s =>
				{
					if (Regex.IsMatch(s, @"^@.*?:\d*$"))
					{
						var link = new Hyperlink();
						link.Tag = s;
						link.Inlines.Add(s);
						link.Click += LinkClicked;
						return new Inline[]
						{
							new Run(" "),
							link
						};
					}
					else
					{
						return new Inline[]
						{
							new Run(" "),
							new Run(s) {Foreground = System.Windows.Media.Brushes.Black}
						};
					}
				}).Skip(1);

				Dispatcher.Invoke(() =>
				{
					var p = new Paragraph();
					p.Inlines.Add(new Run(userName + ": ") { Foreground = System.Windows.Media.Brushes.DarkGray });
					p.Inlines.AddRange(inlines);
					RichTextBoxConsole.Document.Blocks.Add(p);
					RichTextBoxConsole.ScrollToEnd();
				});
			});

			try
			{
				await Connection.Start();
			}
			catch (HttpRequestException)
			{
				StatusText.Content = "Unable to connect to server: Start server before connecting clients.";
				return;
			}

			SignInPanel.Visibility = Visibility.Collapsed;
			ChatPanel.Visibility = Visibility.Visible;
			ButtonSend.IsEnabled = true;
			TextBoxMessage.Focus();
			RichTextBoxConsole.AppendText("Connected to server at " + ServerURI + "\r");
		}

		void LinkClicked(object sender, RoutedEventArgs e)
		{
			var link = ((Hyperlink)sender).Tag.ToString();
			MessageBox.Show("clicked " + link, "TODO: jump");
		}

		void Connection_Closed()
		{
			var dispatcher = Application.Current.Dispatcher;
			dispatcher.Invoke(() => ChatPanel.Visibility = Visibility.Collapsed);
			dispatcher.Invoke(() => ButtonSend.IsEnabled = false);
			dispatcher.Invoke(() => StatusText.Content = "You have been disconnected.");
			dispatcher.Invoke(() => SignInPanel.Visibility = Visibility.Visible);
		}

		async void SignInButton_Click(object sender, RoutedEventArgs e)
		{
			UserName = UserNameTextBox.Text;
			GroupName = GroupNameTextBox.Text;
			if (!String.IsNullOrEmpty(GroupName) && !String.IsNullOrEmpty(UserName))
			{
				StatusText.Visibility = Visibility.Visible;
				StatusText.Content = "Connecting to server...";
				await ConnectAsync();
				HubProxy.Invoke("JoinRoom", UserName, GroupName);
			}
		}

		public void Closing()
		{
			if (Connection != null)
			{
				Connection.Stop();
				Connection.Dispose();
			}
		}
	}
}