using FORMATION.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace FORMATION.Data
{
   
        public class BloggydbContext : DbContext
    {
        public BloggydbContext(DbContextOptions<BloggydbContext> options) : base(options)
        {
        }

        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<BlogPostLike> BlogPostLike { get; set; }
        public DbSet<BlogPostComment> BlogPostComment { get; set; }
    }
}
