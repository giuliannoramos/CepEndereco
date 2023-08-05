using Microsoft.AspNetCore.Mvc;
using CepEndereco.Api.Interfaces;
using MassTransit;

namespace CepEndereco.Api.Controllers;

[ApiController]
[Route("/api")]
public partial class CepEnderecoController : ControllerBase
{
    private readonly IEnderecoService _enderecoService;
    private readonly IPublishEndpoint _publishEndpoint;

    public CepEnderecoController(IEnderecoService enderecoService, IPublishEndpoint publishEndpoint)
    {
        _enderecoService = enderecoService;
        _publishEndpoint = publishEndpoint;
    }

    [HttpGet("{cep}")]
    public async Task<IActionResult> BuscarEnderecoPorCep(string cep)
    {
        var endereco = await _enderecoService.BuscarEndereco(cep);

        // Enviar mensagem RabbitMQ
        await _publishEndpoint.Publish(endereco);

        return Ok(endereco);
    }
}