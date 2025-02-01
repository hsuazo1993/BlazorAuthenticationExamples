using System.Security.Claims;
using BlazorFirebaseAuth.Auth;
using BlazorFirebaseAuth.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Cookie.Name = ".AspNetCore.AuthCookie";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration.GetValue<string>("Firebase:AuthConfig:GoogleClientId")!;
    options.ClientSecret = builder.Configuration.GetValue<string>("Firebase:AuthConfig:GoogleClientSecret")!;
    options.CallbackPath = new PathString("/google-signin-success");
    options.SaveTokens = true; // Enable saving tokens

    options.CorrelationCookie.SameSite = SameSiteMode.Unspecified;

    options.Events.OnCreatingTicket = context =>
    {
        string email = context.User.GetProperty("email").GetString()!;
        bool emailVerified = context.User.GetProperty("email_verified").GetBoolean()!;
        string name = context.User.GetProperty("name").GetString()!;
        string givenName = context.User.GetProperty("given_name").GetString()!;
        string familyName = context.User.GetProperty("family_name").GetString()!;
        string picture = context.User.GetProperty("picture").GetString()!;

        var claims = new List<Claim>
        {
            new Claim("email", email),
            new Claim("email_verified", emailVerified.ToString()),
            new Claim("username", name),
            new Claim("first_name", givenName),
            new Claim("last_name", familyName),
            new Claim("picture", picture),
        };

        context.Identity!.AddClaims(claims);

        return Task.CompletedTask;
    };
});

builder.Services.AddScoped<IAuthService, FirebaseAuthService>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
