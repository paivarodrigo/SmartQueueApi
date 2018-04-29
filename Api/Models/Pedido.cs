using System;
using System.Collections.Generic;

namespace Api.Models
{
    public class Pedido
    {
        public int Id { get; set; }

        public decimal Valor { get; set; }

        public DateTime Data { get; set; }

        public string Status { get; set; }

        public IEnumerable<ItemPedido> ItensPedido { get; set; }
    }
}
