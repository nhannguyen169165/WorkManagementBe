using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
namespace WorkManagement.Hubs
{
    public class ProjectManagementHub : Hub
    {
        public async Task SendMessage(object data)
        {
            await Clients.All.SendAsync("ReceiveMessage", data);
        }
    }
}
