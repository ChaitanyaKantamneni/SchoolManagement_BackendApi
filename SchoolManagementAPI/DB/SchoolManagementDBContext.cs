using Microsoft.EntityFrameworkCore;
using SchoolManagementAPI.Models;

namespace SchoolManagementAPI.DB
{
    public class SchoolManagementDBContext:DbContext
    {
        public SchoolManagementDBContext(DbContextOptions<SchoolManagementDBContext> options) : base(options) { }

        public DbSet<TblUser> tbl_users { get; set; }
    }
}
