﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Speech.Recognition;

namespace Utility
{
    public partial class BArchDetofIssuedBooks : Form
    {
        SpeechRecognitionEngine sre=new SpeechRecognitionEngine();

        SqlConnection con;
        SqlDataAdapter da;
        DataSet ds = new DataSet();

        public BArchDetofIssuedBooks()
        {
            InitializeComponent();

            try
            {
                con = new SqlConnection(@"Data Source=RUMMAN92\SQLEXPRESS;Initial Catalog=BArch;Integrated Security=True");
                con.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void BArchDetofIssuedBooks_Load(object sender, EventArgs e)
        {
            try
            {
                string det = string.Format("select [Card Number] from BArchIssueConfirm");
                da = new SqlDataAdapter(det, con);

                da.Fill(ds, "BArchIssueConfirm");
                foreach (DataRow dr in ds.Tables["BArchIssueConfirm"].Rows)
                {
                    listBox1.Items.Add(dr["Card Number"].ToString());
                }

                Choices ch = new Choices();
                ch.Add(new string[] { "search", "close" });

                GrammarBuilder gb = new GrammarBuilder(ch);
                Grammar g = new Grammar(gb);

                sre.LoadGrammarAsync(g);
                sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(sre_SpeechRecognized);
                sre.SetInputToDefaultAudioDevice();
                sre.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            try
            {
                if (e.Result.Text == "search")
                {
                    search();
                }

                else if (e.Result.Text == "close")
                {
                    this.Close();
                    sre.Dispose();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failure");
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string query = string.Format("select * from BArchIssueConfirm where [Card Number]='{0}'", listBox1.SelectedItem.ToString());
                da = new SqlDataAdapter(query, con);

                da.Fill(ds, "BArchIssueConfirm");

                foreach (DataRow dr in ds.Tables["BArchIssueConfirm"].Rows)
                {
                    textBox6.Text = dr["Name of Student"].ToString();
                    textBox2.Text = dr["Card Number"].ToString();
                    textBox3.Text = dr["Book ID"].ToString();
                    textBox4.Text = dr["Time"].ToString();
                }

                string det = string.Format("select Name from BArchDetails where [ID Number]='{0}'", textBox3.Text);
                da = new SqlDataAdapter(det, con);

                da.Fill(ds, "BArchDetails");

                foreach (DataRow dr in ds.Tables["BArchDetails"].Rows)
                {
                    textBox1.Text = dr["Name"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void search()
        {
            if (!(listBox1.Items.Contains(textBox5.Text)))
            {
                MessageBox.Show("Card Number not found !", "Not Found");
            }

            else
            {
                try
                {
                    string query = string.Format("select * from BArchIssueConfirm where [Card Number]='{0}'", textBox5.Text);
                    da = new SqlDataAdapter(query, con);

                    da.Fill(ds, "BArchIssueConfirm");

                    foreach (DataRow dr in ds.Tables["BArchIssueConfirm"].Rows)
                    {
                        textBox2.Text = dr["Card Number"].ToString();
                        textBox3.Text = dr["Book ID"].ToString();
                        textBox4.Text = dr["Time"].ToString();
                        textBox6.Text = dr["Name of Student"].ToString();
                    }

                    string det = string.Format("select Name from BArchDetails where [ID Number]='{0}'", textBox3.Text);
                    da = new SqlDataAdapter(det, con);

                    da.Fill(ds, "BArchDetails");

                    foreach (DataRow dr in ds.Tables["BArchDetails"].Rows)
                    {
                        textBox1.Text = dr["Name"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
