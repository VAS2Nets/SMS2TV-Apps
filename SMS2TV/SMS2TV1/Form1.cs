using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace SMS2TV1
{
    public partial class Form1 : Form
    {
        private SqlConnection conn;
        private SqlCommand command;
        private SqlDataReader rd;

        private MySqlConnection mconn;
        private MySqlCommand mcommand;
        private MySqlDataReader mrd;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.CenterToScreen();
            conn = new SqlConnection();
            conn.ConnectionString = "Data Source=.;Initial Catalog=smsDB;Integrated Security=True";
            command = new SqlCommand();
            command.Connection = conn;

            mconn = new MySqlConnection();
            mconn.ConnectionString = "server=localhost;user=root;database=sms2tv;port=3306;password='';";
            mcommand = new MySqlCommand();
            mcommand.Connection = mconn;
        }

        private void loginbutton_Click(object sender, EventArgs e)
        {
            loginMethod();
        }

        private void loginMethod()
        {

            try
            {
                mconn.Open();
                mcommand.CommandText = "select username, password from user where username = @u and password = @p";
                mcommand.Parameters.AddWithValue("@u", usernametextBox.Text);
                mcommand.Parameters.AddWithValue("@p", passwordtextBox.Text);
                mrd = mcommand.ExecuteReader();
                if (mrd.Read())
                {
                    MainForm mf = new MainForm();
                    mf.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Invalid Credentials!!!");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            finally
            {
                mconn.Close();
            }
        }
    }
}
