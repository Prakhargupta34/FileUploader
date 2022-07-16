using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FileUploader.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using FileUploader.Attributes;
using FileUploader.Shared.Constants;

namespace FileUploader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ValidateModel]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FilesController(IFileService fileService)
        {
            _fileService = fileService;
        }
        
        [HttpPost("Upload")]
        [Authorize(Roles = $"{Shared.Constants.Roles.Administrator}, {Shared.Constants.Roles.User}")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var clientId = GetClientId();
            _fileService.UploadFile(file, clientId).GetAwaiter().GetResult();
            return Ok();
        }
        
        [HttpGet]
        [Authorize(Roles = $"{Shared.Constants.Roles.Administrator}, {Shared.Constants.Roles.User}")]
        public async Task<IActionResult> GetFile([FromQuery]string fileName)
        {
            var clientId = GetClientId();
            var response = _fileService.DownloadFile(fileName, clientId).GetAwaiter().GetResult();
            return File(response.ResponseStream, response.ContentType, response.FileName);
        }
        
        [HttpGet("GetShareableUrl")]
        [Authorize(Roles = $"{Shared.Constants.Roles.Administrator}, {Shared.Constants.Roles.User}")]
        public async Task<IActionResult> GetShareableUrl([FromQuery]string fileName, [FromQuery] int expiryInMins)
        {
            var clientId = GetClientId();
            var url = _fileService.GetShareableUrl(fileName, expiryInMins, clientId).GetAwaiter().GetResult();
            return Ok(url);
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
}