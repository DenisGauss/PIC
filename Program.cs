﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PIC16F64_Simulator
{
    static class Program
    {
        #region Main

        [STAThread]
        static void Main() //MainFunktion
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GUI()); //GUI Element erzeugen
        }

        #endregion Main
    }
}
