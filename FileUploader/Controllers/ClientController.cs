using System.Threading.Tasks;
using FileUploader.Attributes;
using FileUploader.Service;
using FileUploader.Service.Models.RequestModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FileUploader.Controllers;

[Route("api/[controller]")]
[ApiController]
[ValidateModel]
public class ClientController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientController(IClientService clientService)
    {
        _clientService = clientService;
    }
    [HttpPost]
    public async Task<IActionResult> CreateClient([FromBody] ClientRequest clientRequest)
    {
        var client = _clientService.CreateClient(clientRequest).GetAwaiter().GetResult();
        return Ok(client);
    }
}
