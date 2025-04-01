
using Music_Booking_App.Core.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Music_Booking_App.Core.Helpers;

namespace Music_Booking_App.Services.Authentication.Configurations
{
    public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
    {
        private readonly JwtConfig _jwtOption;
        public ConfigureJwtBearerOptions(IOptions<JwtConfig> options)
        {
            _jwtOption = options.Value;
        }

        public void Configure(JwtBearerOptions options)
        {
            Configure(null, options);
        }

        public void Configure(string name, JwtBearerOptions options)
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(_jwtOption.SecretKey.GetBytes()),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            options.Events = new JwtBearerEvents
            {
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    if (!context.Response.HasStarted)
                    {
                        throw new UnauthorizedException();
                    }

                    return Task.CompletedTask;
                },
                OnForbidden = _ => throw new ForbiddenException(),
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];

                    if (!string.IsNullOrEmpty(accessToken) &&
                        context.HttpContext.Request.Path.StartsWithSegments("/notifications"))
                    {
                        // Read the token out of the query string
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                }
            };
        }
    }
}
