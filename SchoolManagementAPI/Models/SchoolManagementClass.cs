using MySqlConnector;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace SchoolManagementAPI.Models
{
    public class SchoolManagementClass
    {
    }

    //public class SchoolDetails
    //{
    //    public string? ID { get; set; }
    //    public string? Name { get; set; }
    //    public string? MobileNo { get; set; }
    //    public string? Email { get; set; }
    //    public string? Website { get; set; }
    //    public string? Address { get; set; }
    //    public string? CreatedBy { get; set; }
    //    public string? CreatedIP { get; set; }
    //    public DateTime? CreatedDate { get; set; }
    //    public string? ModifiedBy { get; set; }
    //    public string? ModifiedIP { get; set; }
    //    public DateTime? ModifiedDate { get; set; }
    //    public string? Flag { get; set; }
    //    public string? Status { get; set; }
    //    public int? Limit { get; set; }
    //    public DateTime? LastCreatedDate { get; set; }
    //    public int? LastID { get; set; }
    //    public int? totalcount { get; set; }
    //    public string? SortColumn { get; set; }
    //    public string? SortDirection { get; set; }
    //    public int? Offset { get; set; }
    //}

    public class SchoolDetails
    {
        public string? SchoolID { get; set; }
        public string? ID { get; set; }
        public string? Name { get; set; }
        public string? MobileNo { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? Address { get; set; }
        public DateTime? AvailableFrom { get; set; }
        public string? Description { get; set; }
        public string? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedIP { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public string? ModifiedIP { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? Flag { get; set; }
        public string? Status { get; set; }
        public int? Limit { get; set; }
        public DateTime? LastCreatedDate { get; set; }
        public int? LastID { get; set; }
        public int? totalcount { get; set; }
        public string? SortColumn { get; set; }
        public string? SortDirection { get; set; }
        public int? Offset { get; set; }
    }

    //public class tblUsers
    //{
    //    public string? SchoolID { get; set; }
    //    public string? FirstName { get; set; }
    //    public string? LastName { get; set; }
    //    public string? Email { get; set; }
    //    public string? MobileNo { get; set; }
    //    public string? Password { get; set; }
    //    public string? RollId { get; set; }
    //    public string? IsActive { get; set; }
    //    public string? CreatedBy { get; set; }
    //    public string? CreatedIP { get; set; }
    //    public DateTime? CreatedDate { get; set; }
    //    public string? ModifiedBy { get; set; }
    //    public string? ModifiedIP { get; set; }
    //    public DateTime? ModifiedDate { get; set; }        
    //    public string? Flag { get; set; }
    //    public string? OldPassword { get; set; }
    //    public string? NewPassword { get; set; }
    //    public string? Status { get; set; }
    //    public string? FileName { get; set; }
    //    public string? FilePath { get; set; }
    //    public string? AccessToken { get; set; }


        //}

    public class TblUser
    {
        // ===== Table Columns =====

        public long ID { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? MobileNo { get; set; }

        public string? Password { get; set; }

        public string? RollId { get; set; }

        public string? SchoolID { get; set; }

        public string? IsActive { get; set; }

        public string? CreatedBy { get; set; }

        public string? CreatedIP { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? ModifiedBy { get; set; }

        public string? ModifiedIP { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string? FileName { get; set; }

        public string? FilePath { get; set; }


        // ===== Stored Procedure Only Parameters =====

        public string? Flag { get; set; }

        public string? OldPassword { get; set; }

        public string? NewPassword { get; set; }

        public int? Limit { get; set; }

        public int? Offset { get; set; }


        // ===== API / Response Only =====

        public string? Status { get; set; }

        public string? Message { get; set; }

        public string? AccessToken { get; set; }
        public string? SchoolName { get; set; }
    }

    public class UserOTP
    {
        public string Email { get; set; }
        public string OTP { get; set; }
        public DateTime ExpiryTime { get; set; }
    }

    public class SendOtpRequest
    {
        public string Email { get; set; }
    }

    public class VerifyOtpRequest
    {
        public string Email { get; set; }
        public string OTP { get; set; }
        public string NewPassword { get; set; }
    }

    public class tblAcademicYear
    {
        public string? SchoolID { get; set; }
        public string? ID { get; set; }
        public string? Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        public string? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedIp { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public string? ModifiedIp { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? Flag { get; set; }
        public string? Status { get; set; }
        public int? Limit { get; set; }
        public DateTime? LastCreatedDate { get; set; }
        public int? LastID { get; set; }
        public int? totalcount { get; set; }
        public string? SortColumn { get; set; }
        public string? SortDirection { get; set; }
        public int? Offset { get; set; }
        public string? SchoolName { get; set; }        
    }

    public class tblSyllabus
    {
        public int? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }
        public string? Name { get; set; }
        public DateTime? AvailableFrom { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedIp { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public string? ModifiedIp { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [NotMapped]
        public string? Flag { get; set; }
        [NotMapped]
        public string? Status { get; set; }
        [NotMapped]
        public int? Limit { get; set; }
        [NotMapped]
        public DateTime? LastCreatedDate { get; set; }
        [NotMapped]
        public int? LastID { get; set; }
        [NotMapped]
        public int? totalcount { get; set; }
        [NotMapped]
        public string? SortColumn { get; set; }
        [NotMapped]
        public string? SortDirection { get; set; }
        [NotMapped]
        public int? Offset { get; set; }
        public string? SchoolName { get; set; }
        public string? AcademicYearName { get; set; }
    }

    public class ExportSyllabusRequest
    {
        public string Flag { get; set; } = "2";
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }
        public string? Name { get; set; }
    }

    public class tblModules
    {
        public string? ID { get; set; }
        public string? ModuleName { get; set; }
        public string? Description { get; set; }
        public string? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedIp { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public string? ModifiedIp { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? Flag { get; set; }
        public string? Status { get; set; }
        public int? Limit { get; set; }
        public int? Offset { get; set; }
        public DateTime? LastCreatedDate { get; set; }
        public int? LastID { get; set; }
        public string? SortColumn { get; set; }
        public string? SortDirection { get; set; }
        public int? totalCount { get; set; }
        [NotMapped]
        public List<tblPages>? Pages { get; set; }
    }

    public class tblPages
    {
        public string? ID { get; set; }
        public string? ModuleID { get; set; }
        public string? PageName { get; set; }
        public string? Description { get; set; }
        public string? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedIp { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public string? ModifiedIp { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [NotMapped]
        public string? Flag { get; set; }
        public string? ModuleName { get; set; }
        [NotMapped]
        public string? Status { get; set; }
        [NotMapped]
        public int? Limit { get; set; }
        [NotMapped]
        public int? Offset { get; set; }
        [NotMapped]
        public DateTime? LastCreatedDate { get; set; }
        [NotMapped]
        public int? LastID { get; set; }
        [NotMapped]
        public int? TotalCount { get; set; }
        [NotMapped]
        public string? SortColumn { get; set; }
        [NotMapped]
        public string? SortDirection { get; set; }
        [NotMapped]
        public string? CanView { get; set; }
        [NotMapped]
        public string? CanAdd { get; set; }
        [NotMapped]
        public string? CanEdit { get; set; }
        [NotMapped]
        public string? CanDelete { get; set; }
    }


    public class tblClass
    {
        public string? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }
        public string? Name { get; set; }
        public string? Syllabus { get; set; }
        public string? Description { get; set; }
        public string? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedIp { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public string? ModifiedIp { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? Flag { get; set; }
        public string? Status { get; set; }
        public int? Limit { get; set; }
        public DateTime? LastCreatedDate { get; set; }
        public int? LastID { get; set; }
        public int? totalcount { get; set; }
        public string? SortColumn { get; set; }
        public string? SortDirection { get; set; }
        public int? Offset { get; set; }
        public string? SchoolName { get; set; }
        public string? AcademicYearName { get; set; }
        public string? SyllabusName { get; set; }
    }

    public class tblClassDivision
    {
        public string? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }
        public string? Class { get; set; }
        public string? Name { get; set; }
        public string? Strength { get; set; }
        public string? Description { get; set; }
        public string? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedIp { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public string? ModifiedIp { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? Flag { get; set; }
        public string? Status { get; set; }
        public string? SNo { get; set; }
        public string? SyllabusClassName { get; set; }
        public int? Limit { get; set; }
        public DateTime? LastCreatedDate { get; set; }
        public int? LastID { get; set; }
        public int? totalcount { get; set; }
        public string? SortColumn { get; set; }
        public string? SortDirection { get; set; }
        public int? Offset { get; set; }
        public string? SchoolName { get; set; }
        public string? AcademicYearName { get; set; }
        public string? ClassName { get; set; }
    }

    public class tblStaff
    {
        public string? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }
        public string? StaffType { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? MobileNumber { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Qualification { get; set; }
        public string? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedIp { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public string? ModifiedIp { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [NotMapped]
        public string? Flag { get; set; }
        [NotMapped]
        public string? Status { get; set; }
        [NotMapped]
        public int? Limit { get; set; }
        [NotMapped]
        public int? Offset { get; set; }
        [NotMapped]
        public DateTime? LastCreatedDate { get; set; }
        [NotMapped]
        public int? LastID { get; set; }
        [NotMapped]
        public int? TotalCount { get; set; }
        [NotMapped]
        public string? SortColumn { get; set; }
        [NotMapped]
        public string? SortDirection { get; set; }
        public string? SchoolName { get; set; }
        public string? AcademicYearName { get; set; }
    }

    public class UserToken
    {
        public string Email { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime AccessExpiry { get; set; }
        public DateTime RefreshExpiry { get; set; }
    }

    public class RefreshTokenRequest
    {
        public string Email { get; set; }
        public string RefreshToken { get; set; }
    }

    public class tblRoles
    {
        public string? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? RoleName { get; set; }
        public string? Description { get; set; }
        public string? IsActive { get; set; }
        public string? SchoolName { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedIp { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public string? ModifiedIp { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? Flag { get; set; }
        public string? Status { get; set; }
        [NotMapped]
        public int? Limit { get; set; }
        [NotMapped]
        public int? Offset { get; set; }
        [NotMapped]
        public DateTime? LastCreatedDate { get; set; }
        [NotMapped]
        public int? LastID { get; set; }
        [NotMapped]
        public int? TotalCount { get; set; }
        [NotMapped]
        public string? SortColumn { get; set; }
        [NotMapped]
        public string? SortDirection { get; set; }
    }

    public class tblUserRoles
    {
        public string? UserID { get; set; }
        public string? RoleID { get; set; }
        public string? Flag { get; set; }
        public string? Status { get; set; }
    }

    public class tblRolePermissions
    {
        public string? RoleID { get; set; }
        public string? PageID { get; set; }
        public string? CanView { get; set; }
        public string? CanAdd { get; set; }
        public string? CanEdit { get; set; }
        public string? CanDelete { get; set; }
        public string? Flag { get; set; }
        public string? Status { get; set; }
    }

    public class tblSubjects
    {
        public string? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }
        public string? Class { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Topics { get; set; }
        public string? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedIp { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public string? ModifiedIp { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? Flag { get; set; }
        public string? Status { get; set; }
        public string? SNo { get; set; }
        public string? SyllabusClassName { get; set; }
        public int? Limit { get; set; }
        public DateTime? LastCreatedDate { get; set; }
        public int? LastID { get; set; }
        public int? totalcount { get; set; }
        public string? SortColumn { get; set; }
        public string? SortDirection { get; set; }
        public int? Offset { get; set; }
        public string? SchoolName { get; set; }
        public string? AcademicYearName { get; set; }
        public string? ClassName { get; set; }
    }

    public class tblSubjectStaff
    {
        public string? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }
        public string? Class { get; set; }
        public string? StaffName { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedIp { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public string? ModifiedIp { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? Flag { get; set; }
        public string? Status { get; set; }
        public int? Limit { get; set; }
        public DateTime? LastCreatedDate { get; set; }
        public int? LastID { get; set; }
        public int? totalcount { get; set; }
        public string? SortColumn { get; set; }
        public string? SortDirection { get; set; }
        public int? Offset { get; set; }
        public string? SchoolName { get; set; }
        public string? AcademicYearName { get; set; }
        public string? ClassName { get; set; }
    }

    public class tblAdmission
    {
        public string? ID { get; set; }
        public string? AdmissionNo { get; set; }
        public DateTime? AcadamicYear { get; set; }
        public string? Class { get; set; }
        public string? Division { get; set; }
        public DateTime? DateOfJoin { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? AadharNo { get; set; }
        public string? StudentMobileNo { get; set; }
        public string? StudentEmail { get; set; }
        public DateTime? DOB { get; set; }
        public string? Gender { get; set; }
        public string? BloodGroup { get; set; }
        public string? Nationality { get; set; }
        public string? Religion { get; set; }
        public string? Caste { get; set; }
        public string? DocumentDetails { get; set; }
        public string? FatherName { get; set; }
        public string? FatherQualification { get; set; }
        public string? FatherOccupation { get; set; }
        public string? FatherMobile { get; set; }
        public string? FatherEmail { get; set; }
        public string? MotherName { get; set; }
        public string? MotherQualification { get; set; }
        public string? MotherOccupation { get; set; }
        public string? MotherMobile { get; set; }
        public string? MotherEmail { get; set; }
        public string? Address { get; set; }
        public string? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedIp { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public string? ModifiedIp { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? Flag { get; set; }
        public string? Status { get; set; }
    }
}
