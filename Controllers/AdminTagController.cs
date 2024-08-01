using FORMATION.Data;
using FORMATION.Models.Domain;
using FORMATION.Models.ViewModels;
using FORMATION.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FORMATION.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminTagController : Controller
    {
       
        private ITagRepository tagRepository;
        public AdminTagController(ITagRepository tagRepository)
        {
          
            this.tagRepository = tagRepository;


        }
        // GET: AdminTagController
        public ActionResult Index()
        {
            return View();
        }

        // GET: AdminTagController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdminTagController/Create
      
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
    
        [HttpPost]
        [ActionName("Add")]
        public async Task<IActionResult> Add(AddTagRequest addTagRequest)
        {
            ValidateAddTagRequest(addTagRequest);

            if (ModelState.IsValid == false)
            {
                return View();
            }

            // Mapping AddTagRequest to Tag domain model
            var tag = new Tag
            {
                Name = addTagRequest.Name,
                DisplayName = addTagRequest.DisplayName
            };

            await tagRepository.AddAsync(tag);

            return RedirectToAction("List");
        }

        [HttpGet]
        [ActionName("List")]
        public async Task<IActionResult> List(
           string? searchQuery,
           string? sortBy,
           string? sortDirection,
           int pageSize = 3,
           int pageNumber = 1)
        {
            var totalRecords = await tagRepository.CountAsync();
            var totalPages = Math.Ceiling((decimal)totalRecords / pageSize);

            if (pageNumber > totalPages)
            {
                pageNumber--;
            }

            if (pageNumber < 1)
            {
                pageNumber++;
            }


            ViewBag.TotalPages = totalPages;
            ViewBag.SearchQuery = searchQuery;
            ViewBag.SortBy = sortBy;
            ViewBag.SortDirection = sortDirection;
            ViewBag.PageSize = pageSize;
            ViewBag.PageNumber = pageNumber;

            // use dbContext to read the tags
            var tags = await tagRepository.GetAllAsync(searchQuery, sortBy, sortDirection, pageNumber, pageSize);

            return View(tags);
        }

        // GET: AdminTagController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var tag = await tagRepository.GetAsync(id);

            if (tag != null)
            {
                var editTagRequest = new EditTagRequest
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    DisplayName = tag.DisplayName
                };

                return View(editTagRequest);
            }

            return View(null);
        }
   
        [HttpPost]
        public async Task<IActionResult> Edit(EditTagRequest editTagRequest)
        {
            var tag = new Tag
            {
                Id = editTagRequest.Id,
                Name = editTagRequest.Name,
                DisplayName = editTagRequest.DisplayName
            };

            var updatedTag = await tagRepository.UpdateAsync(tag);

            if (updatedTag != null)
            {
                // Show success notification
            }
            else
            {
                // Show error notification
            }

            return RedirectToAction("Edit", new { id = editTagRequest.Id });
        }
    
        [HttpPost]
        public async Task<IActionResult> Delete(EditTagRequest editTagRequest)
        {
            var deletedTag = await tagRepository.DeleteAsync(editTagRequest.Id);

            if (deletedTag != null)
            {
                // Show success notification
                return RedirectToAction("List");
            }

            // Show an error notification
            return RedirectToAction("Edit", new { id = editTagRequest.Id });
        }
        private void ValidateAddTagRequest(AddTagRequest addTagRequest)
        {
            if (addTagRequest.Name is not not null && addTagRequest.DisplayName is not null)
            {
                if (addTagRequest.Name == addTagRequest.DisplayName)
                {
                    ModelState.AddModelError("DisplayName", "Name cannot be the same as display  ");
                }
            }
        }








    }
}
