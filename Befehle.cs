using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PIC16F64_Simulator
{
    public partial class PIC
    {
        #region shortcuts
        private void checkInterrupts()
        {
            checkTMR0Interrupt();
            checkPortRBInterrupt();
            checkINTInterrupt();
        }
        private void checkInterrupts_IncCounters()
        {
            Duration++;
            ProgramCounter++;
            CommandCounter++;
            checkInterrupts();
        }
        private void checkInterrupts_doubleTMRO()
        {
            //check two times, because return has two cycles
            checkTMR0Interrupt();
            checkInterrupts();
        }
        
        #endregion
        //* THE ASSEMBLER FUNCTIONS OF THE PIC16C84 ARE IMPLEMENTED HERE * //
        //-----------------------------------------------------------------//

        //implemented
        #region Befehle
        private void xorlw()
        {
            //get literal
            int lit = m_iOpCode & 0xFF;

            //xor literal and w and safe in w
            m_oWRegister.WRegisterValue = lit ^ m_oWRegister.WRegisterValue;

            //check if zero has to be set or unset
            m_oSFRMemory.checkForZeroSetOrUnset(m_oWRegister.WRegisterValue);

            checkInterrupts_IncCounters();
        }

        //implemented
        private void sublw()
        {
            //get literal
            int value = m_iOpCode & 0xFF;

            //check if carry has to be set or unset
            m_oSFRMemory.checkForCarrySetOrUnsetSubstraction(value - m_oWRegister.WRegisterValue);

            //check if digit carry has to be set or unset
            m_oSFRMemory.checkForDcSetOrUnsetSubstraction((value & 0xF) - (m_oWRegister.WRegisterValue & 0xF));

            //check for overflow and get value from adress
            value = checkForOverflow(value - m_oWRegister.WRegisterValue);

            m_oWRegister.WRegisterValue = value;

            //check if zero has to be set or unset
            m_oSFRMemory.checkForZeroSetOrUnset(value);

            checkInterrupts_IncCounters();
        }

        //todo
        private void sleep()
        {
            //set WatchdogTimer to 0
            WatchDog = 0;

            //set /TO Flag
            m_oSFRMemory.setTOFlag();

            //unset /PD Flag
            m_oSFRMemory.unsetPDFlag();

            checkInterrupts_IncCounters();
        }

        //implemented
        private void re_turn()
        {
            CommandCounter++;
            ProgramCounter = m_oStack.popStack();
            Duration += 2;

            checkInterrupts_doubleTMRO();
        }

        //implemented
        private void retlw()
        {
            //set wregister to the literal
            m_oWRegister.WRegisterValue = m_iOpCode & 0xFF;

            //get last pcl on stack
            ProgramCounter = m_oStack.popStack();

            CommandCounter++;
            Duration += 2;

            checkInterrupts_doubleTMRO();
        }

        //implemented
        private void retfie()
        {
            ProgramCounter = m_oStack.popStack();

            //set GIE to 1
            setRegisterValue(0x0B, setBitAtPosition(getRegisterValue(0x0B), 7, true));

            Duration += 2;
            CommandCounter++;

            checkInterrupts_doubleTMRO();
        }

        //implemented
        private void movlw()
        {
            //get literal
            int lit = m_iOpCode & 0x00FF;

            //move literal to w
            m_oWRegister.WRegisterValue = lit;

            checkInterrupts_IncCounters();
        }

        //implemented
        private void iorlw()
        {
            //get literal
            int lit = m_iOpCode & 0xFF;

            //xor literal and w and safe in w
            m_oWRegister.WRegisterValue = lit | m_oWRegister.WRegisterValue;

            //check if zero has to be set or unset
            m_oSFRMemory.checkForZeroSetOrUnset(m_oWRegister.WRegisterValue);

            checkInterrupts_IncCounters();
        }

        //implemented
        private void go_to()
        {
            ProgramCounter = m_iOpCode & 0x07FF;
            Duration += 2;
            CommandCounter++;

            checkInterrupts_doubleTMRO();
        }

        //implemented
        private void clrwdt()
        {
            //set WatchDogTimer to 0
            WatchDog = 0;

            //set TO Flag
            m_oSFRMemory.setTOFlag();

            //set PD Flag
            m_oSFRMemory.setPDFlag();

            checkInterrupts_IncCounters();
        }

        //implemented
        private void call()
        {
            //pcl of next instruction to stack
            m_oStack.pushStack(++ProgramCounter);

            //set pcl to the called pcl
            ProgramCounter = m_iOpCode & 0x7FF;

            CommandCounter++;
            Duration += 2;

            checkInterrupts_doubleTMRO();
        }

        //implemented
        private void andlw()
        {
            //check for overflow and get value from adress
            int value = (m_iOpCode & 0xFF) & m_oWRegister.WRegisterValue;

            m_oWRegister.WRegisterValue = value;

            //check if zero has to be set or unset
            m_oSFRMemory.checkForZeroSetOrUnset(value);

            checkInterrupts_IncCounters();
        }

        //implemented
        private void addlw()
        {
            int value = m_iOpCode & 0xFF;

            //check if carry has to be set or unset
            m_oSFRMemory.checkForCarrySetOrUnset(value + m_oWRegister.WRegisterValue);

            //check if digit carry has to be set or unset
            m_oSFRMemory.checkForDcSetOrUnset((value & 0xF) + (m_oWRegister.WRegisterValue & 0xF));

            //check for overflow and get value from adress
            value = checkForOverflow(value + m_oWRegister.WRegisterValue);

            m_oWRegister.WRegisterValue = value;

            //check if zero has to be set or unset
            m_oSFRMemory.checkForZeroSetOrUnset(value);

            checkInterrupts_IncCounters();
        }

        //implemented
        private void btfss()
        {
            //check indirect adressing
            int addr = checkIndirectAddressing(m_iOpCode & 0x007F);

            //check for mirrored adresses (if bank1 is active)
            addr = checkMirrorBankAddress(addr);

            //get value from adress
            int value = getRegisterValue(addr);

            //get bit to test
            int bit = (m_iOpCode >> 7) & 0x7;

            bool bitIsSet = getBitAtPosition(value, bit);

            if (bitIsSet)
            {
                ProgramCounter += 2;
                Duration += 2;
                CommandCounter++;
                checkInterrupts_doubleTMRO();
            }

            else
            {
                checkInterrupts_IncCounters();
            }
        }

        //implemented
        private void btfsc()
        {
            //check indirect adressing
            int addr = checkIndirectAddressing(m_iOpCode & 0x007F);

            //check for mirrored adresses (if bank1 is active)
            addr = checkMirrorBankAddress(addr);

            //get value from adress
            int value = getRegisterValue(addr);

            //get bit to test
            int bit = (m_iOpCode >> 7) & 0x7;

            bool bitIsSet = getBitAtPosition(value, bit);

            if (bitIsSet)
            {
                checkInterrupts_IncCounters();
            }

            else
            {
                ProgramCounter += 2;
                Duration += 2;
                CommandCounter++;
                checkInterrupts_doubleTMRO();
            }
        }

        //implemented
        private void bsf()
        {
            //check indirect adressing
            int addr = checkIndirectAddressing(m_iOpCode & 0x007F);

            //check for mirrored adresses (if bank1 is active)
            addr = checkMirrorBankAddress(addr);

            //get value from adress
            int value = getRegisterValue(addr);

            //get bit to change
            int bit = (m_iOpCode >> 7) & 0x7;

            //check latch logic
            if (addr == 0x05 || addr == 0x06)
                checkLatchLogic(addr, setBitAtPosition(value, bit, true));
            else
                //change this bit and write back to register
                setRegisterValue(addr, setBitAtPosition(value, bit, true));

            checkInterrupts_IncCounters();
        }

        //implemented
        private void bcf()
        {
            //check indirect adressing
            int addr = checkIndirectAddressing(m_iOpCode & 0x007F);

            //check for mirrored adresses (if bank1 is active)
            addr = checkMirrorBankAddress(addr);

            //get value from adress
            int value = getRegisterValue(addr);

            //get bit to change
            int bit = (m_iOpCode >> 7) & 7;

            //check latch logic
            if (addr == 0x05 || addr == 0x06)
                checkLatchLogic(addr, setBitAtPosition(value, bit, false));
            else
                //change this bit and write back to register
                setRegisterValue(addr, setBitAtPosition(value, bit, false));

            checkInterrupts_IncCounters();
        }

        //implemented
        private void swapf()
        {
            //check indirect adressing
            int addr = checkIndirectAddressing(m_iOpCode & 0x007F);

            //check for mirrored adresses (if bank1 is active)
            addr = checkMirrorBankAddress(addr);

            //get value from adress
            int value = getRegisterValue(addr);

            //get lownibble from value
            int lownibble = (value & 0x0F) << 4;

            //get highnibble from value
            int highnibble = (value & 0xF0) >> 4;

            //swap nibbles 
            value = lownibble | highnibble;

            //check target
            if ((m_iOpCode & 0x80) == 0)
            {
                //target = working register
                m_oWRegister.WRegisterValue = value;
            }
            else if ((m_iOpCode & 0x80) == 128)
            {
                //if target = porta/portb -> check latch
                if (addr == 0x05 || addr == 0x06)
                    checkLatchLogic(addr, value);
                //target = file register
                else
                    setRegisterValue(addr, value);
            }

            m_iDuration += 1;
            m_iCommandCounter++;
            ProgramCounter++;
            checkInterrupts();
        }

        //implemented
        private void rrf()
        {
            //check indirect adressing
            int addr = checkIndirectAddressing(m_iOpCode & 0x007F);

            //check for mirrored adresses (if bank1 is active)
            addr = checkMirrorBankAddress(addr);

            //check if carry is set or not
            bool carryIsSet = m_oSFRMemory.checkCarryFlag();

            //check if bit0 is 1 -> if yes set carry
            if ((getRegisterValue(addr) & 0x01) == 0x01) m_oSFRMemory.setCarry();

            //check for overflow and get value from adress
            int value = getRegisterValue(addr) >> 1;

            //check if carry was set, if yes shift it into value
            if (carryIsSet) value = value | 0x80;

            //check target
            if ((m_iOpCode & 0x80) == 0)
            {
                //target = working register
                m_oWRegister.WRegisterValue = value;
            }
            else if ((m_iOpCode & 0x80) == 128)
            {
                //if target = porta/portb -> check latch
                if (addr == 0x05 || addr == 0x06)
                    checkLatchLogic(addr, value);
                //target = file register
                else
                    setRegisterValue(addr, value);
            }

            checkInterrupts_IncCounters();
        }

        //implemented
        private void xorwf()
        {
            //check indirect adressing
            int addr = checkIndirectAddressing(m_iOpCode & 0x007F);

            //check for mirrored adresses (if bank1 is active)
            addr = checkMirrorBankAddress(addr);

            //check for overflow and get value from adress
            int value = getRegisterValue(addr) ^ m_oWRegister.WRegisterValue;

            //check target
            if ((m_iOpCode & 0x80) == 0)
            {
                //target = working register
                m_oWRegister.WRegisterValue = value;
            }
            else if ((m_iOpCode & 0x80) == 128)
            {
                //if target = porta/portb -> check latch
                if (addr == 0x05 || addr == 0x06)
                    checkLatchLogic(addr, value);
                //target = file register
                else
                    setRegisterValue(addr, value);
            }

            //check if zero has to be set or unset
            m_oSFRMemory.checkForZeroSetOrUnset(value);

            checkInterrupts_IncCounters();
        }

        //implemented
        private void nop()
        {
            checkInterrupts_IncCounters();
        }

        //implemented
        private void subwf()
        {
            //check indirect adressing
            int addr = checkIndirectAddressing(m_iOpCode & 0x007F);

            //check for mirrored adresses (if bank1 is active)
            addr = checkMirrorBankAddress(addr);

            //check if carry has to be set or unset
            m_oSFRMemory.checkForCarrySetOrUnsetSubstraction(getRegisterValue(addr) - m_oWRegister.WRegisterValue);

            //check if digit carry has to be set or unset
            m_oSFRMemory.checkForDcSetOrUnsetSubstraction((getRegisterValue(addr) & 0xF) - (m_oWRegister.WRegisterValue & 0xF));

            //check for overflow and get value from adress
            int value = checkForOverflow(getRegisterValue(addr) - m_oWRegister.WRegisterValue);

            //check target
            if ((m_iOpCode & 0x80) == 0)
            {
                //target = working register
                m_oWRegister.WRegisterValue = value;
            }
            else if ((m_iOpCode & 0x80) == 128)
            {
                //if target = porta/portb -> check latch
                if (addr == 0x05 || addr == 0x06)
                    checkLatchLogic(addr, value);
                //target = file register
                else
                    setRegisterValue(addr, value);
            }

            //check if zero has to be set or unset
            m_oSFRMemory.checkForZeroSetOrUnset(value);

            checkInterrupts_IncCounters();
        }

        //implemented
        private void rlf()
        {
            //check indirect adressing
            int addr = checkIndirectAddressing(m_iOpCode & 0x007F);

            //check for mirrored adresses (if bank1 is active)
            addr = checkMirrorBankAddress(addr);

            bool carryIsSet = m_oSFRMemory.checkCarryFlag();

            //check if carry has to be set or unset
            m_oSFRMemory.checkForCarrySetOrUnset(getRegisterValue(addr) << 1);

            //check for overflow and get value from adress
            int value = checkForOverflow(getRegisterValue(addr) << 1);

            //check if carry was set, if yes shift it into value
            if (carryIsSet) value = value | 0x01;

            //check target
            if ((m_iOpCode & 0x80) == 0)
            {
                //target = working register
                m_oWRegister.WRegisterValue = value;
            }
            else if ((m_iOpCode & 0x80) == 128)
            {
                //if target = porta/portb -> check latch
                if (addr == 0x05 || addr == 0x06)
                    checkLatchLogic(addr, value);
                //target = file register
                else
                    setRegisterValue(addr, value);
            }

            checkInterrupts_IncCounters();
        }

        //implemented
        private void movwf()
        {
            //check indirect adressing
            int addr = checkIndirectAddressing(m_iOpCode & 0x007F);

            //check for mirrored adresses (if bank1 is active)
            addr = checkMirrorBankAddress(addr);

            //set f = wregister
            if (addr == 0x05 || addr == 0x06)
                checkLatchLogic(addr, m_oWRegister.WRegisterValue);
            else
                setRegisterValue(addr, m_oWRegister.WRegisterValue);

            checkInterrupts_IncCounters();
        }

        //implemented
        private void movf()
        {
            //check indirect adressing
            int addr = checkIndirectAddressing(m_iOpCode & 0x007F);

            //check for mirrored adresses (if bank1 is active)
            addr = checkMirrorBankAddress(addr);

            int value = getRegisterValue(addr);

            //check target
            if ((m_iOpCode & 0x80) == 0)
            {
                //target = working register
                m_oWRegister.WRegisterValue = value;
            }
            else if ((m_iOpCode & 0x80) == 128)
            {
                //if target = porta/portb -> check latch
                if (addr == 0x05 || addr == 0x06)
                    checkLatchLogic(addr, value);
                //target = file register
                else
                    setRegisterValue(addr, value);
            }

            m_oSFRMemory.checkForZeroSetOrUnset(value);

            checkINT_IncCounters;
        }

        //implemented
        private void iorwf()
        {
            //check indirect adressing
            int addr = checkIndirectAddressing(m_iOpCode & 0x007F);

            //check for mirrored adresses (if bank1 is active)
            addr = checkMirrorBankAddress(addr);

            //get value from adress
            int value = getRegisterValue(addr) | m_oWRegister.WRegisterValue;

            //check target
            if ((m_iOpCode & 0x80) == 0)
            {
                //target = working register
                m_oWRegister.WRegisterValue = value;
            }
            else if ((m_iOpCode & 0x80) == 128)
            {
                //if target = porta/portb -> check latch
                if (addr == 0x05 || addr == 0x06)
                    checkLatchLogic(addr, value);
                //target = file register
                else
                    setRegisterValue(addr, value);
            }

            //check if zero has to be set or unset
            m_oSFRMemory.checkForZeroSetOrUnset(value);

            checkInterrupts_IncCounters();
        }

        //implemented
        private void incfsz()
        {
            //check indirect adressing
            int addr = checkIndirectAddressing(m_iOpCode & 0x007F);

            //check for mirrored adresses (if bank1 is active)
            addr = checkMirrorBankAddress(addr);

            //check for overflow and get value from adress
            int value = checkForOverflow(getRegisterValue(addr) + 1);

            //check target
            if ((m_iOpCode & 0x80) == 0)
            {
                //target = working register
                m_oWRegister.WRegisterValue = value;
            }
            else if ((m_iOpCode & 0x80) == 128)
            {
                //if target = porta/portb -> check latch
                if (addr == 0x05 || addr == 0x06)
                    checkLatchLogic(addr, value);
                //target = file register
                else
                    setRegisterValue(addr, value);
            }

            //check if value=0
            if (value == 0)
            {
                ProgramCounter += 2; //skip the next command
                Duration += 2;
                checkInterrupts_doubleTMRO();
            }
            else
            {
                ProgramCounter++;
                Duration++;
                checkInterrupts();
            }

            m_iCommandCounter++;
        }

        //implemented
        private void incf()
        {
            //check indirect adressing
            int addr = checkIndirectAddressing(m_iOpCode & 0x007F);

            //check for mirrored adresses (if bank1 is active)
            addr = checkMirrorBankAddress(addr);

            //check for overflow and get value from adress
            int value = checkForOverflow(getRegisterValue(addr) + 1);

            //check target
            if ((m_iOpCode & 0x80) == 0)
            {
                //target = working register
                m_oWRegister.WRegisterValue = value;
            }
            else if ((m_iOpCode & 0x80) == 128)
            {
                //if target = porta/portb -> check latch
                if (addr == 0x05 || addr == 0x06)
                    checkLatchLogic(addr, value);
                //target = file register
                else
                    setRegisterValue(addr, value);
            }

            //check if zero has to be set or unset
            m_oSFRMemory.checkForZeroSetOrUnset(value);

            checkInterrupts_IncCounters();
        }

        //implemented
        private void decfsz()
        {
            //check indirect adressing
            int addr = checkIndirectAddressing(m_iOpCode & 0x007F);

            //check for mirrored adresses (if bank1 is active)
            addr = checkMirrorBankAddress(addr);

            //check for overflow and get value from adress
            int value = checkForOverflow(getRegisterValue(addr) - 1);

            //check target
            if ((m_iOpCode & 0x80) == 0)
            {
                //target = working register
                m_oWRegister.WRegisterValue = value;
            }
            else if ((m_iOpCode & 0x80) == 128)
            {
                //if target = porta/portb -> check latch
                if (addr == 0x05 || addr == 0x06)
                    checkLatchLogic(addr, value);
                //target = file register
                else
                    setRegisterValue(addr, value);
            }

            //check if value=0
            if (value == 0)
            {
                ProgramCounter += 2; //skip the next command
                Duration += 2;
                checkInterrupts_doubleTMRO();
                CommandCounter++;
            }
            else
            {
                checkInterrupts_IncCounters();
            }


        }

        //implemented
        private void addwf()
        {
            //check indirect adressing
            int addr = checkIndirectAddressing(m_iOpCode & 0x007F);

            //check for mirrored adresses (if bank1 is active)
            addr = checkMirrorBankAddress(addr);

            //check if carry has to be set or unset
            m_oSFRMemory.checkForCarrySetOrUnset(getRegisterValue(addr) + m_oWRegister.WRegisterValue);

            //check if digit carry has to be set or unset
            m_oSFRMemory.checkForDcSetOrUnset((getRegisterValue(addr) & 0xF) + (m_oWRegister.WRegisterValue & 0xF));

            //check for overflow and get value from adress
            int value = checkForOverflow(getRegisterValue(addr) + m_oWRegister.WRegisterValue);

            //check target
            if ((m_iOpCode & 0x80) == 0)
            {
                //target = working register
                m_oWRegister.WRegisterValue = value;
            }
            else if ((m_iOpCode & 0x80) == 128)
            {
                //if target = porta/portb -> check latch
                if (addr == 0x05 || addr == 0x06)
                    checkLatchLogic(addr, value);
                //target = file register
                else
                    setRegisterValue(addr, value);
            }

            //check if zero has to be set or unset
            m_oSFRMemory.checkForZeroSetOrUnset(value);

            checkInterrupts_IncCounters();
        }

        //implemented
        private void andwf()
        {
            //check indirect adressing
            int addr = checkIndirectAddressing(m_iOpCode & 0x007F);

            //check for mirrored adresses (if bank1 is active)
            addr = checkMirrorBankAddress(addr);

            //check for overflow and get value from adress
            int value = getRegisterValue(addr) & m_oWRegister.WRegisterValue;

            //check target
            if ((m_iOpCode & 0x80) == 0)
            {
                //target = working register
                m_oWRegister.WRegisterValue = value;
            }
            else if ((m_iOpCode & 0x80) == 128)
            {
                //if target = porta/portb -> check latch
                if (addr == 0x05 || addr == 0x06)
                    checkLatchLogic(addr, value);
                //target = file register
                else
                    setRegisterValue(addr, value);
            }

            //check if zero has to be set or unset
            m_oSFRMemory.checkForZeroSetOrUnset(value);

            checkInterrupts_IncCounters();
        }

        //implemented
        private void clrf()
        {
            //check indirect adressing
            int addr = checkIndirectAddressing(m_iOpCode & 0x007F);

            //check for mirrored adresses (if bank1 is active)
            addr = checkMirrorBankAddress(addr);

            //check Latch Logic
            if (addr == 0x05 || addr == 0x06)
                checkLatchLogic(addr, 0x00);
            else
                setRegisterValue(addr, 0x00);

            //check if zero has to be set or unset
            m_oSFRMemory.checkForZeroSetOrUnset(getRegisterValue(addr));

            checkInterrupts_IncCounters();
        }

        //implemented
        private void clrw()
        {
            m_oWRegister.WRegisterValue = 0x00;

            //check if zero has to be set or unset
            m_oSFRMemory.checkForZeroSetOrUnset(0);

            checkInterrupts_IncCounters();
        }

        //implemented
        private void comf()
        {
            //check indirect adressing
            int addr = checkIndirectAddressing(m_iOpCode & 0x007F);

            //check for mirrored adresses (if bank1 is active)
            addr = checkMirrorBankAddress(addr);

            //get value from adress and complement it
            int value = (~getRegisterValue(addr)) & 0xFF;

            //check target
            if ((m_iOpCode & 0x80) == 0)
            {
                //target = working register
                m_oWRegister.WRegisterValue = value;
            }
            else if ((m_iOpCode & 0x80) == 128)
            {
                //if target = porta/portb -> check latch
                if (addr == 0x05 || addr == 0x06)
                    checkLatchLogic(addr, value);
                //target = file register
                else
                    setRegisterValue(addr, value);
            }

            //check if zero has to be set or unset
            m_oSFRMemory.checkForZeroSetOrUnset(value);

            checkInterrupts_IncCounters();
        }

        //implemented
        private void decf()
        {
            //check indirect adressing
            int addr = checkIndirectAddressing(m_iOpCode & 0x007F);

            //check for mirrored adresses (if bank1 is active)
            addr = checkMirrorBankAddress(addr);

            //check for overflow and get value from adress
            int value = checkForOverflow(getRegisterValue(addr) - 1);

            //check target
            if ((m_iOpCode & 0x80) == 0)
            {
                //target = working register
                m_oWRegister.WRegisterValue = value;
            }
            else if ((m_iOpCode & 0x80) == 128)
            {
                //if target = porta/portb -> check latch
                if (addr == 0x05 || addr == 0x06)
                    checkLatchLogic(addr, value);
                //target = file register
                else
                    setRegisterValue(addr, value);
            }

            //check if zero has to be set or unset
            m_oSFRMemory.checkForZeroSetOrUnset(value);

            checkInterrupts_IncCounters();
        }
        #endregion
    }
}
