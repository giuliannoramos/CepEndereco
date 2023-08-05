using System.Net;
using System.Text.RegularExpressions;
using CepEndereco.Api.Dtos;
using CepEndereco.Api.Interfaces;

namespace CepEndereco.Api.Services
{
    public partial class EnderecoService : IEnderecoService
    {
        private readonly IViacepService _viacepService;
        private readonly IRedisService _redisService;

        public EnderecoService(IViacepService viacepService, IRedisService redisService)
        {
            _viacepService = viacepService;
            _redisService = redisService;
        }

        public async Task<EnderecoResponse<EnderecoDto>> BuscarEndereco(string cep)
        {
            // Valida o CEP fornecido
            var enderecoResponse = ValidarCEP(cep);

            // Se vier diferente de nulo retorna o erro
            if (enderecoResponse != null)
                return enderecoResponse;

            // Verifica se o endereço está em cache
            var nomeChave = $"endereco_{cep}";
            var enderecoRedis = await _redisService.ObterDadosCacheAsync<EnderecoResponse<EnderecoDto>>(nomeChave);

            if (enderecoRedis != null)
            {
                enderecoRedis.FonteDados = "Cache";
                return enderecoRedis;
            }

            try
            {
                // Consulta o serviço externo para obter informações do endereço pelo CEP
                var enderecoViaCep = await _viacepService.ObterEnderecoPorCepAsync(cep);

                if (enderecoViaCep == null || string.IsNullOrEmpty(enderecoViaCep.Cep))
                {
                    // Retorna erro caso o CEP não seja encontrado
                    enderecoResponse = new EnderecoResponse<EnderecoDto>
                    {
                        DadosRetorno = null,
                        FonteDados = "ViaCEP",
                        CodigoHttp = HttpStatusCode.NotFound,
                        ErroRetorno = "CEP não encontrado"
                    };
                }
                else
                {
                    // Retorna os dados do endereço encontrado e armazena em cache
                    enderecoResponse = new EnderecoResponse<EnderecoDto>
                    {
                        DadosRetorno = enderecoViaCep,
                        FonteDados = "ViaCEP",
                        CodigoHttp = HttpStatusCode.OK,
                        ErroRetorno = null
                    };

                    await _redisService.SalvarDadosCacheAsync(nomeChave, enderecoResponse, TimeSpan.FromSeconds(3600));
                }

                return enderecoResponse;
            }
            catch (Exception ex)
            {
                // Tratar erros de consulta à API de CEP ou de cache.
                return new EnderecoResponse<EnderecoDto>
                {
                    DadosRetorno = null,
                    FonteDados = "ViaCEP",
                    CodigoHttp = HttpStatusCode.InternalServerError,
                    ErroRetorno = "Erro ao consultar o CEP: " + ex.Message
                };
            }
        }

        // Método privado para validar o formato do CEP fornecido.
        private static EnderecoResponse<EnderecoDto> ValidarCEP(string cep)
        {
            // Verifica se é vazio
            if (string.IsNullOrEmpty(cep))
            {
                return new EnderecoResponse<EnderecoDto>
                {
                    DadosRetorno = null,
                    FonteDados = "N/A",
                    CodigoHttp = HttpStatusCode.BadRequest,
                    ErroRetorno = "CEP não fornecido"
                };
            }

            // Verifica se o CEP é somente números
            if (!Regex.IsMatch(cep, "^[0-9]{8}$"))
            {
                return new EnderecoResponse<EnderecoDto>
                {
                    DadosRetorno = null,
                    FonteDados = "N/A",
                    CodigoHttp = HttpStatusCode.BadRequest,
                    ErroRetorno = "CEP inválido. O CEP deve conter exatamente 8 dígitos numéricos."
                };
            }

            // Verifica se o CEP não é composto por todos os dígitos iguais
            if (cep.Distinct().Count() == 1)
            {
                return new EnderecoResponse<EnderecoDto>
                {
                    DadosRetorno = null,
                    FonteDados = "N/A",
                    CodigoHttp = HttpStatusCode.BadRequest,
                    ErroRetorno = "CEP inválido. O CEP não pode ser composto por todos os dígitos iguais."
                };
            }

            // Verifica se o CEP não começa com o dígito 0
            if (cep.StartsWith("0"))
            {
                return new EnderecoResponse<EnderecoDto>
                {
                    DadosRetorno = null,
                    FonteDados = "N/A",
                    CodigoHttp = HttpStatusCode.BadRequest,
                    ErroRetorno = "CEP inválido. O CEP não pode começar com o dígito 0."
                };
            }

            // Se o CEP passou por todas as verificações, então é considerado válido
            // e retornamos null, indicando que não há erros na validação.
            return null;
        }
    }
}