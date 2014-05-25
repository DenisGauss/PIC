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
        private Hashtable m_htSFRRegister;
        private Hashtable m_htGPRRegister;
        #endregion
        //Konstruktor
        public GUI()
        {
            InitializeComponent();
            m_oPIC = new PIC();
            checkWatchDog = false;

            #region initSFRHash
            //fill hashtable SFR register
            m_htSFRRegister = new Hashtable(16);
            m_htSFRRegister.Add(0x00, tbReg00);
            m_htSFRRegister.Add(0x01, tbReg01);
            m_htSFRRegister.Add(0x02, tbReg02);
            m_htSFRRegister.Add(0x03, tbReg03);
            m_htSFRRegister.Add(0x04, tbReg04);
            m_htSFRRegister.Add(0x05, tbReg05);
            m_htSFRRegister.Add(0x06, tbReg06);
            m_htSFRRegister.Add(0x08, tbReg08);
            m_htSFRRegister.Add(0x09, tbReg09);
            m_htSFRRegister.Add(0x0A, tbReg0A);
            m_htSFRRegister.Add(0x0B, tbReg0B);
            m_htSFRRegister.Add(0x81, tbReg81);
            m_htSFRRegister.Add(0x85, tbReg85);
            m_htSFRRegister.Add(0x86, tbReg86);
            m_htSFRRegister.Add(0x88, tbReg88);
            m_htSFRRegister.Add(0x89, tbReg89);
            #endregion

            #region initGPRHash

            // fill hashtable GPRRegister
            m_htGPRRegister = new Hashtable(36);
            m_htGPRRegister.Add(0x0C, tbReg0C);
            m_htGPRRegister.Add(0x0D, tbReg0D);
            m_htGPRRegister.Add(0x0E, tbReg0E);
            m_htGPRRegister.Add(0x0F, tbReg0F);
            m_htGPRRegister.Add(0x10, tbReg10);
            m_htGPRRegister.Add(0x11, tbReg11);
            m_htGPRRegister.Add(0x12, tbReg12);
            m_htGPRRegister.Add(0x13, tbReg13);
            m_htGPRRegister.Add(0x14, tbReg14);
            m_htGPRRegister.Add(0x15, tbReg15);
            m_htGPRRegister.Add(0x16, tbReg16);
            m_htGPRRegister.Add(0x17, tbReg17);
            m_htGPRRegister.Add(0x18, tbReg18);
            m_htGPRRegister.Add(0x19, tbReg19);
            m_htGPRRegister.Add(0x1A, tbReg1A);
            m_htGPRRegister.Add(0x1B, tbReg1B);
            m_htGPRRegister.Add(0x1C, tbReg1C);
            m_htGPRRegister.Add(0x1D, tbReg1D);
            m_htGPRRegister.Add(0x1E, tbReg1E);
            m_htGPRRegister.Add(0x1F, tbReg1F);
            m_htGPRRegister.Add(0x20, tbReg20);
            m_htGPRRegister.Add(0x21, tbReg21);
            m_htGPRRegister.Add(0x22, tbReg22);
            m_htGPRRegister.Add(0x23, tbReg23);
            m_htGPRRegister.Add(0x24, tbReg24);
            m_htGPRRegister.Add(0x25, tbReg25);
            m_htGPRRegister.Add(0x26, tbReg26);
            m_htGPRRegister.Add(0x27, tbReg27);
            m_htGPRRegister.Add(0x28, tbReg28);
            m_htGPRRegister.Add(0x29, tbReg29);
            m_htGPRRegister.Add(0x2A, tbReg2A);
            m_htGPRRegister.Add(0x2B, tbReg2B);
            m_htGPRRegister.Add(0x2C, tbReg2C);
            m_htGPRRegister.Add(0x2D, tbReg2D);
            m_htGPRRegister.Add(0x2E, tbReg2E);
            m_htGPRRegister.Add(0x2F, tbReg2F);

            #endregion initGPRHash
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
            m_oPIC.Speed = trbSpeed.Value * 50;
            textBox_speed.Text = Convert.ToString(m_oPIC.Speed);
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
            m_oPIC.Step = false;
            startButton.Enabled = false;
            stopButton.Enabled = true;
            resetButton.Enabled = true;
            ladenToolStripMenuItem.Enabled = false;
            m_tCommandExecutor = new Thread(new ThreadStart(start));
            m_tCommandExecutor.Start();
        }
        private void start()
        {
            //possible because pcl is initialized with 0
            LoadCommand(m_oPIC.getNextBefehlszeile(m_oPIC));
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
             GUI_UPDATE();

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

        private void nextButton_Click(object sender, EventArgs e)
        {
            ladenToolStripMenuItem.Enabled = false;
            startButton.Enabled = true;
            stopButton.Enabled = false;
            resetButton.Enabled = true;
            nextButton.Enabled = true;
            m_oPIC.Step = true;
            if (m_tCommandExecutor == null)
            {
                m_tCommandExecutor = new Thread(new ThreadStart(start));
                m_tCommandExecutor.Start();
            }
            else
            {
                LoadCommand(m_oPIC.getNextBefehlszeile(m_oPIC));
            }
        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            m_oPIC.Speed = Convert.ToInt32(textBox_speed.Text);
        }

        private void Label_Duration_Click(object sender, EventArgs e)
        {

        }

        private void lblBank0_Click(object sender, EventArgs e)
        {

        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            m_tCommandExecutor.Abort();
            startButton.Enabled = true;
            stopButton.Enabled = false;
            return;
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            if (m_tCommandExecutor != null)
            {
                m_tCommandExecutor.Abort();
                m_oPIC.resetPIC();
                GUI_UPDATE();
                startButton.Enabled = true;
                return;
            }
            else
            {
                startButton.Enabled = true;
                m_oPIC.resetPIC();
                GUI_UPDATE();
                return;
            }
        }

        private void tbStack1_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbStack6_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbStack2_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbStack7_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbStack0_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbStack5_TextChanged(object sender, EventArgs e)
        {

        } 

    }
}
