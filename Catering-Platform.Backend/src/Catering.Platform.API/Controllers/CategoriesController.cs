using Catering.Platform.API.Models;
using Catering.Platform.API.Validators;
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
        private UpdateCategoryRequestValidator _updateCategoryRequestValidator;

        public CategoriesController(
            ICategoryService categoryService,
            CreateCategoryRequestValidator createCategoryRequestValidator,
            UpdateCategoryRequestValidator updateCategoryRequestValidator)
        {
            _categoryService = categoryService;
            _createCategoryRequestValidator = createCategoryRequestValidator;
            _updateCategoryRequestValidator = updateCategoryRequestValidator;
        }

        // дублирование кода в методах при трасформации, использовать automapper(удобно при сложных моделях с вложениями,
        // но стал платным, много рефлексии внутри automapper-a, желательно избегать automapper)
        [HttpGet]
        public async Task<ActionResult> Categories(CancellationToken ct = default)
        {
            var result = await _categoryService.GetAllAsync(ct);
            var categoryViewModels = result.Select(CategoryViewModel.MapFrom);
            return Ok(categoryViewModels);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Category([FromRoute] Guid id, CancellationToken ct = default)
        {
            var result = await _categoryService.GetByIdAsync(id, ct);
            if (result != null)
            {
                var categoryViewModel = CategoryViewModel.MapFrom(result);
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
            [FromBody] UpdateCategoryRequest request,
            CancellationToken ct = default)
        {
            var validationResult = await _updateCategoryRequestValidator.ValidateAsync(request, ct);

            if (validationResult.IsValid)
            {
                var result = await _categoryService.UpdateAsync(id, request, ct);
                return Ok(result);
            }

            return BadRequest();
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Guid>> Delete(
            [FromRoute] Guid id,
            CancellationToken ct = default)
        {
            var result = await _categoryService.DeleteAsync(id, ct);
            return Ok(result);
        }
    }
}