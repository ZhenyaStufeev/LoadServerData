using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR
{
    public class SignalRHub : Hub
    {
        public SignalRHub() : base()
        { }
        public async Task SendDubugMessage(string message)
        {
            await Clients.All.SendAsync(message);
        }
        public async override Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
    }
}
