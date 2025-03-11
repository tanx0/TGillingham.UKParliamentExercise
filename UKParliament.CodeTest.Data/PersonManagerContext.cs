using Microsoft.EntityFrameworkCore;

namespace UKParliament.CodeTest.Data;

public class PersonManagerContext(DbContextOptions<PersonManagerContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<PersonEntity>().HasKey(d => d.Id);  // Set primary key
        
        modelBuilder.Entity<PersonEntity>()
            .HasOne(p => p.Department)
            .WithMany(d => d.People)
            .HasForeignKey(p => p.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade); 

        //todo: endure DepartmentId starts with 1
        modelBuilder.Entity<DepartmentEntity>().HasKey(d => d.Id);  // Set primary key

        var dep1 = new DepartmentEntity { Id = 1, Name = "Sales" };
        var dep2 = new DepartmentEntity { Id = 2, Name = "Marketing" };
        var dep3 = new DepartmentEntity { Id = 3, Name = "Finance" };        
        var dep4 = new DepartmentEntity { Id = 4, Name = "HR" };        

        modelBuilder.Entity<DepartmentEntity>().HasData(dep1, dep2, dep3, dep4);
    }

    public DbSet<PersonEntity> People { get; set; }

    public DbSet<DepartmentEntity> Departments { get; set; }
}