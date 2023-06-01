using AutoMapper;
using DaisyForum.BackendServer.Data.Entities;
using DaisyForum.ViewModels.Contents;

namespace DaisyStudy.Models.Mappings;
public class KnowledgeBaseProfile : Profile
{
    public KnowledgeBaseProfile()
    {
        CreateMap<KnowledgeBasesFromCSV, KnowledgeBaseCreateRequest>()
            .ForMember(dst => dst.CategoryId, opt => opt.MapFrom(x => 1))
            .ForMember(dst => dst.Title, opt => opt.MapFrom(x => x.Title))
            .ForMember(dst => dst.Description, opt => opt.MapFrom(x => x.Title))
            .ForMember(dst => dst.Problem, opt => opt.MapFrom(x => x.Body))
            .ForMember(dst => dst.Labels, opt => opt.MapFrom(x => x.Tags));

        CreateMap<KnowledgeBase, KnowledgeBasesFromCSV>()
        .ForMember(dst => dst.Id, opt => opt.MapFrom(x => x.Id))
        .ForMember(dst => dst.Title, opt => opt.MapFrom(x => x.Title))
        .ForMember(dst => dst.Tags, opt => opt.MapFrom(x => x.Labels.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)));
    }
}