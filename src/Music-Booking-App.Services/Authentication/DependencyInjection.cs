

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Music_Booking_App.Services.Authentication.Configurations;

namespace Music_Booking_App.Services.Authentication
{
    public static class DependencyInjection
    {
        public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddOptions<JwtConfig>()
          .BindConfiguration(nameof(JwtConfig))
          .ValidateOnStart();

            // services.Configure<SumSubConfig>(configuration.GetSection("SumSubConfig"));

            services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();

            services.AddAuthentication(auth =>
           {
               auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
               auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
           }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, null!);

            services.AddSwaggerGen(c =>
             {
                 c.SwaggerDoc("v1", new OpenApiInfo { Title = "Music Booking API", Version = "v1" });
                 c.AddSecurityDefinition("Bearer",
                     new OpenApiSecurityScheme
                     {
                         Name = "Authorization",
                         Type = SecuritySchemeType.Http,
                         Scheme = "Bearer",
                         In = ParameterLocation.Header,
                         Description = "Bearer Token",
                         BearerFormat = "JWT"
                     });
                 c.AddSecurityRequirement(new OpenApiSecurityRequirement
             {
                 {
                     new OpenApiSecurityScheme
                     {
                         Name = "Bearer",
                         Reference = new OpenApiReference
                         {
                             Id = "Bearer",
                             Type = ReferenceType.SecurityScheme
                         }
                     },
                     new List<string>()
                 }
             });
             });

            return services;
        }
    }
}
