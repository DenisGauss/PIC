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