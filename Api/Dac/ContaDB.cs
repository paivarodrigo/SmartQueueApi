using Api.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Api.Dac
{
    public class ContaDB : IContaDac
    {
        private readonly IConfiguration Configuration;

        public ContaDB(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public Pedido AdicionarPedido(int contaId, IEnumerable<ItemPedido> itensPedido)
        {
            DataTable dataTableItens = new DataTable();
            dataTableItens.Columns.Add("ProdutoId", typeof(int));
            dataTableItens.Columns.Add("Quantidade", typeof(int));

            foreach (ItemPedido itemPedido in itensPedido)
            {
                dataTableItens.Rows.Add(itemPedido.ProdutoId, itemPedido.Quantidade);
            }

            DynamicParameters parametros = new DynamicParameters();
            parametros.Add("ContaID", contaId);
            parametros.Add("ItensPedido", dataTableItens.AsTableValuedParameter("dbo.ItemPedido"));

            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                return con.QueryFirstOrDefault<Pedido>("Contas.AdicionarPedido", parametros, commandType: CommandType.StoredProcedure);
            }
        }

        public Historico ConsultarPorReservaId(int reservaId)
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                return con.QueryFirstOrDefault<Historico>("Contas.ConsultarPorReservaID",
                    new
                    {
                        ReservaID = reservaId
                    }, commandType: CommandType.StoredProcedure);
            }
        }

        public bool CancelarPedido(int pedidoId)
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                return con.QueryFirst<int>(@"
                    IF EXISTS (SELECT 1 FROM Pedidos WHERE ID = @PedidoID
                                AND StatusID IN (SELECT ID FROM PedidosStatus WHERE Nome IN ('Pendente')))
                    BEGIN
                        DECLARE @StatusID INT = (SELECT TOP 1 ID
                                                   FROM PedidosStatus
                                                  WHERE Nome = 'Cancelado');
                        
                        UPDATE Pedidos SET StatusID = @StatusID WHERE ID = @PedidoID;

                        SELECT 1;
                    END
                    ELSE
                    BEGIN
                        SELECT 0;
                    END",
                    new
                    {
                        PedidoID = pedidoId
                    }) == 1;
            }
        }

        public Conta FecharConta(int reservaId)
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                return con.QueryFirstOrDefault<Conta>("Contas.FecharConta",
                    new
                    {
                        ReservaID = reservaId
                    }, commandType: CommandType.StoredProcedure);
            }
        }

        //public bool ProcessarPedido(int pedidoId)
        //{
        //    using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
        //        return con.QueryFirst(@"
        //            IF EXISTS (SELECT 1 FROM Pedidos WHERE ID = @PedidoID
        //                        AND StatusID IN (SELECT ID FROM PedidosStatus WHERE Nome = 'Em Fila'))
        //            BEGIN
        //                DECLARE @StatusID INT = (SELECT TOP 1 ID
        //                                           FROM PedidosStatus
        //                                          WHERE Nome = 'Processando');

        //                UPDATE Pedidos SET StatusID = @StatusID WHERE ID = @PedidoID;

        //                SELECT 1;
        //            END
        //            ELSE
        //            BEGIN
        //                SELECT 0;
        //            END", new { PedidoID = pedidoId });
        //}

        public bool FinalizarPedido(int pedidoId)
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                return con.QueryFirst(@"
                    IF EXISTS (SELECT 1 FROM Pedidos WHERE ID = @PedidoID
                                AND StatusID IN (SELECT ID FROM PedidosStatus WHERE Nome = 'Pendente'))
                    BEGIN
                        DECLARE @StatusID INT = (SELECT TOP 1 ID
                                                   FROM PedidosStatus
                                                  WHERE Nome = 'Finalizado');
                        
                        UPDATE Pedidos SET StatusID = @StatusID WHERE ID = @PedidoID;

                        SELECT 1;
                    END
                    ELSE
                    BEGIN
                        SELECT 0;
                    END",
                    new
                    {
                        PedidoID = pedidoId
                    });
            }
        }
    }
}
