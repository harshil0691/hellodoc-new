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
        private readonly ConcurrentDictionary<int?, List<string>> _requestConnections = new ConcurrentDictionary<int?, List<string>>();

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
                List<string> list = new List<string>();
                list.Add(connectionId);
                _requestConnections.TryAdd(requestid, list);
            }
            else
            {
                List<string> list = new List<string>();
                list = _requestConnections.GetValueOrDefault(requestid);
                list.Add(connectionId);
                _requestConnections.TryRemove(requestid,out _);
                _requestConnections.TryAdd(requestid, list);
            }
        }

        public void RemoveConnection(int? userId,int? requestId)
        {
            _userConnections.TryRemove(userId, out _);
            _requestConnections.TryRemove(requestId, out _);
        }

        public string GetConnectionId(int? userId,int? requestId)
        {
            _userConnections.TryGetValue(userId, out var connectionId);
            _requestConnections.TryGetValue(requestId, out var connReqList);
            if (connReqList.Contains(connectionId))
            {
                return connectionId;
            }
            return connectionId;
        }

        public List<string> GetConnectionIdsForGroupChat(int? senderId, int? requestId)
        {
            _userConnections.TryGetValue(senderId, out var connectionId);
            _requestConnections.TryGetValue(requestId, out var connReqList);
            var list = new List<string>();
            if (connReqList.Contains(connectionId))
            {
                connReqList.Remove(connectionId);
                return connReqList;
            }
            return connReqList;
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
            var AspId = _contextAccessor.HttpContext?.Session.GetInt32("Aspid");
            var RequestId = _contextAccessor.HttpContext?.Session.GetInt32("RequestId");
            _connectionManager.RemoveConnection(AspId,RequestId);
            return base.OnDisconnectedAsync(exception);
        }


        public async Task SendMessage(string chattype,string message,string SenderAspid,string ReciverAspid,string sentfrom,string requestid)
        {
            ChatModal chat = new ChatModal();
            chat.SenderAspId = int.Parse(SenderAspid);
            chat.ReciverAspId = int.Parse(ReciverAspid);
            chat.message = message;
            chat.sentFrom = sentfrom;
            chat.requestid = int.Parse(requestid);
            chat.chatType = chattype;

            _chatRepo.SaveChats(chat);

            if(chattype == "groupchat")
            {
                List<string> reciverConnectionId = _connectionManager.GetConnectionIdsForGroupChat(int.Parse(SenderAspid), int.Parse(requestid));

                if (reciverConnectionId != null)
                {
                    foreach(var connectionId in reciverConnectionId)
                    {
                        await Clients.Client(connectionId).SendAsync("ReceiveMessage", message, "reciver");
                    }                }
            }
            else
            {
                string reciverConnectionId = _connectionManager.GetConnectionId(int.Parse(ReciverAspid), int.Parse(requestid));

                if (reciverConnectionId != null)
                {
                    await Clients.Client(reciverConnectionId).SendAsync("ReceiveMessage", message, "reciver");
                }
            }
        }
    }
}

