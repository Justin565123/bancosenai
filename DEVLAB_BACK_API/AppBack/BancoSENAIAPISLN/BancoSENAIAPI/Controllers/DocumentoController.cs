using Microsoft.AspNetCore.Mvc;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DocumentoController : Controller
    {
        private readonly string _caminhoRaiz = Path.Combine(
            Directory.GetCurrentDirectory(),
            "ClienteArquivos"
        );

        private static List<Models.DocumentoMetadado> _documentosMetadados = new List<Models.DocumentoMetadado>();
        private static int _nextId = 1;

        [HttpPost("upload/{codigoCliente}")]
        public async Task<IActionResult> AnexarArquivo(int codigoCliente, IFormFile arquivo)
        {
            
            if (arquivo == null || arquivo.Length == 0)
            {
                return BadRequest("Nenhum arquivo foi enviado.");
            }

          
            long limiteEmBytes = 2 * 1024 * 1024; // 2 MB
            if (arquivo.Length > limiteEmBytes)
            {
                return BadRequest("O tamanho do arquivo excede o limite máximo permitido de 2 MB.");
            }
            // ==========================================
            // R06G: Validação de Extensões Permitidas (.pdf, .jpg, .png)
            // ==========================================
            string extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();
            string[] extensoesPermitidas = { ".pdf", ".jpg", ".png" };

            if (!extensoesPermitidas.Contains(extensao))
            {
                return BadRequest($"A extensão '{extensao}' não é permitida. Apenas arquivos .pdf, .jpg e .png são homologados.");
            }

            // --- PROCESSAMENTO DO ARQUIVO ---
            string pastaCliente = Path.Combine(_caminhoRaiz, codigoCliente.ToString());

            if (!Directory.Exists(pastaCliente))
            {
                Directory.CreateDirectory(pastaCliente);
            }


            string nameOriginal = Path.GetFileNameWithoutExtension(arquivo.FileName);
            string novoNome = $"{codigoCliente}_{nameOriginal}_{Guid.NewGuid()}{extensao}";
            string caminhoFinal = Path.Combine(pastaCliente, novoNome);

            using (var stream = new FileStream(caminhoFinal, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            var documentoMetadados = new Models.DocumentoMetadado
            {
                Id = _nextId++,
                Name = nameOriginal,
                Extensao = extensao,
                Caminho = caminhoFinal,
                CodigoCliente = codigoCliente,
            };

            _documentosMetadados.Add(documentoMetadados);

            return Ok(new { mensagem = "Documento anexado com sucesso", arquivoSalvo = novoNome });
        }

        [HttpGet("listar/{codigoCliente}")]
        public IActionResult Listagem(int codigoCliente)
        {
            var documentos = _documentosMetadados
                .Where(d => d.CodigoCliente == codigoCliente)
                .ToList();

            if (!documentos.Any())
            {
                return NotFound(new { mensagem = $"Nenhum documento foi encontrado, verifique seu cadastro. {codigoCliente}" });
            }

            return Ok(documentos);
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> Download(int id)
        {
            var documento = _documentosMetadados.FirstOrDefault(d => d.Id == id);

            if (documento == null)
            {
                return NotFound(new { mensagem = "Documento não encontrado." });
            }

            if (!System.IO.File.Exists(documento.Caminho))
            {
                return NotFound(new { mensagem = "O arquivo físico não foi encontrado no servidor." });
            }

            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(documento.Caminho);
            string nomeArquivoCompleto = $"{documento.Name}{documento.Extensao}";

            return File(fileBytes, "application/octet-stream", nomeArquivoCompleto);
        }

        [HttpDelete("excluir/{id}")]
        public IActionResult Excluir(int id)
        {
            var documento = _documentosMetadados.FirstOrDefault(d => d.Id == id);

            if (documento == null)
            {
                return NotFound(new { mensagem = "Documento não encontrado." });
            }

            if (System.IO.File.Exists(documento.Caminho))
            {
                System.IO.File.Delete(documento.Caminho);
            }

            _documentosMetadados.Remove(documento);

            return Ok(new { mensagem = "Documento e arquivo removidos com sucesso!" });
        }
    }
}