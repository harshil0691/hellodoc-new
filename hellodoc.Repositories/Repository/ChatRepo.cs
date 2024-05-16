
using hellodoc.DbEntity.DataContext;
using hellodoc.DbEntity.DataModels;
using hellodoc.DbEntity.ViewModels;
using hellodoc.Repositories.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.Repositories.Repository
{
    public class ChatRepo : IChatRepo
    {
        private readonly ApplicationDbContext _context;

        public ChatRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public ChatModal GetChats(PartialViewModal partialView)
        {
            ChatModal chatModal = new ChatModal();

            switch (partialView.ReciverType)
            {
                case "physician":
                    var physician = _context.Physicians.FirstOrDefault(p => p.Physicianid == partialView.physicianid);

                    if (physician != null)
                    {
                        var chatlist = _context.Messages.Where(p => p.Senderaspid == partialView.ChatSenderAspid && p.Recieveraspid == physician.Aspnetuserid && p.Requestid == partialView.requestid);

                        chatModal.photoPath = physician.Photo ?? "./images/avatar.png";
                        chatModal.Name = physician.Firstname + " " + physician.Lastname;
                        chatModal.messages = chatlist.ToList();
                        chatModal.SenderAspId = partialView.ChatSenderAspid;
                        chatModal.ReciverAspId = physician.Aspnetuserid??1;
                    }
                    break;

                case "admin":
                    var chatlist1 = _context.Messages.Where(p => p.Senderaspid == partialView.ChatSenderAspid && p.Requestid == partialView.requestid);

                    chatModal.Name = "Admin";
                    chatModal.messages = chatlist1.ToList();
                    chatModal.SenderAspId = partialView.ChatSenderAspid;
                    chatModal.ReciverAspId = 95;
                    chatModal.photoPath = "./images/avatar.png";
                    break;

                case "user":
                    var request = _context.Requests.Include(r => r.User).FirstOrDefault(r => r.Requestid == partialView.ReciverRequestid);
                  
                    chatModal.photoPath = "./images/avatar.png";
                    chatModal.Name = request?.User?.Firstname +" "+ request?.User?.Lastname;
                    break;
            }

            return chatModal;
        }

        public void SaveChats(ChatModal chatModal)
        {
            Message message = new Message();
            message.Message1 = chatModal.message;
            message.Senderaspid = chatModal.SenderAspId;
            message.Recieveraspid = chatModal.ReciverAspId;
            message.Senttime = DateTime.Now;
            message.Sentfrom = chatModal.sentFrom;
            message.Requestid = chatModal.requestid;

            _context.Messages.Add(message);
            _context.SaveChanges();
        }
    }
}
