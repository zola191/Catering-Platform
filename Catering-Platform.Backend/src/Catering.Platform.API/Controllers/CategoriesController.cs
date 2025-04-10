using Catering.Platform.API.Models;
using Catering.Platform.API.Requests;
using Catering.Platform.API.Validators;
using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Requests;
using Catering.Platform.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Catering.Platform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private ICategoryService _categoryService;
        private CreateCategoryRequestValidator _createCategoryRequestValidator;

        public CategoriesController(
            ICategoryService categoryService,
            CreateCategoryRequestValidator createCategoryRequestValidator)
        {
            _categoryService = categoryService;
            _createCategoryRequestValidator = createCategoryRequestValidator;
        }

        [HttpGet]
        public async Task<ActionResult> Categories(CancellationToken ct = default)
        {
            var result = await _categoryService.GetAllAsync(ct);
            // дублирование кода в методах при трасформации, использовать automapper(удобно при сложных моделях с вложениями,
            // но стал платным, много рефлексии внутри automapper-a, желательно избегать automapper)
            // 
            var categoryViewModels = result.Select(CategoryViewModel.MapFrom);
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
            // добавить валидацию на разрешенные значения см. на правила entity слоя Domain
            var validationResult = await _createCategoryRequestValidator.ValidateAsync(request, ct);
            if (validationResult.IsValid)
            {
                //1. создать отдельный сервис для Mapping
                //2. передать request в _categoryService, и логику преобразования выполнить в _categoryService что было сделано
                var result = await _categoryService.AddAsync(request, ct);
                return Ok(result);
            }
            // open problem объекты для межкоммуникационного взаимодействия
            // 

            return BadRequest(validationResult.Errors);
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