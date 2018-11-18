using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;

namespace Until_Biome
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            redBrush = new SolidBrush(Color.Red);
            yellowBrush = new SolidBrush(Color.Yellow);
            blackpen = new Pen(Color.Black, 4);
            circleSize = new Size(15,15);
            
        }

        Bitmap bmp;
        Graphics g;
        Pen blackpen;
        Brush redBrush, yellowBrush;
        Size circleSize;
        VoronoiStruct.Voronoi vmap = null;
        Random rand = new Random();

        private void readMap(string path, out VoronoiStruct.Voronoi map)
        {
            StreamReader sr = new StreamReader(path);
            string json = sr.ReadToEnd();
            map = JsonConvert.DeserializeObject<VoronoiStruct.Voronoi>(json);
        }

        void drawPoint(Brush brush, VoronoiStruct.Point pos)
        {
            // this func is just a short hand for writting FillEllipse
            Point pos1 = new Point(pos.x - circleSize.Width / 2, pos.y - circleSize.Height / 2);
            g.FillEllipse(brush, new RectangleF(pos1, circleSize));
        }

        void drawLine(Pen pen, VoronoiStruct.Line line)
        {
            drawLine(pen, line.a, line.b);
        }

        void drawLine(Pen pen, VoronoiStruct.Point pos1, VoronoiStruct.Point pos2)
        {
            g.DrawLine(pen, pos1.x, pos1.y, pos2.x, pos2.y);
        }

        void drawVoronoi(VoronoiStruct.Voronoi map)
        {
            if (map == null)
                return;
            bmp = new Bitmap(map.width, map.height);
            g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            pictureBox1.Image = bmp;
            foreach (var item in map.polygons)
            {
                drawPoint(redBrush, item.focus);
            }
            foreach (var item in map.polygons)
            {
                foreach (var item2 in item.edges)
                {
                    drawLine(blackpen, item2.line);
                }
            }
            pictureBox1.Invalidate();
        }

        void giveelevation_assignline(int x, int y, float value)
        {
            foreach(var item in vmap.polygons)
            {
                foreach(var item2 in item.edges)
                {
                    if(item2.line.a.x == x && item2.line.a.y == y && item2.line.a.elevation != 1)
                    {
                        item2.line.a.elevation = value;
                    }
                    if (item2.line.b.x == x && item2.line.b.y == y && item2.line.b.elevation != 1)
                    {
                        item2.line.b.elevation = value;
                    }
                }
            }
        }

        void calculate_polygoncenter(ref VoronoiStruct.Voronoi map)
        {
            float x = 0, y = 0;
            foreach (var item1 in map.polygons)
            {
                foreach (var item in item1.edges)
                {
                    x += (item.line.a.x + item.line.b.x);
                    y += (item.line.a.y + item.line.b.y);
                }
                x /= (2 * item1.edges.Count);
                y /= (2 * item1.edges.Count);
                item1.centerX = x;
                item1.centerY = y;
                System.Diagnostics.Debug.WriteLine("center x:" + x + "  y: " + y);
                x = 0; y = 0;
            }
        }

        void calelevation(ref VoronoiStruct.Voronoi map)
        {
            float x1 = 0, y1 = 0;
            float x2 = 0, y2 = 0;
            foreach (var item1 in map.polygons)
            {
                /*x = Math.Abs(item.centerX - map.polygons[Toppolygon].centerX);
                y = Math.Abs(item.centerY - map.polygons[Toppolygon].centerY);*/
                foreach(var item2 in item1.edges)
                {
                    x1 = 800 - Math.Abs(item2.line.a.x - map.polygons[Toppolygon].centerX);
                    y1 = 800 - Math.Abs(item2.line.a.y - map.polygons[Toppolygon].centerY);
                    x2 = 800 - Math.Abs(item2.line.b.x - map.polygons[Toppolygon].centerX);
                    y2 = 800 - Math.Abs(item2.line.b.y - map.polygons[Toppolygon].centerY);
                    item2.line.a.elevation = (float)(Math.Sqrt(x1 * x1 + y1 * y1));
                    item2.line.b.elevation = (float)(Math.Sqrt(x2 * x2 + y2 * y2));
                    System.Diagnostics.Debug.WriteLine("elevation: " + item2.line.a.elevation + "\t" + item2.line.b.elevation);
                    x1 = 0;y1 = 0;x2 = 0;y2 = 0;
                }
            }
        }

        //List<VoronoiStruct.Point> riversource = new List<VoronoiStruct.Point>(); 
        VoronoiStruct.Point riversource;
        void find_river_source(VoronoiStruct.Polygon polygon)
        {
            int n;
            n = rand.Next(0, polygon.edges.Count);
            riversource = polygon.edges[n].line.a;
        }

        int river_generator(VoronoiStruct.Point pos)
        {
            //float [,]temp;
            List<VoronoiStruct.Point> connect = new List<VoronoiStruct.Point>();
            List<VoronoiStruct.Line> line = new List<VoronoiStruct.Line>();
            float temp = 1000;
            foreach (var item1 in vmap.polygons)
            {
                foreach(var item2 in item1.edges)
                {
                    if (pos.x == item2.line.a.x && pos.y == item2.line.a.y)
                    {
                        connect.Add(item2.line.b);
                        line.Add(item2.line);
                        if(item2.line.b.elevation <= temp)
                        {
                            temp = item2.line.b.elevation;
                        }
                    }
                    if (pos.x == item2.line.b.x && pos.y == item2.line.b.y)
                    {
                        connect.Add(item2.line.a);
                        line.Add(item2.line);
                        if (item2.line.a.elevation <= temp)
                        {
                            temp = item2.line.a.elevation;
                        }
                    }
                }
            }
            if (pos.elevation <= temp)
            {
                return -1;
            }
            int i = 0;
            for (; i < connect.Count; ++i)
            {
                if (connect[i].elevation == temp) break;
            }
            foreach (var item in line)
            {
                if (item.a.elevation == temp || item.b.elevation == temp)
                {
                    foreach (var item1 in vmap.polygons)
                    {
                        foreach (var item2 in item1.edges)
                        {
                            if (item.a.x == item2.line.a.x && item.a.y == item2.line.a.y
                                && item.b.x == item2.line.b.x && item.b.y == item2.line.b.y)
                            {
                                item2.line.isriver = true;
                                return river_generator(connect[i]);
                            }

                        }
                    }
                }
            }
            return 0;

        }

        //int pointsum = 0;
        //int i ,j, n;
        int Toppolygon;
        private void elevationbutton_Click(object sender, EventArgs e)
        {
            if (vmap == null)
            {
                MessageBox.Show("No map");
                return;
            }
            /*i = 0;j = 0;pointsum = 0;
            foreach(var item1 in vmap.polygons)
            {
                foreach(var item2 in vmap.polygons[i].edges)
                {
                    pointsum++;
                    ++j;
                }
                ++i;
            }
            MessageBox.Show(Convert.ToString(pointsum));*/
            int i = 0, n;
            Toppolygon = -1;
            bool beTop;
            while (i < 10000)
            {
                beTop = true;
                n = rand.Next(0, vmap.polygons.Count);
                foreach(var item in vmap.polygons[n].edges)
                {
                    if (item.line.a.x <= 100 || item.line.a.x >= 700) beTop = false;
                    else if (item.line.a.y <= 100 || item.line.a.y >= 700) beTop = false;
                    else if (item.line.b.x <= 100 || item.line.b.x >= 700) beTop = false;
                    else if (item.line.b.y <= 100 || item.line.b.y >= 700) beTop = false;
                }
                if (beTop)
                {
                    Toppolygon = n;
                    System.Diagnostics.Debug.WriteLine("Top is: " + Convert.ToString(n));
                    break;
                }
                ++i;
                if (i == 10000) MessageBox.Show("No find corresponding polygon");
            }
            if (Toppolygon == -1) return;
            foreach(var item in vmap.polygons[Toppolygon].edges)
            {
                giveelevation_assignline(item.line.a.x, item.line.a.y, 1000);
                giveelevation_assignline(item.line.b.x, item.line.b.y, 1000);
            }
            //System.Diagnostics.Debug.WriteLine("Hi");
            calculate_polygoncenter(ref vmap);
            //System.Diagnostics.Debug.WriteLine(vmap.polygons[Toppolygon].centerX + "\t" + vmap.polygons[Toppolygon].centerY);
            calelevation(ref vmap);
            find_river_source(vmap.polygons[Toppolygon]);
            if (river_generator(riversource) == -1) System.Diagnostics.Debug.WriteLine("Hi");
            foreach(var item in vmap.polygons)
            {
                foreach(var item2 in item.edges)
                {
                    //System.Diagnostics.Debug.WriteLine(item2.line.isriver);
                    if (item2.line.isriver)
                    {
                        drawPoint(yellowBrush, item2.line.a);
                        drawPoint(yellowBrush, item2.line.b);
                    }
                }
            }


            pictureBox1.Invalidate();
        }

        private void openfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                readMap(openFileDialog1.FileName, out vmap);
                //if(vmap != null) MessageBox.Show("file input success");
                drawVoronoi(vmap);
            }
        }
    }
}
