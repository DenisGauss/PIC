using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIC16F64_Simulator
{
    class Uebersetzter
    {

        #region MemberVariables

        private String m_sFilePath;

        #endregion

        #region Functions

        /// <summary>
        /// Bekommt die Datei und übersetzt diese für die Anzeige
        /// </summary>
        public Uebersetzter(String filepath)
        {
            this.m_sFilePath = filepath;
        }

        public void readFile()
        {
            Befehlszeile codeLine = new Befehlszeile();
            StreamReader streamReader = new StreamReader(this.m_sFilePath);
            String sLine = "";
            int lineNr = 0;
            while (sLine != null)
            {
                sLine = streamReader.ReadLine();
                if (sLine != null)
                {
                   // codeLine = parseLine(sLine, lineNr);
                    if (codeLine != null)
                    {
                       // CodeLineSet.Instance.addCodeLine(codeLine);
                        lineNr++;
                    }
                }
            }
        }//readFile()
        #endregion
    }
}

    