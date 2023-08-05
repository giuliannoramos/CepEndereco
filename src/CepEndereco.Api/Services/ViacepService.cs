using CepEndereco.Api.Dtos;
using CepEndereco.Api.Interfaces;

namespace CepEndereco.Api.Services
{
    public class ViacepService : IViacepService
    {
        private readonly HttpClient _httpClient;

        public ViacepService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<EnderecoDto> ObterEnderecoPorCepAsync(string cep)
        {
            string url = $"https://viacep.com.br/ws/{cep}/json/";
            var response = await _httpClient.GetFromJsonAsync<EnderecoDto>(url);
            return response;
        }
    }
}