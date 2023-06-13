namespace DaisyForum.WebPortal.Extensions;

public interface IRecaptchaExtension
{
    Task<bool> VerifyAsync(string token);
}