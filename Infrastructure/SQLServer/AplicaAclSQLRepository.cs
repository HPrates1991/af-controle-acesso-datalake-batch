using System;
using System.Data;
using System.Data.SqlClient;
using Domain.Repositories;
using Domain.StatusSolicitacaoEnum;
using Domain.StatusStorageEnum;

namespace Infrastructure.SQLServer
{
    public class AplicaAclSQLRepository : IAplicaAclSQLRepository
    {
        public DataTable getSolicitacoes(StatusSolicitacao StatusSol){
            string connStr= "Server=tcp:sqlsv-bigdata-hlg.database.windows.net,1433;Initial Catalog=sqldb-gestao-acessos-hlg;Persist Security Info=False;User ID=db-srv-admin;Password=*k7Ug4pJRQ6B8gX;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            string Status=StatusSol.ToString();            

            DataTable dt = new DataTable();
            using(SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("PrListSolicitacao",conn);
                
                cmd.CommandType = CommandType.StoredProcedure;           
                cmd.Parameters.AddWithValue("@StSolicitacao", SqlDbType.VarChar).Value = Status;
                using(SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }              
                conn.Close();
            }
            return dt;
        }

        public void SetStatusSolicitacao(Int64 IdSolicitacao, StatusSolicitacao StatusSol){
            string connStr= "Server=tcp:sqlsv-bigdata-hlg.database.windows.net,1433;Initial Catalog=sqldb-gestao-acessos-hlg;Persist Security Info=False;User ID=db-srv-admin;Password=*k7Ug4pJRQ6B8gX;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            string Status= StatusSol.ToString();

            using(SqlConnection conn = new SqlConnection(connStr)){

                conn.Open();

                SqlCommand cmd = new SqlCommand("PrAtualizarStatusSolicitacao",conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdSolicitacao", SqlDbType.BigInt).Value = IdSolicitacao;                
                cmd.Parameters.AddWithValue("@StSolicitacao", SqlDbType.VarChar).Value = Status;
                cmd.ExecuteNonQuery();

                conn.Close();

             }

        }
    
        public void SetStatusInfoStorage (Int64 IdSolicitacao, Int64 IdStorage, StatusStorage StatusInfoS, string dsObs){

            string connStr= "Server=tcp:sqlsv-bigdata-hlg.database.windows.net,1433;Initial Catalog=sqldb-gestao-acessos-hlg;Persist Security Info=False;User ID=db-srv-admin;Password=*k7Ug4pJRQ6B8gX;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            string Status=StatusInfoS.ToString();

            using(SqlConnection conn = new SqlConnection(connStr)){
                
                //Atualizando estatus do storage
                conn.Open();
                SqlCommand cmd = new SqlCommand("PrAtualizarStatusInfoStorage",conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdSolicitacao", SqlDbType.BigInt).Value = IdSolicitacao;
                cmd.Parameters.AddWithValue("@IdInfoStorage", SqlDbType.BigInt).Value = IdStorage;
                cmd.Parameters.AddWithValue("@stInfoStorage", SqlDbType.VarChar).Value = Status;
                cmd.Parameters.AddWithValue("@DsObs", SqlDbType.VarChar).Value = dsObs;
                cmd.ExecuteNonQuery();


                int total_storages = 0;
                int total_storages_concluidos = 0;
                int total_storages_concluidos_erro = 0;

                string query = "select count(*) from TbInfoStorage where IdSolicitacao="+IdSolicitacao.ToString();

                SqlCommand cmd2 = new SqlCommand(query, conn);
                total_storages = (int)cmd2.ExecuteScalar();

                query = "select count(*) from TbInfoStorage where IdSolicitacao="+IdSolicitacao.ToString()+" and lower(StInfoStorage) like '%PROCESSADO%'";
                cmd2.CommandText = query;
                cmd2.Connection = conn;
                total_storages_concluidos = (int)cmd2.ExecuteScalar();

                query = "select count(*) from TbInfoStorage where IdSolicitacao="+IdSolicitacao.ToString()+" and lower(StInfoStorage) like 'PROCESSADO_COM_ERRO'";
                cmd2.CommandText = query;
                cmd2.Connection = conn;
                total_storages_concluidos_erro = (int)cmd2.ExecuteScalar();


                if(total_storages == total_storages_concluidos ){

                    if(total_storages_concluidos_erro > 0){

                        cmd.CommandText = "PrAtualizarStatusSolicitacao";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = conn;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@IdSolicitacao", SqlDbType.BigInt).Value = IdSolicitacao;
                        cmd.Parameters.AddWithValue("@StSolicitacao", SqlDbType.VarChar).Value = "PROCESSADO_COM_ERRO";
                        cmd.ExecuteNonQuery();

                    }else{
                        cmd.CommandText = "PrAtualizarStatusSolicitacao";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = conn;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@IdSolicitacao", SqlDbType.BigInt).Value = IdSolicitacao;
                        cmd.Parameters.AddWithValue("@StSolicitacao", SqlDbType.VarChar).Value = "PROCESSADO";
                        cmd.ExecuteNonQuery();
                    }                                      

                }
                
                conn.Close();
            }
        }
    
        public DataTable getStorages(Int64 IdSolicitacao, StatusStorage StatusInfoS){

            string connStr= "Server=tcp:sqlsv-bigdata-hlg.database.windows.net,1433;Initial Catalog=sqldb-gestao-acessos-hlg;Persist Security Info=False;User ID=db-srv-admin;Password=*k7Ug4pJRQ6B8gX;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            string Status=StatusInfoS.ToString();

            DataTable dt = new DataTable();
            using(SqlConnection conn = new SqlConnection(connStr))
            {
                

                conn.Open();

                SqlCommand cmd = new SqlCommand("PrListInfoStorage",conn);
                
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdSolicitacao", SqlDbType.BigInt).Value = IdSolicitacao;
                cmd.Parameters.AddWithValue("@StInfoStorage  ", SqlDbType.VarChar).Value = Status;
                using(SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
                
                conn.Close();

            }
            return dt;

        }
    
    }
}