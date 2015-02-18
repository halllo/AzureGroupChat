using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace ASPNETServer
{
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
			return base.OnConnected();
		}
		public override Task OnDisconnected(bool stopCalled)
		{
			return base.OnDisconnected(stopCalled);
		}
	}
}