using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Globalization;

namespace PIC16F64_Simulator
{
    public partial class GUI : Form
    {

        #region Variablen
        private Thread m_tCommandExecutor;
        private Befehlszeile m_letzteZeile;
        private PIC m_oPIC;
        private bool checkWatchDog;
        #endregion
        //Konstruktor
        public GUI()
        {
            InitializeComponent();
            //m_oPIC = new PIC();
            checkWatchDog = false;
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


        /// <summary>
        /// Dateiauswahl mit Übergabe an den Parser
        /// </summary>
        private void ladenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.InitialDirectory = "C:\\";
            ofd.Filter = "LST-Datei (*.lst)|*lst";
            ofd.Title = "Bitte LST-Datei zum öffnen auswählen";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                List.Items.Clear();
                BefehlszeilenSatz.Instance.m_BefehlszeilenList.Clear();
                callParser(ofd.FileName);
            }
        }

        /// <summary>
        /// Übergibt dem Parser den Dateipfad, der diese dann für die Anzeige übersetzt
        /// </summary>
        private void callParser(String filepath)
        {
            Uebersetzter parse = new Uebersetzter(filepath);
            parse.readFile();
            fillListView();
        }//callParser()
        private void fillListView()
        {
            foreach (Befehlszeile zeile in BefehlszeilenSatz.Instance.m_BefehlszeilenList)
            {
                ListViewItem item = new ListViewItem();
                item.SubItems.Add(zeile.LineNr.ToString());
                item.SubItems.Add(zeile.PclAsString);
                item.SubItems.Add(zeile.State);
                item.SubItems.Add(zeile.Command);
                List.Items.Add(item);
            }
           
        }//fillListView()
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

        private Object InvokeIfRequired(Control target, Delegate methodToInvoke)
        {
            if (target.InvokeRequired)
            {
                // the control must be changed by invoke since it comes from a background thread.
                return target.Invoke(methodToInvoke);
            }
            else
            {
                // The control can be changed directly
                return methodToInvoke.DynamicInvoke();
            }
        }//InvokeIfRequired()

        private void startButton_Click(object sender, EventArgs e)
        {
            startButton.Enabled = false;
            resetButton.Enabled = true;
            ladenToolStripMenuItem.Enabled = false;
            m_tCommandExecutor = new Thread(new ThreadStart(start));
            m_tCommandExecutor.Start();
        }
        private void start()
        {
            //possible because pcl is initialized with 0
           // LoadCommand(m_oPicCpu.getNextCodeLine(m_oPicCpu));
        }//run()
        public void LoadCommand(Befehlszeile aktuelleZeile)
        {
             if (aktuelleZeile == null)
             {
                 InvokeIfRequired(startButton, (MethodInvoker)delegate()
                 {
                     startButton.Enabled = true;
                 });
                 return;
             }

             //unmark previous codeLine
             InvokeIfRequired(List, (MethodInvoker)delegate()
             {
                 if (m_letzteZeile != null)
                 {
                     List.Items[m_letzteZeile.LineNr].BackColor = Color.White;
                 }

             });
             //highlight current codeline
             InvokeIfRequired(List, (MethodInvoker)delegate()
             {
                 List.Items[aktuelleZeile.LineNr].BackColor = Color.LightSkyBlue;
                 m_letzteZeile = aktuelleZeile;
                 List.EnsureVisible(aktuelleZeile.LineNr);
             });

             //update gui
            // GUIUpdate();

             //check for breakpoint
             InvokeIfRequired(List, (MethodInvoker)delegate()
             {
                 if (List.Items[aktuelleZeile.LineNr].Checked == true)
                 {
                     List.Items[aktuelleZeile.LineNr].Checked = false;
                     startButton.Enabled = true;
                     m_tCommandExecutor.Abort();
                     return;
                 }
             });

             //cache TrisA&TrisB for latch logic
             m_oPIC.cachedTrisA = m_oPIC.getRegisterValue(0x85);
             m_oPIC.cachedTrisB = m_oPIC.getRegisterValue(0x86);

             //execute the Assembler command set get ProgramCounter for the next Command
             m_oPIC.executeCommand(aktuelleZeile);

             //check if TrisA or TrisB was changed (Latch Logic)
             if (m_oPIC.cachedTrisA != m_oPIC.getRegisterValue(0x85))
                 m_oPIC.writeLatchToPort(m_oPIC.getRegisterValue(0x85), 0x05, m_oPIC.LatchA);

             if (m_oPIC.cachedTrisB != m_oPIC.getRegisterValue(0x86))
                 m_oPIC.writeLatchToPort(m_oPIC.getRegisterValue(0x86), 0x06, m_oPIC.LatchB);

             //checks if watchdog is globally enabled
             if (checkWatchDog)
              {
                  m_oPIC.incWatchDog();
              }
             
             //wait 500ms before executing the next Command
             m_oPIC.timeOut(m_oPIC.Speed);

             //check if step-button was pushed
             if (m_oPIC.Step == false)
             {
                 //call this function again with the codeLine for the next Command
                 LoadCommand(BefehlszeilenSatz.Instance.getNextBefehlszeile(m_oPIC.ProgramCounter));
             }
             else
             {
                 m_tCommandExecutor.Abort();
                 return;
             }

        }

        private void watchDogButton_Click(object sender, EventArgs e)
        {
            if (watchDogPanel.BackColor == Color.Red)
            {
                watchDogPanel.BackColor = Color.Green;
                watchDogButton.Text = "Deaktivieren";
                checkWatchDog=true;
            }
            else
            {
                watchDogPanel.BackColor = Color.Red;
                watchDogButton.Text = "Aktivieren";
                checkWatchDog = false;
            }




        } 

    }
}
