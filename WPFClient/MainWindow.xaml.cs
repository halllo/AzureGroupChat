using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.AspNet.SignalR.Client;

namespace WPFClient
{
	public partial class MainWindow : Window
	{
		public String GroupName { get; set; }
		public String UserName { get; set; }
		public IHubProxy HubProxy { get; set; }
		//const string ServerURI = "http://localhost:8080/signalr";
		//const string ServerURI = "http://localhost:57403/signalr";
		//const string ServerURI = "https://localhost:44300/signalr";
		const string ServerURI = "https://manuelnaujoksintegratedchat.azurewebsites.net/signalr";
		public HubConnection Connection { get; set; }

		public MainWindow()
		{
			InitializeComponent();
		}

		void ButtonSend_Click(object sender, RoutedEventArgs e)
		{
			HubProxy.Invoke("Send", UserName, GroupName, TextBoxMessage.Text);
			TextBoxMessage.Text = String.Empty;
			TextBoxMessage.Focus();
		}

		async Task ConnectAsync()
		{
			Connection = new HubConnection(ServerURI);
			Connection.Closed += Connection_Closed;
			HubProxy = Connection.CreateHubProxy("MyHub");
			HubProxy.On<string, string>("AddMessage", (userName, message) =>
				this.Dispatcher.Invoke(() =>
				{
					var postfix = new TextRange(RichTextBoxConsole.Document.ContentEnd, RichTextBoxConsole.Document.ContentEnd) { Text = userName + ": " };
					postfix.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Blue);

					var content = new TextRange(RichTextBoxConsole.Document.ContentEnd, RichTextBoxConsole.Document.ContentEnd) { Text = message + "\n" };
					content.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);
				})
			);

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

		void WPFClient_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (Connection != null)
			{
				Connection.Stop();
				Connection.Dispose();
			}
		}
	}
}
