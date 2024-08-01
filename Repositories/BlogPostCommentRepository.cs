using FORMATION.Data;
using FORMATION.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace FORMATION.Repositories
{
    public class BlogPostCommentRepository: IBlogPostCommentRepository
    {
        private readonly BloggydbContext bloggieDbContext;

        public BlogPostCommentRepository(BloggydbContext bloggieDbContext)
        {
            this.bloggieDbContext = bloggieDbContext;
        }

        public async Task<BlogPostComment> AddAsync(BlogPostComment blogPostComment)
        {
            await bloggieDbContext.BlogPostComment.AddAsync(blogPostComment);
            await bloggieDbContext.SaveChangesAsync();
            return blogPostComment;
        }

        public async Task<IEnumerable<BlogPostComment>> GetCommentsByBlogIdAsync(Guid blogPostId)
        {
            return await bloggieDbContext.BlogPostComment.Where(x => x.BlogPostId == blogPostId)
                 .ToListAsync();
        }
    }
}
