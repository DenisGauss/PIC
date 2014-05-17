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

        private String filepath;

        #endregion

        #region Functions

        /// <summary>
        /// Bekommt die Datei und übersetzt diese für die Anzeige
        /// </summary>
        public Uebersetzter(String gui_filepath)
        {

        }

        public void readFile()
        {
            Befehlszeile codeLine = new Befehlszeile();
            StreamReader streamReader = new StreamReader(filepath);
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

    