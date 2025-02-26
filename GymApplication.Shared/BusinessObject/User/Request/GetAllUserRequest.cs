﻿using System.ComponentModel.DataAnnotations.Schema;
using FluentValidation;
using GymApplication.Shared.Attribute;
using GymApplication.Shared.BusinessObject.User.Response;
using GymApplication.Shared.Common;
using GymApplication.Shared.Emuns;
using MediatR;

namespace GymApplication.Shared.BusinessObject.User.Request;

public sealed class GetAllUserRequestValidation : AbstractValidator<GetAllUserRequest>
{
    public GetAllUserRequestValidation()
    {
        RuleFor(u => u.CurrentPage)
            .GreaterThan(0)
            .WithMessage("CurrentPage must be greater than 0");

        RuleFor(u => u.PageSize)
            .GreaterThan(0)
            .WithMessage("PageSize must be greater than 0");

        RuleFor(u => u.SortBy)
            .Must(u => u is "fullName" or "email" or "phoneNumber" or "dateOfBirth")
            .WithMessage("SortBy must be either fullName, email, phoneNumber or dateOfBirth");

        RuleFor(u => u.SortOrder)
            .Must(u => u is "asc" or "desc")
            .WithMessage("SortOrder must be either asc or desc");
        
        RuleFor(u => u.SearchBy)
            .Must(u => u is "fullName" or "email" or "phoneNumber" or "dateOfBirth")
            .WithMessage("SearchBy must be either fullName, email, phoneNumber or dateOfBirth");
    }
}

public sealed class GetAllUserRequest : IRequest<Result<PagedResult<UserResponse>>>
{
    public string? Search { get; set; }
    public string? SearchBy { get; set; } = "email";
    public string SortOrder { get; set; } = "asc";
    public string? SortBy { get; set; } = "email";
    
    [NotMapped]
    [SwaggerIgnore]
    public Role Role { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}