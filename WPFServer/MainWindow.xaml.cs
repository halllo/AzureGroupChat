using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;

namespace WPFServer
{
	public partial class MainWindow : Window
	{
		public IDisposable SignalR { get; set; }
		const string ServerURI = "http://localhost:8080";

		public MainWindow()
		{
			InitializeComponent();
		}

		private void ButtonStart_Click(object sender, RoutedEventArgs e)
		{
			WriteToConsole("Starting server...");
			ButtonStart.IsEnabled = false;
			Task.Run(() => StartServer());
		}

		private void ButtonStop_Click(object sender, RoutedEventArgs e)
		{
			SignalR.Dispose();
			Close();
		}

		private void StartServer()
		{
			try
			{
				SignalR = WebApp.Start(ServerURI);
			}
			catch (TargetInvocationException)
			{
				WriteToConsole("A server is already running at " + ServerURI);
				this.Dispatcher.Invoke(() => ButtonStart.IsEnabled = true);
				return;
			}
			this.Dispatcher.Invoke(() => ButtonStop.IsEnabled = true);
			WriteToConsole("Server started at " + ServerURI);
		}

		public void WriteToConsole(String message)
		{
			if (!(RichTextBoxConsole.CheckAccess()))
			{
				this.Dispatcher.Invoke(() =>
					WriteToConsole(message)
				);
				return;
			}
			RichTextBoxConsole.AppendText(message + "\r");
		}
	}






	class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			app.UseCors(CorsOptions.AllowAll);
			app.MapSignalR();
		}
	}

	public class MyHub : Hub
	{
		public void Send(string userName, string roomName, string message)
		{
			Clients.Group(roomName).addMessage(userName, message);
		}
		public async Task JoinRoom(string userName, string roomName)
		{
			await Groups.Add(Context.ConnectionId, roomName);
			Clients.Group(roomName).addMessage("", userName + " joined.");
		}
		public Task LeaveRoom(string roomName)
		{
			return Groups.Remove(Context.ConnectionId, roomName);
		}
		public override Task OnConnected()
		{
			Application.Current.Dispatcher.Invoke(() =>
				((MainWindow)Application.Current.MainWindow).WriteToConsole("Client connected: " + Context.ConnectionId));

			return base.OnConnected();
		}
		public override Task OnDisconnected()
		{
			Application.Current.Dispatcher.Invoke(() =>
				((MainWindow)Application.Current.MainWindow).WriteToConsole("Client disconnected: " + Context.ConnectionId));

			return base.OnDisconnected();
		}
	}
}
