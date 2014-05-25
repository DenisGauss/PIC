using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Windows.Forms;


namespace PIC16F64_Simulator
{
    public partial class PIC
    {

        #region Variablen

        private int[] m_aGPRMemory;
        private int[] m_aSFRMemory;
        private int[] m_aStack;

        private int m_iProgramCounter;
        private int m_iCommandCounter;
        private int m_iCachedTrisA;
        private int m_iCachedTrisB;
        private int m_iLatchPortA;
        private int m_iLatchPortB;
        private int m_iDuration;
        private int m_iWRegister;
        private int m_iWatchDogTimer;
        private int m_iPreScaler;
        private int m_iSpeed;
        private int m_iOpCode;
        private int m_iStackPointer;
        private bool m_bStep;

        #endregion

        #region Konstruktor

        public PIC()
        {
            m_bStep = false;
            m_iProgramCounter = 0;
            m_iWRegister = 0;
            m_iCommandCounter = 0;
            m_iDuration = 0;
            m_iSpeed = 250;
            m_iWatchDogTimer = 0;

            //Initialisierung des GPR (size=48, because 0x2F is the highest adress in GPR)
            m_aGPRMemory = new int[48];
            System.Array.Clear(m_aGPRMemory, 0, m_aGPRMemory.Length);

            // Initialisierung des SFR (int size 138, because of the highest Adress in SFR is 89h)
            m_aSFRMemory = new int[138];
            System.Array.Clear(m_aSFRMemory, 0, m_aSFRMemory.Length);
            SFRinitialize();

            // Initialisierung des Stack
            m_aStack = new int[8];
            System.Array.Clear(m_aStack, 0, 8);

        }//PIC()

        #endregion Kosntruktor

        #region Getter/Setter

        public int[] getStack()
        {
            return this.m_aStack;
        }

        public int[] getSFRMemory()
        {
            return this.m_aSFRMemory;
        }

        public int[] getGRPMemory()
        {
            return this.m_aGPRMemory;
        }

        public void setGPRMemoryValue(int iAdress, int iValue)
        {
            m_aGPRMemory[iAdress] = iValue;
        }

        public int WRegisterValue
        {
            get { return m_iWRegister; }
            set { m_iWRegister = value; }
        }

        public int ProgramCounter
        {
            get { return m_iProgramCounter; }
            set
            {
                m_iProgramCounter = value;

                //update PCL (8 low bits of the programcounter) in data structure for SFR
                setSFRMemoryValue(0x02, value & 0xFF);

                // update PCLATH (5 high bits of the programcounter)
                value = value >> 8;
                setSFRMemoryValue(0x0A, value & 0x1F);
            }
        }

        public int cachedTrisA
        {
            get { return m_iCachedTrisA; }
            set { m_iCachedTrisA = value; }
        }

        public int cachedTrisB
        {
            get { return m_iCachedTrisB; }
            set { m_iCachedTrisB = value; }
        }

        public bool Step
        {
            get { return m_bStep; }
            set { m_bStep = value; }
        }

        public int LatchA
        {
            get { return m_iLatchPortA; }
            set { m_iLatchPortA = value; }
        }

        public int LatchB
        {
            get { return m_iLatchPortB; }
            set { m_iLatchPortB = value; }
        }

        public int Speed
        {
            get { return m_iSpeed; }
            set { m_iSpeed = value; }
        }

        public int WatchDog
        {
            get { return m_iWatchDogTimer; }
            set { m_iWatchDogTimer = value; }
        }

        public int CommandCounter
        {
            get { return m_iCommandCounter; }
            set { m_iCommandCounter = value; }
        }

        public int Duration
        {
            get { return m_iDuration; }
            set { m_iDuration = value; }
        }

        public int getWRegister()
        {
            return this.m_iWRegister;
        }

        #endregion

        #region Functions

        #region Allgemeine Funktions

        /// <summary>
        /// returns the Codeline for the next m_sCommand
        /// </summary>
        /// <param name="aCpu"></param>
        /// <returns>returns the Codeline for the next m_sCommand</returns>
        public Befehlszeile getNextBefehlszeile(PIC aCpu)
        {
            foreach (Befehlszeile aLine in BefehlszeilenSatz.Instance.m_BefehlszeilenList)
            {
                if (aCpu.m_iProgramCounter == aLine.Pcl)
                    return aLine;
            }

            return null;
        }

        /// <summary>
        /// checks for adress mirror (SFR set GPR)
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        internal int checkMirrorBankAddress(int addr)
        {
            if (addr == 0x01 && checkRP0Flag()) addr = 0x81; //OPTION_REG
            if (addr == 0x05 && checkRP0Flag()) addr = 0x85; //TRISA
            if (addr == 0x06 && checkRP0Flag()) addr = 0x86; //TRISB
            if (addr == 0x08 && checkRP0Flag()) addr = 0x88; //EECON1
            if (addr == 0x09 && checkRP0Flag()) addr = 0x89; //EECON2
            return addr;
        }

        /// <summary>
        /// checks if indirect adressing is used
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        internal int checkIndirectAddressing(int addr)
        {
            if (addr == 0x00) return getSFRMemory()[0x04];
            return addr;
        }

        /// <summary>
        /// checks the latch logic if a command writes to porta or portb
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public void checkLatchLogic(int addr, int value)
        {
            //PortA
            if (addr == 0x05)
            {
                //write value to write to the LatchA
                LatchA = value;

                //get TrisA value
                int trisA = getRegisterValue(0x85);

                //check which ports are OUTgoing ports, only this ports can be set!
                writeLatchToPort(trisA, addr, LatchA);
            }

            //PortB
            if (addr == 0x06)
            {
                //write value to write to the LatchA
                LatchB = value;

                //get TrisA value
                int trisB = getRegisterValue(0x86);

                //check which ports are OUTgoing ports, only this ports can be set!
                writeLatchToPort(trisB, addr, LatchB);
            }
        }

        /// <summary>
        /// checks if a port is OUT and if yes, writes the Latch to port
        /// </summary>
        /// <param name="tris"></param>
        /// <param name="addr"></param>
        /// <param name="latch"></param>
        public void writeLatchToPort(int tris, int addr, int latch)
        {
            //check tris/port bit0
            if (!getBitAtPosition(tris, 0)) setRegisterValue(addr, setBitAtPosition(getRegisterValue(addr), 0, getBitAtPosition(latch, 0)));

            //check tris/port bit1
            if (!getBitAtPosition(tris, 1)) setRegisterValue(addr, setBitAtPosition(getRegisterValue(addr), 1, getBitAtPosition(latch, 1)));

            //check tris/port bit2
            if (!getBitAtPosition(tris, 2)) setRegisterValue(addr, setBitAtPosition(getRegisterValue(addr), 2, getBitAtPosition(latch, 2)));

            //check tris/port bit3
            if (!getBitAtPosition(tris, 3)) setRegisterValue(addr, setBitAtPosition(getRegisterValue(addr), 3, getBitAtPosition(latch, 3)));

            //check tris/port bit4
            if (!getBitAtPosition(tris, 4)) setRegisterValue(addr, setBitAtPosition(getRegisterValue(addr), 4, getBitAtPosition(latch, 4)));

            //if PortB also check bits 5,6,7
            if (addr == 0x06)
            {
                //check tris/port bit5
                if (!getBitAtPosition(tris, 5)) setRegisterValue(addr, setBitAtPosition(getRegisterValue(addr), 5, getBitAtPosition(latch, 5)));

                //check tris/port bit6
                if (!getBitAtPosition(tris, 6)) setRegisterValue(addr, setBitAtPosition(getRegisterValue(addr), 6, getBitAtPosition(latch, 6)));

                //check tris/port bit7
                if (!getBitAtPosition(tris, 7)) setRegisterValue(addr, setBitAtPosition(getRegisterValue(addr), 7, getBitAtPosition(latch, 7)));
            }
        }

        /// <summary>
        /// sets bit at a specific position of a value and returns this value
        /// </summary>
        /// <param name="regvalue"></param>
        /// <param name="pos"></param>
        /// <param name="bit"></param>
        /// <returns></returns>
        internal int setBitAtPosition(int regvalue, int pos, bool bit)
        {
            int set = 0x00;
            int unset = 0x00;
            if (pos == 0) { set = 0x01; unset = 0xFE; }
            else if (pos == 1) { set = 0x02; unset = 0xFD; }
            else if (pos == 2) { set = 0x04; unset = 0xFB; }
            else if (pos == 3) { set = 0x08; unset = 0xF7; }
            else if (pos == 4) { set = 0x10; unset = 0xEF; }
            else if (pos == 5) { set = 0x20; unset = 0xDF; }
            else if (pos == 6) { set = 0x40; unset = 0xBF; }
            else if (pos == 7) { set = 0x80; unset = 0x7F; }

            if (bit)
            {
                if ((regvalue & set) == 0x00) return regvalue |= set;
                return regvalue;
            }

            if ((regvalue & set) == 0x00) return regvalue;
            return regvalue &= unset;
        }

        /// <summary>
        /// checks if a bit in a value is set and returns true or false
        /// </summary>
        /// <param name="regvalue"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        internal bool getBitAtPosition(int regvalue, int pos)
        {
            if (((regvalue >> pos) & 0x01) == 0x00) return false;
            else return true;
        }

        /// <summary>
        /// get Register value of a SFR unset GPR
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        internal int getRegisterValue(int addr)
        {
            if (addr < 0x80 && addr > 0x0B) return getGRPMemory()[addr];
            else return getSFRMemory()[addr];
        }

        /// <summary>
        /// sets register value of a SFR unset GPR
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="value"></param>
        internal void setRegisterValue(int addr, int value)
        {
            if (addr < 0x80 && addr > 0x0B) setSFRMemoryValue(addr, value);
            else setSFRMemoryValue(addr, value);
        }

        /// <summary>
        /// Checks for an overflow
        /// </summary>
        /// <param name="l_iResult"></param>
        /// <returns></returns>
        internal int checkForOverflow(int value)
        {
            if (value < 0)
            {
                value += 0x100;
            }
            else if (value > 0xFF)
            {
                value -= 0x100;
            }
            return value;
        }

        /// <summary>
        /// increments the prescaler
        /// </summary>
        internal void incPreScaler()
        {
            m_iPreScaler++;

            //prescaler is assigned to tmr0
            if (getBitAtPosition(getRegisterValue(0x81), 3) == false)
            {
                //prescaler max reached? (can be max. 256 if PS2 PS1 and PS0 are set) -> reset prescaler
                if (m_iPreScaler >= (Math.Pow(2, getRegisterValue(0x81) & 0x07) * 2))
                {
                    m_iPreScaler = 0;

                    //increment TMR0
                    int value = checkForOverflow(getRegisterValue(0x01) + 1);
                    setRegisterValue(0x01, value);

                    //check for TMR0 Overflow
                    if (getRegisterValue(0x01) == 0x00)
                    {
                        //interrupt occured, set T0IF Flag
                        setRegisterValue(0x0B, setBitAtPosition(getRegisterValue(0x0B), 2, true));
                    }
                }
            }

            //prescaler is assigned to wdt
            else
            {
                //prescaler max? (can be max. 128 if PS2 PS1 and PS0 are set) -> reset prescaler
                if (m_iPreScaler >= (Math.Pow(2, getRegisterValue(0x81) & 0x07)))
                {
                    //reset and inc Watchdog
                    m_iPreScaler = 0;
                    WatchDog++;
                }
            }

        }

        /// <summary>
        /// increments the watchdog timer
        /// </summary>
        public void incWatchDog()
        {
            //check if prescaler is assigned to the watchdog
            if (getBitAtPosition(getRegisterValue(0x81), 3))
            {
                incPreScaler();
            }
            else
            {
                WatchDog++;
            }

            //18ms are reched -> Watchdog reset!
            if (WatchDog == 18000)
            {
                WatchDog = 0;
                activateWatchDogReset();
            }
        }

        /// <summary>
        /// watchdog reset
        /// </summary>
        internal void activateWatchDogReset()
        {
            ProgramCounter = 0x0000;
            getSFRMemory()[0x02] = 0x00; //PCL

            //Status-Reg
            setPDFlag(); //PD
            unsetTOFlag(); //TO
            setBitAtPosition(getRegisterValue(0x03), 7, false); //IRP
            setBitAtPosition(getRegisterValue(0x03), 6, false); //RP1
            setBitAtPosition(getRegisterValue(0x03), 5, false); //RP0

            getSFRMemory()[0x0A] = 0x00;  //PCLATH
            getSFRMemory()[0x0B] &= 0x01; //INTCON

            getSFRMemory()[0x81] = 0xFF; //OPTION_REG
            getSFRMemory()[0x85] = 0x1F; //TRISA
            getSFRMemory()[0x86] = 0xFF; //TRISB
        }

        /// <summary>
        /// gives the actuel command to decoder, then it will be executed
        /// </summary>
        /// <param name="actuelCodeLine"></param>
        /// <param name="oThread"></param>
        public void executeCommand(Befehlszeile aCodeLine)
        {
            // decodes the opcode set calls the matching function
            decodeOpCode(aCodeLine.OpCode);
        }

        /// <summary>
        /// realises a timeout, the picCPU is sleeping for a time.
        /// </summary>
        public void timeOut(int time)
        {
            System.Threading.Thread.Sleep(time);

        }

        public void decodeOpCode(int opCode)
        {
            m_iOpCode = opCode;
            if ((opCode & 0x3F00) == 0x0700) addwf();
            else if ((opCode & 0x3F00) == 0x0500) andwf();
            else if ((opCode & 0x3F80) == 0x0180) clrf();
            else if ((opCode & 0x3F80) == 0x0100) clrw();
            else if ((opCode & 0x3F00) == 0x0900) comf();
            else if ((opCode & 0x3F00) == 0x0300) decf();
            else if ((opCode & 0x3F00) == 0x0B00) decfsz();
            else if ((opCode & 0x3F00) == 0x0A00) incf();
            else if ((opCode & 0x3F00) == 0x0F00) incfsz();
            else if ((opCode & 0x3F00) == 0x0400) iorwf();
            else if ((opCode & 0x3F00) == 0x0800) movf();
            else if ((opCode & 0x3F80) == 0x0080) movwf();
            else if ((opCode & 0x3F9F) == 0x0000) nop();
            else if ((opCode & 0x3F00) == 0x0D00) rlf();
            else if ((opCode & 0x3F00) == 0x0C00) rrf();
            else if ((opCode & 0x3F00) == 0x0200) subwf();
            else if ((opCode & 0x3F00) == 0x0E00) swapf();
            else if ((opCode & 0x3F00) == 0x0600) xorwf();
            else if ((opCode & 0x3C00) == 0x1000) bcf();
            else if ((opCode & 0x3C00) == 0x1400) bsf();
            else if ((opCode & 0x3C00) == 0x1800) btfsc();
            else if ((opCode & 0x3C00) == 0x1C00) btfss();
            else if ((opCode & 0x3E00) == 0x3E00) addlw();
            else if ((opCode & 0x3F00) == 0x3900) andlw();
            else if ((opCode & 0x3800) == 0x2000) call();
            else if ((opCode & 0x3FFF) == 0x0064) clrwdt();
            else if ((opCode & 0x3800) == 0x2800) go_to();
            else if ((opCode & 0x3F00) == 0x3800) iorlw();
            else if ((opCode & 0x3C00) == 0x3000) movlw();
            else if ((opCode & 0x3FFF) == 0x0009) retfie();
            else if ((opCode & 0x3C00) == 0x3400) retlw();
            else if ((opCode & 0x3FFF) == 0x0008) re_turn();
            else if ((opCode & 0x3FFF) == 0x0063) sleep();
            else if ((opCode & 0x3E00) == 0x3C00) sublw();
            else if ((opCode & 0x3F00) == 0x3A00) xorlw();
        }

        #endregion Allgemeine Funktions

        #region Reset Functions

        /// <summary>
        /// resets all member variables
        /// </summary>
        public void resetPIC()
        {
            m_bStep = false;
            m_iWatchDogTimer = 0;
            m_iProgramCounter = 0;
            m_iPreScaler = 0;
            m_iCommandCounter = 0;
            m_iDuration = 0;

            resetGPRMemory();
            resetSFRMemory();
            resetStack();

        }

        public void resetWRegister()
        {
            m_iWRegister = 0;
        }

        public void resetGPRMemory()
        {
            System.Array.Clear(m_aGPRMemory, 0, m_aGPRMemory.Length);
        }

        public void resetSFRMemory()
        {
            System.Array.Clear(m_aSFRMemory, 0, m_aSFRMemory.Length);
        }

        public void resetStack()
        {
            System.Array.Clear(m_aStack, 0, 8);
        }

        #endregion Reset Funtions

        #region SFRMemory Functions

        public void SFRinitialize()
        {
            System.Array.Clear(m_aSFRMemory, 0, m_aSFRMemory.Length);
            m_aSFRMemory[0x03] = 0x18;
            m_aSFRMemory[0x81] = 0xff;
            m_aSFRMemory[0x85] = 0x1f;
            m_aSFRMemory[0x86] = 0xff;
        }

        /// <summary>
        /// checks if carrybit has to be set or unset
        /// </summary>
        /// <param name="value"></param>
        public void checkForCarrySetOrUnset(int value)
        {
            if (value > 0xFF) setCarry();
            else unsetCarry();
        }

        /// <summary>
        /// checks if carrybit has to be set or unset FOR SUBSTRACTION COMMANDS
        /// </summary>
        /// <param name="value"></param>
        public void checkForCarrySetOrUnsetSubstraction(int value)
        {
            if (value < 0) unsetCarry();
            else setCarry();
        }

        /// <summary>
        /// checks if digit carry has to be set or unset
        /// </summary>
        /// <param name="value"></param>
        public void checkForDcSetOrUnset(int value)
        {
            if (value > 0xF) setDCFlag();
            else unsetDCFlag();
        }

        /// <summary>
        /// checks if digit carry has to be set or unset SUBSTRACTION
        /// </summary>
        /// <param name="value"></param>
        public void checkForDcSetOrUnsetSubstraction(int value)
        {
            if (value < 0x00) unsetDCFlag();
            else setDCFlag();
        }

        /// <summary>
        /// checks if zero has to be set or unset
        /// </summary>
        /// <param name="value"></param>
        public void checkForZeroSetOrUnset(int value)
        {
            if (value == 0) setZero();
            else unsetZero();
        }

        /// <summary>
        /// check if RP0 is set unset not
        /// </summary>
        /// <returns></returns>
        public bool checkRP0Flag()
        {
            if ((m_aSFRMemory[0x03] & 0x20) == 0x00) return false;
            return true;
        }

        /// <summary>
        /// check if PD is set unset not
        /// </summary>
        /// <returns></returns>
        public bool checkPDFlag()
        {
            if ((m_aSFRMemory[0x03] & 0x08) == 0x00) return false;
            return true;
        }

        /// <summary>
        /// check if TO is set unset not
        /// </summary>
        /// <returns></returns>
        public bool checkTOFlag()
        {
            if ((m_aSFRMemory[0x03] & 0x10) == 0x00) return false;
            return true;
        }

        /// <summary>
        /// checks if carrybit is set unset not
        /// </summary>
        /// <returns></returns>
        public bool checkCarryFlag()
        {
            if ((m_aSFRMemory[0x03] & 0x01) == 0x00) return false;
            return true;
        }

        /// <summary>
        /// set value at a register adress
        /// </summary>
        /// <param name="iAdress"></param>
        /// <param name="iValue"></param>
        public void setSFRMemoryValue(int iAdress, int iValue)
        {
            m_aSFRMemory[iAdress] = iValue;
        }

        /// <summary>
        /// checks if zerobit is set unset not
        /// </summary>
        /// <returns></returns>
        public bool checkZeroFlag()
        {
            if ((m_aSFRMemory[0x03] & 0x04) == 0x00) return false;
            return true;
        }

        /// <summary>
        /// checks if DC flag is set unset not
        /// </summary>
        /// <returns></returns>
        public bool checkDCFlag()
        {
            if ((m_aSFRMemory[0x03] & 0x02) == 0x00) return false;
            return true;
        }

        #region SFRMemory Internals

        internal void setDCFlag()
        {
            m_aSFRMemory[0x03] |= 0x02;
        }

        internal void unsetDCFlag()
        {
            m_aSFRMemory[0x03] &= 0xFD;
        }

        internal void setTOFlag()
        {
            m_aSFRMemory[0x03] |= 0x10;
        }

        internal void unsetTOFlag()
        {
            m_aSFRMemory[0x03] &= 0xEF;
        }

        internal void setPDFlag()
        {
            m_aSFRMemory[0x03] |= 0x08;
        }

        internal void unsetPDFlag()
        {
            m_aSFRMemory[0x03] &= 0xF7;
        }

        internal void setRP0Flag()
        {
            m_aSFRMemory[0x03] |= 0x20;
        }

        internal void unsetRP0Flag()
        {
            m_aSFRMemory[0x03] &= 0xDF;
        }

        internal void setCarry()
        {
            m_aSFRMemory[0x03] |= 0x01;
        }

        internal void unsetCarry()
        {
            m_aSFRMemory[0x03] &= 0xFE;
        }

        internal void setZero()
        {
            m_aSFRMemory[0x3] |= 0x04;
        }

        internal void unsetZero()
        {
            m_aSFRMemory[0x3] &= 0xFB;
        }

        #endregion SFRMemory Internals

        #endregion SFRMemory

        #region Stack Functions

        /// <summary>
        /// pushes a new Adress on the Stack
        /// </summary>
        /// <param name="PCLValue"></param>
        public bool pushStack(int PCLValue)
        {
            if (m_iStackPointer > 7)
            {
                MessageBox.Show("PIC-Stacküberlauf: Ausführung wird beendet...", "PIC-Stacküberlauf", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            m_aStack[m_iStackPointer] = PCLValue;
            m_iStackPointer++;
            return true;
        }

        /// <summary>
        /// drops the last Adress from the Stack
        /// </summary>
        /// <returns></returns>
        public int popStack()
        {
            m_iStackPointer--;
            int value = m_aStack[m_iStackPointer];
            m_aStack[m_iStackPointer] = 0x00;
            return value;
        }

        #endregion Stack Functions

        #region Interrupt Functions

        /// <summary>
        /// logic of INT Interrupt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void checkINTInterrupt()
        {
            //check if GIE and INTE and INTf are set
            if (getBitAtPosition(getRegisterValue(0x0B), 7) && getBitAtPosition(getRegisterValue(0x0B), 4) && (getBitAtPosition(getRegisterValue(0x0B), 1)))
            {
                executeInterrupt();
            }

        }

        /// <summary>
        /// checks if INTInterrupt occured and sets the INTF flag if yes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void INTInterrupt()
        {
            //check for rising/falling edge (if INTEDG == RB0)
            if (getBitAtPosition(getRegisterValue(0x81), 6) == getBitAtPosition(getRegisterValue(0x06), 0))
            {
                //the interrupt occured, set the INTF Flag
                setRegisterValue(0x0B, setBitAtPosition(getRegisterValue(0x0B), 1, true));
            }
        }

        /// <summary>
        /// logic of TMR0 Interrupt
        /// </summary>
        public void checkTMR0Interrupt()
        {
            //check if T0CS is not set -> timer mode active
            if (getBitAtPosition(getRegisterValue(0x81), 5) == false)
            {
                //increment TMR0
                int value = checkForOverflow(getRegisterValue(0x01) + 1);
                setRegisterValue(0x01, value);

                //check for TMR0 Overflow
                if (getRegisterValue(0x01) == 0x00)
                {
                    //interrupt occured, set T0IF Flag
                    setRegisterValue(0x0B, setBitAtPosition(getRegisterValue(0x0B), 2, true));
                }
            }

            //check if GIE and T0IE and TOIF are set
            if (getBitAtPosition(getRegisterValue(0x0B), 7) && getBitAtPosition(getRegisterValue(0x0B), 5) && getBitAtPosition(getRegisterValue(0x0B), 2))
            {
                //execute the interrupt
                executeInterrupt();
            }

        }

        /// <summary>
        /// logic of PORT RB Interrupt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void checkPortRBInterrupt()
        {
            //check if RBIF is set
            if (getBitAtPosition(getRegisterValue(0x0B), 0))
            {
                //check if GIE and RBIE are set
                if (getBitAtPosition(getRegisterValue(0x0B), 7) && getBitAtPosition(getRegisterValue(0x0B), 3))
                {
                    //execute the interrupt
                    executeInterrupt();
                }
            }
        }

        /// <summary>
        /// checks if PortRBInterrupt occured and sets RBIF if yes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PortRBInterrupt()
        {
            //the interrupt occured, set the RBIF Flag
            setRegisterValue(0x0B, setBitAtPosition(getRegisterValue(0x0B), 0, true));
        }

        /// <summary>
        /// logic for the PortRA4 Event(T0CKI)
        /// </summary>
        public void PortRA4Interrupt()
        {
            //check if T0CS is set -> RA4 T0CKI active
            if (getBitAtPosition(getRegisterValue(0x81), 5) == true)
            {
                //check for edge
                if (getBitAtPosition(getRegisterValue(0x81), 4) != getBitAtPosition(getRegisterValue(0x05), 4))
                {
                    //check if prescaler is assigned to tmr0
                    if (getBitAtPosition(getRegisterValue(0x81), 3) == false)
                        incPreScaler();

                    //check if prescaler is assigned to watchdog
                    if (getBitAtPosition(getRegisterValue(0x81), 3) == true)
                    {
                        //increment TMR0
                        int value = checkForOverflow(getRegisterValue(0x01) + 1);
                        setRegisterValue(0x01, value);

                        //check for TMR0 Overflow
                        if (getRegisterValue(0x01) == 0x00)
                        {
                            //interrupt occured, set T0IF Flag
                            setRegisterValue(0x0B, setBitAtPosition(getRegisterValue(0x0B), 2, true));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// executes an interrupt
        /// </summary>
        /// 
        public void executeInterrupt()
        {
            //set GIE to 0
            setRegisterValue(0x0B, setBitAtPosition(getRegisterValue(0x0B), 7, false));
            pushStack(ProgramCounter);
            ProgramCounter = 0x04;
        }

        #endregion Interrupt Functions

        #endregion
    }
}
