namespace Api.Models
{
    public class ItemPedido
    {
        public int Id { get; set; }

        public int Quantidade { get; set; }

        public string Status { get; set; }

        public Produto Produto { get; set; }
    }
}
