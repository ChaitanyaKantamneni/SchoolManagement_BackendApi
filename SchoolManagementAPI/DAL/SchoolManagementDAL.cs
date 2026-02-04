using MySqlConnector;
using Newtonsoft.Json;
using SchoolManagementAPI.Models;
using System.Data;
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
                    else // Fetch / Insert / Update / Search flags
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
                cmd.Parameters.AddWithValue("p_IsActive", user.IsActive.HasValue ? (object)(user.IsActive.Value ? 1 : 0) : DBNull.Value);
                cmd.Parameters.AddWithValue("p_Flag", Clean(user.Flag) ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_OldPassword", Clean(user.OldPassword) ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_NewPassword", Clean(user.NewPassword) ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_FileName", Clean(user.FileName) ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_FilePath", Clean(user.FilePath) ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_Limit", user.Limit ?? 100);
                cmd.Parameters.AddWithValue("p_Offset", user.Offset ?? 0);

                conn.Open();

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
                        IsActive = reader["IsActive"] == DBNull.Value ? null : Convert.ToBoolean(reader["IsActive"]),
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

        public void InsertUserToken(string email,string accessToken,string refreshToken,DateTime accessExpiryUtc,DateTime refreshExpiryUtc)
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
                        else if (academicYear.Flag == "2" || academicYear.Flag == "3" || academicYear.Flag == "7")
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
                        else if (syllabus.Flag == "2" || syllabus.Flag == "3" || syllabus.Flag == "7")
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var Syllabus = new tblSyllabus
                                    {
                                        //ID = reader["ID"] == DBNull.Value ? null : Convert.ToInt32(reader["ID"]),
                                        ID = reader["ID"] == DBNull.Value ? null : Convert.ToInt32(reader["ID"]),
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
                return new List<tblModules>
                {
                    new tblModules
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

                    conn.Open();

                    if (Class.Flag != null)
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

                    conn.Open();

                    if (classDivision.Flag != null)
                    {
                        if (classDivision.Flag == "6")
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
                        else
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var ClassDivision = new tblClassDivision
                                    {
                                        ID = reader["ID"]?.ToString(),
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

                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var staffRecord = new tblStaff
                            {
                                ID = reader["ID"]?.ToString(),
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

                return StaffList;
            }
            catch (Exception ex)
            {
                return new List<tblStaff>
        {
            new tblStaff
            {
                Status = $"ERROR: {ex.Message}"
            }
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
                        if (reader.Read())
                        {
                            roles.Add(new tblRoles
                            {
                                ID = reader["NewID"]?.ToString(),
                                Status = reader["Message"]?.ToString()
                            });
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

                cmd.Parameters.AddWithValue("p_ID", (object?)CleanParam(page.ID) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModuleID", (object?)CleanParam(page.ModuleID) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_PageName", (object?)CleanParam(page.PageName) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_IsActive", (object?)CleanParam(page.IsActive) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(page.CreatedBy) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_CreatedIp", (object?)CleanParam(page.CreatedIp) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_CreatedDate", page.CreatedDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(page.ModifiedBy) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModifiedIp", (object?)CleanParam(page.ModifiedIp) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModifiedDate", page.ModifiedDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_Flag", (object?)CleanParam(page.Flag) ?? DBNull.Value);

                conn.Open();
                if (page.Flag != null)
                {
                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        pages.Add(new tblPages
                        {
                            ID = reader["ID"]?.ToString(),
                            ModuleID = reader["ModuleID"]?.ToString(),
                            PageName = reader["PageName"]?.ToString(),
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
                return pages;
            }
            catch (Exception ex)
            {
                return new List<tblPages> { new tblPages { Status = $"ERROR: {ex.Message}" } };
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

                conn.Open();
                if (subject.Flag != null)
                {
                    if (subject.Flag == "6")
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var ClassDivision = new tblSubjects
                                {
                                    SNo = reader["SNo"]?.ToString(),
                                    SyllabusClassName = reader["SyllabusClassName"]?.ToString(),
                                    Status = reader["Message"]?.ToString()
                                };

                                Subjects.Add(ClassDivision);
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
                return new List<tblSubjects> { new tblSubjects { Status = $"ERROR: {ex.Message}" } };
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

                conn.Open();
                if (subjectStaff.Flag != null)
                {
                    using var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        SubjectStaffs.Add(new tblSubjectStaff
                        {
                            ID = reader["ID"]?.ToString(),
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
                return SubjectStaffs;
            }
            catch (Exception ex)
            {
                return new List<tblSubjectStaff> { new tblSubjectStaff { Status = $"ERROR: {ex.Message}" } };
            }
        }

        public List<tblAdmission> Tbl_Admission_CRUD_Operations(tblAdmission admission)
        {
            var Admissions = new List<tblAdmission>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {
                using var conn = new MySqlConnection(_connectionString);
                using var cmd = new MySqlCommand("Proc_Tbl_Admission", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Add parameters based on the tblAdmission class
                cmd.Parameters.AddWithValue("p_ID", (object?)CleanParam(admission.ID) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_AdmissionNo", (object?)CleanParam(admission.AdmissionNo) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_AcadamicYear", (object?)admission.AcadamicYear ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Class", (object?)CleanParam(admission.Class) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Division", (object?)CleanParam(admission.Division) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_DateOfJoin", (object?)admission.DateOfJoin ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_FirstName", (object?)CleanParam(admission.FirstName) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_MiddleName", (object?)CleanParam(admission.MiddleName) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_LastName", (object?)CleanParam(admission.LastName) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_AadharNo", (object?)CleanParam(admission.AadharNo) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_StudentMobileNo", (object?)CleanParam(admission.StudentMobileNo) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_StudentEmail", (object?)CleanParam(admission.StudentEmail) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_DOB", (object?)admission.DOB ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Gender", (object?)CleanParam(admission.Gender) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_BloodGroup", (object?)CleanParam(admission.BloodGroup) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Nationality", (object?)CleanParam(admission.Nationality) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Religion", (object?)CleanParam(admission.Religion) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Caste", (object?)CleanParam(admission.Caste) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_DocumentDetails", (object?)CleanParam(admission.DocumentDetails) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_FatherName", (object?)CleanParam(admission.FatherName) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_FatherQualification", (object?)CleanParam(admission.FatherQualification) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_FatherOccupation", (object?)CleanParam(admission.FatherOccupation) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_FatherMobile", (object?)CleanParam(admission.FatherMobile) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_FatherEmail", (object?)CleanParam(admission.FatherEmail) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_MotherName", (object?)CleanParam(admission.MotherName) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_MotherQualification", (object?)CleanParam(admission.MotherQualification) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_MotherOccupation", (object?)CleanParam(admission.MotherOccupation) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_MotherMobile", (object?)CleanParam(admission.MotherMobile) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_MotherEmail", (object?)CleanParam(admission.MotherEmail) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_IsActive", (object?)CleanParam(admission.IsActive) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(admission.CreatedBy) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_CreatedIp", (object?)CleanParam(admission.CreatedIp) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(admission.ModifiedBy) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModifiedIp", (object?)CleanParam(admission.ModifiedIp) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Flag", (object?)CleanParam(admission.Flag) ?? DBNull.Value);

                conn.Open();
                if (admission.Flag != null)
                {
                    using var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Admissions.Add(new tblAdmission
                        {
                            ID = reader["ID"]?.ToString(),
                            AdmissionNo = reader["AdmissionNo"]?.ToString(),
                            AcadamicYear = reader["AcadamicYear"] == DBNull.Value ? null : Convert.ToDateTime(reader["AcadamicYear"]),
                            Class = reader["Class"]?.ToString(),
                            Division = reader["Division"]?.ToString(),
                            DateOfJoin = reader["DateOfJoin"] == DBNull.Value ? null : Convert.ToDateTime(reader["DateOfJoin"]),
                            FirstName = reader["FirstName"]?.ToString(),
                            MiddleName = reader["MiddleName"]?.ToString(),
                            LastName = reader["LastName"]?.ToString(),
                            AadharNo = reader["AadharNo"]?.ToString(),
                            StudentMobileNo = reader["StudentMobileNo"]?.ToString(),
                            StudentEmail = reader["StudentEmail"]?.ToString(),
                            DOB = reader["DOB"] == DBNull.Value ? null : Convert.ToDateTime(reader["DOB"]),
                            Gender = reader["Gender"]?.ToString(),
                            BloodGroup = reader["BloodGroup"]?.ToString(),
                            Nationality = reader["Nationality"]?.ToString(),
                            Religion = reader["Religion"]?.ToString(),
                            Caste = reader["Caste"]?.ToString(),
                            DocumentDetails = reader["DocumentDetails"]?.ToString(),
                            FatherName = reader["FatherName"]?.ToString(),
                            FatherQualification = reader["FatherQualification"]?.ToString(),
                            FatherOccupation = reader["FatherOccupation"]?.ToString(),
                            FatherMobile = reader["FatherMobile"]?.ToString(),
                            FatherEmail = reader["FatherEmail"]?.ToString(),
                            MotherName = reader["MotherName"]?.ToString(),
                            MotherQualification = reader["MotherQualification"]?.ToString(),
                            MotherOccupation = reader["MotherOccupation"]?.ToString(),
                            MotherMobile = reader["MotherMobile"]?.ToString(),
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
                return Admissions;
            }
            catch (Exception ex)
            {
                return new List<tblAdmission> { new tblAdmission { Status = $"ERROR: {ex.Message}" } };
                //sample changed by anilKuamar
            }
        }

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
                        else if (bus.Flag == "2" || bus.Flag == "3" || bus.Flag == "7")
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
                                        EveningStartTime = reader["EveningStartTime"] == DBNull.Value? (TimeSpan?)null: (TimeSpan)reader["EveningStartTime"],
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
                        else if (route.Flag == "2" || route.Flag == "3" || route.Flag == "7")
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
                LogException(ex, "SchoolManagementDAL", "Tbl_bus_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(route));
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
                LogException(ex, "SchoolManagementDAL", "Tbl_bus_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(stop));
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
                    cmd.Parameters.AddWithValue("p_Amount", (object?)CleanParam(fare.Amount) ?? DBNull.Value);
                    //cmd.Parameters.AddWithValue("p_Description", (object?)CleanParam(fare.Description) ?? DBNull.Value);
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
                        else if (fare.Flag == "2" || fare.Flag == "3" || fare.Flag == "7" || fare.Flag == "4")
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
                                        Amount = reader["Amount"]?.ToString(),
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
                                        Amount = reader["Amount"]?.ToString(),
                                        //Description = reader["Description"]?.ToString(),
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
                LogException(ex, "SchoolManagementDAL", "Tbl_bus_CRUD_Operations", Newtonsoft.Json.JsonConvert.SerializeObject(fare));
                return new List<tblFare>
                {
                    new tblFare
                    {
                        Status = $"ERROR: {ex.Message}"
                    }
                };
            }
        }


    }

}