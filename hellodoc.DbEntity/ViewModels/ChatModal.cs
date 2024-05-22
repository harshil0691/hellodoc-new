using hellodoc.DbEntity.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels
{
    public partial class ChatModal
    {
        public int SenderAspId { get; set; }
        public int ReciverAspId { get; set; }
        public string Name { get; set; }
        public int PhysicianId { get; set; }
        public string photoPath { get; set; }
        public string message { get; set; }
        public string sentFrom { get; set; }
        public int requestid { get; set; }
        public string chatType { get; set; }
        public List<Message> messages { get; set; }
    }
}
