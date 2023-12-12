using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
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
            string connectionString = ReadConnectionStringFromFile();
            connection = new SqlConnection(connectionString);
        }

        public void OpenConnection()
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }
        public string ReadConnectionStringFromFile()
        {
            string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string lgAmsSystemFolder = Path.Combine(documentsFolder, "LGU-AMS-SYSTEM");
            string filePath = Path.Combine(lgAmsSystemFolder, "connectionString.txt");

            try
            {
                // Check if the "LGU-AMS-SYSTEM" folder exists, and create it if not
                if (!Directory.Exists(lgAmsSystemFolder))
                {
                    Directory.CreateDirectory(lgAmsSystemFolder);
                    Console.WriteLine($"Created folder: {lgAmsSystemFolder}");


                    MessageBox.Show("The system failed to find the \"LGU-AMS-SYSTEM\" at your document folders and the connectionString.txt. The system may not work properly.");
                }

                string connectionString = File.ReadAllText(filePath);
                return connectionString;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading the connection string: {ex.Message}");
                return null;
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
