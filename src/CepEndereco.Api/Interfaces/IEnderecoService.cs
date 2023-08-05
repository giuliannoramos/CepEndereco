using System;
using System.Threading.Tasks;
using CepEndereco.Api.Dtos;

namespace CepEndereco.Api.Interfaces
{
    public interface IEnderecoService
    {
        // Busca o endereço pelo CEP fornecido
        Task<EnderecoResponse<EnderecoDto>> BuscarEndereco(string cep);
    }
}