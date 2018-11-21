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
            unit = new VoronoiStruct.Biome[800, 800];
        }
        public World(int a, int b)
        {
            unit = new VoronoiStruct.Biome[a, b];
        }
        public VoronoiStruct.Biome[,] unit;
    }
}
