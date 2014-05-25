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
        #region MemberVariables

        //holds a reference to the picCPU
        private PIC m_oPicCPU;

        //holds a reference to the Gui
        private GUI m_oGui;

        //using the existing class SerialPort for I/O-Connection
        private SerialPort m_oSerialPort;

        //timeout value
        private int m_iReadTimeOut = 400;

        //portname
        private String m_sPortName;

        //ConnectionStatus
        public enum ConnectionState { IDLE, CONNECTED, ABORTED, };
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
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="cpu"></param>
        public COM(ref PIC cpu, ref GUI gui, String portName)
        {
            actuelConnectionState = ConnectionState.IDLE;
            m_oPicCPU = cpu;
            m_oGui = gui;
            m_sPortName = portName;
            m_oSerialPort = new SerialPort(m_sPortName, 4800, Parity.None, 8, StopBits.One);
        }

        /// <summary>
        /// loop for serial port communication
        /// </summary>
        public void run()
        {
            connect();

            while ((actuelConnectionState == ConnectionState.IDLE) || (actuelConnectionState == ConnectionState.CONNECTED))
            {
                update();
                m_oGui.GUI_UPDATE();
            }
        }

        /// <summary>
        /// tries to open the connection
        /// </summary>
        private void connect()
        {
            try
            {
                m_oSerialPort.Open();
                m_oSerialPort.ReadTimeout = m_iReadTimeOut;
                actuelConnectionState = ConnectionState.CONNECTED;
            }
            catch
            {
                actuelConnectionState = ConnectionState.ABORTED;
                m_oGui.GUI_UPDATE();
            }
        }

        /// <summary>
        /// calls the decodeDataToSend/encodeReceivedData functions
        /// </summary>
        private void update() 
        {
            String temp = sendAndReceive(decodeDataToSend(m_oPicCPU.getSFRMemory()[0x05], m_oPicCPU.getSFRMemory()[0x06], m_oPicCPU.getSFRMemory()[0x85], m_oPicCPU.getSFRMemory()[0x86]));
            
            if (temp != null) encodeReceivedData(temp);

        }

        /// <summary>
        /// decodes the data for communication with serial port
        /// </summary>
        /// <param name="portA"></param>
        /// <param name="portB"></param>
        /// <param name="trisA"></param>
        /// <param name="trisB"></param>
        /// <returns></returns>
        private char[] decodeDataToSend(int portA, int portB, int trisA, int trisB)
        {
            //Computer to platine
            char[] temp =
                {(char)(0x30 + (trisA >> 4)),//high
                (char)(0x30 + (trisA & 0xF)),//low
                (char)(0x30 + (portA >> 4)),//high
                (char)(0x30 + (portA & 0xF)),//low
                (char)(0x30 + (trisB >> 4)),//high
                (char)(0x30 + (trisB & 0xF)),//low
                (char)(0x30 + (portB >> 4)),//high
                (char)(0x30 + (portB & 0xF)),//low
                (char)0xD};
            return temp;
        }

        /// <summary>
        /// encodes the data received over the serial port
        /// </summary>
        /// <param name="data"></param>
        private void encodeReceivedData(String data)
        {
            try
            {
                char[] temp = data.ToCharArray();

                //check for changes PortA
                if (m_oPicCPU.getSFRMemory()[0x05] != ((temp[0] & 0x01) << 4) + (temp[1] & 0x0F))
                {
                    m_oPicCPU.getSFRMemory()[0x05] = ((temp[0] & 0x01) << 4) + (temp[1] & 0x0F);
                    m_oPicCPU.PortRA4Interrupt();
                }

                //check for changes PortB
                if (m_oPicCPU.getSFRMemory()[0x06] != ((temp[2] & 0x0F) << 4) + (temp[3] & 0x0F))
                {
                    m_oPicCPU.getSFRMemory()[0x06] = ((temp[2] & 0x0F) << 4) + (temp[3] & 0x0F);
                    m_oPicCPU.INTInterrupt();
                }
            }
            catch 
            {
                actuelConnectionState = ConnectionState.ABORTED;
                m_oGui.GUI_UPDATE();
            }
        }

        /// <summary>
        /// tries to send the decoded data to the serial port and to receive data.
        /// </summary>
        /// <param name="data">is returned to the update() function and then sent to encodeReceivedData() for encoding</param>
        /// <returns></returns>
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
                m_oGui.GUI_UPDATE();
            }

            return temp;
        }
        #endregion
    }
}
