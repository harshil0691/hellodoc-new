﻿using hellodoc.DbEntity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.Repositories.Repository.Interface
{
    public interface IChatRepo
    {
        ChatModal GetChats(PartialViewModal partialView);
        void SaveChats(ChatModal chatModal);
    }
}
