using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace PIC16F64_Simulator
{
    partial class GUI
    {
        public void GUI_UPDATE()
        {

            #region Update Register
            //SFR Register
            for (int i = 0x00; i < 0x0C; i++)
            {
                if (i == 0x07) continue;

                InvokeIfRequired(((TextBox)m_htSFRRegister[i]), (MethodInvoker)delegate()
                {
                    ((TextBox)m_htSFRRegister[i]).Text = String.Format("0x{0:x2}", System.Convert.ToInt32(m_oPIC.getSFRMemory()[i].ToString()));
                });
            }

            //SFR Register
            for (int i = 0x81; i < 0x8A; i++)
            {
                if (i == 0x82 || i == 0x82 || i == 0x83 || i == 0x84 || i == 0x87) continue;

                InvokeIfRequired(((TextBox)m_htSFRRegister[i]), (MethodInvoker)delegate()
                {
                    ((TextBox)m_htSFRRegister[i]).Text = String.Format("0x{0:x2}", System.Convert.ToInt32(m_oPIC.getSFRMemory()[i].ToString()));
                });
            }


            //GPR Register
            for (int i = 0x0C; i < 0x30; i++)
            {
                InvokeIfRequired(((TextBox)m_htGPRRegister[i]), (MethodInvoker)delegate()
                {
                    ((TextBox)m_htGPRRegister[i]).Text = String.Format("0x{0:x2}", System.Convert.ToInt32(m_oPIC.getGPRMemory()[i].ToString()));
                });
            }

            //W Register
            InvokeIfRequired((tb_RegW), (MethodInvoker)delegate()
            {
                tb_RegW.Text = String.Format("0x{0:x2}", System.Convert.ToInt32(m_oPIC.WRegisterValue));
            });
            #endregion Update Register

            #region WatchDog Update
            //Watchdog
            InvokeIfRequired((tb_WatchDogCounter), (MethodInvoker)delegate()
            {
                if (watchDogPanel.BackColor == Color.Green)
                {
                    tb_WatchDogCounter.Enabled = true;
                    tb_WatchDogCounter.Text = m_oPIC.WatchDog.ToString();
                    tb_WatchDogCounter.Enabled = false;
                }
            });
            #endregion

            #region Laufzeit Update
            InvokeIfRequired((lbl_Laufzeit), (MethodInvoker)delegate()
            {
                lbl_Laufzeit.Text = String.Format("{0} µs", System.Convert.ToInt32(m_oPIC.Duration));
            });
            #endregion

            #region Status Register Update
            //StatusRegister -> ZeroFlag
            InvokeIfRequired((cb_StatusZ), (MethodInvoker)delegate()
            {
                cb_StatusZ.Enabled = true;
                cb_StatusZ.Checked = m_oPIC.checkZeroFlag();
                cb_StatusZ.Enabled = false;
            });

            //StatusRegister -> !PD
            InvokeIfRequired((cb_StatusPD), (MethodInvoker)delegate()
            {
                cb_StatusPD.Enabled = true;
                cb_StatusPD.Checked = m_oPIC.checkPDFlag();
                cb_StatusPD.Enabled = false;
            });

            //StatusRegister -> !TO
            InvokeIfRequired((cb_StatusTO), (MethodInvoker)delegate()
            {
                cb_StatusPD.Enabled = true;
                cb_StatusPD.Checked = m_oPIC.checkTOFlag();
                cb_StatusPD.Enabled = false;
            });

            //StatusRegister -> CarryFlag
            InvokeIfRequired((cb_StatusC), (MethodInvoker)delegate()
            {
                cb_StatusC.Enabled = true;
                cb_StatusC.Checked = m_oPIC.checkCarryFlag();
                cb_StatusC.Enabled = false;
            });

            //StatusRegister -> DCFlag
            InvokeIfRequired((cb_StatusDc), (MethodInvoker)delegate()
            {
                cb_StatusDc.Enabled = true;
                cb_StatusDc.Checked = m_oPIC.checkDCFlag();
                cb_StatusDc.Enabled = false;
            });

            //StatusRegister -> RP0
            InvokeIfRequired((cb_StatusRp0), (MethodInvoker)delegate()
            {
                cb_StatusRp0.Enabled = true;
                cb_StatusRp0.Checked = m_oPIC.checkRP0Flag();
                cb_StatusRp0.Enabled = false;
            });
            #endregion Status Register Update

            #region Bank Update
            //Aktive Bank
            InvokeIfRequired((lbl_Bank), (MethodInvoker)delegate()
            {
                if (m_oPIC.checkRP0Flag()) lbl_Bank.Text = "Bank1";
                else lbl_Bank.Text = "Bank0";
            });
            #endregion

            #region Stack Update
            //Stack Updaten 
            InvokeIfRequired((tbStack0), (MethodInvoker)delegate()
            {
                tbStack0.Text = String.Format("0x{0:x4}", System.Convert.ToInt32(m_oPIC.getStack()[0].ToString()));
            });
            InvokeIfRequired((tbStack1), (MethodInvoker)delegate()
            {
                tbStack1.Text = String.Format("0x{0:x4}", System.Convert.ToInt32(m_oPIC.getStack()[1].ToString()));
            });
            InvokeIfRequired((tbStack2), (MethodInvoker)delegate()
            {
                tbStack2.Text = String.Format("0x{0:x4}", System.Convert.ToInt32(m_oPIC.getStack()[2].ToString()));
            });
            InvokeIfRequired((tbStack3), (MethodInvoker)delegate()
            {
                tbStack3.Text = String.Format("0x{0:x4}", System.Convert.ToInt32(m_oPIC.getStack()[3].ToString()));
            });
            InvokeIfRequired((tbStack4), (MethodInvoker)delegate()
            {
                tbStack4.Text = String.Format("0x{0:x4}", System.Convert.ToInt32(m_oPIC.getStack()[4].ToString()));
            });
            InvokeIfRequired((tbStack5), (MethodInvoker)delegate()
            {
                tbStack5.Text = String.Format("0x{0:x4}", System.Convert.ToInt32(m_oPIC.getStack()[5].ToString()));
            });
            InvokeIfRequired((tbStack6), (MethodInvoker)delegate()
            {
                tbStack6.Text = String.Format("0x{0:x4}", System.Convert.ToInt32(m_oPIC.getStack()[6].ToString()));
            });
            InvokeIfRequired((tbStack7), (MethodInvoker)delegate()
            {
                tbStack7.Text = String.Format("0x{0:x4}", System.Convert.ToInt32(m_oPIC.getStack()[7].ToString()));
            });
            #endregion Stack Update

            #region TRISA Update

            //TrisA Label for RA4
            InvokeIfRequired((lbl_PortRa4), (MethodInvoker)delegate()
            {
                if ((m_oPIC.getSFRMemory()[0x85] & 0x10) == 0x00) lbl_PortRa4.Text = "OUT";
                else lbl_PortRa4.Text = "IN";
            });

            //TrisA Label for RA3
            InvokeIfRequired((lbl_PortRa3), (MethodInvoker)delegate()
            {
                if ((m_oPIC.getSFRMemory()[0x85] & 0x08) == 0x00) lbl_PortRa3.Text = "OUT";
                else lbl_PortRa3.Text = "IN";
            });

            //TrisA Label for RA2
            InvokeIfRequired((lbl_PortRa2), (MethodInvoker)delegate()
            {
                if ((m_oPIC.getSFRMemory()[0x85] & 0x04) == 0x00) lbl_PortRa2.Text = "OUT";
                else lbl_PortRa2.Text = "IN";
            });

            //TrisA Label for RA1
            InvokeIfRequired((lbl_PortRa1), (MethodInvoker)delegate()
            {
                if ((m_oPIC.getSFRMemory()[0x85] & 0x02) == 0x00) lbl_PortRa1.Text = "OUT";
                else lbl_PortRa1.Text = "IN";
            });

            //TrisA Label for RA0
            InvokeIfRequired((lbl_PortRa0), (MethodInvoker)delegate()
            {
                if ((m_oPIC.getSFRMemory()[0x85] & 0x01) == 0x00) lbl_PortRa0.Text = "OUT";
                else lbl_PortRa0.Text = "IN";
            });

            //Checkbox RA0
            InvokeIfRequired((cb_PortRa0), (MethodInvoker)delegate()
            {
                cb_PortRa0.Enabled = true;
                if ((m_oPIC.getSFRMemory()[0x05] & 0x01) == 0x00) cb_PortRa0.Checked = false;
                else cb_PortRa0.Checked = true;

                if (lbl_PortRa0.Text == "OUT")
                    cb_PortRa0.Enabled = false;

                if (m_oHwPort != null)
                {
                    if (m_oHwPort.actuelConnectionState == COM.ConnectionState.CONNECTED)
                        cb_PortRa0.Enabled = false;
                }

            });

            //Checkbox RA1
            InvokeIfRequired((cb_PortRa1), (MethodInvoker)delegate()
            {
                cb_PortRa1.Enabled = true;
                if ((m_oPIC.getSFRMemory()[0x05] & 0x02) == 0x00) cb_PortRa1.Checked = false;
                else cb_PortRa1.Checked = true;

                if (lbl_PortRa1.Text == "OUT")
                    cb_PortRa1.Enabled = false;

                if (m_oHwPort != null)
                {
                    if (m_oHwPort.actuelConnectionState == COM.ConnectionState.CONNECTED)
                        cb_PortRa0.Enabled = false;
                }

            });

            //Checkbox RA2
            InvokeIfRequired((cb_PortRa2), (MethodInvoker)delegate()
            {
                cb_PortRa2.Enabled = true;
                if ((m_oPIC.getSFRMemory()[0x05] & 0x04) == 0x00) cb_PortRa2.Checked = false;
                else cb_PortRa2.Checked = true;

                if (lbl_PortRa2.Text == "OUT")
                    cb_PortRa2.Enabled = false;

                if (m_oHwPort != null)
                {
                    if (m_oHwPort.actuelConnectionState == COM.ConnectionState.CONNECTED)
                        cb_PortRa0.Enabled = false;
                }

            });

            //Checkbox RA3
            InvokeIfRequired((cb_PortRa3), (MethodInvoker)delegate()
            {
                cb_PortRa3.Enabled = true;
                if ((m_oPIC.getSFRMemory()[0x05] & 0x08) == 0x00) cb_PortRa3.Checked = false;
                else cb_PortRa3.Checked = true;

                if (lbl_PortRa3.Text == "OUT")
                    cb_PortRa3.Enabled = false;

                if (m_oHwPort != null)
                {
                    if (m_oHwPort.actuelConnectionState == COM.ConnectionState.CONNECTED)
                        cb_PortRa0.Enabled = false;
                }

            });

            //Checkbox RA4
            InvokeIfRequired((cb_PortRa4), (MethodInvoker)delegate()
            {
                cb_PortRa4.Enabled = true;
                if ((m_oPIC.getSFRMemory()[0x05] & 0x10) == 0x00) cb_PortRa4.Checked = false;
                else cb_PortRa4.Checked = true;

                if (lbl_PortRa4.Text == "OUT")
                    cb_PortRa4.Enabled = false;

                if (m_oHwPort != null)
                {
                    if (m_oHwPort.actuelConnectionState == COM.ConnectionState.CONNECTED)
                        cb_PortRa0.Enabled = false;
                }

            });

            #endregion TRISA

            #region TRISB Update

            //TrisB Label for RB7
            InvokeIfRequired((lbl_PortRb7), (MethodInvoker)delegate()
            {
                if ((m_oPIC.getSFRMemory()[0x86] & 0x80) == 0x00) lbl_PortRb7.Text = "OUT";
                else lbl_PortRb7.Text = "IN";
            });

            //TrisB Label for RB6
            InvokeIfRequired((lbl_PortRb6), (MethodInvoker)delegate()
            {
                if ((m_oPIC.getSFRMemory()[0x86] & 0x40) == 0x00) lbl_PortRb6.Text = "OUT";
                else lbl_PortRb6.Text = "IN";
            });

            //TrisB Label for RB5
            InvokeIfRequired((lbl_PortRb5), (MethodInvoker)delegate()
            {
                if ((m_oPIC.getSFRMemory()[0x86] & 0x20) == 0x00) lbl_PortRb5.Text = "OUT";
                else lbl_PortRb5.Text = "IN";
            });

            //TrisB Label for RB4
            InvokeIfRequired((lbl_PortRb4), (MethodInvoker)delegate()
            {
                if ((m_oPIC.getSFRMemory()[0x86] & 0x10) == 0x00) lbl_PortRb4.Text = "OUT";
                else lbl_PortRb4.Text = "IN";
            });

            //TrisB Label for RB3
            InvokeIfRequired((lbl_PortRb3), (MethodInvoker)delegate()
            {
                if ((m_oPIC.getSFRMemory()[0x86] & 0x08) == 0x00) lbl_PortRb3.Text = "OUT";
                else lbl_PortRb3.Text = "IN";
            });

            //TrisB Label for RB2
            InvokeIfRequired((lbl_PortRb2), (MethodInvoker)delegate()
            {
                if ((m_oPIC.getSFRMemory()[0x86] & 0x04) == 0x00) lbl_PortRb2.Text = "OUT";
                else lbl_PortRb2.Text = "IN";
            });

            //TrisB Label for RB1
            InvokeIfRequired((lbl_PortRb1), (MethodInvoker)delegate()
            {
                if ((m_oPIC.getSFRMemory()[0x86] & 0x02) == 0x00) lbl_PortRb1.Text = "OUT";
                else lbl_PortRb1.Text = "IN";
            });

            //TrisB Label for RB0
            InvokeIfRequired((lbl_PortRb0), (MethodInvoker)delegate()
            {
                if ((m_oPIC.getSFRMemory()[0x86] & 0x01) == 0x00) lbl_PortRb0.Text = "OUT";
                else lbl_PortRb0.Text = "IN";
            });

            //Checkbox RB0
            InvokeIfRequired((cb_PortRb0), (MethodInvoker)delegate()
            {
                cb_PortRb0.Enabled = true;
                if ((m_oPIC.getSFRMemory()[0x06] & 0x01) == 0x00) cb_PortRb0.Checked = false;
                else cb_PortRb0.Checked = true;

                if (lbl_PortRb0.Text == "OUT")
                    cb_PortRb0.Enabled = false;

                if (m_oHwPort != null)
                {
                    if (m_oHwPort.actuelConnectionState == COM.ConnectionState.CONNECTED)
                        cb_PortRa0.Enabled = false;
                }

            });

            //Checkbox RB1
            InvokeIfRequired((cb_PortRb1), (MethodInvoker)delegate()
            {
                cb_PortRb1.Enabled = true;
                if ((m_oPIC.getSFRMemory()[0x06] & 0x02) == 0x00) cb_PortRb1.Checked = false;
                else cb_PortRb1.Checked = true;

                if (lbl_PortRb1.Text == "OUT")
                    cb_PortRb1.Enabled = false;

                if (m_oHwPort != null)
                {
                    if (m_oHwPort.actuelConnectionState == COM.ConnectionState.CONNECTED)
                        cb_PortRa0.Enabled = false;
                }

            });

            //Checkbox RB2
            InvokeIfRequired((cb_PortRb2), (MethodInvoker)delegate()
            {
                cb_PortRb2.Enabled = true;
                if ((m_oPIC.getSFRMemory()[0x06] & 0x04) == 0x00) cb_PortRb2.Checked = false;
                else cb_PortRb2.Checked = true;

                if (lbl_PortRb2.Text == "OUT")
                    cb_PortRb2.Enabled = false;

                if (m_oHwPort != null)
                {
                    if (m_oHwPort.actuelConnectionState == COM.ConnectionState.CONNECTED)
                        cb_PortRa0.Enabled = false;
                }
            });

            //Checkbox RB3
            InvokeIfRequired((cb_PortRb3), (MethodInvoker)delegate()
            {
                cb_PortRb3.Enabled = true;
                if ((m_oPIC.getSFRMemory()[0x06] & 0x08) == 0x00) cb_PortRb3.Checked = false;
                else cb_PortRb3.Checked = true;

                if (lbl_PortRb3.Text == "OUT")
                    cb_PortRb3.Enabled = false;

                if (m_oHwPort != null)
                {
                    if (m_oHwPort.actuelConnectionState == COM.ConnectionState.CONNECTED)
                        cb_PortRa0.Enabled = false;
                }

            });

            //Checkbox RB4
            InvokeIfRequired((cb_PortRb4), (MethodInvoker)delegate()
            {
                cb_PortRb4.Enabled = true;
                if ((m_oPIC.getSFRMemory()[0x06] & 0x10) == 0x00) cb_PortRb4.Checked = false;
                else cb_PortRb4.Checked = true;

                if (lbl_PortRb4.Text == "OUT")
                    cb_PortRb4.Enabled = false;

                if (m_oHwPort != null)
                {
                    if (m_oHwPort.actuelConnectionState == COM.ConnectionState.CONNECTED)
                        cb_PortRa0.Enabled = false;
                }
            });

            //Checkbox RB5
            InvokeIfRequired((cb_PortRb5), (MethodInvoker)delegate()
            {
                cb_PortRb5.Enabled = true;
                if ((m_oPIC.getSFRMemory()[0x06] & 0x20) == 0x00) cb_PortRb5.Checked = false;
                else cb_PortRb5.Checked = true;

                if (lbl_PortRb5.Text == "OUT")
                    cb_PortRb5.Enabled = false;

                if (m_oHwPort != null)
                {
                    if (m_oHwPort.actuelConnectionState == COM.ConnectionState.CONNECTED)
                        cb_PortRa0.Enabled = false;
                }
            });

            //Checkbox RB6
            InvokeIfRequired((cb_PortRb6), (MethodInvoker)delegate()
            {
                cb_PortRb6.Enabled = true;
                if ((m_oPIC.getSFRMemory()[0x06] & 0x40) == 0x00) cb_PortRb6.Checked = false;
                else cb_PortRb6.Checked = true;

                if (lbl_PortRb6.Text == "OUT")
                    cb_PortRb6.Enabled = false;

                if (m_oHwPort != null)
                {
                    if (m_oHwPort.actuelConnectionState == COM.ConnectionState.CONNECTED)
                        cb_PortRa0.Enabled = false;
                }
            });

            //Checkbox RB7
            InvokeIfRequired((cb_PortRb7), (MethodInvoker)delegate()
            {
                cb_PortRb7.Enabled = true;
                if ((m_oPIC.getSFRMemory()[0x06] & 0x80) == 0x00) cb_PortRb7.Checked = false;
                else cb_PortRb7.Checked = true;

                if (lbl_PortRb7.Text == "OUT")
                    cb_PortRb7.Enabled = false;

                if (m_oHwPort != null)
                {
                    if (m_oHwPort.actuelConnectionState == COM.ConnectionState.CONNECTED)
                        cb_PortRa0.Enabled = false;
                }
            });

            #endregion TRISB

            #region COM Schnittstelle Update

            //SerialPort Textbox
            InvokeIfRequired((serialPanel), (MethodInvoker)delegate()
            {
                if (m_tSerialPortThread != null)
                {
                    if (m_oHwPort.actuelConnectionState == COM.ConnectionState.CONNECTED) serialPanel.BackColor = Color.Green;
                    else if (m_oHwPort.actuelConnectionState == COM.ConnectionState.IDLE) serialPanel.BackColor = Color.Yellow;
                    else
                    {
                        serialPanel.BackColor = Color.Red; m_tSerialPortThread.Abort();
                    }
                }
            });

            //"Connect"-Button
            InvokeIfRequired((btn_VerbindeCom), (MethodInvoker)delegate()
            {
                if (m_oHwPort != null)
                {
                    if (m_oHwPort.actuelConnectionState == COM.ConnectionState.ABORTED || m_oHwPort.actuelConnectionState == COM.ConnectionState.IDLE)
                        btn_VerbindeCom.Enabled = true;
                }
            });

            #endregion COM Schnittstelle

        }

        public class NewListView : ListView
        {
            public NewListView()
            {
                this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint, true);
                this.DoubleBuffered = true;
            }
        }
    }
}