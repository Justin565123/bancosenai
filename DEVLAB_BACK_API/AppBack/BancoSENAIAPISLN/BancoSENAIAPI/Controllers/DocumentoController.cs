using Microsoft.AspNetCore.Mvc;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/vi/[controller]")]
    public class DocumentoController : Controller
    {
        private readonly string _caminhoRaiz = Path.Combine(
            Directory.GetcurrentDirectory(), 
            "ClienteArquivos"
            );

        private static List<Models.DocumentoMetadado>
            _documentosMetadados
            = new List<Models.DocumentoMetadado>();
        private static int _nextId = 1;

        [HttpPost("upload/{codigoCliente}")]
        public async Task<IActionResult> AnexarArquivo(int codigoCliente, IFormFile arquivo)
        {

        }
    }
}
