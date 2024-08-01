using FORMATION.Models.Domain;
using FORMATION.Models.ViewModels;
using FORMATION.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FORMATION.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminBlogPostsController : Controller
    {
        private readonly ITagRepository tagRepository;
        private readonly IblogPostRepository blogPostRepository;

        public AdminBlogPostsController(ITagRepository tagRepository, IblogPostRepository blogPostRepository)
        {
            this.tagRepository = tagRepository;
            this.blogPostRepository = blogPostRepository;
        }

        // GET: AdminBlogPostsController
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            // get tags from repository
            var tags = await tagRepository.GetAllAsync();

            var model = new AddBlogPostRequest
            {
                Tags = tags.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddBlogPostRequest addBlogPostRequest)
        {
            // Map view model to domain model
            var blogPost = new BlogPost
            {
                Heading = addBlogPostRequest.Heading,
                PageTitle = addBlogPostRequest.PageTitle,
                Content = addBlogPostRequest.Content,
                ShortDescription = addBlogPostRequest.ShortDescription,
                FeaturedImageUrl = addBlogPostRequest.FeaturedImageUrl,
                UrlHandle = addBlogPostRequest.UrlHandle,
                PublishedDate = addBlogPostRequest.PublishedDate,
                Author = addBlogPostRequest.Author,
                Visible = addBlogPostRequest.Visible,
            };

            // Map Tags from selected tags
            var selectedTags = new List<Tag>();
            foreach (var selectedTagId in addBlogPostRequest.SelectedTag)
            {
                var selectedTagIdAsGuid = Guid.Parse(selectedTagId);
                var existingTag = await tagRepository.GetAsync(selectedTagIdAsGuid);

                if (existingTag != null)
                {
                    selectedTags.Add(existingTag);
                }
              

            }
            //blogPost.Tags = selectedTags;

            // Mapping tags back to domain model



            await blogPostRepository.AddAsync(blogPost);

            return RedirectToAction("Add");
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            // Call the repository 
            var blogPosts = await blogPostRepository.GetAllAsync();

            return View(blogPosts);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            // Retrieve the result from the repository 
            var blogPost = await blogPostRepository.GetAsync(id);
            var tagsDomainModel = await tagRepository.GetAllAsync();

            if (blogPost != null)
            {
                // map the domain model into the view model
                var model = new EditBlogPostRequest
                {
                    Id = blogPost.Id,
                    Heading = blogPost.Heading,
                    PageTitle = blogPost.PageTitle,
                    Content = blogPost.Content,
                    Author = blogPost.Author,
                    FeaturedImageUrl = blogPost.FeaturedImageUrl,
                    UrlHandle = blogPost.UrlHandle,
                    ShortDescription = blogPost.ShortDescription,
                    PublishedDate = blogPost.PublishedDate,
                    Visible = blogPost.Visible,
                    Tags = tagsDomainModel.Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    }),
                    SelectedTag = blogPost.Tags.Select(x => x.Id.ToString()).ToArray()
                };

                return View(model);

            }

            // pass data to view
            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditBlogPostRequest editBlogPostRequest)
        {
            // map view model back to domain model
            var blogPostDomainModel = new BlogPost
            {
                Id = editBlogPostRequest.Id,
                Heading = editBlogPostRequest.Heading,
                PageTitle = editBlogPostRequest.PageTitle,
                Content = editBlogPostRequest.Content,
                Author = editBlogPostRequest.Author,
                ShortDescription = editBlogPostRequest.ShortDescription,
                FeaturedImageUrl = editBlogPostRequest.FeaturedImageUrl,
                PublishedDate = editBlogPostRequest.PublishedDate,
                UrlHandle = editBlogPostRequest.UrlHandle,
                Visible = editBlogPostRequest.Visible
            };

            // Map tags into domain model

            var selectedTags = new List<Tag>();
            foreach (var selectedTag in editBlogPostRequest.SelectedTag)
            {
                if (Guid.TryParse(selectedTag, out var tag))
                {
                    var foundTag = await tagRepository.GetAsync(tag);

                    if (foundTag != null)
                    {
                        selectedTags.Add(foundTag);
                    }
                }
            }

            //blogPostDomainModel.Tags = selectedTags;

            // Submit information to repository to update
            var updatedBlog = await blogPostRepository.UpdateAsync(blogPostDomainModel);

            if (updatedBlog != null)
            {
                // Show success notification
                return RedirectToAction("Edit");
            }

            // Show error notification
            return RedirectToAction("Edit");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(EditBlogPostRequest editBlogPostRequest)
        {
            // Talk to repository to delete this blog post and tags
            var deletedBlogPost = await blogPostRepository.DeleteAsync(editBlogPostRequest.Id);

            if (deletedBlogPost != null)
            {
                // Show success notification
                return RedirectToAction("List");
            }

            // Show error notification
            return RedirectToAction("Edit", new { id = editBlogPostRequest.Id });
        }



        // GET: AdminBlogPostsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdminBlogPostsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminBlogPostsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminBlogPostsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AdminBlogPostsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminBlogPostsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AdminBlogPostsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
