using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CarsDatabase
{

    public partial class frmCars : Form
    {
        //Global Variables
        bool newMode = false;
        Int32 currentCar; 
        Int32 carCount;
        SQLiteConnection conn = sQLiteConnection(); //Database connection object
        List<Car> cars;

        public frmCars()
        {
            InitializeComponent();
            cars = sQLiteRead(conn);
            carCount = cars.Count;
            fillForm(cars[0]);
        }

        //fillForm Helper Function -
        //this function will take a specific car from the list
        //and update the textboxes with data for that car
        public void fillForm(Car currCar)
        {
            //Transform Date
            string[] formatDate = currCar.dateRegistered.Split('-');
            Array.Reverse(formatDate);
            string date = String.Join("/", formatDate);

            //Transform Cost
            string cost = $"€{currCar.rentalPerDay.ToString()}";

            //Find Index
            int index = cars.FindIndex(car => car.vehicleRegNo == currCar.vehicleRegNo);

            txtVehicleRegNo.Text = currCar.vehicleRegNo;
            txtMake.Text = currCar.make;
            txtEngineSize.Text = currCar.engineSize;
            txtDateRegistered.Text = date;
            txtRentalPerDay.Text= cost;
            chkAvailable.Checked = currCar.available.ToString() =="1";
            currentCar = index + 1;
            txtRecordCount.Text = $"{currentCar} of {carCount}";
    }
        
   
        //First Button
        private void btnFirst_Click(object sender, EventArgs e)
        {
            fillForm(cars[0]);
        }

        //Previous Button
        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (currentCar > 1)
            {
                fillForm(cars[currentCar - 2]);
            }
        }

        //Next Button
        private void btnNext_Click(object sender, EventArgs e)
        {
            if(currentCar < carCount)
            {
                fillForm(cars[currentCar]);
            }
        }

        //Last Button
        private void btnLast_Click(object sender, EventArgs e)
        {
            fillForm(cars[carCount - 1]);
        }

        //Update Button
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            //Transform Date
            string[] formatDate = txtDateRegistered.Text.Split('/');
            Array.Reverse(formatDate);
            string finalDate = String.Join("-", formatDate);

            //Validate date entered into textbox
            Regex regex = new Regex(@"(((0|1)[0-9]|2[0-9]|3[0-1])\/(0[1-9] | 1[0-2])\/((19|20)\d\d))$");
            bool isValid = regex.IsMatch(txtDateRegistered.Text.Trim());
            DateTime dt;
            isValid = DateTime.TryParseExact(txtDateRegistered.Text, "dd/MM/yyyy", new CultureInfo("en-GB"), DateTimeStyles.None, out dt);
            if (!isValid)
            {
                MessageBox.Show("Invalid date.");
                return;
            }
            else
            {
                MessageBox.Show("Valid date.");
            }

            //Transform Cost
            string finalCost = txtRentalPerDay.Text;
            if (txtRentalPerDay.Text.Substring(0, 1) == "€")
            {
                finalCost = txtRentalPerDay.Text.Substring(1);
            }

            try
            {
                string uid = cars[currentCar - 1].uid.ToString();
                string vehicleRegNo = txtVehicleRegNo.Text;
                string make = txtMake.Text;
                string engineSize = txtEngineSize.Text;
                string date = finalDate;
                string cost = finalCost;
                string available = chkAvailable.Checked ? "1" : "0";
                SQLiteCommand cmd;
                cmd = conn.CreateCommand();
                cmd.CommandText = $"UPDATE tblCar SET VehicleRegNo= '{vehicleRegNo}', Make = '{make}', EngineSize = '{engineSize}', DateRegistered = '{date}', RentalPerDay = {cost}, Available = {available} WHERE uid = {uid}";
                cmd.ExecuteNonQuery();
                cars = sQLiteRead(conn);
                MessageBox.Show("Record updated successfully");
            }
            catch (Exception)
            {

                MessageBox.Show("Failed to update record");
            }

        }

        //Add Button
        private void btnAdd_Click(object sender, EventArgs e)
        {

            if (newMode)
            {
                //Transform Date
                string[] formatDate = txtDateRegistered.Text.Split('/');
                Array.Reverse(formatDate);
                string finalDate = String.Join("-", formatDate);

                //Validate date entered into textbox
                Regex regex = new Regex(@"(((0|1)[0-9]|2[0-9]|3[0-1])\/(0[1-9] | 1[0-2])\/((19|20)\d\d))$");
                bool isValid = regex.IsMatch(txtDateRegistered.Text.Trim());
                DateTime dt;
                isValid = DateTime.TryParseExact(txtDateRegistered.Text, "dd/MM/yyyy", new CultureInfo("en-GB"), DateTimeStyles.None, out dt);
                if (!isValid)
                {
                    MessageBox.Show("Invalid date.");
                    return;
                }
                else
                {
                    MessageBox.Show("Valid date.");
                }

                //Transform Cost
                string finalCost = txtRentalPerDay.Text;
                if (txtRentalPerDay.Text.Substring(0, 1) == "€")
                {
                    finalCost = txtRentalPerDay.Text.Substring(1);
                }

                string uid = cars[currentCar - 1].uid.ToString();
                string vehicleRegNo = txtVehicleRegNo.Text;
                string make = txtMake.Text;
                string engineSize = txtEngineSize.Text;
                string date = finalDate;
                string cost = finalCost;
                string available = chkAvailable.Checked ? "1" : "0";
                SQLiteCommand cmd; //Database command object
                cmd = conn.CreateCommand();
                cmd.CommandText = $"INSERT INTO tblCar (VehicleRegNo,Make,EngineSize,DateRegistered,RentalPerDay,Available) VALUES ('{vehicleRegNo}','{make}','{engineSize}','{date}', {cost},{available})";
                cmd.ExecuteNonQuery(); //Let's execute the SQL
                cars = sQLiteRead(conn);
                carCount = cars.Count;
                ToggleNewMode();
                fillForm(cars[carCount - 1]);
                MessageBox.Show("Record added successfully.");

            }
            else
            {
                ToggleNewMode();
            }
        }

        // Toggle New Mode
        private void ToggleNewMode()
        {
            newMode = !newMode;
            btnUpdate.Enabled = !btnUpdate.Enabled;
            btnDelete.Enabled = !btnDelete.Enabled;
            btnSearch.Enabled = !btnSearch.Enabled;
            btnExit.Enabled = !btnExit.Enabled;
            btnFirst.Enabled = !btnFirst.Enabled;
            btnPrevious.Enabled = !btnPrevious.Enabled;
            btnNext.Enabled = !btnNext.Enabled;
            btnLast.Enabled = !btnLast.Enabled;
            txtVehicleRegNo.Text = "";
            txtMake.Text = "";
            txtEngineSize.Text = "";
            txtDateRegistered.Text = DateTime.Now.ToShortDateString();
            txtRentalPerDay.Text = "";
            chkAvailable.Checked = false;
            txtVehicleRegNo.Focus();
        }


        //Connect to the Database
        private static SQLiteConnection sQLiteConnection()
        {   
            //Create a new database connection
            SQLiteConnection conn = new SQLiteConnection(@"Data Source=hire.db");
            try
            {  
                //Open the connection
                conn.Open();
            }
            catch (Exception error)
            {   //If connection unsuccessful, display error emssage
                MessageBox.Show(error.Message);
            }
            return conn;
        }

        //Read data from Database and return list of all cars within Database
        static List<Car> sQLiteRead(SQLiteConnection conn)
        {
            SQLiteDataReader sQLiteDataReader; //Data Reader object
            SQLiteCommand sQLiteCommand; //Database command object
            sQLiteCommand = conn.CreateCommand();
            sQLiteCommand.CommandText = "SELECT * FROM tblCar";

            sQLiteDataReader = sQLiteCommand.ExecuteReader(); //The sQliteDataReader allows us to run through each row per loop
            List<Car> list = new List<Car>();
            while (sQLiteDataReader.Read()) //Returns true if there is still a result line to read
            {
                Car car = new Car(); //Make a new car object
                car.uid = sQLiteDataReader.GetInt64(0);
                car.vehicleRegNo = sQLiteDataReader.GetString(1);
                car.make = sQLiteDataReader.GetString(2);
                car.engineSize = sQLiteDataReader.GetString(3);
                car.dateRegistered = sQLiteDataReader.GetString(4);
                car.rentalPerDay = sQLiteDataReader.GetDecimal(5);
                car.available = sQLiteDataReader.GetInt64(6);
                list.Add(car);
            }
            return list;
        }

        //Exit Button
        private void btnExit_Click(object sender, EventArgs e)
        {
            conn.Close();
            this.Close();
        }

        //Cancel Button
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (newMode)
            {
                ToggleNewMode();
                fillForm(cars[currentCar - 1]);
            } else
            {
                fillForm(cars[currentCar - 1]);
            }
        }
        
        //Delete Button
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (carCount != 1)
            {
                string uid = cars[currentCar - 1].uid.ToString();
                SQLiteCommand cmd;
                cmd = conn.CreateCommand();
                cmd.CommandText = $"DELETE FROM tblCar WHERE uid = {uid}";
                cmd.ExecuteNonQuery();
                cars = sQLiteRead(conn);
                carCount = cars.Count;
                if (currentCar > 1)
                {
                    fillForm(cars[currentCar-2]);
                } else
                {
                    fillForm(cars[0]);
                }
            } else
            {
                MessageBox.Show("Cannot delete last record in database.");
            }
        }

        //Search Button
        private void btnSearch_Click(object sender, EventArgs e)
        {
            frmSearch search = new frmSearch();
            search.ShowDialog();
        }
    }
    // Class defining a row entry
    public class Car
    {
        public Int64 uid;
        public string vehicleRegNo;
        public string make;
        public string engineSize;
        public string dateRegistered;
        public decimal rentalPerDay;
        public Int64 available;
        
    }
}
