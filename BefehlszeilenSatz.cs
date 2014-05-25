using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIC16F64_Simulator
{
    public class BefehlszeilenSatz
    {
        //Objekt vom BefehlszeilenSatz erstellen
        static readonly BefehlszeilenSatz instanz = new BefehlszeilenSatz(); 

        #region Getter/Setter
        //Neue Liste erstellen mit Get/Set
        public List<Befehlszeile> m_BefehlszeilenList { get; set; }

        //Instanz zurueckgeben
        public static BefehlszeilenSatz Instanz
        {
            get
            {
                return instanz;
            }
        }

        #endregion Getter/Setter

        #region Funktionen

        //Befehlszeile zur Befehlszeilen-Liste hinzufuegen
        public void addBefehlszeile(Befehlszeile aLine)
        {
            m_BefehlszeilenList.Add(aLine);
        } 

        //Naechste Befehlszeile in der Liste zurueckgeben
        public Befehlszeile getNextBefehlszeile(int pcl)
        {
            foreach (Befehlszeile aLine in m_BefehlszeilenList)
            {
                if (aLine.Pcl == pcl)
                    return aLine;
            }

            return null;
        }

        //Konstruktor fuer Objekt der Klasse
        private BefehlszeilenSatz()
        {
            m_BefehlszeilenList = new List<Befehlszeile>();
        }

        #endregion Funktionen
    }
}
