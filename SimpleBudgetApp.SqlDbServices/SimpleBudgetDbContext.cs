using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using SimpleBudgetApp;

namespace SimpleBudgetApp.SqlDbServices;

public class SimpleBudgetDbContext : DbContext
{
    public SimpleBudgetDbContext(DbContextOptions<SimpleBudgetDbContext> options)
      : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Category> Categories { get; set; }
}