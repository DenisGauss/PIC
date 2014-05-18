using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace picsim
{
    public class Stack
    {
        private int[] m_aStack;

        private int m_iStackPointer;

        /// <summary>
        /// contructor for the Stack
        /// </summary>
        public Stack()
        {
            m_aStack = new int[8];
            System.Array.Clear(m_aStack, 0, 8);
        }

        /// <summary>
        /// getter for the Stack array
        /// </summary>
        /// <returns></returns>
        public int[] getStack()
        {
            return this.m_aStack;
        }

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
    }
}
