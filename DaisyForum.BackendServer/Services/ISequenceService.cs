namespace DaisyForum.BackendServer.Services;

public interface ISequenceService
{
    Task<int> GetKnowledgeBaseNewId();
}