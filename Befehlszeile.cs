﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIC16F64_Simulator
{
    public class Befehlszeile
    {
        #region Variablen

        //Befehlszeile in die Bestandteile aufteilen für Übersichtlichkeit
        private String m_sState;    //Sprungadresse als String
        private String m_sOpCode;   //OperationCode als String
        private int m_iOpCode;      // Befehlscode als Int
        private String m_sCommand;  //Befehl als String
        private String m_sPc;       //Programmcounter als String
        private int m_iPc;          //Programmcounter als Int
        private int m_iLineNr;      //Zeilennummer

        #endregion

        #region Setter/Getter

        public int LineNr
        {
            get { return this.m_iLineNr; }
            set { this.m_iLineNr = value; }
        }

        public int OpCode
        {
            get { return this.m_iOpCode; }
            set { this.m_iOpCode = value; }
        }

        public int Pcl 
        {
            get { return this.m_iPc; }
            set { this.m_iPc = value; }
        }

        public String Command
        {
            get { return this.m_sCommand; }
            set { this.m_sCommand = value; }
        }

        public String State
        {
            get { return this.m_sState; }
            set { this.m_sState = value; }
        }

        public String OpCodeAsString
        {
            get { return this.m_sOpCode; }
            set { this.m_sOpCode = value; }
        }

        public String PclAsString
        {
            get { return this.m_sPc; }
            set { this.m_sPc = value; }
        }

        #endregion
    }
}
        