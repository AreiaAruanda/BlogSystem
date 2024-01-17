using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

public class BloggingContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<User> Users { get; set; }
    public string DbPath { get; }

    public BloggingContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "blogging2.db");
        //var folder = Environment.CurrentDirectory;
        //DbPath = System.IO.Path.Join(folder, "blogging3.db");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>()
            .HasMany(b => b.Posts)
            .WithOne(b => b.Blog)
            .HasForeignKey(b => b.BlogId)
            .HasPrincipalKey(b => b.Id);

        modelBuilder.Entity<User>()
           .HasMany(u => u.Posts)
           .WithOne(u => u.User)
           .HasForeignKey(u => u.UserId)
           .HasPrincipalKey(u => u.Id);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}

public class User
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public List<Post>? Posts { get; set; } = new();
}
public class Blog
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Url { get; set; }
    public List<Post>? Posts { get; set; } = new();
}

public class Post
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public int BlogId { get; set; }
    public int UserId { get; set; }

    public Blog? Blog { get; set; }
    public User? User { get; set; }
}
