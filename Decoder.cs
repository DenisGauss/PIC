using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PIC16F64_Simulator
{
    public partial class PIC
    {
        #region MemberVariables

        private int m_iOpCode;

        #endregion

        #region Functions

        /// <summary>
        /// decodes the m_iOpCode set calls the function
        /// </summary>
        /// <param name="m_iOpCode"></param>
        /// <s></s>

        public void decodeOpCode(int opCode)
        {
            this.m_iOpCode = opCode;
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
        }//decodeOpCode
        #endregion 
    }

}