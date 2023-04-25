using DaisyForum.WebPortal.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddHttpClient();

//IdentityModelEventSource.ShowPII = true; //Add this line
services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";
})
    .AddCookie("Cookies")
    .AddOpenIdConnect("oidc", options =>
    {
        options.Authority = configuration["Authorization:AuthorityUrl"];
        options.RequireHttpsMetadata = false;
        options.GetClaimsFromUserInfoEndpoint = true;

        options.ClientId = configuration["Authorization:ClientId"];
        options.ClientSecret = configuration["Authorization:ClientSecret"];
        options.ResponseType = "code";

        options.SaveTokens = true;

        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("offline_access");
        options.Scope.Add("api.knowledgespace");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = "name",
            RoleClaimType = "role"
        };
    });

// Add services to the container.
var mvcBuilder = services.AddControllersWithViews();
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
if (environment == Environments.Development)
{
    mvcBuilder.AddRazorRuntimeCompilation();
}

//Declare DI containers
services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

services.AddTransient<ICategoryApiClient, CategoryApiClient>();
services.AddTransient<IKnowledgeBaseApiClient, KnowledgeBaseApiClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

