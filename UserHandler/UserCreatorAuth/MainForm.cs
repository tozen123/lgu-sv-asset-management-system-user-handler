using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UserCreatorAuth
{
    public partial class MainForm : Form
    {
        private DatabaseConnection databaseConnection;
        private string selectUser;
        public MainForm()
        {
            InitializeComponent();

            databaseConnection = new DatabaseConnection(); // Initialize DatabaseConnection Class Instance
            buttonViewProfile.Enabled = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RetrieveAllUser();

            buttonCreateUser.Enabled = false;
            buttonRetrieve.Enabled = false;
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            RetrieveAllUser();
        }

        private void RetrieveAllUser()
        {
            string query = @"
                            SELECT
                            u.*,
                            CASE
                                WHEN ac.userId IS NOT NULL OR aa.userId IS NOT NULL THEN 'TRUE'
                                ELSE 'FALSE'
                            END AS isRegistered
                            FROM
                                Users u
                            LEFT JOIN
                                AssetCoordinator ac ON u.userID = ac.userId
                            LEFT JOIN
                                AssetAdministrator aa ON u.userID = aa.userId;
                            
                        ";
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            DataTable dataTable = databaseConnection.ReadFromDatabase(query, parameters);

            dataGridView1.DataSource = dataTable;

            //Stretch The Tables
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }
        private void RetrieveSpecificUsers(string prefix)
        {
            string query = "SELECT * FROM Users WHERE userID LIKE @param";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@param", prefix + "%" }
            };

            DataTable dataTable = databaseConnection.ReadFromDatabase(query, parameters);

            dataGridView1.DataSource = dataTable;

            // Stretch The Tables
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void buttonCreateUser_Click(object sender, EventArgs e)
        {
            string chosen_type = comboBoxUserTypeCreation.SelectedItem.ToString();

            if (string.IsNullOrEmpty(chosen_type))
            {
                MessageBox.Show("An error occured: please choose from the user type", "Error");
                return;
            }

            string query = "SELECT userID From Users ";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            DataTable resultTable = databaseConnection.ReadFromDatabase(query, parameters);

            int count = 0;

            foreach (DataRow row in resultTable.Rows)
            {
                count++;
            }

            if (chosen_type.Equals("Viewer"))
            {
                string starting_number = "01";
                string newId = starting_number + "-" + (count + 1).ToString();

                CreateUser(newId, BasicPasswordGenerator());
            }
            else if(chosen_type.Equals("Coordinator"))
            {
                string starting_number = "02";
                string newId = starting_number + "-" + (count + 1).ToString();

                CreateUser(newId, BasicPasswordGenerator());

            }
            else if (chosen_type.Equals("Administrator"))
            {
                string starting_number = "03";
                string newId = starting_number + "-" + (count + 1).ToString();

                CreateUser(newId, BasicPasswordGenerator());

            }
            buttonCreateUser.Enabled = false;
        }
        private string BasicPasswordGenerator()
        {
            Random r = new Random();
            return r.Next(0000, 9999).ToString();
        }

        private void comboBoxUserTypeCreation_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonCreateUser.Enabled = true;
        }

        private void CreateUser(string generated_id, string generated_password)
        {
            string query = "INSERT INTO Users (userId, userPassword) VALUES (@userId, @userPassword)";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@userId", generated_id },
                { "@userPassword", generated_password }
            };

            databaseConnection.UploadToDatabase(query, parameters);
            databaseConnection.CloseConnection();

            MessageBox.Show("New user account has been succesfully created. \n\n" + 
                "UserId: " + generated_id + "\nPassword: " + BasicPasswordGenerator() +
                "\n\nPlease save the information above.", 
                "Success");

            buttonCreateUser.Enabled = false;
            comboBoxUserTypeCreation.SelectedItem = null;

            RetrieveAllUser();
        }

        private void comboBoxUserTypeRetrieve_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonRetrieve.Enabled = true;
        }

        private void buttonRetrieve_Click(object sender, EventArgs e)
        {
            buttonRetrieve.Enabled = false;
            string type = comboBoxUserTypeRetrieve.SelectedItem.ToString();

            if (type.Equals("All"))
            {
                RetrieveAllUser();
            }
            else if (type.Equals("Viewer"))
            {
                RetrieveSpecificUsers("01");
            }
            else if (type.Equals("Coordinator"))
            {
                RetrieveSpecificUsers("02");
            }
            else if (type.Equals("Administrator"))
            {
                RetrieveSpecificUsers("03");
            }

        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                textBoxUserID.Text = row.Cells[0].Value.ToString();
                textBoxPassword.Text = row.Cells[1].Value.ToString();

                selectUser = textBoxUserID.Text;
                Console.WriteLine("debug: " + selectUser);  
                buttonViewProfile.Enabled = true;

                string type = textBoxUserID.Text.Split('-')[0].ToString();
                Console.WriteLine("TYPE: " + type);
                //read
                string query = "null";
                switch (type)
                {
                    case "01":
                        query = "SELECT assetViewerFName, assetViewerMName, assetViewerLName, " +
                                "assetViewerPhoneNum, assetViewerEmail, " +
                                "assetViewerAddress, assetViewerOffice " +
                                "FROM AssetViewer WHERE userId = @UserId";
                        break;
                    case "02":
                        query = "SELECT FName, MName, LName, " +
                                "PhoneNumber, Email, " +
                                "Address, Office " +
                                "FROM AssetCoordinator WHERE userId = @UserId";
                        break;
                    case "03":
                        query = "SELECT FName, MName, LName, " +
                                "PhoneNumber, Email, " +
                                "Address, Office " +
                                "FROM AssetAdministrator WHERE userId = @UserId";
                        break;
                }

                Console.WriteLine("textBoxUserID.Text: " + textBoxUserID.Text);
                Console.WriteLine("query: " + query);
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                        { "@UserId", textBoxUserID.Text }

                };

                DataTable resultTable = databaseConnection.ReadFromDatabase(query, parameters);

                string resultString = "";

                if (resultTable.Rows.Count > 0)
                {
                    foreach (DataRow _row in resultTable.Rows)
                    {
                        foreach (DataColumn col in resultTable.Columns)
                        {
                            resultString += _row[col] + "/";
                        }
                    }

                    // Add the fetch data to the textboxes
                    Console.WriteLine("asdasd " + resultString);

                    textBoxName.Text = resultString.Split('/')[0] + " " + resultString.Split('/')[1] + " " + resultString.Split('/')[2];
                    textBoxOffice.Text = resultString.Split('/')[6];
                    databaseConnection.CloseConnection();
                }
                else
                {
                    textBoxName.Text = "";
                    textBoxOffice.Text = "";
                }




            }
        }

        private void buttonViewProfile_Click(object sender, EventArgs e)
        {
            byte[] imageByte = null;

            string user_query = "SELECT userImage FROM Users WHERE userID = @n_user_id";
            Dictionary<string, object> user_parameters = new Dictionary<string, object>();
            user_parameters.Add("@n_user_id", selectUser);

            DataTable user_resultTable = databaseConnection.ReadFromDatabase(user_query, user_parameters);
            Console.WriteLine("DataTable content: ");

            if (user_resultTable.Rows.Count > 0 && user_resultTable.Rows[0][0] != DBNull.Value)
            {
                imageByte = user_resultTable.Rows[0].Field<byte[]>(0);
                ViewProfileImageForm viewProfileImageForm = new ViewProfileImageForm(Utilities.ConvertByteArrayToImage(imageByte));
                viewProfileImageForm.Show();

            } 
            else
            {
                MessageBox.Show("This user does not have profile image uploaded in the database", "No image");
            }

            

            databaseConnection.CloseConnection();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            string searchKeyword = textBoxSearch.Text.Trim();

            DataTable dataTable = (DataTable)dataGridView1.DataSource;


            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(searchKeyword))
                {
                    string filterExpression = $"userID LIKE '%{searchKeyword}%'";


                    dataTable.DefaultView.RowFilter = filterExpression;
                }
                else
                {

                    dataTable.DefaultView.RowFilter = string.Empty;
                }
            }
        }
    }
}
