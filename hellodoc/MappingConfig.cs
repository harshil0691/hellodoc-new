using AutoMapper;
using hellodoc.DbEntity.DataModels;
using hellodoc.DbEntity.ViewModels.AdminAccess;
using Microsoft.Build.Framework.Profiler;

namespace hellodoc
{
    public class MappingConfig : Profile
    {
        public MappingConfig() 
        {
            CreateMap<Role, CreateRoleModal>().ReverseMap();
        }
    }
}
