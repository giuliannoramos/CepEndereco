using System.Threading.Tasks;
using CepEndereco.Api.Dtos;

namespace CepEndereco.Api.Interfaces
{
    public interface IViacepService
    {
        // Obtém um endereço da API ViaCEP com base no CEP fornecido
        Task<EnderecoDto> ObterEnderecoPorCepAsync(string cep);
    }
}