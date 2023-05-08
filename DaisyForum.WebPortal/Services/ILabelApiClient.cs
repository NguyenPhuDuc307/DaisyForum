using DaisyForum.ViewModels.Contents;

namespace DaisyForum.WebPortal.Services;

public interface ILabelApiClient
{
    Task<List<LabelViewModel>> GetPopularLabels(int take);

    Task<LabelViewModel> GetLabelById(string labelId);
}