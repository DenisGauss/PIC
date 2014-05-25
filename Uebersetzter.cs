using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Globalization;
using System.Windows.Forms;

namespace PIC16F64_Simulator
{
    class Uebersetzter
    {
        #region Variables

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
                   codeLine = parseLine(sLine, lineNr);
                    if (codeLine != null)
                    {
                        BefehlszeilenSatz.Instance.addBefehlszeile(codeLine);
                        lineNr++;
                    }
                }
            }
        }//readFile()
        private Befehlszeile parseLine(String sLine, int lineNr)
        {
            Befehlszeile line = new Befehlszeile();

            string[] myStrings = sLine.Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            //m_iPc + binCode vorhanden 
            if (myStrings.Length > 2 && myStrings[0].Length == 4)
            {
                line.PclAsString = myStrings[0];
                line.Pcl = int.Parse(myStrings[0], NumberStyles.AllowHexSpecifier);
                line.OpCodeAsString = myStrings[1];
                line.OpCode = int.Parse(myStrings[1], NumberStyles.AllowHexSpecifier);
                line.State = "";
                line.LineNr = lineNr;
                for (int i = 3; i < myStrings.Length; i++)
                {
                    if (myStrings[i].StartsWith(";")) break;
                    line.Command += myStrings[i] + " ";
                }

                return line;
            }

            // zeilennummer + state vorhanden (zeilennummer + kommentare ausschließen)
            if (myStrings.Length == 2 && myStrings[1] != ";")
            {
                line.PclAsString = "";
                line.Pcl = 99999;
                line.OpCodeAsString = "";
                line.OpCode = 99999;
                line.State = myStrings[1];
                line.Command = "";
                line.LineNr = lineNr;

                return line;
            }

            return null;
        }//parseLine()
        #endregion
    }
}

    