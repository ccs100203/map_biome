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

    class Parabola
    {
        public Parabola(VoronoiStruct.Point focus, int n, Rectangle regin, int id)
        {
            this.id = id;
            points = new List<PointF>();
            this.k = focus.y;
            this.h = (n + focus.x) / 2.0;
            this.c = -(n - focus.x) / 2.0;
            this.focus = focus;
            this.intersections = new List<Intersection>();
            this.regin = regin;
            if (this.c > 0)
            {
                this.points.Add(new PointF(focus.x, focus.y));
                this.points.Add(new PointF(focus.x, focus.y));
            }
            else if (this.c == 0)
            {
                for (float x = focus.x; x >= 0; --x)
                {
                    this.points.Add(new PointF(x, focus.y));
                }
            }
            else
            {
                for (float y = regin.Top; y <= regin.Bottom; ++y)
                {
                    float x = (float)(Math.Pow(y - this.k, 2) / (4 * this.c) + this.h);
                    this.points.Add(new PointF(x, y));
                }
            }
        }

        public int id;
        public List<PointF> points;
        public VoronoiStruct.Point focus;
        public double k, h, c;
        private List<Intersection> intersections;
        private Rectangle regin;

        public List<Intersection> getIntersections()
        {
            for (int i = 0; i < intersections.Count; i++)
            {
                for (int j = 0; j < intersections[i].points.Length; j++)
                {
                    if (!contain(intersections[i].points[j]) ||
                        !regin.Contains(System.Drawing.Point.Round(intersections[i].points[j])))
                    {
                        if (intersections[i].points.Length == 1)
                        {
                            intersections.RemoveAt(i);
                            --i;
                            break;
                        }
                        else
                        {
                            PointF[] temp = new PointF[1];
                            temp[0] = intersections[i].points[
                                (intersections[i].points.Length - 1 - j)];
                            intersections[i].points = temp;
                            --j;
                        }
                    }
                }
            }
            return intersections;
        }

        private bool contain(PointF point)
        {
            float y1 = (float)Math.Floor(point.Y);
            float y2 = (float)Math.Floor(point.Y) + 1;
            float x1 = (float)(Math.Pow(y1 - this.k, 2) / (4 * this.c) + this.h);
            float x2 = (float)(Math.Pow(y2 - this.k, 2) / (4 * this.c) + this.h);
            if (points.Contains(new PointF(x1, y1)) || points.Contains(new PointF(x2, y2)))
            {
                return true;
            }
            return false;
        }

        public void dealIntersect(Parabola obj)
        {
            var intersections = this.intersect(obj);
            if (intersections != null && intersections.GetLength(0) == 2)
            {
                bool gotErased = false;
                points.RemoveAll((PointF point) =>
                {
                    if (this.focus.x <= obj.focus.x && point.Y > intersections[0].Y && point.Y < intersections[1].Y)
                    {
                        gotErased = true;
                        return true;
                    }
                    else
                        return false;
                });
                if (gotErased)
                {
                    obj.points.RemoveAll((PointF point) =>
                    {
                        if (point.Y < intersections[0].Y || point.Y > intersections[1].Y)
                            return true;
                        else
                            return false;
                    });
                    int[] ids = new int[2];
                    ids[0] = id;
                    ids[1] = obj.id;
                    Intersection temp = new Intersection(intersections[0], intersections[1]);
                    temp.parentID = ids;
                    this.intersections.Add(temp);
                }
            }
        }
        public PointF[] intersect(Parabola obj)
        {
            if (this == obj || this.c > 0 || obj.c > 0)
                return null;
            double A = obj.c - c;
            double B = -2 * (obj.c * k - c * obj.k);
            double C = -(4 * c * obj.c * (obj.h - h) - obj.c * k * k + c * obj.k * obj.k);
            double distance = B * B - 4 * A * C;
            if (A == 0)
            {
                if (B == 0)
                {
                    return null;
                }
                double y = -C / B;
                double x = Math.Pow(y - k, 2) / (4 * c) + h;
                PointF[] s = new PointF[1];
                s[0] = new PointF((float)x, (float)y);
                return s;
            }
            else
            {
                if (distance < 0)
                    return null;
                else if (distance == 0)
                {
                    double y = (-B + Math.Sqrt(distance)) / (2 * A);
                    double x = Math.Pow(y - k, 2) / (4 * c) + h;
                    PointF[] s = new PointF[1];
                    s[0] = new PointF((float)x, (float)y);
                    return s;
                }
                else
                {
                    double y1 = (-B + Math.Sqrt(distance)) / (2 * A);
                    double y2 = (-B - Math.Sqrt(distance)) / (2 * A);
                    double x1 = Math.Pow(y1 - k, 2) / (4 * c) + h;
                    double x2 = Math.Pow(y2 - k, 2) / (4 * c) + h;
                    PointF[] s = new PointF[2];
                    s[0] = new PointF((float)x1, (float)y1);
                    s[1] = new PointF((float)x2, (float)y2);
                    Array.Sort(s, (PointF a, PointF b) =>
                    {
                        float x = a.Y - b.Y;
                        // round away from zero
                        return (int)(Math.Sign(x) * Math.Ceiling(Math.Abs(x)));
                    });
                    return s;
                }
            }
        }
    }

    class Intersection
    {
        public Intersection()
        {
            parentID = new int[2];
            parentID[0] = -1;
            parentID[1] = -1;
            points = null;
        }
        public Intersection(PointF a) :
            this()
        {
            points = new PointF[1];
            points[0] = a;
        }
        public Intersection(PointF a, PointF b) :
            this()
        {
            points = new PointF[2];
            points[0] = a;
            points[1] = b;
        }
        public PointF[] points;
        public int[] parentID;
    }

    struct Line
    {
        public Line(VoronoiStruct.Point a, VoronoiStruct.Point b)
        {
            this.a = a;
            this.b = b;
            isriver = false;
        }
        public Line(int ax, int ay, int bx, int by)
        {
            this.a = new VoronoiStruct.Point(ax, ay);
            this.b = new VoronoiStruct.Point(bx, by);
            isriver = false;
        }
        public VoronoiStruct.Point a, b;
        public bool isriver;
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
