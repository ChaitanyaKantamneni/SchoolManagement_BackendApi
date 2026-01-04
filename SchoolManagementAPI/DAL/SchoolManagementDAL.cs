using MySqlConnector;
using SchoolManagementAPI.Models;
using System.Data;
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
        public List<tblUsers> Tbl_Users_CRUD_Operations(tblUsers user)
        {
            var USER = new List<tblUsers>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                using (var cmd = new MySqlCommand("Proc_Tbl_User", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("p_Name", (object?)CleanParam(user.FirstName) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Email", (object?)CleanParam(user.Email) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_MobileNo", (object?)CleanParam(user.MobileNo) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Password", (object?)CleanParam(user.Password) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(user.CreatedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedIP", (object?)CleanParam(user.CreatedIP) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedDate", user.CreatedDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(user.ModifiedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedIP", (object?)CleanParam(user.ModifiedIP) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedDate", user.ModifiedDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_RollId", (object?)CleanParam(user.RollId) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_LastName", (object?)CleanParam(user.LastName) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_IsActive", (object?)CleanParam(user.IsActive) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Flag", (object?)CleanParam(user.Flag) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_OldPassword", (object?)CleanParam(user.OldPassword) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_NewPassword", (object?)CleanParam(user.NewPassword) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_FileName", (object?)CleanParam(user.FileName) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_FilePath", (object?)CleanParam(user.FilePath) ?? DBNull.Value);

                    conn.Open();

                    if (user.Flag != null)
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var User = new tblUsers
                                {
                                    FirstName = reader["FirstName"]?.ToString(),
                                    Email = reader["Email"]?.ToString(),
                                    MobileNo = reader["MobileNo"]?.ToString(),
                                    Password = reader["Password"]?.ToString(),
                                    RollId = reader["RollId"]?.ToString(),
                                    LastName = reader["LastName"]?.ToString(),
                                    IsActive = reader["IsActive"]?.ToString(),
                                    CreatedBy = reader["CreatedBy"]?.ToString(),
                                    CreatedIP = reader["CreatedIP"]?.ToString(),
                                    CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                    ModifiedBy = reader["ModifiedBy"]?.ToString(),
                                    ModifiedIP = reader["ModifiedIP"]?.ToString(),
                                    ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ModifiedDate"]),
                                    FileName = reader["FileName"]?.ToString(),
                                    FilePath = reader["FilePath"]?.ToString(),
                                    Status = reader["Message"]?.ToString()
                                };

                                USER.Add(User);
                            }
                        }
                    }
                }

                return USER;
            }
            catch (Exception ex)
            {
                return new List<tblUsers>
                {
                    new tblUsers
                    {
                        Status = $"ERROR: {ex.Message}"
                    }
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

                    conn.Open();

                    if (academicYear.Flag != null)
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var Academicyear = new tblAcademicYear
                                {
                                    //ID = reader["ID"] == DBNull.Value ? null : Convert.ToInt32(reader["ID"]),
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

                return AcademicYears;
            }
            catch (Exception ex)
            {
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

                    cmd.Parameters.AddWithValue("p_ID", (object?)CleanParam(syllabus.ID) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Name", (object?)CleanParam(syllabus.Name) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_AvailableFrom", syllabus.AvailableFrom ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_Description", (object?)CleanParam(syllabus.Description) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_IsActive", (object?)CleanParam(syllabus.IsActive) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(syllabus.CreatedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_CreatedIP", (object?)CleanParam(syllabus.CreatedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(syllabus.ModifiedBy) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ModifiedIP", (object?)CleanParam(syllabus.ModifiedIp) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_flag", (object?)CleanParam(syllabus.Flag) ?? DBNull.Value);

                    conn.Open();

                    if (syllabus.Flag != null)
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var Syllabus = new tblSyllabus
                                {
                                    //ID = reader["ID"] == DBNull.Value ? null : Convert.ToInt32(reader["ID"]),
                                    ID = reader["ID"]?.ToString(),
                                    Name = reader["Name"]?.ToString(),
                                    AvailableFrom = reader["AvailableFrom"] == DBNull.Value ? null : Convert.ToDateTime(reader["AvailableFrom"]),
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

                                Syllabuses.Add(Syllabus);
                            }
                        }
                    }
                }

                return Syllabuses;
            }
            catch (Exception ex)
            {
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

        public static class DateTimeHelper
        {
            public static DateTime NowIST()
            {
                var istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, istZone);
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
                    AccessExpiry = reader["ExpiryAt"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["ExpiryAt"]),
                    RefreshExpiry = reader["RefreshExpiryAt"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["RefreshExpiryAt"])
                };
            }
            return null;
        }

        public void InsertUserToken(string email, string accessToken, string refreshToken, DateTime accessExpiry, DateTime refreshExpiry)
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
            cmd.Parameters.AddWithValue("p_issuedAt", DateTimeHelper.NowIST());
            cmd.Parameters.AddWithValue("p_expiryAt", accessExpiry);
            cmd.Parameters.AddWithValue("p_refreshExpiryAt", refreshExpiry);

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
                cmd.Parameters.AddWithValue("p_RoleName", (object?)CleanParam(role.RoleName) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Description", (object?)CleanParam(role.Description) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_IsActive", (object?)CleanParam(role.IsActive) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(role.CreatedBy) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_CreatedIp", (object?)CleanParam(role.CreatedIp) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_CreatedDate", role.CreatedDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(role.ModifiedBy) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModifiedIp", (object?)CleanParam(role.ModifiedIp) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModifiedDate", role.ModifiedDate ?? (object)DBNull.Value);
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

        public List<tblModules> Tbl_Modules_CRUD_Operations(tblModules module)
        {
            var modules = new List<tblModules>();

            string CleanParam(string? value)
            {
                return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string" ? null : value;
            }

            try
            {
                using var conn = new MySqlConnection(_connectionString);
                using var cmd = new MySqlCommand("Proc_Tbl_Module", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("p_ID", (object?)CleanParam(module.ID) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModuleName", (object?)CleanParam(module.ModuleName) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Description", (object?)CleanParam(module.Description) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_IsActive", (object?)CleanParam(module.IsActive) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_CreatedBy", (object?)CleanParam(module.CreatedBy) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_CreatedIp", (object?)CleanParam(module.CreatedIp) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_CreatedDate", module.CreatedDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModifiedBy", (object?)CleanParam(module.ModifiedBy) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModifiedIp", (object?)CleanParam(module.ModifiedIp) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_ModifiedDate", module.ModifiedDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_Flag", (object?)CleanParam(module.Flag) ?? DBNull.Value);

                conn.Open();
                if (module.Flag != null)
                {
                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        modules.Add(new tblModules
                        {
                            ID = reader["ID"]?.ToString(),
                            ModuleName = reader["ModuleName"]?.ToString(),
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
                return modules;
            }
            catch (Exception ex)
            {
                return new List<tblModules> { new tblModules { Status = $"ERROR: {ex.Message}" } };
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

    }
}