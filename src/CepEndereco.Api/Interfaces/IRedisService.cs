using System;
using System.Threading.Tasks;
using CepEndereco.Api.Dtos;

namespace CepEndereco.Api.Interfaces
{
    public interface IRedisService
    {
        // Obtém dados em cache com base na chave fornecida
        Task<T> ObterDadosCacheAsync<T>(string chave);

        // Salva os dados em cache com uma chave e tempo de expiração fornecidos
        Task SalvarDadosCacheAsync<T>(string chave, T dados, TimeSpan tempoExpiracao);
    }
}