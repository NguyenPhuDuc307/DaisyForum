using Newtonsoft.Json;

namespace DaisyForum.BackendServer.Models;

public class TokenResponseModel
{
    // Response Model from Google Recaptcha V3 Verify API
    [JsonProperty("success")]
    public bool Success { get; set; }

    [JsonProperty("score")]
    public decimal Score { get; set; }

    [JsonProperty("action")]
    public string? Action { get; set; }

    [JsonProperty("error-codes")]
    public List<string>? ErrorCodes { get; set; }
}
