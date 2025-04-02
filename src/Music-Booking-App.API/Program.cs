
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Music_Booking_App.Core.Helpers;
using Music_Booking_App.Data.Commands.Implementations;
using Music_Booking_App.Data.Commands.Interfaces;
using Music_Booking_App.Data.Executers.Implementations;
using Music_Booking_App.Data.Executers.Interfaces;
using Music_Booking_App.Data.Helpers.Implementations;
using Music_Booking_App.Data.Helpers.Interfaces;
using Music_Booking_App.Data.Queries.Implementations;
using Music_Booking_App.Data.Queries.Interfaces;
using Music_Booking_App.Migrations;
using Music_Booking_App.Models.Entiites;
using Music_Booking_App.Models.Helpers.Validators;
using Music_Booking_App.Models.RequestModels;
using Music_Booking_App.Services.Authentication;
using Music_Booking_App.Services.Authentication.Implementations;
using Music_Booking_App.Services.Authentication.Interfaces;
using Music_Booking_App.Services.BL.Implementations;
using Music_Booking_App.Services.BL.Interfaces;
using Music_Booking_App.Services.BL.InternalProviders.NotificationMs;
using Music_Booking_App.Services.Helpers;
using Serilog;
using Serilog.Events;

namespace Music_Booking_App.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            Log.Logger = new LoggerConfiguration()
             .Enrich.FromLogContext()
             .WriteTo.Console(LogEventLevel.Information)
            .CreateLogger();

            builder.Host.UseSerilog();

            builder.Services.AddLogging(config =>
            {
                config.AddConsole();
                config.AddSerilog(dispose: true);
                config.AddFilter("Music_Booking_App.Services.BL.Implementations.AuthenticationService", LogLevel.Debug);
            });

            builder.Logging.ClearProviders();

            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
             .AddEnvironmentVariables()
            .AddUserSecrets<Program>();

            /*  builder.WebHost.ConfigureKestrel(serverOptions =>
              {
                  serverOptions.Listen(IPAddress.Any, 3025);
              });*/

            builder.Services.AddControllers();

            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DbConnectionString")));
            builder.Services.ConfigureAuthentication(builder.Configuration);

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHttpClient();

            builder.Services.AddTransient<IReadExecuter, ReadExecuter>();
            builder.Services.AddTransient<IWriteExecuter, WriteExecuter>();
            builder.Services.AddTransient<IReadUtilities, ReadUtilities>();
            builder.Services.AddTransient<IWriteUtilities, WriteUtilities>();
            builder.Services.AddTransient<ICommon, Common>();

            builder.Services.AddTransient<ITestService, TestService>();
            builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();
            builder.Services.AddTransient<INotificationService, NotificationService>();
            builder.Services.AddTransient<ITeamMemberService, TeamMemberService>();
            builder.Services.AddTransient<IBookingService, BookingService>();

            builder.Services.AddTransient<IOtpService, OtpService>();
            builder.Services.AddTransient<ITokenService, TokenService>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IPasswordHash, PasswordHasher>();

            builder.Services.AddTransient<IDapperQueryRepository<Test>, DapperQueryRepository<Test>>();
            builder.Services.AddTransient<IDapperCommandRepository<Test>, DapperCommandRepository<Test>>();
            builder.Services.AddTransient<IDapperQueryRepository<User>, DapperQueryRepository<User>>();
            builder.Services.AddTransient<IDapperCommandRepository<User>, DapperCommandRepository<User>>();
            builder.Services.AddTransient<IDapperQueryRepository<OTP>, DapperQueryRepository<OTP>>();
            builder.Services.AddTransient<IDapperCommandRepository<OTP>, DapperCommandRepository<OTP>>();
            builder.Services.AddTransient<IDapperQueryRepository<Artiste>, DapperQueryRepository<Artiste>>();
            builder.Services.AddTransient<IDapperCommandRepository<Artiste>, DapperCommandRepository<Artiste>>();
            builder.Services.AddTransient<IDapperQueryRepository<Event>, DapperQueryRepository<Event>>();
            builder.Services.AddTransient<IDapperCommandRepository<Event>, DapperCommandRepository<Event>>();
            builder.Services.AddTransient<IDapperQueryRepository<Booking>, DapperQueryRepository<Booking>>();
            builder.Services.AddTransient<IDapperCommandRepository<Booking>, DapperCommandRepository<Booking>>();
            builder.Services.AddTransient<IDapperQueryRepository<Ticket>, DapperQueryRepository<Ticket>>();
            builder.Services.AddTransient<IDapperCommandRepository<Ticket>, DapperCommandRepository<Ticket>>();

            builder.Services.AddTransient<ITestValidator, TestValidator>();
            builder.Services.AddTransient<IAuthenticationValidator, AuthenticationValidator>();
            builder.Services.AddTransient<IBookingValidator, BookingValidator>();

            builder.Services.AddScoped<IValidator<CreateTestRequestModel>, TestRequestModelValidator>();
            builder.Services.AddScoped<IValidator<UpdateTestRequestModel>, UpdateTestRequestModelValidator>();
            builder.Services.AddScoped<IValidator<SendOtpRequestModel>, SendOtpRequestModelValidator>();
            builder.Services.AddScoped<IValidator<VerifyOtpRequestModel>, VerifyOtpRequestModelValidator>();
            builder.Services.AddScoped<IValidator<SendDefaultPassRequest>, SendDefaultPassRequestValidator>();
            builder.Services.AddTransient<IValidator<SignUpRequestModel>, SignUpRequestModelValidator>();
            builder.Services.AddTransient<IValidator<LoginRequestModel>, LoginRequestModelValidator>();
            builder.Services.AddTransient<IValidator<ResetPasswordRequestModel>, ResetPasswordRequestModelValidator>();
            builder.Services.AddTransient<IValidator<ChangePasswordRequestModel>, ChangePasswordRequestModelValidator>();
            builder.Services.AddTransient<IValidator<CreateArtisteRequestModel>, CreateArtisteRequestValidator>();
            builder.Services.AddTransient<IValidator<CreateEventRequestModel>, CreateEventRequestValidator>();
            builder.Services.AddTransient<IValidator<BookingRequestModel>, BookingRequestValidator>();
            builder.Services.AddTransient<IValidator<ApprovalReviewRequestModel>, ApprovalReviewRequestValidator>();

            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            builder.Services.AddHealthChecks()
           .AddCheck("self", () => HealthCheckResult.Healthy());

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "MusicBookingAPI V1");
            });


            app.UseHttpsRedirection();
            app.UseCors();

            app.UseAuthorization();

            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    var result = System.Text.Json.JsonSerializer.Serialize(
                        new
                        {
                            status = report.Status.ToString(),
                            checks = report.Entries.Select(e => new
                            {
                                name = e.Key,
                                status = e.Value.Status.ToString(),
                                description = e.Value.Description
                            })
                        });
                    await context.Response.WriteAsync(result);
                }
            });


            app.MapControllers();

            app.Run();
        }
    }
}
