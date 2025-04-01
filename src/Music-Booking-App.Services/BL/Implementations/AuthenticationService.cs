using AutoMapper;
using Music_Booking_App.Core.Constants;
using Music_Booking_App.Core.Helpers;
using Music_Booking_App.Data.Commands.Interfaces;
using Music_Booking_App.Data.Queries.Interfaces;
using Music_Booking_App.Models.Entiites;
using Music_Booking_App.Models.Enums;
using Music_Booking_App.Models.RequestModels;
using Music_Booking_App.Models.ViewModels;
using Music_Booking_App.Services.Authentication.Interfaces;
using Music_Booking_App.Services.BL.Interfaces;
using Music_Booking_App.Services.BL.InternalProviders.NotificationMs;
using Music_Booking_App.Services.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Security.Claims;
using StatusCodes = Music_Booking_App.Core.Constants.StatusCodes;

namespace Music_Booking_App.Services.BL.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserService _userService;
        private readonly IDapperQueryRepository<User> _userQueryRepository;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly int _lockoutDuration;
        private readonly int _otpExpiryMinutes;
        private readonly IAuthenticationValidator _validate;
        private readonly INotificationService _notifyMs;
        private readonly IOtpService _otpService;
        private readonly IDapperCommandRepository<OTP> _otpCommandRepository;
        private readonly IDapperQueryRepository<OTP> _otpQueryRepository;
        public AuthenticationService(IUserService userService,
                                     IDapperQueryRepository<User> userQueryRepository,
                                     IMapper mapper,
                                     ITokenService tokenService,
                                     IHttpContextAccessor httpContextAccessor,
                                     IConfiguration configuration,
                                     IAuthenticationValidator validate,
                                     INotificationService notifyMs,
                                     IOtpService otpService,
                                     IDapperCommandRepository<OTP> otpCommandRepository,
                                     IDapperQueryRepository<OTP> otpQueryRepository)
        {
            _userService = userService;
            _userQueryRepository = userQueryRepository;
            _mapper = mapper;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _lockoutDuration = _configuration.GetValue<int>("LockOutSettings:LockOutDuration", defaultValue: 15);
            _otpExpiryMinutes = _configuration.GetValue<int>("OtpSettings:ExpirationMinutes", defaultValue: 3);
            _validate = validate;
            _notifyMs = notifyMs;
            _otpService = otpService;
            _otpCommandRepository = otpCommandRepository;
            _otpQueryRepository = otpQueryRepository;
        }
        public async Task<BaseResponse<OtpViewModel>> SendOtp(SendOtpRequestModel request)
        {
            try
            {
                var validationResult = await _validate.ValidateSendOtpRequestAsync(request);
                if (!validationResult.IsValid)
                {
                    Log.Information($"OTP send request validation failed for email: {request.Email}. " +
                                                                  $"Validation errors: {string.Join(" | ", validationResult.Errors)}");
                    return BaseResponse<OtpViewModel>
                        .Failure(string.Join(" | ", validationResult.Errors), StatusCodes.ModelValidationError);
                }
                if (request.AuthType == EmailTemplates.AdminForgotPassword)
                {
                    var defaultPassword = StringExtension.GenerateAlhaNumericString(8);
                    var user = await _userService.FindByEmailAsync(request.Email);
                    if (user == null)
                    {
                        Log.Information($"User not found for email: {request.Email}");
                        return BaseResponse<OtpViewModel>
                            .Failure(ResponseMessages.NoRecordFound, StatusCodes.NoRecordFound);
                    }
                    user.DefaultPassword = defaultPassword;
                    await _userService.UpdateAsync(user);
                    await _notifyMs.SendEmailAsync(request.Email, EmailTemplates.AdminForgotPassword.ToString(), defaultPassword);
                }
                else
                {
                    var otp = _otpService.GenerateOTP();
                    var otpDetails = _mapper.Map<OTP>(request);
                    if (otpDetails == null)
                    {
                        Log.Error($"Failed to map SendOtpRequestModel to OTP for email: {request.Email}");
                        return BaseResponse<OtpViewModel>.Failure("Mapping failed", StatusCodes.GeneralError);
                    }
                    otpDetails.UserId = Guid.NewGuid();
                    otpDetails.Otp = otp;

                    var activeOtp = await GetActiveOtpAsync(request.Email);
                    if (activeOtp != null)
                    {
                        // Calculate the remaining time
                        var timeElapsed = DateTime.UtcNow - activeOtp.CreationDate;
                        var remainingTime = _otpExpiryMinutes - timeElapsed.TotalMinutes;

                        // Ensure remaining time is not negative
                        remainingTime = Math.Max(remainingTime, 0);

                        var message = $"An OTP has already been sent to {request.Email}. Please try again after {remainingTime:F0} minutes.";

                        Log.Information("Attempt to request a new OTP for email: {Email}. Existing OTP created at {CreationDate}. " +
                            "New OTP request denied due to active OTP.", request.Email, activeOtp.CreationDate);

                        return BaseResponse<OtpViewModel>.Failure(message, StatusCodes.TooManyRequests);
                    }
                    else
                    {
                        Log.Information($"Adding new OTP record for email: {request.Email}.");
                        await _otpCommandRepository.AddAsync(otpDetails);
                    }

                    //await _otpService.SendEmailAsync(request.Email, "Verify Email Address", otp);
                    if (request.AuthType == EmailTemplates.SendOtp)
                        await _notifyMs.SendEmailAsync(request.Email, EmailTemplates.SendOtp.ToString(), otp);

                    if (request.AuthType == EmailTemplates.ForgotPassword)
                        await _notifyMs.SendEmailAsync(request.Email, EmailTemplates.ForgotPassword.ToString(), otp);
                }

                Log.Information($"OTP sent successfully to email: {request.Email}.");

                return BaseResponse<OtpViewModel>
                    .Success(ResponseMessages.OperationSuccessful, StatusCodes.Successful, null /*_mapper.Map<OtpViewModel>(nOtp)*/);
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred: {ex.Message} \n StackTrace: {ex.StackTrace}");

                return BaseResponse<OtpViewModel>.Failure(ResponseMessages.GeneralError, StatusCodes.FatalError);
            }
        }


        public async Task<BaseResponse<OtpViewModel>> VerifyOtp(VerifyOtpRequestModel request)
        {
            try
            {
                var validationResult = await _validate.ValidateOtpRequestAsync(request);
                if (!validationResult.IsValid)
                {
                    Log.Information($"OTP request validation failed for email: {request.Email}. Validation errors: {string.Join(" | ", validationResult.Errors)}");
                    return BaseResponse<OtpViewModel>
                        .Failure(string.Join(" | ", validationResult.Errors), StatusCodes.ModelValidationError);
                }

                var existingOtp = await OtpRequestQuery(request.Email, request.Otp);
                if (existingOtp == null)
                {
                    Log.Information($"No matching OTP found for email: {request.Email} and OTP: {request.Otp}");
                    return BaseResponse<OtpViewModel>
                        .Failure(ResponseMessages.NoRecordFound, StatusCodes.NoRecordFound);
                }

                if (existingOtp.IsUsed)
                {
                    Log.Information($"OTP has already been used for email: {request.Email} and OTP: {request.Otp}.");
                    return BaseResponse<OtpViewModel>
                       .Failure(ResponseMessages.OtpAlreadyUsed, StatusCodes.BadRequest);
                }
                if (existingOtp.IsExpired(_otpExpiryMinutes))
                {
                    Log.Information($"OTP expired for email: {request.Email} and OTP: {request.Otp}.");

                    return BaseResponse<OtpViewModel>
                       .Failure(ResponseMessages.TimeoutOrExpired, StatusCodes.TimeoutOrExpired);
                }

                var existingUser = await _userService.FindByEmailAsync(request.Email);
                if (existingUser != null && existingUser.IsActive)
                {
                    Log.Information($"OTP verification successful for email: {request.Email}");

                    // Mark the OTP as used
                    existingOtp.IsUsed = true;
                    await _otpCommandRepository.UpdateAsync(existingOtp);

                    return BaseResponse<OtpViewModel>
                   .Success(ResponseMessages.OperationSuccessful, StatusCodes.Successful, null);
                }
                if (existingUser != null && (!existingUser.EmailConfirmed || existingUser.EmailConfirmed))
                {
                    Log.Information($"Duplicate record, email already exists: {request.Email}");
                    return BaseResponse<OtpViewModel>
                   .Failure(ResponseMessages.DuplicateRecord, StatusCodes.DuplicateRecord);
                }

                var newUser = new User
                {
                    Id = existingOtp.UserId,
                    Email = existingOtp.Email,
                    IsActive = false,
                    EmailConfirmed = true,
                    UserName = existingOtp.UserId.ToString(),
                    CreationDate = DateTime.UtcNow,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                var createUserResult = await _userService.CreateAsync(newUser);
                if (!createUserResult)
                {
                    Log.Information($"Failed to create user for email: {request.Email}.");
                    return BaseResponse<OtpViewModel>.Failure(ResponseMessages.GeneralError, StatusCodes.GeneralError);
                }

                // Mark the OTP as used after successful user creation
                existingOtp.IsUsed = true;
                await _otpCommandRepository.UpdateAsync(existingOtp);
                Log.Information($"OTP verification successful for email: {request.Email}. User created successfully.");

                return BaseResponse<OtpViewModel>
                    .Success(ResponseMessages.OperationSuccessful, StatusCodes.Successful, null);
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred: {ex.Message} \n StackTrace: {ex.StackTrace}");

                return BaseResponse<OtpViewModel>.Failure(ResponseMessages.GeneralError, StatusCodes.FatalError);
            }
        }




        /* public async Task<BaseResponse<DefaultPassViewModel>> SendDefaultPassword(SendDefaultPassRequest request)
         {
             try
             {
                 var validationResult = await _validate.ValidateDefaultPassRequestAsync(request);
                 if (!validationResult.IsValid)
                 {
                     Log.Information($"Request validation failed for email: {request.Email}. " +
                                                                   $"Validation errors: {string.Join(" | ", validationResult.Errors)}");
                     return BaseResponse<DefaultPassViewModel>
                         .Failure(string.Join(" | ", validationResult.Errors), StatusCodes.ModelValidationError);
                 }
                 string defaultPass;
                 var existingUser = await IsDuplicateRecord(request.Email);
                 if (existingUser != null && request.AuthType == EmailTemplates.APForgotPassword)
                 {
                     defaultPass = StringExtension.GenerateAlhaNumericString(8);
                     existingUser.DefaultPassword = defaultPass;
                     await _userService.UpdateAsync(existingUser);

                     await _notifyMs.SendEmailAsync(request.Email, EmailTemplates.APForgotPassword.ToString(), defaultPass);

                     return BaseResponse<DefaultPassViewModel>
                    .Success(ResponseMessages.OperationSuccessful, StatusCodes.Successful, null *//*_mapper.Map<OtpViewModel>(nOtp)*//*);
                 }
                 if (existingUser != null)
                 {
                     //resend the mail containing the defaultpass

                     defaultPass = StringExtension.GenerateAlhaNumericString(8);

                     existingUser.DefaultPassword = defaultPass;
                     await _userService.UpdateAsync(existingUser);
                     await _notifyMs.SendEmailAsync(request.Email, EmailTemplates.APDefaultPasssword.ToString(), defaultPass);

                     return BaseResponse<DefaultPassViewModel>
                    .Success(ResponseMessages.OperationSuccessful, StatusCodes.Successful, null *//*_mapper.Map<OtpViewModel>(nOtp)*//*);
                 }
                 defaultPass = StringExtension.GenerateAlhaNumericString(8);
                 var newUser = new User
                 {
                     Email = request.Email,
                     DefaultPassword = defaultPass,
                     IsActive = false,
                     EmailConfirmed = false,
                     IsPasswordUpdated = false,
                     AccountStatus = AccountStatus.Pending.ToString(),
                     SecurityStamp = Guid.NewGuid().ToString()
                 };
                 var createUserResult = await _userService.CreateAsync(newUser);
                 if (!createUserResult)
                 {
                     Log.Information($"Failed to create user for email: {request.Email}.");
                     return BaseResponse<DefaultPassViewModel>.Failure(ResponseMessages.GeneralError, StatusCodes.GeneralError);
                 }

                 if (request.AuthType == EmailTemplates.APDefaultPasssword)
                     await _notifyMs.SendEmailAsync(request.Email, EmailTemplates.APDefaultPasssword.ToString(), defaultPass);

                 if (request.AuthType == EmailTemplates.APForgotPassword)
                     await _notifyMs.SendEmailAsync(request.Email, EmailTemplates.APForgotPassword.ToString(), defaultPass);

                 Log.Information($"Default Password sent successfully to email: {request.Email}.");

                 return BaseResponse<DefaultPassViewModel>
                     .Success(ResponseMessages.OperationSuccessful, StatusCodes.Successful, null *//*_mapper.Map<OtpViewModel>(nOtp)*//*);

             }
             catch (Exception ex)
             {
                 Log.Error($"An error occurred: {ex.Message} \n StackTrace: {ex.StackTrace}");

                 return BaseResponse<DefaultPassViewModel>.Failure(ResponseMessages.GeneralError, StatusCodes.FatalError);
             }

         }*/

        public async Task<BaseResponse<SignUpViewModel>> SignUp(SignUpRequestModel request)
        {
            try
            {
                var validationResult = await _validate.ValidateSignUpRequestAsync(request);
                if (!validationResult.IsValid)
                {
                    Log.Information($"Sign-up request validation failed for email: {request.Email}. Validation errors: {string.Join(" | ", validationResult.Errors)}");
                    return BaseResponse<SignUpViewModel>
                        .Failure(string.Join(" | ", validationResult.Errors), StatusCodes.ModelValidationError);
                }

                var user = await _userService.FindByEmailAsync(request.Email);
                if (user == null || !user.EmailConfirmed)
                {
                    Log.Information($"User not found or email not confirmed for email: {request.Email}");
                    return BaseResponse<SignUpViewModel>
                        .Failure(ResponseMessages.VerifyEmail, StatusCodes.NoRecordFound);
                }

                if (user.IsActive)
                {
                    Log.Information($"User already exists for email: {request.Email}");
                    return BaseResponse<SignUpViewModel>
                        .Failure(ResponseMessages.DuplicateRecord, StatusCodes.DuplicateRecord);
                }

                var addPasswordResult = await _userService.AddPasswordAsync(user, request.Password);
                if (!addPasswordResult)
                {
                    Log.Information($"Failed to add password for email");
                    return BaseResponse<SignUpViewModel>.Failure(ResponseMessages.InvalidAttempt, StatusCodes.ModelValidationError);
                }
                /*if (user.IsPasswordUpdated || user.IsActive)
                {
                    Log.Information($"User already exists for email: {request.Email}");
                    return BaseResponse<SignUpViewModel>
                        .Failure(ResponseMessages.DuplicateRecord, StatusCodes.DuplicateRecord);
                }*/

                /* if (user.DefaultPassword != request.DefaultPassword)
                 {
                     return BaseResponse<SignUpViewModel>
                         .Failure(ResponseMessages.WrongCredentials, StatusCodes.Invalid);
                 }*/

                user.Name = request.Name;
                user.UserName = request.Email;
                //user.OrganizationName = request.OrganizationName;
                //user.IsAdmin = request.IsAdmin;
                user.IsActive = true;
                user.LastUpdateDate = DateTime.UtcNow;
                user.SecurityStamp = Guid.NewGuid().ToString();

                await _userService.UpdateAsync(user);
                Log.Information($"User successfully signed up and updated for email: {request.Email}");


                var userView = _mapper.Map<SignUpViewModel>(request);

                return BaseResponse<SignUpViewModel>
                        .Success(ResponseMessages.OperationSuccessful, StatusCodes.Successful, userView);
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred: {ex.Message} \n StackTrace: {ex.StackTrace}");

                return BaseResponse<SignUpViewModel>.Failure(ResponseMessages.GeneralError, StatusCodes.FatalError);
            }

        }

        public async Task<BaseResponse<LoginViewModel>> Login(LoginRequestModel request)
        {
            try
            {
                var validationResult = await _validate.ValidateLoginRequestAsync(request);
                if (!validationResult.IsValid)
                {
                    Log.Information($"Login request validation failed for email: {request.Email}." +
                        $" Validation errors: {string.Join(" | ", validationResult.Errors)}");

                    return BaseResponse<LoginViewModel>
                            .Failure(string.Join(" | ", validationResult.Errors), StatusCodes.ModelValidationError);
                }

                var user = await _userService.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    Log.Information($"User not found for email: {request.Email}");
                    return BaseResponse<LoginViewModel>
                        .Failure(ResponseMessages.NoRecordFound, StatusCodes.NoRecordFound);
                }

                if (user.IsDeactivated)
                {
                    Log.Information($"User account deactivated for email: {request.Email}");
                    return BaseResponse<LoginViewModel>.Failure(ResponseMessages.Deactivated, StatusCodes.ModelValidationError);
                }

                if (!user.IsActive)
                {
                    Log.Information($"User account inactive for email: {request.Email}");
                    return BaseResponse<LoginViewModel>.Failure(ResponseMessages.VerifyEmail, StatusCodes.ModelValidationError);
                }

                /*if (!user.IsPasswordUpdated)
                {
                    Log.Information($"User account inactive for email: {request.Email}");
                    return BaseResponse<LoginViewModel>.Failure(ResponseMessages.VerifyEmail, StatusCodes.ModelValidationError);
                }*/

                if (await _userService.IsLockedOutAsync(user))
                {
                    // Calculate the remaining time
                    var timeElapsed = DateTime.UtcNow - user.LockOutStart;
                    var remainingTime = _lockoutDuration - timeElapsed.TotalMinutes;

                    // Ensure remaining time is not negative
                    remainingTime = Math.Max(remainingTime, 0);

                    var message = $"Account locked for {request.Email}. Please try again after {remainingTime:F0} minutes.";

                    Log.Information("Account locked for email: {Email}.", request.Email);
                    return BaseResponse<LoginViewModel>.Failure(message, StatusCodes.Locked);
                }

                var checkPasswordResult = await _userService.CheckPasswordAsync(user, request.Password);
                if (!checkPasswordResult)
                {
                    Log.Information($"Invalid password for email: {request.Email}");
                    await _userService.LockoutAsync(user); // Increment failed access count and lockout if necessary
                    return BaseResponse<LoginViewModel>.Failure(ResponseMessages.InvalidAttempt, StatusCodes.ModelValidationError);
                }

                user.AccessFailedCount = 0;
                user.IsActive = true;
                user.LastLoginDate = DateTime.UtcNow;
                user.AccountStatus = AccountStatus.Active.ToString();
                //user.SecurityStamp = Guid.NewGuid().ToString();
                await _userService.UpdateAsync(user);

                var response = _mapper.Map<LoginViewModel>(user);
                var (token, refreshToken) = await _tokenService.GenerateTokenAsync(user, request.RememberMe);
                response.AccessToken = token;
                response.RefreshToken = refreshToken;

                Log.Information($"User signed in successfully for email: {request.Email}");

                return BaseResponse<LoginViewModel>
                            .Success(ResponseMessages.OperationSuccessful, StatusCodes.Successful, response);

            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred: {ex.Message} \n StackTrace: {ex.StackTrace}");

                return BaseResponse<LoginViewModel>.Failure(ResponseMessages.GeneralError, StatusCodes.FatalError);
            }
        }

        public async Task<BaseResponse<PasswordViewModel>> ResetPassword(ResetPasswordRequestModel request)
        {
            try
            {
                var validationResult = await _validate.ValidateResetPasswordRequestAsync(request);
                if (!validationResult.IsValid)
                {
                    Log.Information($"Password reset request validation failed for email: {request.Email}. " +
                                                  $"Validation errors: {string.Join(" | ", validationResult.Errors)}");
                    return BaseResponse<PasswordViewModel>
                            .Failure(string.Join(" | ", validationResult.Errors), StatusCodes.ModelValidationError);
                }

                var user = await _userService.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    Log.Information($"No user found for email: {request.Email}");
                    return BaseResponse<PasswordViewModel>.Failure(ResponseMessages.NoRecordFound, StatusCodes.NoRecordFound);
                }
                /* if (user.DefaultPassword != request.DefaultPassword)
                 {
                     return BaseResponse<PasswordViewModel>
                         .Failure(ResponseMessages.WrongCredentials, StatusCodes.Invalid);
                 }*/

                var resetResult = await _userService.AddPasswordAsync(user, request.Password);
                if (!resetResult)
                {
                    Log.Information($"Failed to reset password for email: {request.Email}");
                    return BaseResponse<PasswordViewModel>.Failure(ResponseMessages.InvalidAttempt, StatusCodes.ModelValidationError);
                }

                var response = _mapper.Map<PasswordViewModel>(user);
                Log.Information($"Password reset successfully for email: {request.Email}");
                return BaseResponse<PasswordViewModel>
                    .Success(ResponseMessages.OperationSuccessful, StatusCodes.Successful, response);

            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred: {ex.Message} \n StackTrace: {ex.StackTrace}");

                return BaseResponse<PasswordViewModel>.Failure(ResponseMessages.GeneralError, StatusCodes.FatalError);
            }
        }

        public async Task<BaseResponse<PasswordViewModel>> ChangePassword(ChangePasswordRequestModel request, ClaimsPrincipal userClaims)
        {
            try
            {
                var validClaim = await _tokenService.ValidateSecurityStamp(userClaims);
                if (validClaim == null)
                {
                    Log.Information($"Invalid token");
                    return BaseResponse<PasswordViewModel>.Failure(ResponseMessages.InvalidToken, StatusCodes.Invalid);
                }

                var userEmail = validClaim.FindFirstValue(ClaimTypes.Email);

                var validationResult = await _validate.ValidateChangePasswordRequestAsync(request);
                if (!validationResult.IsValid)
                {
                    Log.Information($"Password change request validation failed for email: {userEmail}. Validation errors: {string.Join(" | ", validationResult.Errors)}");
                    return BaseResponse<PasswordViewModel>
                            .Failure(string.Join(" | ", validationResult.Errors), StatusCodes.ModelValidationError);
                }

                var user = await _userService.FindByEmailAsync(userEmail);
                if (user == null || !user.IsActive)
                {
                    Log.Information($"No user found for email: {userEmail}");
                    return BaseResponse<PasswordViewModel>.Failure(ResponseMessages.NoRecordFound, StatusCodes.NoRecordFound);
                }

                var result = await _userService.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
                if (!result)
                {
                    Log.Information($"Failed to change password for email: {userEmail}");
                    return BaseResponse<PasswordViewModel>.Failure(ResponseMessages.InvalidAttempt, StatusCodes.ModelValidationError);
                }
                var response = _mapper.Map<PasswordViewModel>(user);
                Log.Information($"Password changed successfully for email: {userEmail}");
                return BaseResponse<PasswordViewModel>
                    .Success(ResponseMessages.OperationSuccessful, StatusCodes.Successful, response);

            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred: {ex.Message} \n StackTrace: {ex.StackTrace}");

                return BaseResponse<PasswordViewModel>.Failure(ResponseMessages.GeneralError, StatusCodes.FatalError);
            }
        }

        public async Task<BaseResponse<LogoutViewModel>> Logout()
        {
            try
            {
                var token = GetToken();
                var validationResult = _validate.ValidateToken(token);
                if (!validationResult.IsValid)
                    return BaseResponse<LogoutViewModel>
                        .Failure(validationResult.Message, StatusCodes.ModelValidationError);

                var username = _tokenService.GetUsernameFromExpiredToken(token);
                if (username == null)
                {
                    Log.Information($"Invalid token provided for logout.");
                    return BaseResponse<LogoutViewModel>.Failure(ResponseMessages.InvalidAttempt, StatusCodes.ModelValidationError);
                }

                var query = new Dictionary<string, string>
                {
                    { nameof(User.UserName), username }
                };

                var user = await _userQueryRepository.GetByDefaultAsync(query);
                if (user == null)
                {
                    Log.Information($"User not found for username: {username}");
                    return BaseResponse<LogoutViewModel>.Failure(ResponseMessages.NoRecordFound, StatusCodes.NoRecordFound);
                }

                if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
                {
                    return BaseResponse<LogoutViewModel>.Failure(ResponseMessages.AccountLocked, StatusCodes.Locked);
                }

                // Invalidate the refresh token
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = DateTime.MinValue;
                user.SecurityStamp = Guid.NewGuid().ToString();

                // Update the user
                var result = await _userService.UpdateAsync(user);
                if (!result)
                {
                    Log.Error($"Failed to update user during logout for username: {username}");
                    return BaseResponse<LogoutViewModel>
                        .Failure(ResponseMessages.GeneralError, StatusCodes.GeneralError);
                }

                Log.Information($"User successfully logged out for username: {username}");

                var view = _mapper.Map<LogoutViewModel>(user);

                return BaseResponse<LogoutViewModel>
                    .Success(ResponseMessages.OperationSuccessful, StatusCodes.Successful, view);
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred during logout: {ex.Message} \n StackTrace: {ex.StackTrace}");
                return BaseResponse<LogoutViewModel>.Failure(ResponseMessages.GeneralError, StatusCodes.GeneralError);
            }
        }

        private async Task<User> IsDuplicateRecord(string email)
        {
            var queryFilters = new Dictionary<string, string>
            {
                {nameof(User.Email), email}
            };

            var existingUser = await _userQueryRepository.GetByDefaultAsync(queryFilters);

            return existingUser;
        }

        private string GetToken()
        {
            var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return null;
            }

            var token = authHeader["Bearer ".Length..].Trim(); // Extract the token
            return token;
        }

        private async Task<OTP> OtpRequestQuery(string email, string otp)
        {
            var queryFilters = new Dictionary<string, string>
            {
                    {nameof(OTP.Email), email},
                    { nameof(OTP.Otp), otp }
            };
            var result = await _otpQueryRepository.GetByDefaultAsync(queryFilters);

            return result;
        }

        private async Task<OTP> GetActiveOtpAsync(string email)
        {
            var queryFilters = new Dictionary<string, string>
            {
                {nameof(OTP.Email), email}
            };

            var existingOtps = await _otpQueryRepository.GetByAsync(queryFilters, int.MaxValue, 1);

            // Return the first OTP if it exists and is not expired
            return existingOtps.FirstOrDefault(otp => !otp.IsExpired(_otpExpiryMinutes) && !otp.IsUsed);
        }

    }
}
