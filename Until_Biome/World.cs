using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Until_Biome
{
    class World
    {
        public World()
        {
            unit = new int[800, 800];
        }
        public World(int a, int b)
        {
            unit = new int[a, b];
        }
        public int[,] unit;
    }
}
