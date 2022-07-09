using FileUploader.Models;
using FileUploader.Service;
using FileUploader.Service.Impl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using FileUploader.Database.Impl;
using FileUploader.exception;
using System;

namespace FileUploader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;
        private ILoginService _loginService;

        public LoginController(IConfiguration config)
        {
            _config = config;
            _loginService = new LoginService(new FileUploader.Database.Impl.Database());
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] UserLogin userLogin)
        {
            try
            {
                string token = _loginService.LoginAndCreateToken(userLogin.UserName, userLogin.Password, _config);
                return Ok(token);
            } catch (UserNotFoundException e)
            {
                return NotFound(e.Message);
            } catch (Exception e)
            {
                throw e;
            }
        }
    }
}
