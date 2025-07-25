﻿using Catering.Platform.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Catering.Platform.API.Middlewares
{
    public class ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> _logger)
        : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                var problemDetails = ex switch
                {
                    ValidationException ve => new ProblemDetails
                    {
                        Status = 400,
                        Title = "Validation error",
                        Detail = ve.Message,
                        Extensions = { ["errors"] = ve.Errors }
                    },

                    TenantNotFoundException tnfe => new ProblemDetails
                    {
                        Status = 404,
                        Title = "Tenant not found",
                        Detail = tnfe.Message
                    },

                    TenantInactiveException tie => new ProblemDetails
                    {
                        Status = 403,
                        Title = "Tenant inactive",
                        Detail = tie.Message
                    },

                    _ => new ProblemDetails
                    {
                        Status = 500,
                        Title = "Internal server error",
                        Detail = "An unexpected error occurred"
                    }
                };

                context.Response.StatusCode = problemDetails.Status.Value;
                await context.Response.WriteAsJsonAsync(problemDetails);

                _logger.LogError(ex, "Error processing request: {Message}", ex.Message);
            }
        }
    }
}
