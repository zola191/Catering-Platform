using Catering.Platform.API.Validators;
using Catering.Platform.API.ViewModels;
using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Applications.Features.Categories.Create;
using Catering.Platform.Applications.Features.Categories.GetAll;
using Catering.Platform.Applications.Validators;
using Catering.Platform.Domain.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catering.Platform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private ICategoryService _categoryService;
        private CreateCategoryCommandValidator _createCategoryRequestValidator;
        private UpdateCategoryRequestValidator _updateCategoryRequestValidator;
        private IMediator _mediator;

        public CategoriesController(
            ICategoryService categoryService,
            CreateCategoryCommandValidator createCategoryRequestValidator,
            UpdateCategoryRequestValidator updateCategoryRequestValidator,
            IMediator mediator)
        {
            _categoryService = categoryService;
            _createCategoryRequestValidator = createCategoryRequestValidator;
            _updateCategoryRequestValidator = updateCategoryRequestValidator;
            _mediator = mediator;
        }

        // дублирование кода в методах при трасформации, использовать automapper(удобно при сложных моделях с вложениями,
        // но стал платным, много рефлексии внутри automapper-a, желательно избегать automapper)
        [HttpGet]
        public async Task<ActionResult> Categories([FromQuery] GetAllQuery query)
        {
            // если swagger не сможет, то создать пустой GetAllQuery для передачи в _mediator

            var result = await _mediator.Send(query);
            var categoryViewModels = result.Select(CategoryViewModel.MapToViewModel);
            return Ok(categoryViewModels);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Category([FromRoute] Guid id, CancellationToken ct = default)
        {
            var result = await _categoryService.GetByIdAsync(id, ct);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateCategoryCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }

            catch (FluentValidation.ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            
            catch (Exception ex)
            {
                //вернуть 500 internal server error
                return BadRequest();
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Guid>> Update(
            [FromRoute] Guid id,
            [FromBody] UpdateCategoryCommand command,
            CancellationToken ct = default)
        {
            var validationResult = await _updateCategoryRequestValidator.ValidateAsync(command, ct);

            if (validationResult.IsValid)
            {
                var result = await _categoryService.UpdateAsync(id, command, ct);
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