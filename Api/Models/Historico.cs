using System;

namespace Api.Models
{
    public class Historico
    {
        public DateTime DataReserva { get; set; }

        public int QuantidadePessoas { get; set; }

        public string Mesa { get; set; }

        public decimal Valor { get; set; }

        public string Pedidos { get; set; }
    }
}
