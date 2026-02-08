using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace Financial_Management_Server.Models;

public partial class PersonalFinanceDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public PersonalFinanceDbContext()
    {
    }

    public PersonalFinanceDbContext(DbContextOptions<PersonalFinanceDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Budget> Budgets { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Savinggoal> Savinggoals { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<Usertaxprofile> Usertaxprofiles { get; set; }

    public virtual DbSet<Wallet> Wallets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Budget>(entity =>
        {
            entity.HasKey(e => e.BudgetId).HasName("PRIMARY");

            entity.ToTable("budgets");

            entity.HasIndex(e => e.CategoryId, "category_id");

            entity.HasIndex(e => e.UserId, "user_id");

            entity.Property(e => e.BudgetId).HasColumnName("budget_id");
            entity.Property(e => e.AmountLimit)
                .HasPrecision(15, 2)
                .HasColumnName("amount_limit");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Category).WithMany(p => p.Budgets)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("budgets_ibfk_2");

            entity.HasOne(d => d.User).WithMany(p => p.Budgets)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("budgets_ibfk_1");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PRIMARY");

            entity.ToTable("categories");

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(50)
                .HasColumnName("category_name");
            entity.Property(e => e.Type)
                .HasColumnType("enum('Income','Expense')")
                .HasColumnName("type");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PRIMARY");

            entity.ToTable("notifications");

            entity.HasIndex(e => e.UserId, "user_id");

            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.IsRead)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_read");
            entity.Property(e => e.Message)
                .HasColumnType("text")
                .HasColumnName("message");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.Type)
                .HasDefaultValueSql("'Info'")
                .HasColumnType("enum('Info','Warning','Success')")
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("notifications_ibfk_1");
        });

        modelBuilder.Entity<Savinggoal>(entity =>
        {
            entity.HasKey(e => e.GoalId).HasName("PRIMARY");

            entity.ToTable("savinggoals");

            entity.HasIndex(e => e.UserId, "user_id");

            entity.Property(e => e.GoalId).HasColumnName("goal_id");
            entity.Property(e => e.CurrentAmount)
                .HasPrecision(15, 2)
                .HasDefaultValueSql("'0.00'")
                .HasColumnName("current_amount");
            entity.Property(e => e.GoalName)
                .HasMaxLength(100)
                .HasColumnName("goal_name");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'Active'")
                .HasColumnType("enum('Active','Completed','Cancelled')")
                .HasColumnName("status");
            entity.Property(e => e.TargetAmount)
                .HasPrecision(15, 2)
                .HasColumnName("target_amount");
            entity.Property(e => e.TargetDate).HasColumnName("target_date");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Savinggoals)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("savinggoals_ibfk_1");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PRIMARY");

            entity.ToTable("transactions");

            entity.HasIndex(e => e.CategoryId, "category_id");

            entity.HasIndex(e => e.GoalId, "goal_id");

            entity.HasIndex(e => e.UserId, "user_id");

            entity.HasIndex(e => e.WalletId, "wallet_id");

            entity.Property(e => e.TransactionId).HasColumnName("transaction_id");
            entity.Property(e => e.Amount)
                .HasPrecision(15, 2)
                .HasColumnName("amount");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.GoalId).HasColumnName("goal_id");
            entity.Property(e => e.Note)
                .HasColumnType("text")
                .HasColumnName("note");
            entity.Property(e => e.TransactionDate).HasColumnName("transaction_date");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.WalletId).HasColumnName("wallet_id");

            entity.Property(e => e.IsDelete)
              .HasDefaultValue(false)
              .HasColumnName("is_delete");

            entity.HasQueryFilter(w => !w.IsDelete);

            entity.HasOne(d => d.Category).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("transactions_ibfk_2");

            entity.HasOne(d => d.Goal).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.GoalId)
                .HasConstraintName("transactions_ibfk_4");

            entity.HasOne(d => d.User).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("transactions_ibfk_1");

            entity.HasOne(d => d.Wallet).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.WalletId)
                .HasConstraintName("transactions_ibfk_3");
        });

        modelBuilder.Entity<User>(entity => {
            entity.ToTable("users");
            entity.Property(e => e.Id).HasColumnName("user_id");
        });

        modelBuilder.Entity<Usertaxprofile>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("usertaxprofile");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.WalletId).HasColumnName("wallet_id");
            entity.Property(e => e.SavingRate)
                .HasPrecision(5, 2)
                .HasDefaultValueSql("'10.00'")
                .HasColumnName("saving_rate");

            entity.HasOne(d => d.User).WithOne(p => p.Usertaxprofile)
                .HasForeignKey<Usertaxprofile>(d => d.UserId)
                .HasConstraintName("usertaxprofile_ibfk_1");

            entity.HasOne(d => d.Wallet).WithOne(p => p.Usertaxprofile)
                 .HasForeignKey<Usertaxprofile>(d => d.WalletId)
                 .HasConstraintName("usertaxprofile_ibfk_2");
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(e => e.WalletId).HasName("PRIMARY");
            entity.ToTable("wallets");

            entity.HasIndex(e => e.UserId, "user_id");

            entity.Property(e => e.WalletId).HasColumnName("wallet_id");

            entity.Property(e => e.Balance)
                .HasPrecision(15, 2)
                .HasDefaultValue(0.00m)
                .HasColumnName("balance");

            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.Property(e => e.WalletName)
                .HasMaxLength(50)
                .IsRequired()
                .HasColumnName("wallet_name");

            entity.Property(e => e.WalletType)
                .HasColumnType("enum('Spendable','Savings')")
                .HasDefaultValueSql("'Spendable'")
                .HasColumnName("wallet_type");

            entity.Property(e => e.CardNumber)
                .HasMaxLength(20)
                .HasColumnName("card_number");

            entity.Property(e => e.CardHolderName)
                .HasMaxLength(100)
                .HasColumnName("card_holder_name");

            entity.Property(e => e.BankId)
                .HasColumnName("bank_id");

            entity.Property(e => e.IsDefault)
                .HasColumnName("is_default");

            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");

            entity.HasQueryFilter(w => !w.IsDelete);

            entity.HasOne(d => d.User).WithMany(p => p.Wallets)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("wallets_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
