using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace WorkManagement.Hubs
{
    public class TaskManagementHub : Hub
    {
        public async Task SendRequesChangeStatusTask(int projectId)
        {
            await Clients.All.SendAsync("ReceiveRequesChangeStatusTask", projectId);
        }
        public async Task SendRequestNewTaskData(int projectId)
        {
            await Clients.All.SendAsync("ReceiveRequestNewTaskData", projectId);
        }
    }

}
