using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace PIC16F64_Simulator
{
    class PIC
    {

        #region MemberVariables

        /// <summary>
        /// private member for the program counter
        /// </summary>
        private int m_iProgramCounter;

        /// <summary>
        /// private member for the command counter
        /// </summary>
        private int m_iCommandCounter;

        /// <summary>
        /// true=only one m_bStep   false=all steps
        /// </summary>
        private bool m_bStep;

        /// <summary>
        /// caches the TrisA value after every command
        /// </summary>
        private int m_iCachedTrisA;

        /// <summary>
        /// caches the TrisB value after every command
        /// </summary>
        private int m_iCachedTrisB;

        /// <summary>
        /// private member for the Stack
        /// </summary>
        private Stack m_oStack;

        /// <summary>
        /// private member for the PortA Latch
        /// </summary>
        private int m_iLatchPortA;

        /// <summary>
        /// private member for the PortB Latch
        /// </summary>
        private int m_iLatchPortB;

        /// <summary>
        /// private member for time duration
        /// </summary>
        private int m_iDuration;

        /// <summary>
        /// private member for the GPR Memory
        /// </summary>
        private GPRMemory m_oGPRMemory;

        /// <summary>
        /// private Member for the Working Register
        /// </summary>
        private WRegister m_oWRegister;

        /// <summary>
        /// private member for the SFR Memory
        /// </summary>
        private SFRMemory m_oSFRMemory;

        /// <summary>
        /// private member for the Watchdog
        /// </summary>
        private int m_iWatchDogTimer;

        /// <summary>
        /// private member for the prescaler
        /// </summary>
        private int m_iPreScaler;

        /// <summary>
        /// execution speed, set with the radiobutton
        /// </summary>
        private int m_iSpeed;

        #endregion

        #region Getter/Setter

        public SFRMemory getSFRMemory()
        {
            return this.m_oSFRMemory;
        }

        public GPRMemory getGPRMemory()
        {
            return this.m_oGPRMemory;
        }

        #endregion

        #region Konstruktor

        public PIC()
        {
            m_bStep = false;
            m_iProgramCounter = 0;
            m_iCommandCounter = 0;
            m_iDuration = 0;
            m_iSpeed = 250;
            m_iWatchDogTimer = 0;
            m_oGPRMemory = new GPRMemory();
            m_oWRegister = new WRegister();
            m_oSFRMemory = new SFRMemory();
            m_oStack = new Stack();
           
        }//PicCPU()

        #endregion

        #region Funktionen

        public void resetCPU()
        {
            m_bStep = false;
            m_iWatchDogTimer = 0;
            m_iProgramCounter = 0;
            m_iPreScaler = 0;
            m_iCommandCounter = 0;
            m_iDuration = 0;
            m_oGPRMemory = new GPRMemory();
            m_oWRegister = new WRegister();
            m_oSFRMemory = new SFRMemory();
            m_oStack = new Stack();
        }//resetCPU()

        public Befehlszeile getNextCodeLine(PIC aCpu)
        {
            foreach (CodeLine aLine in BefehlszeilenSatz.Instance.m_CodeLineList)
            {
                if (aCpu.m_iProgramCounter == aLine.Pcl)
                    return aLine;
            }

            return null;
        }//getNextCodeLine()

        #endregion
    }
}
