using DaisyForum.BackendServer.Models;
using Newtonsoft.Json;

namespace DaisyForum.BackendServer.Extensions;

public class RecaptchaExtension : IRecaptchaExtension
{
    private IConfiguration _configuration { get; }
    private static string? GoogleSecretKey { get; set; }
    private static string? GoogleRecaptchaVerifyApi { get; set; }
    private static decimal RecaptchaThreshold { get; set; }
    public RecaptchaExtension()
    {
        _configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .Build();

        GoogleRecaptchaVerifyApi = _configuration.GetSection("GoogleRecaptcha").GetSection("VefiyAPIAddress").Value ?? "";
        GoogleSecretKey = _configuration.GetSection("GoogleRecaptcha").GetSection("Secretkey").Value ?? "";

        var hasThresholdValue = decimal.TryParse(_configuration.GetSection("RecaptchaThreshold").Value ?? "", out var threshold);
        if (hasThresholdValue)
        {
            RecaptchaThreshold = threshold;
        }
    }
    public async Task<bool> VerifyAsync(string token)
    {
        if (String.IsNullOrEmpty(token))
        {
            throw new Exception("Token cannot be null!");
        }
        using (var client = new HttpClient())
        {
            var response = await client.GetStringAsync($"{GoogleRecaptchaVerifyApi}?secret={GoogleSecretKey}&response={token}");
            var tokenResponse = JsonConvert.DeserializeObject<TokenResponseModel>(response);
            if (tokenResponse != null)
            {
                if (!tokenResponse.Success || tokenResponse.Score < RecaptchaThreshold)
                {
                    return false;
                }
            }
        }
        return true;
    }
}
