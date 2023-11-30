﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace UserCreatorAuth
{
    class DatabaseConnection : IDisposable
    {
        private SqlConnection connection;
        private string connectionString = "Data Source=TOZEN\\SQLEXPRESS;Initial Catalog=LGU_AMS_DB;Integrated Security=True;";

        public DatabaseConnection()
        {
            connection = new SqlConnection(connectionString);
        }

        public void OpenConnection()
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }

        public void CloseConnection()
        {
            if (connection.State != ConnectionState.Closed)
            {
                connection.Close();
            }
        }

        public void Dispose()
        {
            CloseConnection();
            connection.Dispose();
        }

        public void UploadToDatabase(string query, Dictionary<string, object> parameters)
        {
            OpenConnection();

            try
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                    }

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        public DataTable ReadFromDatabase(string query, Dictionary<string, object> parameters)
        {
            OpenConnection();

            DataTable dataTable = new DataTable();

            try
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                    }

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return dataTable;
        }
    }
}
