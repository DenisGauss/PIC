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

        private int[] m_aGPRMemory;             //GPRMemory als Array declarieren
        private int[] m_aSFRMemory;             //SFRMemory als Array declarieren
        private int[] m_aStack;                 //Stack als Array declarieren

        private int m_iProgramCounter;          
        private int m_iCommandCounter;          
        private int m_iCachedTrisA;             
        private int m_iCachedTrisB;
        private int m_iLatchPortA;
        private int m_iLatchPortB;
        private int m_iDuration;                //Laufzeit
        private int m_iWRegister;
        private int m_iWatchDogTimer;
        private int m_iPreScaler;               //Vorteiler
        private int m_iSpeed;                   //Geschwindigkeit
        private int m_iOpCode;
        private int m_iStackPointer;
        private bool m_bStep;                   //Schrittmodus boolean

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

            //Initialisierung des GPR (Größe=48, weil 0x2F die größte Adresse im GPR ist)
            m_aGPRMemory = new int[48];
            System.Array.Clear(m_aGPRMemory, 0, m_aGPRMemory.Length);

            // Initialisierung des SFR (Größe=138, weil 89h die größte Adresse im SFT ist)
            m_aSFRMemory = new int[138];
            System.Array.Clear(m_aSFRMemory, 0, m_aSFRMemory.Length);
            SFRinitialize();

            // Initialisierung des Stack 8 BIT
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

        public int[] getGPRMemory()
        {
            return this.m_aGPRMemory;
        }

        public void setGPRMemoryValue(int iAdress, int iValue)
        {
            m_aGPRMemory[iAdress] = iValue;
        }

        public void setSFRMemoryValue(int iAdress, int iValue)
        {
            if (iAdress == 6)
            {
            
            }
            m_aSFRMemory[iAdress] = iValue;
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

        internal bool getBitAtPosition(int regvalue, int pos)
        {
            if (((regvalue >> pos) & 0x01) == 0x00) return false;
            else return true;
        }

        internal int getRegisterValue(int addr)
        {
            if (addr < 0x80 && addr > 0x0B) return getGPRMemory()[addr];
            else return getSFRMemory()[addr];
        }

        internal void setRegisterValue(int addr, int value)
        {
            if (addr < 0x80 && addr > 0x0B) setGPRMemoryValue(addr, value);
            else setSFRMemoryValue(addr, value);
        }

        public Befehlszeile getNextBefehlszeile(PIC aCpu)
        {
            foreach (Befehlszeile aLine in BefehlszeilenSatz.Instanz.m_BefehlszeilenList)
            {
                if (aCpu.m_iProgramCounter == aLine.Pcl)
                    return aLine;
            }

            return null;
        }

        #endregion

        #region Functions

        #region Allgemeine Funktionen  

        //überprüft den Wechsel einer Bank
        internal int checkMirrorBankAddress(int addr)
        {
            if (addr == 0x01 && checkRP0Flag()) addr = 0x81; //OPTION_REG
            if (addr == 0x05 && checkRP0Flag()) addr = 0x85; //TRISA
            if (addr == 0x06 && checkRP0Flag()) addr = 0x86; //TRISB
            if (addr == 0x08 && checkRP0Flag()) addr = 0x88; //EECON1
            if (addr == 0x09 && checkRP0Flag()) addr = 0x89; //EECON2
            return addr;
        }

        ///überprüft, ob indirekte Adressierung genutzt wird
        internal int checkIndirectAddressing(int addr)
        {
            if (addr == 0x00) return getSFRMemory()[0x04];
            return addr;
        }

        //überprüft nach Überlauf
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

        //erhöht den Vorteiler
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

        //erhöht den WatchDogTimer
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
                resetWatchDog();
            }
        }

        //übergibt das aktuelle Kommando zum Auführen an den Decoder
        public void executeCommand(Befehlszeile aCodeLine)
        {
            decodeOpCode(aCodeLine.OpCode);
        }

        //löst eine Ruhephase für den PIC aus
        public void timeOut(int time)
        {
            System.Threading.Thread.Sleep(time);

        }

        //entschlüsselt die Befehl anhand seines Bitmusters
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

        #endregion Allgemeine Funktionen

        #region Latchfunktion

        ///überprüft Latchfunktion für ausgehende Ports
        public void checkLatchLogic(int addr, int value)
        {
            //PortA
            if (addr == 0x05)
            {
                //Schreibt Wert in LatchA
                LatchA = value;

                //holt TRISA Wert
                int trisA = getRegisterValue(0x85);

                //Schreibt Latch für ausgehende Ports
                writeLatchToPort(trisA, addr, LatchA);
            }

            //PortB
            if (addr == 0x06)
            {
                //Schreibt Wert in LatchB
                LatchB = value;

                //holt TRISB Wert
                int trisB = getRegisterValue(0x86);

                //Schreibt Latch für ausgehende Ports
                writeLatchToPort(trisB, addr, LatchB);
            }
        }

        //schreibt Latchwerte in Ports für ausgehende Werte
        public void writeLatchToPort(int tris, int addr, int latch)
        {
            //überprüft tris/port bit0
            if (!getBitAtPosition(tris, 0)) setRegisterValue(addr, setBitAtPosition(getRegisterValue(addr), 0, getBitAtPosition(latch, 0)));

            //überprüft tris/port bit1
            if (!getBitAtPosition(tris, 1)) setRegisterValue(addr, setBitAtPosition(getRegisterValue(addr), 1, getBitAtPosition(latch, 1)));

            //überprüft tris/port bit2
            if (!getBitAtPosition(tris, 2)) setRegisterValue(addr, setBitAtPosition(getRegisterValue(addr), 2, getBitAtPosition(latch, 2)));

            //überprüft tris/port bit3
            if (!getBitAtPosition(tris, 3)) setRegisterValue(addr, setBitAtPosition(getRegisterValue(addr), 3, getBitAtPosition(latch, 3)));

            //überprüft tris/port bit4
            if (!getBitAtPosition(tris, 4)) setRegisterValue(addr, setBitAtPosition(getRegisterValue(addr), 4, getBitAtPosition(latch, 4)));

            //für PortB werden auch überprüft bits 5,6,7
            if (addr == 0x06)
            {
                //überprüft tris/port bit5
                if (!getBitAtPosition(tris, 5)) setRegisterValue(addr, setBitAtPosition(getRegisterValue(addr), 5, getBitAtPosition(latch, 5)));

                //überprüft tris/port bit6
                if (!getBitAtPosition(tris, 6)) setRegisterValue(addr, setBitAtPosition(getRegisterValue(addr), 6, getBitAtPosition(latch, 6)));

                //überprüft tris/port bit7
                if (!getBitAtPosition(tris, 7)) setRegisterValue(addr, setBitAtPosition(getRegisterValue(addr), 7, getBitAtPosition(latch, 7)));
            }
        }

        #endregion Latchfunktion

        #region Reset Functionen

        public void resetPIC()
        {
            m_bStep = false;
            m_iWatchDogTimer = 0;
            m_iProgramCounter = 0;
            m_iPreScaler = 0;
            m_iCommandCounter = 0;
            m_iDuration = 0;

            resetSFRMemory();
            resetStack();
            resetWRegister();

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
            SFRinitialize();
            
        }

        public void resetStack()
        {
            System.Array.Clear(m_aStack, 0, m_aStack.Length);
        }

        internal void resetWatchDog()
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

        #endregion Reset Funtionen

        #region SFRMemory Functionen

        //initialisieren des SFR
        public void SFRinitialize()
        {
            System.Array.Clear(m_aSFRMemory, 0, m_aSFRMemory.Length);
            m_aSFRMemory[0x03] = 0x18;
            m_aSFRMemory[0x81] = 0xff;
            m_aSFRMemory[0x85] = 0x1f;
            m_aSFRMemory[0x86] = 0xff;
        }

        //überprüft ob das Carrybit gesetzt werden muss oder nicht
        public void checkForCarrySetOrUnset(int value)
        {
            if (value > 0xFF) setCarry();
            else unsetCarry();
        }

        //überprüft ob das Carrybit gesetzt werden muss oder nicht (nur Substraktionen)
        public void checkForCarrySetOrUnsetSubstraction(int value)
        {
            if (value < 0) unsetCarry();
            else setCarry();
        }

        //überprüft ob das DC gesetzt werden muss oder nicht
        public void checkForDcSetOrUnset(int value)
        {
            if (value > 0xF) setDCFlag();
            else unsetDCFlag();
        }

        //überprüft ob das DC gesetzt werden muss oder nicht (nur Substraktionen)
        public void checkForDcSetOrUnsetSubstraction(int value)
        {
            if (value < 0x00) unsetDCFlag();
            else setDCFlag();
        }

        //überprüft ob das Zerobit gesetzt werden muss oder nicht
        public void checkForZeroSetOrUnset(int value)
        {
            if (value == 0) setZero();
            else unsetZero();
        }

        //überprüft ob das RP0 Flag gesetzt ist
        public bool checkRP0Flag()
        {
            if ((m_aSFRMemory[0x03] & 0x20) == 0x00) return false;
            return true;
        }

        //überprüft ob das PD Flag gesetzt ist
        public bool checkPDFlag()
        {
            if ((m_aSFRMemory[0x03] & 0x08) == 0x00) return false;
            return true;
        }

        //überprüft ob das TO Flag gesetzt ist
        public bool checkTOFlag()
        {
            if ((m_aSFRMemory[0x03] & 0x10) == 0x00) return false;
            return true;
        }

        //überprüft ob das Carry Flag gesetzt ist
        public bool checkCarryFlag()
        {
            if ((m_aSFRMemory[0x03] & 0x01) == 0x00) return false;
            return true;
        }

        //überprüft ob das Zero Flag gesetzt ist
        public bool checkZeroFlag()
        {
            if ((m_aSFRMemory[0x03] & 0x04) == 0x00) return false;
            return true;
        }

        //überprüft ob das DC Flag gesetzt ist
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

        #endregion SFRMemory Functionen

        #region Stack Functions

        //fügt dem Stack eine neue Adresse hinzu
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

        //entfernt die letzte Adresse vom Stack
        public int popStack()
        {
            m_iStackPointer--;
            int value = m_aStack[m_iStackPointer];
            m_aStack[m_iStackPointer] = 0x00;
            return value;
        }

        #endregion Stack Functions

        #region Interrupt Functionen

        //Logik des INT Interruppts
        public void checkINTInterrupt()
        {
            //check if GIE and INTE and INTf are set
            if (getBitAtPosition(getRegisterValue(0x0B), 7) && getBitAtPosition(getRegisterValue(0x0B), 4) && (getBitAtPosition(getRegisterValue(0x0B), 1)))
            {
                executeInterrupt();
            }

        }

        //überprüft ob INTInterrupt stattgefunden hat und setzt INTF FLAG 
        public void INTInterrupt()
        {
            //check for rising/falling edge (if INTEDG == RB0)
            if (getBitAtPosition(getRegisterValue(0x81), 6) == getBitAtPosition(getRegisterValue(0x06), 0))
            {
                //the interrupt occured, set the INTF Flag
                setRegisterValue(0x0B, setBitAtPosition(getRegisterValue(0x0B), 1, true));
            }
        }

        //Logik des TMR0 Interrupts
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

        //Logik des PORT RB Interrupts
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

        //überprüft ob Port RB Interrupt stattgefunden hat und setzt RBIF FLAG
        public void PortRBInterrupt()
        {
            //the interrupt occured, set the RBIF Flag
            setRegisterValue(0x0B, setBitAtPosition(getRegisterValue(0x0B), 0, true));
        }

        //Logik des PORT RA4 Intterrupts(T0CKI)
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

        //führt ein Interrupt aus
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
