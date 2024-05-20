using hellodoc.Controllers;
using hellodoc.DbEntity.ViewModels;
using hellodoc.Repositories.Repository.Interface;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using hellodoc.DbEntity.DataModels;
using System;
using Org.BouncyCastle.Ocsp;


namespace hellodoc.Hubs
{
    public class ConnectionManager
    {
        private readonly ConcurrentDictionary<int?, string> _userConnections = new ConcurrentDictionary<int?, string>();
        private readonly ConcurrentDictionary<int?, string> _requestConnections = new ConcurrentDictionary<int?, string>();

        public void AddConnection(int? userId,int? requestid, string connectionId)
        {
            if (_userConnections.GetValueOrDefault(userId) == null)
            {
                _userConnections.TryAdd(userId, connectionId);
            }
            else
            {
                _userConnections.TryRemove(userId, out _);
                _userConnections.TryAdd(userId, connectionId);
            }

            if (_requestConnections.GetValueOrDefault(requestid) == null)
            {
                _userConnections.TryAdd(requestid, connectionId);
            }
            else
            {
                _userConnections.TryRemove(requestid, out _);
                _userConnections.TryAdd(requestid, connectionId);
            }
        }

        public void RemoveConnection(int? userId)
        {
            _userConnections.TryRemove(userId, out _);
        }

        public string GetConnectionId(int? userId)
        {
            _userConnections.TryGetValue(userId, out var connectionId);
            return connectionId;
        }
    }


    public class ChatHub : Hub
    {
        private readonly IChatRepo _chatRepo;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ConnectionManager _connectionManager;

        public ChatHub(IChatRepo chatRepo, IHttpContextAccessor httpContextAccessor,ConnectionManager connectionManager)
        {
            _chatRepo = chatRepo;
            _contextAccessor = httpContextAccessor;
            _connectionManager = connectionManager;
        }
        public override Task OnConnectedAsync()
        {
            var AspId = _contextAccessor.HttpContext?.Session.GetInt32("Aspid");
            var RequestId = _contextAccessor.HttpContext?.Session.GetInt32("RequestId");
            _connectionManager.AddConnection(AspId , RequestId, Context.ConnectionId);
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }


        public async Task SendMessage(string user, string message,string SenderAspid,string ReciverAspid,string sentfrom,string requestid)
        {
            ChatModal chat = new ChatModal();
            chat.SenderAspId = int.Parse(SenderAspid);
            chat.ReciverAspId = int.Parse(ReciverAspid);
            chat.message = message;
            chat.sentFrom = sentfrom;
            chat.requestid = int.Parse(requestid);

            _chatRepo.SaveChats(chat);

            string reciverConnectionId = _connectionManager.GetConnectionId(int.Parse(ReciverAspid));

            if (reciverConnectionId != null)
            {
                await Clients.Client(reciverConnectionId).SendAsync("ReceiveMessage", message, "reciver");
            }
            
        }
    }
}

