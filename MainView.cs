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
            MessageBox.Show(
                " PIC16C84 Simulator "
                + Environment.NewLine
                + Environment.NewLine
                + "TINF12B5"
                + Environment.NewLine
                + "Martin Zerler"
                + Environment.NewLine
                + "Denis Gauss"
                + Environment.NewLine
                + "Contact: martin.zerler@yahoo.de"
                + Environment.NewLine
                + Environment.NewLine
                + "Last Update: 12.05 2014"
                + Environment.NewLine
                + "Version 1.0",
                "Info",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void gprBox_Enter(object sender, EventArgs e)
        {

        }

        private void trbSpeed_Scroll(object sender, EventArgs e)
        {

        }

        private void label69_Click(object sender, EventArgs e)
        {
            
        }

        private void ladenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.InitialDirectory = "C:\\";
            ofd.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            ofd.Title = "Bitte Datei zum öffnen auswählen";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
             
            }
        }

        private void List_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dokumentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(@System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\doku.pdf");
            }
            catch { MessageBox.Show("doku.pdf not found", "File not found", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
    }
}
