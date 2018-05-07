using System;

namespace Api.Models
{
    public class Conta
    {
        public int Id { get; set; }

        public int ReservaId { get; set; }

        public DateTime DataAbertura { get; set; }

        public DateTime DataFechamento { get; set; }

        public decimal Valor { get; set; }
    }
}
