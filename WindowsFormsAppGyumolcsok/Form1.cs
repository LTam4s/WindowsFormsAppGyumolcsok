﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WindowsFormsAppGyumolcsok
{
    public partial class Form1 : Form
    {
        MySqlConnection conn = null;
        MySqlCommand cmd = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            numericUpDownEgysegar.Maximum = int.MaxValue;
            numericUpDownMennyiseg.Maximum = int.MaxValue;
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
            builder.Server = "localhost";
            builder.UserID = "root";
            builder.Password = "";
            builder.Database = "gyumolcsok";
            conn = new MySqlConnection(builder.ConnectionString);
            try
            {
                conn.Open();
                cmd = conn.CreateCommand();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + "A program leáll!");
                Environment.Exit(0);
                throw;
            }
            conn.Close();
            gyumolcsok_lista_update();
        }

        private void gyumolcsok_lista_update()
        {
            listBox1.Items.Clear();
            cmd.CommandText = "SELECT `id`, `nev`, `egysegar`, `mennyiseg` FROM `gyumolcsok` WHERE 1";
            conn.Open();
            using (MySqlDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    gyumolcsok uj = new gyumolcsok(dr.GetInt32("id"), dr.GetString("nev"), dr.GetInt32("egysegar"), dr.GetInt32("mennyiseg"));
                    listBox1.Items.Add(uj);
                }
            }
            conn.Close();
        }

        private void insert_Click(object sender, EventArgs e)
        {
            conn.Open();
            if (string.IsNullOrEmpty(textBoxNev.Text))
            {
                MessageBox.Show("Adjon meg nevet!");
                textBoxNev.Focus();
                conn.Close();
                return;
            }
            cmd.CommandText = "INSERT INTO `gyumolcsok`(`nev`, `egysegar`, `mennyiseg`) VALUES (@nev, @ar, @mennyiseg)";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@nev", textBoxNev.Text);
            cmd.Parameters.AddWithValue("@ar", numericUpDownEgysegar.Value.ToString());
            cmd.Parameters.AddWithValue("@mennyiseg", numericUpDownMennyiseg.Value.ToString());
            try
            {
                if (cmd.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("Sikeresen rögzítve!");
                    textBoxId.Text = "";
                    textBoxNev.Text = "";
                    numericUpDownEgysegar.Value = numericUpDownEgysegar.Minimum;
                    numericUpDownMennyiseg.Value = numericUpDownMennyiseg.Minimum;

                }
                else
                {
                    MessageBox.Show("sikertelen rögzítés!");
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                conn.Close();
            }
            conn.Close();
            gyumolcsok_lista_update();
        }
    }
}
