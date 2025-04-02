using MailKit.Net.Smtp;
using MimeKit;
using Music_Booking_App.Services.Authentication.Interfaces;

namespace Music_Booking_App.Services.Authentication.Implementations
{
    public class OtpService : IOtpService
    {
        public string GenerateOTP()
        {
            var random = new Random();
            var otp = random.Next(1000, 9999);
            return otp.ToString();
        }


        public async Task SendEmailAsync(string recipientEmail, string subject, string value)
        {
            string message = string.Empty;
            if (subject.Equals("verify email address", StringComparison.CurrentCultureIgnoreCase))
                message = ComposeOTPSignUpMail(value);

            if (subject.Equals("forgot password", StringComparison.CurrentCultureIgnoreCase))
                message = ComposeForgetPasswordMail(value);

            if (subject.Equals("admin forgot password", StringComparison.CurrentCultureIgnoreCase))
                message = ComposeAdminForgetPasswordMail(value);

            if (subject.Equals("request rejected", StringComparison.CurrentCultureIgnoreCase))
                message = ComposeRejectionMail(value);

            if (subject.Equals("request approved", StringComparison.CurrentCultureIgnoreCase))
                message = ComposeApprovalMail(value);


            #region Others
            /* else if (subject.ToLower() == "password reset")
             {
                 message = ComposePasswordReset(firstname, recipientEmail, defaultPass);
             }
             else if (subject.ToLower() == "signup")
             {
                 message = SignUp(firstname, recipientEmail, defaultPass);
             }*/
            #endregion

            await MailSender(recipientEmail, subject, message);
        }

        private string ComposeOTPSignUpMail(string otp)
        {
            string message = $@"<html>
             <body>
             <p>Hello there,</p>
             <p>Thank you for creating an account with us. To activate your account and complete the registration process, please use the following one-time password (OTP):</p>
             <p>Your OTP: {otp}.</p>                    
             <p>Please enter this OTP to verify your email address and activate your account..</p>
             <p>Note: This OTP is valid for a limited time, and for security reasons, do not share it with anyone.</p>
             <p>If you did not sign up for an account with us, please disregard this email.</p>
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


        private static string ComposeRejectionMail(string rejectionDetails)
        {
            string message = $@"<html>
            <body>
            <p>Hello,</p>
            <p>We regret to inform you that your request has been rejected.</p>
            <p>Below are the details regarding the reason for rejection:</p>
            <p>{rejectionDetails}</p>
            <p>We encourage you to review the feedback provided and make the necessary adjustments before resubmitting your request.</p>
            <p>If you require further assistance, please feel free to reach out to our support team.</p>
            <p style=""margin-top: 1em;""></p>
            <p>Best Regards,</p>
            <p>Transcap Team</p>
            <p style=""margin-top: 1em;""></p>
            <p><strong>NB:</strong> If you believe this rejection was made in error, please contact our support team immediately.</p>
            </body>
            </html>";

            return message;
        }

        private static string ComposeApprovalMail(string approvalDetails)
        {
            string message = $@"<html>
            <body>
            <p>Hello,</p>
            <p>We are pleased to inform you that your request has been approved.</p>
            <p>Below are the details of your approved request:</p>
            <p>{approvalDetails}</p>
            <p>You may proceed with the next steps as required. If you have any further inquiries, feel free to reach out to our support team.</p>
            <p style=""margin-top: 1em;""></p>
            <p>Best Regards,</p>
            <p>Transcap Team</p>
            <p style=""margin-top: 1em;""></p>
            <p><strong>NB:</strong> If you need additional clarification, do not hesitate to contact our support team.</p>
            </body>
            </html>";

            return message;
        }

        private async Task MailSender(string recipientEmail, string subject, string body)
        {
            var emailMessage = new MimeMessage();
            //emailMessage.From.Add(new MailboxAddress("", "odufeso1@gmail.com"));
            emailMessage.From.Add(new MailboxAddress("", "luluadegunju@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("", recipientEmail));
            emailMessage.Subject = subject;

            BodyBuilder bodyBuilder = new()
            {
                HtmlBody = body
            };

            emailMessage.Body = bodyBuilder.ToMessageBody();


            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync("smtp.gmail.com", 465, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    // await client.AuthenticateAsync("odufeso1@gmail.com", "aldcheqgzfrtaewz");
                    await client.AuthenticateAsync("luluadegunju@gmail.com", "fpbctubmclavhham");

                    await client.SendAsync(emailMessage);
                }

                catch (Exception ex)
                {
                    throw new ApplicationException("Failed to send email.", ex);
                }

                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }

        }
    }
}
