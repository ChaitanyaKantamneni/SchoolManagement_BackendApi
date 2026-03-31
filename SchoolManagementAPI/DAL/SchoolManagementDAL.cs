using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using MySqlConnector;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.IsisMtt.X509;
using Org.BouncyCastle.Tls;
using Org.BouncyCastle.Utilities;
using SchoolManagementAPI.Models;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace SchoolManagementAPI.DAL
{
    public class SchoolManagementDAL
    {
        private readonly string _connectionString;
        public SchoolManagementDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void LogException(Exception ex, string controller, string action, string data)
        {
            using var conn = new MySqlConnection(_connectionString);
            using var cmd = new MySqlCommand("Proc_InsertExceptionLog", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("p_Message", ex.Message);
            cmd.Parameters.AddWithValue("p_StackTrace", ex.StackTrace);
            cmd.Parameters.AddWithValue("p_Controller", controller);
            cmd.Parameters.AddWithValue("p_Action", action);
            cmd.Parameters.AddWithValue("p_Data", data);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        //public List<SchoolDetails> Tbl_SchoolDetails_CRUD(SchoolDetails school)
        //{
        //    var schools = new List<SchoolDetails>();

        //    string CleanParam(string? value)
        //    {
        //        return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
        //    }

        //    try
        //    {
        //        using (var conn = new MySqlConnection(_connectionString))
        //        using (var cmd = new MySqlCommand("Proc_Tbl_SchoolDetails", conn))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;

        //            cmd.Parameters.AddWithValue("p_ID", (object?)CleanParam(school.ID) ?? DBNull.Value);
        //            cmd.Parameters.AddWithValue("p_Name", (object?)CleanParam(school.Name) ?? DBNull.Value);
        //            cmd.Parameters.AddWithValue("p_MobileNo", (object?)CleanParam(school.MobileNo) ?? DBNull.Value);
        //            cmd.Parameters.AddWithValue("p_Email", (object?)CleanParam(school.Email) ?? DBNull.Value);
        //            cmd.Parameters.AddWithValue("p_Website", (object?)CleanParam(school.Website) ?? DBNull.Value);
        //            cmd.Parameters.AddWithValue("p_Address", (object?)CleanParam(school.Address) ?? DBNull.Value);
        //            cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(school.CreatedBy) ?? DBNull.Value);
        //            cmd.Parameters.AddWithValue("p_CreatedIp", (object?)CleanParam(school.CreatedIP) ?? DBNull.Value);
        //            cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(school.ModifiedBy) ?? DBNull.Value);
        //            cmd.Parameters.AddWithValue("p_ModifiedIp", (object?)CleanParam(school.ModifiedIP) ?? DBNull.Value);
        //            cmd.Parameters.AddWithValue("p_Flag", (object?)CleanParam(school.Flag) ?? DBNull.Value);
        //            cmd.Parameters.AddWithValue("p_Limit", school.Limit ?? 100);
        //            cmd.Parameters.AddWithValue("p_Offset", school.Offset ?? 0);

        //            conn.Open();

        //            if (!string.IsNullOrEmpty(school.Flag))
        //            {
        //                using (var reader = cmd.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        var s = new SchoolDetails
        //                        {
        //                            ID = reader["ID"]?.ToString(),
        //                            Name = reader["Name"]?.ToString(),
        //                            MobileNo = reader["MobileNo"]?.ToString(),
        //                            Email = reader["Email"]?.ToString(),
        //                            Website = reader["Website"]?.ToString(),
        //                            Address = reader["Address"]?.ToString(),
        //                            CreatedBy = reader["CreatedBy"]?.ToString(),
        //                            CreatedIP = reader["CreatedIp"]?.ToString(),
        //                            CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
        //                            ModifiedBy = reader["ModifiedBy"]?.ToString(),
        //                            ModifiedIP = reader["ModifiedIp"]?.ToString(),
        //                            ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
        //                            Status = reader["Message"]?.ToString()
        //                        };

        //                        schools.Add(s);
        //                    }
        //                }
        //            }
        //        }

        //        return schools;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogException(ex, "SchoolManagementDAL", "Tbl_SchoolDetails_CRUD", Newtonsoft.Json.JsonConvert.SerializeObject(school));
        //        return new List<SchoolDetails>
        //        {
        //            new SchoolDetails
        //            {
        //                Status = $"ERROR: {ex.Message}"
        //            }
        //        };
        //    }
        //}


        public static class DateTimeHelper
        {
            private static readonly TimeZoneInfo IstTimeZone =
                TimeZoneInfo.FindSystemTimeZoneById(
                    RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                        ? "India Standard Time"
                        : "Asia/Kolkata"
                );

            public static DateTime NowIST()
            {
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IstTimeZone);
            }

            public static DateTime ToIST(DateTime utcDateTime)
            {
                if (utcDateTime.Kind != DateTimeKind.Utc)
                    utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);

                return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, IstTimeZone);
            }
        }

        public List<TblUser> Tbl_Users_CRUD_Operations(TblUser user)
        {
            var users = new List<TblUser>();

            string? Clean(string? value)
                => string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string"
                    ? null
                    : value;

            try
            {
                using var conn = new MySqlConnection(_connectionString);
                using var cmd = new MySqlCommand("Proc_Tbl_User", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("p_Name", Clean(user.FirstName) ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_SchoolID", Clean(user.SchoolID) ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_Email", Clean(user.Email) ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_MobileNo", Clean(user.MobileNo) ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_Password", Clean(user.Password) ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_CreatedBy", Clean(user.CreatedBy) ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_CreatedIP", Clean(user.CreatedIP) ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_CreatedDate", user.CreatedDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModifiedBy", Clean(user.ModifiedBy) ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModifiedIP", Clean(user.ModifiedIP) ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModifiedDate", user.ModifiedDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_RollId", Clean(user.RollId) ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_LastName", Clean(user.LastName) ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_IsActive", Clean(user.IsActive) ?? (object)DBNull.Value);
                //cmd.Parameters.AddWithValue("p_IsActive", user.IsActive.HasValue ? (object)(user.IsActive.Value ? 1 : 0) : DBNull.Value);
                cmd.Parameters.AddWithValue("p_Flag", Clean(user.Flag) ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_OldPassword", Clean(user.OldPassword) ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_NewPassword", Clean(user.NewPassword) ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_FileName", Clean(user.FileName) ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_FilePath", Clean(user.FilePath) ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_Limit", user.Limit ?? 100);
                cmd.Parameters.AddWithValue("p_Offset", user.Offset ?? 0);

                conn.Open();

                if (!string.IsNullOrEmpty(user.Flag))
                {
                    if (user.Flag == "4")
                    {
                        using var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            users.Add(new TblUser
                            {
                                ID = reader["ID"] == DBNull.Value ? 0 : Convert.ToInt64(reader["ID"]),
                                SchoolID = reader["SchoolID"]?.ToString(),
                                FirstName = reader["FirstName"]?.ToString(),
                                LastName = reader["LastName"]?.ToString(),
                                Email = reader["Email"]?.ToString(),
                                MobileNo = reader["MobileNo"]?.ToString(),
                                RollId = reader["RollId"]?.ToString(),
                                IsActive = reader["IsActive"].ToString(),
                                CreatedBy = reader["CreatedBy"]?.ToString(),
                                CreatedIP = reader["CreatedIP"]?.ToString(),
                                CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                ModifiedIP = reader["ModifiedIP"]?.ToString(),
                                ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                FileName = reader["FileName"]?.ToString(),
                                FilePath = reader["FilePath"]?.ToString(),
                                Message = reader["Message"]?.ToString(),
                                SchoolName = reader["SchoolName"]?.ToString()
                            });
                        }
                    }
                    else
                    {
                        using var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            users.Add(new TblUser
                            {
                                ID = reader["ID"] == DBNull.Value ? 0 : Convert.ToInt64(reader["ID"]),
                                SchoolID = reader["SchoolID"]?.ToString(),
                                FirstName = reader["FirstName"]?.ToString(),
                                LastName = reader["LastName"]?.ToString(),
                                Email = reader["Email"]?.ToString(),
                                MobileNo = reader["MobileNo"]?.ToString(),
                                RollId = reader["RollId"]?.ToString(),
                                IsActive = reader["IsActive"].ToString(),
                                CreatedBy = reader["CreatedBy"]?.ToString(),
                                CreatedIP = reader["CreatedIP"]?.ToString(),
                                CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                ModifiedIP = reader["ModifiedIP"]?.ToString(),
                                ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                FileName = reader["FileName"]?.ToString(),
                                FilePath = reader["FilePath"]?.ToString(),
                                Message = reader["Message"]?.ToString()
                            });
                        }
                    }
                }
                return users;
            }
            catch (Exception ex)
            {
                LogException(ex, "SchoolManagementDAL", "Tbl_Users_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(user));
                return new List<TblUser>
                {
                    new TblUser
                    {
                        Message = $"ERROR: {ex.Message}"
                    }
                };
            }
        }

        public UserToken GetUserTokenByRefresh(string email, string refreshToken = null)
        {
            using var conn = new MySqlConnection(_connectionString);
            using var cmd = new MySqlCommand("ManageUserToken", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("p_action", "GET_REFRESH");
            cmd.Parameters.AddWithValue("p_email", email ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("p_accessToken", DBNull.Value);
            cmd.Parameters.AddWithValue("p_refreshToken", refreshToken ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("p_issuedAt", DBNull.Value);
            cmd.Parameters.AddWithValue("p_expiryAt", DBNull.Value);
            cmd.Parameters.AddWithValue("p_refreshExpiryAt", DBNull.Value);

            conn.Open();
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new UserToken
                {
                    Email = reader["Email"]?.ToString(),
                    AccessToken = reader["AccessToken"]?.ToString(),
                    RefreshToken = reader["RefreshToken"]?.ToString(),
                    AccessExpiry = Convert.ToDateTime(reader["ExpiryAt"]),
                    RefreshExpiry = Convert.ToDateTime(reader["RefreshExpiryAt"])
                };
            }

            return null;
        }

        public void InsertUserToken(string email, string accessToken, string refreshToken, DateTime accessExpiryUtc, DateTime refreshExpiryUtc)
        {
            using var conn = new MySqlConnection(_connectionString);
            using var cmd = new MySqlCommand("ManageUserToken", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("p_action", "INSERT");
            cmd.Parameters.AddWithValue("p_email", email);
            cmd.Parameters.AddWithValue("p_accessToken", accessToken);
            cmd.Parameters.AddWithValue("p_refreshToken", refreshToken);

            // IST in DB
            cmd.Parameters.AddWithValue("p_issuedAt", DateTimeHelper.NowIST());
            cmd.Parameters.AddWithValue("p_expiryAt", DateTimeHelper.ToIST(accessExpiryUtc));
            cmd.Parameters.AddWithValue("p_refreshExpiryAt", DateTimeHelper.ToIST(refreshExpiryUtc));

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void RevokeUserToken(string refreshToken)
        {
            using var conn = new MySqlConnection(_connectionString);
            using var cmd = new MySqlCommand("ManageUserToken", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("p_action", "REVOKE");
            cmd.Parameters.AddWithValue("p_email", DBNull.Value);
            cmd.Parameters.AddWithValue("p_accessToken", DBNull.Value);
            cmd.Parameters.AddWithValue("p_refreshToken", refreshToken);
            cmd.Parameters.AddWithValue("p_issuedAt", DBNull.Value);
            cmd.Parameters.AddWithValue("p_expiryAt", DBNull.Value);
            cmd.Parameters.AddWithValue("p_refreshExpiryAt", DBNull.Value);

            conn.Open();
            cmd.ExecuteNonQuery();
        }


        //Masters Module
        public List<SchoolDetails> Tbl_SchoolDetails_CRUD(SchoolDetails school)
        {
            var schools = new List<SchoolDetails>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {
                using var conn = new MySqlConnection(_connectionString);
                using var cmd = new MySqlCommand("Proc_Tbl_SchoolDetails", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Add parameters
                cmd.Parameters.AddWithValue("p_ID", (object?)CleanParam(school.ID) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Name", (object?)CleanParam(school.Name) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_MobileNo", (object?)CleanParam(school.MobileNo) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Email", (object?)CleanParam(school.Email) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Website", (object?)CleanParam(school.Website) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Address", (object?)CleanParam(school.Address) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_IsActive", school.IsActive ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(school.CreatedBy) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_CreatedIp", (object?)CleanParam(school.CreatedIP) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(school.ModifiedBy) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModifiedIp", (object?)CleanParam(school.ModifiedIP) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Flag", (object?)CleanParam(school.Flag) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Limit", school.Limit ?? 100);
                cmd.Parameters.AddWithValue("p_Offset", school.Offset ?? 0);
                cmd.Parameters.AddWithValue("p_LastCreatedDate", school.LastCreatedDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_LastID", school.LastID ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_SortColumn", (object?)CleanParam(school.SortColumn) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_SortDirection", (object?)CleanParam(school.SortDirection) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_SchoolID", (object?)CleanParam(school.SchoolID) ?? DBNull.Value);

                conn.Open();

                if (!string.IsNullOrEmpty(school.Flag))
                {
                    if (school.Flag == "6" || school.Flag == "8") // Count flags
                    {
                        using var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            schools.Add(new SchoolDetails
                            {
                                totalcount = reader["totalCount"] != DBNull.Value ? Convert.ToInt32(reader["totalCount"]) : (int?)null
                            });
                        }
                    }
                    else if (school.Flag == "1" || school.Flag == "5")
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (reader["Message"]?.ToString() == "School name already exists")
                                {
                                    schools.Add(new SchoolDetails
                                    {
                                        Status = reader["Message"]?.ToString()
                                    });
                                }
                                else
                                {
                                    schools.Add(new SchoolDetails
                                    {
                                        ID = reader["ID"]?.ToString(),
                                        Name = reader["Name"]?.ToString(),
                                        MobileNo = reader["MobileNo"]?.ToString(),
                                        Email = reader["Email"]?.ToString(),
                                        Website = reader["Website"]?.ToString(),
                                        Address = reader["Address"]?.ToString(),
                                        AvailableFrom = reader["AvailableFrom"] == DBNull.Value ? null : Convert.ToDateTime(reader["AvailableFrom"]),
                                        Description = reader["Description"]?.ToString(),
                                        IsActive = reader["IsActive"].ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIP = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIP = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString()
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        using var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            schools.Add(new SchoolDetails
                            {
                                ID = reader["ID"]?.ToString(),
                                Name = reader["Name"]?.ToString(),
                                MobileNo = reader["MobileNo"]?.ToString(),
                                Email = reader["Email"]?.ToString(),
                                Website = reader["Website"]?.ToString(),
                                Address = reader["Address"]?.ToString(),
                                AvailableFrom = reader["AvailableFrom"] == DBNull.Value ? null : Convert.ToDateTime(reader["AvailableFrom"]),
                                Description = reader["Description"]?.ToString(),
                                IsActive = reader["IsActive"].ToString(),
                                CreatedBy = reader["CreatedBy"]?.ToString(),
                                CreatedIP = reader["CreatedIp"]?.ToString(),
                                CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                ModifiedIP = reader["ModifiedIp"]?.ToString(),
                                ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                Status = reader["Message"]?.ToString()
                            });
                        }
                    }
                }

                return schools;
            }
            catch (Exception ex)
            {
                LogException(ex, "SchoolManagementDAL", "Tbl_SchoolDetails_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(school));
                return new List<SchoolDetails>
                {
                    new SchoolDetails { Status = $"ERROR: {ex.Message}" }
                };
            }
        }

        public List<tblAcademicYear> Tbl_AcademicYear_CRUD_Operations(tblAcademicYear academicYear)
        {
            var AcademicYears = new List<tblAcademicYear>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                using (var cmd = new MySqlCommand("Proc_AcademicYear", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("p_SchoolID", (object?)CleanParam(academicYear.SchoolID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ID", (object?)CleanParam(academicYear.ID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Name", (object?)CleanParam(academicYear.Name) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_StartDate", academicYear.StartDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_EndDate", academicYear.EndDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Description", (object?)CleanParam(academicYear.Description) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_IsActive", (object?)CleanParam(academicYear.IsActive) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(academicYear.CreatedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedIP", (object?)CleanParam(academicYear.CreatedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(academicYear.ModifiedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedIP", (object?)CleanParam(academicYear.ModifiedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_flag", (object?)CleanParam(academicYear.Flag) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Limit", academicYear.Limit ?? 100);
                    cmd.Parameters.AddWithValue("p_LastCreatedDate", academicYear.LastCreatedDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastID", academicYear.LastID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortColumn", (object?)CleanParam(academicYear.SortColumn) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortDirection", (object?)CleanParam(academicYear.SortDirection) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Offset", academicYear.Offset ?? (object)DBNull.Value);

                    conn.Open();

                    if (academicYear.Flag != null)
                    {
                        if (academicYear.Flag == "6" || academicYear.Flag == "8")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var Academicyear = new tblAcademicYear
                                    {
                                        totalcount = reader["totalCount"] != DBNull.Value ? Convert.ToInt32(reader["totalCount"]) : (int?)null
                                    };

                                    AcademicYears.Add(Academicyear);
                                }
                            }
                        }
                        else if (academicYear.Flag == "2" || academicYear.Flag == "3" || academicYear.Flag == "4" || academicYear.Flag == "7")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var Academicyear = new tblAcademicYear
                                    {
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        ID = reader["ID"]?.ToString(),
                                        Name = reader["Name"]?.ToString(),
                                        StartDate = reader["StartDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["StartDate"]),
                                        EndDate = reader["EndDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["EndDate"]),
                                        Description = reader["Description"]?.ToString(),
                                        IsActive = reader["IsActive"]?.ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString(),
                                        SchoolName = reader["SchoolName"]?.ToString()
                                    };

                                    AcademicYears.Add(Academicyear);
                                }
                            }
                        }
                        else if (academicYear.Flag == "1" || academicYear.Flag == "5")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (reader["Message"]?.ToString() == "Academic Year name already exists")
                                    {
                                        AcademicYears.Add(new tblAcademicYear
                                        {
                                            Status = reader["Message"]?.ToString()
                                        });
                                    }
                                    else
                                    {
                                        AcademicYears.Add(new tblAcademicYear
                                        {
                                            SchoolID = reader["SchoolID"]?.ToString(),
                                            ID = reader["ID"]?.ToString(),
                                            Name = reader["Name"]?.ToString(),
                                            StartDate = reader["StartDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["StartDate"]),
                                            EndDate = reader["EndDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["EndDate"]),
                                            Description = reader["Description"]?.ToString(),
                                            IsActive = reader["IsActive"]?.ToString(),
                                            CreatedBy = reader["CreatedBy"]?.ToString(),
                                            CreatedIp = reader["CreatedIp"]?.ToString(),
                                            CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                            ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                            ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                            ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                            Status = reader["Message"]?.ToString()
                                        });
                                    }
                                }
                            }
                        }
                        else
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var Academicyear = new tblAcademicYear
                                    {
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        ID = reader["ID"]?.ToString(),
                                        Name = reader["Name"]?.ToString(),
                                        StartDate = reader["StartDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["StartDate"]),
                                        EndDate = reader["EndDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["EndDate"]),
                                        Description = reader["Description"]?.ToString(),
                                        IsActive = reader["IsActive"]?.ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString()
                                    };

                                    AcademicYears.Add(Academicyear);
                                }
                            }
                        }
                    }
                }

                return AcademicYears;
            }
            catch (Exception ex)
            {
                LogException(ex, "SchoolManagementDAL", "Tbl_AcademicYear_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(academicYear));
                return new List<tblAcademicYear>
                {
                    new tblAcademicYear
                    {
                        Status = $"ERROR: {ex.Message}"
                    }
                };
            }
        }

        public List<tblSyllabus> Tbl_Syllabus_CRUD_Operations(tblSyllabus syllabus)
        {
            var Syllabuses = new List<tblSyllabus>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                using (var cmd = new MySqlCommand("Proc_Syllabus", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("p_SchoolID", (object?)CleanParam(syllabus.SchoolID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_AcademicYear", (object?)CleanParam(syllabus.AcademicYear) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ID", syllabus.ID);
                    cmd.Parameters.AddWithValue("p_Name", (object?)CleanParam(syllabus.Name) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_AvailableFrom", syllabus.AvailableFrom ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Description", (object?)CleanParam(syllabus.Description) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_IsActive", syllabus.IsActive);
                    cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(syllabus.CreatedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedIP", (object?)CleanParam(syllabus.CreatedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(syllabus.ModifiedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedIP", (object?)CleanParam(syllabus.ModifiedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_flag", (object?)CleanParam(syllabus.Flag) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Limit", syllabus.Limit ?? 100);
                    cmd.Parameters.AddWithValue("p_LastCreatedDate", syllabus.LastCreatedDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastID", syllabus.LastID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortColumn", (object?)CleanParam(syllabus.SortColumn) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortDirection", (object?)CleanParam(syllabus.SortDirection) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Offset", syllabus.Offset ?? (object)DBNull.Value);

                    conn.Open();

                    if (syllabus.Flag != null)
                    {
                        if (syllabus.Flag == "6" || syllabus.Flag == "8")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var Syllabus = new tblSyllabus
                                    {
                                        totalcount = reader["totalCount"] != DBNull.Value ? Convert.ToInt32(reader["totalCount"]) : (int?)null
                                    };

                                    Syllabuses.Add(Syllabus);
                                }
                            }
                        }
                        else if (syllabus.Flag == "2" || syllabus.Flag == "3" || syllabus.Flag == "4" || syllabus.Flag == "7")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var Syllabus = new tblSyllabus
                                    {
                                        //ID = reader["ID"] == DBNull.Value ? null : Convert.ToInt32(reader["ID"]),
                                        ID = reader["ID"] == DBNull.Value ? null : Convert.ToInt32(reader["ID"]),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"].ToString(),
                                        Name = reader["Name"]?.ToString(),
                                        AvailableFrom = reader["AvailableFrom"] == DBNull.Value ? null : Convert.ToDateTime(reader["AvailableFrom"]),
                                        Description = reader["Description"]?.ToString(),
                                        IsActive = reader["IsActive"] == DBNull.Value ? null : Convert.ToBoolean(reader["IsActive"]),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString(),
                                        SchoolName = reader["SchoolName"]?.ToString(),
                                        AcademicYearName = reader["AcademicYearName"]?.ToString()
                                    };

                                    Syllabuses.Add(Syllabus);
                                }
                            }
                        }
                        else if (syllabus.Flag == "1" || syllabus.Flag == "5")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (reader["Message"]?.ToString() == "Syllabus name already exists")
                                    {
                                        Syllabuses.Add(new tblSyllabus
                                        {
                                            Status = reader["Message"]?.ToString()
                                        });
                                    }
                                    else
                                    {
                                        Syllabuses.Add(new tblSyllabus
                                        {
                                            ID = reader["ID"] == DBNull.Value ? null : Convert.ToInt32(reader["ID"]),
                                            SchoolID = reader["SchoolID"]?.ToString(),
                                            AcademicYear = reader["AcademicYear"].ToString(),
                                            Name = reader["Name"]?.ToString(),
                                            AvailableFrom = reader["AvailableFrom"] == DBNull.Value ? null : Convert.ToDateTime(reader["AvailableFrom"]),
                                            Description = reader["Description"]?.ToString(),
                                            IsActive = reader["IsActive"] == DBNull.Value ? null : Convert.ToBoolean(reader["IsActive"]),
                                            CreatedBy = reader["CreatedBy"]?.ToString(),
                                            CreatedIp = reader["CreatedIp"]?.ToString(),
                                            CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                            ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                            ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                            ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                            Status = reader["Message"]?.ToString()
                                        });
                                    }
                                }
                            }
                        }
                        else
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var Syllabus = new tblSyllabus
                                    {
                                        //ID = reader["ID"] == DBNull.Value ? null : Convert.ToInt32(reader["ID"]),
                                        ID = reader["ID"] == DBNull.Value ? null : Convert.ToInt32(reader["ID"]),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"].ToString(),
                                        Name = reader["Name"]?.ToString(),
                                        AvailableFrom = reader["AvailableFrom"] == DBNull.Value ? null : Convert.ToDateTime(reader["AvailableFrom"]),
                                        Description = reader["Description"]?.ToString(),
                                        IsActive = reader["IsActive"] == DBNull.Value ? null : Convert.ToBoolean(reader["IsActive"]),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString()
                                    };

                                    Syllabuses.Add(Syllabus);
                                }
                            }
                        }
                    }
                }

                return Syllabuses;
            }
            catch (Exception ex)
            {
                LogException(ex, "SchoolManagementDAL", "Tbl_Syllabus_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(syllabus));
                return new List<tblSyllabus>
                {
                    new tblSyllabus
                    {
                        Status = $"ERROR: {ex.Message}"
                    }
                };
            }
        }

        public List<tblClass> Tbl_Class_CRUD_Operations(tblClass Class)
        {
            var Classes = new List<tblClass>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                using (var cmd = new MySqlCommand("Proc_Class", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("p_ID", (object?)CleanParam(Class.ID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SchoolID", (object?)CleanParam(Class.SchoolID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_AcademicYear", (object?)CleanParam(Class.AcademicYear) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Name", (object?)CleanParam(Class.Name) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Syllabus", (object?)CleanParam(Class.Syllabus) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Description", (object?)CleanParam(Class.Description) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_IsActive", (object?)CleanParam(Class.IsActive) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(Class.CreatedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedIP", (object?)CleanParam(Class.CreatedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(Class.ModifiedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedIP", (object?)CleanParam(Class.ModifiedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_flag", (object?)CleanParam(Class.Flag) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Limit", Class.Limit ?? 100);
                    cmd.Parameters.AddWithValue("p_LastCreatedDate", Class.LastCreatedDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastID", Class.LastID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortColumn", (object?)CleanParam(Class.SortColumn) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortDirection", (object?)CleanParam(Class.SortDirection) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Offset", Class.Offset ?? (object)DBNull.Value);

                    conn.Open();

                    if (Class.Flag != null)
                    {
                        if (Class.Flag == "6" || Class.Flag == "8")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var classs = new tblClass
                                    {
                                        totalcount = reader["totalCount"] != DBNull.Value ? Convert.ToInt32(reader["totalCount"]) : (int?)null
                                    };

                                    Classes.Add(classs);
                                }
                            }
                        }
                        else if (Class.Flag == "2" || Class.Flag == "3" || Class.Flag == "4" || Class.Flag == "7")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var classs = new tblClass
                                    {
                                        //ID = reader["ID"] == DBNull.Value ? null : Convert.ToInt32(reader["ID"]),
                                        ID = reader["ID"]?.ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        Name = reader["Name"]?.ToString(),
                                        Syllabus = reader["Syllabus"]?.ToString(),
                                        Description = reader["Description"]?.ToString(),
                                        IsActive = reader["IsActive"]?.ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString(),
                                        SchoolName = reader["SchoolName"]?.ToString(),
                                        AcademicYearName = reader["AcademicYearName"]?.ToString(),
                                        SyllabusName = reader["SyllabusName"]?.ToString()
                                    };

                                    Classes.Add(classs);
                                }
                            }
                        }
                        else if (Class.Flag == "1" || Class.Flag == "5")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (reader["Message"]?.ToString() == "Class name already exists")
                                    {
                                        Classes.Add(new tblClass
                                        {
                                            Status = reader["Message"]?.ToString()
                                        });
                                    }
                                    else
                                    {
                                        Classes.Add(new tblClass
                                        {
                                            ID = reader["ID"]?.ToString(),
                                            SchoolID = reader["SchoolID"]?.ToString(),
                                            AcademicYear = reader["AcademicYear"]?.ToString(),
                                            Name = reader["Name"]?.ToString(),
                                            Syllabus = reader["Syllabus"]?.ToString(),
                                            Description = reader["Description"]?.ToString(),
                                            IsActive = reader["IsActive"]?.ToString(),
                                            CreatedBy = reader["CreatedBy"]?.ToString(),
                                            CreatedIp = reader["CreatedIp"]?.ToString(),
                                            CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                            ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                            ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                            ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                            Status = reader["Message"]?.ToString()
                                        });
                                    }
                                }
                            }
                        }
                        else
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var classs = new tblClass
                                    {
                                        //ID = reader["ID"] == DBNull.Value ? null : Convert.ToInt32(reader["ID"]),
                                        ID = reader["ID"]?.ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        Name = reader["Name"]?.ToString(),
                                        Syllabus = reader["Syllabus"]?.ToString(),
                                        Description = reader["Description"]?.ToString(),
                                        IsActive = reader["IsActive"]?.ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString()
                                    };

                                    Classes.Add(classs);
                                }
                            }
                        }
                    }
                }

                return Classes;
            }
            catch (Exception ex)
            {
                LogException(ex, "SchoolManagementDAL", "Tbl_Class_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(Class));
                return new List<tblClass>
                {
                    new tblClass
                    {
                        Status = $"ERROR: {ex.Message}"
                    }
                };
            }
        }

        public List<tblClassDivision> Tbl_ClassDivision_CRUD_Operations(tblClassDivision classDivision)
        {
            var ClassDivisions = new List<tblClassDivision>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                using (var cmd = new MySqlCommand("Proc_ClassDivision", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("p_ID", (object?)CleanParam(classDivision.ID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SchoolID", (object?)CleanParam(classDivision.SchoolID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_AcademicYear", (object?)CleanParam(classDivision.AcademicYear) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Class", (object?)CleanParam(classDivision.Class) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Name", (object?)CleanParam(classDivision.Name) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Strength", (object?)CleanParam(classDivision.Strength) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Description", (object?)CleanParam(classDivision.Description) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_IsActive", (object?)CleanParam(classDivision.IsActive) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(classDivision.CreatedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedIP", (object?)CleanParam(classDivision.CreatedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(classDivision.ModifiedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedIP", (object?)CleanParam(classDivision.ModifiedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_flag", (object?)CleanParam(classDivision.Flag) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Limit", classDivision.Limit ?? 100);
                    cmd.Parameters.AddWithValue("p_LastCreatedDate", classDivision.LastCreatedDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastID", classDivision.LastID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortColumn", (object?)CleanParam(classDivision.SortColumn) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortDirection", (object?)CleanParam(classDivision.SortDirection) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Offset", classDivision.Offset ?? (object)DBNull.Value);

                    conn.Open();

                    if (classDivision.Flag != null)
                    {
                        if (classDivision.Flag == "9")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var ClassDivision = new tblClassDivision
                                    {
                                        SNo = reader["SNo"]?.ToString(),
                                        SyllabusClassName = reader["SyllabusClassName"]?.ToString(),
                                        Status = reader["Message"]?.ToString()
                                    };

                                    ClassDivisions.Add(ClassDivision);
                                }
                            }
                        }
                        else if (classDivision.Flag == "6" || classDivision.Flag == "8")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var ClassDivision = new tblClassDivision
                                    {
                                        totalcount = reader["totalCount"] != DBNull.Value ? Convert.ToInt32(reader["totalCount"]) : (int?)null
                                    };

                                    ClassDivisions.Add(ClassDivision);
                                }
                            }
                        }
                        else if (classDivision.Flag == "2" || classDivision.Flag == "3" || classDivision.Flag == "4" || classDivision.Flag == "7" || classDivision.Flag == "11")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var ClassDivision = new tblClassDivision
                                    {
                                        ID = reader["ID"]?.ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        Class = reader["Class"]?.ToString(),
                                        Name = reader["Name"]?.ToString(),
                                        Strength = reader["Strength"]?.ToString(),
                                        Description = reader["Description"]?.ToString(),
                                        IsActive = reader["IsActive"]?.ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString(),
                                        SchoolName = reader["SchoolName"]?.ToString(),
                                        AcademicYearName = reader["AcademicYearName"]?.ToString(),
                                        ClassName = reader["ClassName"]?.ToString()
                                    };

                                    ClassDivisions.Add(ClassDivision);
                                }
                            }
                        }
                        else if (classDivision.Flag == "1" || classDivision.Flag == "5")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (reader["Message"]?.ToString() == "ClassDivision name already exists")
                                    {
                                        ClassDivisions.Add(new tblClassDivision
                                        {
                                            Status = reader["Message"]?.ToString()
                                        });
                                    }
                                    else
                                    {
                                        ClassDivisions.Add(new tblClassDivision
                                        {
                                            ID = reader["ID"]?.ToString(),
                                            SchoolID = reader["SchoolID"]?.ToString(),
                                            AcademicYear = reader["AcademicYear"]?.ToString(),
                                            Class = reader["Class"]?.ToString(),
                                            Name = reader["Name"]?.ToString(),
                                            Strength = reader["Strength"]?.ToString(),
                                            Description = reader["Description"]?.ToString(),
                                            IsActive = reader["IsActive"]?.ToString(),
                                            CreatedBy = reader["CreatedBy"]?.ToString(),
                                            CreatedIp = reader["CreatedIp"]?.ToString(),
                                            CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                            ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                            ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                            ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                            Status = reader["Message"]?.ToString()
                                        });
                                    }
                                }
                            }
                        }
                        else
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var ClassDivision = new tblClassDivision
                                    {
                                        ID = reader["ID"]?.ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        Class = reader["Class"]?.ToString(),
                                        Name = reader["Name"]?.ToString(),
                                        Strength = reader["Strength"]?.ToString(),
                                        Description = reader["Description"]?.ToString(),
                                        IsActive = reader["IsActive"]?.ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString()
                                    };

                                    ClassDivisions.Add(ClassDivision);
                                }
                            }
                        }

                    }
                }

                return ClassDivisions;
            }
            catch (Exception ex)
            {
                LogException(ex, "SchoolManagementDAL", "Tbl_ClassDivision_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(classDivision));
                return new List<tblClassDivision>
                {
                    new tblClassDivision
                    {
                        Status = $"ERROR: {ex.Message}"
                    }
                };
            }
        }

        public List<tblStaff> Tbl_Staff_CRUD_Operations(tblStaff staff)
        {
            var StaffList = new List<tblStaff>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                using (var cmd = new MySqlCommand("Proc_Staff", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("p_ID", (object?)CleanParam(staff.ID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SchoolID", (object?)CleanParam(staff.SchoolID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_AcademicYear", (object?)CleanParam(staff.AcademicYear) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_StaffType", (object?)CleanParam(staff.StaffType) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_FirstName", (object?)CleanParam(staff.FirstName) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_MiddleName", (object?)CleanParam(staff.MiddleName) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastName", (object?)CleanParam(staff.LastName) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_MobileNumber", (object?)CleanParam(staff.MobileNumber) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Email", (object?)CleanParam(staff.Email) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_DateOfBirth", staff.DateOfBirth ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Qualification", (object?)CleanParam(staff.Qualification) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_IsActive", (object?)CleanParam(staff.IsActive) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(staff.CreatedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedIP", (object?)CleanParam(staff.CreatedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(staff.ModifiedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedIP", (object?)CleanParam(staff.ModifiedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_flag", (object?)CleanParam(staff.Flag) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Limit", staff.Limit ?? 100);
                    cmd.Parameters.AddWithValue("p_LastCreatedDate", staff.LastCreatedDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastID", staff.LastID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortColumn", (object?)CleanParam(staff.SortColumn) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortDirection", (object?)CleanParam(staff.SortDirection) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Offset", staff.Offset ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Name", (object?)CleanParam(staff.FirstName) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SubjectID", (object?)CleanParam(staff.SubjectID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ClassID", (object?)CleanParam(staff.ClassID) ?? DBNull.Value);

                    conn.Open();

                    if (staff.Flag != null)
                    {
                        if (staff.Flag == "6" || staff.Flag == "8")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var staffRecord = new tblStaff
                                    {
                                        TotalCount = reader["totalCount"] != DBNull.Value ? Convert.ToInt32(reader["totalCount"]) : (int?)null
                                    };

                                    StaffList.Add(staffRecord);
                                }
                            }
                        }
                        else if (staff.Flag == "2" || staff.Flag == "3" || staff.Flag == "4" || staff.Flag == "7" || staff.Flag == "9")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var staffRecord = new tblStaff
                                    {
                                        ID = reader["ID"]?.ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        StaffType = reader["StaffType"]?.ToString(),
                                        FirstName = reader["FirstName"]?.ToString(),
                                        MiddleName = reader["MiddleName"]?.ToString(),
                                        LastName = reader["LastName"]?.ToString(),
                                        MobileNumber = reader["MobileNumber"]?.ToString(),
                                        Email = reader["Email"]?.ToString(),
                                        DateOfBirth = reader["DateOfBirth"] == DBNull.Value ? null : Convert.ToDateTime(reader["DateOfBirth"]),
                                        Qualification = reader["Qualification"]?.ToString(),
                                        IsActive = reader["IsActive"]?.ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString(),
                                        SchoolName = reader["SchoolName"]?.ToString(),
                                        AcademicYearName = reader["AcademicYearName"]?.ToString()
                                    };

                                    StaffList.Add(staffRecord);
                                }
                            }
                        }
                        else if (staff.Flag == "1" || staff.Flag == "5")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (reader["Message"]?.ToString() == "Staff already exists")
                                    {
                                        StaffList.Add(new tblStaff
                                        {
                                            Status = reader["Message"]?.ToString()
                                        });
                                    }
                                    else
                                    {
                                        StaffList.Add(new tblStaff
                                        {
                                            ID = reader["ID"]?.ToString(),
                                            SchoolID = reader["SchoolID"]?.ToString(),
                                            AcademicYear = reader["AcademicYear"]?.ToString(),
                                            StaffType = reader["StaffType"]?.ToString(),
                                            FirstName = reader["FirstName"]?.ToString(),
                                            MiddleName = reader["MiddleName"]?.ToString(),
                                            LastName = reader["LastName"]?.ToString(),
                                            MobileNumber = reader["MobileNumber"]?.ToString(),
                                            Email = reader["Email"]?.ToString(),
                                            DateOfBirth = reader["DateOfBirth"] == DBNull.Value ? null : Convert.ToDateTime(reader["DateOfBirth"]),
                                            Qualification = reader["Qualification"]?.ToString(),
                                            IsActive = reader["IsActive"]?.ToString(),
                                            CreatedBy = reader["CreatedBy"]?.ToString(),
                                            CreatedIp = reader["CreatedIp"]?.ToString(),
                                            CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                            ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                            ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                            ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                            Status = reader["Message"]?.ToString()
                                        });
                                    }
                                }
                            }
                        }
                        else if (staff.Flag == "12")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var staffRecord = new tblStaff
                                    {
                                        ID = reader["ID"]?.ToString(),
                                        FirstName = reader["StaffName"]?.ToString(),
                                        MobileNumber = reader["MobileNumber"]?.ToString(),
                                        Email = reader["Email"]?.ToString(),
                                        Status = reader["Message"]?.ToString()
                                    };

                                    StaffList.Add(staffRecord);
                                }
                            }
                        }
                        else
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var staffRecord = new tblStaff
                                    {
                                        ID = reader["ID"]?.ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        StaffType = reader["StaffType"]?.ToString(),
                                        FirstName = reader["FirstName"]?.ToString(),
                                        MiddleName = reader["MiddleName"]?.ToString(),
                                        LastName = reader["LastName"]?.ToString(),
                                        MobileNumber = reader["MobileNumber"]?.ToString(),
                                        Email = reader["Email"]?.ToString(),
                                        DateOfBirth = reader["DateOfBirth"] == DBNull.Value ? null : Convert.ToDateTime(reader["DateOfBirth"]),
                                        Qualification = reader["Qualification"]?.ToString(),
                                        IsActive = reader["IsActive"]?.ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString()
                                    };

                                    StaffList.Add(staffRecord);
                                }
                            }
                        }
                    }

                }

                return StaffList;
            }
            catch (Exception ex)
            {
                LogException(ex, "SchoolManagementDAL", "Tbl_Staff_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(staff));
                return new List<tblStaff>
                {
                    new tblStaff
                    {
                        Status = $"ERROR: {ex.Message}"
                    }
                };
            }
        }

        public List<tblSubjects> Tbl_Subjects_CRUD_Operations(tblSubjects subject)
        {
            var Subjects = new List<tblSubjects>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {
                using var conn = new MySqlConnection(_connectionString);
                using var cmd = new MySqlCommand("Proc_Tbl_Subject", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("p_ID", (object?)CleanParam(subject.ID) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_SchoolID", (object?)CleanParam(subject.SchoolID) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_AcademicYear", (object?)CleanParam(subject.AcademicYear) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Class", (object?)CleanParam(subject.Class) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Name", (object?)CleanParam(subject.Name) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Description", (object?)CleanParam(subject.Description) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Topics", (object?)CleanParam(subject.Topics) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_IsActive", (object?)CleanParam(subject.IsActive) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(subject.CreatedBy) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_CreatedIp", (object?)CleanParam(subject.CreatedIp) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(subject.ModifiedBy) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModifiedIp", (object?)CleanParam(subject.ModifiedIp) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Flag", (object?)CleanParam(subject.Flag) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Limit", subject.Limit ?? 100);
                cmd.Parameters.AddWithValue("p_LastCreatedDate", subject.LastCreatedDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_LastID", subject.LastID ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_SortColumn", (object?)CleanParam(subject.SortColumn) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_SortDirection", (object?)CleanParam(subject.SortDirection) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Offset", subject.Offset ?? (object)DBNull.Value);

                conn.Open();
                if (subject.Flag != null)
                {
                    if (subject.Flag == "9")
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var ClassDivision = new tblSubjects
                                {
                                    SubjectID = reader["SubjectID"]?.ToString(),
                                    SyllabusClassName = reader["SyllabusClassSubjectName"]?.ToString(),
                                    ClassID = reader["ClassID"]?.ToString(),
                                    SyllabusID = reader["SyllabusID"]?.ToString(),
                                    Status = reader["Message"]?.ToString()
                                };

                                Subjects.Add(ClassDivision);
                            }
                        }
                    }
                    else if (subject.Flag == "6" || subject.Flag == "8")
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Subjects.Add(new tblSubjects
                                {
                                    totalcount = reader["totalCount"] != DBNull.Value ? Convert.ToInt32(reader["totalCount"]) : (int?)null
                                });
                            }
                        }
                    }
                    else if (subject.Flag == "2" || subject.Flag == "3" || subject.Flag == "4" || subject.Flag == "7")
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Subjects.Add(new tblSubjects
                                {
                                    ID = reader["ID"]?.ToString(),
                                    SchoolID = reader["SchoolID"]?.ToString(),
                                    AcademicYear = reader["AcademicYear"]?.ToString(),
                                    Class = reader["Class"]?.ToString(),
                                    Name = reader["Name"]?.ToString(),
                                    Topics = reader["Topics"]?.ToString(),
                                    Description = reader["Description"]?.ToString(),
                                    IsActive = reader["IsActive"]?.ToString(),
                                    CreatedBy = reader["CreatedBy"]?.ToString(),
                                    CreatedIp = reader["CreatedIp"]?.ToString(),
                                    CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                    ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                    ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                    ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                    Status = reader["Message"]?.ToString(),
                                    SchoolName = reader["SchoolName"]?.ToString(),
                                    AcademicYearName = reader["AcademicYearName"]?.ToString(),
                                    ClassName = reader["ClassName"]?.ToString()
                                });
                            }
                        }
                    }
                    else if (subject.Flag == "1" || subject.Flag == "5")
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (reader["Message"]?.ToString() == "Subject name already exists")
                                {
                                    Subjects.Add(new tblSubjects
                                    {
                                        Status = reader["Message"]?.ToString()
                                    });
                                }
                                else
                                {
                                    Subjects.Add(new tblSubjects
                                    {
                                        ID = reader["ID"]?.ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        Class = reader["Class"]?.ToString(),
                                        Name = reader["Name"]?.ToString(),
                                        Topics = reader["Topics"]?.ToString(),
                                        Description = reader["Description"]?.ToString(),
                                        IsActive = reader["IsActive"]?.ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString()
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        using var reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            Subjects.Add(new tblSubjects
                            {
                                ID = reader["ID"]?.ToString(),
                                SchoolID = reader["SchoolID"]?.ToString(),
                                AcademicYear = reader["AcademicYear"]?.ToString(),
                                Class = reader["Class"]?.ToString(),
                                Name = reader["Name"]?.ToString(),
                                Topics = reader["Topics"]?.ToString(),
                                Description = reader["Description"]?.ToString(),
                                IsActive = reader["IsActive"]?.ToString(),
                                CreatedBy = reader["CreatedBy"]?.ToString(),
                                CreatedIp = reader["CreatedIp"]?.ToString(),
                                CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                Status = reader["Message"]?.ToString()
                            });
                        }
                    }
                }
                return Subjects;
            }
            catch (Exception ex)
            {
                LogException(ex, "SchoolManagementDAL", "Tbl_Subjects_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(subject));
                return new List<tblSubjects>
                {
                    new tblSubjects
                    {
                        Status = $"ERROR: {ex.Message}"
                    }
                };
            }
        }

        public List<tblSubjectStaff> Tbl_SubjectStaff_CRUD_Operations(tblSubjectStaff subjectStaff)
        {
            var SubjectStaffs = new List<tblSubjectStaff>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {
                using var conn = new MySqlConnection(_connectionString);
                using var cmd = new MySqlCommand("Proc_Tbl_SubjectStaff", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("p_ID", (object?)CleanParam(subjectStaff.ID) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Class", (object?)CleanParam(subjectStaff.Class) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_StaffName", (object?)CleanParam(subjectStaff.StaffName) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(subjectStaff.CreatedBy) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_CreatedIp", (object?)CleanParam(subjectStaff.CreatedIp) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(subjectStaff.ModifiedBy) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModifiedIp", (object?)CleanParam(subjectStaff.ModifiedIp) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Flag", (object?)CleanParam(subjectStaff.Flag) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_IsActive", (object?)CleanParam(subjectStaff.IsActive) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Limit", subjectStaff.Limit ?? 100);
                cmd.Parameters.AddWithValue("p_LastCreatedDate", subjectStaff.LastCreatedDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_LastID", subjectStaff.LastID ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_SortColumn", (object?)CleanParam(subjectStaff.SortColumn) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_SortDirection", (object?)CleanParam(subjectStaff.SortDirection) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Offset", subjectStaff.Offset ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_SchoolID", (object?)CleanParam(subjectStaff.SchoolID) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_AcademicYear", (object?)CleanParam(subjectStaff.AcademicYear) ?? DBNull.Value);

                conn.Open();
                if (subjectStaff.Flag != null)
                {
                    if (subjectStaff.Flag == "6" || subjectStaff.Flag == "8")
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SubjectStaffs.Add(new tblSubjectStaff
                                {
                                    totalcount = reader["totalCount"] != DBNull.Value ? Convert.ToInt32(reader["totalCount"]) : (int?)null
                                });
                            }
                        }
                    }
                    else if (subjectStaff.Flag == "2" || subjectStaff.Flag == "3" || subjectStaff.Flag == "4" || subjectStaff.Flag == "7")
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SubjectStaffs.Add(new tblSubjectStaff
                                {
                                    ID = reader["ID"]?.ToString(),
                                    SchoolID = reader["SchoolID"]?.ToString(),
                                    AcademicYear = reader["AcademicYear"]?.ToString(),
                                    Class = reader["Class"]?.ToString(),
                                    StaffName = reader["StaffName"]?.ToString(),
                                    CreatedBy = reader["CreatedBy"]?.ToString(),
                                    CreatedIp = reader["CreatedIp"]?.ToString(),
                                    CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                    ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                    ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                    ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                    Status = reader["Message"]?.ToString(),
                                    SchoolName = reader["SchoolName"]?.ToString(),
                                    AcademicYearName = reader["AcademicYearName"]?.ToString(),
                                    ClassName = reader["ClassName"]?.ToString(),
                                    StaffFullName = reader["StaffFullName"]?.ToString()
                                });
                            }
                        }
                    }
                    else
                    {
                        using var reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            SubjectStaffs.Add(new tblSubjectStaff
                            {
                                ID = reader["ID"]?.ToString(),
                                SchoolID = reader["SchoolID"]?.ToString(),
                                AcademicYear = reader["AcademicYear"]?.ToString(),
                                Class = reader["Class"]?.ToString(),
                                StaffName = reader["StaffName"]?.ToString(),
                                CreatedBy = reader["CreatedBy"]?.ToString(),
                                CreatedIp = reader["CreatedIp"]?.ToString(),
                                CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                Status = reader["Message"]?.ToString()
                            });
                        }
                    }

                }
                return SubjectStaffs;
            }
            catch (Exception ex)
            {
                return new List<tblSubjectStaff> { new tblSubjectStaff { Status = $"ERROR: {ex.Message}" } };
            }
        }

        public List<tblModules> Tbl_Modules_CRUD_Operations(tblModules module)
        {
            var modules = new List<tblModules>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                using (var cmd = new MySqlCommand("Proc_Tbl_Module", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("p_ID", module.ID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModuleName", (object?)CleanParam(module.ModuleName) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Description", (object?)CleanParam(module.Description) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_IsActive", module.IsActive ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(module.CreatedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedIp", (object?)CleanParam(module.CreatedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(module.ModifiedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedIp", (object?)CleanParam(module.ModifiedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Flag", (object?)CleanParam(module.Flag) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Limit", module.Limit ?? 100);
                    cmd.Parameters.AddWithValue("p_Offset", module.Offset ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastCreatedDate", module.LastCreatedDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastID", module.LastID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortColumn", (object?)CleanParam(module.SortColumn) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortDirection", (object?)CleanParam(module.SortDirection) ?? DBNull.Value);

                    conn.Open();

                    if (!string.IsNullOrWhiteSpace(module.Flag))
                    {
                        if (module.Flag == "6" || module.Flag == "8")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    modules.Add(new tblModules
                                    {
                                        totalCount = reader["totalCount"] != DBNull.Value ? Convert.ToInt32(reader["totalCount"]) : (int?)null
                                    });
                                }
                            }
                        }
                        else if (module.Flag == "1" || module.Flag == "4")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (reader["Message"]?.ToString() == "Module Name Already Exists")
                                    {
                                        modules.Add(new tblModules
                                        {
                                            Status = reader["Message"]?.ToString()
                                        });
                                    }
                                    else
                                    {
                                        modules.Add(new tblModules
                                        {
                                            ID = reader["ID"].ToString(),
                                            ModuleName = reader["ModuleName"]?.ToString(),
                                            Description = reader["Description"]?.ToString(),
                                            IsActive = reader["IsActive"].ToString(),
                                            CreatedBy = reader["CreatedBy"]?.ToString(),
                                            CreatedIp = reader["CreatedIp"]?.ToString(),
                                            CreatedDate = reader["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedDate"]) : (DateTime?)null,
                                            ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                            ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                            ModifiedDate = reader["ModifiedDate"] != DBNull.Value ? Convert.ToDateTime(reader["ModifiedDate"]) : (DateTime?)null,
                                            Status = reader["Message"]?.ToString()
                                        });
                                    }
                                }
                            }
                        }
                        else
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    modules.Add(new tblModules
                                    {
                                        ID = reader["ID"].ToString(),
                                        ModuleName = reader["ModuleName"]?.ToString(),
                                        Description = reader["Description"]?.ToString(),
                                        IsActive = reader["IsActive"].ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedDate"]) : (DateTime?)null,
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] != DBNull.Value ? Convert.ToDateTime(reader["ModifiedDate"]) : (DateTime?)null,
                                        Status = reader["Message"]?.ToString()
                                    });
                                }
                            }
                        }
                    }
                }

                return modules;
            }
            catch (Exception ex)
            {
                LogException(ex, "SchoolManagementDAL", "Tbl_Modules_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(module));
                return new List<tblModules>
                {
                    new tblModules
                    {
                        Status = $"ERROR: {ex.Message}"
                    }
                };
            }
        }

        public List<tblPages> Tbl_Pages_CRUD_Operations(tblPages page)
        {
            var pages = new List<tblPages>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {
                using var conn = new MySqlConnection(_connectionString);
                using var cmd = new MySqlCommand("Proc_Tbl_Page", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("p_ID", page.ID ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModuleID", page.ModuleID ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_PageName", (object?)CleanParam(page.PageName) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Description", (object?)CleanParam(page.Description) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_IsActive", page.IsActive ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(page.CreatedBy) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_CreatedIp", (object?)CleanParam(page.CreatedIp) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(page.ModifiedBy) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModifiedIp", (object?)CleanParam(page.ModifiedIp) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Flag", (object?)CleanParam(page.Flag) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Limit", page.Limit ?? 100);
                cmd.Parameters.AddWithValue("p_LastCreatedDate", page.LastCreatedDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_LastID", page.LastID ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_SortColumn", (object?)CleanParam(page.SortColumn) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_SortDirection", (object?)CleanParam(page.SortDirection) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Offset", page.Offset ?? (object)DBNull.Value);

                conn.Open();

                if (!string.IsNullOrEmpty(page.Flag))
                {
                    if (page.Flag == "6" || page.Flag == "8")
                    {
                        using var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            pages.Add(new tblPages
                            {
                                TotalCount = reader["totalCount"] != DBNull.Value
                                    ? Convert.ToInt32(reader["totalCount"])
                                    : (int?)null
                            });
                        }
                    }
                    else if (page.Flag == "2" || page.Flag == "3" || page.Flag == "4" || page.Flag == "7")
                    {
                        using var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            pages.Add(new tblPages
                            {
                                ID = reader["ID"].ToString(),
                                ModuleID = reader["ModuleID"].ToString(),
                                PageName = reader["PageName"]?.ToString(),
                                Description = reader["Description"]?.ToString(),
                                IsActive = reader["IsActive"].ToString(),
                                CreatedBy = reader["CreatedBy"]?.ToString(),
                                CreatedIp = reader["CreatedIp"]?.ToString(),
                                CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                Status = reader["Message"]?.ToString(),
                                ModuleName = reader["ModuleName"]?.ToString()
                            });
                        }
                    }
                    else if (page.Flag == "1" || page.Flag == "5")
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (reader["Message"]?.ToString() == "Page Name Already Exists")
                                {
                                    pages.Add(new tblPages
                                    {
                                        Status = reader["Message"]?.ToString()
                                    });
                                }
                                else
                                {
                                    pages.Add(new tblPages
                                    {
                                        ID = reader["ID"].ToString(),
                                        ModuleID = reader["ModuleID"].ToString(),
                                        PageName = reader["PageName"]?.ToString(),
                                        Description = reader["Description"]?.ToString(),
                                        IsActive = reader["IsActive"].ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString()
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        using var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            pages.Add(new tblPages
                            {
                                ID = reader["ID"].ToString(),
                                ModuleID = reader["ModuleID"].ToString(),
                                PageName = reader["PageName"]?.ToString(),
                                Description = reader["Description"]?.ToString(),
                                IsActive = reader["IsActive"].ToString(),
                                CreatedBy = reader["CreatedBy"]?.ToString(),
                                CreatedIp = reader["CreatedIp"]?.ToString(),
                                CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                Status = reader["Message"]?.ToString()
                            });
                        }
                    }
                }

                return pages;
            }
            catch (Exception ex)
            {
                LogException(ex, "SchoolManagementDAL", "Tbl_Pages_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(page));
                return new List<tblPages>
                {
                    new tblPages { Status = $"ERROR: {ex.Message}" }
                };
            }
        }

        public List<tblRoles> Tbl_Roles_CRUD_Operations(tblRoles role)
        {
            var roles = new List<tblRoles>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {
                using var conn = new MySqlConnection(_connectionString);
                using var cmd = new MySqlCommand("Proc_Tbl_Role", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("p_ID", (object?)CleanParam(role.ID) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_SchoolID", (object?)CleanParam(role.SchoolID) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_RoleName", (object?)CleanParam(role.RoleName) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Description", (object?)CleanParam(role.Description) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_IsActive", (object?)CleanParam(role.IsActive) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(role.CreatedBy) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_CreatedIp", (object?)CleanParam(role.CreatedIp) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(role.ModifiedBy) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModifiedIp", (object?)CleanParam(role.ModifiedIp) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Flag", (object?)CleanParam(role.Flag) ?? DBNull.Value);

                conn.Open();
                if (role.Flag != null)
                {
                    using var reader = cmd.ExecuteReader();
                    if (role.Flag == "6")
                    {
                        while (reader.Read())
                        {
                            if (reader.Read())
                            {
                                roles.Add(new tblRoles
                                {
                                    ID = reader["NewID"]?.ToString(),
                                    Status = reader["Message"]?.ToString()
                                });
                            }
                        }
                    }
                    else if (role.Flag == "1" || role.Flag == "4")
                    {

                        while (reader.Read())
                        {
                            if (reader["Message"]?.ToString() == "Role Already Exists")
                            {
                                roles.Add(new tblRoles
                                {
                                    Status = reader["Message"]?.ToString()
                                });

                            }
                            else
                            {
                                roles.Add(new tblRoles
                                {
                                    ID = reader["ID"]?.ToString(),
                                    SchoolID = reader["SchoolID"]?.ToString(),
                                    RoleName = reader["RoleName"]?.ToString(),
                                    Description = reader["Description"]?.ToString(),
                                    IsActive = reader["IsActive"]?.ToString(),
                                    CreatedBy = reader["CreatedBy"]?.ToString(),
                                    CreatedIp = reader["CreatedIp"]?.ToString(),
                                    CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                    ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                    ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                    ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                    Status = reader["Message"]?.ToString()
                                });

                            }
                        }

                    }
                    else
                    {
                        while (reader.Read())
                        {
                            roles.Add(new tblRoles
                            {
                                ID = reader["ID"]?.ToString(),
                                SchoolID = reader["SchoolID"]?.ToString(),
                                RoleName = reader["RoleName"]?.ToString(),
                                Description = reader["Description"]?.ToString(),
                                IsActive = reader["IsActive"]?.ToString(),
                                CreatedBy = reader["CreatedBy"]?.ToString(),
                                CreatedIp = reader["CreatedIp"]?.ToString(),
                                CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                Status = reader["Message"]?.ToString()
                            });
                        }
                    }

                }
                return roles;
            }
            catch (Exception ex)
            {
                LogException(ex, "SchoolManagementDAL", "Tbl_Roles_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(role));
                return new List<tblRoles> { new tblRoles { Status = $"ERROR: {ex.Message}" } };
            }
        }

        public List<tblUserRoles> Tbl_UserRoles_CRUD_Operations(tblUserRoles userRole)
        {
            var userRoles = new List<tblUserRoles>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {
                using var conn = new MySqlConnection(_connectionString);
                using var cmd = new MySqlCommand("Proc_Tbl_UserRole", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("p_UserID", (object?)CleanParam(userRole.UserID) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_RoleID", (object?)CleanParam(userRole.RoleID) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Flag", (object?)CleanParam(userRole.Flag) ?? DBNull.Value);

                conn.Open();
                if (userRole.Flag != null)
                {
                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        userRoles.Add(new tblUserRoles
                        {
                            UserID = reader["UserID"]?.ToString(),
                            RoleID = reader["RoleID"]?.ToString(),
                            Status = reader["Message"]?.ToString()
                        });
                    }
                }
                return userRoles;
            }
            catch (Exception ex)
            {
                return new List<tblUserRoles> { new tblUserRoles { Status = $"ERROR: {ex.Message}" } };
            }
        }

        public List<tblRolePermissions> Tbl_RolePermissions_CRUD_Operations(List<tblRolePermissions> rolePerms)
        {
            var perms = new List<tblRolePermissions>();

            int SafeParse(string? value)
            {
                if (string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string")
                    return 0;
                return int.TryParse(value, out int result) ? result : 0;
            }

            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();

                foreach (var rolePerm in rolePerms)
                {
                    using var cmd = new MySqlCommand("Proc_Tbl_RolePermission", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("p_RoleID", SafeParse(rolePerm.RoleID));
                    cmd.Parameters.AddWithValue("p_PageID", SafeParse(rolePerm.PageID));
                    cmd.Parameters.AddWithValue("p_CanView", SafeParse(rolePerm.CanView));
                    cmd.Parameters.AddWithValue("p_CanAdd", SafeParse(rolePerm.CanAdd));
                    cmd.Parameters.AddWithValue("p_CanEdit", SafeParse(rolePerm.CanEdit));
                    cmd.Parameters.AddWithValue("p_CanDelete", SafeParse(rolePerm.CanDelete));
                    cmd.Parameters.AddWithValue("p_Flag", rolePerm.Flag ?? "1");

                    using var reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            perms.Add(new tblRolePermissions
                            {
                                RoleID = reader["RoleID"]?.ToString(),
                                PageID = reader["PageID"]?.ToString(),
                                CanView = reader["CanView"]?.ToString(),
                                CanAdd = reader["CanAdd"]?.ToString(),
                                CanEdit = reader["CanEdit"]?.ToString(),
                                CanDelete = reader["CanDelete"]?.ToString(),
                                Status = reader["Message"]?.ToString() ?? "Success"
                            });
                        }
                    }
                    else
                    {
                        perms.Add(new tblRolePermissions { Status = "No data returned" });
                    }
                }

                return perms;
            }
            catch (Exception ex)
            {
                return new List<tblRolePermissions> { new tblRolePermissions { Status = $"ERROR: {ex.Message}" } };
            }
        }

        public List<tblModules> Tbl_GetRoleMenuPermissions(string roleId)
        {
            var modules = new List<tblModules>();

            try
            {
                using var conn = new MySqlConnection(_connectionString);
                using var cmd = new MySqlCommand("sp_get_role_menu_permissions", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@p_role_ids", roleId);

                conn.Open();
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string moduleId = reader["ModuleID"]?.ToString();
                    string moduleName = reader["ModuleName"]?.ToString();

                    // Check if module already exists in the list
                    var module = modules.FirstOrDefault(m => m.ID == moduleId);
                    if (module == null)
                    {
                        module = new tblModules
                        {
                            ID = moduleId,
                            ModuleName = moduleName,
                            // Use Flag property to store pages temporarily
                            Flag = null,
                            Status = null,
                            CreatedBy = null,
                            CreatedIp = null,
                            CreatedDate = null,
                            ModifiedBy = null,
                            ModifiedIp = null,
                            ModifiedDate = null,
                            Description = null,
                            IsActive = "1"
                        };
                        modules.Add(module);
                    }

                    // Add page to module's Pages (reuse tblPages model)
                    if (module.Description == null)
                        module.Description = ""; // Use Description as a JSON-like string or placeholder

                    if (module.Pages == null)
                        module.Pages = new List<tblPages>();

                    module.Pages.Add(new tblPages
                    {
                        ID = reader["PageID"]?.ToString(),
                        PageName = reader["PageName"]?.ToString(),
                        ModuleID = moduleId,
                        IsActive = "1",
                        // store permissions in Page's properties (reuse)
                        CanView = reader["CanView"]?.ToString(),
                        CanAdd = reader["CanAdd"]?.ToString(),
                        CanEdit = reader["CanEdit"]?.ToString(),
                        CanDelete = reader["CanDelete"]?.ToString()
                    });
                }

                return modules;
            }
            catch (Exception ex)
            {
                return new List<tblModules> { new tblModules { ModuleName = "ERROR", Description = ex.Message } };
            }
        }




        //Academic Module
        public List<tblStudentDetails> Tbl_StudentDetails_CRUD_Operations(tblStudentDetails admission)
        {
            var Admissions = new List<tblStudentDetails>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {
                using var conn = new MySqlConnection(_connectionString);
                using var cmd = new MySqlCommand("Proc_Tbl_StudentDetails", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Add parameters based on the tblStudentDetails class
                cmd.Parameters.AddWithValue("p_ID", (object?)CleanParam(admission.ID) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_SchoolID", (object?)CleanParam(admission.SchoolID) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_AcademicYear", (object?)admission.AcademicYear ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_AdmissionNo", (object?)CleanParam(admission.AdmissionNo) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Syllabus", (object?)CleanParam(admission.Syllabus) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Class", (object?)admission.Class ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Division", (object?)CleanParam(admission.Division) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_JoinDate", (object?)admission.JoinDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_FirstName", (object?)CleanParam(admission.FirstName) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_MiddleName", (object?)CleanParam(admission.MiddleName) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_LastName", (object?)CleanParam(admission.LastName) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_AadharNo", (object?)CleanParam(admission.AadharNo) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_MobileNo", (object?)CleanParam(admission.MobileNo) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_EmailID", (object?)CleanParam(admission.EmailID) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_DOB", (object?)admission.DOB ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Gender", (object?)CleanParam(admission.Gender) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_BloodGroup", (object?)CleanParam(admission.BloodGroup) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Nationality", (object?)CleanParam(admission.Nationality) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Religion", (object?)CleanParam(admission.Religion) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Caste", (object?)CleanParam(admission.Caste) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_DocumentDetails", (object?)CleanParam(admission.DocumentDetails) ?? DBNull.Value);


                cmd.Parameters.AddWithValue("p_IsActive", (object?)CleanParam(admission.IsActive) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(admission.CreatedBy) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_CreatedIp", (object?)CleanParam(admission.CreatedIp) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(admission.ModifiedBy) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModifiedIp", (object?)CleanParam(admission.ModifiedIp) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Flag", (object?)CleanParam(admission.Flag) ?? DBNull.Value);

                cmd.Parameters.AddWithValue("p_Limit", admission.Limit ?? 100);
                cmd.Parameters.AddWithValue("p_LastCreatedDate", admission.LastCreatedDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_LastID", admission.LastID ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_SortColumn", (object?)CleanParam(admission.SortColumn) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_SortDirection", (object?)CleanParam(admission.SortDirection) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Offset", admission.Offset ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("p_DePromotionRemarks", (object?)CleanParam(admission.DePromotionRemarks) ?? DBNull.Value);

                conn.Open();
                if (admission.Flag != null)
                {
                    if (admission.Flag == "9")
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var ClassDivision = new tblStudentDetails
                                {
                                    NewAdmissionNo = reader["NewAdmissionNo"]?.ToString()
                                };

                                Admissions.Add(ClassDivision);
                            }
                        }
                    }
                    else if (admission.Flag == "6" || admission.Flag == "8")
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Admissions.Add(new tblStudentDetails
                                {
                                    totalcount = reader["totalCount"] != DBNull.Value ? Convert.ToInt32(reader["totalCount"]) : (int?)null
                                });
                            }
                        }
                    }
                    else if (admission.Flag == "2" || admission.Flag == "3" || admission.Flag == "4" || admission.Flag == "7")
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (!reader.HasRows && reader.NextResult()) { }
                            while (reader.Read())
                            {
                                Admissions.Add(new tblStudentDetails
                                {
                                    ID = reader["ID"]?.ToString(),
                                    SchoolID = reader["SchoolID"]?.ToString(),
                                    AcademicYear = reader["AcademicYear"]?.ToString(),
                                    AdmissionNo = reader["AdmissionNo"]?.ToString(),
                                    Syllabus = reader["Syllabus"]?.ToString(),
                                    Class = reader["Class"]?.ToString(),
                                    Division = reader["Division"]?.ToString(),
                                    JoinDate = reader["JoinDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["JoinDate"]),
                                    FirstName = reader["FirstName"]?.ToString(),
                                    MiddleName = reader["MiddleName"]?.ToString(),
                                    LastName = reader["LastName"]?.ToString(),
                                    AadharNo = reader["AadharNo"]?.ToString(),
                                    MobileNo = reader["MobileNo"]?.ToString(),
                                    EmailID = reader["EmailID"]?.ToString(),
                                    DOB = reader["DOB"] == DBNull.Value ? null : Convert.ToDateTime(reader["DOB"]),
                                    Gender = reader["Gender"]?.ToString(),
                                    BloodGroup = reader["BloodGroup"]?.ToString(),
                                    Nationality = reader["Nationality"]?.ToString(),
                                    Religion = reader["Religion"]?.ToString(),
                                    Caste = reader["Caste"]?.ToString(),
                                    DocumentDetails = reader["DocumentDetails"]?.ToString(),
                                    IsActive = reader["IsActive"]?.ToString(),
                                    CreatedBy = reader["CreatedBy"]?.ToString(),
                                    CreatedIp = reader["CreatedIp"]?.ToString(),
                                    CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                    ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                    ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                    ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                    Status = reader["Message"]?.ToString(),
                                    SchoolName = reader["SchoolName"]?.ToString(),
                                    AcademicYearName = reader["AcademicYearName"]?.ToString(),
                                    ClassName = reader["ClassName"]?.ToString(),
                                    ClassDivisionName = reader["ClassDivisionName"]?.ToString()
                                });
                            }
                        }
                    }
                    else if (admission.Flag == "1" || admission.Flag == "5")
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (reader["Message"]?.ToString() == "AdmissionNo already exists")
                                {
                                    Admissions.Add(new tblStudentDetails
                                    {
                                        Status = reader["Message"]?.ToString()
                                    });
                                }
                                else
                                {
                                    Admissions.Add(new tblStudentDetails
                                    {
                                        ID = reader["ID"]?.ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        AdmissionNo = reader["AdmissionNo"]?.ToString(),
                                        Syllabus = reader["Syllabus"]?.ToString(),
                                        Class = reader["Class"]?.ToString(),
                                        Division = reader["Division"]?.ToString(),
                                        JoinDate = reader["JoinDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["JoinDate"]),
                                        FirstName = reader["FirstName"]?.ToString(),
                                        MiddleName = reader["MiddleName"]?.ToString(),
                                        LastName = reader["LastName"]?.ToString(),
                                        AadharNo = reader["AadharNo"]?.ToString(),
                                        MobileNo = reader["MobileNo"]?.ToString(),
                                        EmailID = reader["EmailID"]?.ToString(),
                                        DOB = reader["DOB"] == DBNull.Value ? null : Convert.ToDateTime(reader["DOB"]),
                                        Gender = reader["Gender"]?.ToString(),
                                        BloodGroup = reader["BloodGroup"]?.ToString(),
                                        Nationality = reader["Nationality"]?.ToString(),
                                        Religion = reader["Religion"]?.ToString(),
                                        Caste = reader["Caste"]?.ToString(),
                                        DocumentDetails = reader["DocumentDetails"]?.ToString(),
                                        IsActive = reader["IsActive"]?.ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString()
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Admissions.Add(new tblStudentDetails
                                {
                                    ID = reader["ID"]?.ToString(),
                                    SchoolID = reader["SchoolID"]?.ToString(),
                                    AcademicYear = reader["AcademicYear"]?.ToString(),
                                    AdmissionNo = reader["AdmissionNo"]?.ToString(),
                                    Syllabus = reader["Syllabus"]?.ToString(),
                                    Class = reader["Class"]?.ToString(),
                                    Division = reader["Division"]?.ToString(),
                                    JoinDate = reader["JoinDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["JoinDate"]),
                                    FirstName = reader["FirstName"]?.ToString(),
                                    MiddleName = reader["MiddleName"]?.ToString(),
                                    LastName = reader["LastName"]?.ToString(),
                                    AadharNo = reader["AadharNo"]?.ToString(),
                                    MobileNo = reader["MobileNo"]?.ToString(),
                                    EmailID = reader["EmailID"]?.ToString(),
                                    DOB = reader["DOB"] == DBNull.Value ? null : Convert.ToDateTime(reader["DOB"]),
                                    Gender = reader["Gender"]?.ToString(),
                                    BloodGroup = reader["BloodGroup"]?.ToString(),
                                    Nationality = reader["Nationality"]?.ToString(),
                                    Religion = reader["Religion"]?.ToString(),
                                    Caste = reader["Caste"]?.ToString(),
                                    DocumentDetails = reader["DocumentDetails"]?.ToString(),
                                    IsActive = reader["IsActive"]?.ToString(),
                                    CreatedBy = reader["CreatedBy"]?.ToString(),
                                    CreatedIp = reader["CreatedIp"]?.ToString(),
                                    CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                    ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                    ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                    ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                    Status = reader["Message"]?.ToString()
                                });
                            }
                        }
                    }
                }
                return Admissions;
            }
            catch (Exception ex)
            {
                LogException(ex, "SchoolManagementDAL", "Tbl_StudentDet_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(admission));
                return new List<tblStudentDetails>
                {
                    new tblStudentDetails
                    {
                        Status = $"ERROR: {ex.Message}"
                    }
                };
            }
        }

        public List<tblStudentAddressDetails> Tbl_StudentAddressDetails_CRUD_Operations(tblStudentAddressDetails admission)
        {
            var Admissions = new List<tblStudentAddressDetails>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {
                using var conn = new MySqlConnection(_connectionString);
                using var cmd = new MySqlCommand("Proc_Tbl_StudentAddress", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Add parameters based on the tblStudentDetails class
                cmd.Parameters.AddWithValue("p_ID", (object?)CleanParam(admission.ID) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_AdmissionID", (object?)CleanParam(admission.AdmissionID) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_PermanentAddressLine1", (object?)CleanParam(admission.PermanentAddressLine1) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_PermanentAddressLine2", (object?)CleanParam(admission.PermanentAddressLine2) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_PermanentPinCode", (object?)CleanParam(admission.PermanentPinCode) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_PermanentPlace", (object?)CleanParam(admission.PermanentPlace) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_PermanentCountry", (object?)CleanParam(admission.PermanentCountry) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_PermanentState", (object?)CleanParam(admission.PermanentState) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_PermanentDistrict", (object?)CleanParam(admission.PermanentDistrict) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_PermanentCity", (object?)CleanParam(admission.PermanentCity) ?? DBNull.Value);

                cmd.Parameters.AddWithValue("p_TemporaryAddressLine1", (object?)CleanParam(admission.TemporaryAddressLine1) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_TemporaryAddressLine2", (object?)CleanParam(admission.TemporaryAddressLine2) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_TemporaryPinCode", (object?)CleanParam(admission.TemporaryPinCode) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_TemporaryPlace", (object?)CleanParam(admission.TemporaryPlace) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_TemporaryCountry", (object?)CleanParam(admission.TemporaryCountry) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_TemporaryState", (object?)CleanParam(admission.TemporaryState) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_TemporaryDistrict", (object?)CleanParam(admission.TemporaryDistrict) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_TemporaryCity", (object?)CleanParam(admission.TemporaryCity) ?? DBNull.Value);

                cmd.Parameters.AddWithValue("p_IsActive", (object?)CleanParam(admission.IsActive) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(admission.CreatedBy) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_CreatedIp", (object?)CleanParam(admission.CreatedIp) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(admission.ModifiedBy) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModifiedIp", (object?)CleanParam(admission.ModifiedIp) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Flag", (object?)CleanParam(admission.Flag) ?? DBNull.Value);

                cmd.Parameters.AddWithValue("p_Limit", admission.Limit ?? 100);
                cmd.Parameters.AddWithValue("p_LastCreatedDate", admission.LastCreatedDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_LastID", admission.LastID ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_SortColumn", (object?)CleanParam(admission.SortColumn) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_SortDirection", (object?)CleanParam(admission.SortDirection) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Offset", admission.Offset ?? (object)DBNull.Value);

                conn.Open();
                if (admission.Flag != null)
                {
                    if (admission.Flag == "6" || admission.Flag == "8")
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Admissions.Add(new tblStudentAddressDetails
                                {
                                    totalcount = reader["totalCount"] != DBNull.Value ? Convert.ToInt32(reader["totalCount"]) : (int?)null
                                });
                            }
                        }
                    }
                    else if (admission.Flag == "2" || admission.Flag == "3" || admission.Flag == "4" || admission.Flag == "7")
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (!reader.HasRows && reader.NextResult()) { }
                            while (reader.Read())
                            {
                                Admissions.Add(new tblStudentAddressDetails
                                {
                                    ID = reader["ID"]?.ToString(),
                                    AdmissionID = reader["AdmissionID"]?.ToString(),
                                    PermanentAddressLine1 = reader["PermanentAddressLine1"]?.ToString(),
                                    PermanentAddressLine2 = reader["PermanentAddressLine2"]?.ToString(),
                                    PermanentPinCode = reader["PermanentPinCode"]?.ToString(),
                                    PermanentPlace = reader["PermanentPlace"]?.ToString(),
                                    PermanentCountry = reader["PermanentCountry"]?.ToString(),
                                    PermanentState = reader["PermanentState"]?.ToString(),
                                    PermanentDistrict = reader["PermanentDistrict"]?.ToString(),
                                    PermanentCity = reader["PermanentCity"]?.ToString(),
                                    TemporaryAddressLine1 = reader["TemporaryAddressLine1"]?.ToString(),
                                    TemporaryAddressLine2 = reader["TemporaryAddressLine2"]?.ToString(),
                                    TemporaryPinCode = reader["TemporaryPinCode"]?.ToString(),
                                    TemporaryPlace = reader["TemporaryPlace"]?.ToString(),
                                    TemporaryCountry = reader["TemporaryCountry"]?.ToString(),
                                    TemporaryState = reader["TemporaryState"]?.ToString(),
                                    TemporaryDistrict = reader["TemporaryDistrict"]?.ToString(),
                                    TemporaryCity = reader["TemporaryCity"]?.ToString(),
                                    IsActive = reader["IsActive"]?.ToString(),
                                    CreatedBy = reader["CreatedBy"]?.ToString(),
                                    CreatedIp = reader["CreatedIp"]?.ToString(),
                                    CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                    ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                    ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                    ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                    Status = reader["Message"]?.ToString()
                                });
                            }
                        }
                    }
                    else if (admission.Flag == "1" || admission.Flag == "5")
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (reader["Message"]?.ToString() == "AdmissionNo already exists")
                                {
                                    Admissions.Add(new tblStudentAddressDetails
                                    {
                                        Status = reader["Message"]?.ToString()
                                    });
                                }
                                else
                                {
                                    Admissions.Add(new tblStudentAddressDetails
                                    {
                                        ID = reader["ID"]?.ToString(),
                                        AdmissionID = reader["AdmissionID"]?.ToString(),
                                        PermanentAddressLine1 = reader["PermanentAddressLine1"]?.ToString(),
                                        PermanentAddressLine2 = reader["PermanentAddressLine2"]?.ToString(),
                                        PermanentPinCode = reader["PermanentPinCode"]?.ToString(),
                                        PermanentPlace = reader["PermanentPlace"]?.ToString(),
                                        PermanentCountry = reader["PermanentCountry"]?.ToString(),
                                        PermanentState = reader["PermanentState"]?.ToString(),
                                        PermanentDistrict = reader["PermanentDistrict"]?.ToString(),
                                        PermanentCity = reader["PermanentCity"]?.ToString(),
                                        TemporaryAddressLine1 = reader["TemporaryAddressLine1"]?.ToString(),
                                        TemporaryAddressLine2 = reader["TemporaryAddressLine2"]?.ToString(),
                                        TemporaryPinCode = reader["TemporaryPinCode"]?.ToString(),
                                        TemporaryPlace = reader["TemporaryPlace"]?.ToString(),
                                        TemporaryCountry = reader["TemporaryCountry"]?.ToString(),
                                        TemporaryState = reader["TemporaryState"]?.ToString(),
                                        TemporaryDistrict = reader["TemporaryDistrict"]?.ToString(),
                                        TemporaryCity = reader["TemporaryCity"]?.ToString(),
                                        IsActive = reader["IsActive"]?.ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString()
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Admissions.Add(new tblStudentAddressDetails
                                {
                                    ID = reader["ID"]?.ToString(),
                                    AdmissionID = reader["AdmissionID"]?.ToString(),
                                    PermanentAddressLine1 = reader["PermanentAddressLine1"]?.ToString(),
                                    PermanentAddressLine2 = reader["PermanentAddressLine2"]?.ToString(),
                                    PermanentPinCode = reader["PermanentPinCode"]?.ToString(),
                                    PermanentPlace = reader["PermanentPlace"]?.ToString(),
                                    PermanentCountry = reader["PermanentCountry"]?.ToString(),
                                    PermanentState = reader["PermanentState"]?.ToString(),
                                    PermanentDistrict = reader["PermanentDistrict"]?.ToString(),
                                    PermanentCity = reader["PermanentCity"]?.ToString(),
                                    TemporaryAddressLine1 = reader["TemporaryAddressLine1"]?.ToString(),
                                    TemporaryAddressLine2 = reader["TemporaryAddressLine2"]?.ToString(),
                                    TemporaryPinCode = reader["TemporaryPinCode"]?.ToString(),
                                    TemporaryPlace = reader["TemporaryPlace"]?.ToString(),
                                    TemporaryCountry = reader["TemporaryCountry"]?.ToString(),
                                    TemporaryState = reader["TemporaryState"]?.ToString(),
                                    TemporaryDistrict = reader["TemporaryDistrict"]?.ToString(),
                                    TemporaryCity = reader["TemporaryCity"]?.ToString(),
                                    IsActive = reader["IsActive"]?.ToString(),
                                    CreatedBy = reader["CreatedBy"]?.ToString(),
                                    CreatedIp = reader["CreatedIp"]?.ToString(),
                                    CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                    ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                    ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                    ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                    Status = reader["Message"]?.ToString()
                                });
                            }
                        }
                    }
                }
                return Admissions;
            }
            catch (Exception ex)
            {
                LogException(ex, "SchoolManagementDAL", "Tbl_StudentAddressDetails_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(admission));
                return new List<tblStudentAddressDetails>
                {
                    new tblStudentAddressDetails
                    {
                        Status = $"ERROR: {ex.Message}"
                    }
                };
            }
        }

        public List<tblStudentParentDetails> Tbl_StudentParentDetails_CRUD_Operations(tblStudentParentDetails admission)
        {
            var Admissions = new List<tblStudentParentDetails>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {
                using var conn = new MySqlConnection(_connectionString);
                using var cmd = new MySqlCommand("Proc_Tbl_StudentParentDetails", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Add parameters based on the tblStudentDetails class
                cmd.Parameters.AddWithValue("p_ID", (object?)CleanParam(admission.ID) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_AdmissionID", (object?)CleanParam(admission.AdmissionID) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_FatherName", (object?)CleanParam(admission.FatherName) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_FatherQualification", (object?)CleanParam(admission.FatherQualification) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_FatherOccupation", (object?)CleanParam(admission.FatherOccupation) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_FatherContact", (object?)CleanParam(admission.FatherContact) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_FatherAadhar", (object?)CleanParam(admission.FatherAadhar) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_FatherEmail", (object?)CleanParam(admission.FatherEmail) ?? DBNull.Value);

                cmd.Parameters.AddWithValue("p_MotherName", (object?)CleanParam(admission.MotherName) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_MotherQualification", (object?)CleanParam(admission.MotherQualification) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_MotherOccupation", (object?)CleanParam(admission.MotherOccupation) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_MotherContact", (object?)CleanParam(admission.MotherContact) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_MotherAadhar", (object?)CleanParam(admission.MotherAadhar) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_MotherEmail", (object?)CleanParam(admission.MotherEmail) ?? DBNull.Value);

                cmd.Parameters.AddWithValue("p_IsActive", (object?)CleanParam(admission.IsActive) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(admission.CreatedBy) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_CreatedIp", (object?)CleanParam(admission.CreatedIp) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(admission.ModifiedBy) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModifiedIp", (object?)CleanParam(admission.ModifiedIp) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Flag", (object?)CleanParam(admission.Flag) ?? DBNull.Value);

                cmd.Parameters.AddWithValue("p_Limit", admission.Limit ?? 100);
                cmd.Parameters.AddWithValue("p_LastCreatedDate", admission.LastCreatedDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_LastID", admission.LastID ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_SortColumn", (object?)CleanParam(admission.SortColumn) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_SortDirection", (object?)CleanParam(admission.SortDirection) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Offset", admission.Offset ?? (object)DBNull.Value);

                conn.Open();
                if (admission.Flag != null)
                {
                    if (admission.Flag == "6" || admission.Flag == "8")
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Admissions.Add(new tblStudentParentDetails
                                {
                                    totalcount = reader["totalCount"] != DBNull.Value ? Convert.ToInt32(reader["totalCount"]) : (int?)null
                                });
                            }
                        }
                    }
                    else if (admission.Flag == "2" || admission.Flag == "3" || admission.Flag == "4" || admission.Flag == "7")
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (!reader.HasRows && reader.NextResult()) { }
                            while (reader.Read())
                            {
                                Admissions.Add(new tblStudentParentDetails
                                {
                                    ID = reader["ID"]?.ToString(),
                                    AdmissionID = reader["AdmissionID"]?.ToString(),
                                    FatherName = reader["FatherName"]?.ToString(),
                                    FatherQualification = reader["FatherQualification"]?.ToString(),
                                    FatherOccupation = reader["FatherOccupation"]?.ToString(),
                                    FatherContact = reader["FatherContact"]?.ToString(),
                                    FatherAadhar = reader["FatherAadhar"]?.ToString(),
                                    FatherEmail = reader["FatherEmail"]?.ToString(),
                                    MotherName = reader["MotherName"]?.ToString(),
                                    MotherQualification = reader["MotherQualification"]?.ToString(),
                                    MotherOccupation = reader["MotherOccupation"]?.ToString(),
                                    MotherContact = reader["MotherContact"]?.ToString(),
                                    MotherAadhar = reader["MotherAadhar"]?.ToString(),
                                    MotherEmail = reader["MotherEmail"]?.ToString(),
                                    IsActive = reader["IsActive"]?.ToString(),
                                    CreatedBy = reader["CreatedBy"]?.ToString(),
                                    CreatedIp = reader["CreatedIp"]?.ToString(),
                                    CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                    ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                    ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                    ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                    Status = reader["Message"]?.ToString()
                                });
                            }
                        }
                    }
                    else if (admission.Flag == "1" || admission.Flag == "5")
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (reader["Message"]?.ToString() == "Parent details for this AdmissionID already exists")
                                {
                                    Admissions.Add(new tblStudentParentDetails
                                    {
                                        Status = reader["Message"]?.ToString()
                                    });
                                }
                                else
                                {
                                    Admissions.Add(new tblStudentParentDetails
                                    {
                                        ID = reader["ID"]?.ToString(),
                                        AdmissionID = reader["AdmissionID"]?.ToString(),
                                        FatherName = reader["FatherName"]?.ToString(),
                                        FatherQualification = reader["FatherQualification"]?.ToString(),
                                        FatherOccupation = reader["FatherOccupation"]?.ToString(),
                                        FatherContact = reader["FatherContact"]?.ToString(),
                                        FatherAadhar = reader["FatherAadhar"]?.ToString(),
                                        FatherEmail = reader["FatherEmail"]?.ToString(),
                                        MotherName = reader["MotherName"]?.ToString(),
                                        MotherQualification = reader["MotherQualification"]?.ToString(),
                                        MotherOccupation = reader["MotherOccupation"]?.ToString(),
                                        MotherContact = reader["MotherContact"]?.ToString(),
                                        MotherAadhar = reader["MotherAadhar"]?.ToString(),
                                        MotherEmail = reader["MotherEmail"]?.ToString(),
                                        IsActive = reader["IsActive"]?.ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString()
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Admissions.Add(new tblStudentParentDetails
                                {
                                    ID = reader["ID"]?.ToString(),
                                    AdmissionID = reader["AdmissionID"]?.ToString(),
                                    FatherName = reader["FatherName"]?.ToString(),
                                    FatherQualification = reader["FatherQualification"]?.ToString(),
                                    FatherOccupation = reader["FatherOccupation"]?.ToString(),
                                    FatherContact = reader["FatherContact"]?.ToString(),
                                    FatherAadhar = reader["FatherAadhar"]?.ToString(),
                                    FatherEmail = reader["FatherEmail"]?.ToString(),
                                    MotherName = reader["MotherName"]?.ToString(),
                                    MotherQualification = reader["MotherQualification"]?.ToString(),
                                    MotherOccupation = reader["MotherOccupation"]?.ToString(),
                                    MotherContact = reader["MotherContact"]?.ToString(),
                                    MotherAadhar = reader["MotherAadhar"]?.ToString(),
                                    MotherEmail = reader["MotherEmail"]?.ToString(),
                                    IsActive = reader["IsActive"]?.ToString(),
                                    CreatedBy = reader["CreatedBy"]?.ToString(),
                                    CreatedIp = reader["CreatedIp"]?.ToString(),
                                    CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                    ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                    ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                    ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                    Status = reader["Message"]?.ToString()
                                });
                            }
                        }
                    }
                }
                return Admissions;
            }
            catch (Exception ex)
            {
                LogException(ex, "SchoolManagementDAL", "Tbl_StudentParentDetails_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(admission));
                return new List<tblStudentParentDetails>
                {
                    new tblStudentParentDetails
                    {
                        Status = $"ERROR: {ex.Message}"
                    }
                };
            }
        }

        public List<tblStudentAcademicDetails> Tbl_StudentAcademicDetails_CRUD_Operations(tblStudentAcademicDetails admission)
        {
            var Admissions = new List<tblStudentAcademicDetails>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {
                using var conn = new MySqlConnection(_connectionString);
                using var cmd = new MySqlCommand("Proc_Tbl_StudentAcademicDetails", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Add parameters based on the tblStudentDetails class
                cmd.Parameters.AddWithValue("p_ID", (object?)CleanParam(admission.ID) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_AdmissionID", (object?)CleanParam(admission.AdmissionID) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Syllabus", (object?)CleanParam(admission.Syllabus) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Class", (object?)CleanParam(admission.Class) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Division", (object?)CleanParam(admission.Division) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_JoinDate", admission.JoinDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_School", (object?)CleanParam(admission.SchoolID) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_AcademicYear", (object?)CleanParam(admission.AcademicYear) ?? DBNull.Value);

                cmd.Parameters.AddWithValue("p_IsActive", (object?)CleanParam(admission.IsActive) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(admission.CreatedBy) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_CreatedIp", (object?)CleanParam(admission.CreatedIp) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(admission.ModifiedBy) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModifiedIp", (object?)CleanParam(admission.ModifiedIp) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Flag", (object?)CleanParam(admission.Flag) ?? DBNull.Value);

                cmd.Parameters.AddWithValue("p_Limit", admission.Limit ?? 100);
                cmd.Parameters.AddWithValue("p_LastCreatedDate", admission.LastCreatedDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_LastID", admission.LastID ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_SortColumn", (object?)CleanParam(admission.SortColumn) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_SortDirection", (object?)CleanParam(admission.SortDirection) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Offset", admission.Offset ?? (object)DBNull.Value);

                conn.Open();
                if (admission.Flag != null)
                {
                    if (admission.Flag == "6" || admission.Flag == "8")
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Admissions.Add(new tblStudentAcademicDetails
                                {
                                    totalcount = reader["totalCount"] != DBNull.Value ? Convert.ToInt32(reader["totalCount"]) : (int?)null
                                });
                            }
                        }
                    }
                    else if (admission.Flag == "2" || admission.Flag == "3" || admission.Flag == "4" || admission.Flag == "7")
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (!reader.HasRows && reader.NextResult()) { }
                            while (reader.Read())
                            {
                                Admissions.Add(new tblStudentAcademicDetails
                                {
                                    ID = reader["ID"]?.ToString(),
                                    AdmissionID = reader["AdmissionID"]?.ToString(),
                                    Syllabus = reader["Syllabus"]?.ToString(),
                                    Class = reader["Class"]?.ToString(),
                                    Division = reader["Division"]?.ToString(),
                                    JoinDate = reader["JoinDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["JoinDate"]),
                                    SchoolID = reader["School"]?.ToString(),
                                    AcademicYear = reader["AcademicYear"]?.ToString(),
                                    IsActive = reader["IsActive"]?.ToString(),
                                    CreatedBy = reader["CreatedBy"]?.ToString(),
                                    CreatedIp = reader["CreatedIp"]?.ToString(),
                                    CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                    ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                    ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                    ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                    Status = reader["Message"]?.ToString()
                                });
                            }
                        }
                    }
                    else if (admission.Flag == "1" || admission.Flag == "5")
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (reader["Message"]?.ToString() == "AdmissionNo already exists")
                                {
                                    Admissions.Add(new tblStudentAcademicDetails
                                    {
                                        Status = reader["Message"]?.ToString()
                                    });
                                }
                                else
                                {
                                    Admissions.Add(new tblStudentAcademicDetails
                                    {
                                        ID = reader["ID"]?.ToString(),
                                        AdmissionID = reader["AdmissionID"]?.ToString(),
                                        Syllabus = reader["Syllabus"]?.ToString(),
                                        Class = reader["Class"]?.ToString(),
                                        Division = reader["Division"]?.ToString(),
                                        JoinDate = reader["JoinDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["JoinDate"]),
                                        SchoolID = reader["School"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        IsActive = reader["IsActive"]?.ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString()
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Admissions.Add(new tblStudentAcademicDetails
                                {
                                    ID = reader["ID"]?.ToString(),
                                    AdmissionID = reader["AdmissionID"]?.ToString(),
                                    Syllabus = reader["Syllabus"]?.ToString(),
                                    Class = reader["Class"]?.ToString(),
                                    Division = reader["Division"]?.ToString(),
                                    JoinDate = reader["JoinDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["JoinDate"]),
                                    SchoolID = reader["School"]?.ToString(),
                                    AcademicYear = reader["AcademicYear"]?.ToString(),
                                    IsActive = reader["IsActive"]?.ToString(),
                                    CreatedBy = reader["CreatedBy"]?.ToString(),
                                    CreatedIp = reader["CreatedIp"]?.ToString(),
                                    CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                    ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                    ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                    ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                    Status = reader["Message"]?.ToString()
                                });
                            }
                        }
                    }
                }
                return Admissions;
            }
            catch (Exception ex)
            {
                LogException(ex, "SchoolManagementDAL", "Tbl_StudentAcademicDetails_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(admission));
                return new List<tblStudentAcademicDetails>
                {
                    new tblStudentAcademicDetails
                    {
                        Status = $"ERROR: {ex.Message}"
                    }
                };
            }
        }

        public List<tblStudentTransportationDetails> Tbl_StudentTransportationDetails_CRUD_Operations(tblStudentTransportationDetails admission)
        {
            var Admissions = new List<tblStudentTransportationDetails>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {
                using var conn = new MySqlConnection(_connectionString);
                using var cmd = new MySqlCommand("Proc_Tbl_StudentTransportationDetails", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Add parameters based on the tblStudentDetails class
                cmd.Parameters.AddWithValue("p_ID", (object?)CleanParam(admission.ID) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_AdmissionID", (object?)CleanParam(admission.AdmissionID) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Route", (object?)CleanParam(admission.Route) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Stop", (object?)CleanParam(admission.Stop) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Bus", (object?)CleanParam(admission.Bus) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Fare", (object?)CleanParam(admission.Fare) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_StartDate", admission.StartDate ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("p_IsActive", (object?)CleanParam(admission.IsActive) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(admission.CreatedBy) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_CreatedIp", (object?)CleanParam(admission.CreatedIp) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(admission.ModifiedBy) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModifiedIp", (object?)CleanParam(admission.ModifiedIp) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Flag", (object?)CleanParam(admission.Flag) ?? DBNull.Value);

                cmd.Parameters.AddWithValue("p_Limit", admission.Limit ?? 100);
                cmd.Parameters.AddWithValue("p_LastCreatedDate", admission.LastCreatedDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_LastID", admission.LastID ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_SortColumn", (object?)CleanParam(admission.SortColumn) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_SortDirection", (object?)CleanParam(admission.SortDirection) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Offset", admission.Offset ?? (object)DBNull.Value);

                conn.Open();
                if (admission.Flag != null)
                {
                    if (admission.Flag == "6" || admission.Flag == "8")
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Admissions.Add(new tblStudentTransportationDetails
                                {
                                    totalcount = reader["totalCount"] != DBNull.Value ? Convert.ToInt32(reader["totalCount"]) : (int?)null
                                });
                            }
                        }
                    }
                    else if (admission.Flag == "2" || admission.Flag == "3" || admission.Flag == "7")
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (!reader.HasRows && reader.NextResult()) { }
                            while (reader.Read())
                            {
                                Admissions.Add(new tblStudentTransportationDetails
                                {
                                    ID = reader["ID"]?.ToString(),
                                    AdmissionID = reader["AdmissionID"]?.ToString(),
                                    Route = reader["Route"]?.ToString(),
                                    Stop = reader["Stop"]?.ToString(),
                                    Bus = reader["Bus"]?.ToString(),
                                    Fare = reader["Fare"]?.ToString(),
                                    StartDate = reader["StartDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["StartDate"]),
                                    IsActive = reader["IsActive"]?.ToString(),
                                    CreatedBy = reader["CreatedBy"]?.ToString(),
                                    CreatedIp = reader["CreatedIp"]?.ToString(),
                                    CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                    ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                    ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                    ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                    Status = reader["Message"]?.ToString()
                                });
                            }
                        }
                    }
                    else if (admission.Flag == "4")
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (!reader.HasRows && reader.NextResult()) { }
                            while (reader.Read())
                            {
                                Admissions.Add(new tblStudentTransportationDetails
                                {
                                    ID = reader["ID"]?.ToString(),
                                    AdmissionID = reader["AdmissionID"]?.ToString(),
                                    Route = reader["Route"]?.ToString(),
                                    Stop = reader["Stop"]?.ToString(),
                                    Bus = reader["Bus"]?.ToString(),
                                    Fare = reader["Fare"]?.ToString(),
                                    StartDate = reader["StartDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["StartDate"]),
                                    IsActive = reader["IsActive"]?.ToString(),
                                    CreatedBy = reader["CreatedBy"]?.ToString(),
                                    CreatedIp = reader["CreatedIp"]?.ToString(),
                                    CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                    ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                    ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                    ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                    Status = reader["Message"]?.ToString(),
                                    RouteName = reader["RouteName"]?.ToString(),
                                    StopName = reader["StopName"]?.ToString(),
                                    BusName = reader["BusName"]?.ToString(),
                                    FareName = reader["FareName"]?.ToString(),
                                });
                            }
                        }
                    }
                    else if (admission.Flag == "1" || admission.Flag == "5")
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (reader["Message"]?.ToString() == "AdmissionNo already exists")
                                {
                                    Admissions.Add(new tblStudentTransportationDetails
                                    {
                                        Status = reader["Message"]?.ToString()
                                    });
                                }
                                else
                                {
                                    Admissions.Add(new tblStudentTransportationDetails
                                    {
                                        ID = reader["ID"]?.ToString(),
                                        AdmissionID = reader["AdmissionID"]?.ToString(),
                                        Route = reader["Route"]?.ToString(),
                                        Stop = reader["Stop"]?.ToString(),
                                        Bus = reader["Bus"]?.ToString(),
                                        Fare = reader["Fare"]?.ToString(),
                                        StartDate = reader["StartDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["StartDate"]),
                                        IsActive = reader["IsActive"]?.ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString()
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Admissions.Add(new tblStudentTransportationDetails
                                {
                                    ID = reader["ID"]?.ToString(),
                                    AdmissionID = reader["AdmissionID"]?.ToString(),
                                    Route = reader["Route"]?.ToString(),
                                    Stop = reader["Stop"]?.ToString(),
                                    Bus = reader["Bus"]?.ToString(),
                                    Fare = reader["Fare"]?.ToString(),
                                    StartDate = reader["StartDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["StartDate"]),
                                    IsActive = reader["IsActive"]?.ToString(),
                                    CreatedBy = reader["CreatedBy"]?.ToString(),
                                    CreatedIp = reader["CreatedIp"]?.ToString(),
                                    CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                    ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                    ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                    ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                    Status = reader["Message"]?.ToString()
                                });
                            }
                        }
                    }
                }
                return Admissions;
            }
            catch (Exception ex)
            {
                LogException(ex, "SchoolManagementDAL", "Tbl_StudentTransportationDetails_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(admission));
                return new List<tblStudentTransportationDetails>
                {
                    new tblStudentTransportationDetails
                    {
                        Status = $"ERROR: {ex.Message}"
                    }
                };
            }
        }

        public List<tblAllotClassTeacher> Tbl_AllotClassTeacher_CRUD_Operations(tblAllotClassTeacher ClassTeacher)
        {
            var ClassTeachers = new List<tblAllotClassTeacher>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                using (var cmd = new MySqlCommand("Proc_AllotClassTeacher", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("p_ID", (object?)CleanParam(ClassTeacher.ID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SchoolID", (object?)CleanParam(ClassTeacher.SchoolID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_AcademicYear", (object?)CleanParam(ClassTeacher.AcademicYear) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Class", (object?)CleanParam(ClassTeacher.Class) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Division", (object?)CleanParam(ClassTeacher.Division) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ClassTeacher", (object?)CleanParam(ClassTeacher.ClassTeacher) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_IsActive", (object?)CleanParam(ClassTeacher.IsActive) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(ClassTeacher.CreatedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedIP", (object?)CleanParam(ClassTeacher.CreatedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(ClassTeacher.ModifiedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedIP", (object?)CleanParam(ClassTeacher.ModifiedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_flag", (object?)CleanParam(ClassTeacher.Flag) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Limit", ClassTeacher.Limit ?? 100);
                    cmd.Parameters.AddWithValue("p_LastCreatedDate", ClassTeacher.LastCreatedDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastID", ClassTeacher.LastID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortColumn", (object?)CleanParam(ClassTeacher.SortColumn) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortDirection", (object?)CleanParam(ClassTeacher.SortDirection) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Offset", ClassTeacher.Offset ?? (object)DBNull.Value);

                    conn.Open();

                    if (ClassTeacher.Flag != null)
                    {
                        if (ClassTeacher.Flag == "6" || ClassTeacher.Flag == "8")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var ClassDivisionTeacher = new tblAllotClassTeacher
                                    {
                                        totalcount = reader["totalCount"] != DBNull.Value ? Convert.ToInt32(reader["totalCount"]) : (int?)null
                                    };

                                    ClassTeachers.Add(ClassDivisionTeacher);
                                }
                            }
                        }
                        else if (ClassTeacher.Flag == "2" || ClassTeacher.Flag == "3" || ClassTeacher.Flag == "4" || ClassTeacher.Flag == "7")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var ClassDivisionTeacher = new tblAllotClassTeacher
                                    {
                                        //ID = reader["ID"] == DBNull.Value ? null : Convert.ToInt32(reader["ID"]),
                                        ID = reader["ID"]?.ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        Class = reader["Class"]?.ToString(),
                                        Division = reader["Division"]?.ToString(),
                                        ClassTeacher = reader["ClassTeacher"]?.ToString(),
                                        IsActive = reader["IsActive"]?.ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString(),
                                        SchoolName = reader["SchoolName"]?.ToString(),
                                        AcademicYearName = reader["AcademicYearName"]?.ToString(),
                                        ClassName = reader["ClassName"]?.ToString(),
                                        StaffName = reader["StaffName"]?.ToString(),
                                        DivisionName = reader["DivisionName"]?.ToString()
                                    };

                                    ClassTeachers.Add(ClassDivisionTeacher);
                                }
                            }
                        }
                        else if (ClassTeacher.Flag == "1" || ClassTeacher.Flag == "5")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (reader["Message"]?.ToString() == "Class Teacher already Allocated")
                                    {
                                        ClassTeachers.Add(new tblAllotClassTeacher
                                        {
                                            Status = reader["Message"]?.ToString()
                                        });
                                    }
                                    else
                                    {
                                        ClassTeachers.Add(new tblAllotClassTeacher
                                        {
                                            ID = reader["ID"]?.ToString(),
                                            SchoolID = reader["SchoolID"]?.ToString(),
                                            AcademicYear = reader["AcademicYear"]?.ToString(),
                                            Class = reader["Class"]?.ToString(),
                                            Division = reader["Division"]?.ToString(),
                                            ClassTeacher = reader["ClassTeacher"]?.ToString(),
                                            IsActive = reader["IsActive"]?.ToString(),
                                            CreatedBy = reader["CreatedBy"]?.ToString(),
                                            CreatedIp = reader["CreatedIp"]?.ToString(),
                                            CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                            ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                            ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                            ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                            Status = reader["Message"]?.ToString()
                                        });
                                    }
                                }
                            }
                        }
                        else
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var ClassDivisionTeacher = new tblAllotClassTeacher
                                    {
                                        //ID = reader["ID"] == DBNull.Value ? null : Convert.ToInt32(reader["ID"]),
                                        ID = reader["ID"]?.ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        Class = reader["Class"]?.ToString(),
                                        Division = reader["Division"]?.ToString(),
                                        ClassTeacher = reader["ClassTeacher"]?.ToString(),
                                        IsActive = reader["IsActive"]?.ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString(),
                                    };

                                    ClassTeachers.Add(ClassDivisionTeacher);
                                }
                            }
                        }
                    }
                }

                return ClassTeachers;
            }
            catch (Exception ex)
            {
                LogException(ex, "SchoolManagementDAL", "Tbl_AllotClassTeacher_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(ClassTeacher));
                return new List<tblAllotClassTeacher>
                {
                    new tblAllotClassTeacher
                    {
                        Status = $"ERROR: {ex.Message}"
                    }
                };
            }
        }

        //Transportation Module
        public List<tblBus> Tbl_bus_CRUD_Operations(tblBus bus)
        {
            var Buses = new List<tblBus>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                using (var cmd = new MySqlCommand("Proc_Bus", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("p_SchoolID", (object?)CleanParam(bus.SchoolID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_AcademicYear", (object?)CleanParam(bus.AcademicYear) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ID", (object?)CleanParam(bus.ID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Name", (object?)CleanParam(bus.Name) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_RegNo", (object?)CleanParam(bus.RegNo) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Driver", (object?)CleanParam(bus.Driver) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_AssistantName", (object?)CleanParam(bus.AssistantName) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_AssistantMobNo", (object?)CleanParam(bus.AssistantMobNo) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_OtherDetails", (object?)CleanParam(bus.OtherDetails) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_MorningStartTime", bus.MorningStartTime);
                    cmd.Parameters.AddWithValue("p_EveningStartTime", bus.EveningStartTime);
                    cmd.Parameters.AddWithValue("p_DistanceCostPerKM", (object?)CleanParam(bus.DistanceCostPerKM) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_MaxCapacity", bus.MaxCapacity ?? 100);
                    cmd.Parameters.AddWithValue("p_Description", (object?)CleanParam(bus.Description) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_IsActive", (object?)CleanParam(bus.IsActive) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(bus.CreatedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedIP", (object?)CleanParam(bus.CreatedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(bus.ModifiedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedIP", (object?)CleanParam(bus.ModifiedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Flag", bus.Flag ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Limit", bus.Limit ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastCreatedDate", bus.LastCreatedDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastID", bus.LastID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortColumn", bus.SortColumn ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortDirection", bus.SortDirection ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Offset", bus.Offset ?? (object)DBNull.Value);

                    conn.Open();

                    if (bus.Flag != null)
                    {
                        if (bus.Flag == "6" || bus.Flag == "8")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var buss = new tblBus
                                    {
                                        totalcount = reader["totalCount"] != DBNull.Value ? Convert.ToInt32(reader["totalCount"]) : (int?)null
                                    };

                                    Buses.Add(buss);
                                }
                            }
                        }
                        else if (bus.Flag == "2" || bus.Flag == "3" || bus.Flag == "4" || bus.Flag == "7")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var buss = new tblBus
                                    {
                                        //ID = reader["ID"] == DBNull.Value ? null : Convert.ToInt32(reader["ID"]),
                                        ID = reader["ID"].ToString(),
                                        Name = reader["Name"]?.ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        RegNo = reader["RegNo"]?.ToString(),
                                        Driver = reader["Driver"]?.ToString(),
                                        AssistantName = reader["AssistantName"]?.ToString(),
                                        AssistantMobNo = reader["AssistantMobNo"]?.ToString(),
                                        OtherDetails = reader["OtherDetails"]?.ToString(),
                                        MorningStartTime = reader["MorningStartTime"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)reader["MorningStartTime"],
                                        EveningStartTime = reader["EveningStartTime"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)reader["EveningStartTime"],
                                        DistanceCostPerKM = reader["DistanceCostPerKM"]?.ToString(),
                                        MaxCapacity = reader["MaxCapacity"] == DBNull.Value ? null : Convert.ToInt32(reader["MaxCapacity"]),
                                        Description = reader["Description"]?.ToString(),
                                        IsActive = reader["IsActive"].ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString(),
                                        SchoolName = reader["SchoolName"]?.ToString(),
                                        AcademicYearName = reader["AcademicYearName"]?.ToString()
                                    };

                                    Buses.Add(buss);
                                }
                            }
                        }
                        else if (bus.Flag == "1" || bus.Flag == "5")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (reader["Message"]?.ToString() == "Bus name already exists")
                                    {
                                        var buss = new tblBus
                                        {
                                            Status = reader["Message"]?.ToString(),
                                        };

                                        Buses.Add(buss);
                                    }
                                    else
                                    {
                                        var buss = new tblBus
                                        {
                                            ID = reader["ID"].ToString(),
                                            Name = reader["Name"]?.ToString(),
                                            SchoolID = reader["SchoolID"]?.ToString(),
                                            AcademicYear = reader["AcademicYear"]?.ToString(),
                                            RegNo = reader["RegNo"]?.ToString(),
                                            Driver = reader["Driver"]?.ToString(),
                                            AssistantName = reader["AssistantName"]?.ToString(),
                                            AssistantMobNo = reader["AssistantMobNo"]?.ToString(),
                                            OtherDetails = reader["OtherDetails"]?.ToString(),
                                            MorningStartTime = reader["MorningStartTime"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)reader["MorningStartTime"],
                                            EveningStartTime = reader["EveningStartTime"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)reader["EveningStartTime"],
                                            DistanceCostPerKM = reader["DistanceCostPerKM"]?.ToString(),
                                            MaxCapacity = reader["MaxCapacity"] == DBNull.Value ? null : Convert.ToInt32(reader["MaxCapacity"]),
                                            Description = reader["Description"]?.ToString(),
                                            IsActive = reader["IsActive"].ToString(),
                                            CreatedBy = reader["CreatedBy"]?.ToString(),
                                            CreatedIp = reader["CreatedIp"]?.ToString(),
                                            CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                            ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                            ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                            ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                            Status = reader["Message"]?.ToString()
                                        };

                                        Buses.Add(buss);
                                    }
                                }
                            }
                        }
                        else
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var buss = new tblBus
                                    {
                                        ID = reader["ID"].ToString(),
                                        Name = reader["Name"]?.ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        RegNo = reader["RegNo"]?.ToString(),
                                        Driver = reader["Driver"]?.ToString(),
                                        AssistantName = reader["AssistantName"]?.ToString(),
                                        AssistantMobNo = reader["AssistantMobNo"]?.ToString(),
                                        OtherDetails = reader["OtherDetails"]?.ToString(),
                                        MorningStartTime = reader["MorningStartTime"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)reader["MorningStartTime"],
                                        EveningStartTime = reader["EveningStartTime"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)reader["EveningStartTime"],
                                        DistanceCostPerKM = reader["DistanceCostPerKM"]?.ToString(),
                                        MaxCapacity = reader["MaxCapacity"] == DBNull.Value ? null : Convert.ToInt32(reader["MaxCapacity"]),
                                        Description = reader["Description"]?.ToString(),
                                        IsActive = reader["IsActive"].ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString()
                                    };

                                    Buses.Add(buss);
                                }
                            }
                        }
                    }
                }

                return Buses;
            }
            catch (Exception ex)
            {
                LogException(ex, "SchoolManagementDAL", "Tbl_bus_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(bus));
                return new List<tblBus>
                {
                    new tblBus
                    {
                        Status = $"ERROR: {ex.Message}"
                    }
                };
            }
        }

        public List<tblRoute> Tbl_route_CRUD_Operations(tblRoute route)
        {
            var Routes = new List<tblRoute>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                using (var cmd = new MySqlCommand("Proc_Route", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("p_SchoolID", (object?)CleanParam(route.SchoolID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_AcademicYear", (object?)CleanParam(route.AcademicYear) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ID", (object?)CleanParam(route.ID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Name", (object?)CleanParam(route.Name) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Distance", (object?)CleanParam(route.Distance) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Description", (object?)CleanParam(route.Description) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_IsActive", (object?)CleanParam(route.IsActive) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(route.CreatedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedIP", (object?)CleanParam(route.CreatedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(route.ModifiedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedIP", (object?)CleanParam(route.ModifiedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Flag", route.Flag ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Limit", route.Limit ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastCreatedDate", route.LastCreatedDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastID", route.LastID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortColumn", route.SortColumn ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortDirection", route.SortDirection ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Offset", route.Offset ?? (object)DBNull.Value);

                    conn.Open();

                    if (route.Flag != null)
                    {
                        if (route.Flag == "6" || route.Flag == "8")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var routess = new tblRoute
                                    {
                                        totalcount = reader["totalCount"] != DBNull.Value ? Convert.ToInt32(reader["totalCount"]) : (int?)null
                                    };

                                    Routes.Add(routess);
                                }
                            }
                        }
                        else if (route.Flag == "2" || route.Flag == "3" || route.Flag == "4" || route.Flag == "7")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var routess = new tblRoute
                                    {
                                        //ID = reader["ID"] == DBNull.Value ? null : Convert.ToInt32(reader["ID"]),
                                        ID = reader["ID"].ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        Name = reader["Name"]?.ToString(),
                                        Distance = reader["Distance"]?.ToString(),
                                        Description = reader["Description"]?.ToString(),
                                        IsActive = reader["IsActive"].ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString(),
                                        SchoolName = reader["SchoolName"]?.ToString(),
                                        AcademicYearName = reader["AcademicYearName"]?.ToString()
                                    };

                                    Routes.Add(routess);
                                }
                            }
                        }
                        else if (route.Flag == "1" || route.Flag == "5")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (reader["Message"]?.ToString() == "Route name already exists")
                                    {
                                        var routess = new tblRoute
                                        {
                                            Status = reader["Message"]?.ToString(),
                                        };

                                        Routes.Add(routess);
                                    }
                                    else
                                    {
                                        var routess = new tblRoute
                                        {
                                            ID = reader["ID"].ToString(),
                                            SchoolID = reader["SchoolID"]?.ToString(),
                                            AcademicYear = reader["AcademicYear"]?.ToString(),
                                            Name = reader["Name"]?.ToString(),
                                            Distance = reader["Distance"]?.ToString(),
                                            Description = reader["Description"]?.ToString(),
                                            IsActive = reader["IsActive"].ToString(),
                                            CreatedBy = reader["CreatedBy"]?.ToString(),
                                            CreatedIp = reader["CreatedIp"]?.ToString(),
                                            CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                            ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                            ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                            ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                            Status = reader["Message"]?.ToString()
                                        };

                                        Routes.Add(routess);
                                    }
                                }
                            }
                        }
                        else
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var routess = new tblRoute
                                    {
                                        ID = reader["ID"].ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        Name = reader["Name"]?.ToString(),
                                        Distance = reader["Distance"]?.ToString(),
                                        Description = reader["Description"]?.ToString(),
                                        IsActive = reader["IsActive"].ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString()
                                    };

                                    Routes.Add(routess);
                                }
                            }
                        }
                    }
                }

                return Routes;
            }
            catch (Exception ex)
            {
                LogException(ex, "SchoolManagementDAL", "Tbl_route_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(route));
                return new List<tblRoute>
                {
                    new tblRoute
                    {
                        Status = $"ERROR: {ex.Message}"
                    }
                };
            }
        }

        public List<tblStops> Tbl_stops_CRUD_Operations(tblStops stop)
        {
            var Stops = new List<tblStops>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                using (var cmd = new MySqlCommand("Proc_Stops", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_ID", (object?)CleanParam(stop.ID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SchoolID", (object?)CleanParam(stop.SchoolID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_AcademicYear", (object?)CleanParam(stop.AcademicYear) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Route", (object?)CleanParam(stop.Route) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_StopOrder", (object?)CleanParam(stop.StopOrder) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_StopName", (object?)CleanParam(stop.StopName) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Distance", (object?)CleanParam(stop.Distance) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Description", (object?)CleanParam(stop.Description) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_IsActive", (object?)CleanParam(stop.IsActive) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(stop.CreatedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedIP", (object?)CleanParam(stop.CreatedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(stop.ModifiedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedIP", (object?)CleanParam(stop.ModifiedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Flag", stop.Flag ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Limit", stop.Limit ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastCreatedDate", stop.LastCreatedDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastID", stop.LastID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortColumn", stop.SortColumn ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortDirection", stop.SortDirection ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Offset", stop.Offset ?? (object)DBNull.Value);

                    conn.Open();

                    if (stop.Flag != null)
                    {
                        if (stop.Flag == "6" || stop.Flag == "8")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var stopss = new tblStops
                                    {
                                        totalcount = reader["totalCount"] != DBNull.Value ? Convert.ToInt32(reader["totalCount"]) : (int?)null
                                    };

                                    Stops.Add(stopss);
                                }
                            }
                        }
                        else if (stop.Flag == "2" || stop.Flag == "3" || stop.Flag == "7" || stop.Flag == "4")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var stopss = new tblStops
                                    {
                                        //ID = reader["ID"] == DBNull.Value ? null : Convert.ToInt32(reader["ID"]),
                                        ID = reader["ID"].ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        Route = reader["Route"]?.ToString(),
                                        StopOrder = reader["StopOrder"]?.ToString(),
                                        StopName = reader["StopName"]?.ToString(),
                                        Distance = reader["Distance"]?.ToString(),
                                        Description = reader["Description"]?.ToString(),
                                        IsActive = reader["IsActive"].ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString(),
                                        SchoolName = reader["SchoolName"]?.ToString(),
                                        AcademicYearName = reader["AcademicYearName"]?.ToString(),
                                        RouteName = reader["RouteName"]?.ToString()
                                    };

                                    Stops.Add(stopss);
                                }
                            }
                        }
                        else if (stop.Flag == "1" || stop.Flag == "5")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (reader["Message"]?.ToString() == "Stop already exists")
                                    {
                                        var stopss = new tblStops
                                        {
                                            Status = reader["Message"]?.ToString(),
                                        };

                                        Stops.Add(stopss);
                                    }
                                    else
                                    {
                                        var stopss = new tblStops
                                        {
                                            ID = reader["ID"].ToString(),
                                            SchoolID = reader["SchoolID"]?.ToString(),
                                            AcademicYear = reader["AcademicYear"]?.ToString(),
                                            Route = reader["Route"]?.ToString(),
                                            StopOrder = reader["StopOrder"]?.ToString(),
                                            StopName = reader["StopName"]?.ToString(),
                                            Distance = reader["Distance"]?.ToString(),
                                            Description = reader["Description"]?.ToString(),
                                            IsActive = reader["IsActive"].ToString(),
                                            CreatedBy = reader["CreatedBy"]?.ToString(),
                                            CreatedIp = reader["CreatedIp"]?.ToString(),
                                            CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                            ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                            ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                            ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                            Status = reader["Message"]?.ToString()
                                        };

                                        Stops.Add(stopss);
                                    }
                                }
                            }
                        }
                        else
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var stopss = new tblStops
                                    {
                                        ID = reader["ID"].ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        Route = reader["Route"]?.ToString(),
                                        StopOrder = reader["StopOrder"]?.ToString(),
                                        StopName = reader["StopName"]?.ToString(),
                                        Distance = reader["Distance"]?.ToString(),
                                        Description = reader["Description"]?.ToString(),
                                        IsActive = reader["IsActive"].ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString()
                                    };

                                    Stops.Add(stopss);
                                }
                            }
                        }
                    }
                }

                return Stops;
            }
            catch (Exception ex)
            {
                LogException(ex, "SchoolManagementDAL", "Tbl_stops_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(stop));
                return new List<tblStops>
                {
                    new tblStops
                    {
                        Status = $"ERROR: {ex.Message}"
                    }
                };
            }
        }

        public List<tblFare> Tbl_fare_CRUD_Operations(tblFare fare)
        {
            var Fare = new List<tblFare>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                using (var cmd = new MySqlCommand("Proc_Fare", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_ID", (object?)CleanParam(fare.ID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SchoolID", (object?)CleanParam(fare.SchoolID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_AcademicYear", (object?)CleanParam(fare.AcademicYear) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_RouteID", (object?)CleanParam(fare.RouteID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_StopID", (object?)CleanParam(fare.StopID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_BusID", (object?)CleanParam(fare.BusID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Amount", fare.Amount ?? (object)DBNull.Value);  // now safe if Amount is decimal?                    //cmd.Parameters.AddWithValue("p_Description", (object?)CleanParam(fare.Description) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_IsActive", (object?)CleanParam(fare.IsActive) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(fare.CreatedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedIP", (object?)CleanParam(fare.CreatedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(fare.ModifiedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedIP", (object?)CleanParam(fare.ModifiedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Flag", fare.Flag ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Limit", fare.Limit ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastCreatedDate", fare.LastCreatedDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastID", fare.LastID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortColumn", fare.SortColumn ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortDirection", fare.SortDirection ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Offset", fare.Offset ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_StopName", (object?)CleanParam(fare.StopName) ?? DBNull.Value);

                    conn.Open();

                    if (fare.Flag != null)
                    {
                        if (fare.Flag == "6" || fare.Flag == "8")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var fares = new tblFare
                                    {
                                        totalcount = reader["totalCount"] != DBNull.Value ? Convert.ToInt32(reader["totalCount"]) : (int?)null
                                    };

                                    Fare.Add(fares);
                                }
                            }
                        }
                        else if (fare.Flag == "2" || fare.Flag == "3" || fare.Flag == "7" || fare.Flag == "4" || fare.Flag == "9" || fare.Flag == "10")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var Fares = new tblFare
                                    {
                                        //ID = reader["ID"] == DBNull.Value ? null : Convert.ToInt32(reader["ID"]),
                                        ID = reader["ID"].ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        RouteID = reader["RouteID"]?.ToString(),
                                        StopID = reader["StopID"]?.ToString(),
                                        BusID = reader["BusID"]?.ToString(),
                                        Amount = reader["Amount"] == DBNull.Value ? null : Convert.ToDecimal(reader["Amount"]),
                                        //Description = reader["Description"]?.ToString(),
                                        IsActive = reader["IsActive"].ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString(),
                                        SchoolName = reader["SchoolName"]?.ToString(),
                                        AcademicYearName = reader["AcademicYearName"]?.ToString(),
                                        RouteName = reader["RouteName"]?.ToString(),
                                        StopName = reader["StopName"]?.ToString(),
                                        BusName = reader["BusName"]?.ToString()
                                    };

                                    Fare.Add(Fares);
                                }
                            }
                        }
                        else if (fare.Flag == "1" || fare.Flag == "5")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (reader["Message"]?.ToString() == "Fare already exists for this stop, bus, and route")
                                    {
                                        var Fares = new tblFare
                                        {
                                            Status = reader["Message"]?.ToString(),
                                        };

                                        Fare.Add(Fares);
                                    }
                                    else
                                    {
                                        var Fares = new tblFare
                                        {
                                            ID = reader["ID"].ToString(),
                                            SchoolID = reader["SchoolID"]?.ToString(),
                                            AcademicYear = reader["AcademicYear"]?.ToString(),
                                            RouteID = reader["RouteID"]?.ToString(),
                                            StopID = reader["StopID"]?.ToString(),
                                            BusID = reader["BusID"]?.ToString(),
                                            Amount = reader["Amount"] == DBNull.Value ? null : Convert.ToDecimal(reader["Amount"]),
                                            IsActive = reader["IsActive"].ToString(),
                                            CreatedBy = reader["CreatedBy"]?.ToString(),
                                            CreatedIp = reader["CreatedIp"]?.ToString(),
                                            CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                            ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                            ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                            ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                            Status = reader["Message"]?.ToString()
                                        };

                                        Fare.Add(Fares);
                                    }
                                }
                            }
                        }
                        else
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var faress = new tblFare
                                    {
                                        ID = reader["ID"].ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        RouteID = reader["RouteID"]?.ToString(),
                                        StopID = reader["StopID"]?.ToString(),
                                        BusID = reader["BusID"]?.ToString(),
                                        Amount = reader["Amount"] == DBNull.Value ? null : Convert.ToDecimal(reader["Amount"]),                                        //Description = reader["Description"]?.ToString(),
                                        IsActive = reader["IsActive"].ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString()
                                    };

                                    Fare.Add(faress);
                                }
                            }
                        }
                    }
                }

                return Fare;
            }
            catch (Exception ex)
            {
                LogException(ex, "SchoolManagementDAL", "Tbl_fare_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(fare));
                return new List<tblFare>
                {
                    new tblFare
                    {
                        Status = $"ERROR: {ex.Message}"
                    }
                };
            }
        }



        //Time Table Module
        public List<TblWorkingDays> Tbl_WorkingDays_CRUD_Operations(TblWorkingDays wrkdays)
        {
            var Wrkdays = new List<TblWorkingDays>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {

                using (var conn = new MySqlConnection(_connectionString))
                using (var cmd = new MySqlCommand("Proc_WorkingDays", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_ID", (object?)CleanParam(wrkdays.ID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SchoolID", (object?)CleanParam(wrkdays.SchoolID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_AcademicYear", (object?)CleanParam(wrkdays.AcademicYear) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Day", (object?)CleanParam(wrkdays.Day) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_StartTime", wrkdays.StartTime);
                    cmd.Parameters.AddWithValue("p_EndTime", wrkdays.EndTime);
                    cmd.Parameters.AddWithValue("p_IsActive", (object?)CleanParam(wrkdays.IsActive) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(wrkdays.CreatedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedIP", (object?)CleanParam(wrkdays.CreatedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(wrkdays.ModifiedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedIP", (object?)CleanParam(wrkdays.ModifiedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Flag", wrkdays.Flag ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Limit", wrkdays.Limit ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastCreatedDate", wrkdays.LastCreatedDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastID", wrkdays.LastID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortColumn", wrkdays.SortColumn ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortDirection", wrkdays.SortDirection ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Offset", wrkdays.Offset ?? (object)DBNull.Value);

                    conn.Open();

                    if (wrkdays.Flag != null)
                    {
                        if (wrkdays.Flag == "6" || wrkdays.Flag == "8")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var Wrkdayss = new TblWorkingDays
                                    {
                                        totalcount = reader["totalCount"] != DBNull.Value ? Convert.ToInt32(reader["totalCount"]) : (int?)null
                                    };

                                    Wrkdays.Add(Wrkdayss);
                                }
                            }
                        }
                        else if (wrkdays.Flag == "2" || wrkdays.Flag == "3" || wrkdays.Flag == "7" || wrkdays.Flag == "4")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var wrkdayss = new TblWorkingDays
                                    {
                                        //ID = reader["ID"] == DBNull.Value ? null : Convert.ToInt32(reader["ID"]),
                                        ID = reader["ID"].ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        Day = reader["Day"]?.ToString(),
                                        StartTime = reader["StartTime"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)reader["StartTime"],
                                        EndTime = reader["EndTime"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)reader["EndTime"],
                                        IsActive = reader["IsActive"].ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString(),
                                        SchoolName = reader["SchoolName"]?.ToString(),
                                        AcademicYearName = reader["AcademicYearName"]?.ToString()
                                    };

                                    Wrkdays.Add(wrkdayss);
                                }
                            }
                        }
                        else if (wrkdays.Flag == "1" || wrkdays.Flag == "5")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (reader["Message"]?.ToString() == "Working day already exists")
                                    {
                                        Wrkdays.Add(new TblWorkingDays
                                        {
                                            Status = reader["Message"]?.ToString()
                                        });
                                    }
                                    else
                                    {
                                        Wrkdays.Add(new TblWorkingDays
                                        {
                                            ID = reader["ID"].ToString(),
                                            SchoolID = reader["SchoolID"]?.ToString(),
                                            AcademicYear = reader["AcademicYear"]?.ToString(),
                                            Day = reader["Day"]?.ToString(),
                                            StartTime = reader["StartTime"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)reader["StartTime"],
                                            EndTime = reader["EndTime"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)reader["EndTime"],
                                            IsActive = reader["IsActive"].ToString(),
                                            CreatedBy = reader["CreatedBy"]?.ToString(),
                                            CreatedIp = reader["CreatedIp"]?.ToString(),
                                            CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                            ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                            ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                            ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                            Status = reader["Message"]?.ToString()
                                        });
                                    }
                                }
                            }
                        }
                        else
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var Wrkdays1 = new TblWorkingDays
                                    {
                                        ID = reader["ID"].ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        Day = reader["Day"]?.ToString(),
                                        StartTime = reader["StartTime"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)reader["StartTime"],
                                        EndTime = reader["EndTime"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)reader["EndTime"],
                                        IsActive = reader["IsActive"].ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString()
                                    };

                                    Wrkdays.Add(Wrkdays1);
                                }
                            }
                        }
                    }
                }

                return Wrkdays;
            }
            catch (Exception ex)
            {
                LogException(ex, "SchoolManagementDAL", "Tbl_WorkingDays_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(wrkdays));
                return new List<TblWorkingDays>
                {
                    new TblWorkingDays
                    {
                        Status = $"ERROR: {ex.Message}"
                    }
                };
            }
        }

        public List<TblSession> Tbl_Session_CRUD_Operations(TblSession Session1)
        {
            var Sessiontt = new List<TblSession>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {

                using (var conn = new MySqlConnection(_connectionString))
                using (var cmd = new MySqlCommand("Proc_Sessions", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_ID", (object?)CleanParam(Session1.ID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SchoolID", (object?)CleanParam(Session1.SchoolID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_AcademicYear", (object?)CleanParam(Session1.AcademicYear) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Session", (object?)CleanParam(Session1.Session) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_StartTime", Session1.StartTime);
                    cmd.Parameters.AddWithValue("p_EndTime", Session1.EndTime);
                    cmd.Parameters.AddWithValue("p_IsActive", (object?)CleanParam(Session1.IsActive) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(Session1.CreatedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedIP", (object?)CleanParam(Session1.CreatedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(Session1.ModifiedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedIP", (object?)CleanParam(Session1.ModifiedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Flag", Session1.Flag ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Limit", Session1.Limit ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastCreatedDate", Session1.LastCreatedDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastID", Session1.LastID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortColumn", Session1.SortColumn ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortDirection", Session1.SortDirection ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Offset", Session1.Offset ?? (object)DBNull.Value);

                    conn.Open();

                    if (Session1.Flag != null)
                    {
                        if (Session1.Flag == "6" || Session1.Flag == "8")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var Sessiontt1 = new TblSession
                                    {
                                        totalcount = reader["totalCount"] != DBNull.Value ? Convert.ToInt32(reader["totalCount"]) : (int?)null
                                    };

                                    Sessiontt.Add(Sessiontt1);
                                }
                            }
                        }
                        else if (Session1.Flag == "2" || Session1.Flag == "3" || Session1.Flag == "7" || Session1.Flag == "4")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var Sessiontt1 = new TblSession
                                    {
                                        //ID = reader["ID"] == DBNull.Value ? null : Convert.ToInt32(reader["ID"]),
                                        ID = reader["ID"].ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        Session = reader["Session"]?.ToString(),
                                        StartTime = reader["StartTime"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)reader["StartTime"],
                                        EndTime = reader["EndTime"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)reader["EndTime"],
                                        IsActive = reader["IsActive"].ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString(),
                                        SchoolName = reader["SchoolName"]?.ToString(),
                                        AcademicYearName = reader["AcademicYearName"]?.ToString()
                                    };

                                    Sessiontt.Add(Sessiontt1);
                                }
                            }
                        }
                        else if (Session1.Flag == "1" || Session1.Flag == "5")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (reader["Message"]?.ToString() == "Session already exists")
                                    {
                                        Sessiontt.Add(new TblSession
                                        {
                                            Status = reader["Message"]?.ToString()
                                        });
                                    }
                                    else
                                    {
                                        Sessiontt.Add(new TblSession
                                        {
                                            ID = reader["ID"].ToString(),
                                            SchoolID = reader["SchoolID"]?.ToString(),
                                            AcademicYear = reader["AcademicYear"]?.ToString(),
                                            Session = reader["Session"]?.ToString(),
                                            StartTime = reader["StartTime"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)reader["StartTime"],
                                            EndTime = reader["EndTime"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)reader["EndTime"],
                                            IsActive = reader["IsActive"].ToString(),
                                            CreatedBy = reader["CreatedBy"]?.ToString(),
                                            CreatedIp = reader["CreatedIp"]?.ToString(),
                                            CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                            ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                            ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                            ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                            Status = reader["Message"]?.ToString()
                                        });
                                    }
                                }
                            }
                        }
                        else
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var Sessiontt1 = new TblSession
                                    {
                                        ID = reader["ID"].ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        Session = reader["Session"]?.ToString(),
                                        StartTime = reader["StartTime"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)reader["StartTime"],
                                        EndTime = reader["EndTime"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)reader["EndTime"],
                                        IsActive = reader["IsActive"].ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString()
                                    };

                                    Sessiontt.Add(Sessiontt1);
                                }
                            }
                        }
                    }
                }

                return Sessiontt;
            }
            catch (Exception ex)
            {
                LogException(ex, "SchoolManagementDAL", "Tbl_Session_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(Session1));
                return new List<TblSession>
                {
                    new TblSession
                    {
                        Status = $"ERROR: {ex.Message}"
                    }
                };
            }
        }

        public List<TblTimeTable> Tbl_TimeTable_CRUD(TblTimeTable model)
        {
            var list = new List<TblTimeTable>();

            string Clean(string? value)
            {
                return string.IsNullOrWhiteSpace(value) ? null : value;
            }

            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                using (var cmd = new MySqlCommand("Proc_TimeTable_CRUD_Operations", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("p_ID", Clean(model.ID) ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SchoolID", Clean(model.SchoolID) ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_AcademicYear", Clean(model.AcademicYear) ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ClassID", Clean(model.ClassID) ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_DivisionID", Clean(model.DivisionID) ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_DateFrom", model.DateFrom ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_DateTo", model.DateTo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_NoOfPeriods", model.NoOfPeriods ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_IsActive", Clean(model.IsActive) ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_TimetableJSON", model.TimetableJSON ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedBy", Clean(model.CreatedBy) ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedIP", Clean(model.CreatedIP) ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedBy", Clean(model.ModifiedBy) ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedIP", Clean(model.ModifiedIP) ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Flag", model.Flag ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Limit", model.Limit ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Offset", model.Offset ?? (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("p_StartTime", model.StartTime ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_EndTime", model.EndTime ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_DayID", Clean(model.DayID) ?? (object)DBNull.Value);

                    conn.Open();

                    // ================= COUNT FLAGS =================
                    if (model.Flag == "6" || model.Flag == "8")
                    {
                        using var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            list.Add(new TblTimeTable
                            {
                                totalCount = reader["totalCount"] == DBNull.Value ? null : Convert.ToInt32(reader["totalCount"])
                            });
                        }
                    }

                    // ================= FETCH BY ID =================
                    else if (model.Flag == "4")
                    {
                        TblTimeTable header = null;
                        var details = new List<TblTimeTableDetail>();

                        using var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            if (header == null)
                            {
                                header = new TblTimeTable
                                {
                                    ID = reader["TimeTableID"]?.ToString(),
                                    SchoolID = reader["SchoolID"]?.ToString(),
                                    AcademicYear = reader["AcademicYear"]?.ToString(),
                                    ClassID = reader["ClassID"]?.ToString(),
                                    DivisionID = reader["DivisionID"]?.ToString(),
                                    DateFrom = reader["DateFrom"] == DBNull.Value ? null : Convert.ToDateTime(reader["DateFrom"]),
                                    DateTo = reader["DateTo"] == DBNull.Value ? null : Convert.ToDateTime(reader["DateTo"]),
                                    NoOfPeriods = reader["NoOfPeriods"] == DBNull.Value ? null : Convert.ToInt32(reader["NoOfPeriods"]),
                                    IsActive = reader["IsActive"]?.ToString(),
                                    TimeTableDetails = new List<TblTimeTableDetail>()
                                };
                            }

                            if (reader["DetailID"] != DBNull.Value)
                            {
                                details.Add(new TblTimeTableDetail
                                {
                                    ID = reader["DetailID"].ToString(),
                                    TimeTableID = reader["TimeTableID"]?.ToString(),
                                    DayID = reader["DayID"]?.ToString(),
                                    PeriodNo = reader["PeriodNo"] == DBNull.Value ? null : Convert.ToInt32(reader["PeriodNo"]),
                                    SessionID = reader["SessionID"]?.ToString(),
                                    StartTime = reader["StartTime"] == DBNull.Value ? null : (TimeSpan?)reader["StartTime"],
                                    EndTime = reader["EndTime"] == DBNull.Value ? null : (TimeSpan?)reader["EndTime"],
                                    SubjectID = reader["SubjectID"]?.ToString(),
                                    StaffID = reader["StaffID"]?.ToString()
                                });
                            }
                        }

                        if (header != null)
                        {
                            header.TimeTableDetails = details;
                            list.Add(header);
                        }
                    }

                    // ================= INSERT / UPDATE =================

                    else if (model.Flag == "1" || model.Flag == "5")
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (reader["Message"]?.ToString() == "TimeTable already exists for this Class & Division" || reader["Message"]?.ToString() == "Staff already allocated for this time slot")
                                {
                                    list.Add(new TblTimeTable
                                    {
                                        Status = reader["Message"]?.ToString()
                                    });
                                }
                                else
                                {
                                    list.Add(new TblTimeTable
                                    {
                                        ID = reader["ID"]?.ToString(),
                                        Status = reader["Message"]?.ToString()
                                    });
                                }
                            }
                        }
                    }

                    else if (model.Flag == "2" || model.Flag == "3")
                    {
                        using var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            list.Add(new TblTimeTable
                            {
                                ID = reader["ID"]?.ToString(),
                                SchoolID = reader["SchoolID"]?.ToString(),
                                AcademicYear = reader["AcademicYear"]?.ToString(),
                                ClassID = reader["ClassID"]?.ToString(),
                                DivisionID = reader["DivisionID"]?.ToString(),
                                DateFrom = reader["DateFrom"] == DBNull.Value ? null : Convert.ToDateTime(reader["DateFrom"]),
                                DateTo = reader["DateTo"] == DBNull.Value ? null : Convert.ToDateTime(reader["DateTo"]),
                                NoOfPeriods = reader["NoOfPeriods"] == DBNull.Value ? null : Convert.ToInt32(reader["NoOfPeriods"]),
                                IsActive = reader["IsActive"]?.ToString(),
                                SchoolName = reader["SchoolName"]?.ToString(),
                                AcademicYearName = reader["AcademicYearName"]?.ToString()
                            });
                        }
                    }

                    else if (model.Flag == "10")
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new TblTimeTable
                                {
                                    StartTime = reader["StartTime"] == DBNull.Value ? null : (TimeSpan?)reader["StartTime"],
                                    EndTime = reader["EndTime"] == DBNull.Value ? null : (TimeSpan?)reader["EndTime"],
                                    TimeSlot = reader["TimeSlot"]?.ToString()
                                });
                            }
                        }
                    }

                    else if (model.Flag == "11")
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new TblTimeTable
                                {
                                    StaffID = reader["StaffID"]?.ToString(),
                                    StaffName = reader["StaffName"]?.ToString(),
                                    StaffEmail = reader["StaffEmail"]?.ToString()
                                });
                            }
                        }
                    }

                    else if (model.Flag == "12")
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new TblTimeTable
                                {
                                    TimeTableID = reader["TimeTableID"]?.ToString(),
                                    DayID = reader["DayID"]?.ToString(),
                                    Day = reader["Day"]?.ToString(),
                                    PeriodNo = reader["PeriodNo"] == DBNull.Value ? null : Convert.ToInt32(reader["PeriodNo"]),
                                    StartTime = reader["StartTime"] == DBNull.Value ? null : (TimeSpan?)reader["StartTime"],
                                    EndTime = reader["EndTime"] == DBNull.Value ? null : (TimeSpan?)reader["EndTime"],
                                    SubjectID = reader["SubjectID"]?.ToString(),
                                    SubjectName = reader["SubjectName"]?.ToString(),
                                    StaffID = reader["StaffID"]?.ToString(),
                                    StaffName = reader["StaffName"]?.ToString(),
                                    ClassName = reader["ClassName"]?.ToString(),
                                    DivisionName = reader["DivisionName"]?.ToString(),
                                });
                            }
                        }
                    }

                    else if (model.Flag == "9")
                    {
                        using var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            list.Add(new TblTimeTable
                            {
                                ID = reader["ID"]?.ToString(),
                                SchoolID = reader["SchoolID"]?.ToString(),
                                AcademicYear = reader["AcademicYear"]?.ToString(),
                                ClassID = reader["ClassID"]?.ToString(),
                                DivisionID = reader["DivisionID"]?.ToString(),
                                DayID = reader["DayID"].ToString(),
                                Day = reader["Day"].ToString(),
                                PeriodNo = reader["PeriodNo"] == DBNull.Value ? null : Convert.ToInt32(reader["PeriodNo"]),
                                SessionID = reader["SessionID"].ToString(),
                                StartTime = reader["StartTime"] == DBNull.Value ? null : (TimeSpan?)reader["StartTime"],
                                EndTime = reader["EndTime"] == DBNull.Value ? null : (TimeSpan?)reader["EndTime"],
                                SubjectID = reader["DayID"].ToString(),
                                StaffID = reader["DayID"].ToString(),
                                ClassName = reader["ClassName"]?.ToString(),
                                DivisionName = reader["DivisionName"]?.ToString(),
                                SubjectName = reader["SubjectName"]?.ToString()
                            });
                        }
                    }

                    // ================= FETCH ALL / ACTIVE =================
                    else
                    {
                        using var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            list.Add(new TblTimeTable
                            {
                                ID = reader["ID"]?.ToString(),
                                SchoolID = reader["SchoolID"]?.ToString(),
                                AcademicYear = reader["AcademicYear"]?.ToString(),
                                ClassID = reader["ClassID"]?.ToString(),
                                DivisionID = reader["DivisionID"]?.ToString(),
                                DateFrom = reader["DateFrom"] == DBNull.Value ? null : Convert.ToDateTime(reader["DateFrom"]),
                                DateTo = reader["DateTo"] == DBNull.Value ? null : Convert.ToDateTime(reader["DateTo"]),
                                NoOfPeriods = reader["NoOfPeriods"] == DBNull.Value ? null : Convert.ToInt32(reader["NoOfPeriods"]),
                                IsActive = reader["IsActive"]?.ToString()
                            });
                        }
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                LogException(ex, "SchoolManagementDAL", "Tbl_TimeTable_CRUD", Newtonsoft.Json.JsonConvert.SerializeObject(model));
                return new List<TblTimeTable>
                {
                    new TblTimeTable { Status = "ERROR: " + ex.Message }
                };
            }
        }

        //Exam Module
        public List<tblExamType> Tbl_ExamType_CRUD_Operations(tblExamType examtype)
        {
            var ExamType = new List<tblExamType>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                using (var cmd = new MySqlCommand("Proc_ExamType", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_ID", (object?)CleanParam(examtype.ID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SchoolID", (object?)CleanParam(examtype.SchoolID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_AcademicYear", (object?)CleanParam(examtype.AcademicYear) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ExamTypeName", (object?)CleanParam(examtype.ExamTypeName) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Priority", (object?)CleanParam(examtype.Priority) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ExamType", (object?)CleanParam(examtype.ExamType) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_MaxMark", (object?)CleanParam(examtype.MaxMark) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_PassMarks", (object?)CleanParam(examtype.PassMarks) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ExamDuration", (object?)CleanParam(examtype.ExamDuration) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_NoofQuestion", (object?)CleanParam(examtype.NoofQuestion) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Instructions", (object?)CleanParam(examtype.Instructions) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_IsActive", (object?)CleanParam(examtype.IsActive) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(examtype.CreatedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedIP", (object?)CleanParam(examtype.CreatedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(examtype.ModifiedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedIP", (object?)CleanParam(examtype.ModifiedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Flag", examtype.Flag ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Limit", examtype.Limit ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastCreatedDate", examtype.LastCreatedDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastID", examtype.LastID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortColumn", examtype.SortColumn ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortDirection", examtype.SortDirection ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Offset", examtype.Offset ?? (object)DBNull.Value);

                    conn.Open();

                    if (examtype.Flag != null)
                    {
                        if (examtype.Flag == "6" || examtype.Flag == "8")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var faress = new tblExamType
                                    {
                                        totalcount = reader["totalCount"] != DBNull.Value ? Convert.ToInt32(reader["totalCount"]) : (int?)null
                                    };

                                    ExamType.Add(faress);
                                }
                            }
                        }
                        else if (examtype.Flag == "2" || examtype.Flag == "3" || examtype.Flag == "7" || examtype.Flag == "4")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var examtypee = new tblExamType
                                    {
                                        //ID = reader["ID"] == DBNull.Value ? null : Convert.ToInt32(reader["ID"]),
                                        ID = reader["ID"].ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        ExamTypeName = reader["ExamTypeName"]?.ToString(),
                                        Priority = reader["Priority"]?.ToString(),
                                        ExamType = reader["ExamType"]?.ToString(),
                                        MaxMark = reader["MaxMark"]?.ToString(),
                                        PassMarks = reader["PassMarks"]?.ToString(),
                                        ExamDuration = reader["ExamDuration"]?.ToString(),
                                        NoofQuestion = reader["NoofQuestion"]?.ToString(),
                                        Instructions = reader["Instructions"]?.ToString(),

                                        IsActive = reader["IsActive"].ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString(),
                                        SchoolName = reader["SchoolName"]?.ToString(),
                                        AcademicYearName = reader["AcademicYearName"]?.ToString(),

                                    };

                                    ExamType.Add(examtypee);
                                }
                            }
                        }
                        else
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var examtypeee = new tblExamType
                                    {
                                        ID = reader["ID"].ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        ExamTypeName = reader["ExamTypeName"]?.ToString(),
                                        Priority = reader["Priority"]?.ToString(),
                                        ExamType = reader["ExamType"]?.ToString(),
                                        MaxMark = reader["MaxMark"]?.ToString(),
                                        PassMarks = reader["PassMarks"]?.ToString(),
                                        ExamDuration = reader["ExamDuration"]?.ToString(),
                                        NoofQuestion = reader["NoofQuestion"]?.ToString(),
                                        Instructions = reader["Instructions"]?.ToString(),
                                        IsActive = reader["IsActive"].ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString()
                                    };

                                    ExamType.Add(examtypeee);
                                }
                            }
                        }
                    }
                }

                return ExamType;
            }
            catch (Exception ex)
            {
                LogException(ex, "SchoolManagementDAL", "Tbl_ExamType_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(ExamType));
                return new List<tblExamType>
                {
                    new tblExamType
                    {
                        Status = $"ERROR: {ex.Message}"
                    }
                };
            }
        }
        public List<tblSetExam> Tbl_SetExam_CRUD_Operations(tblSetExam examtype)
        {
            var ExamType = new List<tblSetExam>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                using (var cmd = new MySqlCommand("Proc_Tbl_SetExam", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_ID", (object?)CleanParam(examtype.ID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SchoolID", (object?)CleanParam(examtype.SchoolID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_AcademicYear", (object?)CleanParam(examtype.AcademicYear) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Syllabus", (object?)CleanParam(examtype.Syllabus) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Class", (object?)CleanParam(examtype.Class) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Divisions", (object?)CleanParam(examtype.Divisions) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ExamType", (object?)CleanParam(examtype.ExamType) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Subjects", (object?)CleanParam(examtype.Subjects) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_MaxMarks", (object?)CleanParam(examtype.MaxMarks) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_PassMarks", (object?)CleanParam(examtype.PassMarks) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ExamDateAndTime", (object?)CleanParam(examtype.ExamDateAndTime) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Duration", (object?)CleanParam(examtype.Duration) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_NoOfQuestion", (object?)CleanParam(examtype.NoOfQuestion) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Instructions", (object?)CleanParam(examtype.Instructions) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_IsActive", (object?)CleanParam(examtype.IsActive) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(examtype.CreatedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedIP", (object?)CleanParam(examtype.CreatedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(examtype.ModifiedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedIP", (object?)CleanParam(examtype.ModifiedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Flag", examtype.Flag ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Limit", examtype.Limit ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastCreatedDate", examtype.LastCreatedDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastID", examtype.LastID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortColumn", examtype.SortColumn ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortDirection", examtype.SortDirection ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Offset", examtype.Offset ?? (object)DBNull.Value);

                    conn.Open();

                    if (examtype.Flag != null)
                    {
                        if (examtype.Flag == "6" || examtype.Flag == "8")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var faress = new tblSetExam
                                    {
                                        totalcount = reader["totalCount"] != DBNull.Value ? Convert.ToInt32(reader["totalCount"]) : (int?)null
                                    };

                                    ExamType.Add(faress);
                                }
                            }
                        }
                        else if (examtype.Flag == "2" || examtype.Flag == "3" || examtype.Flag == "7" || examtype.Flag == "4")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var examtypee = new tblSetExam
                                    {
                                        //ID = reader["ID"] == DBNull.Value ? null : Convert.ToInt32(reader["ID"]),
                                        ID = reader["ID"].ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        Syllabus = reader["Syllabus"]?.ToString(),
                                        Class = reader["Class"]?.ToString(),
                                        Divisions = reader["Divisions"]?.ToString(),
                                        ExamType = reader["ExamType"]?.ToString(),
                                        Subjects = reader["Subjects"]?.ToString(),
                                        MaxMarks = reader["MaxMarks"]?.ToString(),
                                        PassMarks = reader["PassMarks"]?.ToString(),
                                        ExamDateAndTime = reader["ExamDateAndTime"]?.ToString(),
                                        Duration = reader["Duration"]?.ToString(),
                                        NoOfQuestion = reader["NoOfQuestion"]?.ToString(),
                                        Instructions = reader["Instructions"]?.ToString(),
                                        ClassName = reader["ClassName"]?.ToString(),
                                        SyllabusName = reader["SyllabusName"]?.ToString(),
                                        DivisionName = reader["DivisionName"]?.ToString(),
                                        ExamTypeName = reader["ExamTypeName"]?.ToString(),
                                        SubjectName = reader["SubjectName"]?.ToString(),
                                        IsActive = reader["IsActive"].ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString(),
                                        SchoolName = reader["SchoolName"]?.ToString(),
                                        AcademicYearName = reader["AcademicYearName"]?.ToString(),

                                    };

                                    ExamType.Add(examtypee);
                                }
                            }
                        }
                        else if (examtype.Flag == "10")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var examtypee = new tblSetExam
                                    {
                                        RowID = reader["RowID"]?.ToString(),
                                        ID = reader["ID"].ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        Class = reader["Class"]?.ToString(),
                                        Divisions = reader["Divisions"]?.ToString(),
                                        ExamType = reader["ExamType"]?.ToString(),
                                        Subjects = reader["Subjects"]?.ToString(),
                                        MaxMarks = reader["MaxMarks"]?.ToString(),
                                        PassMarks = reader["PassMarks"]?.ToString(),
                                        ExamDateAndTime = reader["ExamDateAndTime"]?.ToString(),
                                        Duration = reader["Duration"]?.ToString(),
                                        NoOfQuestion = reader["NoOfQuestion"]?.ToString(),
                                        Instructions = reader["Instructions"]?.ToString(),
                                        IsActive = reader["IsActive"].ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        SubjectIndex = reader["SubjectIndex"]?.ToString(),
                                        SubjectID = reader["SubjectID"]?.ToString(),
                                        IndividualSubjectName = reader["IndividualSubjectName"]?.ToString(),
                                        SubjectExamDateAndTime = reader["SubjectExamDateAndTime"] == DBNull.Value ? null : Convert.ToDateTime(reader["SubjectExamDateAndTime"]),
                                        DivisionList = reader["DivisionList"]?.ToString(),
                                        SchoolName = reader["SchoolName"]?.ToString(),
                                        AcademicYearName = reader["AcademicYearName"]?.ToString(),
                                        ClassName = reader["ClassName"]?.ToString(),
                                        SyllabusName = reader["SyllabusName"]?.ToString(),
                                        ExamTypeName = reader["ExamTypeName"]?.ToString(),
                                        AttendanceMarked = reader["AttendanceMarked"]?.ToString()
                                    };

                                    ExamType.Add(examtypee);
                                }
                            }
                        }
                        else
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var examtypeee = new tblSetExam
                                    {
                                        ID = reader["ID"].ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        Syllabus = reader["Syllabus"]?.ToString(),
                                        Class = reader["Class"]?.ToString(),
                                        Divisions = reader["Divisions"]?.ToString(),
                                        ExamType = reader["ExamType"]?.ToString(),
                                        Subjects = reader["Subjects"]?.ToString(),
                                        MaxMarks = reader["MaxMarks"]?.ToString(),
                                        PassMarks = reader["PassMarks"]?.ToString(),
                                        ExamDateAndTime = reader["ExamDateAndTime"]?.ToString(),
                                        Duration = reader["Duration"]?.ToString(),
                                        NoOfQuestion = reader["NoOfQuestion"]?.ToString(),
                                        Instructions = reader["Instructions"]?.ToString(),
                                        IsActive = reader["IsActive"].ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString()
                                    };

                                    ExamType.Add(examtypeee);
                                }
                            }
                        }
                    }
                }

                return ExamType;
            }
            catch (Exception ex)
            {
                LogException(ex, "SchoolManagementDAL", "Tbl_SetExam_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(ExamType));
                return new List<tblSetExam>
                {
                    new tblSetExam
                    {
                        Status = $"ERROR: {ex.Message}"
                    }
                };
            }
        }


        public List<tblExamAttendence> Tbl_ExamAttendece_CRUD_Operations(tblExamAttendence exam)
        {
            var result = new List<tblExamAttendence>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                using (var cmd = new MySqlCommand("Proc_ExamAttendance", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("p_ID", (object?)CleanParam(exam.ID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SchoolID", (object?)CleanParam(exam.SchoolID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_AcademicYear", (object?)CleanParam(exam.AcademicYear) ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("p_ExamID", (object?)CleanParam(exam.ExamID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SubjectID", (object?)CleanParam(exam.SubjectID) ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("p_Class", (object?)CleanParam(exam.Class) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Division", (object?)CleanParam(exam.Division) ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("p_AdmissionID", (object?)CleanParam(exam.AdmissionID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Attendance", (object?)CleanParam(exam.Attendance) ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("p_IsActive", (object?)CleanParam(exam.IsActive) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(exam.CreatedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedIP", (object?)CleanParam(exam.CreatedIp) ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(exam.ModifiedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedIP", (object?)CleanParam(exam.ModifiedIp) ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("p_Flag", exam.Flag ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Limit", exam.Limit ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastCreatedDate", exam.LastCreatedDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastID", exam.LastID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortColumn", exam.SortColumn ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortDirection", exam.SortDirection ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Offset", exam.Offset ?? (object)DBNull.Value);

                    conn.Open();

                    if (exam.Flag == "6" || exam.Flag == "8")
                    {
                        using var reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            result.Add(new tblExamAttendence
                            {
                                totalcount = reader["totalCount"] != DBNull.Value ? Convert.ToInt32(reader["totalCount"]) : (int?)null
                            });
                        }
                    }

                    else if (exam.Flag == "2" || exam.Flag == "3" || exam.Flag == "4" || exam.Flag == "7")
                    {
                        using var reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            result.Add(new tblExamAttendence
                            {
                                ID = reader["ID"]?.ToString(),
                                SchoolID = reader["SchoolID"]?.ToString(),
                                AcademicYear = reader["AcademicYear"]?.ToString(),

                                ExamID = reader["ExamID"]?.ToString(),
                                SubjectID = reader["SubjectID"]?.ToString(),

                                AdmissionID = reader["AdmissionID"]?.ToString(),
                                Attendance = reader["Attendance"]?.ToString(),

                                ExamName = reader["ExamName"]?.ToString(),
                                SubjectName = reader["SubjectName"]?.ToString(),

                                IsActive = reader["IsActive"]?.ToString(),

                                CreatedBy = reader["CreatedBy"]?.ToString(),
                                CreatedIp = reader["CreatedIp"]?.ToString(),
                                CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),

                                ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),

                                Status = reader["Message"]?.ToString(),
                                SchoolName = reader["SchoolName"]?.ToString(),
                                AcademicYearName = reader["AcademicYearName"]?.ToString(),
                                StudentName = reader["StudentName"]?.ToString()
                            });
                        }
                    }

                    else if (exam.Flag == "9")
                    {
                        using var reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            result.Add(new tblExamAttendence
                            {
                                ID = reader["ID"]?.ToString(),
                                SchoolID = reader["SchoolID"]?.ToString(),
                                AcademicYear = reader["AcademicYear"]?.ToString(),

                                ExamID = reader["ExamID"]?.ToString(),
                                SubjectID = reader["SubjectID"]?.ToString(),

                                AdmissionID = reader["AdmissionID"]?.ToString(),
                                Attendance = reader["Attendance"]?.ToString(),
                                IsActive = reader["IsActive"]?.ToString(),

                                ClassName = reader["ClassName"]?.ToString(),
                                DivisionName = reader["ClassDivisionName"]?.ToString(),
                                SubjectName = reader["SubjectName"]?.ToString(),
                                ExamName = reader["ExamTypeName"]?.ToString(),
                                StudentName = reader["StudentName"]?.ToString(),



                                CreatedBy = reader["CreatedBy"]?.ToString(),
                                CreatedIp = reader["CreatedIp"]?.ToString(),
                                CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),

                                ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),

                                Status = reader["Message"]?.ToString()
                            });
                        }
                    }

                    else
                    {
                        using var reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            result.Add(new tblExamAttendence
                            {
                                ID = reader["ID"]?.ToString(),
                                SchoolID = reader["SchoolID"]?.ToString(),
                                AcademicYear = reader["AcademicYear"]?.ToString(),

                                ExamID = reader["ExamID"]?.ToString(),
                                SubjectID = reader["SubjectID"]?.ToString(),

                                AdmissionID = reader["AdmissionID"]?.ToString(),
                                Attendance = reader["Attendance"]?.ToString(),

                                IsActive = reader["IsActive"]?.ToString(),

                                CreatedBy = reader["CreatedBy"]?.ToString(),
                                CreatedIp = reader["CreatedIp"]?.ToString(),
                                CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),

                                ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),

                                Status = reader["Message"]?.ToString()
                            });
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                LogException(ex, "SchoolManagementDAL", "Tbl_ExamAttendece_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(exam));

                return new List<tblExamAttendence>
                {
                    new tblExamAttendence
                    {
                        Status = $"ERROR: {ex.Message}"
                    }
                };
            }
        }

        public List<feeCategory> Tbl_feeCategory_CRUD_Operations(feeCategory fee)
        {
            var Routes = new List<feeCategory>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                using (var cmd = new MySqlCommand("Proc_feeCategory", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("p_SchoolID", (object?)CleanParam(fee.SchoolID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_AcademicYear", (object?)CleanParam(fee.AcademicYear) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ID", (object?)CleanParam(fee.ID) ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("p_FeeCategoryName", (object?)CleanParam(fee.FeeCategoryName) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_FeeType", (object?)CleanParam(fee.FeeType) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_FeeCollectionDuration", (object?)CleanParam(fee.FeeCollectionDuration) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_FeeDueDay", (object?)CleanParam(fee.FeeDueDay) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_FineType", (object?)CleanParam(fee.FineType) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_FineValue", (object?)CleanParam(fee.FineValue) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_FineCollectionType", (object?)CleanParam(fee.FineCollectionType) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_FineIncrementIn", (object?)CleanParam(fee.FineIncrementIn) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Description", (object?)CleanParam(fee.Description) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_IsActive", (object?)CleanParam(fee.IsActive) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(fee.CreatedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedIP", (object?)CleanParam(fee.CreatedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(fee.ModifiedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedIP", (object?)CleanParam(fee.ModifiedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Flag", fee.Flag ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Limit", fee.Limit ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastCreatedDate", fee.LastCreatedDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastID", fee.LastID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortColumn", fee.SortColumn ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortDirection", fee.SortDirection ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Offset", fee.Offset ?? (object)DBNull.Value);

                    conn.Open();

                    if (fee.Flag != null)
                    {
                        if (fee.Flag == "6" || fee.Flag == "8")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var routess = new feeCategory
                                    {
                                        totalcount = reader["totalCount"] != DBNull.Value ? Convert.ToInt32(reader["totalCount"]) : (int?)null
                                    };

                                    Routes.Add(routess);
                                }
                            }
                        }
                        else if (fee.Flag == "2" || fee.Flag == "4" || fee.Flag == "7")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var routesgs = new feeCategory
                                    {
                                        //ID = reader["ID"] == DBNull.Value ? null : Convert.ToInt32(reader["ID"]),
                                        ID = reader["ID"].ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),

                                        FeeCategoryName = reader["FeeCategoryName"]?.ToString(),
                                        FeeType = reader["FeeType"]?.ToString(),
                                        FeeCollectionDuration = reader["FeeCollectionDuration"]?.ToString(),
                                        FeeDueDay = reader["FeeDueDay"]?.ToString(),
                                        FineType = reader["FineType"]?.ToString(),
                                        FineValue = reader["FineValue"]?.ToString(),
                                        FineCollectionType = reader["FineCollectionType"]?.ToString(),
                                        FineIncrementIn = reader["FineIncrementIn"]?.ToString(),
                                        Description = reader["Description"]?.ToString(),
                                        IsActive = reader["IsActive"].ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString(),
                                        SchoolName = reader["SchoolName"]?.ToString(),
                                        AcademicYearName = reader["AcademicYearName"]?.ToString()
                                    };

                                    Routes.Add(routesgs);
                                }
                            }
                        }
                        else if (fee.Flag == "1" || fee.Flag == "5")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (reader["Message"]?.ToString() == "Fee Category Already Exists")
                                    {
                                        var routesgs = new feeCategory
                                        {
                                            Status = reader["Message"]?.ToString(),
                                        };

                                        Routes.Add(routesgs);
                                    }
                                    else
                                    {
                                        var routess = new feeCategory
                                        {
                                            ID = reader["ID"].ToString(),
                                            SchoolID = reader["SchoolID"]?.ToString(),
                                            AcademicYear = reader["AcademicYear"]?.ToString(),

                                            FeeCategoryName = reader["FeeCategoryName"]?.ToString(),
                                            FeeType = reader["FeeType"]?.ToString(),
                                            FeeCollectionDuration = reader["FeeCollectionDuration"]?.ToString(),
                                            FeeDueDay = reader["FeeDueDay"]?.ToString(),
                                            FineType = reader["FineType"]?.ToString(),
                                            FineValue = reader["FineValue"]?.ToString(),
                                            FineCollectionType = reader["FineCollectionType"]?.ToString(),
                                            FineIncrementIn = reader["FineIncrementIn"]?.ToString(),
                                            Description = reader["Description"]?.ToString(),
                                            IsActive = reader["IsActive"].ToString(),
                                            CreatedBy = reader["CreatedBy"]?.ToString(),
                                            CreatedIp = reader["CreatedIp"]?.ToString(),
                                            CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                            ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                            ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                            ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                            Status = reader["Message"]?.ToString()
                                        };

                                        Routes.Add(routess);
                                    }
                                }
                            }
                        }
                        else
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var routess = new feeCategory
                                    {
                                        ID = reader["ID"].ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),

                                        FeeCategoryName = reader["FeeCategoryName"]?.ToString(),
                                        FeeType = reader["FeeType"]?.ToString(),
                                        FeeCollectionDuration = reader["FeeCollectionDuration"]?.ToString(),
                                        FeeDueDay = reader["FeeDueDay"]?.ToString(),
                                        FineType = reader["FineType"]?.ToString(),
                                        FineValue = reader["FineValue"]?.ToString(),
                                        FineCollectionType = reader["FineCollectionType"]?.ToString(),
                                        FineIncrementIn = reader["FineIncrementIn"]?.ToString(),
                                        Description = reader["Description"]?.ToString(),
                                        IsActive = reader["IsActive"].ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString()
                                    };

                                    Routes.Add(routess);
                                }
                            }
                        }
                    }
                }

                return Routes;
            }
            catch (Exception ex)
            {
                LogException(ex, "SchoolManagementDAL", "Tbl_feeCategory_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(fee));
                return new List<feeCategory>
                {
                    new feeCategory
                    {
                        Status = $"ERROR: {ex.Message}"
                    }
                };
            }
        }
        public List<tblfeeAllocation> Tbl_feeallocation_CRUD_Operations(tblfeeAllocation fee)
        {
            var Routes = new List<tblfeeAllocation>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                using (var cmd = new MySqlCommand("Proc_Tbl_FeeAllocation", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("p_SchoolID", (object?)CleanParam(fee.SchoolID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_AcademicYear", (object?)CleanParam(fee.AcademicYear) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ID", (object?)CleanParam(fee.ID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Syllabus", (object?)CleanParam(fee.Syllabus) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Class", (object?)CleanParam(fee.Class) ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("p_Divisions", (object?)CleanParam(fee.Divisions) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_FeeCategory", (object?)CleanParam(fee.FeeCategory) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Amount", (object?)CleanParam(fee.Amount) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_StartDate", (object?)CleanParam(fee.StartDate) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_EndDate", (object?)CleanParam(fee.EndDate) ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("p_IsActive", (object?)CleanParam(fee.IsActive) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(fee.CreatedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedIP", (object?)CleanParam(fee.CreatedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(fee.ModifiedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedIP", (object?)CleanParam(fee.ModifiedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Flag", fee.Flag ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Limit", fee.Limit ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastCreatedDate", fee.LastCreatedDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastID", fee.LastID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortColumn", fee.SortColumn ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortDirection", fee.SortDirection ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Offset", fee.Offset ?? (object)DBNull.Value);

                    conn.Open();

                    if (fee.Flag != null)
                    {
                        if (fee.Flag == "6" || fee.Flag == "8")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var routess = new tblfeeAllocation
                                    {
                                        totalcount = reader["totalCount"] != DBNull.Value ? Convert.ToInt32(reader["totalCount"]) : (int?)null
                                    };

                                    Routes.Add(routess);
                                }
                            }
                        }
                        else if (fee.Flag == "2" || fee.Flag == "3" || fee.Flag == "4" || fee.Flag == "7")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var routesgs = new tblfeeAllocation
                                    {
                                        //ID = reader["ID"] == DBNull.Value ? null : Convert.ToInt32(reader["ID"]),
                                        ID = reader["ID"].ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        Syllabus = reader["Syllabus"]?.ToString(),
                                        Class = reader["Class"]?.ToString(),
                                        Divisions = reader["Divisions"]?.ToString(),
                                        FeeCategory = reader["FeeCategory"]?.ToString(),
                                        Amount = reader["Amount"]?.ToString(),
                                        StartDate = reader["StartDate"]?.ToString(),
                                        EndDate = reader["EndDate"]?.ToString(),
                                        IsActive = reader["IsActive"].ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString(),
                                        SchoolName = reader["SchoolName"]?.ToString(),
                                        AcademicYearName = reader["AcademicYearName"]?.ToString(),
                                        SyllabusName = reader["SyllabusName"]?.ToString(),
                                        ClassName = reader["ClassName"]?.ToString(),
                                        DivisionName = reader["DivisionName"]?.ToString(),
                                        FeeCategoryName = reader["FeeCategoryName"]?.ToString()
                                    };

                                    Routes.Add(routesgs);
                                }
                            }
                        }
                        else if (fee.Flag == "1" || fee.Flag == "5")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (reader["Message"]?.ToString() == "Fee already allocated for this category")
                                    {
                                        var routesgs = new tblfeeAllocation
                                        {
                                            Status = reader["Message"]?.ToString(),
                                        };

                                        Routes.Add(routesgs);
                                    }
                                    else
                                    {
                                        var routess = new tblfeeAllocation
                                        {
                                            ID = reader["ID"].ToString(),
                                            SchoolID = reader["SchoolID"]?.ToString(),
                                            AcademicYear = reader["AcademicYear"]?.ToString(),
                                            Syllabus = reader["Syllabus"]?.ToString(),
                                            Class = reader["Class"]?.ToString(),
                                            Divisions = reader["Divisions"]?.ToString(),
                                            FeeCategory = reader["FeeCategory"]?.ToString(),
                                            Amount = reader["Amount"]?.ToString(),
                                            StartDate = reader["StartDate"]?.ToString(),
                                            EndDate = reader["EndDate"]?.ToString(),
                                            IsActive = reader["IsActive"].ToString(),
                                            CreatedBy = reader["CreatedBy"]?.ToString(),
                                            CreatedIp = reader["CreatedIp"]?.ToString(),
                                            CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                            ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                            ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                            ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                            Status = reader["Message"]?.ToString()
                                        };

                                        Routes.Add(routess);
                                    }
                                }
                            }
                        }
                        else
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var routess = new tblfeeAllocation
                                    {
                                        ID = reader["ID"].ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        Syllabus = reader["Syllabus"]?.ToString(),
                                        Class = reader["Class"]?.ToString(),
                                        Divisions = reader["Divisions"]?.ToString(),
                                        FeeCategory = reader["FeeCategory"]?.ToString(),
                                        Amount = reader["Amount"]?.ToString(),
                                        StartDate = reader["StartDate"]?.ToString(),
                                        EndDate = reader["EndDate"]?.ToString(),
                                        IsActive = reader["IsActive"].ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString()
                                    };

                                    Routes.Add(routess);
                                }
                            }
                        }
                    }
                }

                return Routes;
            }
            catch (Exception ex)
            {
                LogException(ex, "SchoolManagementDAL", "Tbl_feeAllocation_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(fee));
                return new List<tblfeeAllocation>
                {
                    new tblfeeAllocation
                    {
                        Status = $"ERROR: {ex.Message}"
                    }
                };
            }
        }

        public List<tblfeeDiscountCategory> Tbl_FeeDiscountCategory_CRUD_Operations(tblfeeDiscountCategory fee)
        {
            var Routes = new List<tblfeeDiscountCategory>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                using (var cmd = new MySqlCommand("Proc_FeeDiscountCategory", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("p_SchoolID", (object?)CleanParam(fee.SchoolID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_AcademicYear", (object?)CleanParam(fee.AcademicYear) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ID", (object?)CleanParam(fee.ID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Name", (object?)CleanParam(fee.Name) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_FeeCategory", (object?)CleanParam(fee.FeeCategory) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_StartDate", (object?)CleanParam(fee.StartDate) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_EndDate", (object?)CleanParam(fee.EndDate) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_DiscountType", (object?)CleanParam(fee.DiscountType) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_MinAmountForDiscount", (object?)CleanParam(fee.MinAmountForDiscount) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_DiscountValuePerInstallment", (object?)CleanParam(fee.DiscountValuePerInstallment) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Description", (object?)CleanParam(fee.Description) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_IsActive", (object?)CleanParam(fee.IsActive) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(fee.CreatedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedIP", (object?)CleanParam(fee.CreatedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(fee.ModifiedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedIP", (object?)CleanParam(fee.ModifiedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Flag", fee.Flag ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Limit", fee.Limit ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastCreatedDate", fee.LastCreatedDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastID", fee.LastID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortColumn", fee.SortColumn ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortDirection", fee.SortDirection ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Offset", fee.Offset ?? (object)DBNull.Value);

                    conn.Open();

                    if (fee.Flag != null)
                    {
                        if (fee.Flag == "6" || fee.Flag == "8")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var routess = new tblfeeDiscountCategory
                                    {
                                        totalcount = reader["totalCount"] != DBNull.Value ? Convert.ToInt32(reader["totalCount"]) : (int?)null
                                    };

                                    Routes.Add(routess);
                                }
                            }
                        }
                        else if (fee.Flag == "2" || fee.Flag == "3" || fee.Flag == "4" || fee.Flag == "7")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var routesgs = new tblfeeDiscountCategory
                                    {
                                        //ID = reader["ID"] == DBNull.Value ? null : Convert.ToInt32(reader["ID"]),
                                        ID = reader["ID"].ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        Name = reader["Name"]?.ToString(),
                                        DiscountType = reader["DiscountType"]?.ToString(),
                                        MinAmountForDiscount = reader["MinAmountForDiscount"]?.ToString(),
                                        DiscountValuePerInstallment = reader["DiscountValuePerInstallment"]?.ToString(),
                                        //Syllabus = reader["Syllabus"]?.ToString(),
                                        //Class = reader["Class"]?.ToString(),
                                        //Divisions = reader["Divisions"]?.ToString(),
                                        FeeCategory = reader["FeeCategory"]?.ToString(),
                                        //Amount = reader["Amount"]?.ToString(),
                                        StartDate = reader["StartDate"]?.ToString(),
                                        EndDate = reader["EndDate"]?.ToString(),
                                        IsActive = reader["IsActive"].ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString(),
                                        SchoolName = reader["SchoolName"]?.ToString(),
                                        AcademicYearName = reader["AcademicYearName"]?.ToString(),
                                        //SyllabusName = reader["SyllabusName"]?.ToString(),
                                        FeeCategoryName = reader["FeeCategoryName"]?.ToString()
                                    };

                                    Routes.Add(routesgs);
                                }
                            }
                        }
                        else if (fee.Flag == "1" || fee.Flag == "5")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (reader["Message"]?.ToString() == "Fee already discount for this category")
                                    {
                                        var routesgs = new tblfeeDiscountCategory
                                        {
                                            Status = reader["Message"]?.ToString(),
                                        };

                                        Routes.Add(routesgs);
                                    }
                                    else
                                    {
                                        var routess = new tblfeeDiscountCategory
                                        {
                                            ID = reader["ID"].ToString(),
                                            SchoolID = reader["SchoolID"]?.ToString(),
                                            AcademicYear = reader["AcademicYear"]?.ToString(),
                                            Name = reader["Name"]?.ToString(),
                                            DiscountType = reader["DiscountType"]?.ToString(),
                                            MinAmountForDiscount = reader["MinAmountForDiscount"]?.ToString(),
                                            DiscountValuePerInstallment = reader["DiscountValuePerInstallment"]?.ToString(),
                                            //Syllabus = reader["Syllabus"]?.ToString(),
                                            //Class = reader["Class"]?.ToString(),
                                            //Divisions = reader["Divisions"]?.ToString(),
                                            FeeCategory = reader["FeeCategory"]?.ToString(),
                                            //Amount = reader["Amount"]?.ToString(),
                                            StartDate = reader["StartDate"]?.ToString(),
                                            EndDate = reader["EndDate"]?.ToString(),
                                            IsActive = reader["IsActive"].ToString(),
                                            CreatedBy = reader["CreatedBy"]?.ToString(),
                                            CreatedIp = reader["CreatedIp"]?.ToString(),
                                            CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                            ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                            ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                            ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                            Status = reader["Message"]?.ToString()
                                        };

                                        Routes.Add(routess);
                                    }
                                }
                            }
                        }
                        else
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var routess = new tblfeeDiscountCategory
                                    {
                                        ID = reader["ID"].ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        Name = reader["Name"]?.ToString(),
                                        DiscountType = reader["DiscountType"]?.ToString(),
                                        MinAmountForDiscount = reader["MinAmountForDiscount"]?.ToString(),
                                        DiscountValuePerInstallment = reader["DiscountValuePerInstallment"]?.ToString(),
                                        //Syllabus = reader["Syllabus"]?.ToString(),
                                        //Class = reader["Class"]?.ToString(),
                                        //Divisions = reader["Divisions"]?.ToString(),
                                        FeeCategory = reader["FeeCategory"]?.ToString(),
                                        //Amount = reader["Amount"]?.ToString(),
                                        StartDate = reader["StartDate"]?.ToString(),
                                        EndDate = reader["EndDate"]?.ToString(),
                                        IsActive = reader["IsActive"].ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString()
                                    };

                                    Routes.Add(routess);
                                }
                            }
                        }
                    }
                }

                return Routes;
            }
            catch (Exception ex)
            {
                LogException(ex, "SchoolManagementDAL", "Tbl_FeeDiscountCategory_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(fee));
                return new List<tblfeeDiscountCategory>
                {
                    new tblfeeDiscountCategory
                    {
                        Status = $"ERROR: {ex.Message}"
                    }
                };
            }
        }
        // Fee Discount
        public List<tblfeeDiscount> Tbl_FeeDiscount_CRUD_Operations(tblfeeDiscount fee)
        {
            var Routes = new List<tblfeeDiscount>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                using (var cmd = new MySqlCommand("Proc_FeeDiscount", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("p_SchoolID", (object?)CleanParam(fee.SchoolID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_AcademicYear", (object?)CleanParam(fee.AcademicYear) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ID", (object?)CleanParam(fee.ID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Class", (object?)CleanParam(fee.Class) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Division", (object?)CleanParam(fee.Division) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Student", (object?)CleanParam(fee.Student) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_DiscountCategory", (object?)CleanParam(fee.DiscountCategory) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(fee.CreatedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedIP", (object?)CleanParam(fee.CreatedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(fee.ModifiedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedIP", (object?)CleanParam(fee.ModifiedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Flag", fee.Flag ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Limit", fee.Limit ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastCreatedDate", fee.LastCreatedDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastID", fee.LastID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortColumn", fee.SortColumn ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortDirection", fee.SortDirection ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Offset", fee.Offset ?? (object)DBNull.Value);

                    conn.Open();

                    if (fee.Flag != null)
                    {
                        if (fee.Flag == "6" || fee.Flag == "8")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var routess = new tblfeeDiscount
                                    {
                                        totalcount = reader["totalCount"] != DBNull.Value ? Convert.ToInt32(reader["totalCount"]) : (int?)null
                                    };

                                    Routes.Add(routess);
                                }
                            }
                        }
                        else if (fee.Flag == "2" || fee.Flag == "3" || fee.Flag == "4" || fee.Flag == "7")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var routesgs = new tblfeeDiscount
                                    {
                                        //ID = reader["ID"] == DBNull.Value ? null : Convert.ToInt32(reader["ID"]),
                                        ID = reader["ID"].ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        Class = reader["Class"]?.ToString(),
                                        Division = reader["Division"]?.ToString(),
                                        Student = reader["Student"]?.ToString(),
                                        DiscountCategory = reader["DiscountCategory"]?.ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString(),
                                        SchoolName = reader["SchoolName"]?.ToString(),
                                        AcademicYearName = reader["AcademicYearName"]?.ToString(),
                                        FeeDiscountCategoryName = reader["FeeDiscountCategoryName"]?.ToString(),
                                        ClassName = reader["ClassName"]?.ToString(),
                                        ClassDivisionName = reader["ClassDivisionName"]?.ToString(),
                                        StudentFullName = reader["StudentFullName"]?.ToString()
                                    };

                                    Routes.Add(routesgs);
                                }
                            }
                        }
                        else if (fee.Flag == "1" || fee.Flag == "5")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (reader["Message"]?.ToString() == "Discount already assigned to this student")
                                    {
                                        var routesgs = new tblfeeDiscount
                                        {
                                            Status = reader["Message"]?.ToString(),
                                        };

                                        Routes.Add(routesgs);
                                    }
                                    else
                                    {
                                        var routess = new tblfeeDiscount
                                        {
                                            ID = reader["ID"].ToString(),
                                            SchoolID = reader["SchoolID"]?.ToString(),
                                            AcademicYear = reader["AcademicYear"]?.ToString(),
                                            Class = reader["Class"]?.ToString(),
                                            Division = reader["Division"]?.ToString(),
                                            Student = reader["Student"]?.ToString(),
                                            DiscountCategory = reader["DiscountCategory"]?.ToString(),
                                            CreatedBy = reader["CreatedBy"]?.ToString(),
                                            CreatedIp = reader["CreatedIp"]?.ToString(),
                                            CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                            ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                            ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                            ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                            Status = reader["Message"]?.ToString()
                                        };

                                        Routes.Add(routess);
                                    }
                                }
                            }
                        }
                        else
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var routess = new tblfeeDiscount
                                    {
                                        ID = reader["ID"].ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        Class = reader["Class"]?.ToString(),
                                        Division = reader["Division"]?.ToString(),
                                        Student = reader["Student"]?.ToString(),
                                        DiscountCategory = reader["DiscountCategory"]?.ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                        ModifiedIp = reader["ModifiedIp"]?.ToString(),
                                        ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                        Status = reader["Message"]?.ToString()
                                    };

                                    Routes.Add(routess);
                                }
                            }
                        }
                    }
                }

                return Routes;
            }
            catch (Exception ex)
            {
                LogException(ex, "SchoolManagementDAL", "Tbl_FeeDiscount_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(fee));
                return new List<tblfeeDiscount>
        {
            new tblfeeDiscount
            {
                Status = $"ERROR: {ex.Message}"
            }
        };
            }
        }

        public List<tblStudentTransfer> Tbl_StudentTransfer_CRUD_Operations(tblStudentTransfer fee)
        {
            var Routes = new List<tblStudentTransfer>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                using (var cmd = new MySqlCommand("Proc_Tbl_StudentTransfer", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("p_ID", (object?)CleanParam(fee.ID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SchoolID", (object?)CleanParam(fee.SchoolID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_AcademicYear", (object?)CleanParam(fee.AcademicYear) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_AdmissionNo", (object?)CleanParam(fee.AdmissionNo) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Syllabus", (object?)CleanParam(fee.Syllabus) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Class", (object?)CleanParam(fee.Class) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Division", (object?)CleanParam(fee.Division) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_TransferDate", fee.TransferDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_TransferReason", (object?)CleanParam(fee.TransferReason) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(fee.CreatedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedIP", (object?)CleanParam(fee.CreatedIp) ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(fee.ModifiedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedIP", (object?)CleanParam(fee.ModifiedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Flag", fee.Flag ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Limit", fee.Limit ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastCreatedDate", fee.LastCreatedDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastID", fee.LastID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortColumn", fee.SortColumn ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_SortDirection", fee.SortDirection ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Offset", fee.Offset ?? (object)DBNull.Value);

                    conn.Open();

                    if (fee.Flag != null)
                    {
                        if (fee.Flag == "3")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var routess = new tblStudentTransfer
                                    {
                                        totalcount = reader["totalCount"] != DBNull.Value ? Convert.ToInt32(reader["totalCount"]) : (int?)null
                                    };

                                    Routes.Add(routess);
                                }
                            }
                        }
                        else if (fee.Flag == "2" )
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var routesgs = new tblStudentTransfer
                                    {
                                        ID = reader["ID"].ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        Class = reader["Class"]?.ToString(),
                                        Division = reader["Division"]?.ToString(),
                                        AdmissionNo = reader["AdmissionNo"]?.ToString(),
                                        TransferDate = reader["TransferDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["TransferDate"]),
                                        TransferReason = reader["TransferReason"]?.ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                        Status = reader["Message"]?.ToString()
                                    };

                                    Routes.Add(routesgs);
                                }
                            }
                        }
                        else if (fee.Flag == "1")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (reader["Message"]?.ToString() == "Cannot transfer: Fee dues pending")
                                    {
                                        var routesgs = new tblStudentTransfer
                                        {
                                            Status = reader["Message"]?.ToString(),
                                        };

                                        Routes.Add(routesgs);
                                    }
                                    else
                                    {
                                        var routess = new tblStudentTransfer
                                        {
                                            ID = reader["ID"].ToString(),
                                            SchoolID = reader["SchoolID"]?.ToString(),
                                            AcademicYear = reader["AcademicYear"]?.ToString(),
                                            Class = reader["Class"]?.ToString(),
                                            Division = reader["Division"]?.ToString(),
                                            AdmissionNo = reader["AdmissionNo"]?.ToString(),
                                            TransferDate = reader["TransferDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["TransferDate"]),
                                            TransferReason = reader["TransferReason"]?.ToString(),
                                            CreatedBy = reader["CreatedBy"]?.ToString(),
                                            CreatedIp = reader["CreatedIp"]?.ToString(),
                                            CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                            
                                            Status = reader["Message"]?.ToString()
                                        };

                                        Routes.Add(routess);
                                    }
                                }
                            }
                        }
                        else
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var routess = new tblStudentTransfer
                                    {
                                        ID = reader["ID"].ToString(),
                                        SchoolID = reader["SchoolID"]?.ToString(),
                                        AcademicYear = reader["AcademicYear"]?.ToString(),
                                        Class = reader["Class"]?.ToString(),
                                        Division = reader["Division"]?.ToString(),
                                        AdmissionNo = reader["AdmissionNo"]?.ToString(),
                                        TransferDate = reader["TransferDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["TransferDate"]),
                                        TransferReason = reader["TransferReason"]?.ToString(),
                                        CreatedBy = reader["CreatedBy"]?.ToString(),
                                        CreatedIp = reader["CreatedIp"]?.ToString(),
                                        CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),

                                        Status = reader["Message"]?.ToString()
                                    };

                                    Routes.Add(routess);
                                }
                            }
                        }
                    }
                }

                return Routes;
            }
            catch (Exception ex)
            {
                LogException(ex, "SchoolManagementDAL", "Tbl_StudentTransfer_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(fee));
                return new List<tblStudentTransfer>
                {
                    new tblStudentTransfer
                    {
                        Status = $"ERROR: {ex.Message}"
                    }
                };
            }
        }

        //Dashboard
        //public List<DashboardDataDetails> Proc_DashboardData_DAL(DashboardDataDetails fee)
        //{
        //    var resultList = new List<DashboardDataDetails>();

        //    string CleanParam(string? value)
        //    {
        //        return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string"
        //            ? null
        //            : value;
        //    }

        //    try
        //    {
        //        using (var conn = new MySqlConnection(_connectionString))
        //        using (var cmd = new MySqlCommand("Proc_DashboardData", conn))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;

        //            cmd.Parameters.AddWithValue("p_SchoolID", (object?)CleanParam(fee.SchoolID) ?? DBNull.Value);
        //            cmd.Parameters.AddWithValue("p_AcademicYear", (object?)CleanParam(fee.AcademicYear) ?? DBNull.Value);
        //            cmd.Parameters.AddWithValue("p_Flag", fee.Flag ?? (object)DBNull.Value);

        //            conn.Open();


        public DashboardResponse GetDashboardData(DashboardRequest req)
        {

            DashboardResponse response = new DashboardResponse();

            using (var conn = new MySqlConnection(_connectionString))
            {

                using (var cmd = new MySqlCommand("Proc_DashboardData", conn))
                {

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("p_SchoolID", req.SchoolID);
                    cmd.Parameters.AddWithValue("p_AcademicYear", req.AcademicYear);
                    cmd.Parameters.AddWithValue("p_ClassID", req.ClassID);
                    cmd.Parameters.AddWithValue("p_DivisionID", req.DivisionID);

                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {

                        response.counts = new DashboardCounts();

                        if (reader.Read())
                        {

                            response.counts.ClassCount = Convert.ToInt32(reader["ClassCount"]);
                            response.counts.DivisionsCount = Convert.ToInt32(reader["DivisionsCount"]);
                            response.counts.StudentsCount = Convert.ToInt32(reader["StudentsCount"]);
                            response.counts.StaffCount = Convert.ToInt32(reader["StaffCount"]);

                        }


                        /* student chart */

                        reader.NextResult();

                        response.studentChart = new List<StudentChart>();

                        while (reader.Read())
                        {

                            response.studentChart.Add(new StudentChart
                            {

                                Name = reader["Name"].ToString(),
                                StudentCount = Convert.ToInt32(reader["StudentCount"])

                            });

                        }


                        /* staff chart */

                        reader.NextResult();

                        response.staffChart = new List<StaffChart>();

                        while (reader.Read())
                        {

                            response.staffChart.Add(new StaffChart
                            {

                                StaffType = reader["StaffType"].ToString(),
                                Count = Convert.ToInt32(reader["Count"])

                            });

                        }


                        /* attendance */

                        reader.NextResult();

                        response.attendance = new List<AttendanceChart>();

                        while (reader.Read())
                        {

                            response.attendance.Add(new AttendanceChart
                            {

                                Month = reader["Month"].ToString(),
                                Attendance = Convert.ToDouble(reader["Attendance"])

                            });

                        }


                        /* fees */

                        reader.NextResult();

                        response.fees = new List<FeeChart>();

                        while (reader.Read())
                        {

                            response.fees.Add(new FeeChart
                            {

                                Month = reader["Month"].ToString(),
                                Amount = Convert.ToDouble(reader["Amount"])

                            });

                        }

                    }

                }

            }

            return response;

        }
    }

}