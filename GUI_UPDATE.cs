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

            //Bank0 Label
            InvokeIfRequired((lblBank0), (MethodInvoker)delegate()
            {
                if (m_oPIC.checkRP0Flag()) lblBank0.ForeColor = Color.Red;
                else lblBank0.ForeColor = Color.DarkGreen;
            });

            //Bank1 Label
            InvokeIfRequired((lblBank1), (MethodInvoker)delegate()
            {
                if (m_oPIC.checkRP0Flag()) lblBank1.ForeColor = Color.DarkGreen;
                else lblBank1.ForeColor = Color.Red;
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