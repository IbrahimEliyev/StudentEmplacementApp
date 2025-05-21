using Microsoft.AspNetCore.SignalR;

namespace StudentEmplacementApp
{
    public class ChatHub : Hub
    {
       public override async Task OnConnectedAsync()
        {
           await Clients.All.SendAsync("RecieveMessage", $"{Context.ConnectionId} has joined to chat");
        }
    }
}
