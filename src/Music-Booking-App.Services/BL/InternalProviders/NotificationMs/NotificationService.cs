

using Music_Booking_App.Core.Helpers;
using Music_Booking_App.Models.DTOs;
using Music_Booking_App.Services.BL.InternalProviders.NotificationMs.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace Music_Booking_App.Services.BL.InternalProviders.NotificationMs
{
    public class NotificationService : INotificationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<NotificationService> _logger;
        private readonly ICommon _common;
        //private readonly IRabbitMQPublisher _rabbitMqPublisher;
        private readonly string _notificationMsUrl;
        private readonly string _allowedIp;
        private readonly string _demoAppUrl;


        public NotificationService(IConfiguration configuration,
                                   ILogger<NotificationService> logger,
                                   ICommon common,/*
                                   IRabbitMQPublisher rabbitMqPublisher,*/
                                   IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _common = common;
            //_rabbitMqPublisher = rabbitMqPublisher;
            _notificationMsUrl = _configuration.GetValue<string>("NotificationService:BaseUrl");
            _allowedIp = _configuration.GetValue<string>("ApplicationUserIp:GeneralIp");
            _httpClientFactory = httpClientFactory;
            _demoAppUrl = _configuration.GetValue<string>("AppInfo:FrontEndUrl");
        }

        public async Task SendEmailAsync(string recipientEmail, string templateName, string value)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var apiKeyResponse = await EnsureApplicationUserExistsAsync(client);
                client.DefaultRequestHeaders.Add("ApiKey", apiKeyResponse.Data.ApiKey);

                var templateTypeResponse = await EnsureTemplateTypeExistsAsync(client, "Email");

                // Determine the template name based on the subject
                var subject = GetSubjectByTemplateName(templateName);

                var templateResponse = await EnsureTemplateExistsAsync(client, templateName, templateTypeResponse.Data.TemplateTypeId, subject);

                // Customize the content by replacing the placeholder with the dynamic value
                var templateContent = templateResponse.Data.TemplateContent;

                /*if (templateName == EmailTemplates.VerifyShareholder.ToString())
                    value = $"{_demoAppUrl}/{value}";
*/

                var updatedMessage = templateContent.Replace("{placeholder}", value);

                var sendEmailRequest = new
                {
                    TemplateName = templateName,
                    Body = updatedMessage,
                    Receivers = new[] { recipientEmail },
                    Cc = (string[])null,
                    Bcc = (string[])null
                };

                var sendEmailUrl = $"{_notificationMsUrl}/api/v1/EmailRequest";
                var sendEmailResponse = await client.PostAsJsonAsync(sendEmailUrl, sendEmailRequest);

                if (!sendEmailResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to send email via API. Falling back to RabbitMQ.");
                    // await SendViaRabbitMqAsync(recipientEmail, templateName, updatedMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email");
            }
        }

        private async Task<ApplicationUserDto> EnsureApplicationUserExistsAsync(HttpClient client)
        {
            var appUserName = "Hub Service";
            var getAppUserByNameUrl = $"{_notificationMsUrl}/api/v1/ApplicationUser/{appUserName}";
            var response = await client.GetAsync(getAppUserByNameUrl);

            if (!response.IsSuccessStatusCode)
            {
                var createAppUserUrl = $"{_notificationMsUrl}/api/v1/ApplicationUser";
                var request = new
                {
                    Name = appUserName,
                    AllowedIp = _allowedIp,
                    IsActive = true
                };

                response = await client.PostAsJsonAsync(createAppUserUrl, request);
                response.EnsureSuccessStatusCode();
                response = await client.GetAsync(getAppUserByNameUrl);
            }

            return await response.Content.ReadFromJsonAsync<ApplicationUserDto>();
        }

        private async Task<TemplateTypeDto> EnsureTemplateTypeExistsAsync(HttpClient client, string templateTypeName)
        {
            var getTemplateTypeByNameUrl = $"{_notificationMsUrl}/api/v1/TemplateType/Name?templateTypeName={templateTypeName}";
            var response = await client.GetAsync(getTemplateTypeByNameUrl);

            if (!response.IsSuccessStatusCode)
            {
                var addTemplateTypeUrl = $"{_notificationMsUrl}/api/v1/TemplateType";
                var request = new { TemplateTypeName = templateTypeName };

                response = await client.PostAsJsonAsync(addTemplateTypeUrl, request);
                response.EnsureSuccessStatusCode();
                response = await client.GetAsync(getTemplateTypeByNameUrl);
            }

            return await response.Content.ReadFromJsonAsync<TemplateTypeDto>();
        }

        private async Task<TemplateDto> EnsureTemplateExistsAsync(HttpClient client, string templateName, string templateTypeId, string subject)
        {
            var getTemplateByNameUrl = $"{_notificationMsUrl}/api/v1/Template/Name?templateName={templateName}";
            var response = await client.GetAsync(getTemplateByNameUrl);

            string templateContent = GetComposedMailTemplateBySubject(subject);

            if (!response.IsSuccessStatusCode)
            {
                var addTemplateUrl = $"{_notificationMsUrl}/api/v1/Template";
                var request = new
                {
                    TemplateName = templateName,
                    Subject = subject,
                    TemplateContent = templateContent,
                    TemplateTypeId = templateTypeId
                };

                response = await client.PostAsJsonAsync(addTemplateUrl, request);
                response.EnsureSuccessStatusCode();
                response = await client.GetAsync(getTemplateByNameUrl);
            }

            return await response.Content.ReadFromJsonAsync<TemplateDto>();
        }

        private static string GetSubjectByTemplateName(string templateName)
        {
            return templateName switch
            {
                "SendOtp" => "Verify Email Address",
                "ForgotPassword" => "Reset Password",
                "AdminForgotPassword" => "Admin Password Reset",
                "VerifyShareholder" => "Complete Shareholder Verification",
                "AdminOnboarding" => "Admin Onboarding",
                "Account Status Update" => "Account Status Update",
                _ => throw new ArgumentException($"Unknown template: {templateName}")
            };
        }

        private static string GetComposedMailTemplateBySubject(string subject)
        {
            return subject switch
            {
                "Verify Email Address" => ComposeOTPSignUpMail("{placeholder}"),
                "Reset Password" => ComposeForgetPasswordMail("{placeholder}"),
                "Admin Password Reset" => ComposeAdminForgetPasswordMail("{placeholder}"),
                "Complete Shareholder Verification" => ComposeVerificationLinkMail("{placeholder}"),
                "Admin Onboarding" => ComposeOnboardingMail("{defaultPass}", "{adminOnboardingUrl}"),
                // "Account Status Update" => ComposeAccountStatusMail("{status}"),
                _ => throw new ArgumentException($"Unknown subject: {subject}")
            };
        }

        /*private async Task SendViaRabbitMqAsync(string recipientEmail, string templateName, string templateContent)
        {
            try
            {
                _logger.LogInformation("Attempting to log audit");
                _logger.LogInformation("Audit log successfully published and saved locally");

                var message = new
                {
                    Receiver = recipientEmail,
                    TemplateId = templateName,
                    Body = templateContent
                };

                var jsonData = _common.GetJsonString(message);
                try
                {
                    _rabbitMqPublisher.PublishTo<string>("EmailRequestQueue", jsonData);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to publish to RabbitMQ. Continuing with local save.");
                }

                _logger.LogInformation("Audit log successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending via RabbitMq");
            }
        }
*/
        private static string ComposeVerificationLinkMail(string verificationLink)
        {
            string message = $@"<html>
                <body>
                <p>Hello there,</p>
                <p>As a shareholder, you are required to complete the verification process</p>
                <p>This link will direct you to our secure verification platform, where you can complete the necessary steps.</p>
                <p>Please click the link below to proceed:</p>
                <p>
                <a href='{verificationLink}' style='display: inline-block; padding: 10px 20px; font-size: 16px; color: white; background-color: blue; text-decoration: none; border-radius: 5px;'>Verification Link</a>
                </p>
                <p>Thank you for your prompt action.</p>
                <p style=""margin-top: 1em;""></p>
                <p>Best Regards,</p>
                <p>Mondu Team</p>
                <p style=""margin-top: 1em;""></p>
                <p><strong>NB:</strong> If you received this message in error, please disregard this email.</p>
                </body>
                </html>";

            return message;
        }

        private static string ComposeOTPSignUpMail(string otp)
        {
            string message = $@"<html>
                <body>
                <p>Hello there,</p>
                <p>Thank you for creating an account with us.</p>
                <p>Please enter the provided OTP to verify your email address and activate your account.</p>
                <p>Your OTP: <strong>{otp}</strong></p> 
                <p>This OTP is valid for a limited time, and for security reasons, please do not share it with anyone.</p>
                <p style=""margin-top: 1em;""></p>
                <p>Best Regards,</p>
                <p>Mondu Team</p>
                <p style=""margin-top: 1em;""></p>
                <p><strong>NB:</strong> If you did not sign up for an account with us, please disregard this email.</p>
                </body>
                </html>";

            return message;
        }

        private static string ComposeForgetPasswordMail(string otp)
        {
            string message = $@"<html>
        <body>
        <p>Hello there,</p>
        <p>We received a request to reset your password. To proceed with the reset, please use the OTP provided below:</p>
        <p>Your OTP: <strong>{otp}</strong></p>
        <p>This OTP is valid for a limited time. For security reasons, please do not share this code with anyone.</p>
        <p style=""margin-top: 1em;""></p>
        <p>Best Regards,</p>
        <p>Mondu Team</p>
        <p style=""margin-top: 1em;""></p>
        <p><strong>NB:</strong> If you did not request a password reset, please ignore this email, and your account will remain secure.</p>
        </body>
        </html>";

            return message;
        }

        private static string ComposeAdminForgetPasswordMail(string defaultPass)
        {
            string message = $@"<html>
        <body>
        <p>Hello there,</p>
        <p>We received a request to reset your password. To proceed with the reset, please use the default password provided below:</p>
        <p>Your Default Password: <strong>{defaultPass}</strong></p>
        <p>For security reasons, please do not share this code with anyone.</p>
        <p style=""margin-top: 1em;""></p>
        <p>Best Regards,</p>
        <p>Mondu Team</p>
        <p style=""margin-top: 1em;""></p>
        <p><strong>NB:</strong> If you did not request a password reset, please ignore this email, and your account will remain secure.</p>
        </body>
        </html>";

            return message;
        }

        private static string ComposeOnboardingMail(string defaultPass, string adminOnboardingUrl)
        {
            string message = $@"<html>
                <body>
                <p>Hello there,</p>
                <p>An account has been created for you by our admin team.</p>
                <p>Please enter the provided Default Password to verify your email address and activate your account.</p>
                <p>Default Password: <strong>{defaultPass}</strong></p> 
                <p>For security reasons, please do not share it with anyone.</p>
                <p>Please click the link below to proceed:</p>
                <p>
                <a href='{adminOnboardingUrl}' style='display: inline-block; padding: 10px 20px; font-size: 16px; color: white; background-color: blue; text-decoration: none; border-radius: 5px;'>Activate Account</a>
                </p>
                <p style=""margin-top: 1em;""></p>
                <p>Best Regards,</p>
                <p>Mondu</p>
                <p style=""margin-top: 1em;""></p>
                <p><strong>NB:</strong> If you received this message in error, please disregard this email.</p>
                </body>
                </html>";

            return message;
        }
    }
}
