using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;

namespace Bludiste
{
    class Astar
    {
        // Proměnná pro uložení obrázku bludiště
        public Bitmap Image { get; private set; }
        // Proměnná třídy POZICE pro uložení souřadnic agenta
        private Pozice agent;
        // Ziskana 2D matice, která obsahuje 0 pro průchody a 1 pro překážky v bludišti
        public int[,] HraciDeska { get; set; }
        // Stanovení cíle pro agenta na souřadnice, které se nachází v pravém dolním rohu
        private int[] Cil = { 31, 31 };
        // Kolekce pro ukládání navštívených pozic
        public List<Pozice> Stack { get; private set;}
        // Expandované stavy, tedy nové vrcholy nalezené při prohledávání prostoru
        public List<Pozice> Next { get; private set; } 
        // Kolekce, která uchováva celkovou cestu agenta k cíli
        //public List<Pozice> AktualniCesta { get; set; }

        // Cislovaci autorita, ktera dava kazdemu vrcholu jedinecne ID
        public int IDVrcholu { get; private set; }

        // Konstruktor třídy ASTAR
        public Astar()
        {
            Image = new Bitmap(@"d:\MAZE1.bmp");
            DefinujHraciPole();
            IDVrcholu = 0;
            //AktualniCesta = new List<Pozice>();
            Stack = new List<Pozice>();
            Next = new List<Pozice>();
            agent = new Pozice(IDVrcholu, 1, 5, 19, 0,1);
            ZobrazHraciPole(agent.AktPozice[0], agent.AktPozice[1]);
            IDVrcholu++;
        }

        // Vypocita heuristickou funkci po dane souradnice
        // -----------------------------------------------
        // Její výpočet je stanoven jako vzdálenost od cíle v X a to samé v Y, výsledkem je poté součet těchto dvou hodnot
        private int HeuristickaFunkce(int x, int y, int AktualniCenaCesty)
        {
            int suma = AktualniCenaCesty;
            if (Cil[0] > x)
                suma += (Cil[0] - x);
            else
                suma += (x - Cil[0]);

            if (Cil[1] > y)
                suma += (Cil[1] - y);
            else
                suma += (y - Cil[1]);
            // *2 - zvýšení hodnoty heuristické funkce, pomocí násobení výsledku. Tak aby měla větší vliv, jelikož posuny mají 
            //      hodnoty jako +3 -- +5.
            suma = suma * 3;
            return suma;
        }
        // Metoda, která kontroluje pozici agenta a zjišťuje, zda se nenachází v cíli
        private bool Vyhra(int X, int Y, int beh)
        {
            if(X == Cil[0] && Y == Cil[1])
            {
                Console.WriteLine("VYHRA ... nasel jsem cil. Cil jsem nalezl za {0}",beh);
                return true;
            }
            else
                return false;
        }

        // Metoda, ve které se uskutečňuje pohyb agenta po hracím poli
        public void PohybujSe(int pocetBehu)
        {
            for(int b = 0; b < pocetBehu; b++)
            {
                // Pomocné pole pro uložení výsledku posunu agenta ve třídě POZICE
                int[] report_pomocna = new int[4];
                // Do tohoto pole si uložím možné směry agenta, které zjišťuji v metodě expandujVrcholy
                int[] expanduj_pomocna = ExpandujVrcholy(agent.AktPozice[0], agent.AktPozice[1]);
                // Směr je nejprve na sever, protože směry v expanduj_pomocna jsou uloženy od severu
                int smer = 0;
                // Projdu všechny směry v expanduj_pomocna
                foreach (int i in expanduj_pomocna)
                {
                    // Pokud je dany smer povoleny, tedy -- 1, tak se vytvori nova POZICE s hodnotami tohoto policka
                    if (i == 1)
                    {
                        // Prozkoumám pozici daným směrem a získám report se vším potřebným pro vytvoření této pozice
                        report_pomocna = agent.Pohyb(smer);
                        // Výpočet heuristické funkce pro danou pozici
                        // Vstupem je X -- Y -- aktuální cena cesty
                        int celkovaCena = HeuristickaFunkce(report_pomocna[1], report_pomocna[2], report_pomocna[0]);
                        // Přidání nalezené pozice do NEXT, tedy budoucích pozicí, kam je možné jít
                        Next.Add(new Pozice(IDVrcholu, report_pomocna[0], report_pomocna[1], report_pomocna[2], report_pomocna[3],celkovaCena));
                        // Zvýším ID, aby další vrchol měl jedičné
                        IDVrcholu++;
                        // Porovnám pozice v NEXT podle celkové ceny cesty SESTUPNE
                        IEnumerable<Pozice> zalozni = Next.OrderBy(a => a.CelkovaCenaCesty);
                        Next = new List<Pozice>();
                        // Porovnanou kolekci si uložím z5 do původní, tedy do NEXT
                        foreach(Pozice p in zalozni)
                        {
                            Next.Add(p);
                        }
                    }
                    // Zvýším proměnnou směr, čímž půjdu na směr posunutý o 90°
                    smer++;
                }
                // Do prozkoumaných pozic ve STACK si uložím současnou pozici agenta, tedy poslední prozkoumanou
                Stack.Add(agent);
                // Vezmu první pozici v NEXT, tedy ta s nejnižší celkovou cenou cesty
                agent = Next[0];
                // Kontrola zda tato nová pozice, která se bude při dalším běhu této metody prozkoumávat není výherní
                if(Vyhra(agent.AktPozice[0], agent.AktPozice[1],b))
                {
                    break;
                }
                // Vykreslím hrací pole, kde pozici agenta znázorňuje prázdná pozice
                ZobrazHraciPole(agent.AktPozice[0], agent.AktPozice[1]);
                // Smažu pozici, kterou jsem si uložil do agenta z NEXT
                Next.RemoveAt(0);

            }

            Console.WriteLine("\n---------------STACK-----------------\n|X      |Y      |AktCena|CelkCena");
            foreach (Pozice p in Stack)
                Console.WriteLine(p);

            Console.WriteLine("\n---------------NEXT-----------------\n|X      |Y      |AktCena|CelkCena");
            foreach (Pozice p in Next)
                Console.WriteLine(p);

            Console.WriteLine("\n---------------AGENT-----------------\n|X      |Y      |AktCena|CelkCena");
            Console.WriteLine(agent);
        }

        /// <summary>
        /// Prozkouma ctyri smery, kterymi lze jit a vrati pole ve kterem jsou povoleny smery. Zkontroluje duplicitu a zda se 
        /// na tomto políčku nenachází překážka, tedy 1
        /// </summary>
        /// <param name="X">Sloupec</param>
        /// <param name="Y">Radek</param>
        /// <returns>Vraci pole s jednotlivymi smery [x x x x] -- 0 NELZE -- 1 LZE jit na tuto pozici</returns>
        private int[] ExpandujVrcholy(int X, int Y)
        {
            // Vracene pole, ktere obsahuje informaci o čtyřech směrech
            int[] vhodneSmery = new int[4];
            // Ctyri pole o dvou pozicich
            List<int[]> novaPozice = new List<int[]>();
            int[] smer = { 0, 1, 2, 3 };
            // Vypočtu čtyři směry, tedy jejich konkrétní souřadnice na základě zadané pozice agenta X,Y
            foreach(int i in smer)
            {
                novaPozice.Add(new int[2]);
                if (i == 0)
                { 
                    novaPozice[0][0] = X;
                    novaPozice[0][1] = Y - 1;
                }
                else if (i == 1)
                {
                    novaPozice[1][0] = X - 1;
                    novaPozice[1][1] = Y;
                }
                else if (i == 2)
                {
                    novaPozice[2][0] = X;
                    novaPozice[2][1] = Y + 1;
                }
                else if (i == 3)
                {
                    novaPozice[3][0] = X + 1;
                    novaPozice[3][1] = Y;
                }
            }
            // [X sloupec, Y radek]
            for (int i = 0; i < 4; i++)
            {
                // Kontrola zda jsem v kladných hodnotách souřadnic
                if(novaPozice[i][0] >= 0 && novaPozice[i][1] >= 0)
                {
                    // Kontrola duplicity
                    if (!Duplicita(novaPozice[i][0], novaPozice[i][1]))
                    {
                        // Kontrola zda tato pozice obsahuje 0, tedy cestou nikoliv 1 jako překážku
                        // Pokud tomu tak je, zapíši si to výstupního pole 1 pro daný směr 
                        if (HraciDeska[novaPozice[i][0], novaPozice[i][1]] == 0)
                            vhodneSmery[i] = 1;

                        // V opačném případě se tímto směrem nachází překážka a zapíšu 0
                        else
                            vhodneSmery[i] = 0;
                    }
                    else
                        vhodneSmery[i] = 0;
                }
                else
                    vhodneSmery[i] = 0;
            }
            return vhodneSmery;
        }

        /// <summary>
        /// Kontrola zda nove souradnice jiz nejsou ulozene v NEXT nebo ve STACK, tedy v budoucich nebo navstivenych
        /// </summary>
        /// <param name="x">Radek</param>
        /// <param name="y">Sloupec</param>
        /// <returns></returns>
        private bool Duplicita(int x, int y)
        {
            // Pomocna prommena, kde si počítám míru shody
            int porovnani = 0;
            bool odpoved = false;
            int[] souradnice_Nove = { x, y };
            // Kontrola pro STACK
            for (int v = 0; v < Stack.Count; v++)
            {
                // Načtu si kontrolované souřadnice ze STACK
                int[] souradnice_Ulozene = Stack[v].AktPozice;

                for (int c = 0; c < 2; c++)
                {
                    // Kontrola zda X souřadnice se rovnají s X uloženými, to samé poté pro y. Porovnání si zvýším
                    if (souradnice_Nove[c] == souradnice_Ulozene[c])
                        porovnani++;
                    else
                    {
                        porovnani = 0;
                        break;
                    }
                }

                // Nove souradnice se rovnaji s nekterymi ulozenymi ve STACK, proto se vyskoci z vrchni FOR a vrati FALSE
                if (porovnani == 2)
                {
                    odpoved = true;
                    break;
                }
            }
            // Kontrola pro NEXT
            for (int u = 0; u < Next.Count; u++)
            {
                int[] souradnice_Ulozene = Next[u].AktPozice;
                for (int c = 0; c < 2; c++)
                {
                    if (souradnice_Nove[c] == souradnice_Ulozene[c])
                        porovnani++;
                    else
                    {
                        porovnani = 0;
                        break;
                    }
                }

                // Nove souradnice se rovnaji s nekterymi ulozenymi v NEXT, proto se vyskoci z vrchni FOR a vrati FALSE
                if (porovnani == 2)
                {
                    odpoved = true;
                    break;
                }
            }

            return odpoved;
        }

        // |--------------------------------------------------------------------------------------------------------------------------------
        // |HRACI POLE - VYTVORENI A VYKRESLENI
        // |--------------------------------------------------------------------------------------------------------------------------------
        private void DefinujHraciPole()
        {
            // Inicializuji si pole a dám mu rozměry podle počtu pixelů v obrázku v jednom a ve druhém směru
            HraciDeska = new int[Image.Width,Image.Height];

            for (int i = 0; i < Image.Height; i++)
            {
                for (int j = 0; j < Image.Width; j++)
                {
                    Color a = Image.GetPixel(j, i);
                    // Cerna
                    if (a.A == 255 && a.R == 0 && a.B == 0 && a.R == 0)
                    {
                        HraciDeska[j, i] = 1;
                    }
                    // Bile pole
                    else if (a.A == 255 && a.R == 255 && a.B == 255 && a.R == 255)
                    {
                        HraciDeska[j, i] = 0;
                    }
                }
            }


        }


        private void ZobrazHraciPole(int x, int y)
        {
            Console.Clear();
            // Radky
            for (int i = 0; i < HraciDeska.GetLength(1); i++)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                // Sloupce
                for (int j = 0; j < HraciDeska.GetLength(0); j++)
                {
                    if (HraciDeska[j, i] == 0)
                        Console.ForegroundColor = ConsoleColor.Black;
                    else
                        Console.ForegroundColor = ConsoleColor.Green;

                    if (j == x && i == y)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.Write("  ");
                        Console.BackgroundColor = ConsoleColor.Black;

                    }
                    else
                        Console.Write("{0} ", HraciDeska[j, i]);
                }
                Console.Write("\n");
            }
            // Vypis obsahu LISTU next
            Console.WriteLine("\n---------------NEXT-----------------\n|X      |Y      |AktCena|CelkCena");
            foreach (Pozice p in Next)
                Console.WriteLine(p);
            Thread.Sleep(400);
        }

    }
}
