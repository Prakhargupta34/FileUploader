﻿using FileUploader.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using FileUploader.Attributes;
using FileUploader.Service.Interfaces;
using FileUploader.Service.Models.RequestModels;
using FileUploader.Service.Services;

namespace FileUploader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ValidateModel]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;
        private ILoginService _loginService;

        public LoginController(IConfiguration config, ILoginService loginService)
        {
            _config = config;
            _loginService = loginService;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] UserLogin userLogin)
        {
            string token = _loginService.LoginAndCreateToken(userLogin.UserName, userLogin.Password, _config);
            return Ok(token);
        }
    }
}