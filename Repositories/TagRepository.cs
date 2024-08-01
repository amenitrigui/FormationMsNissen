using FORMATION.Data;
using FORMATION.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace FORMATION.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly BloggydbContext bloggydbContext;
        public TagRepository(BloggydbContext bloggydbContext) 
        {
            this.bloggydbContext = bloggydbContext;
        }
        public async Task<IEnumerable<Tag>> GetAllAsync(
             string? searchQuery,
             string? sortBy,
             string? sortDirection,
             int pageNumber = 1,
             int pageSize = 100)
        {
            var query = bloggydbContext.Tags.AsQueryable();

            // Filtering
            if (string.IsNullOrWhiteSpace(searchQuery) == false)
            {
                query = query.Where(x => x.Name.Contains(searchQuery) ||
                                         x.DisplayName.Contains(searchQuery));
            }

            // Sorting
            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                var isDesc = string.Equals(sortDirection, "Desc", StringComparison.OrdinalIgnoreCase);

                if (string.Equals(sortBy, "Name", StringComparison.OrdinalIgnoreCase))
                {
                    query = isDesc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name);
                }

                if (string.Equals(sortBy, "DisplayName", StringComparison.OrdinalIgnoreCase))
                {
                    query = isDesc ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName);
                }
            }

            // Pagination
            // Skip 0 Take 5 -> Page 1 of 5 results
            // Skip 5 Take next 5 -> Page 2 of 5 results
            var skipResults = (pageNumber - 1) * pageSize;
            query = query.Skip(skipResults).Take(pageSize);

            return await query.ToListAsync();

            // return await bloggieDbContext.Tags.ToListAsync();
        }


        public async Task<Tag> AddAsync(Tag tag)
        {
            await bloggydbContext.Tags.AddAsync(tag);
            await bloggydbContext.SaveChangesAsync();
            return tag;
        }

        public async Task<Tag?> DeleteAsync(Guid id)
        {
            var existingTag = await bloggydbContext.Tags.FindAsync(id);

            if (existingTag != null)
            {
                bloggydbContext.Tags.Remove(existingTag);
                await bloggydbContext.SaveChangesAsync();
                return existingTag;
            }

            return null;
        }

        public Task<Tag?> GetAsync(Guid id)
        {
            return bloggydbContext.Tags.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Tag?> UpdateAsync(Tag tag)
        {
            var existingTag = await bloggydbContext.Tags.FindAsync(tag.Id);

            if (existingTag != null)
            {
                existingTag.Name = tag.Name;
                existingTag.DisplayName = tag.DisplayName;

                await bloggydbContext.SaveChangesAsync();

                return existingTag;
            }

            return null;
        }

        public async Task<int> CountAsync()
        {
            return await bloggydbContext.Tags.CountAsync();
        }


    }
}

