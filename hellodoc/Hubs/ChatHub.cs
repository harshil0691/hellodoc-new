using hellodoc.Controllers;
using hellodoc.DbEntity.ViewModels;
using hellodoc.Repositories.Repository.Interface;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace hellodoc.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatRepo _chatRepo;

        public ChatHub(IChatRepo chatRepo)
        {
            _chatRepo = chatRepo;
        }

        //private readonly static ConcurrentDictionary<string, string> _userConnectionMap = new ConcurrentDictionary<string, string>();

        //public override async Task OnConnectedAsync()
        //{
        //    // Get the user ID from your authentication system
        //    var userId = Context.UserIdentifier;

        //    if (!string.IsNullOrEmpty(userId))
        //    {
        //        _userConnectionMap.TryAdd(userId, Context.ConnectionId);
        //    }

        //    await base.OnConnectedAsync();
        //}

        //public override async Task OnDisconnectedAsync(Exception exception)
        //{
        //    // Get the user ID from your authentication system
        //    var userId = Context.UserIdentifier;

        //    if (!string.IsNullOrEmpty(userId))
        //    {
        //        _userConnectionMap.TryRemove(userId, out _);
        //    }

        //    await base.OnDisconnectedAsync(exception);
        //}


        public async Task SendMessage(string user, string message,string SenderAspid,string ReciverAspid,string sentfrom,string requestid)
        {
            ChatModal chat = new ChatModal();
            chat.SenderAspId = int.Parse(SenderAspid);
            chat.ReciverAspId = int.Parse(ReciverAspid);
            chat.message = message;
            chat.sentFrom = sentfrom;
            chat.requestid = int.Parse(requestid);

            _chatRepo.SaveChats(chat);

            //if (_userConnectionMap.TryGetValue(SenderAspid, out string connectionId))
            //{
            //    await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
            //}

            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}