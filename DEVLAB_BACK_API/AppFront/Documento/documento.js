// Ajustado para incluir o caminho padrão da controller C#
const URL_API = 'https://localhost:7081/api/v1/Documento';

// HU01: Busca de Documentos por Código do Cliente
async function buscarDocumentos() {
    // Certifique-se de que seu HTML tem id="buscaCodigoCliente" no campo de busca
    const codigoCliente = document.getElementById("buscaCodigoCliente")?.value;

    if (!codigoCliente) {
        alert("Informe o código do cliente para buscar.");
        return;
    }

    try {
        // Rota no backend para buscar por cliente
        const response = await fetch(`${URL_API}/cliente/${codigoCliente}`);

        if (response.ok) {
            const documentos = await response.json();
            renderizarTabela(documentos);
        } else {
            alert("Nenhum documento encontrado ou falha na busca.");
            renderizarTabela([]);
        }
    } catch (error) {
        console.error("Erro na busca:", error);
        alert("Erro ao conectar com o servidor.");
    }
}

function renderizarTabela(documentos) {
    const tbody = document.getElementById("tabelaDocumentos");
    if (!tbody) return;

    tbody.innerHTML = "";

    documentos.forEach(doc => {
        const tr = document.createElement("tr");

        tr.innerHTML = `
            <td>${doc.id}</td>
            <td>${doc.nome}</td>
            <td>${doc.extensao}</td>
            <td>
                <button class="btn btn-warning" style="background-color: #ffc107; border: none; padding: 5px 10px; cursor: pointer;"
                        onclick="baixarDocumento('${doc.id}', '${doc.nome}')">
                    Baixar
                </button>
                <button class="btn btn-danger" style="background-color: #dc3545; color: white; border: none; padding: 5px 10px; cursor: pointer;"
                        onclick="excluirDocumento('${doc.id}')">
                    Excluir
                </button>
            </td>
        `;
        tbody.appendChild(tr);
    });
}

// HU02: Download de Arquivo
async function baixarDocumento(id, nomeArquivo) {
    try {
        const response = await fetch(`${URL_API}/download/${id}`);
        if (!response.ok) throw new Error("Falha no download");

        const blob = await response.blob();
        const urlBlob = window.URL.createObjectURL(blob);

        const a = document.createElement("a");
        a.href = urlBlob;
        a.download = nomeArquivo || "documento";
        document.body.appendChild(a);
        a.click();
        a.remove();
        window.URL.revokeObjectURL(urlBlob);
    } catch (error) {
        console.error("Erro no download:", error);
        alert("Erro ao realizar o download do arquivo.");
    }
}

// HU03: Exclusão de Arquivo
async function excluirDocumento(id) {
    if (!confirm("Deseja realmente excluir este documento?")) return;

    try {
        const response = await fetch(`${URL_API}/${id}`, {
            method: "DELETE"
        });

        if (response.ok) {
            alert("Documento excluído com sucesso!");
            buscarDocumentos();
        } else {
            alert("Erro ao excluir o documento.");
        }
    } catch (error) {
        console.error("Erro na exclusão:", error);
        alert("Erro ao conectar com o servidor.");
    }
}

// HU04: Upload com Atualização Automática
async function enviarDocumento() {
    const codigoClienteInput = document.getElementById("codigoCliente");
    const inputArquivo = document.getElementById("arquivo");

    const codigoCliente = codigoClienteInput?.value;
    const arquivo = inputArquivo?.files[0];

    if (!codigoCliente || !arquivo) {
        alert("Informe o código do cliente e selecione um arquivo.");
        return;
    }

    const dadosArquivo = new FormData();
    // A chave 'arquivo' precisa ter exatamente o mesmo nome da propriedade tratada no backend (ex: IFormFile arquivo)
    dadosArquivo.append("arquivo", arquivo);

    try {
        const response = await fetch(`${URL_API}/upload/${codigoCliente}`, {
            method: "POST",
            body: dadosArquivo
        });

        if (response.ok) {
            alert("Documento enviado com sucesso!");

            // Limpa o formulário de envio
            codigoClienteInput.value = "";
            inputArquivo.value = "";

            // Atualização Automática (HU04): Preenche o campo de busca e recarrega os dados
            const buscaInput = document.getElementById("buscaCodigoCliente");
            if (buscaInput) {
                buscaInput.value = codigoCliente;
            }
            await buscarDocumentos();
        } else {
            let mensagemErro = "Falha ao enviar o documento";
            try {
                const erroObj = await response.json();
                mensagemErro = erroObj.message || mensagemErro;
            } catch {
                mensagemErro = await response.text() || mensagemErro;
            }
            alert("Erro: " + mensagemErro);
        }
    } catch (error) {
        console.error("Erro na requisição:", error);
        alert("Erro ao conectar com o servidor. Verifique se o backend está executando.");
    }
}