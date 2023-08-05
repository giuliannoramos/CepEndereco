using System;
using System.Text.Json;
using System.Threading.Tasks;
using CepEndereco.Api.Interfaces;
using StackExchange.Redis;

namespace CepEndereco.Api.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _redisDatabase;

        public RedisService(ConnectionMultiplexer redisConnection)
        {
            _redisDatabase = redisConnection.GetDatabase();
        }

        // Obtem dados em cache com base na chave fornecida
        public async Task<T> ObterDadosCacheAsync<T>(string chave)
        {
            RedisValue dadosJson = await _redisDatabase.StringGetAsync(chave);

            if (dadosJson.IsNullOrEmpty)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(dadosJson);
        }

        // Salva os dados em cache com uma chave e tempo de expiração fornecidos
        public async Task SalvarDadosCacheAsync<T>(string chave, T dados, TimeSpan tempoExpiracao)
        {
            if (dados == null)
            {
                throw new ArgumentNullException(nameof(dados));
            }

            string dadosJson = JsonSerializer.Serialize(dados);
            await _redisDatabase.StringSetAsync(chave, dadosJson, tempoExpiracao);
        }
    }
}