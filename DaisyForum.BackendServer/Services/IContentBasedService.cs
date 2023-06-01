using DaisyForum.ViewModels.Contents;

namespace DaisyForum.BackendServer.Services;

public interface IContentBasedService
{
    List<KnowledgeBasesFromCSV> GetData(string filePath);
    Task SeedData(List<KnowledgeBaseCreateRequest> requests, string UserId);
}