using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIC16F64_Simulator
{
    class COM
    {
        #region Variables


        private PIC m_oPIC;                                             //PIC Referenz
        private GUI m_oGUI;                                             //GUI Referenz
        private SerialPort m_oSerialPort;                               //SerialPort Referenz
        private int m_iTimeOut= 400;                               //Timeout Zeit
        private String m_sPortName;                                     //Portname
        public enum ConnectionState { IDLE, CONNECTED, ABORTED, };      //Zustaende als Enum
        private ConnectionState m_eConnectionState;                     

        #endregion

        #region Getter/Setter

        public SerialPort sPort
        {
            get { return this.m_oSerialPort; }
        }

        public ConnectionState actuelConnectionState
        {
            get { return this.m_eConnectionState; }
            set { this.m_eConnectionState = value; }
        }

        #endregion

        #region Functions
        //Konstruktor
        public COM(ref PIC cpu, ref GUI gui, String portName)
        {
            actuelConnectionState = ConnectionState.IDLE;
            m_oPIC = cpu;
            m_oGUI = gui;
            m_sPortName = portName;
            m_oSerialPort = new SerialPort(m_sPortName, 4800, Parity.None, 8, StopBits.One);
        }

        //Schleife fuer die Verbindung
        public void run()
        {
            connect();

            while ((actuelConnectionState == ConnectionState.IDLE) || (actuelConnectionState == ConnectionState.CONNECTED))
            {
                update();
                m_oGUI.GUI_UPDATE();
            }
        }

        //Versuche Verbindung herzustellen
        private void connect()
        {
            try
            {
                m_oSerialPort.Open();
                m_oSerialPort.ReadTimeout= m_iTimeOut;
                actuelConnectionState = ConnectionState.CONNECTED;
            }
            catch
            {
                actuelConnectionState = ConnectionState.ABORTED;
                m_oGUI.GUI_UPDATE();
            }
        }

        //Dekodier und Verschluessel-Funktion aufrufen
        private void update() 
        {
            String temp = sendAndReceive(decodeDataToSend(m_oPIC.getSFR()[0x05], m_oPIC.getSFR()[0x06], m_oPIC.getSFR()[0x85], m_oPIC.getSFR()[0x86]));
            
            if (temp != null) encodeReceivedData(temp);

        }

        //Zu Sendende Daten werden dekodiert
        private char[] decodeDataToSend(int portA, int portB, int trisA, int trisB)
        {
            //Computer zu Platine
            char[] temp =
                {(char)(0x30 + (trisA >> 4)),   //high
                (char)(0x30 + (trisA & 0xF)),   //low
                (char)(0x30 + (portA >> 4)),    //high
                (char)(0x30 + (portA & 0xF)),   //low
                (char)(0x30 + (trisB >> 4)),    //high
                (char)(0x30 + (trisB & 0xF)),   //low
                (char)(0x30 + (portB >> 4)),    //high
                (char)(0x30 + (portB & 0xF)),   //low
                (char)0xD};
            return temp;
        }

            /// <summary>
        //Eingehenden Daten verschluesseln
        private void encodeReceivedData(String data)
        {
            try
            {
                char[] temp = data.ToCharArray();

                //Ueberpruefe PortA auf Aenderungen
                if (m_oPIC.getSFR()[0x05] != ((temp[0] & 0x01) << 4) + (temp[1] & 0x0F))
                {
                    m_oPIC.getSFR()[0x05] = ((temp[0] & 0x01) << 4) + (temp[1] & 0x0F);
                    m_oPIC.PortRA4Interrupt();
                }

                //Ueberpruefe PortB auf Aenderungen
                if (m_oPIC.getSFR()[0x06] != ((temp[2] & 0x0F) << 4) + (temp[3] & 0x0F))
                {
                    m_oPIC.getSFR()[0x06] = ((temp[2] & 0x0F) << 4) + (temp[3] & 0x0F);
                    m_oPIC.INTInterrupt();
                }
            }
            catch 
            {
                actuelConnectionState = ConnectionState.ABORTED;
                m_oGUI.GUI_UPDATE();
            }
        }

        //Sende und Empfange Daten
        private String sendAndReceive(char[] data)
        {
            String temp = null;
            try
            {
                m_oSerialPort.Write(data, 0, 9);
                temp = m_oSerialPort.ReadTo("\r");
                actuelConnectionState = ConnectionState.CONNECTED;
            }
            catch
            {
                actuelConnectionState = ConnectionState.ABORTED;
                m_oGUI.GUI_UPDATE();
            }

            return temp;
        }
        #endregion
    }
}
