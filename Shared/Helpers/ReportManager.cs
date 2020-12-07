using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Shared.Helpers
{
    public class ReportManager
    {
        public async static Task<DataSet> ExecuteProcedureReport(string sql, List<NpgsqlParameter> parameters,string  conns)
        {
            DataSet o_dataSet = null;
            try
            {
                using (var conn = new NpgsqlConnection(conns))
                {
                    conn.Open();
                    NpgsqlCommand comm = new NpgsqlCommand();
                    comm.CommandTimeout = 300;
                    foreach (var parameter in parameters)
                    {
                        comm.Parameters.Add(parameter);
                    }
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.CommandText = sql;
                    comm.Connection = conn;

                    //var reader = comm.ExecuteReader();
                    NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(comm);
                    o_dataSet = new DataSet();
                    dataAdapter.Fill(o_dataSet);
                    //var dataTable = new DataTable();
                    //dataTable.Load(reader);

                    //while (reader.Read())
                    //{
                    //    Console.Write("{0}\n", reader[0]);
                    //}
             

                    //o_dataSet.Tables.Add(dataTable);

                    conn.Close();
                }
            }
            catch (Exception e)
            {
                //Logger.Exception(e);
            }
            return o_dataSet;
        }
    }
}
