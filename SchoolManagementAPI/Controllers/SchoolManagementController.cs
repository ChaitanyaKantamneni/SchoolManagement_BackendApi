using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using SchoolManagementAPI.DAL;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using static SchoolManagementAPI.DAL.SchoolManagementDAL;

namespace SchoolManagementAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolManagementController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SchoolManagementDAL dbop;
        private readonly ILogger<SchoolManagementController> _logger;

        public SchoolManagementController(
            IConfiguration configuration,
            ILogger<SchoolManagementController> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var connectionString = _configuration.GetConnectionString("DefaultConnection")
                                   ?? throw new ArgumentNullException("Connection string not found");

            dbop = new SchoolManagementDAL(connectionString);
        }

        [AllowAnonymous]
        [HttpPost("Tbl_Users_CRUD_Operations")]
        public async Task<IActionResult> Tbl_Users_CRUD_Operations([FromForm] tblUsers user, [FromForm] List<IFormFile>? files)
        {
            try
            {

                if ((user.Flag == "1" || user.Flag == "7") && files != null && files.Count > 0)
                {
                    if (files.Count > 1)
                        return BadRequest("Only one image can be uploaded.");

                    var file = files[0];
                    if (file.Length == 0)
                        return BadRequest("Invalid file.");

                    string[] allowedTypes = { "image/jpeg", "image/png", "image/gif", "image/bmp" };
                    if (!allowedTypes.Contains(file.ContentType))
                        return BadRequest("Invalid image format.");

                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "images");
                    Directory.CreateDirectory(uploadsFolder); // Will not recreate if exists

                    string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    string relativePath = Path.Combine("images", uniqueFileName).Replace("\\", "/");

                    user.FileName = uniqueFileName;
                    user.FilePath = "/" + relativePath;
                }
                var result = dbop.Tbl_Users_CRUD_Operations(user);

                if (result == null || result.Count == 0)
                {
                    return BadRequest(new { StatusCode = 400, Message = "No result returned or operation failed." });
                }

                // Check for any user with error status
                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return BadRequest(new { StatusCode = 400, Message = error.Status });
                }

                if (user.Flag == "4")
                {
                    var dbUser = result[0];
                    if (string.IsNullOrEmpty(dbUser.RollId))
                        return Unauthorized(new { message = "Invalid credentials" });

                    var tokenService = new TokenService(_configuration);
                    var (accessToken, refreshToken, accessExpiry, refreshExpiry) = tokenService.GenerateTokens(
                        dbUser.Email,
                        $"{dbUser.FirstName} {dbUser.LastName}",
                        dbUser.RollId
                    );

                    // Check for existing valid token (latest by email)
                    var existingToken = dbop.GetUserTokenByRefresh(dbUser.Email);
                    if (existingToken != null && existingToken.AccessExpiry > DateTime.UtcNow)
                    {
                        return Ok(new
                        {
                            StatusCode = 200,
                            Success = true,
                            Message = dbUser.Status,
                            Data = result,
                            token = existingToken.AccessToken,
                            refreshToken = existingToken.RefreshToken,
                            role = dbUser.RollId,
                            email = dbUser.Email
                        });
                    }

                    // Revoke old token if exists
                    if (existingToken != null)
                        dbop.RevokeUserToken(existingToken.RefreshToken);

                    // Insert new token in DB
                    dbop.InsertUserToken(dbUser.Email, accessToken, refreshToken, accessExpiry, refreshExpiry);

                    return Ok(new
                    {
                        accessToken,
                        refreshToken,
                        role = dbUser.RollId,
                        email = dbUser.Email
                    });
                }




                // Optional: You can trigger mail only on registration (Flag = "1")
                //if (user.Flag == "1")
                //{
                //    SendRegistrationEmailAsync(user.Email, user.FirstName, user.Password, false);
                //    SendRegistrationEmailAsync(user.Email, user.FirstName, user.Password, true);
                //}
                //if (user.Flag == "5")
                //{
                //    var dbResult = result.FirstOrDefault();

                //    if (dbResult == null || dbResult.Status?.ToString() != "UserExists")
                //    {
                //        return NotFound(new { Message = "User not found or inactive." });
                //    }

                //    string email = dbResult.Email;
                //    string name = dbResult.FirstName;

                //    if (string.IsNullOrEmpty(email))
                //    {
                //        return BadRequest(new { Message = "Email is missing in the user record." });
                //    }

                //    // Rate limit check
                //    if (_otpCache.TryGetValue(email, out var existingOtp))
                //    {
                //        var sentTimeApprox = existingOtp.ExpiryTime.AddMinutes(-10);
                //        if ((DateTime.Now - sentTimeApprox).TotalSeconds < 60)
                //        {
                //            return BadRequest(new { Message = "OTP already sent. Please wait a minute before requesting again." });
                //        }
                //    }

                //    var otp = GenerateOTP(6);
                //    var expiryTime = DateTime.Now.AddMinutes(10);

                //    _otpCache[email] = new UserOTP
                //    {
                //        Email = email,
                //        OTP = otp,
                //        ExpiryTime = expiryTime
                //    };

                //    var emailBody = $"Hi {name},\n\nYour OTP for password reset is: {otp}\n\nThis OTP is valid for 10 minutes.";
                //    await SendEmailOtpAsync(email, "Your OTP for Password Reset", emailBody);

                //    return Ok(new { StatusCode = 200, Message = "OTP sent successfully to your email." });
                //}

                return Ok(new
                {
                    StatusCode = 200,
                    Success = true,
                    Message = result.First().Status,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Success = false,
                    Message = "Internal server error.",
                    Error = ex.Message
                });
            }
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.RefreshToken))
                return Unauthorized();

            var tokenRecord = dbop.GetUserTokenByRefresh(request.Email, request.RefreshToken);

            if (tokenRecord == null)
                return Unauthorized(new { message = "Invalid refresh token" });

            if (tokenRecord.RefreshExpiry < DateTimeHelper.NowIST())
                return Unauthorized(new { message = "Refresh token expired" });

            var tokenService = new TokenService(_configuration);
            var (newAccess, newRefresh, accessExpiry, refreshExpiry) =
                tokenService.GenerateTokens(
                    tokenRecord.Email,
                    "",
                    ""
                );

            dbop.RevokeUserToken(tokenRecord.RefreshToken);

            dbop.InsertUserToken(
                tokenRecord.Email,
                newAccess,
                newRefresh,
                accessExpiry,
                refreshExpiry
            );

            return Ok(new
            {
                accessToken = newAccess,
                refreshToken = newRefresh
            });
        }

        [HttpPost("Tbl_Roles_CRUD_Operations")]
        public IActionResult Tbl_Roles_CRUD_Operations([FromBody] tblRoles role)
        {
            try
            {
                var result = dbop.Tbl_Roles_CRUD_Operations(role);

                if (result == null || result.Count == 0)
                {
                    return BadRequest(new { StatusCode = 400, Message = "No result returned or operation failed." });
                }

                // Check for any error status
                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return BadRequest(new { StatusCode = 400, Message = error.Status });
                }

                return Ok(new { StatusCode = 200, Success = true, Message = result.First().Status, Data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { StatusCode = 400, Success = false, Message = "Internal server error.", Error = ex.Message });
            }
        }

        [HttpPost("Tbl_UserRoles_CRUD_Operations")]
        public IActionResult Tbl_UserRoles_CRUD_Operations([FromBody] tblUserRoles userRole)
        {
            try
            {
                var result = dbop.Tbl_UserRoles_CRUD_Operations(userRole);

                if (result == null || result.Count == 0)
                {
                    return BadRequest(new { StatusCode = 400, Message = "No result returned or operation failed." });
                }

                // Check for any error status
                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return BadRequest(new { StatusCode = 400, Message = error.Status });
                }

                return Ok(new { StatusCode = 200, Success = true, Message = result.First().Status, Data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { StatusCode = 400, Success = false, Message = "Internal server error.", Error = ex.Message });
            }
        }

        [HttpPost("Tbl_Modules_CRUD_Operations")]
        public IActionResult Tbl_Modules_CRUD_Operations([FromBody] tblModules module)
        {
            try
            {
                var result = dbop.Tbl_Modules_CRUD_Operations(module);

                if (result == null || result.Count == 0)
                {
                    return BadRequest(new { StatusCode = 400, Message = "No result returned or operation failed." });
                }

                // Check for any error status
                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return BadRequest(new { StatusCode = 400, Message = error.Status });
                }

                return Ok(new { StatusCode = 200, Success = true, Message = result.First().Status, Data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { StatusCode = 400, Success = false, Message = "Internal server error.", Error = ex.Message });
            }
        }

        [HttpPost("Tbl_Pages_CRUD_Operations")]
        public IActionResult Tbl_Pages_CRUD_Operations([FromBody] tblPages page)
        {
            try
            {
                var result = dbop.Tbl_Pages_CRUD_Operations(page);

                if (result == null || result.Count == 0)
                {
                    return BadRequest(new { StatusCode = 400, Message = "No result returned or operation failed." });
                }

                // Check for any error status
                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return BadRequest(new { StatusCode = 400, Message = error.Status });
                }

                return Ok(new { StatusCode = 200, Success = true, Message = result.First().Status, Data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { StatusCode = 400, Success = false, Message = "Internal server error.", Error = ex.Message });
            }
        }

        [HttpPost("Tbl_RolePermissions_CRUD_Operations")]
        public IActionResult Tbl_RolePermissions_CRUD_Operations([FromBody] List<tblRolePermissions> rolePerms)
        {
            try
            {
                // Call the CRUD operation with the list of role permissions
                var result = dbop.Tbl_RolePermissions_CRUD_Operations(rolePerms);

                if (result == null || result.Count == 0)
                {
                    return BadRequest(new { StatusCode = 400, Message = "No result returned or operation failed." });
                }

                // Check for any error status in the result
                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return BadRequest(new { StatusCode = 400, Message = error.Status });
                }

                return Ok(new { StatusCode = 200, Success = true, Message = result.First().Status, Data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { StatusCode = 400, Success = false, Message = "Internal server error.", Error = ex.Message });
            }
        }

        [HttpGet("Tbl_GetRoleMenuPermissions/{roleId}")]
        public IActionResult Tbl_GetRoleMenuPermissions(string roleId)
        {
            try
            {
                var result = dbop.Tbl_GetRoleMenuPermissions(roleId);

                if (result == null || result.Count == 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = "No menu data found for this role."
                    });
                }

                // Error handling if DAL returned error message
                if (result.Any(m => m.ModuleName == "ERROR"))
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Description
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Success = true,
                    Message = "Menu loaded successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Success = false,
                    Message = "Internal server error",
                    Error = ex.Message
                });
            }
        }

        [HttpPost("GetPermissionsByRole")]
        public IActionResult GetPermissionsByRole([FromBody] tblRolePermissions model)
        {
            try
            {
                // Wrap the single model into a list (as expected by the method)
                var list = new List<tblRolePermissions> { model };

                // Call the DAL method (database operation)
                var result = dbop.Tbl_RolePermissions_CRUD_Operations(list);

                // Return the result in the expected format
                return Ok(new { StatusCode = 200, Data = result });
            }
            catch (Exception ex)
            {
                // Return an error message if anything goes wrong
                return BadRequest(new { StatusCode = 400, Message = "Internal server error", Error = ex.Message });
            }
        }

        //    private async Task SendRegistrationEmailAsync(string toEmail, string userName, string userPassword, bool isAdmin)
        //    {
        //        string actualRecipient = isAdmin ? "chaitanyakantamneni6@gmail.com" : toEmail;
        //        string subject = isAdmin ? "New User Registered" : "Welcome to Buserele Family!";
        //        string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "emaillog.jpg");
        //        string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "buserelelog.jpg");

        //        var emailMessage = new MimeMessage();
        //        emailMessage.From.Add(new MailboxAddress("BUSERELE Property Management", _configuration["Smtp:Username"]));
        //        emailMessage.To.Add(MailboxAddress.Parse(actualRecipient));
        //        emailMessage.Subject = subject;

        //        var builder = new BodyBuilder();

        //        var headerImage = builder.LinkedResources.Add(imagePath);
        //        headerImage.ContentId = "CompanyLogo";
        //        var footerLogo = builder.LinkedResources.Add(logoPath);
        //        footerLogo.ContentId = "BusereleLogo";

        //        string htmlBody = isAdmin
        //            ? $@"
        //    <html>
        //    <head>
        //        <style>
        //            body {{ font-family: Arial, sans-serif; }}
        //            .container {{ text-align: center; padding: 20px; background-color: #000; color: #fff; }}
        //            .content {{ margin: 20px; padding: 20px; background-color: #fff; color: #000; border-radius: 10px; }}
        //            .footer {{ margin-top: 20px; font-size: 12px; }}
        //        </style>
        //    </head>
        //    <body>
        //        <div class='container'>
        //            <div class='content'>
        //                <img src='cid:CompanyLogo' alt='Company Logo' width='200' />
        //                <h2>New User Registration</h2>
        //                <p><strong>Name:</strong> {userName}</p>
        //                <p><strong>Email:</strong> {toEmail}</p>
        //                <img src='cid:BusereleLogo' alt='Buserele Logo' width='150' />
        //            </div>
        //            <p class='footer'>&copy; GVR Infosystems Pvt Ltd 2025. All rights reserved.</p>
        //        </div>
        //    </body>
        //    </html>"
        //            : $@"
        //    <html>
        //    <head>
        //    <style>
        //        body {{ font-family: Arial, sans-serif; }}
        //        .container {{ text-align: center; padding: 20px; background-color: #000; color: #fff; }}
        //        .content {{ margin: 20px; padding: 20px; background-color: #fff; color: #000; border-radius: 10px; }}
        //        .footer {{ margin-top: 20px; font-size: 12px; }}
        //    </style>
        //</head>
        //<body>
        //    <div class='container'>
        //        <img src='cid:CompanyLogo' alt='Company Logo' width='200' />
        //        <h2>Dear {userName},</h2>
        //        <p>Welcome to Beserele Family!</p>
        //        <p>Thank you for registering. Please login using the credentials below:</p>
        //        <div class='content'>
        //            <p><strong>UserID:</strong> {toEmail}</p>
        //            <p><strong>Password:</strong> {userPassword}</p>
        //        </div>
        //        <p>If you have any questions, feel free to contact us.</p>
        //        <p>Visit us at <a href='https://www.buserele.com'>www.buserele.com</a></p>
        //        <p class='footer'>&copy; GVR Infosystems Pvt Ltd 2025. All rights reserved.</p>
        //        <br>
        //        <img src='cid:BusereleLogo' alt='Buserele Logo' width='150' />
        //    </div>
        //</body>
        //    </html>";

        //        builder.HtmlBody = htmlBody;
        //        emailMessage.Body = builder.ToMessageBody();

        //        //try
        //        //{
        //        //    using var smtpClient = new MailKit.Net.Smtp.SmtpClient();
        //        //    await smtpClient.ConnectAsync(_configuration["Smtp:Host"], int.Parse(_configuration["Smtp:Port"]), false);
        //        //    await smtpClient.AuthenticateAsync(_configuration["Smtp:Username"], _configuration["Smtp:Password"]);
        //        //    await smtpClient.SendAsync(emailMessage);
        //        //    await smtpClient.DisconnectAsync(true);
        //        //}
        //        //catch (Exception ex)
        //        //{
        //        //    Console.WriteLine($"Error sending email: {ex.Message}");
        //        //    throw;
        //        //}


        //        try
        //        {
        //            var smtpClient = new System.Net.Mail.SmtpClient(_configuration["Smtp:smtpServer"])
        //            {
        //                Port = int.Parse(_configuration["Smtp:Port"]),
        //                Credentials = new NetworkCredential(_configuration["Smtp:Username"], _configuration["Smtp:Password"]),
        //                EnableSsl = false
        //            };

        //            Console.WriteLine("smtpClient", smtpClient.ToString());

        //            var mailMessage = new MailMessage
        //            {
        //                From = new MailAddress(_configuration["Smtp:FromEmail"], "Support Team"),
        //                Subject = subject,
        //                IsBodyHtml = true,
        //                Body = htmlBody
        //            };
        //            mailMessage.To.Add(actualRecipient);
        //            await smtpClient.SendMailAsync(mailMessage);
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Error sending email: {ex.Message}");
        //            throw ex;
        //        }
        //    }

        //    private static Dictionary<string, UserOTP> _otpCache = new Dictionary<string, UserOTP>();

        //    private static Random random = new Random();

        //    private string GenerateOTP(int length = 6)
        //    {
        //        var otp = new StringBuilder();

        //        for (int i = 0; i < length; i++)
        //        {
        //            otp.Append(random.Next(0, 10));
        //        }

        //        return otp.ToString();
        //    }

        //    private async Task SendEmailOtpAsync(string toEmail, string subject, string body)
        //    {

        //        try
        //        {

        //            if (string.IsNullOrEmpty(toEmail) || !IsValidEmail(toEmail))
        //            {
        //                throw new ArgumentException("Invalid email address.");
        //            }

        //            var smtpClient = new System.Net.Mail.SmtpClient(_configuration["Smtp:smtpServer"])
        //            {
        //                Port = int.Parse(_configuration["Smtp:Port"]),
        //                Credentials = new NetworkCredential(_configuration["Smtp:Username"], _configuration["Smtp:Password"]),
        //                EnableSsl = false
        //            };

        //            var mailMessage = new MailMessage
        //            {
        //                From = new MailAddress(_configuration["Smtp:FromEmail"], "Support Team"),
        //                Subject = subject,
        //                IsBodyHtml = true,
        //                Body = body
        //            };
        //            mailMessage.To.Add(toEmail);
        //            await smtpClient.SendMailAsync(mailMessage);
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Error sending email: {ex.Message}");
        //            throw ex;
        //        }
        //    }

        //    private bool IsValidEmail(string email)
        //    {
        //        try
        //        {
        //            var mailAddress = new System.Net.Mail.MailAddress(email);
        //            return mailAddress.Address == email;
        //        }
        //        catch
        //        {
        //            return false;
        //        }
        //    }

        //    [HttpPost]
        //    [Route("VerifyOTP")]
        //    public IActionResult VerifyOTP([FromBody] VerifyOtpRequest request)
        //    {
        //        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.OTP))
        //        {
        //            return BadRequest(new { Message = "Email and OTP are required." });
        //        }
        //        if (!_otpCache.TryGetValue(request.Email, out UserOTP userOtp))
        //        {
        //            return BadRequest(new { Message = "OTP not found or has expired." });
        //        }
        //        if (userOtp.ExpiryTime < DateTime.Now)
        //        {
        //            _otpCache.Remove(request.Email); // Remove expired OTP
        //            return BadRequest(new { Message = "OTP has expired." });
        //        }
        //        if (userOtp.OTP != request.OTP)
        //        {
        //            return BadRequest(new { Message = "Invalid OTP." });
        //        }
        //        _otpCache.Remove(request.Email);

        //        return Ok(new { StatusCode = 200, Message = "OTP is valid." });
        //    }

        [HttpPost("Tbl_AcademicYear_CRUD_Operations")]
        public IActionResult Tbl_AcademicYear_CRUD_Operations([FromBody] tblAcademicYear academicYear)
        {
            var result = dbop.Tbl_AcademicYear_CRUD_Operations(academicYear);

            if (result == null || result.Count == 0)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Success = false,
                    Message = "No result returned or operation failed."
                });
            }

            var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
            if (error != null)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Success = false,
                    Message = error.Status
                });
            }

            return Ok(new
            {
                StatusCode = 200,
                Success = true,
                Message = result.First().Status,
                Data = result
            });
        }

        [HttpPost("Tbl_Syllabus_CRUD_Operations")]
        public IActionResult Tbl_Syllabus_CRUD_Operations([FromBody] tblSyllabus syllabus)
        {
            var result = dbop.Tbl_Syllabus_CRUD_Operations(syllabus);

            if (result == null || result.Count == 0)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Success = false,
                    Message = "No result returned or operation failed."
                });
            }

            var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
            if (error != null)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Success = false,
                    Message = error.Status
                });
            }

            return Ok(new
            {
                StatusCode = 200,
                Success = true,
                Message = result.First().Status,
                Data = result
            });
        }

        [HttpPost("Tbl_Class_CRUD_Operations")]
        public IActionResult Tbl_Class_CRUD_Operations([FromBody] tblClass Class)
        {
            var result = dbop.Tbl_Class_CRUD_Operations(Class);

            if (result == null || result.Count == 0)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Success = false,
                    Message = "No result returned or operation failed."
                });
            }

            var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
            if (error != null)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Success = false,
                    Message = error.Status
                });
            }

            return Ok(new
            {
                StatusCode = 200,
                Success = true,
                Message = result.First().Status,
                Data = result
            });
        }

        [HttpPost("Tbl_ClassDivision_CRUD_Operations")]
        public IActionResult Tbl_ClassDivision_CRUD_Operations([FromBody] tblClassDivision classDivision)
        {
            var result = dbop.Tbl_ClassDivision_CRUD_Operations(classDivision);

            if (result == null || result.Count == 0)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Success = false,
                    Message = "No result returned or operation failed."
                });
            }

            var statusText = result.First().Status?.ToLower() ?? string.Empty;

            if (classDivision.Flag == "1" && statusText.Contains("classdivision name already exists"))
            {
                return Conflict(new
                {
                    StatusCode = 409,
                    Success = false,
                    Message = result.First().Status
                });
            }

            if (statusText.Contains("error"))
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Success = false,
                    Message = result.First().Status
                });
            }

            return Ok(new
            {
                StatusCode = 200,
                Success = true,
                Message = result.First().Status,
                Data = result
            });
        }

        [HttpPost("Tbl_Staff_CRUD_Operations")]
        public IActionResult Tbl_Staff_CRUD_Operations([FromBody] tblStaff staff)
        {
            var result = dbop.Tbl_Staff_CRUD_Operations(staff);

            if (result == null || result.Count == 0)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Success = false,
                    Message = "No result returned or operation failed."
                });
            }

            var statusText = result.First().Status?.ToLower() ?? string.Empty;

            if (staff.Flag == "1" && statusText.Contains("staff member already exists"))
            {
                return Conflict(new
                {
                    StatusCode = 409,
                    Success = false,
                    Message = result.First().Status
                });
            }

            if (statusText.Contains("error"))
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Success = false,
                    Message = result.First().Status
                });
            }

            return Ok(new
            {
                StatusCode = 200,
                Success = true,
                Message = result.First().Status,
                Data = result
            });
        }

        [HttpPost("Tbl_Subject_CRUD_Operations")]
        public IActionResult Tbl_Subject_CRUD_Operations([FromBody] tblSubjects subject)
        {
            var result = dbop.Tbl_Subjects_CRUD_Operations(subject);

            if (result == null || result.Count == 0)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Success = false,
                    Message = "No result returned or operation failed."
                });
            }

            var statusText = result.First().Status?.ToLower() ?? string.Empty;

            if (statusText.Contains("error"))
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Success = false,
                    Message = result.First().Status
                });
            }

            return Ok(new
            {
                StatusCode = 200,
                Success = true,
                Message = result.First().Status,
                Data = result
            });
        }

        [HttpPost("Tbl_SubjectStaff_CRUD_Operations")]
        public IActionResult Tbl_SubjectStaff_CRUD_Operations([FromBody] tblSubjectStaff subjectStaff)
        {
            var result = dbop.Tbl_SubjectStaff_CRUD_Operations(subjectStaff);

            if (result == null || result.Count == 0)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Success = false,
                    Message = "No result returned or operation failed."
                });
            }

            var statusText = result.First().Status?.ToLower() ?? string.Empty;

            if (statusText.Contains("error"))
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Success = false,
                    Message = result.First().Status
                });
            }

            return Ok(new
            {
                StatusCode = 200,
                Success = true,
                Message = result.First().Status,
                Data = result
            });
        }

        [HttpPost("Tbl_Admissions_CRUD_Operations")]
        public IActionResult Tbl_Admissions_CRUD_Operations([FromBody] tblAdmission admission)
        {
            var result = dbop.Tbl_Admission_CRUD_Operations(admission);

            if (result == null || result.Count == 0)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Success = false,
                    Message = "No result returned or operation failed."
                });
            }

            var statusText = result.First().Status?.ToLower() ?? string.Empty;

            if (statusText.Contains("error"))
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Success = false,
                    Message = result.First().Status
                });
            }

            return Ok(new
            {
                StatusCode = 200,
                Success = true,
                Message = result.First().Status,
                Data = result
            });
        }
    }
}