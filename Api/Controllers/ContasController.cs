using Api.Dac;
using Api.Models;
using Api.Utils;
using Microsoft.AspNetCore.Mvc;
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
        private readonly ILoggerDac _logger;

        public ContasController(IContaDac contaDac, ILoggerDac logger)
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
        [Route("CancelarPedido/{pedidoId}")]
        public IActionResult CancelarPedido(int pedidoId)
        {
            try
            {
                bool pedidoCancelado = _contaDac.CancelarPedido(pedidoId);
                if (!pedidoCancelado)
                    return NotFound("Este pedido não existe ou já foi cancelado.");

                return Ok("Pedido cancelado com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.ContasCancelarPedido, ex, ex.Message);
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }

        //[HttpPost]
        //[Route("ProcessarPedido/{pedidoId}")]
        //public IActionResult ProcessarPedido(int pedidoId)
        //{
        //    try
        //    {
        //        bool pedidoProcessando = _contaDac.ProcessarPedido(pedidoId);
        //        if (!pedidoProcessando)
        //            return NotFound("Este pedido não existe ou já foi processado.");

        //        return Ok("O pedido está sendo processado.");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(EventosLog.ContasProcessarPedido, ex, ex.Message);
        //        return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
        //    }
        //}

        [HttpPost]
        [Route("FinalizarPedido/{pedidoId}")]
        public IActionResult FinalizarPedido(int pedidoId)
        {
            try
            {
                bool pedidoFinalizado = _contaDac.FinalizarPedido(pedidoId);
                if (!pedidoFinalizado)
                    return NotFound("Este pedido não existe ou já foi finalizado.");

                return Ok("O pedido foi finalizado com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.ContasFinalizarPedido, ex, ex.Message);
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }

        [HttpPost]
        [Route("FecharConta/{reservaId}")]
        public IActionResult FecharConta(int reservaId)
        {
            try
            {
                Conta conta = _contaDac.FecharConta(reservaId);
                if (conta == null)
                    return NotFound("Não há uma conta em aberto.");

                return Ok(conta);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.ContasFecharConta, ex, ex.Message);
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }
    }
}