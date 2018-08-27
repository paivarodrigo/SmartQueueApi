using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Api.Models;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Api.Dac
{
    public class ProdutoDB : IProdutoDac
    {
        private readonly IConfiguration Configuration;

        public ProdutoDB(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public string BuscarPorContaId(int contaId)
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                return con.QueryFirstOrDefault<string>(@"
                    SELECT SUBSTRING(
	                (
		                SELECT ', ' + PR.Nome + ' R$' + REPLACE(CONVERT(VARCHAR, PR.Valor), '.', ',') + ' x ' + CONVERT(VARCHAR, SUM(IPE.Quantidade))
		                    FROM Contas C
		                   INNER JOIN Pedidos PE ON PE.ContaID = C.ID
		                   INNER JOIN ItensPedidos IPE ON IPE.PedidoID = PE.ID
		                   INNER JOIN Produtos PR ON PR.ID = IPE.ProdutoID
		                   INNER JOIN PedidosStatus PS ON PS.ID = PE.StatusID
		                   WHERE C.ID = @ContaID
		                     AND PS.Nome = 'Finalizado'
		                   GROUP BY PR.Nome, PR.Valor
		                     FOR XML PATH('')
	                ), 3, 100000) AS Produtos", new { ContaID = contaId });
        }

        public Produto BuscarPorId(int id)
        {
            DynamicParameters parametros = new DynamicParameters();
            parametros.Add("ID", id);

            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                return con.QueryFirstOrDefault<Produto>(@"
                    SELECT ID,
		                   Nome,
		                   Descricao,
		                   Valor,
		                   CategoriaID,
		                   Imagem
	                  FROM Produtos
	                 WHERE ID = @ID;", new { ID = id });
        }

        public IEnumerable<Categoria> ListarCategorias()
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                return con.Query<Categoria>(@"
                    SELECT ID,
		                   Caracteristica,
		                   Tamanho
	                  FROM Categorias;", null);
        }

        public IEnumerable<Produto> ListarProdutos()
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                return con.Query<Produto>(@"
                    SELECT ID,
		                   Nome,
		                   Descricao,
		                   Valor,
		                   CategoriaID
	                  FROM Produtos;", null);
        }

        public IEnumerable<Produto> ConsultarRanking()
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                return con.Query<Produto>(@"
                    SELECT TOP 5 RANK() OVER (ORDER BY Total DESC) AS Posicao,
		                   ProdutoID,
		                   Total
	                  INTO #Ranking
	                  FROM (SELECT ProdutoID,
				                   SUM(Quantidade) AS Total
			                  FROM ItensPedidos IPE
			                 INNER JOIN Pedidos PE ON PE.ID = IPE.PedidoID
			                 INNER JOIN PedidosStatus PS ON PS.ID = PE.StatusID
			                 WHERE PS.Nome IN('Em Fila', 'Processando', 'Finalizado')
			                 GROUP BY ProdutoID) AS A;

	                SELECT P.ID,
	                	   P.Nome,
	                	   P.Descricao,
	                	   P.Valor,
	                	   P.CategoriaID,
	                	   P.Imagem
	                  FROM dbo.Produtos P
	                 INNER JOIN #Ranking R ON P.ID = R.ProdutoID
	                 ORDER BY R.Posicao;", null);
        }

        public IEnumerable<Tuple<int, string>> ListarPedidosPendentes()
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                return con.Query<Tuple<int, string>>(@"
                    SELECT PED.ID AS Item1
	                     , SUBSTRING(
		                    (
		   	                    SELECT ', ' + PR.Nome + ' x ' + CONVERT(VARCHAR, SUM(IPE.Quantidade))
		   	                      FROM Contas C
		   	                     INNER JOIN Pedidos PE ON PE.ContaID = C.ID
		   	                     INNER JOIN ItensPedidos IPE ON IPE.PedidoID = PE.ID
		   	                     INNER JOIN Produtos PR ON PR.ID = IPE.ProdutoID
		   	                     WHERE PE.ID = PED.ID
		   	                     GROUP BY PR.Nome, PR.Valor
		   	                       FOR XML PATH('')
		                    ), 3, 100000) AS Item2
	                  FROM Pedidos PED
	                 INNER JOIN PedidosStatus PS ON PS.ID = PED.StatusID
	                 WHERE PS.Nome IN ('Em Fila', 'Processando', 'Finalizado');");
        }
    }
}
