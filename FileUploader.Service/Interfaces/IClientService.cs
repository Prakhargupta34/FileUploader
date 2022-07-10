using System.Threading.Tasks;
using FileUploader.Service.Models.RequestModels;
using FileUploader.Service.Models.ResponseModels;

namespace FileUploader.Service;

public interface IClientService
{
    Task<ClientResponse> CreateClient(ClientRequest clientRequest);
}