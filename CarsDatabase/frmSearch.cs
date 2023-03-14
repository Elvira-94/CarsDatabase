using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarsDatabase
{
    public partial class frmSearch : Form
    {
        public frmSearch()
        {
            InitializeComponent();
           

        }

        //Close Button
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        //Run Button
        private void btnRun_Click(object sender, EventArgs e)
        {
            
            string field = null;
            if (cboField.SelectedItem != null)
            {
                field = cboField.SelectedItem.ToString();
            }
            else
            {
                MessageBox.Show("Please enter a category from the field criteria");
                return;
            }

            string op = null;
            if (cboOperator.SelectedItem != null)
            {
                op = cboOperator.SelectedItem.ToString();
            }
            else
            {
                MessageBox.Show("Please enter a category from the operator criteria");
                return;
            }

            string value = null;
            if (txtValue.Text != "")
            {
                value = txtValue.Text;
                if(field == "Available")
                {
                    if (value.ToLower() == "yes")
                    {
                        value = "1";
                    } else if (value.ToLower() == "no")
                    {
                        value = "0";
                    }
                }
            }
            else
            {
                MessageBox.Show("Please enter the car make, engine size, rental per day or availability.");
                return;
            }



            //Build the SQL query based on the search criteria
            //Add the appropriate SQL syntax based on the operator selected
            //Execute the query and fill the datagridview with the results

            SQLiteConnection conn = new SQLiteConnection(@"Data Source=hire.db");
            conn.Open();
            string query;
            if(op == "=")
            {
                query = $"SELECT VehicleRegNo, Make, EngineSize, DateRegistered, RentalPerDay, Available FROM tblCar WHERE \"{field}\" LIKE \"%{value}%\"";

            } else
            {
                query = $"SELECT VehicleRegNo, Make, EngineSize, DateRegistered, RentalPerDay, Available FROM tblCar WHERE \"{field}\" {op} \"{value}\"";
            }
            SQLiteCommand cmd = new SQLiteCommand(query, conn);
            DataTable dt = new DataTable();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
            adapter.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        private void frmSearch_Load(object sender, EventArgs e)
        {
            //Populate cboField with the field names Make, EngineSize, RentalPerDay and Available
            cboField.Items.Add("Make");
            cboField.Items.Add("EngineSize");
            cboField.Items.Add("RentalPerDay");
            cboField.Items.Add("Available");

            //Populate cboOperator with =, <, >, <=, >= 
            cboOperator.Items.Add("=");
            cboOperator.Items.Add("<");
            cboOperator.Items.Add(">");
            cboOperator.Items.Add("<=");
            cboOperator.Items.Add(">=");
        }
    }
}
