using AutoMapper;
using DaisyForum.BackendServer.Data.Entities;
using DaisyForum.ViewModels.Contents;

namespace Chat.Web.Mappings
{
    public class RoomProfile : Profile
    {
        public RoomProfile()
        {
            CreateMap<Room, RoomViewModel>()
                .ForMember(dst => dst.Admin, opt => opt.MapFrom(x => x.Admin.UserName));

            CreateMap<RoomViewModel, Room>();
        }
    }
}
