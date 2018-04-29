using System;
using System.Collections.Generic;

namespace Api.Models
{
    public class Conta
    {
        public int Id { get; set; }

        public decimal valor { get; set; }

        public DateTime DataAbertura { get; set; }

        public DateTime DataFechamento { get; set; }

        public IEnumerable<Pedido> Pedidos { get; set; }
    }
}
