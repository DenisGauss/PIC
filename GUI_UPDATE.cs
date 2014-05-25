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
            for (int i = 0x00; i < 0x0C; i++)
            {
                if (i == 0x07) continue;

                InvokeIfRequired(((TextBox)m_htSFRRegister[i]), (MethodInvoker)delegate()
                {
                    ((TextBox)m_htSFRRegister[i]).Text = String.Format("0x{0:x2}", System.Convert.ToInt32(m_oPIC.getSFRMemory()[i].ToString()));
                });
            }

            //GPR Registers
            for (int i = 0x0C; i < 0x30; i++)
            {
                InvokeIfRequired(((TextBox)m_htGPRRegister[i]), (MethodInvoker)delegate()
                {
                    ((TextBox)m_htGPRRegister[i]).Text = String.Format("0x{0:x2}", System.Convert.ToInt32(m_oPIC.getGPRMemory()[i].ToString()));
                });
            }

            //SFR Registers 0x81 - 0x89
            for (int i = 0x81; i < 0x8A; i++)
            {
                if (i == 0x82 || i == 0x82 || i == 0x83 || i == 0x84 || i == 0x87) continue;

                InvokeIfRequired(((TextBox)m_htSFRRegister[i]), (MethodInvoker)delegate()
                {
                    ((TextBox)m_htSFRRegister[i]).Text = String.Format("0x{0:x2}", System.Convert.ToInt32(m_oPIC.getSFRMemory()[i].ToString()));
                });
            }

            InvokeIfRequired((Label_Duration), (MethodInvoker)delegate()
            {
                Label_Duration.Text = String.Format("{0} µs", System.Convert.ToInt32(m_oPIC.Duration));
            });
            //StatusRegister -> ZeroFlag
            InvokeIfRequired((cbStatusZ), (MethodInvoker)delegate()
            {
                cbStatusZ.Enabled = true;
                cbStatusZ.Checked = m_oPIC.checkZeroFlag();
                cbStatusZ.Enabled = false;
            });

            //StatusRegister -> /PD
            InvokeIfRequired((cbStatusPD), (MethodInvoker)delegate()
            {
                cbStatusPD.Enabled = true;
                cbStatusPD.Checked = m_oPIC.checkPDFlag();
                cbStatusPD.Enabled = false;
            });

            //StatusRegister -> /TO
            InvokeIfRequired((cbStatusTO), (MethodInvoker)delegate()
            {
                cbStatusPD.Enabled = true;
                cbStatusPD.Checked = m_oPIC.checkTOFlag();
                cbStatusPD.Enabled = false;
            });

            //StatusRegister -> CarryFlag
            InvokeIfRequired((cbStatusC), (MethodInvoker)delegate()
            {
                cbStatusC.Enabled = true;
                cbStatusC.Checked = m_oPIC.checkCarryFlag();
                cbStatusC.Enabled = false;
            });

            //StatusRegister -> DCFlag
            InvokeIfRequired((cbStatusDc), (MethodInvoker)delegate()
            {
                cbStatusDc.Enabled = true;
                cbStatusDc.Checked = m_oPIC.checkDCFlag();
                cbStatusDc.Enabled = false;
            });

            //StatusRegister -> RP0
            InvokeIfRequired((cbStatusRp0), (MethodInvoker)delegate()
            {
                cbStatusRp0.Enabled = true;
                cbStatusRp0.Checked = m_oPIC.checkRP0Flag();
                cbStatusRp0.Enabled = false;
            });

            //Bank0
            InvokeIfRequired((lblBank0), (MethodInvoker)delegate()
            {
                if (m_oPIC.checkRP0Flag()) lblBank0.ForeColor = Color.Red;
                else lblBank0.ForeColor = Color.DarkGreen;
            });

            //Bank1
            InvokeIfRequired((lblBank1), (MethodInvoker)delegate()
            {
                if (m_oPIC.checkRP0Flag()) lblBank1.ForeColor = Color.DarkGreen;
                else lblBank1.ForeColor = Color.Red;
            });
            //Stack
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

            //TrisA Label for RA4
            InvokeIfRequired((lblPortRa4), (MethodInvoker)delegate()
            {
                if ((m_oPIC.getSFRMemory()[0x85] & 0x10) == 0x00) lblPortRa4.Text = "OUT";
                else lblPortRa4.Text = "IN";
            });

            //TrisA Label for RA3
            InvokeIfRequired((lblPortRa3), (MethodInvoker)delegate()
            {
                if ((m_oPIC.getSFRMemory()[0x85] & 0x08) == 0x00) lblPortRa3.Text = "OUT";
                else lblPortRa3.Text = "IN";
            });

            //TrisA Label for RA2
            InvokeIfRequired((lblPortRa2), (MethodInvoker)delegate()
            {
                if ((m_oPIC.getSFRMemory()[0x85] & 0x04) == 0x00) lblPortRa2.Text = "OUT";
                else lblPortRa2.Text = "IN";
            });

            //TrisA Label for RA1
            InvokeIfRequired((lblPortRa1), (MethodInvoker)delegate()
            {
                if ((m_oPIC.getSFRMemory()[0x85] & 0x02) == 0x00) lblPortRa1.Text = "OUT";
                else lblPortRa1.Text = "IN";
            });

            //TrisA Label for RA0
            InvokeIfRequired((lblPortRa0), (MethodInvoker)delegate()
            {
                if ((m_oPIC.getSFRMemory()[0x85] & 0x01) == 0x00) lblPortRa0.Text = "OUT";
                else lblPortRa0.Text = "IN";
            });

            //TrisB Label for RB7
            InvokeIfRequired((lblPortRb7), (MethodInvoker)delegate()
            {
                if ((m_oPIC.getSFRMemory()[0x86] & 0x80) == 0x00) lblPortRb7.Text = "OUT";
                else lblPortRb7.Text = "IN";
            });

            //TrisB Label for RB6
            InvokeIfRequired((lblPortRb6), (MethodInvoker)delegate()
            {
                if ((m_oPIC.getSFRMemory()[0x86] & 0x40) == 0x00) lblPortRb6.Text = "OUT";
                else lblPortRb6.Text = "IN";
            });

            //TrisB Label for RB5
            InvokeIfRequired((lblPortRb5), (MethodInvoker)delegate()
            {
                if ((m_oPIC.getSFRMemory()[0x86] & 0x20) == 0x00) lblPortRb5.Text = "OUT";
                else lblPortRb5.Text = "IN";
            });

            //TrisB Label for RB4
            InvokeIfRequired((lblPortRb4), (MethodInvoker)delegate()
            {
                if ((m_oPIC.getSFRMemory()[0x86] & 0x10) == 0x00) lblPortRb4.Text = "OUT";
                else lblPortRb4.Text = "IN";
            });

            //TrisB Label for RB3
            InvokeIfRequired((lblPortRb3), (MethodInvoker)delegate()
            {
                if ((m_oPIC.getSFRMemory()[0x86] & 0x08) == 0x00) lblPortRb3.Text = "OUT";
                else lblPortRb3.Text = "IN";
            });

            //TrisB Label for RB2
            InvokeIfRequired((lblPortRb2), (MethodInvoker)delegate()
            {
                if ((m_oPIC.getSFRMemory()[0x86] & 0x04) == 0x00) lblPortRb2.Text = "OUT";
                else lblPortRb2.Text = "IN";
            });

            //TrisB Label for RB1
            InvokeIfRequired((lblPortRb1), (MethodInvoker)delegate()
            {
                if ((m_oPIC.getSFRMemory()[0x86] & 0x02) == 0x00) lblPortRb1.Text = "OUT";
                else lblPortRb1.Text = "IN";
            });

            //TrisB Label for RB0
            InvokeIfRequired((lblPortRb0), (MethodInvoker)delegate()
            {
                if ((m_oPIC.getSFRMemory()[0x86] & 0x01) == 0x00) lblPortRb0.Text = "OUT";
                else lblPortRb0.Text = "IN";
            });

            //Checkbox RA0
            InvokeIfRequired((cbPortRa0), (MethodInvoker)delegate()
            {
                cbPortRa0.Enabled = true;
                if ((m_oPIC.getSFRMemory()[0x05] & 0x01) == 0x00) cbPortRa0.Checked = false;
                else cbPortRa0.Checked = true;

                if (lblPortRa0.Text == "OUT")
                    cbPortRa0.Enabled = false;

              
            });

            //Checkbox RA1
            InvokeIfRequired((cbPortRa1), (MethodInvoker)delegate()
            {
                cbPortRa1.Enabled = true;
                if ((m_oPIC.getSFRMemory()[0x05] & 0x02) == 0x00) cbPortRa1.Checked = false;
                else cbPortRa1.Checked = true;

                if (lblPortRa1.Text == "OUT")
                    cbPortRa1.Enabled = false;

              
            });

            //Checkbox RA2
            InvokeIfRequired((cbPortRa2), (MethodInvoker)delegate()
            {
                cbPortRa2.Enabled = true;
                if ((m_oPIC.getSFRMemory()[0x05] & 0x04) == 0x00) cbPortRa2.Checked = false;
                else cbPortRa2.Checked = true;

                if (lblPortRa2.Text == "OUT")
                    cbPortRa2.Enabled = false;

                
            });

            //Checkbox RA3
            InvokeIfRequired((cbPortRa3), (MethodInvoker)delegate()
            {
                cbPortRa3.Enabled = true;
                if ((m_oPIC.getSFRMemory()[0x05] & 0x08) == 0x00) cbPortRa3.Checked = false;
                else cbPortRa3.Checked = true;

                if (lblPortRa3.Text == "OUT")
                    cbPortRa3.Enabled = false;

                
            });

            //Checkbox RA4
            InvokeIfRequired((cbPortRa4), (MethodInvoker)delegate()
            {
                cbPortRa4.Enabled = true;
                if ((m_oPIC.getSFRMemory()[0x05] & 0x10) == 0x00) cbPortRa4.Checked = false;
                else cbPortRa4.Checked = true;

                if (lblPortRa4.Text == "OUT")
                    cbPortRa4.Enabled = false;

              
            });

            //Checkbox RB0
            InvokeIfRequired((cbPortRb0), (MethodInvoker)delegate()
            {
                cbPortRb0.Enabled = true;
                if ((m_oPIC.getSFRMemory()[0x06] & 0x01) == 0x00) cbPortRb0.Checked = false;
                else cbPortRb0.Checked = true;

                if (lblPortRb0.Text == "OUT")
                    cbPortRb0.Enabled = false;

                
            });

            //Checkbox RB1
            InvokeIfRequired((cbPortRb1), (MethodInvoker)delegate()
            {
                cbPortRb1.Enabled = true;
                if ((m_oPIC.getSFRMemory()[0x06] & 0x02) == 0x00) cbPortRb1.Checked = false;
                else cbPortRb1.Checked = true;

                if (lblPortRb1.Text == "OUT")
                    cbPortRb1.Enabled = false;

                
            });

            //Checkbox RB2
            InvokeIfRequired((cbPortRb2), (MethodInvoker)delegate()
            {
                cbPortRb2.Enabled = true;
                if ((m_oPIC.getSFRMemory()[0x06] & 0x04) == 0x00) cbPortRb2.Checked = false;
                else cbPortRb2.Checked = true;

                if (lblPortRb2.Text == "OUT")
                    cbPortRb2.Enabled = false;

            });

            //Checkbox RB3
            InvokeIfRequired((cbPortRb3), (MethodInvoker)delegate()
            {
                cbPortRb3.Enabled = true;
                if ((m_oPIC.getSFRMemory()[0x06] & 0x08) == 0x00) cbPortRb3.Checked = false;
                else cbPortRb3.Checked = true;

                if (lblPortRb3.Text == "OUT")
                    cbPortRb3.Enabled = false;

                
            });

            //Checkbox RB4
            InvokeIfRequired((cbPortRb4), (MethodInvoker)delegate()
            {
                cbPortRb4.Enabled = true;
                if ((m_oPIC.getSFRMemory()[0x06] & 0x10) == 0x00) cbPortRb4.Checked = false;
                else cbPortRb4.Checked = true;

                if (lblPortRb4.Text == "OUT")
                    cbPortRb4.Enabled = false;

                
            });

            //Checkbox RB5
            InvokeIfRequired((cbPortRb5), (MethodInvoker)delegate()
            {
                cbPortRb5.Enabled = true;
                if ((m_oPIC.getSFRMemory()[0x06] & 0x20) == 0x00) cbPortRb5.Checked = false;
                else cbPortRb5.Checked = true;

                if (lblPortRb5.Text == "OUT")
                    cbPortRb5.Enabled = false;

                
            });

            //Checkbox RB6
            InvokeIfRequired((cbPortRb6), (MethodInvoker)delegate()
            {
                cbPortRb6.Enabled = true;
                if ((m_oPIC.getSFRMemory()[0x06] & 0x40) == 0x00) cbPortRb6.Checked = false;
                else cbPortRb6.Checked = true;

                if (lblPortRb6.Text == "OUT")
                    cbPortRb6.Enabled = false;

                
            });

            //Checkbox RB7
            InvokeIfRequired((cbPortRb7), (MethodInvoker)delegate()
            {
                cbPortRb7.Enabled = true;
                if ((m_oPIC.getSFRMemory()[0x06] & 0x80) == 0x00) cbPortRb7.Checked = false;
                else cbPortRb7.Checked = true;

                if (lblPortRb7.Text == "OUT")
                    cbPortRb7.Enabled = false;

                
            });

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