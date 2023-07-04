namespace DaisyForum.BackendServer.Extensions;

public interface IRecaptchaExtension
{
    Task<bool> VerifyAsync(string token);
}