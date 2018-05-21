using System;

namespace Api.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        public int MesaId { get; set; }

        public DateTime DataReserva { get; set; }

        public int QuantidadePessoas { get; set; }

        public string SenhaCheckIn { get; set; }

        public DateTime DataCheckIn { get; set; }

        public DateTime DataCheckOut { get; set; }

        public TimeSpan TempoDeEspera { get; set; }

        public string Status { get; set; }
    }
}
