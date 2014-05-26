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
        private COM m_oHwPort;
        private Thread m_tSerialPortThread;
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
            m_htSFRRegister.Add(0x00, tb_Reg00);
            m_htSFRRegister.Add(0x01, tb_Reg01);
            m_htSFRRegister.Add(0x02, tb_Reg02);
            m_htSFRRegister.Add(0x03, tb_Reg03);
            m_htSFRRegister.Add(0x04, tb_Reg04);
            m_htSFRRegister.Add(0x05, tb_Reg05);
            m_htSFRRegister.Add(0x06, tb_Reg06);
            m_htSFRRegister.Add(0x08, tb_Reg08);
            m_htSFRRegister.Add(0x09, tb_Reg09);
            m_htSFRRegister.Add(0x0A, tb_Reg0A);
            m_htSFRRegister.Add(0x0B, tb_Reg0B);
            m_htSFRRegister.Add(0x81, tb_Reg81);
            m_htSFRRegister.Add(0x85, tb_Reg85);
            m_htSFRRegister.Add(0x86, tb_Reg86);
            m_htSFRRegister.Add(0x88, tb_Reg88);
            m_htSFRRegister.Add(0x89, tb_Reg89);
            #endregion

            #region initGPRHash

            // fill hashtable GPRRegister
            m_htGPRRegister = new Hashtable(36);
            m_htGPRRegister.Add(0x0C, tb_Reg0C);
            m_htGPRRegister.Add(0x0D, tb_Reg0D);
            m_htGPRRegister.Add(0x0E, tb_Reg0E);
            m_htGPRRegister.Add(0x0F, tb_Reg0F);
            m_htGPRRegister.Add(0x10, tb_Reg10);
            m_htGPRRegister.Add(0x11, tb_Reg11);
            m_htGPRRegister.Add(0x12, tb_Reg12);
            m_htGPRRegister.Add(0x13, tb_Reg13);
            m_htGPRRegister.Add(0x14, tb_Reg14);
            m_htGPRRegister.Add(0x15, tb_Reg15);
            m_htGPRRegister.Add(0x16, tb_Reg16);
            m_htGPRRegister.Add(0x17, tb_Reg17);
            m_htGPRRegister.Add(0x18, tb_Reg18);
            m_htGPRRegister.Add(0x19, tb_Reg19);
            m_htGPRRegister.Add(0x1A, tb_Reg1A);
            m_htGPRRegister.Add(0x1B, tb_Reg1B);
            m_htGPRRegister.Add(0x1C, tb_Reg1C);
            m_htGPRRegister.Add(0x1D, tb_Reg1D);
            m_htGPRRegister.Add(0x1E, tb_Reg1E);
            m_htGPRRegister.Add(0x1F, tb_Reg1F);
            m_htGPRRegister.Add(0x20, tb_Reg20);
            m_htGPRRegister.Add(0x21, tb_Reg21);
            m_htGPRRegister.Add(0x22, tb_Reg22);
            m_htGPRRegister.Add(0x23, tb_Reg23);
            m_htGPRRegister.Add(0x24, tb_Reg24);
            m_htGPRRegister.Add(0x25, tb_Reg25);
            m_htGPRRegister.Add(0x26, tb_Reg26);
            m_htGPRRegister.Add(0x27, tb_Reg27);
            m_htGPRRegister.Add(0x28, tb_Reg28);
            m_htGPRRegister.Add(0x29, tb_Reg29);
            m_htGPRRegister.Add(0x2A, tb_Reg2A);
            m_htGPRRegister.Add(0x2B, tb_Reg2B);
            m_htGPRRegister.Add(0x2C, tb_Reg2C);
            m_htGPRRegister.Add(0x2D, tb_Reg2D);
            m_htGPRRegister.Add(0x2E, tb_Reg2E);
            m_htGPRRegister.Add(0x2F, tb_Reg2F);

            #endregion initGPRHash

            #region Eventhandlers
            //Eventhandlers von den Interrupts Checkboxen
            cb_PortRa0.Click += new System.EventHandler(portAChecked);
            cb_PortRa1.Click += new System.EventHandler(portAChecked);
            cb_PortRa2.Click += new System.EventHandler(portAChecked);
            cb_PortRa3.Click += new System.EventHandler(portAChecked);
            cb_PortRa4.Click += new System.EventHandler(portAChecked);
            cb_PortRa4.Click += new System.EventHandler(RA4InterruptHandler);
            cb_PortRb0.Click += new System.EventHandler(portBChecked);
            cb_PortRb0.Click += new System.EventHandler(INTInterruptHandler);
            cb_PortRb1.Click += new System.EventHandler(portBChecked);
            cb_PortRb2.Click += new System.EventHandler(portBChecked);
            cb_PortRb3.Click += new System.EventHandler(portBChecked);
            cb_PortRb4.Click += new System.EventHandler(portBChecked);
            cb_PortRb4.Click += new System.EventHandler(RBInterruptHandler);
            cb_PortRb5.Click += new System.EventHandler(portBChecked);
            cb_PortRb5.Click += new System.EventHandler(RBInterruptHandler);
            cb_PortRb6.Click += new System.EventHandler(portBChecked);
            cb_PortRb6.Click += new System.EventHandler(RBInterruptHandler);
            cb_PortRb7.Click += new System.EventHandler(portBChecked);
            cb_PortRb7.Click += new System.EventHandler(RBInterruptHandler);
            #endregion
        }

        //Applikation Beenden - schliesst Programm
        private void beendenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //Version anzeigen
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

        //Geschwindigkeit ueber den Regler einstellbar
        private void trb_Speed_Scroll(object sender, EventArgs e)
        {
            m_oPIC.Speed = trb_Speed.Value * 50;
            tb_Speed.Text = Convert.ToString(m_oPIC.Speed);
        }

        //Datei laden und Befehlszeilen in der GUI anzeigen
        private void ladenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.InitialDirectory = "C:\\";
            ofd.Filter = "LST-Datei (*.lst)|*lst";
            ofd.Title = "Bitte LST-Datei zum öffnen auswählen";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                List.Items.Clear();
                BefehlszeilenSatz.Instanz.m_BefehlszeilenList.Clear();
                callParser(ofd.FileName);
            }
        }

        //Parser wandelt die Datei um sodass diese verarbeitbar ist 
        private void callParser(String filepath)
        {
            Uebersetzter parse = new Uebersetzter(filepath);
            parse.readFile();
            fillListView();
        }

        //BefehlszeilenListe wird mit den Befehlszeile gefuellt
        private void fillListView()
        {
            foreach (Befehlszeile zeile in BefehlszeilenSatz.Instanz.m_BefehlszeilenList)
            {
                ListViewItem item = new ListViewItem();
                item.SubItems.Add(zeile.LineNr.ToString());
                item.SubItems.Add(zeile.PclAsString);
                item.SubItems.Add(zeile.State);
                item.SubItems.Add(zeile.Command);
                List.Items.Add(item);
            }
           
        }

        //Doku.pdf aufrufen oder gegebenfalls Fehlermeldung bringen
        private void dokumentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(@System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\doku.pdf");
            }
            catch { MessageBox.Show("doku.pdf not found", "File not found", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        //Variableen aendern falls notwendig bzw. direkt aendern / Muss mit Invoke gemacht werden da es im Hintergrund laeuft - Thread
        private Object InvokeIfRequired(Control target, Delegate methodToInvoke)
        {
            if (target.InvokeRequired)
            {
                
                return target.Invoke(methodToInvoke);
            }
            else
            {
               
                return methodToInvoke.DynamicInvoke();
            }
        }

        //Start Button gedruekt - ProgrammStart
        private void btn_Start_Click(object sender, EventArgs e)
        {
            m_oPIC.Step = false;
            btn_Start.Enabled = false;
            btn_Stop.Enabled = true;
            btn_Reset.Enabled = false;
            ladenToolStripMenuItem.Enabled = false;
            m_tCommandExecutor = new Thread(new ThreadStart(start));
            m_tCommandExecutor.Start();
        }

        //Ersten bzw. naechsten Befehl einlesen
        private void start()
        {
            LoadCommand(m_oPIC.getNextBefehlszeile(m_oPIC));
        }

        public void LoadCommand(Befehlszeile aktuelleZeile)
    {
             if (aktuelleZeile == null) // Falls Programm nicht läuft Start-Button = Enable
             {
                 InvokeIfRequired(btn_Start, (MethodInvoker)delegate()
                 {
                     btn_Start.Enabled = true;
                 });
                 return;
             }
         

             //Vorherige Zeile entmarkieren
             InvokeIfRequired(List, (MethodInvoker)delegate()
             {
                 if (m_letzteZeile != null)
                 {
                     List.Items[m_letzteZeile.LineNr].BackColor = Color.White;
                 }

             });
             //Aktuelle Zeile markieren
             InvokeIfRequired(List, (MethodInvoker)delegate()
             {
                 List.Items[aktuelleZeile.LineNr].BackColor = Color.LightSkyBlue;
                 m_letzteZeile = aktuelleZeile;
                 List.EnsureVisible(aktuelleZeile.LineNr);
             });

             //GUI Updaten
             GUI_UPDATE();

             //Auf Breakpoints ueberpruefen
             InvokeIfRequired(List, (MethodInvoker)delegate()
             {
                 if (List.Items[aktuelleZeile.LineNr].Checked == true)
                 {
                     List.Items[aktuelleZeile.LineNr].Checked = false;
                     btn_Start.Enabled = true;
                     m_tCommandExecutor.Abort();
                     return;
                 }
             });

             //TRISA und TRISB zwischenspeichern fuer Latch
             m_oPIC.cachedTrisA = m_oPIC.getRegisterValue(0x85);
             m_oPIC.cachedTrisB = m_oPIC.getRegisterValue(0x86);

             //AssemblerBefehl der aktuellen Zeile ausfuehren
             m_oPIC.executeCommand(aktuelleZeile);

             //Ueberpruefen ob TRISA und TRISB vertauscht wurden (Latch Logik)
             if (m_oPIC.cachedTrisA != m_oPIC.getRegisterValue(0x85))
                 m_oPIC.writeLatchToPort(m_oPIC.getRegisterValue(0x85), 0x05, m_oPIC.LatchA);

             if (m_oPIC.cachedTrisB != m_oPIC.getRegisterValue(0x86))
                 m_oPIC.writeLatchToPort(m_oPIC.getRegisterValue(0x86), 0x06, m_oPIC.LatchB);

             //Ueberpruefen ob WatchDog an
             if (checkWatchDog)
              {
                  m_oPIC.incWatchDog();
              }
             
             //Taktfrequenz - 250ms warten bis zum naechsten Befehl
             m_oPIC.timeOut(m_oPIC.Speed);

             //Ueberpruefen ob der Schrittmodus aktiv ist
             if (m_oPIC.Step == false)
             {
                 //Rekursiver aufruf dieser Funktion ( naechster Schritt)
                 LoadCommand(BefehlszeilenSatz.Instanz.getNextBefehlszeile(m_oPIC.ProgramCounter));
             }
             else
             {    
                 //Anhalten bis Next gedrueckt
                 m_tCommandExecutor.Abort();
                 return;
             }

        }
                
        //Ueberpruefen ob an Checkboxen von PortA gecheckt und werte dementsprechend in die Register schreiben
        public void portAChecked(object sender, EventArgs e)
        {
            m_oPIC.getSFRMemory()[0x05] = m_oPIC.setBitAtPosition(m_oPIC.getSFRMemory()[0x05], 0, cb_PortRa0.Checked);
            m_oPIC.getSFRMemory()[0x05] = m_oPIC.setBitAtPosition(m_oPIC.getSFRMemory()[0x05], 1, cb_PortRa1.Checked);
            m_oPIC.getSFRMemory()[0x05] = m_oPIC.setBitAtPosition(m_oPIC.getSFRMemory()[0x05], 2, cb_PortRa2.Checked);
            m_oPIC.getSFRMemory()[0x05] = m_oPIC.setBitAtPosition(m_oPIC.getSFRMemory()[0x05], 3, cb_PortRa3.Checked);
            m_oPIC.getSFRMemory()[0x05] = m_oPIC.setBitAtPosition(m_oPIC.getSFRMemory()[0x05], 4, cb_PortRa4.Checked);

            GUI_UPDATE();
        }

        //Ueberpruefen ob an Checkboxen von PortB gecheckt und werte dementsprechend in die Register schreiben
        public void portBChecked(object sender, EventArgs e)
        {
            m_oPIC.getSFRMemory()[0x06] = m_oPIC.setBitAtPosition(m_oPIC.getSFRMemory()[0x06], 0, cb_PortRb0.Checked);
            m_oPIC.getSFRMemory()[0x06] = m_oPIC.setBitAtPosition(m_oPIC.getSFRMemory()[0x06], 1, cb_PortRb1.Checked);
            m_oPIC.getSFRMemory()[0x06] = m_oPIC.setBitAtPosition(m_oPIC.getSFRMemory()[0x06], 2, cb_PortRb2.Checked);
            m_oPIC.getSFRMemory()[0x06] = m_oPIC.setBitAtPosition(m_oPIC.getSFRMemory()[0x06], 3, cb_PortRb3.Checked);
            m_oPIC.getSFRMemory()[0x06] = m_oPIC.setBitAtPosition(m_oPIC.getSFRMemory()[0x06], 4, cb_PortRb4.Checked);
            m_oPIC.getSFRMemory()[0x06] = m_oPIC.setBitAtPosition(m_oPIC.getSFRMemory()[0x06], 5, cb_PortRb5.Checked);
            m_oPIC.getSFRMemory()[0x06] = m_oPIC.setBitAtPosition(m_oPIC.getSFRMemory()[0x06], 6, cb_PortRb6.Checked);
            m_oPIC.getSFRMemory()[0x06] = m_oPIC.setBitAtPosition(m_oPIC.getSFRMemory()[0x06], 7, cb_PortRb7.Checked);

            GUI_UPDATE();
        }
       
        //Bei Aufruf Interrupt ausfuehren
        public void RA4InterruptHandler(object sender, EventArgs e)
        {
            m_oPIC.PortRA4Interrupt();
            GUI_UPDATE();
        }

        //Bei Aufruf Interrupt ausfuehren
        public void RBInterruptHandler(object sender, EventArgs e)
        {
            m_oPIC.PortRBInterrupt();
            GUI_UPDATE();
        }

        //Bei Aufruf Interrupt ausfuehren
        public void INTInterruptHandler(object sender, EventArgs e)
        {
            m_oPIC.INTInterrupt();
            GUI_UPDATE();
        }

        //Ueberpruefen ob WatchDog aktiviert falls ja Variable setzen
        private void btn_WatchDog_Click(object sender, EventArgs e)
        {
            if (watchDogPanel.BackColor == Color.Red)
            {
                watchDogPanel.BackColor = Color.Green;
                btn_WatchDog.Text = "Deaktivieren";
                checkWatchDog=true;
            }
            else
            {
                watchDogPanel.BackColor = Color.Red;
                btn_WatchDog.Text = "Aktivieren";
                checkWatchDog = false;
            }




        }

        //Schrittmodus aktivieren andere Buttons
        private void btn_Next_Click(object sender, EventArgs e)
        {
            ladenToolStripMenuItem.Enabled = false;
            btn_Start.Enabled = true;
            btn_Stop.Enabled = false;
            btn_Reset.Enabled = true;
            btn_Next.Enabled = true;
            m_oPIC.Step = true;
            if (m_tCommandExecutor == null)
            {
                m_tCommandExecutor = new Thread(new ThreadStart(start));
                m_tCommandExecutor.Start();
            }
            
            {
                LoadCommand(m_oPIC.getNextBefehlszeile(m_oPIC));
            }
        }

        //Eingegebene Quarzfrequenz als Taktfrequenz setzten
        private void btn_TaktSetzen_Click(object sender, EventArgs e)
        {
            m_oPIC.Speed = Convert.ToInt32(tb_Speed.Text);
        }

        //Befehlsabarbeitung abbrechen
        private void btn_Stop_Click(object sender, EventArgs e)
        {
            m_tCommandExecutor.Abort();
            btn_Start.Enabled = true;
            btn_Stop.Enabled = false;
            btn_Reset.Enabled = true;
            ladenToolStripMenuItem.Enabled = true;
           
            return;
        }
 
        //Reset durchfuehren - Werte Ruecksetzten - COM Verbindung trennen
        private void btn_Reset_Click(object sender, EventArgs e)
        {
            //Serial Port Thread beenden
            if (m_tSerialPortThread != null)
            {
                m_oHwPort.sPort.Close();
                m_oHwPort.actuelConnectionState = COM.ConnectionState.IDLE;
                m_tSerialPortThread.Abort();
            }

            //COM Verbinde Button wieder an
            btn_VerbindeCom.Enabled = true;


            if (m_tCommandExecutor != null)
            {
                m_tCommandExecutor.Abort();
                m_oPIC.resetPIC();
                List.EnsureVisible(0);
                GUI_UPDATE();
                btn_Start.Enabled = true;
                
                return;
            }
            else
            {
                btn_Start.Enabled = true;
                m_oPIC.resetPIC();
                GUI_UPDATE();
                return;
            }
        }

        //COM Schnittstelle aktiviern und Farben aendern
        private void btn_VerbindeCom_Click(object sender, EventArgs e)
        {
            if (serialPanel.BackColor == Color.Red)
            {
                //COM Verbinde Button ausschalten
                btn_VerbindeCom.Enabled = false;

                GUI temp = this;

                //Neues COM Objekt erzeugen via Konstruktor
                m_oHwPort = new COM(ref m_oPIC, ref temp, "COM1");
                m_tSerialPortThread = new Thread(new ThreadStart(m_oHwPort.run));
                m_tSerialPortThread.Start();
                while (m_oHwPort.actuelConnectionState == COM.ConnectionState.IDLE)
                {
                    //Waren bis Verbindung Steht
                }
                GUI_UPDATE();
            }
        }


    }
}
