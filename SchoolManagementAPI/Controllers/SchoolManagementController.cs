using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly SchoolManagementDBContext _dbContext;

        public SchoolManagementController(
            IConfiguration configuration,
            ILogger<SchoolManagementController> logger,
            SchoolManagementDBContext dbContext)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var connectionString = _configuration.GetConnectionString("DefaultConnection")
                                   ?? throw new ArgumentNullException("Connection string not found");

            dbop = new SchoolManagementDAL(connectionString);

            _dbContext = dbContext;
        }

        [HttpPost("Tbl_SchoolDetails_CRUD")]
        public IActionResult Tbl_SchoolDetails_CRUD([FromBody] SchoolDetails school)
        {
            try
            {
                var roleId = User.FindFirst("role")?.Value;
                var tokenSchoolId = User.FindFirst("SchoolID")?.Value;
                if (roleId != "1")
                {
                    school.SchoolID = string.IsNullOrWhiteSpace(tokenSchoolId) ? null : tokenSchoolId;
                }

                var result = dbop.Tbl_SchoolDetails_CRUD(school);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "Database returned null result."
                    });
                }

                var error = result.FirstOrDefault(x => !string.IsNullOrEmpty(x.Status) && x.Status.ToLower().Contains("error"));
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
                dbop.LogException(ex, "SchoolManagementController", "Tbl_SchoolDetails_CRUD", Newtonsoft.Json.JsonConvert.SerializeObject(school));
                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }
        }

        [AllowAnonymous]
        [HttpPost("Tbl_Users_CRUD_Operations")]
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

                    string schoolRouteName = dbUser.SchoolName.Replace(" ", "");

                    return Ok(new
                    {
                        accessToken,
                        refreshToken,
                        role = dbUser.RollId,
                        email = dbUser.Email,
                        schoolId = dbUser.SchoolID,
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
                        schoolUrl
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

        //private async Task SendRegistrationEmailAsync(string toEmail, string userName, string userPassword, bool isAdmin)
        //{
        //    string actualRecipient = isAdmin ? "chaitanyakantamneni6@gmail.com" : toEmail;
        //    string subject = isAdmin ? "New User Registered" : "Welcome to Buserele Family!";
        //    string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "emaillog.jpg");
        //    string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "buserelelog.jpg");

        //    var emailMessage = new MimeMessage();
        //    emailMessage.From.Add(new MailboxAddress("BUSERELE Property Management", _configuration["Smtp:Username"]));
        //    emailMessage.To.Add(MailboxAddress.Parse(actualRecipient));
        //    emailMessage.Subject = subject;

        //    var builder = new BodyBuilder();

        //    var headerImage = builder.LinkedResources.Add(imagePath);
        //    headerImage.ContentId = "CompanyLogo";
        //    var footerLogo = builder.LinkedResources.Add(logoPath);
        //    footerLogo.ContentId = "BusereleLogo";

        //    string htmlBody = isAdmin
        //        ? $@"
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
        //        : $@"
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

        //    builder.HtmlBody = htmlBody;
        //    emailMessage.Body = builder.ToMessageBody();

        //    //try
        //    //{
        //    //    using var smtpClient = new MailKit.Net.Smtp.SmtpClient();
        //    //    await smtpClient.ConnectAsync(_configuration["Smtp:Host"], int.Parse(_configuration["Smtp:Port"]), false);
        //    //    await smtpClient.AuthenticateAsync(_configuration["Smtp:Username"], _configuration["Smtp:Password"]);
        //    //    await smtpClient.SendAsync(emailMessage);
        //    //    await smtpClient.DisconnectAsync(true);
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    Console.WriteLine($"Error sending email: {ex.Message}");
        //    //    throw;
        //    //}


        //    try
        //    {
        //        var smtpClient = new System.Net.Mail.SmtpClient(_configuration["Smtp:smtpServer"])
        //        {
        //            Port = int.Parse(_configuration["Smtp:Port"]),
        //            Credentials = new NetworkCredential(_configuration["Smtp:Username"], _configuration["Smtp:Password"]),
        //            EnableSsl = false
        //        };

        //        Console.WriteLine("smtpClient", smtpClient.ToString());

        //        var mailMessage = new MailMessage
        //        {
        //            From = new MailAddress(_configuration["Smtp:FromEmail"], "Support Team"),
        //            Subject = subject,
        //            IsBodyHtml = true,
        //            Body = htmlBody
        //        };
        //        mailMessage.To.Add(actualRecipient);
        //        await smtpClient.SendMailAsync(mailMessage);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error sending email: {ex.Message}");
        //        throw ex;
        //    }
        //}

        // Fetch school details by ID

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

        private async Task SendRegistrationEmailAsync(string toEmail, string userName, string userPassword, bool isAdmin, string? schoolName = null, string? schoolUrl = null)
        {
            string actualRecipient = isAdmin ? "chaitanyakantamneni6@gmail.com" : toEmail;
            string subject = isAdmin ? "New Employee Registered" : "Welcome to Smart Schools ERP";

            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "emaillog.jpg");
            string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "buserelelog.jpg");

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Smart Schools ERP", _configuration["Smtp:Username"]));
            emailMessage.To.Add(MailboxAddress.Parse(actualRecipient));
            emailMessage.Subject = subject;

            var builder = new BodyBuilder();

            // Attach images only if they exist
            if (System.IO.File.Exists(imagePath))
            {
                var headerImage = builder.LinkedResources.Add(imagePath);
                headerImage.ContentId = "CompanyLogo";
            }

            if (System.IO.File.Exists(logoPath))
            {
                var footerLogo = builder.LinkedResources.Add(logoPath);
                footerLogo.ContentId = "FooterLogo";
            }

            // Choose URL and footer based on school
            string loginUrl = !string.IsNullOrEmpty(schoolUrl) ? schoolUrl : "https://www.smartschools.com";
            string loginText = !string.IsNullOrEmpty(schoolName) ? $"Please login at <a href='{loginUrl}'>{schoolName}</a>" : $"Please login at <a href='{loginUrl}'>www.smartschools.com</a>";
            string footerText = !string.IsNullOrEmpty(schoolName) ? $"&copy; {schoolName} 2025. All rights reserved." : "&copy; Smart Schools ERP 2025. All rights reserved.";

            string htmlBody = isAdmin
                ? $@"
    <html>
    <head>
        <style>
            body {{ font-family: Arial, sans-serif; }}
            .container {{ text-align: center; padding: 20px; background-color: #f0f0f0; }}
            .content {{ margin: 20px; padding: 20px; background-color: #ffffff; border-radius: 10px; }}
            .footer {{ margin-top: 20px; font-size: 12px; color: #555555; }}
        </style>
    </head>
    <body>
        <div class='container'>
            <div class='content'>
                {(System.IO.File.Exists(imagePath) ? "<img src='cid:CompanyLogo' alt='Smart Schools ERP Logo' width='200' />" : "")}
                <h2>New Employee Registration</h2>
                <p><strong>Name:</strong> {userName}</p>
                <p><strong>Email:</strong> {toEmail}</p>
                {(System.IO.File.Exists(logoPath) ? "<img src='cid:FooterLogo' alt='Smart Schools ERP Logo' width='150' />" : "")}
            </div>
            <p class='footer'>{footerText}</p>
        </div>
    </body>
    </html>"
                : $@"
    <html>
    <head>
        <style>
            body {{ font-family: Arial, sans-serif; }}
            .container {{ text-align: center; padding: 20px; background-color: #f0f0f0; }}
            .content {{ margin: 20px; padding: 20px; background-color: #ffffff; border-radius: 10px; }}
            .footer {{ margin-top: 20px; font-size: 12px; color: #555555; }}
            .credentials {{ text-align: left; display: inline-block; margin-top: 10px; }}
        </style>
    </head>
    <body>
        <div class='container'>
            {(System.IO.File.Exists(imagePath) ? "<img src='cid:CompanyLogo' alt='Smart Schools ERP Logo' width='200' />" : "")}
            <h2>Welcome, {userName}!</h2>
            <p>Congratulations! Your employee account has been successfully created in <strong>Smart Schools ERP</strong>.</p>
            <div class='content'>
                <h3>Login Details:</h3>
                <div class='credentials'>
                    <p><strong>Email / UserID:</strong> {toEmail}</p>
                    <p><strong>Password:</strong> {userPassword}</p>
                </div>
            </div>
            <p>{loginText}</p>
            <p>If you face any issues, contact your system administrator.</p>
            <p class='footer'>{footerText}</p>
            {(System.IO.File.Exists(logoPath) ? $"<br><img src='cid:FooterLogo' alt='Smart Schools ERP Logo' width='150' />" : "")}
        </div>
    </body>
    </html>";

            builder.HtmlBody = htmlBody;
            emailMessage.Body = builder.ToMessageBody();

            try
            {
                using var smtpClient = new MailKit.Net.Smtp.SmtpClient();
                await smtpClient.ConnectAsync(_configuration["Smtp:Host"], int.Parse(_configuration["Smtp:Port"]), MailKit.Security.SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(_configuration["Smtp:Username"], _configuration["Smtp:Password"]);
                await smtpClient.SendAsync(emailMessage);
                await smtpClient.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }

        [AllowAnonymous]
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

        [HttpPost("Tbl_AcademicYear_CRUD_Operations")]
        public IActionResult Tbl_AcademicYear_CRUD_Operations([FromBody] tblAcademicYear academicYear)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
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

        [HttpPost("ExportSyllabus")]
        public async Task<IActionResult> ExportSyllabus([FromBody] tblSyllabus filter, [FromQuery] string type)
        {
            const int batchSize = 50000;
            DateTime? lastCreatedDate = null;
            int? lastID = null;

            if (type == "excel")
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var stream = new MemoryStream();
                var package = new ExcelPackage(stream);
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

                return File(
                    stream,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"Syllabus_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
                );
            }
            else if (type == "pdf" || type == "print")
            {
                var stream = new MemoryStream();
                var allData = new List<tblSyllabus>();
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

                    allData.AddRange(batch);
                    lastCreatedDate = batch.Last().CreatedDate;
                    lastID = batch.Last().ID;
                }

                // Generate PDF with borders & colors, no Description column
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
                            // Define 7 columns (without Description)
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(); // ID
                                columns.RelativeColumn(); // Name
                                columns.RelativeColumn(); // School Name
                                columns.RelativeColumn(); // Academic Year
                                columns.RelativeColumn(); // Available From
                                columns.RelativeColumn(); // Status
                                columns.RelativeColumn(); // Created Date
                            });

                            // Header row with background color
                            table.Header(header =>
                            {
                                var headerCells = new[] { "ID", "Name", "School Name", "Academic Year", "Available From", "Status", "Created Date" };
                                foreach (var h in headerCells)
                                {
                                    header.Cell()
                                          .Background(Colors.Grey.Lighten2)
                                          .Border(1)
                                          .BorderColor(Colors.Black)
                                          .Padding(3)
                                          .Text(h)
                                          .SemiBold();
                                }
                            });

                            // Data rows with borders
                            foreach (var item in allData)
                            {
                                table.Cell().Border(1).BorderColor(Colors.Black).Padding(3).Text(item.ID?.ToString() ?? "");
                                table.Cell().Border(1).BorderColor(Colors.Black).Padding(3).Text(item.Name ?? "");
                                table.Cell().Border(1).BorderColor(Colors.Black).Padding(3).Text(item.SchoolName ?? "");
                                table.Cell().Border(1).BorderColor(Colors.Black).Padding(3).Text(item.AcademicYearName ?? "");
                                table.Cell().Border(1).BorderColor(Colors.Black).Padding(3).Text(item.AvailableFrom?.ToString("yyyy-MM-dd") ?? "");
                                table.Cell().Border(1).BorderColor(Colors.Black).Padding(3).Text((item.IsActive ?? false) ? "Active" : "Inactive");
                                table.Cell().Border(1).BorderColor(Colors.Black).Padding(3).Text(item.CreatedDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "");
                            }
                        });
                    });
                });

                doc.GeneratePdf(stream);
                stream.Position = 0;

                return File(
                    stream,
                    "application/pdf",
                    $"Syllabus_{DateTime.Now:yyyyMMddHHmmss}.pdf"
                );
            }


            return BadRequest("Invalid export type");
        }

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

                return Ok(new { StatusCode = 200, Success = true, Message = result.First().Status, Data = result });
            }
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_Modules_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(module));
                return BadRequest(new { StatusCode = 500, Success = false, Message = "Internal server error.", Error = ex.Message });
            }
        }

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

                return Ok(new { StatusCode = 200, Success = true, Message = result.First().Status, Data = result });
            }
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_Pages_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(page));
                return BadRequest(new { StatusCode = 500, Success = false, Message = "Internal server error.", Error = ex.Message });
            }
        }

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

        [HttpPost("Tbl_Roles_CRUD_Operations")]
        public IActionResult Tbl_Roles_CRUD_Operations([FromBody] tblRoles role)
        {
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    role.SchoolID = schoolId;
                }

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


        [HttpPost("Tbl_Class_CRUD_Operations")]
        public IActionResult Tbl_Class_CRUD_Operations([FromBody] tblClass Class)
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
            try
            {
                var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
                var schoolId = User.FindFirst("SchoolID")?.Value;

                if (roleId != "1")
                {
                    staff.SchoolID = schoolId;
                }
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
            catch (Exception ex)
            {
                dbop.LogException(ex, "SchoolManagementController", "Tbl_Staff_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(staff));
                return BadRequest(new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error occurred. Please try again.",
                    Error = ex.Message
                });
            }

        }

        [HttpPost("Tbl_Subject_CRUD_Operations")]
        public IActionResult Tbl_Subject_CRUD_Operations([FromBody] tblSubjects subject)
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


        [AllowAnonymous]
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
    }
}