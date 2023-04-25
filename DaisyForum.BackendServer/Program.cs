using DaisyForum.BackendServer.Data;
using DaisyForum.BackendServer.Data.Entities;
using DaisyForum.BackendServer.Extensions;
using DaisyForum.BackendServer.IdentityServer;
using DaisyForum.BackendServer.Services;
using DaisyForum.ViewModels.Systems.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
//using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

var DaisyForumSpecificOrigins = "DaisyForumSpecificOrigins";
var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>(); builder.Host.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));
//1. Setup entity framework
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        configuration.GetConnectionString("DefaultConnection")));
//2. Setup identity
services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

services.AddIdentityServer(options =>
{
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;
})
.AddInMemoryApiResources(Config.Apis)
.AddInMemoryApiScopes(Config.ApiScopes)
.AddInMemoryClients(configuration.GetSection("IdentityServer:Clients"))
.AddInMemoryIdentityResources(Config.Ids)
.AddAspNetIdentity<User>()
.AddProfileService<IdentityProfileService>()
.AddDeveloperSigningCredential();

services.AddCors(options =>
{
    options.AddPolicy(DaisyForumSpecificOrigins,
    builder =>
    {
        if (allowedOrigins != null)
            builder.WithOrigins(allowedOrigins)
                   .AllowAnyMethod()
                   .AllowAnyHeader();
    });
});

services.Configure<IdentityOptions>(options =>
{
    // Default Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.User.RequireUniqueEmail = true;
});
Log.Logger = new LoggerConfiguration()
.Enrich.FromLogContext()
.WriteTo.Console()
.CreateLogger();

services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// Add services to the container.
services.AddControllersWithViews();

// Add validator to the service collection
services.AddFluentValidationAutoValidation();
services.AddFluentValidationClientsideAdapters();
services.AddValidatorsFromAssemblyContaining<RoleCreateRequestValidator>();

services.AddAuthentication()
.AddLocalApi("Bearer", option =>
{
    option.ExpectedScope = "api.daisyforum";
});

services.AddAuthorization(options =>
{
    options.AddPolicy("Bearer", policy =>
    {
        policy.AddAuthenticationSchemes("Bearer");
        policy.RequireAuthenticatedUser();
    });
});


services.AddRazorPages(options =>
{
    options.Conventions.AddAreaFolderRouteModelConvention("Identity", "/Account/", model =>
    {
        foreach (var selector in model.Selectors)
        {
            var attributeRouteModel = selector.AttributeRouteModel;
            if (attributeRouteModel != null)
            {
                attributeRouteModel.Order = -1;
                if (attributeRouteModel.Template != null)
                    attributeRouteModel.Template = attributeRouteModel.Template.Remove(0, "Identity".Length);
            }
        }
    });
});

services.AddAuthentication()
.AddGoogle(googleOptions =>
{
    var googleClientId = configuration.GetSection("Authentication:Google:ClientId").Value;
    var googleClientSecret = configuration.GetSection("Authentication:Google:ClientSecret").Value;

    if (googleClientId != null && googleClientSecret != null)
    {
        googleOptions.ClientId = googleClientId;
        googleOptions.ClientSecret = googleClientSecret;
    }


    googleOptions.Scope.Add("profile");
    googleOptions.Scope.Add("phone");
    // googleOptions.Scope.Add("https://www.googleapis.com/auth/user.birthday.read");
    // googleOptions.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
    // googleOptions.ClaimActions.MapJsonKey("urn:google:locale", "locale", "string");

    // googleOptions.SaveTokens = true;

    // googleOptions.Events.OnCreatingTicket = ctx =>
    // {
    //     List<AuthenticationToken> tokens = ctx.Properties.GetTokens().ToList();

    //     tokens.Add(new AuthenticationToken()
    //     {
    //         Name = "TicketCreated",
    //         Value = DateTime.UtcNow.ToString()
    //     });

    //     ctx.Properties.StoreTokens(tokens);

    //     return Task.CompletedTask;
    // };
});

services.AddTransient<DbInitializer>();
services.AddTransient<IEmailSender, EmailSenderService>();
services.AddTransient<ISequenceService, SequenceService>();
services.AddTransient<IStorageService, FileStorageService>();
services.AddTransient<IStorageService, FileStorageService>();

services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DaisyForum API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            Implicit = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("https://localhost:5000/connect/authorize"),
                Scopes = new Dictionary<string, string> { { "api.daisyforum", "DaisyForum API" } }
            },
        },
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new List<string>{ "api.daisyforum" }
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseErrorWrapping();

app.UseStaticFiles();

app.UseIdentityServer();

app.UseAuthentication();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(DaisyForumSpecificOrigins);

app.UseAuthorization();

app.MapDefaultControllerRoute();

app.MapRazorPages();

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.OAuthClientId("swagger");
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DaisyForum API");
});

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    try
    {
        Log.Information("Seeding data...");
        var dbInitializer = serviceProvider.GetService<DbInitializer>();
        if (dbInitializer != null)
            dbInitializer.Seed()
                         .Wait();
    }
    catch (Exception ex)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();
