using CoffeeBeanExplorer.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeBeanExplorer.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Bean> Beans { get; set; } = null!;
    public DbSet<Origin> Origins { get; set; } = null!;
    public DbSet<Review> Reviews { get; set; } = null!;
    public DbSet<UserList> UserLists { get; set; } = null!;
    public DbSet<BeanTasteNote> BeanTasteNotes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BeanTasteNote>()
            .HasKey(bt => new { bt.BeanId, bt.TasteNote });

        modelBuilder.Entity<Bean>()
            .HasOne(b => b.Origin)
            .WithMany(o => o.Beans)
            .HasForeignKey(b => b.OriginId);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.Bean)
            .WithMany(b => b.Reviews)
            .HasForeignKey(r => r.BeanId);

        modelBuilder.Entity<UserList>()
            .HasOne(ul => ul.User)
            .WithMany(u => u.UserCollections)
            .HasForeignKey(ul => ul.UserId);

        modelBuilder.Entity<UserList>()
            .HasOne(ul => ul.Bean)
            .WithMany(b => b.UserCollections)
            .HasForeignKey(ul => ul.BeanId);
    }
}