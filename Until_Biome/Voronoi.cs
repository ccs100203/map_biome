using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace VoronoiStruct
{
    class Voronoi
    {
        public Voronoi()
        {
            polygons = new List<Polygon>();
        }
        public Voronoi(int width, int height)
        {
            polygons = new List<Polygon>();
            this.width = width;
            this.height = height;
        }
        public int width, height;
        public List<Polygon> polygons;
    }

    enum Biome
    {
        Null = 0,
        Desert = 1,
        Grassland = 2,
        Forest = 4,
        Ocean = 8,
        River = 16,
        Snow = 32,
        Coastline = 64,
        Riverbank = 128,
        Lava = 256,
        Volcano = 512,
    }

    class Polygon
    {
        public Polygon()
        {
            edges = new List<Edge>();
            focus = new Point();
        }
        public Polygon(VoronoiStruct.Point focus)
        {
            edges = new List<Edge>();
            this.focus = focus;
        }
        public Polygon(List<Edge> edges, VoronoiStruct.Point focus)
        {
            this.edges = new List<Edge>();
            this.focus = focus;
        }
        public List<Edge> edges;
        public VoronoiStruct.Point focus;
        public float centerX = -1, centerY = -1;
        public bool isSea = false;
        public float moisture = -1;
        public Biome bio;
        public float elevation = -1;
    }

    class Edge
    {
        public Edge()
        {
            parentID = new int[2];
            parentID[0] = -1;
            parentID[1] = -1;
            line = new Line();
        }
        public Edge(int id1, int id2)
        {
            parentID = new int[2];
            parentID[0] = id1;
            parentID[1] = id2;
            line = new Line();
        }
        public int[] parentID;
        public Line line;
    }

    struct Line
    {
        public Line(VoronoiStruct.Point a, VoronoiStruct.Point b)
        {
            this.a = a;
            this.b = b;
            isRiver = false;
        }
        public Line(int ax, int ay, int bx, int by)
        {
            this.a = new VoronoiStruct.Point(ax, ay);
            this.b = new VoronoiStruct.Point(bx, by);
            isRiver = false;
        }
        public VoronoiStruct.Point a, b;
        public bool isRiver;
    }

    struct Point
    {
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
            elevation = -1;
        }
        public int x, y;
        public float elevation;
    }
}
