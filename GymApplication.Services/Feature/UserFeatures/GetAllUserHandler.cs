﻿using System.Linq.Expressions;
using AutoMapper;
using GymApplication.Repository.Entities;
using GymApplication.Services.Abstractions;
using GymApplication.Shared.BusinessObject.User.Request;
using GymApplication.Shared.BusinessObject.User.Response;
using GymApplication.Shared.Common;
using GymApplication.Shared.Emuns;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace GymApplication.Services.Feature.UserFeatures;

public sealed class GetAllUserHandler : IRequestHandler<GetAllUserRequest, Result<PagedResult<UserResponse>>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ICacheServices _cacheServices;

    public GetAllUserHandler(UserManager<ApplicationUser> userManager, IMapper mapper, ICacheServices cacheServices)
    {
        _userManager = userManager;
        _mapper = mapper;
        _cacheServices = cacheServices;
    }

    public async Task<Result<PagedResult<UserResponse>>> Handle(GetAllUserRequest request, CancellationToken cancellationToken)
    {
        var redisKey = $"GetAllUser:{request.CurrentPage}:{request.PageSize}:{request.SortBy}:{request.SortOrder}:{request.Search}:{request.SearchBy}:{request.Role}";

        var cache = await _cacheServices.GetAsync<PagedResult<UserResponse>>(redisKey, cancellationToken);
        
        if (cache is not null)
        {
            return Result.Success(cache);
        }

        // var users = _userManager.Users
        //     .Where(u => u.IsDeleted == false);

        var users = (await _userManager.GetUsersInRoleAsync(request.Role.ToString()))
            .AsQueryable()
            .Where(u => u.IsDeleted == false);
        
        Expression<Func<ApplicationUser, object>> sortBy = request.SortBy switch
        {
            "fullName" => u => u.FullName!,
            "email" => u => u.Email!,
            "phoneNumber" => u => u.PhoneNumber!,
            "dateOfBirth" => u => u.DateOfBirth,
            _ => u => u.Email!
        };
        
        users = request.SortOrder switch
        {
            "asc" => users.OrderBy(sortBy),
            "desc" => users.OrderByDescending(sortBy),
            _ => users.OrderBy(sortBy)
        };
        
        Expression<Func<ApplicationUser, bool>> searchBy = request.SearchBy switch
        {
            "fullName" => u => u.FullName!.Contains(request.Search!),
            "email" => u => u.Email!.Contains(request.Search!),
            "phoneNumber" => u => u.PhoneNumber!.Contains(request.Search!),
            "dateOfBirth" => u => u.DateOfBirth.ToString().Contains(request.Search!),
            _ => u => u.Email!.Contains(request.Search!)
        };
        
        if (!string.IsNullOrEmpty(request.Search))
        {
            users = users.Where(searchBy);
        }
        
        var list = PagedResult<ApplicationUser>.Create(users.ToList(), request.CurrentPage, request.PageSize);
        
        var response = _mapper.Map<PagedResult<UserResponse>>(list);
        
        await _cacheServices.SetAsync(redisKey, response, TimeSpan.FromMinutes(5), cancellationToken);
        
        return Result.Success(response);
    }
}