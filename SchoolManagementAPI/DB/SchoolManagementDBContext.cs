using Microsoft.EntityFrameworkCore;
using SchoolManagementAPI.Models;

namespace SchoolManagementAPI.DB
{
    public class SchoolManagementDBContext:DbContext
    {
        public SchoolManagementDBContext(DbContextOptions<SchoolManagementDBContext> options) : base(options) { }

        public DbSet<TblUser> tbl_users { get; set; }
        public DbSet<tblAcademicYear> Tbl_AcademicYear { get; set; }
        public DbSet<tblSyllabus> Tbl_Syllabus { get; set; }
        public DbSet<SchoolDetails> tbl_SchoolDetails { get; set; }
        public DbSet<tblPages> tbl_pages { get; set; }
        public DbSet<tblModules> tbl_modules { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tblSyllabus>().HasNoKey();
            modelBuilder.Entity<tblPages>().HasNoKey();
            base.OnModelCreating(modelBuilder);
        }
    }
}
