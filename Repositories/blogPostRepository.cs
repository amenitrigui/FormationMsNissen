using FORMATION.Data;
using FORMATION.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace FORMATION.Repositories
{
    public class blogPostRepository : IblogPostRepository
    {
        private readonly BloggydbContext bloggydbContext;

        public blogPostRepository(BloggydbContext bloggydbContext) 
        { 
            this.bloggydbContext = bloggydbContext;
        }
        public async Task<BlogPost> AddAsync(BlogPost blogPost)
        {
           await bloggydbContext.AddAsync(blogPost);
            await bloggydbContext.SaveChangesAsync();
            return blogPost;
        }

        public async Task<BlogPost?> DeleteAsync(Guid id)
        {
            var existingBlog = await bloggydbContext.BlogPosts.FindAsync(id);

            if (existingBlog != null)
            {
                bloggydbContext.BlogPosts.Remove(existingBlog);
                await bloggydbContext.SaveChangesAsync();
                return existingBlog;
            }

            return null;
        }

        public async  Task<IEnumerable<BlogPost>> GetAllAsync()
        {
           return await  bloggydbContext.BlogPosts.Include(x =>x.Tags).ToListAsync();
  
        }

        public async Task<BlogPost?> GetAsync(Guid id)
        {
            return await bloggydbContext.BlogPosts.Include(x =>x.Tags).FirstOrDefaultAsync(x => x.Id == id);

        }

        public async Task<BlogPost?> GetByUrlHandleAsync(string urlHandle)
        {
            return await bloggydbContext.BlogPosts.Include(x => x.Tags).FirstOrDefaultAsync(x => x.UrlHandle == urlHandle);
        }

        public async Task<BlogPost?> UpdateAsync(BlogPost blogPost)
        {
          var existingBlog =  await bloggydbContext.BlogPosts.Include(x => x.Tags).FirstOrDefaultAsync(x => x.Id == blogPost.Id);
            if (existingBlog != null)
            {
                existingBlog.Id = blogPost.Id;
                existingBlog.Heading = blogPost.Heading;
                existingBlog.PageTitle = blogPost.PageTitle;
                existingBlog.Content = blogPost.Content;
                existingBlog.ShortDescription = blogPost.ShortDescription;
                existingBlog.Author = blogPost.Author;
                existingBlog.FeaturedImageUrl = blogPost.FeaturedImageUrl;
                existingBlog.UrlHandle = blogPost.UrlHandle;
                existingBlog.Visible = blogPost.Visible;
                existingBlog.PublishedDate = blogPost.PublishedDate;
                //existingBlog.Tags = blogPost.Tags;

                await bloggydbContext.SaveChangesAsync();
                return existingBlog;
            }

            return null;
        
     }
    }
}
