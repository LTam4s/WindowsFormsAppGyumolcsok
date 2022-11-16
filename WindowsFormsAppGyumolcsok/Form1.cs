using MySql.Data.MySqlClient;
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

        private void update_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
            {
                MessageBox.Show("Nincs kijelölve gyümölcs!");
                return;
            }
            cmd.Parameters.Clear();
            gyumolcsok kivalasztott_gyumolcs = (gyumolcsok)listBox1.SelectedItem;
            cmd.CommandText = "UPDATE `gyumolcsok` SET `nev`= @nev,`egysegar`= @egysegar,`mennyiseg`= @mennyiseg WHERE `id` = @id";
            cmd.Parameters.AddWithValue("@id", textBoxId.Text);
            cmd.Parameters.AddWithValue("@nev", textBoxNev.Text);
            cmd.Parameters.AddWithValue("@egysegar", numericUpDownEgysegar.Value);
            cmd.Parameters.AddWithValue("@mennyiseg", numericUpDownMennyiseg.Value);
            conn.Open();
            if (cmd.ExecuteNonQuery() == 1)
            {
                MessageBox.Show("Módosítás sikeres votl!");
                conn.Close();
                textBoxId.Text = "";
                textBoxId.Text = "";
                textBoxNev.Text = "";
                numericUpDownEgysegar.Value = numericUpDownEgysegar.Minimum;
                numericUpDownMennyiseg.Value = numericUpDownMennyiseg.Minimum;
                gyumolcsok_lista_update();
            }
            else
            {
                MessageBox.Show("Az adatok módosítása sikertelen!");
            }
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }

        private void delete_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
            {
                return;
            }
            cmd.CommandText = "DELETE FROM `gyumolcsok` WHERE id = @id";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@id", textBoxId.Text);
            conn.Open();
            if (cmd.ExecuteNonQuery() == 1)
            {
                MessageBox.Show("Törlés sikeres");
                conn.Close();
                textBoxId.Text = "";
                textBoxId.Text = "";
                textBoxNev.Text = "";
                numericUpDownEgysegar.Value = numericUpDownEgysegar.Minimum;
                numericUpDownMennyiseg.Value = numericUpDownMennyiseg.Minimum;
                gyumolcsok_lista_update();
            }
            else
            {
                MessageBox.Show("Törlés sikertelen");
            }
            
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
            {
                return;
            }
            gyumolcsok gyumolcsok = (gyumolcsok)listBox1.SelectedItem;
            textBoxId.Text = gyumolcsok.Id.ToString();
            textBoxNev.Text = gyumolcsok.Nev;
            numericUpDownEgysegar.Value = gyumolcsok.Egysegar;
            numericUpDownMennyiseg.Value = gyumolcsok.Mennyiseg;

        }
    }
}
