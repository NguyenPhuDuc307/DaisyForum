using AutoMapper;
using DaisyForum.BackendServer.Data.Entities;
using DaisyForum.ViewModels.Systems;

namespace Chat.Web.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserViewModel>()
                .ForMember(dst => dst.UserName, opt => opt.MapFrom(x => x.UserName));

            CreateMap<UserViewModel, User>();
        }
    }
}
