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
        [Route("ConsultarHistorico/{usuarioId}")]
        public IActionResult ConsultarHistorico(int usuarioId)
        {
            try
            {
                IEnumerable<Historico> historicos = _reservaDac.ConsultarHistorico(usuarioId);
                if (historicos == null)
                    return NotFound("Não há dados para gerar histórico.");

                return Ok(historicos);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.ReservasConsultarHistorico, ex, ex.Message);
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }

        [HttpPost]
        [Route("SolicitarMesa")]
        public IActionResult SolicitarMesa([FromBody] Reserva reserva)
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
                _logger.LogError(EventosLog.ReservasSolicitarMesa, ex, ex.Message);
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }

        [HttpPost]
        [Route("AtivarReserva/{numeroDaMesa}")]
        public IActionResult AtivarReserva([FromBody] Reserva reserva, int numeroDaMesa)
        {
            try
            {
                if (reserva == null)
                    return BadRequest("Não foi possível ativar a reserva.");

                Conta conta = _reservaDac.AtivarReserva(reserva, numeroDaMesa);
                if (conta == null)
                    return BadRequest("Não foi possível ativar a reserva.");

                return Ok(conta);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.ReservasAtivarReserva, ex, ex.Message);
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }

        [HttpGet]
        [Route("ConsultarTempo/{quantidadePessoas}")]
        public IActionResult ConsultarTempo(int quantidadePessoas)
        {
            try
            {
                //TimeSpan tempoDeEspera = CalcularTempoDeEspera(quantidadePessoas);
                TimeSpan tempoDeEspera = new TimeSpan(0, 28, 0); //0 horas, 28 minutos e 0 segundos
                return Ok(new { TempoDeEspera = tempoDeEspera });
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.ReservasConsultarTempo, ex, ex.Message);
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }

        [HttpPost]
        [Route("CancelarMesa")]
        public IActionResult CancelarMesa([FromBody] Reserva reserva)
        {
            try
            {
                if (reserva == null || String.IsNullOrWhiteSpace(reserva.SenhaCheckIn))
                    return BadRequest("Não foi possível cancelar a reserva.");

                int reservaID = _reservaDac.BuscarReservaIDPorSenha(reserva.UsuarioId, reserva.SenhaCheckIn);

                if (reservaID == 0)
                    return NotFound("A reserva não foi encontrada.");

                if (!_reservaDac.CancelarReserva(reservaID))
                    return BadRequest("Esta reserva já foi utilizada ou cancelada.");

                return Ok("A reserva foi cancelada com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.ReservasCancelarMesa, ex, ex.Message);
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }
    }
}