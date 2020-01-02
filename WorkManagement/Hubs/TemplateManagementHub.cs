
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace WorkManagement.Hubs
{
    public class TemplateManagementHub : Hub
    {
        public async Task SendMessage(object data, int adminId, int uid)
        {
            await Clients.All.SendAsync("ReceiveMessage", data, adminId, uid);
        }

        public async Task SendRequesChangeStatusTemplate(int templateId)
        {
            await Clients.All.SendAsync("ReceiveRequesChangeStatusTemplate",templateId);
        }
    }
}

