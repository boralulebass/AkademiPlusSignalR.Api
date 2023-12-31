﻿using AkademiPlusSignalR.Api.DAL;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AkademiPlusSignalR.Api.Hubs
{
    public class MyHub : Hub
    {
        public static List<string> Names   { get; set; } = new List<string>();
        public static int ClientCount { get; set; } = 0;
        public static int RoomCount { get; set; } = 5;
        private readonly Context _context;
        
        public MyHub( Context context)
        {
            _context = context;
        }
        //public async Task sendname(string name)
        //{
        //    Names.Add(name);
        //    await Clients.All.SendAsync("receivename",name);
        //}
        public async Task sendname(string name)
        {
            if (Names.Count > RoomCount) 
            {
                await Clients.Caller.SendAsync("error", $"Bu odada en fazla {RoomCount} kişi olabilir.");
            }
            Names.Add(name);
            await Clients.All.SendAsync("receivename", name);
        }

        public async Task GetNames()
        {
            await Clients.All.SendAsync("receiveNames", Names);
        }

        public async override Task OnConnectedAsync()
        {
           ClientCount++;
           await Clients.All.SendAsync("receiveClientCount",ClientCount);
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            ClientCount--;
            await Clients.All.SendAsync("receiveClientCount", ClientCount);
        }

        public async Task SendNameByGroup(string name, string roomName)
        {
            var room = _context.Rooms.Where(x=>x.RoomName == roomName).FirstOrDefault();
            if (room != null)
            {
                room.Users.Add(new User()
                {
                    UserName = name,
                });
            }
            else
            {
                var newRoom = new Room() { RoomName = roomName };
                newRoom.Users.Add(new User() { UserName = name });
                _context.Rooms.Add(newRoom);
            }
            await _context.SaveChangesAsync();
            await Clients.Group(roomName).SendAsync("receiveMessageByGroup", name, room.RoomID);
        }
        public async Task GetNameByGroup()
        {
            var rooms = _context.Rooms.Include(x => x.Users).Select(y => new
            {
                RoomID = y.RoomID,
                Users = y.Users.ToList()
            });
            await Clients.All.SendAsync("receiveNamesByGroup", rooms);
        }
        public async Task AddToGroup(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        }
        public async Task RemoveFromGroup(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }
    }
}
