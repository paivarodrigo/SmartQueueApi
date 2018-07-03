using Api.Dac;
using Api.Models;
using Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ContasController : Controller
    {
        private readonly IContaDac _contaDac;
        private readonly ILogger _logger;

        public ContasController(IContaDac contaDac, ILogger<ContasController> logger)
        {
            _contaDac = contaDac;
            _logger = logger;
        }

        [HttpGet]
        [Route("ConsultarConta/{reservaId}")]
        public IActionResult ConsultarConta(int reservaId)
        {
            try
            {
                Historico historicoDeHoje = _contaDac.ConsultarPorReservaId(reservaId);
                if (historicoDeHoje == null)
                    return NotFound("Não há uma conta em aberto.");

                return Ok(historicoDeHoje);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.ContasConsultarConta, ex, ex.Message);
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }

        [HttpPost]
        [Route("RealizarPedido")]
        public IActionResult RealizarPedido([FromBody] JObject jObject)
        {
            try
            {
                if (jObject == null)
                    return BadRequest("Não foi possível fazer o pedido.");

                Conta conta = jObject["conta"]?.ToObject<Conta>();
                IEnumerable<Produto> produtos = jObject["produtos"]?.ToObject<IEnumerable<Produto>>();

                if (conta == null || conta.Id == 0 || produtos == null || !produtos.Any())
                    return BadRequest("Não foi possível fazer o pedido.");

                IEnumerable<ItemPedido> itensPedido = produtos
                    .GroupBy(x => x.Id)
                    .Select(produto => new ItemPedido()
                    {
                        ProdutoId = produto.Key,
                        Quantidade = produto.Count()
                    });

                Pedido pedido = _contaDac.AdicionarPedido(conta.Id, itensPedido);

                if (pedido == null)
                    return BadRequest("Não foi possível fazer o pedido.");

                return Ok(pedido);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.ContasRealizarPedido, ex, ex.Message);
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }

        [HttpPost]
        [Route("EncerrarPedido")]
        public IActionResult EncerrarPedido([FromBody] int pedidoId)
        {
            try
            {
                bool pedidoEncerrado = _contaDac.EncerrarPedido(pedidoId);
                if (!pedidoEncerrado)
                    return NotFound("Este pedido não existe ou já foi encerrado.");

                return Ok("Pedido encerrado com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.ContasEncerrarPedido, ex, ex.Message);
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }
    }
}