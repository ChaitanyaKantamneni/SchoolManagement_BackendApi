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
        public int? inactiveCount { get; set; }
        public int? activeCount { get; set; }
    }

    public class TblUser
    {
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
        public string? NewPassword { get; set; }
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
        public int? inactiveCount { get; set; }
        public int? activeCount { get; set; }
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
        [NotMapped]
        public int? inactiveCount { get; set; }
        [NotMapped]
        public int? activeCount { get; set; }
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
        public int? inactiveCount { get; set; }
        public int? activeCount { get; set; }
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
        public int? inactiveCount { get; set; }
        public int? activeCount { get; set; }
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
        public string? SubjectID { get; set; }
        public string? ClassID { get; set; }
        public string? SyllabusID { get; set; }
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
        public string? StaffFullName { get; set; }
        public string? IsActive { get; set; }
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
        public string? SubjectID { get; set; }
        public string? ClassID { get; set; }
        public string? Name { get; set; }
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



    public class tblStudentDetails
    {
        public string? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }
        public string? AdmissionNo { get; set; }
        public string? Syllabus { get; set; }
        public string? Class { get; set; }
        public string? Division { get; set; }
        public DateTime? JoinDate { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? AadharNo { get; set; }
        public string? MobileNo { get; set; }
        public string? EmailID { get; set; }
        public DateTime? DOB { get; set; }
        public string? Gender { get; set; }
        public string? BloodGroup { get; set; }
        public string? Nationality { get; set; }
        public string? Religion { get; set; }
        public string? Caste { get; set; }
        public string? DocumentDetails { get; set; }
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
        public int? inactiveCount { get; set; }
        public int? activeCount { get; set; }
        public string? SortColumn { get; set; }
        public string? SortDirection { get; set; }
        public int? Offset { get; set; }
        public string? NewAdmissionNo { get; set; }
        public string? SchoolName { get; set; }
        public string? AcademicYearName { get; set; }
        public string? ClassName { get; set; }
        public string? ClassDivisionName { get; set; }
        public string? DePromotionRemarks { get; set; }

    }

    public class tblStudentAddressDetails
    {
        public string? ID { get; set; }
        public string? AdmissionID { get; set; }
        public string? PermanentAddressLine1 { get; set; }
        public string? PermanentAddressLine2 { get; set; }
        public string? PermanentPinCode { get; set; }
        public string? PermanentPlace { get; set; }
        public string? PermanentCountry { get; set; }
        public string? PermanentState { get; set; }
        public string? PermanentDistrict { get; set; }
        public string? PermanentCity { get; set; }


        public string? TemporaryAddressLine1 { get; set; }
        public string? TemporaryAddressLine2 { get; set; }
        public string? TemporaryPinCode { get; set; }
        public string? TemporaryPlace { get; set; }
        public string? TemporaryCountry { get; set; }
        public string? TemporaryState { get; set; }
        public string? TemporaryDistrict { get; set; }
        public string? TemporaryCity { get; set; }


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
    }

    public class tblStudentParentDetails
    {
        public string? ID { get; set; }
        public string? AdmissionID { get; set; }
        public string? FatherName { get; set; }
        public string? Class { get; set; }
        public string? Division { get; set; }
        public string? FatherQualification { get; set; }
        public string? FatherOccupation { get; set; }
        public string? FatherContact { get; set; }
        public string? FatherAadhar { get; set; }
        public string? FatherEmail { get; set; }
        public string? MotherName { get; set; }
        public string? MotherQualification { get; set; }
        public string? MotherOccupation { get; set; }
        public string? MotherContact { get; set; }
        public string? MotherAadhar { get; set; }
        public string? MotherEmail { get; set; }
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
        public string? AcademicYear { get; set; }
        public int? Offset { get; set; }
    }

    public class tblStudentTransportationDetails
    {
        public string? ID { get; set; }
        public string? AdmissionID { get; set; }
        public string? Route { get; set; }
        public string? Stop { get; set; }
        public string? Bus { get; set; }
        public string? Fare { get; set; }
        public DateTime? StartDate { get; set; }
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
        public string? RouteName { get; set; }
        public string? StopName { get; set; }
        public string? BusName { get; set; }
        public string? FareName { get; set; }
    }
    
    public class tblStudentAcademicDetails
    {
        public string? ID { get; set; }
        public string? AdmissionID { get; set; }
        public string? Syllabus { get; set; }
        public string? Class { get; set; }
        public string? Division { get; set; }
        public DateTime? JoinDate { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }
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
    }

    
    public class tblAllotClassTeacher
    {
        public string? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }
        public string? Class { get; set; }
        public string? Division { get; set; }
        public string? ClassTeacher { get; set; }
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
        public string? ClassName { get; set; }
        public string? StaffName { get; set; }
        public string? DivisionName { get; set; }
    }
    public class tblBus
    {
        public string? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }
        public string? Name { get; set; }
        public string? RegNo { get; set; }
        public string? Driver { get; set; }
        public string? AssistantName { get; set; }
        public string? AssistantMobNo { get; set; }

        public string? OtherDetails { get; set; }
        public TimeSpan? MorningStartTime { get; set; }
        public TimeSpan? EveningStartTime { get; set; }
        public string? DistanceCostPerKM { get; set; }
        public int? MaxCapacity { get; set; }


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
    public class tblRoute
    {
        public string? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }
        public string? Name { get; set; }
        public string? Distance { get; set; }

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
    public class tblStops
    {
        public string? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }
        public string? Route { get; set; }
        public string? StopOrder { get; set; }
        public string? StopName { get; set; }

        public string? Distance { get; set; }
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
        public string? RouteName { get; set; }
    }
    public class tblFare
    {
        public string? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }
        public string? RouteID { get; set; }
        public string? StopID { get; set; }
        public string? BusID { get; set; }
        public decimal? Amount { get; set; }   // or public decimal Amount { get; set; } = 0m;  if non-nullable + default
        //public string? Distance { get; set; }
        //public string? Description { get; set; }


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
        public string? RouteName { get; set; }
        public string? StopName { get; set; }
        public string? BusName { get; set; }
    }
    public class TblWorkingDays
    {
        public string? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }
        public string? Day { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
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
        public string? RouteName { get; set; }
    }
    public class TblSession
    {
        public string? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }
        public string? Session { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
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
        public string? RouteName { get; set; }
    }
    public class TblTimeTable
    {
        public string? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }
        public string? ClassID { get; set; }
        public string? DivisionID { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int? NoOfPeriods { get; set; }
        public string? IsActive { get; set; }

        // JSON for Insert / Update
        public string? TimetableJSON { get; set; }

        // Audit
        public string? CreatedBy { get; set; }
        public string? CreatedIP { get; set; }
        public string? ModifiedBy { get; set; }
        public string? ModifiedIP { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // Fetch (Flag 4)
        public List<TblTimeTableDetail>? TimeTableDetails { get; set; }

        // Pagination
        public string? Flag { get; set; }
        public int? Limit { get; set; }
        public int? Offset { get; set; }
        public int? totalCount { get; set; }
        public string? SortColumn { get; set; }
        public string? SortDirection { get; set; }
        public string? Status { get; set; }
        public string? SchoolName { get; set; }
        public string? AcademicYearName { get; set; }
        public string? DayID { get; set; }
        public string? Day { get; set; }
        public int? PeriodNo { get; set; }
        public string? SessionID { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? TimeSlot { get; set; }
        public string? SubjectID { get; set; }
        public string? StaffID { get; set; }
        public string? TimeTableID { get; set; }
        public string? StaffName { get; set; }
        public string? StaffEmail { get; set; }
        public string? ClassName { get; set; }
        public string? DivisionName { get; set; }
        public string? SubjectName { get; set; }
    }
    public class TblTimeTableDetail
    {
        public string? ID { get; set; }
        public string? TimeTableID { get; set; }
        public string? DayID { get; set; }
        public int? PeriodNo { get; set; }
        public string? SessionID { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? SubjectID { get; set; }
        public string? StaffID { get; set; }
    }

    public class tblExamType
    {
        public string? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }
        public string? ExamTypeName { get; set; }
        public string? Priority { get; set; }
        public string? ExamType { get; set; }
        public string? MaxMark { get; set; }
        public string? PassMarks { get; set; }

        public string? ExamDuration { get; set; }
        public string? NoofQuestion { get; set; }
        public string? Instructions { get; set; }

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
    public class tblSetExam
    {
        public string? RowID { get; set; }
        public string? SubjectIndex { get; set; }
        public string? SubjectID { get; set; }
        public DateTime? SubjectExamDateAndTime { get; set; }
        public string? IndividualSubjectName { get; set; }
        public string? AttendanceMarked { get; set; }
        public string? ExamAttendancAndMarksMarked { get; set; }
        public int? IsPublished { get; set;}
        public string? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }
        public string? Syllabus { get; set; }
        public string? Class { get; set; }
        public string? Divisions { get; set; }
        public string? ExamType { get; set; }
        public string? Subjects { get; set; }
        public string? MaxMarks { get; set; }
        public string? PassMarks { get; set; }
        public string? ExamDateAndTime { get; set; }
        public string? Duration { get; set; }
        public string? NoOfQuestion { get; set; }
        public string? Instructions { get; set; }

        public string? ClassName { get; set; }
        public string? SyllabusName { get; set; }
        public string? DivisionName { get; set; }
        public string? ExamTypeName { get; set; }
        public string? SubjectName { get; set; }


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
        public string? DivisionList { get; set; }

        public string? AdmissionNo { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }

        public string? LastName { get; set; }
        public string? ClassDivisionName { get; set; }
        
                    public string? StaffID { get; set; }


    }
    public class tblExamAttendence
    {
        public string? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }

        public string? ExamID { get; set; }
        public string? SubjectID { get; set; }

        public string? Class { get; set; }
        public string? Division { get; set; }

        public string? AdmissionID { get; set; }
        public string? Attendance { get; set; }
        public string? Remarks { get; set; }
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
        public string? StudentName { get; set; }
        public string? DivisionName { get; set; }
        public string? ClassName { get; set; }

        public string? ExamName { get; set; }
        public string? SubjectName { get; set; }
        public List<AttendanceStudent>? Students { get; set; }  // received from frontend
        public string? StudentsJson { get; set; }               // serialized before SP call
    }
    public class AttendanceStudent
    {
        public string AdmissionID { get; set; }
        public string Attendance { get; set; }
        public string Remarks { get; set; }
    }
    public class tblExamMarks
    {
        public string? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }

        public string? ExamID { get; set; }
        public string? SubjectID { get; set; }

        public string? Class { get; set; }
        public string? Division { get; set; }

        public string? AdmissionID { get; set; }
        public string? Marks { get; set; }
        public string? Attendance { get; set; }



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
        public string? StudentName { get; set; }
        public string? DivisionName { get; set; }
        public string? ClassName { get; set; }

        public string? ExamName { get; set; }
        public string? SubjectName { get; set; }
        public string? SubjectMarks { get; set; }
        public string? SubjectResult { get; set; }
        public string? SubjectPercentage { get; set; }
        public string? TotalMarks { get; set; }
        public string? TotalMaxMarks { get; set; }
        public string? TotalPercentage { get; set; }
        public string? MaxMarks { get; set; }
        public string? PassMarks { get; set; }
        public List<ExamMarksStudent>? Students { get; set; }
        public string? StudentsJson { get; set; }
    }
    public class ExamMarksStudent
    {
        public string? AdmissionID { get; set; }
        public string? Marks { get; set; }
    }
    public class tblStudentAttendance
    {
        public string? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }

        public string? AttendanceDate { get; set; }
        public string? LateInMinutes { get; set; }
        public string? Class { get; set; }
        public string? Division { get; set; }
        public string? AdmissionID { get; set; }
        public string? Attendance { get; set; }
        public string? Remarks { get; set; }

        public string? Session { get; set; }
        public string? SessionName { get; set; }
        public string? LeaveType { get; set; }
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
        public string? StudentName { get; set; }
        public string? DivisionName { get; set; }
        public string? ClassName { get; set; }
        public List<StudentAttendanceStudent>? Students { get; set; }
        public string? StudentsJson { get; set; }
        public class StudentAttendanceStudent
        {
            public string? AdmissionID { get; set; }
            public string? Attendance { get; set; }
            public string? LateInMinutes { get; set; }
            public string? AttendanceDate { get; set; }  // ← ADD
            public string? Session { get; set; }          // ← ADD
            public string? Remarks { get; set; }          // ← ADD
        }
    }
    public class tblStaffAttendance
    {
        public string? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? StaffID { get; set; }
        public string? AcademicYear { get; set; }
        public string? AttendanceDate { get; set; }
        public string? Remarks { get; set; }
        public string? Role { get; set; }

        public string? LateInMinutes { get; set; }
        public string? Class { get; set; }
        public string? Division { get; set; }
        public string? AdmissionID { get; set; }
        public string? Attendance { get; set; }
        public string? Session { get; set; }
        public string? leavetype { get; set; }
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
        public string? StudentName { get; set; }
        public string? DivisionName { get; set; }
        public string? ClassName { get; set; }
        public string? StaffName { get; set; }
       
        //public string? Message { get; set; }

        // ← ADD THESE TWO
        public List<StaffAttendanceItem>? Students { get; set; }
        public string? StudentsJson { get; set; }

        public class StaffAttendanceItem
        {
            public string? StaffID { get; set; }
            public string? Attendance { get; set; }
            public string? LateInMinutes { get; set; }
            public string? leavetype { get; set; }
            // needed for Flag 5 update JOIN:
            public string? AttendanceDate { get; set; }
            public string? Session { get; set; }
            public string? Remarks { get; set; }   // add this
        }
    }
    public class feeCategory
    {
        public string? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }
        public string? FeeCategoryName { get; set; }
        public string? FeeType { get; set; }
        public string? FeeCollectionDuration { get; set; }
        public string? FeeDueDay { get; set; }
        public string? FineType { get; set; }

        public string? FineValue { get; set; }
        public string? FineCollectionType { get; set; }
        public string? FineIncrementIn { get; set; }

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

    public class tblfeeAllocation
    {
        public string? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }
        public string? Syllabus { get; set; }
        public string? Class { get; set; }
        public string? Divisions { get; set; }
        public string? FeeCategory { get; set; }

        public string? Amount { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
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
        public string? SyllabusName { get; set; }
        public string? ClassName { get; set; }
        public string? DivisionName { get; set; }
        public string? FeeCategoryName { get; set; }
    }

    public class tblfeeDiscountCategory
    {
        public string? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }
        public string? Name { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? FeeCategory { get; set; }
        public string? DiscountType { get; set; }
        public string? MinAmountForDiscount { get; set; }
        public string? DiscountValuePerInstallment { get; set; }
        public string? Description { get; set; }
        //public string? Syllabus { get; set; }
        //public string? Amount { get; set; }
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
        public string? FeeCategoryName { get; set; }
    }

    public class tblfeeDiscount
    {
        public string? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }
        public string? Class { get; set; }
        public string? Division { get; set; }
        public string? Student { get; set; }
        public string? DiscountCategory { get; set; }
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
        public string? FeeDiscountCategoryName { get; set; }
        public string? ClassName { get; set; }
        public string? ClassDivisionName { get; set; }
        public string? FeeCategoryName { get; set; }
        public string? StudentFullName { get; set; }
        public string? TotalFee { get; set; }
        public string? TotalDiscount { get; set; }
        public string? TotalPaid { get; set; }
        public string? NetPayable { get; set; }
        public string? RemainingAmount { get; set; }
        public string? NewRecordNo { get; set; }
        public string? FeeCategory { get; set; }
    }



    public class tblStudentTransfer
    {
        public string? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }
        public string? AdmissionNo { get; set; }
        public string? Syllabus { get; set; }
        public string? Class { get; set; }
        public string? Division { get; set; }
        public DateTime? TransferDate { get; set; }
        public string? TransferReason { get; set; }
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
    }

    //public class DashboardDataDetails
    //{
    //    public string? SchoolID { get; set; }
    //    public string? AcademicYear { get; set; }
    //    public string? Flag { get; set; }
    public class tblFeeCollection
    {
        public string? ID { get; set; }
        public string? ReceiptNo { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }

        public string? SchoolName { get; set; }
        public string? AcademicYearName { get; set; }
        public string? ClassName { get; set; }
        public string? DivisionName { get; set; }
        public string? StudentName { get; set; }
        public string? FeeCategoryName { get; set; }
        public string? AdmissionNo { get; set; }
        public string? PendingAmount { get; set; }
        public string? Student { get; set; }
        public string? Class { get; set; }
        public string? Division { get; set; }
        public string? FeeCategory { get; set; }
        public string? AmountPaid { get; set; }
        public string? PaymentMode { get; set; }
        public string? TransactionID { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedIp { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public string? ModifiedIp { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public string? TotalFee { get; set; }
        public string? TotalDiscount { get; set; }
        public string? TotalFeePaid { get; set; }
        public string? FeePaid { get; set; }
        public string? NetPayable { get; set; }
        public string? RemainingAmount { get; set; }

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
        public string? SortColumn { get; set; }
        [NotMapped]
        public string? SortDirection { get; set; }
        [NotMapped]
        public int? Offset { get; set; }
        [NotMapped]
        public int? totalcount { get; set; }
    }

    //public class DashboardRequest
    //{
    //    public int? SchoolID { get; set; }
    //    public int? AcademicYear { get; set; }
    //    public int? ClassID { get; set; }
    //    public int? DivisionID { get; set; }

    //}

    //public class DashboardResponse
    //{
    //    public DashboardCounts counts { get; set; }

    //    public List<StudentChart> studentChart { get; set; }

    //    public List<StaffChart> staffChart { get; set; }

    //    public List<AttendanceChart> attendance { get; set; }

    //    public List<FeeChart> fees { get; set; }

    //    public List<RecentAdmission> recentAdmissions { get; set; }

    //    public List<RecentStaff> recentStaff { get; set; }

    //    public List<Notice> notices { get; set; }
    //}

    //public class DashboardCounts
    //{
    //    public int ClassCount { get; set; }
    //    public int DivisionsCount { get; set; }
    //    public int StaffCount { get; set; }
    //    public int StudentsCount { get; set; }
    //}

    //public class StudentChart
    //{
    //    public string Name { get; set; }
    //    public int StudentCount { get; set; }
    //}

    //public class StaffChart
    //{
    //    public string StaffType { get; set; }
    //    public int Count { get; set; }
    //}

    //public class AttendanceChart
    //{
    //    public string Month { get; set; }
    //    public double Attendance { get; set; }
    //}

    //public class FeeChart
    //{
    //    public string Month { get; set; }
    //    public double Attendance { get; set; }
    //    public double Amount { get; set; }
    //}

    //public class RecentAdmission
    //{
    //    public string Name { get; set; }
    //    public string Class { get; set; }
    //    public DateTime CreatedDate { get; set; }
    //}

    //public class RecentStaff
    //{
    //    public string Name { get; set; }
    //    public DateTime JoiningDate { get; set; }
    //}

    //public class Notice
    //{
    //    public string Title { get; set; }
    //    public DateTime CreatedDate { get; set; }
    //}

    public class DashboardRequest
    {
        public int? SchoolID { get; set; }
        public int? AcademicYear { get; set; }
        public int? ClassID { get; set; }
        public int? DivisionID { get; set; }

        public int? UserID { get; set; }
        public string RoleKey { get; set; } = "school_admin";
        public int? BranchID { get; set; }
        public string DateRangeKey { get; set; } = "academic_year";
        public string CompareMode { get; set; } = "previous_period";
        public string DrillLevel { get; set; } = "network";
        public int? DrillEntityID { get; set; }
    }

    public class DashboardResponse
    {
        public List<Notice> notices { get; set; } = new();

        public DashboardCounts counts { get; set; } = new();
        public List<StudentChart> studentChart { get; set; } = new();
        public List<StaffChart> staffChart { get; set; } = new();
        public List<AttendanceChart> attendance { get; set; } = new();
        public List<FeeChart> fees { get; set; } = new();
        public List<RecentAdmission> recentAdmissions { get; set; } = new();
        public List<RecentStaff> recentStaff { get; set; } = new();

        public MiniKpis miniKpis { get; set; } = new();
        public Dictionary<string, object> roleKpis { get; set; } = new();
        public Dictionary<string, List<RoleActivityItem>> roleActivities { get; set; } = new();
        public List<DashboardAlert> alerts { get; set; } = new();
        public DashboardMeta meta { get; set; } = new();
    }

    public class DashboardCounts
    {
        public int ClassCount { get; set; }
        public int DivisionsCount { get; set; }
        public int StaffCount { get; set; }
        public int StudentsCount { get; set; }
    }

    public class StaffChart
    {
        public string StaffType { get; set; }
        public int Count { get; set; }
    }

    public class AttendanceChart
    {
        public string Month { get; set; }
        public double Attendance { get; set; }
    }

    public class StudentChart
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int StudentCount { get; set; }
    }

    public class Notice
    {
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class FeeChart
    {
        public string Month { get; set; }
        public double Attendance { get; set; }
        public double Amount { get; set; }
    }

    public class RecentAdmission
    {
        public string Name { get; set; }
        public string Class { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? JoinDate { get; set; }
    }

    public class RecentStaff
    {
        public string Name { get; set; }
        public DateTime JoiningDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string RoleName { get; set; }   // your proc returns RoleName
    }

    public class MiniKpis
    {
        public double attendancePercent { get; set; }
        public double activeUsers { get; set; }
        public double totalCollection { get; set; }  // IMPORTANT: from proc
        public double upcomingExams { get; set; }
    }

    public class RoleActivityItem
    {
        public DateTime CreatedDate { get; set; }
        public string title { get; set; }
        public string meta { get; set; }
        public DateTime? activityDate { get; set; }
    }

    public class DashboardAlert
    {
        public string severity { get; set; }
        public string title { get; set; }
        public string reasonText { get; set; }
        public string actionText { get; set; }
    }

    public class DashboardMeta
    {
        public DateTime? generatedAt { get; set; }
    }

    public class TblSalarySetting
    {
        public int? ID { get; set; }
        public int? SchoolID { get; set; }
        public int? AcademicYear { get; set; }
        public int? StaffID { get; set; }

        public string? PayHeadJson { get; set; }   // single-table JSON payload
        public string? Description { get; set; }
        public int? IsActive { get; set; }

        public int? CreatedBy { get; set; }
        public string? CreatedIp { get; set; }
        public DateTime? CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }
        public string? ModifiedIp { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public string? SchoolName { get; set; }
        public string? AcademicYearName { get; set; }
        public string? StaffName { get; set; }

        public string? Status { get; set; }

        [NotMapped] public string? Flag { get; set; }
        [NotMapped] public int? Limit { get; set; }
        [NotMapped] public int? Offset { get; set; }
        [NotMapped] public string? SearchName { get; set; }
        [NotMapped] public int? TotalCount { get; set; }
    }


    // =============================================
    // 3) MODEL: TblSalaryPay
    // =============================================
    public class TblSalaryPay
    {
        public int? ID { get; set; }
        public int? SchoolID { get; set; }
        public int? AcademicYear { get; set; }
        public int? StaffID { get; set; }
        public DateTime? PayMonth { get; set; }

        public string? PaymentMode { get; set; }
        public string? ReferenceNo { get; set; }
        public string? PayHeadJson { get; set; }

        public decimal? GrossAmount { get; set; }
        public decimal? DeductionAmount { get; set; }
        public decimal? NetAmount { get; set; }

        public string? Description { get; set; }
        public int? IsActive { get; set; }

        public int? CreatedBy { get; set; }
        public string? CreatedIp { get; set; }
        public DateTime? CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }
        public string? ModifiedIp { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public string? SchoolName { get; set; }
        public string? AcademicYearName { get; set; }
        public string? StaffName { get; set; }

        public string? Status { get; set; }

        [NotMapped] public string? Flag { get; set; }
        [NotMapped] public int? Limit { get; set; }
        [NotMapped] public int? Offset { get; set; }
        [NotMapped] public string? SearchName { get; set; }
        [NotMapped] public int? TotalCount { get; set; }
    }

    public class tblPayrollHead
    {
        public int? ID { get; set; }
        public int? SchoolID { get; set; }
        public int? AcademicYear { get; set; }

        public string? PayHeadName { get; set; }
        public string? HeadType { get; set; }
        public string? Description { get; set; }
        public int? IsActive { get; set; }

        public int? CreatedBy { get; set; }
        public string? CreatedIp { get; set; }
        public DateTime? CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }
        public string? ModifiedIp { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public string? SchoolName { get; set; }
        public string? AcademicYearName { get; set; }

        public string? Status { get; set; }

        [NotMapped] public string? Flag { get; set; }
        [NotMapped] public int? Limit { get; set; }
        [NotMapped] public int? Offset { get; set; }
        [NotMapped] public int? LastID { get; set; }
        [NotMapped] public DateTime? LastCreatedDate { get; set; }
        [NotMapped] public string? SortDirection { get; set; }
        [NotMapped] public string? SearchName { get; set; }
        [NotMapped] public int? totalcount { get; set; }
    }

    public class TblPaymentMode
    {
        public int? ID { get; set; }
        public int? SchoolID { get; set; }
        public int? AcademicYear { get; set; }

        public string? PaymentMode { get; set; }
        public string? Account { get; set; }
        public string? Description { get; set; }
        public int? IsActive { get; set; }

        public int? CreatedBy { get; set; }
        public string? CreatedIp { get; set; }
        public DateTime? CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }
        public string? ModifiedIp { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public string? SchoolName { get; set; }
        public string? AcademicYearName { get; set; }

        public string? Status { get; set; }

        [NotMapped] public string? Flag { get; set; }
        [NotMapped] public int? Limit { get; set; }
        [NotMapped] public int? Offset { get; set; }
        [NotMapped] public int? LastID { get; set; }
        [NotMapped] public DateTime? LastCreatedDate { get; set; }
        [NotMapped] public string? SortDirection { get; set; }
        [NotMapped] public string? SearchName { get; set; }
        [NotMapped] public int? totalcount { get; set; }
    }


    public class TblAdvanceSalary
    {
        public long? ID { get; set; }
        public long? SchoolID { get; set; }
        public long? AcademicYear { get; set; }
        public long? StaffID { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? AdvanceDate { get; set; }
        public int? TenureMonths { get; set; }
        public int? MonthlyDeduction { get; set; }
        public int? MonthsElapsed { get; set; }
        public int? RemainingMonths { get; set; }
        public int? DeductionForCurrentMonth { get; set; }

        public string? Description { get; set; }
        public int? IsActive { get; set; }
        public long? CreatedBy { get; set; }
        public string? CreatedIp { get; set; }
        public DateTime? CreatedDate { get; set; }
        public long? ModifiedBy { get; set; }
        public string? ModifiedIp { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? SchoolName { get; set; }
        public string? AcademicYearName { get; set; }
        public string? StaffName { get; set; }
        public string? Status { get; set; }

        public DateTime? PayMonth { get; set; }

        [NotMapped] public string? Flag { get; set; }
        [NotMapped] public int? Limit { get; set; }
        [NotMapped] public int? Offset { get; set; }
        [NotMapped] public long? LastID { get; set; }
        [NotMapped] public DateTime? LastCreatedDate { get; set; }
        [NotMapped] public string? SortDirection { get; set; }
        [NotMapped] public int? totalcount { get; set; }
    }
    public class tblLeavepolicy
    {
        public string? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }
        public string? LeaveType { get; set; }
        public string? MaxDays { get; set; }
        public string? SchoolName { get; set; }
        public string? AcademicYearName { get; set; }

        public string? Discription { get; set; }

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
    }

    //public class tblLeaveManagement
    //{
    //    public int? ID { get; set; }

    //    public string? SchoolID { get; set; }
    //    public string? AcademicYear { get; set; }

    //    public string? ApplicantID { get; set; }
    //    public int? ApplicantRoleID { get; set; }
    //    public string? ApplicantName { get; set; }

    //    public string? ClassID { get; set; }
    //    public string? DivisionID { get; set; }

    //    public string? LeaveType { get; set; }
    //    public DateTime? FromDate { get; set; }
    //    public DateTime? ToDate { get; set; }
    //    public string? Reason { get; set; }

    //    public string? CurrentApproverID { get; set; }
    //    public int? CurrentApproverRoleID { get; set; }

    //    public string? Status { get; set; }
    //    public string? ApprovalRemarks { get; set; }

    //    public int? IsActive { get; set; }

    //    public string? CreatedBy { get; set; }
    //    public DateTime? CreatedDate { get; set; }

    //    public string? ModifiedBy { get; set; }
    //    public DateTime? ModifiedDate { get; set; }

    //    // FLAGS
    //    public string? Flag { get; set; }

    //    // PAGINATION
    //    public int? Limit { get; set; }
    //    public int? Offset { get; set; }
    //    public DateTime? LastCreatedDate { get; set; }
    //    public int? LastID { get; set; }
    //    public string? SortDirection { get; set; }

    //    // SEARCH
    //    public string? Search { get; set; }

    //    // RESPONSE
    //    public int? totalCount { get; set; }
    //    public string? Message { get; set; }
    //}


public class tblLeaveApplication
    {
        public string? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }

        // Core Distinguishers
        public string? UserType { get; set; }     // 'Staff' or 'Student'
        public string? StaffID { get; set; }
        public string? AdmissionNo { get; set; }

        public string? Class { get; set; }
        public string? Division { get; set; }
        public string? LeavePolicyID { get; set; }

        // Common Leave Fields
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? NoOfDays { get; set; }
        public string? Reason { get; set; }
        public string? ApplicationStatus { get; set; }
        public string? ApprovedBy { get; set; }
        public string? AdminRemarks { get; set; }

        // Standard Auditing
        public string? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedIp { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public string? ModifiedIp { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // Pagination & Search Flags
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
        public string? SortColumn { get; set; }
        [NotMapped]
        public string? SortDirection { get; set; }
        [NotMapped]
        public int? Offset { get; set; }
        [NotMapped]
        public int? totalcount { get; set; }

        // Display Fields (Dynamic JOINs)
        [NotMapped]
        public string? ApplicantName { get; set; }
        [NotMapped]
        public string? SchoolName { get; set; }
        [NotMapped]
        public string? AcademicYearName { get; set; }
        [NotMapped]
        public string? LeaveType { get; set; }
        [NotMapped]
        public string? ClassName { get; set; }
        [NotMapped]
        public string? DivisionName { get; set; }
        [NotMapped]
        public string? ApprovedByName { get; set; }
        [NotMapped]
        public string? UsedOrPendingDays { get; set; }
        [NotMapped]
        public string? RemainingDays { get; set; }
        [NotMapped]
        public string? MaxDays { get; set; }
    }

    //public class tblHomework
    //{
    //    public string? ID { get; set; }
    //    public string? SchoolID { get; set; }
    //    public string? AcademicYear { get; set; }
    //    public string? Class { get; set; }
    //    public string? Division { get; set; }
    //    public string? SubjectID { get; set; }
    //    public string? TeacherID { get; set; }
    //    public string? HomeworkTitle { get; set; }
    //    public string? Description { get; set; }
    //    public DateTime? AssignedDate { get; set; }
    //    public DateTime? SubmissionDate { get; set; }
    //    public string? AttachmentURL { get; set; }
    //    public string? IsActive { get; set; }
    //    public string? CreatedBy { get; set; }
    //    public string? CreatedIp { get; set; }
    //    public DateTime? CreatedDate { get; set; }
    //    public string? ModifiedBy { get; set; }
    //    public string? ModifiedIp { get; set; }
    //    public DateTime? ModifiedDate { get; set; }

    //    // Extra fields from joins
    //    public string? ClassName { get; set; }
    //    public string? DivisionName { get; set; }
    //    public string? SubjectName { get; set; }

    //    // NotMapped control fields
    //    [NotMapped]
    //    public string? Flag { get; set; }
    //    [NotMapped]
    //    public string? Status { get; set; }
    //    [NotMapped]
    //    public int? Limit { get; set; }
    //    [NotMapped]
    //    public DateTime? LastCreatedDate { get; set; }
    //    [NotMapped]
    //    public int? LastID { get; set; }
    //    [NotMapped]
    //    public int? totalcount { get; set; }
    //    [NotMapped]
    //    public string? SortColumn { get; set; }
    //    [NotMapped]
    //    public string? SortDirection { get; set; }
    //    [NotMapped]
    //    public int? Offset { get; set; }
    //}
    public class tblHomework
    {
        public string? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }

        public int? Class { get; set; }
        public int? Division { get; set; }
        public int? SubjectID { get; set; }
        public int? TeacherID { get; set; }

        public string? HomeworkTitle { get; set; }
        public string? Description { get; set; }

        public DateTime? AssignedDate { get; set; }
        public DateTime? SubmissionDate { get; set; }

        public string? AttachmentURL { get; set; }

        public string? IsActive { get; set; }

        public string? CreatedBy { get; set; }
        public string? CreatedIp { get; set; }
        public DateTime? CreatedDate { get; set; }

        public string? ModifiedBy { get; set; }
        public string? ModifiedIp { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // ===== COMMON CONTROL FIELDS =====
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



        // ===== JOIN FIELDS =====
        public string? ClassName { get; set; }
        public string? DivisionName { get; set; }
        public string? SubjectName { get; set; }
        public string? SchoolName { get; set; }
        public string? AcademicYearName { get; set; }
    }
    public class tblHomeworkSubmission
    {
        public string? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }

        public int? HomeworkID { get; set; }

        public string? StudentAdmissionNo { get; set; }
        public int? Class { get; set; }
        public int? Division { get; set; }

        public string? SubmissionText { get; set; }
        public string? AttachmentURL { get; set; }

        public DateTime? SubmissionDate { get; set; }

        public string? SubmissionStatus { get; set; }
        public string? MarksObtained { get; set; }
        public string? Remarks { get; set; }

        public string? IsActive { get; set; }

        public string? CreatedBy { get; set; }
        public string? CreatedIp { get; set; }
        public DateTime? CreatedDate { get; set; }

        public string? ModifiedBy { get; set; }
        public string? ModifiedIp { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // ===== COMMON CONTROL FIELDS =====
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

        // ===== JOIN FIELDS =====
        public string? HomeworkTitle { get; set; }
        public string? StudentName { get; set; }
    }

    public class StudentDocumentsUpload
    {
        public string? AdmissionID { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public string? FilePath { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedIp { get; set; }
        public string? Flag { get; set; }
        public string? IsActive { get; set; }
    }

    public class StudentUploadRequest
    {
        public List<IFormFile>? Files { get; set; }
        public IFormFile? File { get; set; }
        public string SchoolId { get; set; }
        public string AdmissionId { get; set; }
        public string? FileType { get; set; }
    }

    public class LogoUploadRequest
    {
        public IFormFile File { get; set; }
        public string SchoolId { get; set; }
    }

    public class DeleteFileRequest
    {
        public string SchoolId { get; set; }
        public string AdmissionId { get; set; }
        public string FileName { get; set; }
    }
}
