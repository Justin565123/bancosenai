const URL_API = 'https://localhost:7081/api/v1/Documento';

async function enviarDocumento() {
    const codigoCliente = document.getElementById('codigoCliente').value;
    const inputArquivo = document.getElementById('arquivo');
    const arquivo = inputArquivo.files[0];

    // Validação obrigatória: só envia se cliente e arquivo estiverem preenchidos
    if (!codigoCliente || !arquivo) {
        alert("Informe o código do cliente e selecione um arquivo.");
        return;
    }

    // Upload usa FormData pois a API espera um arquivo (IFormFile), e não JSON
    const dadosArquivo = new FormData();
    dadosArquivo.append('arquivo', arquivo)

    const response = await fetch(`${URL_API}/upload/${codigoCliente}`, {
        method: 'POST',
        body: dadosArquivo
    });

    if (response.ok) {
        alert("Documento enviado com sucesso!");
        limparCampos();
        document.getElementById('codigoClienteBusca').value = codigoCliente;
        listarDocumentos();
    } else {
        alert("Erro: Falha ao enviar o documento");
    }
}

async function listarDocumentos() {
    const codigoCliente = document.getElementById('codigoClienteBusca').value;

    if (!codigoCliente) {
        alert("Informe o código do cliente para buscar.");
        return;
    }

    const response = await fetch(`${URL_API}/listar/${codigoCliente}`);
    const corpo = document.getElementById('corpoTabela');
    corpo.innerHTML = '';

    if (!response.ok) {
        return;
    }

    const documentos = await response.json();

    documentos.forEach(d => {
        corpo.innerHTML += `
            <tr>
                <td>${d.id}</td>
                <td>${d.nome}</td>
                <td>${d.extensao}</td>
                <td>
                    <button class="btn-baixar" onclick="baixarDocumento(${d.id})">Baixar</button>
                    <button class="btn-excluir" onclick="excluirDocumento(${d.id})">Excluir</button>
                </td>
            </tr>`;
    });
}

function baixarDocumento(id) {
    // Abre o download em nova aba usando a rota GET de download
    window.open(`${URL_API}/download/${id}`, '_blank');
}

async function excluirDocumento(id) {
    // Caixa de diálogo de confirmação conforme padrão dos demais módulos
    if (confirm(`Deseja realmente excluir o documento ${id}?`)) {
        const response = await fetch(`${URL_API}/excluir/${id}`, { method: 'DELETE' });
        if (response.ok) {
            listarDocumentos();
        }
    }
}

function limparCampos() {
    document.getElementById('codigoCliente').value = '';
    document.getElementById('arquivo').value = '';
}
