using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace Bludiste
{
    class Program
    {
        static void Main(string[] args)
        {
            // Vytvoření nové instance ASTAR
            Astar novaHra = new Astar();
            // Na této instanci zavolám metodu PohybujSe ... 400 X 
            novaHra.PohybujSe(400);

            Console.ReadKey();


        }
    }
}
