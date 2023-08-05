using System.Net;

namespace CepEndereco.Api.Dtos
{
    public class EnderecoResponse<T>
    {
        // Propriedade que armazena os dados de retorno
        public T? DadosRetorno { get; set; }

        // Propriedade para indicar a fonte dos dados (ViaCEP ou cache)
        public string? FonteDados { get; set; }

        // Propriedade que representa o código de status HTTP da resposta
        public HttpStatusCode CodigoHttp { get; set; }

        // Propriedade que indica o erro
        public string? ErroRetorno { get; set; }        
    }
}