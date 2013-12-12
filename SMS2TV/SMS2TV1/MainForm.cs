using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Svt.Caspar;
using Svt.Network;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using MySql.Data;
using System.Net;
using System.IO;

namespace SMS2TV1
{
    public partial class MainForm : Form
    {
        //Declaring Caspargc object
        private CasparDevice cDevice;
        private CasparCGDataCollection cData;
        private bool testConnection;
        private Queue<SMSMessage> playoutList;
        private List<SMSMessage> cache;

        //mysql var
        private MySqlConnection m_conn;
        private MySqlCommand m_command;
        private MySqlDataAdapter mAdapter;
        private DataSet mds;

        private StringBuilder screenText;

        public MainForm()
        {
            InitializeComponent();
            cDevice = new CasparDevice();
            cData = new CasparCGDataCollection();
            playoutList = new Queue<SMSMessage>();
            cache = new List<SMSMessage>();
            screenText = new StringBuilder();
            playoutdataGridView.ColumnCount = 4;
            playoutdataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            playoutdataGridView.ColumnHeadersVisible = true;
            playoutdataGridView.Columns[0].Name = "Time";
            playoutdataGridView.Columns[1].Name = "Sender";
            playoutdataGridView.Columns[2].Name = "Nickname";
            playoutdataGridView.Columns[3].Name = "Message";

            m_conn = new MySqlConnection();
            m_command = new MySqlCommand();
            string mConnectionString = "server=localhost;user=root;database=sms2tv;port=3306;password='';";
            m_conn.ConnectionString = mConnectionString;
            m_command.Connection = m_conn;
            mds = new DataSet();
            

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
           
            this.CenterToScreen();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void panel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            sendAdminMessage();
        }

        private void Connectbutton_Click(object sender, EventArgs e)
        {
            if (!cDevice.IsConnected)
            {
                connectToServer();
            }
            else
            {

            }
        }

        private void connectToServer()
        {
            try
            {
                cDevice.Settings.Hostname = Properties.Settings.Default.Hostname;
                cDevice.Settings.Port = Properties.Settings.Default.Port;
                testConnection = cDevice.Connect();
                if (testConnection)
                {
                    connectionlabel.Text = "Connected";
                    connectionlabel.ForeColor = System.Drawing.Color.Lime;
                }
                else
                {
                    connectionlabel.Text = "Disconnected";
                    connectionlabel.ForeColor = System.Drawing.Color.Red;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void sendAdminMessage()
        {
            try
            {
                if (cDevice.IsConnected)
                {
                    cData.Clear();

                    cData.SetData("f0", adminMessagetextBox.Text);
                    cDevice.Channels[Properties.Settings.Default.CasparChannel - 1].CG.Add(10, 1, Properties.Settings.Default.TimeTable, true, cData);
                    cDevice.Channels[Properties.Settings.Default.CasparChannel - 1].CG.Play(10, 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void adminClearbutton_Click(object sender, EventArgs e)
        {
            adminMessagetextBox.Text = "";
            sendertextBox.Text = "";
        }

        private void approvebutton_Click(object sender, EventArgs e)
        {
            try
            {
               
                SMSMessage sms = new SMSMessage { Nickname = editNicknametextBox.Text, Sender = editTeltextBox.Text, Time = editTimetextBox.Text, Message = editmessagetextBox.Text };
                cache.Add(sms);

                playoutdataGridView.Invoke(new MethodInvoker(() =>
                {
                    playoutList.Enqueue(sms);
                    playoutdataGridView.Rows.Add(sms.Time, sms.Sender, sms.Nickname, sms.Message);
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                editNicknametextBox.Text = row.Cells[1].Value.ToString();
                editTeltextBox.Text = row.Cells[0].Value.ToString();
                editTimetextBox.Text = row.Cells[3].Value.ToString();
                string inputText = row.Cells[2].Value.ToString();
                //BlackList
                string outputText = blackListTest(inputText);
                editmessagetextBox.Text = outputText;
            }
        }

        private void retrieveSmsbutton_Click(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'smsDBDataSet.smsTable' table. You can move, or remove it, as needed.
           // this.smsTableTableAdapter.Fill(this.smsDBDataSet.smsTable);
            try
            {
                string query = "select phonenumber as 'Phone Number', Nickname, message, received_time as 'Time' from sms_table";
                mAdapter = new MySqlDataAdapter(query, m_conn);
                mAdapter.Fill(mds, "sms");
                dataGridView1.DataSource = mds;
                dataGridView1.DataMember = "sms";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void sendToScreenbutton_Click(object sender, EventArgs e)
        {
            try
            {
                if (cDevice.IsConnected)
                {
                    foreach(SMSMessage content in cache)
                    {
                        screenText.Append(content.ToString()).Append("\t\t");
                    }
                        cData.Clear();
                        cData.SetData("f0", screenText.ToString());
                        cDevice.Channels[Properties.Settings.Default.CasparChannel - 1].CG.Add(11, 1, Properties.Settings.Default.Scroller, true, cData);
                        cDevice.Channels[Properties.Settings.Default.CasparChannel - 1].CG.Play(11, 1);
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }


            
        }

        public string blackListTest(string inputText)
        {
            Regex wordFilter = new Regex("(fuck|pussy|dick|motherfucker|ass|bitch)", RegexOptions.IgnoreCase);
            return wordFilter.Replace(inputText, "XXXX");
        }

        private void loadSMSbutton_Click(object sender, EventArgs e)
        {
            try
            {
                String url = "http://v2nportal.com/sms/tv/get_message.php?username=Chat&password=vas2nets&shid=38120&keywords=LOVE,KISS,TED";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader input = new StreamReader(response.GetResponseStream());

                DataSet ds = new DataSet();
                ds.ReadXml(input);

                dataGridView1.DataSource = ds.Tables[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
       
    }
}
