using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using FileUploader.Attributes;
using FileUploader.Service.Interfaces;
using FileUploader.Service.Models.RequestModels;
using FileUploader.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileUploader.Controllers;


[Route("api/[controller]")]
[ApiController]
[ValidateModel]
public class UserController: ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
        
    [Authorize(Roles = $"{Shared.Constants.Roles.Administrator}")]
    [HttpPost]
    public IActionResult Create([FromBody] User user)
    {
        var clientId = GetClientId();
        _userService.CreateUser(user.ToModel(clientId)).GetAwaiter().GetResult();
        return Ok();
    }
    
    private IEnumerable<Claim> GetClaims()
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;

        return identity?.Claims;
    }

    private string GetClientId()
    {
        return GetClaims()?.FirstOrDefault(o => o.Type == ClaimType.ClientId)?.Value;
    }
}