using BlazorFirebaseAuth.Auth;
using BlazorFirebaseAuth.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure Auth
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthenticationServices(options =>
{
    options.ClientId = builder.Configuration.GetValue<string>("Firebase:AuthConfig:GoogleClientId")!;
    options.ClientSecret = builder.Configuration.GetValue<string>("Firebase:AuthConfig:GoogleClientSecret")!;
    options.CallbackPath = "/google-signin-success";
    options.SaveTokens = true;
});
builder.Services.AddScoped<IAuthService, FirebaseAuthService>();


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