using System.Data.Entity;

namespace ViewDemo.Models
{
    public partial class DbModel : DbContext
    {
        public DbModel()
            : base("name=DbModel")
        {
        }
        public virtual DbSet<Member> Members { get; set; }
        public virtual DbSet<Org> Orgs { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
