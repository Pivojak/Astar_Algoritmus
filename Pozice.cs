using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bludiste
{
    class Pozice
    {
        public int ID { get; private set; }
        /// <summary>
        /// 0 - Sever, 1 - Vlevo (zapad), 2 - Jih, 3 - Vpravo (vychod)
        /// </summary>
        public int SmerNatoceni { get; set; }
        /// <summary>
        /// Int[2] -- [0] - X - radek , [1] - Y - sloupec
        /// </summary>
        public int[] AktPozice { get; set; }
        /// <summary>
        /// Cena cesty vcetne posunu na tuto pozici
        /// </summary>
        public int AktCenaCesty { get; set; }
        /// <summary>
        /// AktCenaCesty + hodnota heuristicke funkce, tedy vzdalenost do cile
        /// </summary>
        public int CelkovaCenaCesty { get; set; }

        public Pozice(int id,int x, int y)
        {
            AktPozice = new int[2];
            AktPozice[0] = x;
            AktPozice[1] = y;
            ID = id;
        }
        // Kontruktor třídy POZICE, do které si uložím její ID, aktuální cenu cesty k tomuto vrcholu, X a Y souřadnici
        // také směr natočení 0 - Sever, 1 - Vlevo (zapad), 2 - Jih, 3 - Vpravo (vychod)
        public Pozice(int id, int aktualniCenaCesty, int x, int y, int smerNatoceni, int celkovaCena)
        {
            AktPozice = new int[2];
            ID = id;
            AktCenaCesty = aktualniCenaCesty;
            AktPozice[0] = x;
            AktPozice[1] = y;
            SmerNatoceni = smerNatoceni;
            CelkovaCenaCesty = celkovaCena;
        }

        /// <summary>
        /// Metoda co posune pozici danym smerem a spocte vse potrebne pro vytvoreni teto nove pozice
        /// </summary>
        /// <param name="smer">0 - Sever, 1 - Vlevo, 2 - Jih, 3 - Vpravo</param>
        /// <returns>[aktualniCenaCesty, X - sloupec, Y - radek, smerNatoceni 0 : 4]</returns>
        public int[] Pohyb(int smer)
        {
            // return int [cenaCesty, aktualniPoziceX, aktualniPoziceY, smerNatoceni]
            // Vrací aktualni cenu tohoto vrocholu + cenu kroku pohybu. Tedy 
            // Posun o jedno pole       +3
            // Otoceni o 90 stupnu      +1
            // Otoceni o 180 stupnu     +2
            // Druha a treti hodnota v poli  -- nova pozice agenta
            // Ctvrta hodnota -- novy smerNatoceni po pohybu

            int[] report = new int[4];
            report[0] = AktCenaCesty;
            int[] pomocna = new int[2];
            // Stanoveni novych souradnic X a Y a ulozeni do reportu
            pomocna = ValidujPozici(smer);
            report[1] = pomocna[0];
            report[2] = pomocna[1];
            report[3] = smer;
            // Pokud se požadovaný směr pohybu = s tím, kterým je agent natočen, tak se přičte +3, protože se nemusel otáčet
            if(smer == SmerNatoceni)
            {
                report[0] += 3;
            }
            // Případ kdy se otočí o 90° -- +1 a posune se +3 == +4 celková cena tohoto kroku
            else if(smer == SmerNatoceni - 1 || smer == SmerNatoceni + 1 || smer == SmerNatoceni + 3 || smer == SmerNatoceni - 3)
            {
                // Otoceni o 90 a pak jeste pohyb +3
                report[0] += 4;
            }
            // Otočení o 180° a posun, tedy +5 
            else if(smer == SmerNatoceni - 2 || smer == SmerNatoceni + 2)
            {
                report[0] += 5;
            }
            // Vrátí pole, které obsahuje vše potřebné o pozici daným směrem, tak aby se mohla ve třídě ASTAR vytvořit
            return report;
        }
        // Metoda, která spočítá souřadnice pro daný směr. Slouží jako pomocná metoda pro metodu POHYB
        private int[] ValidujPozici(int smer)
        {
            // [0] - X .. [1] - Y
            int[] novaPozice = new int[2];
            if(smer == 0)
            {
                novaPozice[0] = AktPozice[0];
                novaPozice[1] = AktPozice[1] - 1;
            }
            else if (smer == 1)
            {
                novaPozice[0] = AktPozice[0] - 1;
                novaPozice[1] = AktPozice[1];
            }
            else if (smer == 2)
            {
                novaPozice[0] = AktPozice[0];
                novaPozice[1] = AktPozice[1] + 1;
            }
            else if (smer == 3)
            {
                novaPozice[0] = AktPozice[0] + 1;
                novaPozice[1] = AktPozice[1];
            }
            return novaPozice;

        }

        public override string ToString()
        {
            return String.Format("|X: {0}\t|Y: {1}\t| {2}\t|{3}", AktPozice[0], AktPozice[1],AktCenaCesty,CelkovaCenaCesty);
        }

    }
}
