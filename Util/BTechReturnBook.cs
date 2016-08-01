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
    public partial class BTechReturnBook : Form
    {
        SpeechRecognitionEngine sre = new SpeechRecognitionEngine();

        SqlConnection con;
        SqlDataAdapter da;
        DataSet ds = new DataSet();

        public BTechReturnBook()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void refund()
        {
            try
            {
                con = new SqlConnection(@"Data Source=RUMMAN92;Initial Catalog=BTech;Integrated Security=True");
                con.Open();

                string query = string.Format("select [Number of Copies] from BTechDetails where [ID Number]='{0}'", textBox1.Text);
                da = new SqlDataAdapter(query, con);

                da.Fill(ds, "BTechDetails");

                int number = 0;
                foreach (DataRow dr in ds.Tables["BTechDetails"].Rows)
                {
                    number = Convert.ToInt32(dr["Number of Copies"]);
                }
                number = number + 1;

                string update = string.Format("update BTechDetails set [Number of Copies]={0} where [ID Number]='{1}'", number, textBox1.Text);
                da.UpdateCommand = new SqlCommand(update, con);
                da.UpdateCommand.ExecuteNonQuery();

                string del = string.Format("delete from BTechIssueConfirm where [Card Number]='{0}'", textBox2.Text);
                da.DeleteCommand = new SqlCommand(del, con);
                da.DeleteCommand.ExecuteNonQuery();

                MessageBox.Show("Book Returned", "Return");

                textBox1.Clear();
                textBox2.Clear();

                textBox1.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BTechReturnBook_Load(object sender, EventArgs e)
        {
            Choices ch = new Choices();
            ch.Add(new string[] { "refund", "close" });

            GrammarBuilder gb = new GrammarBuilder(ch);
            Grammar g = new Grammar(gb);

            sre.LoadGrammarAsync(g);
            sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(sre_SpeechRecognized);
            sre.SetInputToDefaultAudioDevice();
            sre.RecognizeAsync(RecognizeMode.Multiple);

        }

        void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            try
            {
                if (e.Result.Text == "refund")
                {
                    refund();
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
    }
}
