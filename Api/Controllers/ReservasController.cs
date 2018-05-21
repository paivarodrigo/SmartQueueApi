using System;
using System.Collections.Generic;
using Api.Dac;
using Api.Models;
using Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ReservasController : Controller
    {
        private readonly IReservaDac _reservaDac;
        private readonly IProdutoDac _produtoDac;
        private readonly ILogger _logger;

        public ReservasController(IReservaDac reservaDac, IProdutoDac produtoDac, ILogger<ReservasController> logger)
        {
            _reservaDac = reservaDac;
            _produtoDac = produtoDac;
            _logger = logger;
        }

        [HttpGet]
        [Route("Historico/{usuarioId}")]
        public IActionResult Historico(int usuarioId)
        {
            try
            {
                IEnumerable<Historico> historicos = _reservaDac.BuscarHistorico(usuarioId);
                if (historicos == null)
                    return NotFound("Não há dados para gerar histórico.");

                return Ok(historicos);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.ReservasHistorico, ex, ex.Message);
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }

        [HttpPost]
        [Route("Solicitar")]
        public IActionResult Solicitar([FromBody] Reserva reserva)
        {
            try
            {
                if (reserva == null)
                    return BadRequest("Não foi possível solicitar a reserva.");
                reserva.SenhaCheckIn = Gerador.GerarSenhaDaReserva();
                reserva = _reservaDac.SolicitarReserva(reserva);
                if (reserva == null)
                    return BadRequest("Já existe uma reserva na fila ou ativa.");

                return Ok(reserva);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.ReservasSolicitar, ex, ex.Message);
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }

        [HttpPost]
        [Route("Ativar/{numeroDaMesa}")]
        public IActionResult Ativar([FromBody] Reserva reserva, int numeroDaMesa)
        {
            try
            {
                if (reserva == null)
                    return BadRequest("Não foi possível ativar a reserva.");

                reserva = _reservaDac.AtivarReserva(reserva, numeroDaMesa);
                if (reserva == null)
                    return BadRequest("Não foi possível ativar a reserva.");

                return Ok(reserva);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.ReservasAtivar, ex, ex.Message);
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }

        [HttpGet]
        [Route("ConsultarTempoDeEspera/{quantidadePessoas}")]
        public IActionResult ConsultarTempoDeEspera(int quantidadePessoas)
        {
            try
            {
                //TimeSpan tempoDeEspera = CalcularTempoDeEspera(quantidadePessoas);
                TimeSpan tempoDeEspera = new TimeSpan(0, 28, 0); //0 horas, 28 minutos e 0 segundos
                return Ok(new { TempoDeEspera = tempoDeEspera });
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.ReservasConsultarTempoDeEspera, ex, ex.Message);
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }
    }
}