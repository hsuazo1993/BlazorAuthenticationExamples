using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;

namespace BlazorFirebaseAuth.Auth
{
    public static class AuthenticationServiceExtensions
    {
        public static void AddAuthenticationServices(
            this IServiceCollection services, 
            Action<GoogleAuthOptions> configureOptions)
        {
            var googleAuthOptions = new GoogleAuthOptions();
            configureOptions.Invoke(googleAuthOptions);

            services.AddAuthentication(options =>
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
                options.ClientId = googleAuthOptions.ClientId;
                options.ClientSecret = googleAuthOptions.ClientSecret;
                options.CallbackPath = new PathString(googleAuthOptions.CallbackPath);
                options.SaveTokens = googleAuthOptions.SaveTokens;
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
        }
    }

    public class GoogleAuthOptions
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string CallbackPath { get; set; } = "/google-signin-success";
        public bool SaveTokens { get; set; } = true;
    }
}
