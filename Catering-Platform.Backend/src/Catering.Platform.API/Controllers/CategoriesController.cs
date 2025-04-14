using Catering.Platform.API.Contracts;
using Catering.Platform.API.ViewModels;
using Catering.Platform.Applications.Features.Categories.Create;
using Catering.Platform.Applications.Features.Categories.Delete;
using Catering.Platform.Applications.Features.Categories.GetAll;
using Catering.Platform.Applications.Features.Categories.GetById;
using Catering.Platform.Applications.Features.Categories.Update;
using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catering.Platform.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper<Category, CategoryViewModel> _mapper;
    public CategoriesController(
        IMediator mediator, 
        IMapper<Category, CategoryViewModel> mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult> Categories([FromQuery] GetAllCategoriesQuery categoriesQuery)
    {
        // если swagger не сможет, то создать пустой GetAllQuery для передачи в _mediator
        var result = await _mediator.Send(categoriesQuery);
        var categoryViewModels = result.Select(CategoryViewModel.MapToViewModel);
        return Ok(categoryViewModels);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> Category([FromRoute] Guid id, CancellationToken ct = default)
    {
        var command = new GetCategoryByIdQuery() { Id = id };
        var result = await _mediator.Send(command, ct);
        var categoryViewModel = _mapper.MapToModel(result);
        return Ok(categoryViewModel);
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
            return Problem(
                title: "Internal Server Error",
                detail: "An unexpected error occurred. Please try again later.",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Guid>> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateCategoryRequest request,
        CancellationToken ct = default)
    {
        var command = new UpdateCategoryCommand()
        {
            Id = id,
            Name = request.Name,
            Description = request.Name
        };

        try
        {
            var result = await _mediator.Send(command, ct);
            return Ok(result);
        }

        catch (FluentValidation.ValidationException ex)
        {
            return BadRequest(ex.Message);
        }

        catch (Exception ex)
        {
            return Problem(
                title: "Internal Server Error",
                detail: "An unexpected error occurred. Please try again later.",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<Guid>> Delete(
        [FromRoute] Guid id,
        CancellationToken ct = default)
    {
        var command = new DeleteCategoryCommand() { Id = id };
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }
}