using Music_Booking_App.Services.Authentication.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;

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


        public async Task SendEmailAsync(string recipientEmail, string subject, string otp)
        {
            string message = string.Empty;
            if (subject.ToLower() == "verify email address")
            {
                message = ComposeOTPSignUpMail(otp);
            }

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
