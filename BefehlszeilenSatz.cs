using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIC16F64_Simulator
{
    public class BefehlszeilenSatz
    {

        static readonly BefehlszeilenSatz instance = new BefehlszeilenSatz();

        #region Getter/Setter
        public List<Befehlszeile> m_BefehlszeilenList { get; set; }

        #endregion Getter/Setter

        #region Functions

        /// <summary>
        /// adds a Befehlszeile to the BefehlszeilenSatz
        /// </summary>
        /// <param name="aLine"></param>
        public void addBefehlszeile(Befehlszeile aLine)
        {
            m_BefehlszeilenList.Add(aLine);
        } //addBefehlszeile()

        /// <summary>
        /// returns the next Befehlszeile
        /// </summary>
        /// <param name="m_iPc"></param>
        /// <returns></returns>
        public Befehlszeile getNextBefehlszeile(int pcl)
        {
            foreach (Befehlszeile aLine in m_BefehlszeilenList)
            {
                if (aLine.Pcl == pcl)
                    return aLine;
            }

            return null;
        }//getNextBefehlszeile()

        /// <summary>
        /// Public method that returns the instance of this singleton class
        /// </summary>
        public static BefehlszeilenSatz Instance
        {
            get
            {
                return instance;
            }
        }//Instance

        /// <summary>
        /// Private Constructor
        /// </summary>
        private BefehlszeilenSatz()
        {
            m_BefehlszeilenList = new List<Befehlszeile>();
        }//BefehlszeilenSatz()

        #endregion Functions
    }
}
