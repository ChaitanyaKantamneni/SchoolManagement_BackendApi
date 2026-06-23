using Microsoft.EntityFrameworkCore;
using SchoolManagementAPI.Models;

namespace SchoolManagementAPI.DB
{
    /// <summary>
    /// SchoolManagementDBContext: Entity Framework Core database context.
    /// This context is used to perform ORM operations for core system tables (users, details, syllabus).
    /// Direct ADO.NET queries are handled via SchoolManagementDAL.cs.
    /// </summary>
    public class SchoolManagementDBContext:DbContext
    {
        /// <summary>
        /// Constructor injection initializing context options from Program.cs configurations.
        /// </summary>
        public SchoolManagementDBContext(DbContextOptions<SchoolManagementDBContext> options) : base(options) { }

        public DbSet<TblUser> tbl_users { get; set; }
        public DbSet<tblAcademicYear> Tbl_AcademicYear { get; set; }
        public DbSet<tblSyllabus> Tbl_Syllabus { get; set; }
        public DbSet<SchoolDetails> tbl_SchoolDetails { get; set; }
        public DbSet<tblPages> tbl_pages { get; set; }
        public DbSet<tblModules> tbl_modules { get; set; }

        /// <summary>
        /// Configures database mapping properties. Keyless models are flagged here to map direct query results.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tblSyllabus>().HasNoKey();
            modelBuilder.Entity<tblPages>().HasNoKey();
            base.OnModelCreating(modelBuilder);
        }
    }
}
