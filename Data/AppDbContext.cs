using Microsoft.EntityFrameworkCore;
using Tallypath.Models;

namespace Tallypath.Data
{
    public class AppDbContext : DbContext
    {
  
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users => Set<User>();
        public DbSet<Group> Groups => Set<Group>();
        public DbSet<GroupMember> GroupMembers => Set<GroupMember>();
        public DbSet<Expense> Expenses => Set<Expense>();
        public DbSet<GroupInvite> GroupInvites=> Set<GroupInvite>();
        public DbSet<UserBalance> UserBalances => Set<UserBalance>();
        public DbSet<Savings> SavingPlans => Set<Savings>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GroupMember>()
                .HasIndex(gm => new { gm.GroupId, gm.UserId })
                .IsUnique(); // user's membership in a group cannot repeat

            modelBuilder.Entity<Expense>()
                .HasOne(m => m.Creator)
                .WithMany(u => u.Expenses)
                .HasForeignKey(m => m.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ExpenseSplit>(entity =>
                {
                    entity.HasKey(es => new { es.ExpenseId, es.UserId });

                    entity.Property(es => es.Share)
                        .IsRequired();

                    entity.HasOne<Expense>()
                        .WithMany(e => e.Splits)
                        .HasForeignKey(es => es.ExpenseId)
                        .OnDelete(DeleteBehavior.Cascade);

                    entity.HasOne<User>()
                        .WithMany()
                        .HasForeignKey(es => es.UserId);
                }
            );

            modelBuilder.Entity<UserBalance>().HasNoKey();

        }
    }
}
