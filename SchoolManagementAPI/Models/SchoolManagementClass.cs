using MySqlConnector;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

/// <summary>
    /// Model DTO schemas mapping inbound JSON requests and database columns.
    /// </summary>
    namespace SchoolManagementAPI.Models
{
    /// <summary>
    /// Data Transfer Object Model: SchoolManagementClass
    /// </summary>
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

    /// <summary>
    /// Data Transfer Object Model: SchoolDetails
    /// </summary>
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
        public string? SchoolIDs { get; set; }
    }

    /// <summary>
    /// Data Transfer Object Model: TblUser
    /// </summary>
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

    /// <summary>
    /// Data Transfer Object Model: UserOTP
    /// </summary>
    public class UserOTP
    {
        public string Email { get; set; }
        public string OTP { get; set; }
        public DateTime ExpiryTime { get; set; }
    }

    /// <summary>
    /// Data Transfer Object Model: SendOtpRequest
    /// </summary>
    public class SendOtpRequest
    {
        public string Email { get; set; }
    }

    /// <summary>
    /// Data Transfer Object Model: VerifyOtpRequest
    /// </summary>
    public class VerifyOtpRequest
    {
        public string Email { get; set; }
        public string OTP { get; set; }
        public string? NewPassword { get; set; }
    }

    /// <summary>
    /// Data Transfer Object Model: tblAcademicYear
    /// </summary>
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

    /// <summary>
    /// Data Transfer Object Model: tblSyllabus
    /// </summary>
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

    /// <summary>
    /// Data Transfer Object Model: tblClass
    /// </summary>
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

    /// <summary>
    /// Data Transfer Object Model: tblClassDivision
    /// </summary>
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

    /// <summary>
    /// Data Transfer Object Model: tblSubjects
    /// </summary>
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
        public int? inactiveCount { get; set; }
        public int? activeCount { get; set; }
    }

    /// <summary>
    /// Data Transfer Object Model: tblSubjectStaff
    /// </summary>
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

    /// <summary>
    /// Data Transfer Object Model: ExportSyllabusRequest
    /// </summary>
    public class ExportSyllabusRequest
    {
        public string Flag { get; set; } = "2";
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }
        public string? Name { get; set; }
    }

    /// <summary>
    /// Data Transfer Object Model: tblModules
    /// </summary>
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

    /// <summary>
    /// Data Transfer Object Model: tblPages
    /// </summary>
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

    /// <summary>
    /// Data Transfer Object Model: tblStaff
    /// </summary>
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
        public int? inactiveCount { get; set; }
        public int? activeCount { get; set; }
        public string? SchoolIDs { get; set; }
    }

    /// <summary>
    /// Data Transfer Object Model: UserToken
    /// </summary>
    public class UserToken
    {
        public string Email { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime AccessExpiry { get; set; }
        public DateTime RefreshExpiry { get; set; }
    }

    /// <summary>
    /// Data Transfer Object Model: RefreshTokenRequest
    /// </summary>
    public class RefreshTokenRequest
    {
        public string Email { get; set; }
        public string RefreshToken { get; set; }
    }

    /// <summary>
    /// Data Transfer Object Model: tblRoles
    /// </summary>
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

    /// <summary>
    /// Data Transfer Object Model: tblUserRoles
    /// </summary>
    public class tblUserRoles
    {
        public string? UserID { get; set; }
        public string? RoleID { get; set; }
        public string? Flag { get; set; }
        public string? Status { get; set; }
    }

    /// <summary>
    /// Data Transfer Object Model: tblRolePermissions
    /// </summary>
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

    /// <summary>
    /// Data Transfer Object Model: tblStudentDetails
    /// </summary>
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

    /// <summary>
    /// Data Transfer Object Model: tblStudentAddressDetails
    /// </summary>
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

    /// <summary>
    /// Data Transfer Object Model: tblStudentParentDetails
    /// </summary>
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
        public String? StudentName { get; set; }

    }

    /// <summary>
    /// Data Transfer Object Model: tblStudentTransportationDetails
    /// </summary>
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
    
    /// <summary>
    /// Data Transfer Object Model: tblStudentAcademicDetails
    /// </summary>
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

    
    /// <summary>
    /// Data Transfer Object Model: tblAllotClassTeacher
    /// </summary>
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
    
    /// <summary>
    /// Data Transfer Object Model: tblBus
    /// </summary>
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
        public int? inactiveCount { get; set; }
        public int? activeCount { get; set; }
    }
    
    /// <summary>
    /// Data Transfer Object Model: tblRoute
    /// </summary>
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
        public int? inactiveCount { get; set; }
        public int? activeCount { get; set; }
    }
    
    /// <summary>
    /// Data Transfer Object Model: tblStops
    /// </summary>
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
        public int? inactiveCount { get; set; }
        public int? activeCount { get; set; }
    }
    
    /// <summary>
    /// Data Transfer Object Model: tblFare
    /// </summary>
    public class tblFare
    {
        public string? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }
        public string? RouteID { get; set; }
        public string? StopID { get; set; }
        public string? BusID { get; set; }
        public decimal? Amount { get; set; }  
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
        public int? inactiveCount { get; set; }
        public int? activeCount { get; set; }
    }
    
    /// <summary>
    /// Data Transfer Object Model: TblWorkingDays
    /// </summary>
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
    
    /// <summary>
    /// Data Transfer Object Model: TblSession
    /// </summary>
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
    
    /// <summary>
    /// Data Transfer Object Model: TblTimeTable
    /// </summary>
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
    
    /// <summary>
    /// Data Transfer Object Model: TblTimeTableDetail
    /// </summary>
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
    
    /// <summary>
    /// Data Transfer Object Model: tblExamType
    /// </summary>
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
    
    /// <summary>
    /// Data Transfer Object Model: tblSetExam
    /// </summary>
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
    
    /// <summary>
    /// Data Transfer Object Model: tblExamAttendence
    /// </summary>
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
    
    /// <summary>
    /// Data Transfer Object Model: AttendanceStudent
    /// </summary>
    public class AttendanceStudent
    {
        public string AdmissionID { get; set; }
        public string Attendance { get; set; }
        public string Remarks { get; set; }
    }
    
    /// <summary>
    /// Data Transfer Object Model: tblExamMarks
    /// </summary>
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
    
    /// <summary>
    /// Data Transfer Object Model: ExamMarksStudent
    /// </summary>
    public class ExamMarksStudent
    {
        public string? AdmissionID { get; set; }
        public string? Marks { get; set; }
    }
    
    /// <summary>
    /// Data Transfer Object Model: tblStudentAttendance
    /// </summary>
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
        /// <summary>
    /// Data Transfer Object Model: StudentAttendanceStudent
    /// </summary>
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
    
    /// <summary>
    /// Data Transfer Object Model: tblStaffAttendance
    /// </summary>
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

        /// <summary>
    /// Data Transfer Object Model: StaffAttendanceItem
    /// </summary>
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
    
    /// <summary>
    /// Data Transfer Object Model: feeCategory
    /// </summary>
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
        public int? inactiveCount { get; set; }
        public int? activeCount { get; set; }
    }
    
    /// <summary>
    /// Data Transfer Object Model: tblfeeAllocation
    /// </summary>
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
    
    /// <summary>
    /// Data Transfer Object Model: tblfeeDiscountCategory
    /// </summary>
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
    
    /// <summary>
    /// Data Transfer Object Model: tblfeeDiscount
    /// </summary>
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
    
    /// <summary>
    /// Data Transfer Object Model: tblStudentTransfer
    /// </summary>
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
    
    /// <summary>
    /// Data Transfer Object Model: tblFeeCollection
    /// </summary>
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
    
    /// <summary>
    /// Data Transfer Object Model: DashboardRequest
    /// </summary>
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
        public string? SchoolIDs { get; set; }
    }
    
    /// <summary>
    /// Data Transfer Object Model: DashboardResponse
    /// </summary>
    public class DashboardResponse
    {
        //public DashboardCounts Counts { get; set; }
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
    
    /// <summary>
    /// Data Transfer Object Model: DashboardCounts
    /// </summary>
    public class DashboardCounts
    {
        public int ClassCount { get; set; }
        public int DivisionsCount { get; set; }
        public int StaffCount { get; set; }
        public int StudentsCount { get; set; }
    }
    
    /// <summary>
    /// Data Transfer Object Model: StaffChart
    /// </summary>
    public class StaffChart
    {
        public string StaffType { get; set; }
        public int Count { get; set; }
    }
    
    /// <summary>
    /// Data Transfer Object Model: AttendanceChart
    /// </summary>
    public class AttendanceChart
    {
        public string Month { get; set; }
        public double Attendance { get; set; }
    }
    
    /// <summary>
    /// Data Transfer Object Model: StudentChart
    /// </summary>
    public class StudentChart
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int StudentCount { get; set; }
    }
    
    /// <summary>
    /// Data Transfer Object Model: Notice
    /// </summary>
    public class Notice
    {
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    
    /// <summary>
    /// Data Transfer Object Model: FeeChart
    /// </summary>
    public class FeeChart
    {
        public string Month { get; set; }
        public double Attendance { get; set; }
        public double Amount { get; set; }
    }
    
    /// <summary>
    /// Data Transfer Object Model: RecentAdmission
    /// </summary>
    public class RecentAdmission
    {
        public string Name { get; set; }
        public string Class { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? JoinDate { get; set; }
    }
   
    /// <summary>
    /// Data Transfer Object Model: RecentStaff
    /// </summary>
    public class RecentStaff
    {
        public string Name { get; set; }
        public DateTime JoiningDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string RoleName { get; set; }   // your proc returns RoleName
    }

    /// <summary>
    /// Data Transfer Object Model: MiniKpis
    /// </summary>
    public class MiniKpis
    {
        public double attendancePercent { get; set; }
        public double activeUsers { get; set; }
        public double totalCollection { get; set; }  // IMPORTANT: from proc
        public double upcomingExams { get; set; }
    }

    /// <summary>
    /// Data Transfer Object Model: RoleActivityItem
    /// </summary>
    public class RoleActivityItem
    {
        public DateTime CreatedDate { get; set; }
        public string title { get; set; }
        public string meta { get; set; }
        public DateTime? activityDate { get; set; }
    }

    /// <summary>
    /// Data Transfer Object Model: DashboardAlert
    /// </summary>
    public class DashboardAlert
    {
        public string severity { get; set; }
        public string title { get; set; }
        public string reasonText { get; set; }
        public string actionText { get; set; }
    }

    /// <summary>
    /// Data Transfer Object Model: DashboardMeta
    /// </summary>
    public class DashboardMeta
    {
        public DateTime? generatedAt { get; set; }
    }

    /// <summary>
    /// Data Transfer Object Model: TblSalarySetting
    /// </summary>
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
    /// <summary>
    /// Data Transfer Object Model: TblSalaryPay
    /// </summary>
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

    /// <summary>
    /// Data Transfer Object Model: tblPayrollHead
    /// </summary>
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

    /// <summary>
    /// Data Transfer Object Model: TblPaymentMode
    /// </summary>
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


    /// <summary>
    /// Data Transfer Object Model: TblAdvanceSalary
    /// </summary>
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
    
    /// <summary>
    /// Data Transfer Object Model: tblLeavepolicy
    /// </summary>
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

    /// <summary>
    /// Data Transfer Object Model: tblLeaveApplication
    /// </summary>
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

        public string? AttachmentURL { get; set; }

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

    /// <summary>
    /// Data Transfer Object Model: TblHolidayCalendar
    /// </summary>
    public class TblHolidayCalendar
    {
        public string? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }

        public string? HolidayName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? HolidayType { get; set; }
        public string? Description { get; set; }

        public string? IsActive { get; set; }

        public string? SchoolName { get; set; }
        public string? AcademicYearName { get; set; }

        public string? CreatedBy { get; set; }
        public string? CreatedIp { get; set; }
        public DateTime? CreatedDate { get; set; }

        public string? ModifiedBy { get; set; }
        public string? ModifiedIp { get; set; }
        public DateTime? ModifiedDate { get; set; }

        [NotMapped] public string? Flag { get; set; }
        [NotMapped] public string? Status { get; set; }
        [NotMapped] public int? Limit { get; set; }
        [NotMapped] public DateTime? LastCreatedDate { get; set; }
        [NotMapped] public int? LastID { get; set; }
        [NotMapped] public int? totalcount { get; set; }
        [NotMapped] public string? SortColumn { get; set; }
        [NotMapped] public string? SortDirection { get; set; }
        [NotMapped] public int? Offset { get; set; }
    }

    /// <summary>
    /// Data Transfer Object Model: TblNotices
    /// </summary>
    public class TblNotices
    {
        public int? NoticeId { get; set; }
        public int? SchoolID { get; set; }
        public int? AcademicYear { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? NoticeType { get; set; }
        public string? Audience { get; set; }
        public string? ClassIds { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? AttachmentPath { get; set; }
        public int? IsActive { get; set; }

        public string? CreatedBy { get; set; }
        public string? CreatedIP { get; set; }
        public DateTime? CreatedAt { get; set; }

        public int? ModifiedBy { get; set; }
        public string? ModifiedIP { get; set; }
        public DateTime? ModifiedAt { get; set; }

        // Joined fields
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
        [NotMapped] public int? TotalCount { get; set; }
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

    /// <summary>
    /// Data Transfer Object Model: tblHomework
    /// </summary>
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
    

    /// <summary>
    /// Data Transfer Object Model: HomeworkUploadRequest
    /// </summary>
    public class HomeworkUploadRequest
    {
        public IFormFile? File { get; set; }
        public string? SchoolId { get; set; }
        public string? HomeworkId { get; set; }
        public string? FileType { get; set; }
    }

    /// <summary>
    /// Data Transfer Object Model: tblHomeworkSubmission
    /// </summary>
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

    /// <summary>
    /// Data Transfer Object Model: StudentDocumentsUpload
    /// </summary>
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

    /// <summary>
    /// Data Transfer Object Model: StudentUploadRequest
    /// </summary>
    public class StudentUploadRequest
    {
        public List<IFormFile>? Files { get; set; }
        public IFormFile? File { get; set; }
        public string? SchoolId { get; set; }
        public string? AdmissionId { get; set; }
        public string? FileType { get; set; }
    }

    /// <summary>
    /// Data Transfer Object Model: LogoUploadRequest
    /// </summary>
    public class LogoUploadRequest
    {
        public IFormFile File { get; set; }
        public string SchoolId { get; set; }
    }

    /// <summary>
    /// Data Transfer Object Model: DeleteFileRequest
    /// </summary>
    public class DeleteFileRequest
    {
        public string SchoolId { get; set; }
        public string AdmissionId { get; set; }
        public string FileName { get; set; }
    }

    /// <summary>
    /// Data Transfer Object Model: tblUnits
    /// </summary>
    public class tblUnits
    {
        public string? ID { get; set; }

        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }

        public string? UnitName { get; set; }
        public string? Abbreviation { get; set; }

        public string? MinimumValue { get; set; }
        public string? MaximumValue { get; set; }
        public string? MinimumDifference { get; set; }

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
    
    /// <summary>
    /// Data Transfer Object Model: tblCategories
    /// </summary>
    public class tblCategories
    {
        public string? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }
        public string? CategoryName { get; set; }
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
    
    /// <summary>
    /// Data Transfer Object Model: tblItems
    /// </summary>
    public class tblItems
    {
        public string? ID { get; set; }
        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }
        public string? CategoryID { get; set; }
        public string? UnitID { get; set; }
        public string? ItemName { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? OpeningStock { get; set; }
        public decimal? ReorderLevel { get; set; }
        public decimal? TaxCGST { get; set; }
        public decimal? TaxSGST { get; set; }
        public string? Description { get; set; }
        public string? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedIp { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public string? ModifiedIp { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // Joined fields
        public string? CategoryName { get; set; }
        public string? UnitName { get; set; }
        public string? SchoolName { get; set; }
        public string? AcademicYearName { get; set; }

        // Pagination & sorting
        public string? Flag { get; set; }
        public int? Limit { get; set; }
        public string? LastCreatedDate { get; set; }
        public int? LastID { get; set; }
        public string? SortColumn { get; set; }
        public string? SortDirection { get; set; }
        public int? Offset { get; set; }

        // Meta
        public string? Status { get; set; }
        public int? totalcount { get; set; }
    }
    
    /// <summary>
    /// Data Transfer Object Model: tblSuppliers
    /// </summary>
    public class tblSuppliers
    {
        public string? ID { get; set; }

        public string? SchoolID { get; set; }

        public string? AcademicYear { get; set; }

        public string? SupplierName { get; set; }

        public string? EmailAddress { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? IsActive { get; set; }

        public string? CreatedBy { get; set; }

        public string? CreatedIP { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? ModifiedBy { get; set; }

        public string? ModifiedIP { get; set; }

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
        public string? SortDirection { get; set; }

        [NotMapped]
        public int? Offset { get; set; }

        public string? SchoolName { get; set; }

        public string? AcademicYearName { get; set; }
    }
   
    /// <summary>
    /// Data Transfer Object Model: tblPurchase
    /// </summary>
    public class tblPurchase
    {
        public string? ID { get; set; }

        public string? SchoolID { get; set; }

        public string? AcademicYear { get; set; }

        public string? SupplierID { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public string? PaymentMode { get; set; }

        public string? Notes { get; set; }

        public string? ItemIDs { get; set; }

        public string? Quantities { get; set; }

        public string? Prices { get; set; }

        public string? CGSTs { get; set; }

        public string? SGSTs { get; set; }

        public string? TotalTaxes { get; set; }

        public string? SubTotals { get; set; }

        public decimal? TotalTaxAmount { get; set; }

        public decimal? GrandTotalAmount { get; set; }

        public string? IsActive { get; set; }

        public string? CreatedBy { get; set; }

        public string? CreatedIP { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? ModifiedBy { get; set; }

        public string? ModifiedIP { get; set; }

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
        public string? SortDirection { get; set; }

        [NotMapped]
        public int? Offset { get; set; }

        public string? SupplierName { get; set; }

        public string? SchoolName { get; set; }

        public string? AcademicYearName { get; set; }
    }
    
    /// <summary>
    /// Data Transfer Object Model: tblSales
    /// </summary>
    public class tblSales
    {
        public string? ID { get; set; }

        public string? SchoolID { get; set; }

        public string? AcademicYear { get; set; }

        public string? ClassID { get; set; }

        public string? DivisionID { get; set; }

        public string? AdmissionNo { get; set; }

        public string? CategoryIDs { get; set; }

        public string? ItemIDs { get; set; }

        public string? Prices { get; set; }

        public string? TaxAmounts { get; set; }

        public string? SubTotals { get; set; }

        public decimal? TotalTaxAmount { get; set; }

        public decimal? GrandTotalAmount { get; set; }

        public string? PaymentMode { get; set; }

        public string? Notes { get; set; }

        public DateTime? SaleDate { get; set; }

        public string? IsActive { get; set; }

        public string? CreatedBy { get; set; }

        public string? CreatedIP { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? ModifiedBy { get; set; }

        public string? ModifiedIP { get; set; }

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
        public string? SortDirection { get; set; }

        [NotMapped]
        public int? Offset { get; set; }

        public string? ClassName { get; set; }

        public string? DivisionName { get; set; }

        public string? StudentName { get; set; }

        public string? SchoolName { get; set; }

        public string? AcademicYearName { get; set; }
        public string? CategoryNames { get; set; }

        public string? ItemNames { get; set; }
    }

    /// <summary>
    /// Data Transfer Object Model: Tbl_HostelMaster
    /// </summary>
    public class Tbl_HostelMaster
    {
        public string? ID { get; set; }

        public string? SchoolID { get; set; }
        public string? AcademicYear { get; set; }

        public string? HostelName { get; set; }
        public string? HostelType { get; set; }

        public string? TotalRooms { get; set; }

        public string? BedCapacity { get; set; }

        public string? Address { get; set; }

        public string? Remarks { get; set; }

        public string? IsActive { get; set; }

        public string? CreatedBy { get; set; }
        public string? CreatedIp { get; set; }
        public DateTime? CreatedDate { get; set; }

        public string? ModifiedBy { get; set; }
        public string? ModifiedIp { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // ===== DISPLAY FIELDS =====

        public string? SchoolName { get; set; }

        public string? AcademicYearName { get; set; }

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
    }

    /// <summary>
    /// Data Transfer Object Model: Tbl_RoomMaster
    /// </summary>
    public class Tbl_RoomMaster
    {
        public string? ID { get; set; }

        public string? SchoolID { get; set; }

        public string? AcademicYear { get; set; }

        public string? HostelID { get; set; }

        public string? RoomNumber { get; set; }

        public string? BedCapacity { get; set; }

        public string? Occupied { get; set; }

        public string? Remarks { get; set; }

        public string? IsActive { get; set; }

        public string? CreatedBy { get; set; }
        public string? CreatedIp { get; set; }
        public DateTime? CreatedDate { get; set; }

        public string? ModifiedBy { get; set; }
        public string? ModifiedIp { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // ===== DISPLAY FIELDS =====

        public string? HostelName { get; set; }

        public string? SchoolName { get; set; }

        public string? AcademicYearName { get; set; }

        public string? OccupiedBeds { get; set; }

        public string? AvailableBeds { get; set; }

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
    }

    /// <summary>
    /// Data Transfer Object Model: Tbl_RoomAllotment
    /// </summary>
    public class Tbl_RoomAllotment
    {
        public string? ID { get; set; }

        public string? SchoolID { get; set; }

        public string? AcademicYear { get; set; }

        public string? HostelID { get; set; }

        public string? RoomID { get; set; }

        public string? StudentID { get; set; }

        public string? AllotmentDate { get; set; }

        public string? Remarks { get; set; }

        public string? IsActive { get; set; }

        public string? CreatedBy { get; set; }
        public string? CreatedIp { get; set; }
        public DateTime? CreatedDate { get; set; }

        public string? ModifiedBy { get; set; }
        public string? ModifiedIp { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // ===== DISPLAY FIELDS =====

        public string? HostelName { get; set; }

        public string? RoomNumber { get; set; }

        public string? BedCapacity { get; set; }

        public string? OccupiedBeds { get; set; }

        public string? AvailableBeds { get; set; }

        public string? SchoolName { get; set; }

        public string? AcademicYearName { get; set; }

        // ===== COMMON CONTROL FIELDS =====

        [NotMapped]
        public string? Flag { get; set; }

        [NotMapped]
        public string? Status { get; set; }

        [NotMapped]
        public int? Limit { get; set; }

        [NotMapped]
        public int? Offset { get; set; }
    }

    /// <summary>
    /// Data Transfer Object Model: Tbl_OutPass
    /// </summary>
    public class Tbl_OutPass
    {
        public string? ID { get; set; }

        public string? SchoolID { get; set; }

        public string? AcademicYear { get; set; }

        public string? HostelID { get; set; }

        public string? RoomID { get; set; }

        public string? StudentID { get; set; }

        public string? OutDateTime { get; set; }

        public string? ExpectedReturnDateTime { get; set; }

        public string? ActualReturnDateTime { get; set; }

        public string? Destination { get; set; }

        public string? Reason { get; set; }

        public string? OutPassStatus { get; set; }

        public string? ApprovedBy { get; set; }

        public string? ApprovedDate { get; set; }

        public string? Remarks { get; set; }

        public string? IsActive { get; set; }

        public string? CreatedBy { get; set; }
        public string? CreatedIp { get; set; }
        public DateTime? CreatedDate { get; set; }

        public string? ModifiedBy { get; set; }
        public string? ModifiedIp { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // ===== DISPLAY FIELDS =====

        public string? HostelName { get; set; }

        public string? RoomNumber { get; set; }

        public string? SchoolName { get; set; }

        public string? AcademicYearName { get; set; }

        public string? StudentName { get; set; }

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
    }

    /// <summary>
    /// Data Transfer Object Model: SchoolFile
    /// </summary>
    public class SchoolFile
    {
        public string? SchoolID { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? FileType { get; set; }
        public string? Flag { get; set; }
    }
}
