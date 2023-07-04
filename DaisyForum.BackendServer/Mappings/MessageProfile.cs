using AutoMapper;
using DaisyForum.BackendServer.Data.Entities;
using DaisyForum.BackendServer.Helpers;
using DaisyForum.ViewModels.Contents;

namespace DaisyForum.BackendServer.Mappings
{
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            CreateMap<Message, MessageViewModel>()
                .ForMember(dst => dst.FromUserName, opt => opt.MapFrom(x => x.FromUser.UserName))
                .ForMember(dst => dst.FromFullName, opt => opt.MapFrom(x => x.FromUser.FirstName + " " + x.FromUser.LastName))
                .ForMember(dst => dst.Avatar, opt => opt.MapFrom(x => x.FromUser.Avatar))
                .ForMember(dst => dst.Content, opt => opt.MapFrom(x => BasicEmojis.ParseEmojis(x.Content)));

            CreateMap<MessageViewModel, Message>();
        }
    }
}
