namespace Api.Models
{
    public class Mesa
    {
        public int Id { get; set; }

        public string Descricao { get; set; }

        public int QuantidadeAssento { get; set; }

        public bool Disponivel { get; set; }
    }
}
