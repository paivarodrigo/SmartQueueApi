using System;

namespace Api.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        public DateTime Horario { get; set; }

        public int QuantidadePessoas { get; set; }

        public string Senha { get; set; }

        public DateTime CheckIn { get; set; }

        public DateTime CheckOut { get; set; }

        public DateTime PrevisaoChegada { get; set; }

        public TimeSpan TempoEspera { get; set; }

        public string Status { get; set; }

        public Conta Conta { get; set; }
    }
}
