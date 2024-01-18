using System;
using System.Linq;
using System.IO;
using System.Threading.Channels;

using var db = new BloggingContext();

Console.WriteLine($"Database path: {db.DbPath}.");

//Deleting table content 
#region
db.Users.RemoveRange(db.Users);
db.Posts.RemoveRange(db.Posts);
db.Blogs.RemoveRange(db.Blogs);
db.SaveChanges();
#endregion

// Reading data from CSV files
var users = ReadUserFromCsv();
var blogs = ReadBlogFromCsv();
var posts = ReadPostFromCsv();

// Adding data to db
db.Users.AddRange(users);
db.Blogs.AddRange(blogs);
db.Posts.AddRange(posts);
db.SaveChanges();

// Displaying the data
DisplayData(db);

//Read from CSVs
static List<User> ReadUserFromCsv()
{
    var users = new List<User>();
    var processedIds = new List<int>(); //tracks if user already exists
    var lines = File.ReadAllLines("User.csv");
    foreach (var line in lines)
    {
        var parts = line.Split(",");
        if (int.TryParse(parts[0], out int id) && !processedIds.Contains(id))
        {
            users.Add(new User
            {
                Id = id,
                Username = parts[1],
                Password = parts[2],
            });
            processedIds.Add(id);
        }
        else
        {
            continue;
        }
    }
    return users;           
}
db.SaveChanges();
static List<Blog> ReadBlogFromCsv()
{
    var processedIds = new List<int>(); //tracks if blog already exists
    var blogs = new List<Blog>();
    var lines = File.ReadAllLines("Blog.csv");

    foreach (var line in lines)
    {
        var parts = line.Split(",");
        if (int.TryParse(parts[0], out int id) && !processedIds.Contains(id))
        {
            blogs.Add(new Blog
            {
                Id = id,
                Name = parts[1],
                Url = parts[2]
            });
            processedIds.Add(id);
        }
        else
        {
            continue;
        }
    }
    return blogs;
}
db.SaveChanges();

static List<Post> ReadPostFromCsv()
{
    var posts = new List<Post>();
    var lines = File.ReadAllLines("Post.csv");

    foreach (var line in lines)
    {
        var parts = line.Split(",");
        posts.Add(new Post
        {
            Id = Convert.ToInt32(parts[0]),
            Title = parts[1],
            Content = parts[2],
            BlogId = Convert.ToInt32(parts[3]),
            UserId = Convert.ToInt32(parts[4])
        });
    }
    return posts;
}
db.SaveChanges();

//Output
static void DisplayData(BloggingContext db)
{
    Console.WriteLine();
    Console.WriteLine("\u001b[1mUSERS:\u001b[0m");
    foreach (var user in db.Users)
    {
        Console.WriteLine();
        Console.WriteLine($"    \u001b[1mUserID:{user.Id}\u001b[0m");
        Console.WriteLine($"        Username: {user.Username}");
        Console.WriteLine($"        Password: {user.Password}");
        Console.WriteLine($"        Posts:");
        foreach (Post post in user.Posts)
        {
            Console.WriteLine($"            - {post.Title}");
        }
    }

    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine("\u001b[1mBLOGS:\u001b[0m");
    foreach (var blog in db.Blogs)
    {
        Console.WriteLine();
        Console.WriteLine($"    \u001b[1mBlogID:{blog.Id}\u001b[0m");
        Console.WriteLine($"        Name: {blog.Name}");
        Console.WriteLine($"        URL: {blog.Url}");
        Console.WriteLine($"        Posts:");
        foreach (Post post in blog.Posts)
        {
            Console.WriteLine($"            - {post.Title}");
        }
    }

    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine("\u001b[1mPOSTS:\u001b[0m");
    Console.WriteLine();

    foreach (var post in db.Posts)
    {
        Console.WriteLine("     \u001b[1mPostID:" + post.Id + "\u001b[0m");
        Console.WriteLine($"         Title: {post.Title}");
        Console.WriteLine($"         Content: {post.Content}");
        Console.WriteLine($"         Published on '{post.Blog.Name}' (BlogID: {post.BlogId})");
        Console.WriteLine($"         By '{post.User.Username}' (UserID: {post.UserId})");
        Console.WriteLine();
    }
}
