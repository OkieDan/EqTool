using EQTool.Dto;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace EQTool.SignalR.Hubs
{
    public class MapHub : Hub
    {
        public async Task SendPlayerLocation(PlayerLocation playerLocation)
        {
            await Clients.All.SendAsync("ReceivePlayerLocation", playerLocation);
            Console.WriteLine($"{playerLocation} > {playerLocation.ServerName}, {playerLocation.PlayerName}, {playerLocation.MapName}, {playerLocation.X}, {playerLocation.Y}, {playerLocation.Z}");
        }
    }
}
