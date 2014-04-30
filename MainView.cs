using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PIC16F64_Simulator
{
    public partial class MainView : Form
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void saveFile_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void MainView_Load(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void beendenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void speichernUnterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string openFD = "";
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                openFD = openFileDialog1.FileName;
            }
        }

        private void speichernToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void versionToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
