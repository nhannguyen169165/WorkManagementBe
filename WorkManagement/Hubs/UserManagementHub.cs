﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
namespace WorkManagement.Hubs
{
    public class UserManagementHub : Hub
    {
        public async Task SendMessage(object data,int adminId)
        {
            await Clients.All.SendAsync("ReceiveMessage", data,adminId);
        }
        public async Task SendResultDeleteUser(int userId)
        {
            await Clients.All.SendAsync("ReceiveResultDeleteUser", userId);
        }
        public async Task SendRequestUserActived(int userId)
        {
            await Clients.All.SendAsync("ReceiveUserActived", userId);
        }
    }
}
