using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
namespace WorkManagement.Hubs
{
    public class ProjectManagementHub : Hub
    {
        public async Task SendMessage(object data,int adminId,int uid)
        {
            await Clients.All.SendAsync("ReceiveMessage", data,adminId,uid);
        }

        public async Task SendChangeMemberData(object data,int adminId, int projectId)
        {
            await Clients.All.SendAsync("ReceiveChangeMemberData", data, adminId, projectId);
        }
    }
}
