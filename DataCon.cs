using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace KanBan
{
    public class DataCon
    {
        public struct SQLJob
        {
            public string SQLText;
            public SqlParameter[] ListOfSQLParameters;
        }

        private static string GetConnectionString()
        {
            return @"Data Source=DESKTOP-8EUNORG\MSSQLSERVER01;Initial Catalog=EthoAudit;Integrated Security=True;TrustServerCertificate=True";
        }

        private static SqlConnection GetConnection()
        {
            return new SqlConnection(GetConnectionString());
        }

        public static DataSet BuildDataSet(string SQLQuery)
        {
            using (var connection = GetConnection())
            {
                using (var adapter = new SqlDataAdapter(SQLQuery, connection))
                {
                    adapter.SelectCommand.CommandTimeout = 600;

                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    return dataset;
                }
            }
        }

        public static DataSet BuildDataSet(string SQLQuery, SqlParameter[] parameters)
        {
            using (var connection = GetConnection())
            {
                using (var adapter = new SqlDataAdapter(SQLQuery, connection))
                {
                    adapter.SelectCommand.CommandTimeout = 600;
                    adapter.SelectCommand.Parameters.AddRange(parameters);

                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    return dataset;
                }
            }
        }

        public static void ExecNonQuery(string SQLQuery)
        {
            using (var connection = GetConnection())
            {
                using (var command = new SqlCommand(SQLQuery, connection))
                {
                    command.CommandTimeout = 600;
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void ExecNonQuery(string SQLQuery, SqlParameter[] parameters)
        {
            using (var connection = GetConnection())
            {
                using (var command = new SqlCommand(SQLQuery, connection))
                {
                    command.CommandTimeout = 600;
                    command.Parameters.AddRange(parameters);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public static bool ExecNonQueryTrans(List<SQLJob> SQLJobList)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandTimeout = 600;

                        try
                        {
                            foreach (var job in SQLJobList)
                            {
                                command.CommandText = job.SQLText;
                                command.Parameters.Clear();
                                command.Parameters.AddRange(job.ListOfSQLParameters);
                                command.ExecuteNonQuery();
                            }
                            transaction.Commit();
                            return true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            return false;
                        }
                    }
                }
            }
        }

        public static string ExecNonQueryTransWithErrMessage(List<SQLJob> SQLJobList)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandTimeout = 600;

                        try
                        {
                            foreach (var job in SQLJobList)
                            {
                                command.CommandText = job.SQLText;
                                command.Parameters.Clear();
                                command.Parameters.AddRange(job.ListOfSQLParameters);
                                command.ExecuteNonQuery();
                            }
                            transaction.Commit();
                            return string.Empty;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return ex.Message;
                        }
                    }
                }
            }
        }
    }
}


/*
                DataSet Auditors = SQLQuery.BuildDataSet(sSQL);
                FName = Auditors.Tables[0].Rows[0]["FirstName"].ToString();
                Surname = Auditors.Tables[0].Rows[0]["Surname"].ToString();
                lblWelcome.Text = "Welcome " + FName + " " + Surname; 

 *               foreach (DataRow ARow in Auditors.Tables[0].Rows)
 *               {
 *                   ARow["FirstName"].ToString();
 *               }
 * 
 * 
 * 
 * 
 * 
 *           string sql = "select 1 as dummy from dbo.SupplierPresortLines with (nolock) " +
 *                                          " where DivisionId = @DivisionId " +
 *                                          " and SupplierPresortLineGroupID = @SupplierPresortLineGroupID  ";

            ArrayList ListOfSQLParameters = new ArrayList();
            ListOfSQLParameters.Add(new System.Data.SqlClient.SqlParameter("DivisionId", DivisionId));
            ListOfSQLParameters.Add(new System.Data.SqlClient.SqlParameter("SupplierPresortLineGroupID", SupplierPresortLineGroupID));

            System.Data.DataSet ds = SupplierPackIT.Data.SQLQuery.BuildDataSet(sql, ListOfSQLParameters);


*/