using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SchoolManagementAPI.DAL;
using SchoolManagementAPI.DB;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.Services;
using System.Data;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using static Org.BouncyCastle.Math.EC.ECCurve;
using static SchoolManagementAPI.DAL.SchoolManagementDAL;

namespace SchoolManagementAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    /// <summary>
    /// SchoolManagementController: Exposes all REST endpoints for academic, finance, and user operations.
    /// Handled via standard ASP.NET HTTP calls mapping straight to Stored Procedures through SchoolManagementDAL.
    /// </summary>
    public class SchoolManagementController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SchoolManagementDAL dbop;
        private readonly IDbContextFactory<SchoolManagementDBContext> _contextFactory;
        private readonly ILogger<SchoolManagementController> _logger;
        private readonly SchoolManagementDBContext _dbContext;
        private readonly FileService _fileService;

        public SchoolManagementController(
            IConfiguration configuration,
            ILogger<SchoolManagementController> logger,
            SchoolManagementDBContext dbContext,
            IDbContextFactory<SchoolManagementDBContext> contextFactory,
            FileService fileService)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var connectionString = _configuration.GetConnectionString("DefaultConnection")
                                   ?? throw new ArgumentNullException("Connection string not found");

            dbop = new SchoolManagementDAL(connectionString);

            _dbContext = dbContext;
            _contextFactory = contextFactory;
            _fileService = fileService;
        }

        [AllowAnonymous]
        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/sendotp
        /// Purpose: Handles transactional and query operations for sendotp.
        /// </summary>
        [HttpPost("sendotp")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
                return BadRequest(new { success = false, message = "Email required" });

            // 🔍 Check user exists
            var userCheck = new TblUser
            {
                Email = request.Email,
                Flag = "3"
            };

            var dbUser = dbop.Tbl_Users_CRUD_Operations(userCheck).FirstOrDefault();

            if (dbUser == null)
                return BadRequest(new { success = false, message = "User not found" });

            var otp = new Random().Next(100000, 999999).ToString();

            // Save OTP via SP
            dbop.UserOTP_Operations(request.Email, otp, "1");

            // Send email
            await SendOtpEmail(request.Email, otp);

            return Ok(new { success = true, message = "OTP sent successfully" });
        }

        [AllowAnonymous]
        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/verifyotp
        /// Purpose: Handles transactional and query operations for verifyotp.
        /// </summary>
        [HttpPost("verifyotp")]
        public IActionResult VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            var dt = dbop.UserOTP_Operations(request.Email, request.OTP, "2");

            if (dt.Rows.Count == 0)
                return BadRequest(new { success = false, message = "Invalid or expired OTP" });

            // Mark OTP as used
            dbop.UserOTP_Operations(request.Email, request.OTP, "3");

            // Get user again
            var user = new TblUser
            {
                Email = request.Email,
                Flag = "3"
            };

            var dbUser = dbop.Tbl_Users_CRUD_Operations(user).FirstOrDefault();

            if (dbUser == null)
                return BadRequest(new { success = false });

            // 🔐 Generate token AFTER OTP
            var tokenService = new TokenService(_configuration);

            string? schoolID = dbUser.RollId != "1" ? dbUser.SchoolID : null;

            var (accessToken, refreshToken, accessExpiryUtc, refreshExpiryUtc) =
                tokenService.GenerateTokens(
                    dbUser.Email,
                    $"{dbUser.FirstName} {dbUser.LastName}",
                    dbUser.RollId,
                    schoolID
                );

            var existingToken = dbop.GetUserTokenByRefresh(dbUser.Email);
            if (existingToken != null)
                dbop.RevokeUserToken(existingToken.RefreshToken);

            dbop.InsertUserToken(
                dbUser.Email,
                accessToken,
                refreshToken,
                accessExpiryUtc,
                refreshExpiryUtc
            );


            return Ok(new
            {
                success = true,
                accessToken,
                refreshToken,
                role = dbUser.RollId,
                email = dbUser.Email,
                schoolId = dbUser.SchoolID,
                schoolRouteName = string.IsNullOrEmpty(dbUser.SchoolName) ? "Admin" : dbUser.SchoolName.Replace(" ", "")
            });
        }

        /// <summary>
        /// Sends an OTP verification email to the user using SMTP credentials.
        /// </summary>
        private async Task SendOtpEmail(string toEmail, string otp)
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Smart Schools ERP", _configuration["Smtp:Username"]));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = "Your Login OTP";

                message.Body = new BodyBuilder
                {
                    HtmlBody = $@"
                <h2>Login OTP Verification</h2>
                <p>Your OTP is:</p>
                <h1 style='color:#2d3093'>{otp}</h1>
                <p>This OTP is valid for 5 minutes.</p>"
                }.ToMessageBody();

                using var smtp = new MailKit.Net.Smtp.SmtpClient();

                await smtp.ConnectAsync(
                    _configuration["Smtp:Host"],
                    int.Parse(_configuration["Smtp:Port"]),
                    MailKit.Security.SecureSocketOptions.Auto
                );

                await smtp.AuthenticateAsync(
                    _configuration["Smtp:Username"],
                    _configuration["Smtp:Password"]
                );

                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);
            }

        /// <summary>
        /// API Endpoint: POST api/SchoolManagement/Tbl_Users_CRUD_Operations
        /// Purpose: Registers new users or updates profile fields (supports profile image file upload).
        /// </summary>
        [AllowAnonymous]
        [HttpPost("Tbl_Users" +
                "_CRUD_Operations")]
        public async Task<IActionResult> Tbl_Users_CRUD_Operations([FromForm] TblUser user, [FromForm] List<IFormFile>? files)
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
                        Directory.CreateDirectory(uploadsFolder);

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

                    if (result == null)
                    {
                        return StatusCode(500, new
                        {
                            StatusCode = 500,
                            Success = false,
                            Message = "Database returned null result."
                        });
                    }

                    var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                    if (error != null)
                    {
                        return StatusCode(500, new
                        {
                            StatusCode = 500,
                            Success = false,
                            Message = error.Status
                        });
                    }

                    if (user.Flag == "4")
                    {
                        var dbUser = result[0];

                        if (string.IsNullOrEmpty(dbUser.RollId))
                            return Unauthorized(new { message = "Invalid credentials" });

                        //if (dbUser.RollId == "1" || dbUser.RollId == "2" || dbUser.RollId == "8")
                        //{
                        //    // NO TOKEN → frontend will trigger OTP
                        //    string schoolRouteName1 = dbUser.SchoolName?.Replace(" ", "");

                        //    return Ok(new
                        //    {
                        //        success = true,
                        //        role = dbUser.RollId,
                        //        email = dbUser.Email,
                        //        schoolId = dbUser.SchoolID,
                        //        schoolName = schoolRouteName1,
                        //        requireOtp = true   // IMPORTANT FLAG
                        //    });
                        //}

                        var tokenService = new TokenService(_configuration);
                        string? schoolID = dbUser.RollId != "1" ? dbUser.SchoolID : null;

                        string? schoolIDs = null;
                        if (dbUser.RollId == "10") // group admin
                        {
                            var ids = dbop.GetStaffSchoolIDs(dbUser.Email);
                            schoolIDs = ids.Count > 0 ? string.Join(",", ids) : null;
                        }

                        var (accessToken, refreshToken, accessExpiryUtc, refreshExpiryUtc) =
                            tokenService.GenerateTokens(
                                dbUser.Email,
                                $"{dbUser.FirstName} {dbUser.LastName}",
                                dbUser.RollId,
                                schoolID,
                                schoolIDs
                            );

                        var existingToken = dbop.GetUserTokenByRefresh(dbUser.Email);
                        if (existingToken != null)
                            dbop.RevokeUserToken(existingToken.RefreshToken);

                        dbop.InsertUserToken(
                            dbUser.Email,
                            accessToken,
                            refreshToken,
                            accessExpiryUtc,
                            refreshExpiryUtc
                        );

                        string schoolRouteName = dbUser.SchoolName.Replace(" ", "");

                        return Ok(new
                        {
                            accessToken,
                            refreshToken,
                            success = true,
                            role = dbUser.RollId,
                            email = dbUser.Email,
                            schoolId = dbUser.SchoolID,
                            schoolIds = schoolIDs,
                            schoolName = schoolRouteName
                        });
                    }
                    else if (user.Flag == "1")
                    {
                        var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                        var schoolId = User.FindFirst("SchoolID")?.Value;

                        if (roleId != "1")
                        {
                            user.SchoolID = schoolId;
                        }
                        string? schoolName = null;
                        string? schoolUrl = null;

                        if (!string.IsNullOrEmpty(user.SchoolID))
                        {
                            var school = GetSchoolById(user.SchoolID);
                            if (school != null)
                            {
                                schoolName = school.Name;
                                schoolUrl = !string.IsNullOrEmpty(school.Website) ? school.Website : "https://www.smartschools.com";
                            }
                        }

                        await SendRegistrationEmailAsync(
                            user.Email,
                            $"{user.FirstName} {user.LastName}",
                            user.Password,
                            isAdmin: false,
                            schoolName,
                            schoolUrl,
                            user.RollId
                        );
                    }


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
                    dbop.LogException(ex, "SchoolManagementController", "Tbl_Users_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(user));
                    return BadRequest(new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Internal server error.",
                        Error = ex.Message
                    });
                }
            }

        /// <summary>
        /// Queries dbop master records to retrieve school settings by ID.
        /// </summary>
        private SchoolDetails? GetSchoolById(string schoolId)
            {
                if (string.IsNullOrEmpty(schoolId))
                    return null;

                var schoolList = dbop.Tbl_SchoolDetails_CRUD(new SchoolDetails
                {
                    Flag = "4",
                    ID = schoolId
                });

                return schoolList.FirstOrDefault();
            }

                /// <summary>
        /// Sends welcome confirmation emails to newly registered school staff.
        /// </summary>
        private async Task SendRegistrationEmailAsync(string toEmail, string userName, string userPassword, bool isAdmin, string? schoolName = null, string? schoolUrl = null, string? Roleid = null)
            {
                if (Roleid == "5")
                {
                    //            string actualRecipient = isAdmin ? "chaitanyakantamneni6@gmail.com" : toEmail;
                    //            string subject = isAdmin ? "New Student Registered" : "Welcome to Smart Schools ERP";

                    //            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "emaillog.jpg");
                    //            string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "buserelelog.jpg");

                    //            var emailMessage = new MimeMessage();
                    //            emailMessage.From.Add(new MailboxAddress("Smart Schools ERP", _configuration["Smtp:Username"]));
                    //            emailMessage.To.Add(MailboxAddress.Parse(actualRecipient));
                    //            emailMessage.Subject = subject;

                    //            var builder = new BodyBuilder();

                    //            // Attach images only if they exist
                    //            if (System.IO.File.Exists(imagePath))
                    //            {
                    //                var headerImage = builder.LinkedResources.Add(imagePath);
                    //                headerImage.ContentId = "CompanyLogo";
                    //            }

                    //            if (System.IO.File.Exists(logoPath))
                    //            {
                    //                var footerLogo = builder.LinkedResources.Add(logoPath);
                    //                footerLogo.ContentId = "FooterLogo";
                    //            }

                    //            // Choose URL and footer based on school
                    //            string loginUrl = !string.IsNullOrEmpty(schoolUrl) ? schoolUrl : "https://www.smartschools.com";
                    //            string loginText = !string.IsNullOrEmpty(schoolName) ? $"Please login at <a href='{loginUrl}'>{schoolName}</a>" : $"Please login at <a href='{loginUrl}'>www.smartschools.com</a>";
                    //            string footerText = !string.IsNullOrEmpty(schoolName) ? $"&copy; {schoolName} 2025. All rights reserved." : "&copy; Smart Schools ERP 2025. All rights reserved.";

                    //            string htmlBody = isAdmin
                    //                ? $@"
                    //<html>
                    //<head>
                    //    <style>
                    //        body {{ font-family: Arial, sans-serif; }}
                    //        .container {{ text-align: center; padding: 20px; background-color: #f0f0f0; }}
                    //        .content {{ margin: 20px; padding: 20px; background-color: #ffffff; border-radius: 10px; }}
                    //        .footer {{ margin-top: 20px; font-size: 12px; color: #555555; }}
                    //    </style>
                    //</head>
                    //<body>
                    //    <div class='container'>
                    //        <div class='content'>
                    //            {(System.IO.File.Exists(imagePath) ? "<img src='cid:CompanyLogo' alt='Smart Schools ERP Logo' width='200' />" : "")}
                    //            <h2>New Student Registration</h2>
                    //            <p><strong>Name:</strong> {userName}</p>
                    //            <p><strong>Email:</strong> {toEmail}</p>
                    //            {(System.IO.File.Exists(logoPath) ? "<img src='cid:FooterLogo' alt='Smart Schools ERP Logo' width='150' />" : "")}
                    //        </div>
                    //        <p class='footer'>{footerText}</p>
                    //    </div>
                    //</body>
                    //</html>"
                    //                : $@"
                    //<html>
                    //<head>
                    //    <style>
                    //        body {{ font-family: Arial, sans-serif; }}
                    //        .container {{ text-align: center; padding: 20px; background-color: #f0f0f0; }}
                    //        .content {{ margin: 20px; padding: 20px; background-color: #ffffff; border-radius: 10px; }}
                    //        .footer {{ margin-top: 20px; font-size: 12px; color: #555555; }}
                    //        .credentials {{ text-align: left; display: inline-block; margin-top: 10px; }}
                    //    </style>
                    //</head>
                    //<body>
                    //    <div class='container'>
                    //        {(System.IO.File.Exists(imagePath) ? "<img src='cid:CompanyLogo' alt='Smart Schools ERP Logo' width='200' />" : "")}
                    //        <h2>Welcome, {userName}!</h2>
                    //        <p>Congratulations! Your Student account has been successfully created in <strong>Smart Schools ERP</strong>.</p>
                    //        <div class='content'>
                    //            <h3>Login Details:</h3>
                    //            <div class='credentials'>
                    //                <p><strong>Email / UserID:</strong> {toEmail}</p>
                    //                <p><strong>Password:</strong> {userPassword}</p>
                    //            </div>
                    //        </div>
                    //        <p>{loginText}</p>
                    //        <p>If you face any issues, contact your system administrator.</p>
                    //        <p class='footer'>{footerText}</p>
                    //        {(System.IO.File.Exists(logoPath) ? $"<br><img src='cid:FooterLogo' alt='Smart Schools ERP Logo' width='150' />" : "")}
                    //    </div>
                    //</body>
                    //</html>";

                    //            builder.HtmlBody = htmlBody;
                    //            emailMessage.Body = builder.ToMessageBody();

                    //            try
                    //            {
                    //                using var smtpClient = new MailKit.Net.Smtp.SmtpClient();
                    //                await smtpClient.ConnectAsync(_configuration["Smtp:Host"], int.Parse(_configuration["Smtp:Port"]), MailKit.Security.SecureSocketOptions.StartTls);
                    //                await smtpClient.AuthenticateAsync(_configuration["Smtp:Username"], _configuration["Smtp:Password"]);
                    //                await smtpClient.SendAsync(emailMessage);
                    //                await smtpClient.DisconnectAsync(true);
                    //            }
                    //            catch (Exception ex)
                    //            {
                    //                Console.WriteLine($"Error sending email: {ex.Message}");
                    //                throw;
                    //            }
                }
                else if (Roleid == "6")
                {
                    //            string actualRecipient = isAdmin ? "chaitanyakantamneni6@gmail.com" : toEmail;
                    //            string subject = isAdmin ? "New Parent Registered" : "Welcome to Smart Schools ERP";

                    //            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "emaillog.jpg");
                    //            string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "buserelelog.jpg");

                    //            var emailMessage = new MimeMessage();
                    //            emailMessage.From.Add(new MailboxAddress("Smart Schools ERP", _configuration["Smtp:Username"]));
                    //            emailMessage.To.Add(MailboxAddress.Parse(actualRecipient));
                    //            emailMessage.Subject = subject;

                    //            var builder = new BodyBuilder();

                    //            // Attach images only if they exist
                    //            if (System.IO.File.Exists(imagePath))
                    //            {
                    //                var headerImage = builder.LinkedResources.Add(imagePath);
                    //                headerImage.ContentId = "CompanyLogo";
                    //            }

                    //            if (System.IO.File.Exists(logoPath))
                    //            {
                    //                var footerLogo = builder.LinkedResources.Add(logoPath);
                    //                footerLogo.ContentId = "FooterLogo";
                    //            }

                    //            // Choose URL and footer based on school
                    //            string loginUrl = !string.IsNullOrEmpty(schoolUrl) ? schoolUrl : "https://www.smartschools.com";
                    //            string loginText = !string.IsNullOrEmpty(schoolName) ? $"Please login at <a href='{loginUrl}'>{schoolName}</a>" : $"Please login at <a href='{loginUrl}'>www.smartschools.com</a>";
                    //            string footerText = !string.IsNullOrEmpty(schoolName) ? $"&copy; {schoolName} 2025. All rights reserved." : "&copy; Smart Schools ERP 2025. All rights reserved.";

                    //            string htmlBody = isAdmin
                    //                ? $@"
                    //<html>
                    //<head>
                    //    <style>
                    //        body {{ font-family: Arial, sans-serif; }}
                    //        .container {{ text-align: center; padding: 20px; background-color: #f0f0f0; }}
                    //        .content {{ margin: 20px; padding: 20px; background-color: #ffffff; border-radius: 10px; }}
                    //        .footer {{ margin-top: 20px; font-size: 12px; color: #555555; }}
                    //    </style>
                    //</head>
                    //<body>
                    //    <div class='container'>
                    //        <div class='content'>
                    //            {(System.IO.File.Exists(imagePath) ? "<img src='cid:CompanyLogo' alt='Smart Schools ERP Logo' width='200' />" : "")}
                    //            <h2>New Parent Registration</h2>
                    //            <p><strong>Name:</strong> {userName}</p>
                    //            <p><strong>Email:</strong> {toEmail}</p>
                    //            {(System.IO.File.Exists(logoPath) ? "<img src='cid:FooterLogo' alt='Smart Schools ERP Logo' width='150' />" : "")}
                    //        </div>
                    //        <p class='footer'>{footerText}</p>
                    //    </div>
                    //</body>
                    //</html>"
                    //                : $@"
                    //<html>
                    //<head>
                    //    <style>
                    //        body {{ font-family: Arial, sans-serif; }}
                    //        .container {{ text-align: center; padding: 20px; background-color: #f0f0f0; }}
                    //        .content {{ margin: 20px; padding: 20px; background-color: #ffffff; border-radius: 10px; }}
                    //        .footer {{ margin-top: 20px; font-size: 12px; color: #555555; }}
                    //        .credentials {{ text-align: left; display: inline-block; margin-top: 10px; }}
                    //    </style>
                    //</head>
                    //<body>
                    //    <div class='container'>
                    //        {(System.IO.File.Exists(imagePath) ? "<img src='cid:CompanyLogo' alt='Smart Schools ERP Logo' width='200' />" : "")}
                    //        <h2>Welcome, {userName}!</h2>
                    //        <p>Congratulations! Your Parent account has been successfully created in <strong>Smart Schools ERP</strong>.</p>
                    //        <div class='content'>
                    //            <h3>Login Details:</h3>
                    //            <div class='credentials'>
                    //                <p><strong>Email / UserID:</strong> {toEmail}</p>
                    //                <p><strong>Password:</strong> {userPassword}</p>
                    //            </div>
                    //        </div>
                    //        <p>{loginText}</p>
                    //        <p>If you face any issues, contact your system administrator.</p>
                    //        <p class='footer'>{footerText}</p>
                    //        {(System.IO.File.Exists(logoPath) ? $"<br><img src='cid:FooterLogo' alt='Smart Schools ERP Logo' width='150' />" : "")}
                    //    </div>
                    //</body>
                    //</html>";

                    //            builder.HtmlBody = htmlBody;
                    //            emailMessage.Body = builder.ToMessageBody();

                    //            try
                    //            {
                    //                using var smtpClient = new MailKit.Net.Smtp.SmtpClient();
                    //                await smtpClient.ConnectAsync(_configuration["Smtp:Host"], int.Parse(_configuration["Smtp:Port"]), MailKit.Security.SecureSocketOptions.StartTls);
                    //                await smtpClient.AuthenticateAsync(_configuration["Smtp:Username"], _configuration["Smtp:Password"]);
                    //                await smtpClient.SendAsync(emailMessage);
                    //                await smtpClient.DisconnectAsync(true);
                    //            }
                    //            catch (Exception ex)
                    //            {
                    //                Console.WriteLine($"Error sending email: {ex.Message}");
                    //                throw;
                    //            }
                }
                else
                {
                    //            string actualRecipient = isAdmin ? "chaitanyakantamneni6@gmail.com" : toEmail;
                    //            string subject = isAdmin ? "New Employee Registered" : "Welcome to Smart Schools ERP";

                    //            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "emaillog.jpg");
                    //            string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "buserelelog.jpg");

                    //            var emailMessage = new MimeMessage();
                    //            emailMessage.From.Add(new MailboxAddress("Smart Schools ERP", _configuration["Smtp:Username"]));
                    //            emailMessage.To.Add(MailboxAddress.Parse(actualRecipient));
                    //            emailMessage.Subject = subject;

                    //            var builder = new BodyBuilder();

                    //            // Attach images only if they exist
                    //            if (System.IO.File.Exists(imagePath))
                    //            {
                    //                var headerImage = builder.LinkedResources.Add(imagePath);
                    //                headerImage.ContentId = "CompanyLogo";
                    //            }

                    //            if (System.IO.File.Exists(logoPath))
                    //            {
                    //                var footerLogo = builder.LinkedResources.Add(logoPath);
                    //                footerLogo.ContentId = "FooterLogo";
                    //            }

                    //            // Choose URL and footer based on school
                    //            string loginUrl = !string.IsNullOrEmpty(schoolUrl) ? schoolUrl : "https://www.smartschools.com";
                    //            string loginText = !string.IsNullOrEmpty(schoolName) ? $"Please login at <a href='{loginUrl}'>{schoolName}</a>" : $"Please login at <a href='{loginUrl}'>www.smartschools.com</a>";
                    //            string footerText = !string.IsNullOrEmpty(schoolName) ? $"&copy; {schoolName} 2025. All rights reserved." : "&copy; Smart Schools ERP 2025. All rights reserved.";

                    //            string htmlBody = isAdmin
                    //                ? $@"
                    //<html>
                    //<head>
                    //    <style>
                    //        body {{ font-family: Arial, sans-serif; }}
                    //        .container {{ text-align: center; padding: 20px; background-color: #f0f0f0; }}
                    //        .content {{ margin: 20px; padding: 20px; background-color: #ffffff; border-radius: 10px; }}
                    //        .footer {{ margin-top: 20px; font-size: 12px; color: #555555; }}
                    //    </style>
                    //</head>
                    //<body>
                    //    <div class='container'>
                    //        <div class='content'>
                    //            {(System.IO.File.Exists(imagePath) ? "<img src='cid:CompanyLogo' alt='Smart Schools ERP Logo' width='200' />" : "")}
                    //            <h2>New Employee Registration</h2>
                    //            <p><strong>Name:</strong> {userName}</p>
                    //            <p><strong>Email:</strong> {toEmail}</p>
                    //            {(System.IO.File.Exists(logoPath) ? "<img src='cid:FooterLogo' alt='Smart Schools ERP Logo' width='150' />" : "")}
                    //        </div>
                    //        <p class='footer'>{footerText}</p>
                    //    </div>
                    //</body>
                    //</html>"
                    //                : $@"
                    //<html>
                    //<head>
                    //    <style>
                    //        body {{ font-family: Arial, sans-serif; }}
                    //        .container {{ text-align: center; padding: 20px; background-color: #f0f0f0; }}
                    //        .content {{ margin: 20px; padding: 20px; background-color: #ffffff; border-radius: 10px; }}
                    //        .footer {{ margin-top: 20px; font-size: 12px; color: #555555; }}
                    //        .credentials {{ text-align: left; display: inline-block; margin-top: 10px; }}
                    //    </style>
                    //</head>
                    //<body>
                    //    <div class='container'>
                    //        {(System.IO.File.Exists(imagePath) ? "<img src='cid:CompanyLogo' alt='Smart Schools ERP Logo' width='200' />" : "")}
                    //        <h2>Welcome, {userName}!</h2>
                    //        <p>Congratulations! Your employee account has been successfully created in <strong>Smart Schools ERP</strong>.</p>
                    //        <div class='content'>
                    //            <h3>Login Details:</h3>
                    //            <div class='credentials'>
                    //                <p><strong>Email / UserID:</strong> {toEmail}</p>
                    //                <p><strong>Password:</strong> {userPassword}</p>
                    //            </div>
                    //        </div>
                    //        <p>{loginText}</p>
                    //        <p>If you face any issues, contact your system administrator.</p>
                    //        <p class='footer'>{footerText}</p>
                    //        {(System.IO.File.Exists(logoPath) ? $"<br><img src='cid:FooterLogo' alt='Smart Schools ERP Logo' width='150' />" : "")}
                    //    </div>
                    //</body>
                    //</html>";

                    //            builder.HtmlBody = htmlBody;
                    //            emailMessage.Body = builder.ToMessageBody();

                    //            try
                    //            {
                    //                using var smtpClient = new MailKit.Net.Smtp.SmtpClient();
                    //                await smtpClient.ConnectAsync(_configuration["Smtp:Host"], int.Parse(_configuration["Smtp:Port"]), MailKit.Security.SecureSocketOptions.StartTls);
                    //                await smtpClient.AuthenticateAsync(_configuration["Smtp:Username"], _configuration["Smtp:Password"]);
                    //                await smtpClient.SendAsync(emailMessage);
                    //                await smtpClient.DisconnectAsync(true);
                    //            }
                    //            catch (Exception ex)
                    //            {
                    //                Console.WriteLine($"Error sending email: {ex.Message}");
                    //                throw;
                    //            }
                }
            }

        [AllowAnonymous]
        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/refresh-token
        /// Purpose: Handles transactional and query operations for refresh-token.
        /// </summary>
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.RefreshToken))
                return Unauthorized(new { message = "Invalid request" });

            var tokenRecord = dbop.GetUserTokenByRefresh(request.Email, request.RefreshToken);
            if (tokenRecord == null)
                return Unauthorized(new { message = "Invalid refresh token" });

            if (tokenRecord.RefreshExpiry < DateTimeHelper.NowIST())
                return Unauthorized(new { message = "Refresh token expired" });

            var dbUser = dbop.Tbl_Users_CRUD_Operations(new TblUser
            {
                Email = request.Email,
                Flag = "11"
            }).FirstOrDefault();

            if (dbUser == null || string.IsNullOrEmpty(dbUser.RollId))
                return Unauthorized(new { message = "Invalid user" });

            string? schoolID = dbUser.RollId != "1" ? dbUser.SchoolID : null;

            var tokenService = new TokenService(_configuration);

            var (accessToken, newRefreshToken, accessExpiryUtc, refreshExpiryUtc) =
                tokenService.GenerateTokens(
                    dbUser.Email,
                    $"{dbUser.FirstName} {dbUser.LastName}",
                    dbUser.RollId,
                    schoolID
                );

            dbop.RevokeUserToken(tokenRecord.RefreshToken);
            dbop.InsertUserToken(
                dbUser.Email,
                accessToken,
                newRefreshToken,
                accessExpiryUtc,
                refreshExpiryUtc
            );

            return Ok(new
            {
                accessToken,
                refreshToken = newRefreshToken,
                role = dbUser.RollId,
                email = dbUser.Email,
                schoolId = dbUser.SchoolID,
                schoolName = dbUser.SchoolName
            });
        }



        //Masters Module
        //[HttpPost("Tbl_SchoolDetails_CRUD")]
        //public IActionResult Tbl_SchoolDetails_CRUD([FromBody] SchoolDetails school)
        //{
        //    try
        //    {
        //        var roleId = User.FindFirst("role")?.Value;
        //        var tokenSchoolId = User.FindFirst("SchoolID")?.Value;
        //        if (roleId != "1")
        //        {
        //            school.SchoolID = string.IsNullOrWhiteSpace(tokenSchoolId) ? null : tokenSchoolId;
        //        }

        //        var result = dbop.Tbl_SchoolDetails_CRUD(school);

        //        if (result == null)
        //        {
        //            return StatusCode(500, new
        //            {
        //                StatusCode = 500,
        //                Success = false,
        //                Message = "Database returned null result."
        //            });
        //        }

        //        var error = result.FirstOrDefault(x => !string.IsNullOrEmpty(x.Status) && x.Status.ToLower().Contains("error"));
        //        if (error != null)
        //        {
        //            return StatusCode(500, new
        //            {
        //                StatusCode = 500,
        //                Success = false,
        //                Message = error.Status
        //            });
        //        }

        //        if (result.First().Status == "School name already exists")
        //        {
        //            return StatusCode(400, new
        //            {
        //                StatusCode = 400,
        //                Success = false,
        //                Message = result.First().Status,
        //                Data = result
        //            });
        //        }

        //        return Ok(new
        //        {
        //            StatusCode = 200,
        //            Success = true,
        //            Message = result.First().Status,
        //            Data = result
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        dbop.LogException(ex, "SchoolManagementController", "Tbl_SchoolDetails_CRUD", Newtonsoft.Json.JsonConvert.SerializeObject(school));
        //        return BadRequest(new
        //        {
        //            StatusCode = 500,
        //            Success = false,
        //            Message = "Internal server error occurred. Please try again.",
        //            Error = ex.Message
        //        });
        //    }
        //}

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_SchoolDetails_CRUD
        /// Purpose: Handles transactional and query operations for Tbl_SchoolDetails_CRUD.
        /// </summary>
        [HttpPost("Tbl_SchoolDetails_CRUD")]
        public IActionResult Tbl_SchoolDetails_CRUD([FromBody] SchoolDetails school)
        {
            try
            {
                var roleId = User.FindFirst("role")?.Value
                                  ?? User.FindFirst(ClaimTypes.Role)?.Value;
                var tokenSchoolId = User.FindFirst("SchoolID")?.Value;
                var tokenSchoolIds = User.FindFirst("SchoolIDs")?.Value; // ── group admin JWT claim

                if (roleId == "10")
                {
                    // Group Admin: clear single SchoolID, inject JWT-authoritative SchoolIDs
                    school.SchoolID = null;
                    school.SchoolIDs = tokenSchoolIds; // always from JWT, never trust client
                }
                else if (roleId != "1")
                {
                    // School Admin / Principal / etc: scope to their one school
                    school.SchoolID = string.IsNullOrWhiteSpace(tokenSchoolId) ? null : tokenSchoolId;
                    school.SchoolIDs = null;
                }
                // Super Admin (role 1): no restriction — both stay null → proc returns all

                var result = dbop.Tbl_SchoolDetails_CRUD(school);

                if (result == null)
                    return StatusCode(500, new { StatusCode = 500, Success = false, Message = "Database returned null result." });

                if (!result.Any())
                    return Ok(new { StatusCode = 200, Success = true, Message = "No schools found", Data = new List<SchoolDetails>() });

                var error = result.FirstOrDefault(x => !string.IsNullOrEmpty(x.Status)
                                  && x.Status.ToLower().Contains("error"));
                if (error != null)
                    return StatusCode(500, new { StatusCode = 500, Success = false, Message = error.Status });

                if (result.First().Status == "School name already exists")
                    return StatusCode(400, new { StatusCode = 400, Success = false, Message = result.First().Status, Data = result });

                return Ok(new { StatusCode = 200, Success = true, Message = result.First().Status, Data = result });
            }
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_SchoolDetails_CRUD",
                    Newtonsoft.Json.JsonConvert.SerializeObject(school));
                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_AcademicYear_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_AcademicYear_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_AcademicYear_CRUD_Operations")]
        public IActionResult Tbl_AcademicYear_CRUD_Operations([FromBody] tblAcademicYear academicYear)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId == "10")
                {
                    // Group Admin
                    academicYear.SchoolID = academicYear.SchoolID;
                }
                else if (roleId != "1")
                {
                    academicYear.SchoolID = schoolId;
                }

                var result = dbop.Tbl_AcademicYear_CRUD_Operations(academicYear);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                if (result.First().Status == "Academic Year name already exists")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
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
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_AcademicYear_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(academicYear));
                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_Syllabus_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_Syllabus_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_Syllabus_CRUD_Operations")]
        public IActionResult Tbl_Syllabus_CRUD_Operations([FromBody] tblSyllabus syllabus)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    syllabus.SchoolID = schoolId;
                }

                var result = dbop.Tbl_Syllabus_CRUD_Operations(syllabus);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                if (result.First().Status == "Syllabus name already exists")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
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
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_Syllabus_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(syllabus));
                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/ExportSyllabusToExcel
        /// Purpose: Handles transactional and query operations for ExportSyllabusToExcel.
        /// </summary>
        [HttpPost("ExportSyllabusToExcel")]
        public async Task<IActionResult> ExportSyllabusToExcel([FromBody] tblSyllabus filter)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            const int batchSize = 50_000;
            DateTime? lastCreatedDate = null;
            int? lastID = null;

            // Do NOT use "using" here for the stream; ASP.NET will manage it
            var stream = new MemoryStream();
            var package = new ExcelPackage(stream);

            var ws = package.Workbook.Worksheets.Add("Syllabus");

            // Header
            ws.Cells[1, 1].Value = "ID";
            ws.Cells[1, 2].Value = "Name";
            ws.Cells[1, 3].Value = "School Name";
            ws.Cells[1, 4].Value = "Academic Year";
            ws.Cells[1, 5].Value = "Available From";
            ws.Cells[1, 6].Value = "Description";
            ws.Cells[1, 7].Value = "Status";
            ws.Cells[1, 8].Value = "Created Date";

            int currentRow = 2;

            while (true)
            {
                var batch = await _dbContext.Tbl_Syllabus
                    .FromSqlRaw(
                        @"CALL Proc_Syllabus(
                    NULL,
                    {0}, {1}, {2},
                    NULL, NULL, NULL, NULL, NULL, NULL, NULL,
                    {3}, {4}, {5}, {6},
                    NULL, NULL, -1
                )",
                        filter.SchoolID,
                        filter.AcademicYear,
                        filter.Name,
                        filter.Flag ?? "2",
                        batchSize,
                        lastCreatedDate,
                        lastID
                    )
                    .AsNoTracking()
                    .ToListAsync();

                if (!batch.Any()) break;

                foreach (var item in batch)
                {
                    ws.Cells[currentRow, 1].Value = item.ID ?? 0;
                    ws.Cells[currentRow, 2].Value = item.Name ?? "";
                    ws.Cells[currentRow, 3].Value = item.SchoolName ?? "";
                    ws.Cells[currentRow, 4].Value = item.AcademicYearName ?? "";
                    ws.Cells[currentRow, 5].Value = item.AvailableFrom?.ToString("yyyy-MM-dd") ?? "";
                    ws.Cells[currentRow, 6].Value = item.Description ?? "";
                    ws.Cells[currentRow, 7].Value = (item.IsActive ?? false) ? "Active" : "Inactive";
                    ws.Cells[currentRow, 8].Value = item.CreatedDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "";
                    currentRow++;
                }

                lastCreatedDate = batch.Last().CreatedDate;
                lastID = batch.Last().ID;
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            package.Save();
            stream.Position = 0;

            // Return stream directly; do NOT dispose it yet
            return File(
                stream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Syllabus_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
            );
        }

        //[HttpPost("ExportSyllabus")]
        //public async Task<IActionResult> ExportSyllabus([FromBody] tblSyllabus filter, [FromQuery] string type)
        //{
        //    const int batchSize = 50000;
        //    DateTime? lastCreatedDate = null;
        //    int? lastID = null;

        //    if (type == "excel")
        //    {
        //        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //        var stream = new MemoryStream();
        //        var package = new ExcelPackage(stream);
        //        var ws = package.Workbook.Worksheets.Add("Syllabus");

        //        // Headers
        //        ws.Cells[1, 1].Value = "ID";
        //        ws.Cells[1, 2].Value = "Name";
        //        ws.Cells[1, 3].Value = "School Name";
        //        ws.Cells[1, 4].Value = "Academic Year";
        //        ws.Cells[1, 5].Value = "Available From";
        //        ws.Cells[1, 6].Value = "Description";
        //        ws.Cells[1, 7].Value = "Status";
        //        ws.Cells[1, 8].Value = "Created Date";

        //        int currentRow = 2;

        //        while (true)
        //        {
        //            var batch = await _dbContext.Tbl_Syllabus
        //                .FromSqlRaw(
        //                    @"CALL Proc_Syllabus(
        //                    NULL,
        //                    {0}, {1}, {2},
        //                    NULL, NULL, NULL, NULL, NULL, NULL, NULL,
        //                    {3}, {4}, {5}, {6},
        //                    NULL, NULL, -1
        //                )",
        //                    filter.SchoolID,
        //                    filter.AcademicYear,
        //                    filter.Name,
        //                    filter.Flag ?? "2",
        //                    batchSize,
        //                    lastCreatedDate,
        //                    lastID
        //                )
        //                .AsNoTracking()
        //                .ToListAsync();

        //            if (!batch.Any()) break;

        //            foreach (var item in batch)
        //            {
        //                ws.Cells[currentRow, 1].Value = item.ID ?? 0;
        //                ws.Cells[currentRow, 2].Value = item.Name ?? "";
        //                ws.Cells[currentRow, 3].Value = item.SchoolName ?? "";
        //                ws.Cells[currentRow, 4].Value = item.AcademicYearName ?? "";
        //                ws.Cells[currentRow, 5].Value = item.AvailableFrom?.ToString("yyyy-MM-dd") ?? "";
        //                ws.Cells[currentRow, 6].Value = item.Description ?? "";
        //                ws.Cells[currentRow, 7].Value = (item.IsActive ?? false) ? "Active" : "Inactive";
        //                ws.Cells[currentRow, 8].Value = item.CreatedDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "";
        //                currentRow++;
        //            }

        //            lastCreatedDate = batch.Last().CreatedDate;
        //            lastID = batch.Last().ID;
        //        }

        //        ws.Cells[ws.Dimension.Address].AutoFitColumns();
        //        package.Save();
        //        stream.Position = 0;

        //        return File(
        //            stream,
        //            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        //            $"Syllabus_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
        //        );
        //    }
        //    else if (type == "pdf" || type == "print")
        //    {
        //        var stream = new MemoryStream();
        //        var allData = new List<tblSyllabus>();
        //        while (true)
        //        {
        //            var batch = await _dbContext.Tbl_Syllabus
        //                .FromSqlRaw(
        //                    @"CALL Proc_Syllabus(
        //        NULL,
        //        {0}, {1}, {2},
        //        NULL, NULL, NULL, NULL, NULL, NULL, NULL,
        //        {3}, {4}, {5}, {6},
        //        NULL, NULL, -1
        //    )",
        //                    filter.SchoolID,
        //                    filter.AcademicYear,
        //                    filter.Name,
        //                    filter.Flag ?? "2",
        //                    batchSize,
        //                    lastCreatedDate,
        //                    lastID
        //                )
        //                .AsNoTracking()
        //                .ToListAsync();

        //            if (!batch.Any()) break;

        //            allData.AddRange(batch);
        //            lastCreatedDate = batch.Last().CreatedDate;
        //            lastID = batch.Last().ID;
        //        }

        //        // Generate PDF with borders & colors, no Description column
        //        var doc = Document.Create(container =>
        //        {
        //            container.Page(page =>
        //            {
        //                page.Size(PageSizes.A4);
        //                page.Margin(20);
        //                page.DefaultTextStyle(x => x.FontSize(10));

        //                page.Header().Text("Syllabus Report").SemiBold().FontSize(14);

        //                page.Content().Table(table =>
        //                {
        //                    // Define 7 columns (without Description)
        //                    table.ColumnsDefinition(columns =>
        //                    {
        //                        columns.RelativeColumn(); // ID
        //                        columns.RelativeColumn(); // Name
        //                        columns.RelativeColumn(); // School Name
        //                        columns.RelativeColumn(); // Academic Year
        //                        columns.RelativeColumn(); // Available From
        //                        columns.RelativeColumn(); // Status
        //                        columns.RelativeColumn(); // Created Date
        //                    });

        //                    // Header row with background color
        //                    table.Header(header =>
        //                    {
        //                        var headerCells = new[] { "ID", "Name", "School Name", "Academic Year", "Available From", "Status", "Created Date" };
        //                        foreach (var h in headerCells)
        //                        {
        //                            header.Cell()
        //                                  .Background(Colors.Grey.Lighten2)
        //                                  .Border(1)
        //                                  .BorderColor(Colors.Black)
        //                                  .Padding(3)
        //                                  .Text(h)
        //                                  .SemiBold();
        //                        }
        //                    });

        //                    // Data rows with borders
        //                    foreach (var item in allData)
        //                    {
        //                        table.Cell().Border(1).BorderColor(Colors.Black).Padding(3).Text(item.ID?.ToString() ?? "");
        //                        table.Cell().Border(1).BorderColor(Colors.Black).Padding(3).Text(item.Name ?? "");
        //                        table.Cell().Border(1).BorderColor(Colors.Black).Padding(3).Text(item.SchoolName ?? "");
        //                        table.Cell().Border(1).BorderColor(Colors.Black).Padding(3).Text(item.AcademicYearName ?? "");
        //                        table.Cell().Border(1).BorderColor(Colors.Black).Padding(3).Text(item.AvailableFrom?.ToString("yyyy-MM-dd") ?? "");
        //                        table.Cell().Border(1).BorderColor(Colors.Black).Padding(3).Text((item.IsActive ?? false) ? "Active" : "Inactive");
        //                        table.Cell().Border(1).BorderColor(Colors.Black).Padding(3).Text(item.CreatedDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "");
        //                    }
        //                });
        //            });
        //        });

        //        doc.GeneratePdf(stream);
        //        stream.Position = 0;

        //        return File(
        //            stream,
        //            "application/pdf",
        //            $"Syllabus_{DateTime.Now:yyyyMMddHHmmss}.pdf"
        //        );
        //    }


        //    return BadRequest("Invalid export type");
        //}

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/ExportSyllabus
        /// Purpose: Handles transactional and query operations for ExportSyllabus.
        /// </summary>
        [HttpPost("ExportSyllabus")]
        public async Task<IActionResult> ExportSyllabus(
    [FromBody] tblSyllabus filter,
    [FromQuery] string type)
        {
            const int batchSize = 50000;
            DateTime? lastCreatedDate = null;
            int? lastID = null;

            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    filter.SchoolID = schoolId;
                }

                if (type == "excel")
                {
                    using var stream = new MemoryStream();
                    using var package = new ExcelPackage(stream);
                    var ws = package.Workbook.Worksheets.Add("Syllabus");

                    // Headers
                    ws.Cells[1, 1].Value = "ID";
                    ws.Cells[1, 2].Value = "Name";
                    ws.Cells[1, 3].Value = "School Name";
                    ws.Cells[1, 4].Value = "Academic Year";
                    ws.Cells[1, 5].Value = "Available From";
                    ws.Cells[1, 6].Value = "Description";
                    ws.Cells[1, 7].Value = "Status";
                    ws.Cells[1, 8].Value = "Created Date";

                    int currentRow = 2;

                    while (true)
                    {
                        using var context = _contextFactory.CreateDbContext();

                        var batch = await context.Tbl_Syllabus
                            .FromSqlRaw(
                                @"CALL Proc_Syllabus(
                            NULL,
                            {0}, {1}, {2},
                            NULL, NULL, NULL, NULL, NULL, NULL, NULL,
                            {3}, {4}, {5}, {6},
                            NULL, NULL, -1
                        )",
                                filter.SchoolID,
                                filter.AcademicYear,
                                filter.Name,
                                filter.Flag ?? "2",
                                batchSize,
                                lastCreatedDate,
                                lastID
                            )
                            .AsNoTracking()
                            .ToListAsync();

                        if (!batch.Any())
                            break;

                        foreach (var item in batch)
                        {
                            ws.Cells[currentRow, 1].Value = item.ID ?? 0;
                            ws.Cells[currentRow, 2].Value = item.Name ?? "";
                            ws.Cells[currentRow, 3].Value = item.SchoolName ?? "";
                            ws.Cells[currentRow, 4].Value = item.AcademicYearName ?? "";
                            ws.Cells[currentRow, 5].Value = item.AvailableFrom?.ToString("yyyy-MM-dd") ?? "";
                            ws.Cells[currentRow, 6].Value = item.Description ?? "";
                            ws.Cells[currentRow, 7].Value = (item.IsActive ?? false) ? "Active" : "Inactive";
                            ws.Cells[currentRow, 8].Value = item.CreatedDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "";
                            currentRow++;
                        }

                        lastCreatedDate = batch.Last().CreatedDate;
                        lastID = batch.Last().ID;
                    }

                    ws.Cells[ws.Dimension.Address].AutoFitColumns();
                    package.Save();
                    stream.Position = 0;

                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"Syllabus_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
                    );
                }

                else if (type == "pdf" || type == "print")
                {
                    using var stream = new MemoryStream();
                    var allData = new List<tblSyllabus>();

                    while (true)
                    {
                        using var context = _contextFactory.CreateDbContext();

                        var batch = await context.Tbl_Syllabus
                            .FromSqlRaw(
                                @"CALL Proc_Syllabus(
                            NULL,
                            {0}, {1}, {2},
                            NULL, NULL, NULL, NULL, NULL, NULL, NULL,
                            {3}, {4}, {5}, {6},
                            NULL, NULL, -1
                        )",
                                filter.SchoolID,
                                filter.AcademicYear,
                                filter.Name,
                                filter.Flag ?? "2",
                                batchSize,
                                lastCreatedDate,
                                lastID
                            )
                            .AsNoTracking()
                            .ToListAsync();

                        if (!batch.Any())
                            break;

                        allData.AddRange(batch);

                        lastCreatedDate = batch.Last().CreatedDate;
                        lastID = batch.Last().ID;
                    }

                    var doc = Document.Create(container =>
                    {
                        container.Page(page =>
                        {
                            page.Size(PageSizes.A4);
                            page.Margin(20);
                            page.DefaultTextStyle(x => x.FontSize(10));

                            page.Header().Text("Syllabus Report").SemiBold().FontSize(14);

                            page.Content().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    for (int i = 0; i < 7; i++)
                                        columns.RelativeColumn();
                                });

                                table.Header(header =>
                                {
                                    var headers = new[]
                                    {
                                "ID","Name","School Name",
                                "Academic Year","Available From",
                                "Status","Created Date"
                            };

                                    foreach (var h in headers)
                                    {
                                        header.Cell()
                                              .Background(Colors.Grey.Lighten2)
                                              .Border(1)
                                              .Padding(3)
                                              .Text(h)
                                              .SemiBold();
                                    }
                                });

                                foreach (var item in allData)
                                {
                                    table.Cell().Border(1).Padding(3).Text(item.ID?.ToString() ?? "");
                                    table.Cell().Border(1).Padding(3).Text(item.Name ?? "");
                                    table.Cell().Border(1).Padding(3).Text(item.SchoolName ?? "");
                                    table.Cell().Border(1).Padding(3).Text(item.AcademicYearName ?? "");
                                    table.Cell().Border(1).Padding(3).Text(item.AvailableFrom?.ToString("yyyy-MM-dd") ?? "");
                                    table.Cell().Border(1).Padding(3).Text((item.IsActive ?? false) ? "Active" : "Inactive");
                                    table.Cell().Border(1).Padding(3).Text(item.CreatedDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "");
                                }
                            });
                        });
                    });

                    doc.GeneratePdf(stream);
                    stream.Position = 0;

                    return File(
                        stream.ToArray(),
                        "application/pdf",
                        $"Syllabus_{DateTime.Now:yyyyMMddHHmmss}.pdf"
                    );
                }

                return BadRequest("Invalid export type");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ExportSyllabus failed");
                return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        [AllowAnonymous]
        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_Class_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_Class_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_Class_CRUD_Operations")]
        public IActionResult Tbl_Class_CRUD_Operations([FromBody] tblClass Class)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    Class.SchoolID = schoolId;
                }
                var result = dbop.Tbl_Class_CRUD_Operations(Class);

                if (result == null || result.Count == 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "No result returned or operation failed."
                    });
                }

                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return BadRequest(new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                if (result.First().Status == "Class name already exists")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
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
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_Class_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(Class));
                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_ClassDivision_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_ClassDivision_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_ClassDivision_CRUD_Operations")]
        public IActionResult Tbl_ClassDivision_CRUD_Operations([FromBody] tblClassDivision classDivision)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    classDivision.SchoolID = schoolId;
                }
                var result = dbop.Tbl_ClassDivision_CRUD_Operations(classDivision);

                if (result == null || result.Count == 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "No result returned or operation failed."
                    });
                }

                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return BadRequest(new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                if (result.First().Status == "ClassDivision name already exists")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
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
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_ClassDivision_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(classDivision));
                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }

        //[HttpPost("Tbl_Staff_CRUD_Operations")]
        //public IActionResult Tbl_Staff_CRUD_Operations([FromBody] tblStaff staff)
        //{
        //    try
        //    {
        //        var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
        //        var schoolId =
        //            User.FindFirst("SchoolID")?.Value;

        //        if (roleId != "1")
        //        {
        //            staff.SchoolID = schoolId;
        //        }
        //        var result = dbop.Tbl_Staff_CRUD_Operations(staff);

        //        if (result == null || result.Count == 0)
        //        {
        //            return BadRequest(new
        //            {
        //                StatusCode = 500,
        //                Success = false,
        //                Message = "No result returned or operation failed."
        //            });
        //        }

        //        var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
        //        if (error != null)
        //        {
        //            return BadRequest(new { StatusCode = 500, Success = false, Message = error.Status });
        //        }

        //        if (result.First().Status == "Staff already exists")
        //        {
        //            return StatusCode(400, new
        //            {
        //                StatusCode = 400,
        //                Success = false,
        //                Message = result.First().Status,
        //                Data = result
        //            });
        //        }

        //        return Ok(new
        //        {
        //            StatusCode = 200,
        //            Success = true,
        //            Message = result.First().Status,
        //            Data = result
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        dbop.LogException(ex, "SchoolManagementController", "Tbl_Staff_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(staff));
        //        return BadRequest(new
        //        {
        //            StatusCode = 500,
        //            Success = false,
        //            Message = "Internal server error occurred. Please try again.",
        //            Error = ex.Message
        //        });
        //    }
        //}

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_Staff_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_Staff_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_Staff_CRUD_Operations")]
        public IActionResult Tbl_Staff_CRUD_Operations([FromBody] tblStaff staff)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                // Existing logic — untouched
                if (roleId != "1")
                {
                    staff.SchoolID = schoolId;
                }

                // For Group Admin inserting via Flag 1, use Flag 13 instead
                // so SchoolID stays NULL on tbl_staff itself
                bool isGroupAdmin = staff.StaffType == "10"; // your Group Admin role ID
                if (isGroupAdmin && staff.Flag == "1")
                    staff.Flag = "13";

                var result = dbop.Tbl_Staff_CRUD_Operations(staff);

                if (result == null || result.Count == 0)
                    return BadRequest(new { StatusCode = 500, Success = false, Message = "No result returned." });

                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                    return BadRequest(new { StatusCode = 500, Success = false, Message = error.Status });

                if (result.First().Status == "Staff already exists")
                    return StatusCode(400, new { StatusCode = 400, Success = false, Message = result.First().Status, Data = result });

                // NEW: If Group Admin and insert/update succeeded, tag the schools
                if (isGroupAdmin && !string.IsNullOrEmpty(staff.SchoolIDs))
                {
                    var insertedStaffId = Convert.ToInt64(result.First().ID);
                    var schoolList = staff.SchoolIDs.Split(',').Where(s => !string.IsNullOrEmpty(s)).ToList();
                    dbop.TagStaffSchools(insertedStaffId, schoolList);
                }

                return Ok(new { StatusCode = 200, Success = true, Message = result.First().Status, Data = result });
            }
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_Staff_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(staff));
                return BadRequest(new { StatusCode = 500, Success = false, Message = "Internal server error.", Error = ex.Message });
            }
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_Subject_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_Subject_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_Subject_CRUD_Operations")]
        public IActionResult Tbl_Subject_CRUD_Operations([FromBody] tblSubjects subject)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    subject.SchoolID = schoolId;
                }

                var result = dbop.Tbl_Subjects_CRUD_Operations(subject);

                if (result == null || result.Count == 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "No result returned or operation failed."
                    });
                }

                var statusText = result.First().Status?.ToLower() ?? string.Empty;

                if (statusText.Contains("error"))
                {
                    return BadRequest(new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = result.First().Status
                    });
                }

                if (result.First().Status == "Subject already exists for one or more selected classes")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
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
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_Subject_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(subject));
                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }

        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_SubjectStaff_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_SubjectStaff_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_SubjectStaff_CRUD_Operations")]
        public IActionResult Tbl_SubjectStaff_CRUD_Operations([FromBody] tblSubjectStaff subjectStaff)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    subjectStaff.SchoolID = schoolId;
                }

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

                if (result.First().Status == "Staff already Allocated for this school and academic year")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
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
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_SubjectStaff_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(subjectStaff));
                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_Modules_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_Modules_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_Modules_CRUD_Operations")]
        public IActionResult Tbl_Modules_CRUD_Operations([FromBody] tblModules module)
        {
            try
            {
                var result = dbop.Tbl_Modules_CRUD_Operations(module);

                if (result == null)
                {
                    return BadRequest(new { StatusCode = 500, Message = "No result returned or operation failed." });
                }

                // Check for any error status
                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return BadRequest(new { StatusCode = 500, Message = error.Status });
                }

                if (result.First().Status == "Module Name Already Exists")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
                    });
                }

                return Ok(new { StatusCode = 200, Success = true, Message = result.First().Status, Data = result });
            }
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_Modules_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(module));
                return BadRequest(new { StatusCode = 500, Success = false, Message = "Internal server error.", Error = ex.Message });
            }
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_Pages_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_Pages_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_Pages_CRUD_Operations")]
        public IActionResult Tbl_Pages_CRUD_Operations([FromBody] tblPages page)
        {
            try
            {
                var result = dbop.Tbl_Pages_CRUD_Operations(page);

                if (result == null)
                {
                    return BadRequest(new { StatusCode = 500, Message = "No result returned or operation failed." });
                }

                // Check for any error status
                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return BadRequest(new { StatusCode = 500, Message = error.Status });
                }

                if (result.First().Status == "Page Name Already Exists")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
                    });
                }

                return Ok(new { StatusCode = 200, Success = true, Message = result.First().Status, Data = result });
            }
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_Pages_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(page));
                return BadRequest(new { StatusCode = 500, Success = false, Message = "Internal server error.", Error = ex.Message });
            }
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/ExportPages
        /// Purpose: Handles transactional and query operations for ExportPages.
        /// </summary>
        [HttpPost("ExportPages")]
        public IActionResult ExportPages([FromBody] tblPages filter, [FromQuery] string type)
        {
            const int batchSize = 50000;
            DateTime? lastCreatedDate = null;
            int? lastID = null;

            // ensure paging values
            filter.Limit = batchSize;
            filter.Offset = -1;
            filter.Flag ??= "2";

            var allData = new List<tblPages>();

            while (true)
            {
                filter.LastCreatedDate = lastCreatedDate;
                filter.LastID = lastID;

                // 🔥 CALL DAL (this already handles INT → string safely)
                var batch = dbop.Tbl_Pages_CRUD_Operations(filter);

                if (batch == null || batch.Count == 0)
                    break;

                allData.AddRange(batch);

                lastCreatedDate = batch.Last().CreatedDate;
                lastID = int.TryParse(batch.Last().ID, out var pid) ? pid : lastID;
            }

            if (type == "excel")
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                var stream = new MemoryStream();
                using var package = new ExcelPackage(stream);
                var ws = package.Workbook.Worksheets.Add("Pages");

                // Headers
                ws.Cells[1, 1].Value = "ID";
                ws.Cells[1, 2].Value = "Module";
                ws.Cells[1, 3].Value = "Page Name";
                ws.Cells[1, 4].Value = "Description";
                ws.Cells[1, 5].Value = "Status";
                ws.Cells[1, 6].Value = "Created Date";

                int row = 2;
                foreach (var item in allData)
                {
                    ws.Cells[row, 1].Value = item.ID;
                    ws.Cells[row, 2].Value = item.ModuleName;
                    ws.Cells[row, 3].Value = item.PageName;
                    ws.Cells[row, 4].Value = item.Description;
                    ws.Cells[row, 5].Value =
                        (item.IsActive == "1" || item.IsActive?.ToLower() == "true")
                            ? "Active"
                            : "Inactive";
                    ws.Cells[row, 6].Value =
                        item.CreatedDate?.ToString("yyyy-MM-dd HH:mm:ss");
                    row++;
                }

                ws.Cells[ws.Dimension.Address].AutoFitColumns();
                package.Save();

                stream.Position = 0;
                return File(
                    stream,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"Pages_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
                );
            }

            if (type == "pdf" || type == "print")
            {
                var stream = new MemoryStream();

                var doc = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(20);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        page.Header()
                            .Text("Pages Report")
                            .SemiBold()
                            .FontSize(14);

                        page.Content().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                string[] headers =
                                {
                            "ID", "Module", "Page Name",
                            "Description", "Status", "Created Date"
                        };

                                foreach (var h in headers)
                                {
                                    header.Cell()
                                          .Background(Colors.Grey.Lighten2)
                                          .Border(1)
                                          .Padding(3)
                                          .Text(h)
                                          .SemiBold();
                                }
                            });

                            foreach (var item in allData)
                            {
                                table.Cell().Border(1).Padding(3).Text(item.ID);
                                table.Cell().Border(1).Padding(3).Text(item.ModuleName);
                                table.Cell().Border(1).Padding(3).Text(item.PageName);
                                table.Cell().Border(1).Padding(3).Text(item.Description);
                                table.Cell().Border(1).Padding(3)
                                    .Text((item.IsActive == "1" || item.IsActive?.ToLower() == "true")
                                        ? "Active"
                                        : "Inactive");
                                table.Cell().Border(1).Padding(3)
                                    .Text(item.CreatedDate?.ToString("yyyy-MM-dd HH:mm:ss"));
                            }
                        });
                    });
                });

                doc.GeneratePdf(stream);
                stream.Position = 0;

                return File(
                    stream,
                    "application/pdf",
                    $"Pages_{DateTime.Now:yyyyMMddHHmmss}.pdf"
                );
            }

            return BadRequest("Invalid export type");
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_Roles_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_Roles_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_Roles_CRUD_Operations")]
        public IActionResult Tbl_Roles_CRUD_Operations([FromBody] tblRoles role)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                //if (roleId != "1")
                //{
                //    role.SchoolID = schoolId;
                //}

                var result = dbop.Tbl_Roles_CRUD_Operations(role);

                if (result == null || result.Count == 0)
                {
                    return BadRequest(new { StatusCode = 500, Message = "No result returned or operation failed." });
                }

                // Check for any error status
                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return BadRequest(new { StatusCode = 500, Message = error.Status });
                }

                if (result.First().Status == "Role Already Exists")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
                    });
                }

                return Ok(new { StatusCode = 200, Success = true, Message = result.First().Status, Data = result });
            }
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_Roles_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(role));
                return BadRequest(new { StatusCode = 500, Success = false, Message = "Internal server error.", Error = ex.Message });
            }
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_UserRoles_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_UserRoles_CRUD_Operations.
        /// </summary>
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

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_RolePermissions_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_RolePermissions_CRUD_Operations.
        /// </summary>
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

        /// <summary>
        /// API Endpoint: Get api/SchoolManagement/Tbl_GetRoleMenuPermissions/{roleId}
        /// Purpose: Handles transactional and query operations for Tbl_GetRoleMenuPermissions/{roleId}.
        /// </summary>
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

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/GetPermissionsByRole
        /// Purpose: Handles transactional and query operations for GetPermissionsByRole.
        /// </summary>
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

        //Academic Module
        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_StudentDetails_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_StudentDetails_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_StudentDetails_CRUD_Operations")]
        public IActionResult Tbl_StudentDetails_CRUD_Operations([FromBody] tblStudentDetails admission)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    admission.SchoolID = schoolId;
                }

                var result = dbop.Tbl_StudentDetails_CRUD_Operations(admission);

                if (result == null || result.Count == 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "No result returned or operation failed."
                    });
                }

                var statusText = result.First().Status?.ToLower() ?? string.Empty;

                if (statusText.Contains("error"))
                {
                    return BadRequest(new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = result.First().Status
                    });
                }

                if (result.First().Status == "AdmissionNo already exists")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
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
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_StudentDetails_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(admission));
                return BadRequest(new { StatusCode = 500, Success = false, Message = "Internal server error.", Error = ex.Message });
            }
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_StudentAddressDetails_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_StudentAddressDetails_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_StudentAddressDetails_CRUD_Operations")]
        public IActionResult Tbl_StudentAddressDetails_CRUD_Operations([FromBody] tblStudentAddressDetails admission)
        {
            try
            {
                var result = dbop.Tbl_StudentAddressDetails_CRUD_Operations(admission);

                if (result == null || result.Count == 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "No result returned or operation failed."
                    });
                }

                var statusText = result.First().Status?.ToLower() ?? string.Empty;

                if (statusText.Contains("error"))
                {
                    return BadRequest(new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = result.First().Status
                    });
                }

                if (result.First().Status == "Page Name Already Exists")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
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
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_StudentAddressDetails_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(admission));
                return BadRequest(new { StatusCode = 500, Success = false, Message = "Internal server error.", Error = ex.Message });
            }
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_StudentParentDetails_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_StudentParentDetails_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_StudentParentDetails_CRUD_Operations")]
        public IActionResult Tbl_StudentParentDetails_CRUD_Operations([FromBody] tblStudentParentDetails admission)
        {
            try
            {
                var result = dbop.Tbl_StudentParentDetails_CRUD_Operations(admission);

                if (result == null || result.Count == 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "No result returned or operation failed."
                    });
                }

                var statusText = result.First().Status?.ToLower() ?? string.Empty;

                if (statusText.Contains("error"))
                {
                    return BadRequest(new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = result.First().Status
                    });
                }

                if (result.First().Status == "Parent details for this AdmissionID already exists")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
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
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_StudentParentDetails_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(admission));
                return BadRequest(new { StatusCode = 500, Success = false, Message = "Internal server error.", Error = ex.Message });
            }
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_StudentAcademicDetails_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_StudentAcademicDetails_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_StudentAcademicDetails_CRUD_Operations")]
        public IActionResult Tbl_StudentAcademicDetails_CRUD_Operations([FromBody] tblStudentAcademicDetails admission)
        {
            try
            {
                var result = dbop.Tbl_StudentAcademicDetails_CRUD_Operations(admission);

                if (result == null || result.Count == 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "No result returned or operation failed."
                    });
                }

                var statusText = result.First().Status?.ToLower() ?? string.Empty;

                if (statusText.Contains("error"))
                {
                    return BadRequest(new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = result.First().Status
                    });
                }

                if (result.First().Status == "Page Name Already Exists")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
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
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_StudentAcademicDetails_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(admission));
                return BadRequest(new { StatusCode = 500, Success = false, Message = "Internal server error.", Error = ex.Message });
            }
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_StudentTransportationDetails_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_StudentTransportationDetails_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_StudentTransportationDetails_CRUD_Operations")]
        public IActionResult Tbl_StudentTransportationDetails_CRUD_Operations([FromBody] tblStudentTransportationDetails admission)
        {
            try
            {
                var result = dbop.Tbl_StudentTransportationDetails_CRUD_Operations(admission);

                if (result == null || result.Count == 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "No result returned or operation failed."
                    });
                }

                var statusText = result.First().Status?.ToLower() ?? string.Empty;

                if (statusText.Contains("error"))
                {
                    return BadRequest(new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = result.First().Status
                    });
                }

                if (result.First().Status == "Page Name Already Exists")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
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
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_StudentTransportationDetails_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(admission));
                return BadRequest(new { StatusCode = 500, Success = false, Message = "Internal server error.", Error = ex.Message });
            }
        }


        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_AllotClassTeacher_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_AllotClassTeacher_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_AllotClassTeacher_CRUD_Operations")]
        public IActionResult Tbl_AllotClassTeacher_CRUD_Operations([FromBody] tblAllotClassTeacher ClassTeacher)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    ClassTeacher.SchoolID = schoolId;
                }

                var result = dbop.Tbl_AllotClassTeacher_CRUD_Operations(ClassTeacher);

                if (result == null || result.Count == 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "No result returned or operation failed."
                    });
                }

                var statusText = result.First().Status?.ToLower() ?? string.Empty;

                if (statusText.Contains("error"))
                {
                    return BadRequest(new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = result.First().Status
                    });
                }

                if (result.First().Status == "Class Teacher already Allocated")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = "Class Teacher already Allocated for this class",
                        Data = result
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
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_AllotClassTeacher_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(ClassTeacher));
                return BadRequest(new { StatusCode = 500, Success = false, Message = "Internal server error.", Error = ex.Message });
            }
        }

        //Transportation Module
        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_Bus_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_Bus_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_Bus_CRUD_Operations")]
        public IActionResult Tbl_Bus_CRUD_Operations([FromBody] tblBus bus)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    bus.SchoolID = schoolId;
                }
                var result = dbop.Tbl_bus_CRUD_Operations(bus);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                if (result.First().Status == "Bus name already exists")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
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
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_Bus_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(bus));
                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_Route_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_Route_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_Route_CRUD_Operations")]
        public IActionResult Tbl_Route_CRUD_Operations([FromBody] tblRoute route)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    route.SchoolID = schoolId;
                }
                var result = dbop.Tbl_route_CRUD_Operations(route);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                if (result.First().Status == "Route name already exists")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
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
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_Route_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(route));
                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_Stops_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_Stops_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_Stops_CRUD_Operations")]
        public IActionResult Tbl_Stops_CRUD_Operations([FromBody] tblStops stop)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    stop.SchoolID = schoolId;
                }
                var result = dbop.Tbl_stops_CRUD_Operations(stop);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                if (result.First().Status == "Stop already exists")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
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
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_Stops_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(stop));
                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_Fare_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_Fare_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_Fare_CRUD_Operations")]
        public IActionResult Tbl_fare_CRUD_Operations([FromBody] tblFare fare)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    fare.SchoolID = schoolId;
                }
                var result = dbop.Tbl_fare_CRUD_Operations(fare);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                if (result.First().Status == "Fare already exists for this stop, bus, and route")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
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
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_Fare_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(fare));
                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }

        //Time Table Module        
        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_WorkingDays_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_WorkingDays_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_WorkingDays_CRUD_Operations")]
        public IActionResult Tbl_WorkingDays_CRUD_Operations([FromBody] TblWorkingDays wrkdays)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    wrkdays.SchoolID = schoolId;
                }
                var result = dbop.Tbl_WorkingDays_CRUD_Operations(wrkdays);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                if (result.First().Status == "Working day already exists")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
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
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_WorkingDays_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(wrkdays));
                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_Session_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_Session_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_Session_CRUD_Operations")]
        public IActionResult Tbl_Session_CRUD_Operations([FromBody] TblSession Session1)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    Session1.SchoolID = schoolId;
                }
                var result = dbop.Tbl_Session_CRUD_Operations(Session1);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                if (result.First().Status == "Session already exists")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
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
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_Session_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(Session1));
                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_TimeTable_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_TimeTable_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_TimeTable_CRUD_Operations")]
        public IActionResult Tbl_TimeTable_CRUD_Operations([FromBody] TblTimeTable model)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    model.SchoolID = schoolId;
                }

                var result = dbop.Tbl_TimeTable_CRUD(model);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                if (result.Any() && (result.First().Status == "TimeTable already exists for this Class & Division" || result.First().Status == "Staff already allocated for this time slot"))
                {
                    return StatusCode(400, new
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
                    Message = result.FirstOrDefault()?.Status,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_TimeTable_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(model));
                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }

        //Exam Module
        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_Examtype_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_Examtype_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_Examtype_CRUD_Operations")]
        public IActionResult Tbl_examtype_CRUD_Operations([FromBody] tblExamType fare)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    fare.SchoolID = schoolId;
                }
                var result = dbop.Tbl_ExamType_CRUD_Operations(fare);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                if (result.First().Status == "Exam Type already exists")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
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
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_Examtype_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(fare));
                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_SetExam_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_SetExam_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_SetExam_CRUD_Operations")]
        public IActionResult Tbl_setExam_CRUD_Operations([FromBody] tblSetExam fare)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    fare.SchoolID = schoolId;
                }
                var result = dbop.Tbl_SetExam_CRUD_Operations(fare);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                if (result.First().Status == "Exam already created for this category")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
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
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_SetExam_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(fare));
                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_ExamAttendence_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_ExamAttendence_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_ExamAttendence_CRUD_Operations")]
        public IActionResult Tbl_ExamAttendece_CRUD_Operations([FromBody] tblExamAttendence fare)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    fare.SchoolID = schoolId;
                }
                var result = dbop.Tbl_ExamAttendece_CRUD_Operations(fare);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
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
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_ExamAttendece_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(fare));
                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_ExamMarks_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_ExamMarks_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_ExamMarks_CRUD_Operations")]
        public IActionResult Tbl_ExamMarks_CRUD_Operations([FromBody] tblExamMarks fare)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    fare.SchoolID = schoolId;
                }
                var result = dbop.Tbl_ExamMarks_CRUD_Operations(fare);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
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
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_ExamMarks_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(fare));
                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_StudentAttendance_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_StudentAttendance_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_StudentAttendance_CRUD_Operations")]
        public IActionResult Tbl_StudentAttendance_CRUD_Operations([FromBody] tblStudentAttendance attendance)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    attendance.SchoolID = schoolId;
                }
                var result = dbop.Tbl_StudentAttendance_CRUD_Operations(attendance);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }
                // Duplicate check — catches both class-level and student-level messages
                if (result.First().Status?.ToLower().Contains("already exists") == true)
                {
                    return StatusCode(400, new
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
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_StudentAttendance_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(attendance));
                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }





        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_StaffAttendance_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_StaffAttendance_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_StaffAttendance_CRUD_Operations")]
        public IActionResult Tbl_StaffAttendance_CRUD_Operations([FromBody] tblStaffAttendance attendance)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    attendance.SchoolID = schoolId;
                }
                var result = dbop.Tbl_StaffAttendance_CRUD_Operations(attendance);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }
                if (result.First().Status == "Already exists")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
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
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_StaffAttendance_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(attendance));
                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }

        //Finance Module

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_FeeCategory_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_FeeCategory_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_FeeCategory_CRUD_Operations")]
        public IActionResult Tbl_FeeCategory_CRUD_Operations([FromBody] feeCategory fee)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    fee.SchoolID = schoolId;
                }
                var result = dbop.Tbl_feeCategory_CRUD_Operations(fee);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                if (result.First().Status == "Fee Category Already Exists")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
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
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_feeCategory_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(fee));
                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_FeeAllocation_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_FeeAllocation_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_FeeAllocation_CRUD_Operations")]
        public IActionResult Tbl_feeAllocation_CRUD_Operations([FromBody] tblfeeAllocation fee)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    fee.SchoolID = schoolId;
                }

                var result = dbop.Tbl_feeallocation_CRUD_Operations(fee);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                if (result.First().Status == "Fee already allocated for this category")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
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
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_feeallocation_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(fee));

                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_FeeDiscountCategory_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_FeeDiscountCategory_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_FeeDiscountCategory_CRUD_Operations")]
        public IActionResult Tbl_FeeDiscountCategory_CRUD_Operations([FromBody] tblfeeDiscountCategory fee)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    fee.SchoolID = schoolId;
                }

                var result = dbop.Tbl_FeeDiscountCategory_CRUD_Operations(fee);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                if (result.First().Status == "Fee already Discount for this category")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
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
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_FeeDiscountCategory_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(fee));

                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }

        // Fee Doscount
        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_FeeDiscount_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_FeeDiscount_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_FeeDiscount_CRUD_Operations")]
        public IActionResult Tbl_FeeDiscount_CRUD_Operations([FromBody] tblfeeDiscount fee)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    fee.SchoolID = schoolId;
                }

                var result = dbop.Tbl_FeeDiscount_CRUD_Operations(fee);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                if (result.First().Status == "Discount already assigned to this student")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
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
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_FeeDiscount_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(fee));

                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }


        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_FeeCollection_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_FeeCollection_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_FeeCollection_CRUD_Operations")]
        public IActionResult Tbl_FeeCollection_CRUD_Operations([FromBody] tblFeeCollection fee)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    fee.SchoolID = schoolId;
                }

                var result = dbop.Tbl_FeeCollection_CRUD_Operations(fee);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
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
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_FeeCollection_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(fee));

                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }

        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_StudentTransfer_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_StudentTransfer_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_StudentTransfer_CRUD_Operations")]
        public IActionResult Tbl_StudentTransfer_CRUD_Operations([FromBody] tblStudentTransfer fare)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    fare.SchoolID = schoolId;
                }
                var result = dbop.Tbl_StudentTransfer_CRUD_Operations(fare);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                if (result.First().Status == "Cannot transfer: Fee dues pending")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
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
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_StudentTransfer_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(fare));
                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }


                /// <summary>
        /// API Endpoint: POST api/SchoolManagement/Dashboard_API
        /// Purpose: Fetches statistics (student counts, attendance sums, fee collections) for layouts.
        /// </summary>
    [HttpPost]
        [Route("Dashboard_API")]
        public IActionResult Dashboard_API([FromBody] DashboardRequest req)
        {
            var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
            var schoolId = User.FindFirst("SchoolID")?.Value;
            var schoolIds = User.FindFirst("SchoolIDs")?.Value; // ── group admin JWT claim

            if (roleId == "10")
            {
                // Group Admin:
                // - If they selected a specific school from dropdown → req.SchoolID is already set from body
                // - If no school selected (aggregate view) → use JWT-authoritative SchoolIDs
                // - NEVER trust client-sent SchoolIDs; always override from JWT for security
                if (req.SchoolID == null)
                {
                    // Aggregate view: pass all tagged school IDs from JWT
                    req.SchoolIDs = schoolIds;
                }
                else
                {
                    // Single school selected: verify it belongs to this group admin
                    var allowedIds = (schoolIds ?? "")
                        .Split(',')
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrEmpty(s))
                        .ToHashSet();

                    if (!allowedIds.Contains(req.SchoolID.ToString()))
                    {
                        return StatusCode(403, new
                        {
                            StatusCode = 403,
                            Success = false,
                            Message = "Access denied: school not assigned to this group admin."
                        });
                    }

                    req.SchoolIDs = null; // single school selected, no need for multi-filter
                }
            }
            else if (roleId != "1" && int.TryParse(schoolId, out var sid))
            {
                // School admin / principal / staff: scope to their single school from JWT
                req.SchoolID = sid;
                req.SchoolIDs = null;
            }
            // Super Admin (role 1): no restriction — both stay null → proc returns all

            var data = dbop.GetDashboardData(req);

            return Ok(new { status = true, data = data });
        }

        //[HttpPost]
        //[Route("Dashboard_API")]
        //public IActionResult Dashboard_API([FromBody] DashboardRequest req)
        //{
        //    var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
        //    var schoolId = User.FindFirst("SchoolID")?.Value;

        //    if (roleId != "1" && int.TryParse(schoolId, out var sid))
        //    {
        //        req.SchoolID = sid;
        //    }

        //    var data = dbop.GetDashboardData(req);

        //    return Ok(new
        //    {

        //        status = true,
        //        data = data

        //    });

        //}


        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_SalarySettings_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_SalarySettings_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_SalarySettings_CRUD_Operations")]
        public IActionResult Tbl_SalarySettings_CRUD_Operations([FromBody] TblSalarySetting req)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1" && int.TryParse(schoolId, out var sid))
                {
                    req.SchoolID = sid;
                }

                var result = dbop.Tbl_SalarySettings_CRUD_Operations(req);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                if (result.Count == 0)
                {
                    return Ok(new
                    {
                        StatusCode = 200,
                        Success = true,
                        Message = "No data found.",
                        Data = result
                    });
                }

                var error = result.FirstOrDefault(x => x.Status?.StartsWith("ERROR:", StringComparison.OrdinalIgnoreCase) == true);
                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                var duplicate = result.FirstOrDefault(x => x.Status == "Salary setting already exists for this staff");
                if (duplicate != null)
                {
                    return StatusCode(409, new
                    {
                        StatusCode = 409,
                        Success = false,
                        Message = duplicate.Status,
                        Data = result
                    });
                }

                var notFound = result.FirstOrDefault(x => x.Status == "Salary setting not found");
                if (notFound != null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Success = false,
                        Message = notFound.Status
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Success = true,
                    Message = result.FirstOrDefault()?.Status ?? "Success",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_SalarySettings_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(req));

                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }


        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_SalaryPay_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_SalaryPay_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_SalaryPay_CRUD_Operations")]
        public IActionResult Tbl_SalaryPay_CRUD_Operations([FromBody] TblSalaryPay req)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1" && int.TryParse(schoolId, out var sid))
                    req.SchoolID = sid;

                var result = dbop.Tbl_SalaryPay_CRUD_Operations(req);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                if (result.Count == 0)
                {
                    return Ok(new
                    {
                        StatusCode = 200,
                        Success = true,
                        Message = "No data found.",
                        Data = result
                    });
                }

                var error = result.FirstOrDefault(x => x.Status?.StartsWith("ERROR:", StringComparison.OrdinalIgnoreCase) == true);
                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                var duplicate = result.FirstOrDefault(x => x.Status == "Salary already paid for this staff and month");
                if (duplicate != null)
                {
                    return StatusCode(409, new
                    {
                        StatusCode = 409,
                        Success = false,
                        Message = duplicate.Status,
                        Data = result
                    });
                }

                var notFound = result.FirstOrDefault(x => x.Status == "Salary pay record not found");
                if (notFound != null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Success = false,
                        Message = notFound.Status
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Success = true,
                    Message = result.FirstOrDefault()?.Status ?? "Success",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_SalaryPay_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(req));
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }


        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_PayrollHead_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_PayrollHead_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_PayrollHead_CRUD_Operations")]
        public IActionResult Tbl_PayrollHead_CRUD_Operations([FromBody] tblPayrollHead ph)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    ph.SchoolID = Convert.ToInt32(schoolId);
                }

                var result = dbop.Tbl_PayrollHead_CRUD_Operations(ph);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                if (result.First().Status == "Payroll head already exists")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Success = true,
                    Message = result.FirstOrDefault()?.Status,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_PayrollHead_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(ph));

                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_PaymentMode_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_PaymentMode_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_PaymentMode_CRUD_Operations")]
        public IActionResult Tbl_PaymentMode_CRUD_Operations([FromBody] TblPaymentMode ph)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    ph.SchoolID = Convert.ToInt32(schoolId);
                }

                var result = dbop.Tbl_PaymentMode_CRUD_Operations(ph);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                if (result.First().Status == "Payment mode already exists")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Success = true,
                    Message = result.FirstOrDefault()?.Status,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_PaymentMode_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(ph));

                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_AdvanceSalary_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_AdvanceSalary_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_AdvanceSalary_CRUD_Operations")]
        public IActionResult Tbl_AdvanceSalary_CRUD_Operations([FromBody] TblAdvanceSalary a)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    a.SchoolID = Convert.ToInt64(schoolId);
                }

                // Optional server-side validation for insert/update
                if ((a.Flag == "1" || a.Flag == "5") && a.TenureMonths.HasValue)
                {
                    if (a.TenureMonths < 1 || a.TenureMonths > 24)
                    {
                        return BadRequest(new
                        {
                            StatusCode = 400,
                            Success = false,
                            Message = "Tenure in months must be between 1 and 24."
                        });
                    }
                }

                var result = dbop.Tbl_AdvanceSalary_CRUD_Operations(a);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Success = true,
                    Message = result.FirstOrDefault()?.Status,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_AdvanceSalary_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(a));

                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }


        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_leavePolicy_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_leavePolicy_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_leavePolicy_CRUD_Operations")]
        public IActionResult Tbl_leavePolicy_CRUD_Operations([FromBody] tblLeavepolicy fee)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    fee.SchoolID = schoolId;
                }
                var result = dbop.Tbl_leavePolicy_CRUD_Operations(fee);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                if (result.First().Status == "Leave Policy Already Exists")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
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
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_leavePolicy_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(fee));
                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }



        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_LeaveApplication_Operations
        /// Purpose: Handles transactional and query operations for Tbl_LeaveApplication_Operations.
        /// </summary>
        [HttpPost("Tbl_LeaveApplication_Operations")]
        public IActionResult Tbl_LeaveApplication_Operations([FromBody] tblLeaveApplication obj)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    obj.SchoolID = schoolId;
                }

                // ===== GET OLD FILE (FOR UPDATE) =====
                string? oldAttachmentUrl = null;
                if (obj.Flag == "5" && !string.IsNullOrEmpty(obj.ID))
                {
                    var existing = new tblLeaveApplication
                    {
                        ID = obj.ID,
                        SchoolID = obj.SchoolID,
                        Flag = "4" // FETCH BY ID (make sure SP supports this)
                    };

                    var existingResult = dbop.Tbl_LeaveApplication_CRUD_Operations(existing);
                    if (existingResult?.Any() == true)
                    {
                        oldAttachmentUrl = existingResult[0].AttachmentURL;
                    }
                }

                var result = dbop.Tbl_LeaveApplication_CRUD_Operations(obj);

                if (result == null)
                    return StatusCode(500, new { Success = false, Message = "DB returned null" });

                if (!result.Any())
                    return Ok(new { Success = false, Message = "No data", Data = result });

                var first = result.First();

                // ERROR CHECK
                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return StatusCode(500, new { Success = false, Message = error.Status });
                }

                // ===== DELETE OLD FILE =====
                if (obj.Flag == "5" && !string.IsNullOrEmpty(oldAttachmentUrl))
                {
                    if (oldAttachmentUrl != obj.AttachmentURL)
                    {
                        var oldFile = Path.GetFileName(oldAttachmentUrl);

                        var deleteRequest = new DeleteLeaveFileRequest
                        {
                            SchoolId = obj.SchoolID,
                            LeaveId = obj.ID,
                            FileName = oldFile,
                            ModifiedBy = obj.ModifiedBy,
                            ModifiedIp = obj.ModifiedIp
                        };

                        _ = Task.Run(() => DeleteLeaveFile(deleteRequest));
                    }
                }

                // ===== MOVE FILE AFTER INSERT =====
                if (obj.Flag == "1" && !string.IsNullOrEmpty(obj.AttachmentURL))
                {
                    var newId = result[0].ID;
                    var fileName = Path.GetFileName(obj.AttachmentURL);

                    var move = dbop.MoveLeaveFileToFinal(obj.SchoolID, newId, fileName);

                    if (move.success)
                    {
                        result[0].AttachmentURL = move.newUrl;

                        // update DB with final URL
                        var updateObj = new tblLeaveApplication
                        {
                            ID = newId,
                            SchoolID = obj.SchoolID,
                            AttachmentURL = move.newUrl,
                            Flag = "5",
                            ModifiedBy = obj.CreatedBy,
                            ModifiedIp = obj.CreatedIp
                        };

                        dbop.Tbl_LeaveApplication_CRUD_Operations(updateObj);
                    }
                }

                return Ok(new
                {
                    Success = true,
                    Message = first.Status ?? "Success",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }
                /// <summary>
        /// API Endpoint: POST api/SchoolManagement/upload-leave-doc
        /// Purpose: Uploads medical certificates or documents supporting leave applications.
        /// </summary>
    [HttpPost("upload-leave-doc")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadLeaveDoc([FromForm] LeaveUploadRequest request)
        {
            if (string.IsNullOrEmpty(request.SchoolId))
                return BadRequest("SchoolId required");

            if (request.File == null)
                return BadRequest("File missing");

            var result = await dbop.SaveLeaveFile(
                request.File,
                request.SchoolId,
                request.LeaveId ?? "temp"
            );

            return Ok(new
            {
                url = result.url,
                fileName = result.fileName
            });
        }

            /// <summary>
    /// Data Transfer Object Model: LeaveUploadRequest
    /// </summary>
    public class LeaveUploadRequest
        {
            public IFormFile? File { get; set; }
            public string? SchoolId { get; set; }
            public string? LeaveId { get; set; }
        }
        /// <summary>
        /// API Endpoint: Delete api/SchoolManagement/delete-leave-file
        /// Purpose: Handles transactional and query operations for delete-leave-file.
        /// </summary>
        [HttpDelete("delete-leave-file")]
        public IActionResult DeleteLeaveFile([FromBody] DeleteLeaveFileRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.SchoolId) || string.IsNullOrEmpty(request.FileName))
                    return BadRequest("SchoolId & FileName required");

                var paths = new List<string>
        {
            Path.Combine(Directory.GetCurrentDirectory(),"Uploads",request.SchoolId,"Leave",request.LeaveId ?? "temp",request.FileName),
            Path.Combine(Directory.GetCurrentDirectory(),"Uploads",request.SchoolId,"Leave","temp",request.FileName)
        };

                string? found = paths.FirstOrDefault(System.IO.File.Exists);

                if (found != null)
                    System.IO.File.Delete(found);

                // clear DB
                if (!string.IsNullOrEmpty(request.LeaveId))
                {
                    var update = new tblLeaveApplication
                    {
                        ID = request.LeaveId,
                        SchoolID = request.SchoolId,
                        AttachmentURL = "",
                        Flag = "5",
                        ModifiedBy = request.ModifiedBy,
                        ModifiedIp = request.ModifiedIp
                    };

                    dbop.Tbl_LeaveApplication_CRUD_Operations(update);
                }

                return Ok(new { message = "Deleted" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

            /// <summary>
    /// Data Transfer Object Model: DeleteLeaveFileRequest
    /// </summary>
    public class DeleteLeaveFileRequest
        {
            public string? SchoolId { get; set; }
            public string? LeaveId { get; set; }
            public string? FileName { get; set; }
            public string? ModifiedBy { get; set; }
            public string? ModifiedIp { get; set; }
        }


        //    [AllowAnonymous]
        //    [HttpPost("upload-student-docs")]
        //    [Consumes("multipart/form-data")]
        //    public async Task<IActionResult> UploadStudentDocs([FromForm] StudentUploadRequest request)
        //    {
        //        // PROFILE
        //        if (request.FileType == "Profile")
        //        {
        //            if (request.File == null)
        //                return BadRequest("No profile file");

        //            // delete old profile
        //            dbop.Tbl_StudentDocumentsUpload_CRUD(new StudentDocumentsUpload
        //            {
        //                AdmissionID = request.AdmissionId,
        //                FileType = "Profile",
        //                Flag = "6"
        //            });

        //            var result = await _fileService.SaveStudentFile(
        //                request.File,
        //                request.SchoolId,
        //                request.AdmissionId
        //            );

        //            dbop.Tbl_StudentDocumentsUpload_CRUD(new StudentDocumentsUpload
        //            {
        //                AdmissionID = request.AdmissionId,
        //                FileName = result.fileName,
        //                FileType = "Profile",
        //                FilePath = result.url,
        //                Flag = "1"
        //            });

        //            return Ok(result.url);
        //        }

        //        // DOCUMENTS
        //        if (request.Files == null || request.Files.Count == 0)
        //            return BadRequest("No files");

        //        foreach (var file in request.Files)
        //        {
        //            var result = await _fileService.SaveStudentFile(
        //                file,
        //                request.SchoolId,
        //                request.AdmissionId
        //            );

        //            dbop.Tbl_StudentDocumentsUpload_CRUD(new StudentDocumentsUpload
        //            {
        //                AdmissionID = request.AdmissionId,
        //                FileName = result.fileName,
        //                FileType = "Document",
        //                FilePath = result.url,
        //                Flag = "1"
        //            });
        //        }

        //        return Ok("Uploaded");
        //    }

        //    [HttpPost("upload-school-logo")]
        //    [Consumes("multipart/form-data")]
        //    public async Task<IActionResult> UploadSchoolLogo([FromForm] LogoUploadRequest request)
        //    {
        //        if (request.File == null)
        //            return BadRequest("No file uploaded");

        //        var result = await _fileService.SaveSchoolLogo(request.File, request.SchoolId);

        //        var school = new SchoolDetails
        //        {
        //            SchoolID = request.SchoolId,
        //            Flag = "5"
        //        };

        //        dbop.Tbl_SchoolDetails_CRUD(school);

        //        return Ok(new { url = result.url });
        //    }

        //    [HttpGet("get-student-files/{admissionId}")]
        //    public IActionResult GetStudentFiles(string admissionId)
        //    {
        //        var data = dbop.Tbl_StudentDocumentsUpload_CRUD(new StudentDocumentsUpload
        //        {
        //            AdmissionID = admissionId,
        //            Flag = "4" // GET
        //        });

        //        return Ok(data);
        //    }

        //    [HttpGet("student/{schoolId}/{admissionId}/{fileName}")]
        //    public IActionResult GetStudentFile(string schoolId, string admissionId, string fileName)
        //    {
        //        try
        //        {
        //            // 🔥 Decode URL (handles spaces like %20)
        //            fileName = Uri.UnescapeDataString(fileName);

        //            // 🔥 Build full physical path
        //            var path = Path.Combine(
        //                Directory.GetCurrentDirectory(),
        //                "Uploads",
        //                schoolId,
        //                "Students",
        //                admissionId,
        //                fileName
        //            );

        //            if (!System.IO.File.Exists(path))
        //                return NotFound($"File not found: {path}");

        //            // 🔥 Return correct content type (image/pdf preview works)
        //            var provider = new FileExtensionContentTypeProvider();
        //            if (!provider.TryGetContentType(fileName, out var contentType))
        //                contentType = "application/octet-stream";

        //            return PhysicalFile(path, contentType);
        //        }
        //        catch (Exception ex)
        //        {
        //            return StatusCode(500, ex.Message);
        //        }
        //    }

        //    [HttpGet("logo/{schoolId}/{fileName}")]
        //    public IActionResult GetLogo(string schoolId, string fileName)
        //    {
        //        var path = _fileService.GetFilePath(schoolId, "LOGO", fileName);

        //        if (!System.IO.File.Exists(path))
        //            return NotFound();

        //        return PhysicalFile(path, "image/png");
        //    }

        //    [HttpDelete("delete-student-file")]
        //    public IActionResult DeleteStudentFile(
        //string schoolId,
        //string admissionId,
        //string fileName)
        //    {
        //        try
        //        {
        //            var path = _fileService.GetFilePath(schoolId, "Students", fileName, admissionId);

        //            if (System.IO.File.Exists(path))
        //                System.IO.File.Delete(path);

        //            // 👉 Update DB (soft delete or flag-based)
        //            var doc = new StudentDocumentsUpload
        //            {
        //                AdmissionID = admissionId,
        //                FileName = fileName,
        //                Flag = "5" // update/delete
        //            };

        //            dbop.Tbl_StudentDocumentsUpload_CRUD(doc);

        //            return Ok("Deleted successfully");
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError(ex, "Delete failed");
        //            return BadRequest(ex.Message);
        //        }
        //    }

                /// <summary>
        /// API Endpoint: POST api/SchoolManagement/upload-student-docs
        /// Purpose: Saves verification documents during student registration processes.
        /// </summary>
    [HttpPost("upload-student-docs")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadStudentDocs([FromForm] StudentUploadRequest request)
        {
            if (string.IsNullOrEmpty(request.SchoolId) ||
                string.IsNullOrEmpty(request.AdmissionId))
                return BadRequest("Invalid data");

            // ===== PROFILE =====
            if (request.FileType == "Profile")
            {
                if (request.File == null)
                    return BadRequest("Profile file missing");

                var result = await _fileService.SaveStudentFile(
                    request.File,
                    request.SchoolId,
                    request.AdmissionId
                );

                dbop.Tbl_StudentDocumentsUpload_CRUD(new StudentDocumentsUpload
                {
                    AdmissionID = request.AdmissionId,
                    FileName = result.fileName,
                    FileType = "Profile",
                    FilePath = result.url,
                    Flag = "1"
                });

                return Ok(result.url);
            }

            // ===== DOCUMENTS =====
            if (request.Files == null || request.Files.Count == 0)
                return BadRequest("No files");

            foreach (var file in request.Files)
            {
                var result = await _fileService.SaveStudentFile(
                    file,
                    request.SchoolId,
                    request.AdmissionId
                );

                dbop.Tbl_StudentDocumentsUpload_CRUD(new StudentDocumentsUpload
                {
                    AdmissionID = request.AdmissionId,
                    FileName = result.fileName,
                    FileType = "Document",
                    FilePath = result.url,
                    Flag = "1"
                });
            }

            return Ok("Uploaded");
        }

        // ================= GET FILES =================
        /// <summary>
        /// API Endpoint: Get api/SchoolManagement/get-student-files/{admissionId}
        /// Purpose: Handles transactional and query operations for get-student-files/{admissionId}.
        /// </summary>
        [HttpGet("get-student-files/{admissionId}")]
        public IActionResult GetStudentFiles(string admissionId)
        {
            var data = dbop.Tbl_StudentDocumentsUpload_CRUD(new StudentDocumentsUpload
            {
                AdmissionID = admissionId,
                Flag = "4" // only active
            });

            return Ok(data);
        }

        // ================= DELETE =================
        /// <summary>
        /// API Endpoint: Delete api/SchoolManagement/delete-student-file
        /// Purpose: Handles transactional and query operations for delete-student-file.
        /// </summary>
        [HttpDelete("delete-student-file")]
        public IActionResult DeleteStudentFile([FromBody] DeleteFileRequest req)
        {
            try
            {
                var path = _fileService.GetFilePath(
                    req.SchoolId,
                    "Students",
                    req.FileName,
                    req.AdmissionId
                );

                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);

                dbop.Tbl_StudentDocumentsUpload_CRUD(new StudentDocumentsUpload
                {
                    AdmissionID = req.AdmissionId,
                    FileName = req.FileName,
                    IsActive = "0",
                    Flag = "9"
                });

                return Ok("Deleted");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ================= SERVE FILE =================
        [AllowAnonymous]
        /// <summary>
        /// API Endpoint: Get api/SchoolManagement/student/{schoolId}/{admissionId}/{fileName}
        /// Purpose: Handles transactional and query operations for student/{schoolId}/{admissionId}/{fileName}.
        /// </summary>
        [HttpGet("student/{schoolId}/{admissionId}/{fileName}")]
        public IActionResult GetStudentFile(string schoolId, string admissionId, string fileName)
        {
            fileName = Uri.UnescapeDataString(fileName);

            var path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Uploads",
                schoolId,
                "Students",
                admissionId,
                fileName
            );

            if (!System.IO.File.Exists(path))
                return NotFound();


            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var contentType))
                contentType = "application/octet-stream";

            return PhysicalFile(path, contentType);
        }

                /// <summary>
        /// API Endpoint: POST api/SchoolManagement/upload-school-logo
        /// Purpose: Configures and updates school logo image parameters.
        /// </summary>
    [HttpPost("upload-school-logo")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadSchoolLogo([FromForm] StudentUploadRequest request)
        {
            if (string.IsNullOrEmpty(request.SchoolId))
                return BadRequest("SchoolId required");

            if (request.File == null)
                return BadRequest("File missing");

            var result = await _fileService.SaveSchoolFile(
                request.File,
                request.SchoolId
            );

            dbop.Proc_Tbl_SchoolFiles(new SchoolFile
            {
                SchoolID = request.SchoolId,
                FileName = result.fileName,
                FilePath = result.url,
                FileType = "SchoolLogo",
                Flag = "1"
            });

            return Ok(new { filePath = result.url });
        }

        /// <summary>
        /// API Endpoint: Get api/SchoolManagement/get-school-logo/{schoolId}
        /// Purpose: Handles transactional and query operations for get-school-logo/{schoolId}.
        /// </summary>
        [HttpGet("get-school-logo/{schoolId}")]
        public IActionResult GetSchoolLogo(string schoolId)
        {
            var data = dbop.Proc_Tbl_SchoolFiles(new SchoolFile
            {
                SchoolID = schoolId,
                FileType = "SchoolLogo",
                Flag = "4"
            });

            return Ok(data.FirstOrDefault());
        }

        /// <summary>
        /// API Endpoint: Delete api/SchoolManagement/delete-school-logo
        /// Purpose: Handles transactional and query operations for delete-school-logo.
        /// </summary>
        [HttpDelete("delete-school-logo")]
        public IActionResult DeleteSchoolLogo([FromBody] SchoolFile req)
        {
            dbop.Proc_Tbl_SchoolFiles(new SchoolFile
            {
                SchoolID = req.SchoolID,
                FileName = req.FileName,
                Flag = "9"
            });

            return Ok("Deleted");
        }

        [AllowAnonymous]
        /// <summary>
        /// API Endpoint: Get api/SchoolManagement/student/{schoolId}/School/{fileName}
        /// Purpose: Handles transactional and query operations for student/{schoolId}/School/{fileName}.
        /// </summary>
        [HttpGet("student/{schoolId}/School/{fileName}")]
        public IActionResult GetSchoolFile(string schoolId, string fileName)
        {
            fileName = Uri.UnescapeDataString(fileName);

            var path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Uploads",
                schoolId,
                "School",
                fileName
            );

            if (!System.IO.File.Exists(path))
            {
                Console.WriteLine("NOT FOUND: " + path);
                return NotFound();
            }

            return PhysicalFile(path, "application/octet-stream");
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_Homework_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_Homework_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_Homework_CRUD_Operations")]
        public IActionResult Tbl_Homework_CRUD_Operations([FromBody] tblHomework obj)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    obj.SchoolID = schoolId;
                }

                var result = dbop.Tbl_Homework_CRUD_Operations(obj);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                var first = result.FirstOrDefault();

                if (first == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "No data returned from database."
                    });
                }

                if (first.Status?.ToLower().Contains("error") == true)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = first.Status
                    });
                }

                if (first.Status == "Homework already exists")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = first.Status,
                        Data = result
                    });
                }

                // ===== MOVE FILE FROM TEMP TO FINAL AFTER INSERT =====
                if (obj.Flag == "1" && result.Count > 0 && !string.IsNullOrEmpty(obj.AttachmentURL))
                {
                    var newHomeworkId = result[0].ID;
                    var fileName = Path.GetFileName(obj.AttachmentURL);

                    Console.WriteLine($"[MOVE] Insert success. HomeworkID: {newHomeworkId}, File: {fileName}");

                    // Move file from temp to final folder
                    var moveResult = dbop.MoveHomeworkFileToFinal(obj.SchoolID, newHomeworkId, fileName);

                    if (moveResult.success)
                    {
                        Console.WriteLine($"[MOVE] File moved to: {moveResult.newUrl}");

                        // Update result with new URL
                        result[0].AttachmentURL = moveResult.newUrl;

                        // Update DB with correct URL
                        var updateObj = new tblHomework
                        {
                            ID = newHomeworkId,
                            SchoolID = obj.SchoolID,
                            AttachmentURL = moveResult.newUrl,
                            Flag = "5",
                            ModifiedBy = obj.CreatedBy,
                            ModifiedIp = obj.CreatedIp
                        };
                        dbop.Tbl_Homework_CRUD_Operations(updateObj);
                    }
                    else
                    {
                        Console.WriteLine($"[MOVE] Failed to move file: {fileName}");
                    }
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Success = true,
                    Message = first.Status,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                dbop.LogException(
                    ex,
                    "SchoolManagementController",
                    "Tbl_Homework_CRUD_Operations",
                    Newtonsoft.Json.JsonConvert.SerializeObject(obj)
                );

                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }
        
                /// <summary>
        /// API Endpoint: POST api/SchoolManagement/upload-homework-doc
        /// Purpose: Allows teaching staff to upload homework assignments.
        /// </summary>
    [HttpPost("upload-homework-doc")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadHomeworkDoc([FromForm] HomeworkUploadRequest request)
        {
            if (string.IsNullOrEmpty(request.SchoolId))
                return BadRequest("SchoolId is required");

            if (request.File == null)
                return BadRequest("No file uploaded");

            // Call DAL method
            var result = await dbop.SaveHomeworkFile(request.File, request.SchoolId, request.HomeworkId ?? "temp");

            return Ok(new { url = result.url, fileName = result.fileName });
        }

            /// <summary>
    /// Data Transfer Object Model: HomeworkUploadRequest
    /// </summary>
    public class HomeworkUploadRequest
        {
            public IFormFile? File { get; set; }
            public string? SchoolId { get; set; }
            public string? HomeworkId { get; set; }
        }

        /// <summary>
        /// API Endpoint: Delete api/SchoolManagement/delete-homework-file
        /// Purpose: Handles transactional and query operations for delete-homework-file.
        /// </summary>
        [HttpDelete("delete-homework-file")]
        public IActionResult DeleteHomeworkFile([FromBody] DeleteHomeworkFileRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.SchoolId) || string.IsNullOrEmpty(request.FileName))
                    return BadRequest("SchoolId and FileName required");

                // Try multiple possible locations
                var possiblePaths = new List<string>
                {
                    // Primary: Homework/{id}/
                    Path.Combine(Directory.GetCurrentDirectory(), "Uploads",
                        request.SchoolId, "Homework", request.HomeworkId ?? "temp", request.FileName),
                    // Fallback: Homework/temp/ (file uploaded before save)
                    Path.Combine(Directory.GetCurrentDirectory(), "Uploads",
                        request.SchoolId, "Homework", "temp", request.FileName),
                    // Fallback: direct in Uploads/{schoolId}/
                    Path.Combine(Directory.GetCurrentDirectory(), "Uploads",
                        request.SchoolId, request.FileName)
                };

                string? foundPath = null;
                foreach (var path in possiblePaths)
                {
                    Console.WriteLine($"[DELETE] Checking: {path}");
                    if (System.IO.File.Exists(path))
                    {
                        foundPath = path;
                        break;
                    }
                }

                // If still not found, search recursively in school folder
                if (foundPath == null)
                {
                    var schoolFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", request.SchoolId);
                    if (Directory.Exists(schoolFolder))
                    {
                        var files = Directory.GetFiles(schoolFolder, request.FileName, SearchOption.AllDirectories);
                        if (files.Length > 0)
                        {
                            foundPath = files[0];
                            Console.WriteLine($"[DELETE] Found via recursive search: {foundPath}");
                        }
                    }
                }

                bool fileDeleted = false;
                if (foundPath != null)
                {
                    System.IO.File.Delete(foundPath);
                    fileDeleted = true;
                    Console.WriteLine($"[DELETE] Physical file deleted: {foundPath}");
                }
                else
                {
                    Console.WriteLine($"[DELETE] File not found in any location: {request.FileName}");
                }

                // 2. Clear AttachmentURL in database if HomeworkId is valid
                bool dbUpdated = false;
                string? dbError = null;

                if (!string.IsNullOrEmpty(request.HomeworkId) && request.HomeworkId != "temp" &&
                    int.TryParse(request.HomeworkId, out int homeworkId))
                {
                    try
                    {
                        var updateObj = new tblHomework
                        {
                            ID = request.HomeworkId,
                            SchoolID = request.SchoolId,
                            AttachmentURL = "", // Clear the attachment URL
                            Flag = "5", // UPDATE flag
                            ModifiedBy = request.ModifiedBy,
                            ModifiedIp = request.ModifiedIp
                        };

                        var result = dbop.Tbl_Homework_CRUD_Operations(updateObj);
                        dbUpdated = true;
                        Console.WriteLine($"[DELETE] Database AttachmentURL cleared for HomeworkID: {homeworkId}, Result: {Newtonsoft.Json.JsonConvert.SerializeObject(result)}");
                    }
                    catch (Exception dbEx)
                    {
                        dbError = dbEx.Message;
                        Console.WriteLine($"[DELETE DB ERROR] {dbEx.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"[DELETE] Skipping DB update - HomeworkId: {request.HomeworkId}");
                }

                return Ok(new
                {
                    message = fileDeleted ? "File deleted successfully" : "File not found on disk",
                    fileName = request.FileName,
                    dbUpdated = dbUpdated,
                    dbError = dbError,
                    searchedPaths = possiblePaths
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DELETE ERROR] {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

            /// <summary>
    /// Data Transfer Object Model: DeleteHomeworkFileRequest
    /// </summary>
    public class DeleteHomeworkFileRequest
        {
            public string? SchoolId { get; set; }
            public string? HomeworkId { get; set; }
            public string? FileName { get; set; }
            public string? ModifiedBy { get; set; }
            public string? ModifiedIp { get; set; }
        }


        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_HomeworkSubmission_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_HomeworkSubmission_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_HomeworkSubmission_CRUD_Operations")]
        public IActionResult Tbl_HomeworkSubmission_CRUD_Operations([FromBody] tblHomeworkSubmission obj)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    obj.SchoolID = schoolId;
                }

                // ===== HANDLE OLD FILE DELETION ON UPDATE =====
                string? oldAttachmentUrl = null;
                if (obj.Flag == "5" && !string.IsNullOrEmpty(obj.ID))
                {
                    // Get existing submission to check for old attachment
                    var existingSubmission = new tblHomeworkSubmission
                    {
                        ID = obj.ID,
                        SchoolID = obj.SchoolID,
                        Flag = "4" // FETCH BY ID
                    };
                    var existingResult = dbop.Tbl_HomeworkSubmission_CRUD_Operations(existingSubmission);

                    if (existingResult?.Any() == true)
                    {
                        oldAttachmentUrl = existingResult[0].AttachmentURL;
                    }
                }

                var result = dbop.Tbl_HomeworkSubmission_CRUD_Operations(obj);

                // ✅ NULL CHECK
                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                // ✅ EMPTY LIST CHECK
                if (!result.Any())
                {
                    return Ok(new
                    {
                        StatusCode = 200,
                        Success = false,
                        Message = "No data returned from database.",
                        Data = result
                    });
                }

                // ✅ SAFE ACCESS
                var first = result.FirstOrDefault();

                // ERROR CHECK
                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                // DUPLICATE CHECK
                if (first?.Status == "Homework already submitted by this student")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = first.Status,
                        Data = result
                    });
                }

                // ===== DELETE OLD FILE IF NEW FILE UPLOADED OR ATTACHMENT CLEARED =====
                if (obj.Flag == "5" && !string.IsNullOrEmpty(oldAttachmentUrl))
                {
                    // Case 1: New file uploaded (AttachmentURL changed)
                    // Case 2: Attachment cleared (AttachmentURL is empty)
                    if (oldAttachmentUrl != obj.AttachmentURL)
                    {
                        var oldFileName = Path.GetFileName(oldAttachmentUrl);
                        Console.WriteLine($"[DELETE OLD] Removing old attachment: {oldFileName}");

                        var deleteRequest = new DeleteHomeworkSubmissionFileRequest
                        {
                            SchoolId = obj.SchoolID,
                            SubmissionId = obj.ID,
                            FileName = oldFileName,
                            ModifiedBy = obj.ModifiedBy,
                            ModifiedIp = obj.ModifiedIp
                        };

                        // Delete old file (don't wait for response)
                        _ = Task.Run(() => DeleteHomeworkSubmissionFile(deleteRequest));
                    }
                }

                // ===== MOVE FILE FROM TEMP TO FINAL AFTER INSERT =====
                if (obj.Flag == "1" && result.Count > 0 && !string.IsNullOrEmpty(obj.AttachmentURL))
                {
                    var newSubmissionId = result[0].ID;
                    var fileName = Path.GetFileName(obj.AttachmentURL);

                    Console.WriteLine($"[MOVE SUBMISSION] Insert success. SubmissionID: {newSubmissionId}, File: {fileName}");

                    // Move file from temp to final folder
                    var moveResult = dbop.MoveHomeworkSubmissionFileToFinal(obj.SchoolID, newSubmissionId, fileName);

                    if (moveResult.success)
                    {
                        Console.WriteLine($"[MOVE SUBMISSION] File moved to: {moveResult.newUrl}");

                        // Update result with new URL
                        result[0].AttachmentURL = moveResult.newUrl;

                        // Update DB with correct URL
                        var updateObj = new tblHomeworkSubmission
                        {
                            ID = newSubmissionId,
                            SchoolID = obj.SchoolID,
                            AttachmentURL = moveResult.newUrl,
                            Flag = "5",
                            ModifiedBy = obj.CreatedBy,
                            ModifiedIp = obj.CreatedIp
                        };
                        dbop.Tbl_HomeworkSubmission_CRUD_Operations(updateObj);
                    }
                    else
                    {
                        Console.WriteLine($"[MOVE SUBMISSION] Failed to move file: {fileName}");
                    }
                }

                // SUCCESS
                return Ok(new
                {
                    StatusCode = 200,
                    Success = true,
                    Message = first?.Status ?? "Success",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_HomeworkSubmission_CRUD_Operations",
                    Newtonsoft.Json.JsonConvert.SerializeObject(obj));

                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }
        // Add these methods to your SchoolManagementController.cs

                /// <summary>
        /// API Endpoint: POST api/SchoolManagement/upload-homework-submission-doc
        /// Purpose: Allows students to upload files submitting their homework.
        /// </summary>
    [HttpPost("upload-homework-submission-doc")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadHomeworkSubmissionDoc([FromForm] HomeworkSubmissionUploadRequest request)
        {
            if (string.IsNullOrEmpty(request.SchoolId))
                return BadRequest("SchoolId is required");

            if (request.File == null)
                return BadRequest("No file uploaded");

            // Call DAL method
            var result = await dbop.SaveHomeworkSubmissionFile(request.File, request.SchoolId, request.SubmissionId ?? "temp");

            return Ok(new { url = result.url, fileName = result.fileName });
        }

            /// <summary>
    /// Data Transfer Object Model: HomeworkSubmissionUploadRequest
    /// </summary>
    public class HomeworkSubmissionUploadRequest
        {
            public IFormFile? File { get; set; }
            public string? SchoolId { get; set; }
            public string? SubmissionId { get; set; }
        }

        /// <summary>
        /// API Endpoint: Delete api/SchoolManagement/delete-homework-submission-file
        /// Purpose: Handles transactional and query operations for delete-homework-submission-file.
        /// </summary>
        [HttpDelete("delete-homework-submission-file")]
        public IActionResult DeleteHomeworkSubmissionFile([FromBody] DeleteHomeworkSubmissionFileRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.SchoolId) || string.IsNullOrEmpty(request.FileName))
                    return BadRequest("SchoolId and FileName required");

                // Try multiple possible locations
                var possiblePaths = new List<string>
        {
            // Primary: HomeworkSubmission/{id}/
            Path.Combine(Directory.GetCurrentDirectory(), "Uploads",
                request.SchoolId, "HomeworkSubmission", request.SubmissionId ?? "temp", request.FileName),
            // Fallback: HomeworkSubmission/temp/
            Path.Combine(Directory.GetCurrentDirectory(), "Uploads",
                request.SchoolId, "HomeworkSubmission", "temp", request.FileName),
            // Fallback: direct in Uploads/{schoolId}/
            Path.Combine(Directory.GetCurrentDirectory(), "Uploads",
                request.SchoolId, request.FileName)
        };

                string? foundPath = null;
                foreach (var path in possiblePaths)
                {
                    Console.WriteLine($"[DELETE SUBMISSION] Checking: {path}");
                    if (System.IO.File.Exists(path))
                    {
                        foundPath = path;
                        break;
                    }
                }

                // If still not found, search recursively
                if (foundPath == null)
                {
                    var schoolFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", request.SchoolId);
                    if (Directory.Exists(schoolFolder))
                    {
                        var files = Directory.GetFiles(schoolFolder, request.FileName, SearchOption.AllDirectories);
                        if (files.Length > 0)
                        {
                            foundPath = files[0];
                            Console.WriteLine($"[DELETE SUBMISSION] Found via recursive search: {foundPath}");
                        }
                    }
                }

                bool fileDeleted = false;
                if (foundPath != null)
                {
                    System.IO.File.Delete(foundPath);
                    fileDeleted = true;
                    Console.WriteLine($"[DELETE SUBMISSION] Physical file deleted: {foundPath}");
                }

                // Clear AttachmentURL in database if SubmissionId is valid
                bool dbUpdated = false;
                if (!string.IsNullOrEmpty(request.SubmissionId) && request.SubmissionId != "temp" &&
                    int.TryParse(request.SubmissionId, out int submissionId))
                {
                    try
                    {
                        var updateObj = new tblHomeworkSubmission
                        {
                            ID = request.SubmissionId,
                            SchoolID = request.SchoolId,
                            AttachmentURL = "", // Clear the attachment URL
                            Flag = "5", // UPDATE flag
                            ModifiedBy = request.ModifiedBy,
                            ModifiedIp = request.ModifiedIp
                        };

                        var result = dbop.Tbl_HomeworkSubmission_CRUD_Operations(updateObj);
                        dbUpdated = true;
                    }
                    catch (Exception dbEx)
                    {
                        Console.WriteLine($"[DELETE SUBMISSION DB ERROR] {dbEx.Message}");
                    }
                }

                return Ok(new
                {
                    message = fileDeleted ? "File deleted successfully" : "File not found on disk",
                    fileName = request.FileName,
                    dbUpdated = dbUpdated,
                    searchedPaths = possiblePaths
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DELETE SUBMISSION ERROR] {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

            /// <summary>
    /// Data Transfer Object Model: DeleteHomeworkSubmissionFileRequest
    /// </summary>
    public class DeleteHomeworkSubmissionFileRequest
        {
            public string? SchoolId { get; set; }
            public string? SubmissionId { get; set; }
            public string? FileName { get; set; }
            public string? ModifiedBy { get; set; }
            public string? ModifiedIp { get; set; }
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_HolidayCalendar_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_HolidayCalendar_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_HolidayCalendar_CRUD_Operations")]
        public IActionResult Tbl_HolidayCalendar_CRUD_Operations([FromBody] TblHolidayCalendar fee)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    fee.SchoolID = schoolId;
                }

                var result = dbop.Tbl_HolidayCalendar_CRUD_Operations(fee);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                var msg = result.First().Status;

                if (msg == "Holiday already exists" ||
                    msg == "Holiday date range overlaps with existing holiday")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = msg,
                        Data = result
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Success = true,
                    Message = msg,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_HolidayCalendar_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(fee));

                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_Notices_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_Notices_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_Notices_CRUD_Operations")]
        public IActionResult Tbl_Notices_CRUD_Operations([FromBody] TblNotices req)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    req.SchoolID = Convert.ToInt32(schoolId);
                }

                var result = dbop.Tbl_Notices_CRUD_Operations(req);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                if (result.Count == 0)
                {
                    return Ok(new
                    {
                        StatusCode = 200,
                        Success = true,
                        Message = "No data found.",
                        Data = result
                    });
                }

                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);
                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                if (result.First().Status == "Notice already exists")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Success = true,
                    Message = result.FirstOrDefault()?.Status ?? "Success",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_Notices_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(req));

                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }


        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_HostelMaster_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_HostelMaster_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_HostelMaster_CRUD_Operations")]
        public IActionResult Tbl_HostelMaster_CRUD_Operations([FromBody] Tbl_HostelMaster hostel)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    hostel.SchoolID = schoolId;
                }

                var result = dbop.Tbl_HostelMaster_CRUD_Operations(hostel);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                if (result.Count == 0)
                {
                    return Ok(new
                    {
                        StatusCode = 200,
                        Success = true,
                        Message = "No data found.",
                        Data = result
                    });
                }

                var error = result.FirstOrDefault
                (
                    x => x.Status?.ToLower().Contains("error") == true
                );

                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                if (result.First().Status == "Hostel already exists")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Success = true,
                    Message = result.FirstOrDefault()?.Status ?? "Success",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                dbop.LogException(
                    ex,
                    "SchoolManagementController",
                    "Tbl_HostelMaster_CRUD_Operations",
                    Newtonsoft.Json.JsonConvert.SerializeObject(hostel)
                );

                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }




        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_RoomMaster_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_RoomMaster_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_RoomMaster_CRUD_Operations")]
        public IActionResult Tbl_RoomMaster_CRUD_Operations([FromBody] Tbl_RoomMaster room)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;

                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    room.SchoolID = schoolId;
                }

                var result = dbop.Tbl_RoomMaster_CRUD_Operations(room);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                if (result.Count == 0)
                {
                    return Ok(new
                    {
                        StatusCode = 200,
                        Success = true,
                        Message = "No data found.",
                        Data = result
                    });
                }

                var error = result.FirstOrDefault
                (
                    x => x.Status?.ToLower().Contains("error") == true
                );

                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                if (result.First().Status == "Room already exists")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Success = true,
                    Message = result.FirstOrDefault()?.Status ?? "Success",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                dbop.LogException(
                    ex,
                    "SchoolManagementController",
                    "Tbl_RoomMaster_CRUD_Operations",
                    Newtonsoft.Json.JsonConvert.SerializeObject(room)
                );

                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }



        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_RoomAllotment_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_RoomAllotment_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_RoomAllotment_CRUD_Operations")]
        public IActionResult Tbl_RoomAllotment_CRUD_Operations([FromBody] Tbl_RoomAllotment allotment)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;

                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    allotment.SchoolID = schoolId;
                }

                var result = dbop.Tbl_RoomAllotment_CRUD_Operations(allotment);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                if (result.Count == 0)
                {
                    return Ok(new
                    {
                        StatusCode = 200,
                        Success = true,
                        Message = "No data found.",
                        Data = result
                    });
                }

                var error = result.FirstOrDefault
                (
                    x => x.Status?.ToLower().Contains("error") == true
                );

                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                if (
                    result.First().Status == "Student already allotted to another room"
                    ||
                    result.First().Status == "Room capacity exceeded"
                )
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Success = true,
                    Message = result.FirstOrDefault()?.Status ?? "Success",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                dbop.LogException(
                    ex,
                    "SchoolManagementController",
                    "Tbl_RoomAllotment_CRUD_Operations",
                    Newtonsoft.Json.JsonConvert.SerializeObject(allotment)
                );

                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }


        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_OutPass_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_OutPass_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_OutPass_CRUD_Operations")]
        public IActionResult Tbl_OutPass_CRUD_Operations([FromBody] Tbl_OutPass outpass)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;

                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    outpass.SchoolID = schoolId;
                }

                var result = dbop.Tbl_OutPass_CRUD_Operations(outpass);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                if (result.Count == 0)
                {
                    return Ok(new
                    {
                        StatusCode = 200,
                        Success = true,
                        Message = "No data found.",
                        Data = result
                    });
                }

                var error = result.FirstOrDefault
                (
                    x => x.Status?.ToLower().Contains("error") == true
                );

                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                if (result.First().Status == "Student already has active outpass")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Success = true,
                    Message = result.FirstOrDefault()?.Status ?? "Success",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                dbop.LogException(
                    ex,
                    "SchoolManagementController",
                    "Tbl_OutPass_CRUD_Operations",
                    Newtonsoft.Json.JsonConvert.SerializeObject(outpass)
                );

                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_Units_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_Units_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_Units_CRUD_Operations")]
        public IActionResult Tbl_Units_CRUD_Operations([FromBody] tblUnits unit)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                // If not super admin
                if (roleId != "1")
                {
                    unit.SchoolID = schoolId;
                }

                var result = dbop.Tbl_Units_CRUD_Operations(unit);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                // Check for DAL exception
                var error = result.FirstOrDefault(x =>
                    x.Status?.ToLower().Contains("error") == true);

                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                // Duplicate validation
                if (result.First().Status == "Unit already exists")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
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
            catch (Exception ex)
            {
                dbop.LogException(
                    ex,
                    "SchoolManagementController",
                    "Tbl_Units_CRUD_Operations",
                    Newtonsoft.Json.JsonConvert.SerializeObject(unit)
                );

                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_Categories_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_Categories_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_Categories_CRUD_Operations")]
        public IActionResult Tbl_Categories_CRUD_Operations([FromBody] tblCategories category)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    category.SchoolID = schoolId;
                }

                var result = dbop.Tbl_Categories_CRUD_Operations(category);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);

                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                if (result.First().Status == "Category already exists")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
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
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_Categories_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(category));

                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_Items_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_Items_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_Items_CRUD_Operations")]
        public IActionResult Tbl_Items_CRUD_Operations([FromBody] tblItems items)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    items.SchoolID = schoolId;
                }

                var result = dbop.Tbl_Items_CRUD_Operations(items);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                var error = result.FirstOrDefault(x => x.Status?.ToLower().Contains("error") == true);

                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                if (result.First().Status == "Item already exists" || result.First().Status == "Invalid CGST Percentage" || result.First().Status == "Invalid SGST Percentage")
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
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
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_Items_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(items));

                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_Suppliers_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_Suppliers_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_Suppliers_CRUD_Operations")]
        public IActionResult Tbl_Suppliers_CRUD_Operations([FromBody] tblSuppliers supplier)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    supplier.SchoolID = schoolId;
                }

                var result = dbop.Tbl_Suppliers_CRUD_Operations(supplier);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                var error = result.FirstOrDefault(x =>
                    x.Status?.ToLower().Contains("error") == true);

                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                var validationMessages = new List<string>
        {
            "Supplier Already Exists",
            "Invalid Email Address",
            "Invalid Phone Number"
        };

                if (validationMessages.Contains(result.First().Status))
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
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
            catch (Exception ex)
            {
                dbop.LogException(
                    ex,
                    "SchoolManagementController",
                    "Tbl_Suppliers_CRUD_Operations",
                    Newtonsoft.Json.JsonConvert.SerializeObject(supplier)
                );

                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_Purchase_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_Purchase_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_Purchase_CRUD_Operations")]
        public IActionResult Tbl_Purchase_CRUD_Operations([FromBody] tblPurchase purchase)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    purchase.SchoolID = schoolId;
                }

                var result = dbop.Tbl_Purchase_CRUD_Operations(purchase);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                var error = result.FirstOrDefault(x =>
                    x.Status?.ToLower().Contains("error") == true);

                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                var validationMessages = new List<string>
        {
            "Supplier Required",
            "Items Required",
            "Invalid Grand Total"
        };

                if (validationMessages.Contains(result.First().Status))
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
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
            catch (Exception ex)
            {
                dbop.LogException(
                    ex,
                    "SchoolManagementController",
                    "Tbl_Purchase_CRUD_Operations",
                    Newtonsoft.Json.JsonConvert.SerializeObject(purchase)
                );

                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/Tbl_Sales_CRUD_Operations
        /// Purpose: Handles transactional and query operations for Tbl_Sales_CRUD_Operations.
        /// </summary>
        [HttpPost("Tbl_Sales_CRUD_Operations")]
        public IActionResult Tbl_Sales_CRUD_Operations([FromBody] tblSales sales)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    sales.SchoolID = schoolId;
                }

                var result = dbop.Tbl_Sales_CRUD_Operations(sales);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                var error = result.FirstOrDefault(x =>
                    x.Status?.ToLower().Contains("error") == true);

                if (error != null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = error.Status
                    });
                }

                var validationMessages = new List<string>
        {
            "Class Required",
            "Division Required",
            "Admission Number Required",
            "Items Required",
            "Invalid Grand Total"
        };

                if (validationMessages.Contains(result.First().Status))
                {
                    return StatusCode(400, new
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.First().Status,
                        Data = result
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
            catch (Exception ex)
            {
                dbop.LogException(
                    ex,
                    "SchoolManagementController",
                    "Tbl_Sales_CRUD_Operations",
                    Newtonsoft.Json.JsonConvert.SerializeObject(sales)
                );

                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }

        [HttpPost("TransitionAcademicYearData")]
        /// <summary>
        /// API Endpoint: Post api/SchoolManagement/TransitionAcademicYearData
        /// Purpose: Initiates cloning of selected academic masters and transportation data to a target academic year.
        /// </summary>
        public IActionResult TransitionAcademicYearData([FromBody] TransitionAcademicYearDataRequest request)
         {
            if (request == null)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Success = false,
                    Message = "Payload is required."
                });
            }

            try
            {
                var result = dbop.TransitionAcademicYearData(request);
                if (result.StartsWith("SUCCESS"))
                {
                    return Ok(new
                    {
                        StatusCode = 200,
                        Success = true,
                        Message = result
                    });
                }

                return BadRequest(new
                {
                    StatusCode = 400,
                    Success = false,
                    Message = result
                });
            }
            catch (Exception ex)
            {
                dbop.LogException(
                    ex,
                    "SchoolManagementController",
                    "TransitionAcademicYearData",
                    Newtonsoft.Json.JsonConvert.SerializeObject(request)
                );

                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred.",
                    Error = ex.Message
                });
            }
        }

        // ==========================================
        //         MESSAGING CONTROLLER ENDPOINTS
        // ==========================================

        [HttpGet("GetGatewayConfig")]
        public IActionResult GetGatewayConfig([FromQuery] string schoolId)
        {
            if (string.IsNullOrEmpty(schoolId)) return BadRequest("SchoolId is required.");
            var config = dbop.GetGatewayConfig(schoolId);
            return Ok(new { StatusCode = 200, Success = true, Data = config });
        }

        [HttpPost("SaveGatewayConfig")]
        public IActionResult SaveGatewayConfig([FromBody] MessagingGatewayConfig config)
        {
            if (config == null || string.IsNullOrEmpty(config.SchoolId)) return BadRequest("Invalid payload or SchoolId.");
            var result = dbop.SaveGatewayConfig(config);
            if (result == "SUCCESS")
            {
                return Ok(new { StatusCode = 200, Success = true, Message = "Gateway config saved successfully." });
            }
            return StatusCode(500, new { StatusCode = 500, Success = false, Message = result });
        }

        [HttpGet("GetTemplates")]
        public IActionResult GetTemplates([FromQuery] string schoolId)
        {
            if (string.IsNullOrEmpty(schoolId)) return BadRequest("SchoolId is required.");
            var templates = dbop.GetTemplates(schoolId);
            return Ok(new { StatusCode = 200, Success = true, Data = templates });
        }

        [HttpPost("SaveTemplate")]
        public IActionResult SaveTemplate([FromBody] MessagingTemplateDto template)
        {
            if (template == null || string.IsNullOrEmpty(template.SchoolId)) return BadRequest("Invalid payload or SchoolId.");
            var result = dbop.SaveTemplate(template);
            if (result == "SUCCESS")
            {
                return Ok(new { StatusCode = 200, Success = true, Message = "Template saved successfully." });
            }
            return StatusCode(500, new { StatusCode = 500, Success = false, Message = result });
        }

        [HttpDelete("DeleteTemplate")]
        public IActionResult DeleteTemplate([FromQuery] string schoolId, [FromQuery] string templateId)
        {
            if (string.IsNullOrEmpty(schoolId) || string.IsNullOrEmpty(templateId)) return BadRequest("SchoolId and TemplateId are required.");
            var result = dbop.DeleteTemplate(schoolId, templateId);
            if (result == "SUCCESS")
            {
                return Ok(new { StatusCode = 200, Success = true, Message = "Template deleted successfully." });
            }
            else if (result == "NOT_FOUND_OR_DEFAULT")
            {
                return BadRequest(new { StatusCode = 400, Success = false, Message = "Cannot delete default templates or template not found." });
            }
            return StatusCode(500, new { StatusCode = 500, Success = false, Message = result });
        }

        [HttpGet("GetDeliveryLogs")]
        public IActionResult GetDeliveryLogs([FromQuery] string schoolId)
        {
            if (string.IsNullOrEmpty(schoolId)) return BadRequest("SchoolId is required.");
            var logs = dbop.GetDeliveryLogs(schoolId);
            return Ok(new { StatusCode = 200, Success = true, Data = logs });
        }

        [HttpPost("SendMessages")]
        public async Task<IActionResult> SendMessages([FromBody] SendMessageRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.SchoolId)) return BadRequest("Invalid message payload.");
            var result = await dbop.SendMessagesViaGateway(request);
            if (result == "SUCCESS")
            {
                return Ok(new { StatusCode = 200, Success = true, Message = "Messages successfully queued / dispatched via Msg91 gateway." });
            }
            return BadRequest(new { StatusCode = 400, Success = false, Message = result });
        }
    }
}