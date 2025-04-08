using Catering.Platform.API.Models;
using Catering.Platform.API.Requests;
using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Catering.Platform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult> Categories(CancellationToken ct = default)
        {
            var result = await _categoryService.GetAllAsync(ct);
            var categoryViewModels = result.Select(c => new CategoryViewModel()
            {
                Name = c.Name,
                Description = c.Description
            });
            return Ok(categoryViewModels);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Category([FromRoute] Guid id, CancellationToken ct = default)
        {
            var result = await _categoryService.GetByIdAsync(id, ct);
            if (result != null)
            {
                var categoryViewModel = new CategoryViewModel()
                {
                    Name = result.Name,
                    Description = result.Description
                };
                return Ok(categoryViewModel);
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create(
            [FromBody] CreateCategoryRequest request,
            CancellationToken ct = default)
        {
            var category = new Category()
            {
                Name = request.Name,
                Description = request.Description
            };

            var result = await _categoryService.UpdateAsync(category, ct);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Guid>> Update(        
            [FromRoute] Guid id,
            UpdateCategoryRequest request, 
            CancellationToken ct = default)
        {
            var existingCategory = await _categoryService.GetByIdAsync(id, ct);
            if (existingCategory != null)
            {
                existingCategory.Name = request.Name;
                existingCategory.Description = request.Description;
                await _categoryService.UpdateAsync(existingCategory, ct);
                return Ok(existingCategory.Id);
            }

            return BadRequest();
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Guid>> Delete(        
            [FromRoute] Guid id,
            CancellationToken ct = default)
        {
            var existingCategory = await _categoryService.GetByIdAsync(id, ct);
            if (existingCategory != null)
            {
                await _categoryService.DeleteAsync(existingCategory, ct);
                return Ok(existingCategory.Id);
            }
            return BadRequest();
        }
    }
}