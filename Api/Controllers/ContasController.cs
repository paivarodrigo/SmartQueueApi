using Api.Dac;
using Api.Models;
using Api.Utils;
using Microsoft.AspNetCore.Mvc;
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
                {
                    return Ok(new Historico() { Valor = 0, Pedidos = string.Empty });
                }

                return Ok(historicoDeHoje);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.ContasConsultarConta, ex, ex.Message);
                return StatusCode(500, "Ops! Um erro ocorreu ao consultar a conta.");
            }
        }

        [HttpPost]
        [Route("RealizarPedido/{contaId}")]
        public IActionResult RealizarPedido([FromBody] IEnumerable<ItemPedido> itensPedido, int contaId)
        {
            try
            {
                if (itensPedido == null || !itensPedido.Any() || contaId == 0)
                {
                    return BadRequest("Ops! Não foi possível realizar o pedido.");
                }

                Pedido pedido = _contaDac.AdicionarPedido(contaId, itensPedido);

                if (pedido == null)
                {
                    return BadRequest("Ops! Não foi possível realizar o pedido.");
                }

                return Ok(pedido);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.ContasRealizarPedido, ex, ex.Message);
                return StatusCode(500, "Ops! Um erro ocorreu ao realizar o pedido.");
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
                {
                    return NotFound("Este pedido não existe ou já foi cancelado.");
                }

                return Ok("Pedido cancelado com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.ContasCancelarPedido, ex, ex.Message);
                return StatusCode(500, "Ops! Um erro ocorreu ao cancelar o pedido.");
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
                {
                    return NotFound("Este pedido não existe ou já foi finalizado.");
                }

                return Ok("O pedido foi finalizado com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.ContasFinalizarPedido, ex, ex.Message);
                return StatusCode(500, "Ops! um erro ocorreu ao finalizar o pedido.");
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
                {
                    return Ok(new Conta() { Id = 0 });
                }

                return Ok(conta);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.ContasFecharConta, ex, ex.Message);
                return StatusCode(500, "Ops! Um erro ocorreu ao fechar a conta.");
            }
        }
    }
}