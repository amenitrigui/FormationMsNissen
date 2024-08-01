using FORMATION.Models.Domain;
using FORMATION.Models.ViewModels;
using FORMATION.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace FORMATION.Controllers
{
    public class BlogsController : Controller
    {
        private readonly IblogPostRepository BlogPostRepository;
        private readonly IBlogPostLikeRepository BlogPostLikeRepository;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
          private readonly IBlogPostCommentRepository blogPostCommentRepository;
        // GET: BlogsController
        public BlogsController(IblogPostRepository blogPostRepository,
             IBlogPostLikeRepository blogPostLikeRepository,
             SignInManager<IdentityUser> signInManager,
             UserManager<IdentityUser> userManager,IBlogPostCommentRepository blogPostCommentRepository
          )
        {
            this.BlogPostRepository = blogPostRepository;
            this.BlogPostLikeRepository = blogPostLikeRepository;
            this.signInManager = signInManager;
            this.userManager = userManager;
           this.blogPostCommentRepository = blogPostCommentRepository;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string urlHandle)
        {
            var liked = false;
            var blogPost = await BlogPostRepository.GetByUrlHandleAsync(urlHandle);
            var blogDetailsViewModel = new BlogDetailsViewModel();

            if (blogPost != null)
            {
                var totalLikes = await BlogPostLikeRepository.GetTotalLikes(blogPost.Id);

                if (signInManager.IsSignedIn(User))
                {
                    // Get like for this blog for this user
                    var likesForBlog = await BlogPostLikeRepository.GetLikesForBlog(blogPost.Id);

                    var userId = userManager.GetUserId(User);

                    if (userId != null)
                    {
                        var likeFromUser = likesForBlog.FirstOrDefault(x => x.UserId == Guid.Parse(userId));
                        liked = likeFromUser != null;
                    }


                }

                // Get comments for blog post
                var blogCommentsDomainModel = await blogPostCommentRepository.GetCommentsByBlogIdAsync(blogPost.Id);

                var blogCommentsForView = new List<BlogComment>();

                foreach (var blogComment in blogCommentsDomainModel)
                {
                    blogCommentsForView.Add(new BlogComment
                    {
                        Description = blogComment.Description,
                        DateAdded = blogComment.DateAdded,
                        Username = (await userManager.FindByIdAsync(blogComment.UserId.ToString())).UserName
                    });
                }

                blogDetailsViewModel = new BlogDetailsViewModel
                {
                    Id = blogPost.Id,
                    Content = blogPost.Content,
                    PageTitle = blogPost.PageTitle,
                    Author = blogPost.Author,
                    FeaturedImageUrl = blogPost.FeaturedImageUrl,
                    Heading = blogPost.Heading,
                    PublishedDate = blogPost.PublishedDate,
                    ShortDescription = blogPost.ShortDescription,
                    UrlHandle = blogPost.UrlHandle,
                    Visible = blogPost.Visible,
                    Tags = blogPost.Tags,
                    TotalLikes = totalLikes,
                    Liked = liked,
                    Comments = blogCommentsForView
                };

            }

            return View(blogDetailsViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Index(BlogDetailsViewModel blogDetailsViewModel)
        {
            if (signInManager.IsSignedIn(User))
            {
                var domainModel = new BlogPostComment
                {
                    BlogPostId = blogDetailsViewModel.Id,
                    Description = blogDetailsViewModel.CommentDescription,
                    UserId = Guid.Parse(userManager.GetUserId(User)),
                    DateAdded = DateTime.Now
                };

                await blogPostCommentRepository.AddAsync(domainModel);
                return RedirectToAction("Index", "Blogs",
                    new { urlHandle = blogDetailsViewModel.UrlHandle });
            }

            return View();
        }
        // GET: BlogsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BlogsController/Create
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

        // GET: BlogsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BlogsController/Edit/5
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

        // GET: BlogsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BlogsController/Delete/5
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
