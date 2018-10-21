using Api.Dac;
using Api.Models;
using Api.Utils;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ReservasController : Controller
    {
        private readonly IClient _client;
        private readonly IReservaDac _reservaDac;
        private readonly IProdutoDac _produtoDac;
        private readonly ILoggerDac _logger;

        public ReservasController(IClient client, IReservaDac reservaDac, IProdutoDac produtoDac, ILoggerDac logger)
        {
            _client = client;
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
                {
                    return NotFound("Não há dados para gerar histórico.");
                }

                return Ok(historicos);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.ReservasConsultarHistorico, ex, ex.Message);
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }

        [HttpPost]
        [Route("SolicitarReserva")]
        public async Task<IActionResult> SolicitarReserva([FromBody] Reserva reserva)
        {
            try
            {
                if (reserva == null)
                {
                    return BadRequest("Não foi possível solicitar a reserva.");
                }

                if (_reservaDac.VerificarReservaUsuario(reserva.UsuarioId))
                {
                    return BadRequest("Já existe uma reserva na fila ou ativa.");
                }
                else
                {
                    if (_reservaDac.VerificarLotacaoMesas())
                    {
                        reserva = _reservaDac.AdicionarReserva(reserva);
                        List<Reserva> reservas = _reservaDac.BuscarReservasNaFila();
                        List<int> tempos = await _client.BuscarTemposPrevistos(reservas.Count);

                        int i = 0;
                        foreach (var res in reservas)
                        {
                            res.MinutosDeEspera = tempos[i++];
                        }
                    }
                    else
                    {
                        reserva.MinutosDeEspera = 1; // 1 minuto é o padrão definido para espera quando não há fila
                        reserva = _reservaDac.AdicionarReserva(reserva);
                    }
                }

                return Ok(reserva);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.ReservasSolicitarMesa, ex, ex.Message);
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }

        [HttpPost]
        [Route("AtivarReserva/{senhaDaMesa}")]
        public IActionResult AtivarReserva([FromBody] Reserva reserva, string senhaDaMesa)
        {
            try
            {
                if (reserva == null)
                {
                    return BadRequest("Não foi possível ativar a reserva.");
                }

                Conta conta = _reservaDac.AtivarReserva(reserva, senhaDaMesa);
                if (conta == null)
                {
                    return BadRequest("Não foi possível ativar a reserva.");
                }

                return Ok(conta);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.ReservasAtivarReserva, ex, ex.Message);
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }

        [HttpPost]
        [Route("CancelarReserva")]
        public IActionResult CancelarReserva([FromBody] Reserva reserva)
        {
            try
            {
                if (reserva == null || string.IsNullOrWhiteSpace(reserva.Status))
                {
                    return BadRequest("Não foi possível cancelar a reserva.");
                }

                int reservaID = _reservaDac.BuscarReservaIDPorStatus(reserva.UsuarioId, reserva.Status);

                if (reservaID == 0)
                {
                    return NotFound("A reserva não foi encontrada.");
                }

                if (!_reservaDac.CancelarReserva(reservaID))
                {
                    return BadRequest("Esta reserva já foi utilizada ou cancelada.");
                }

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