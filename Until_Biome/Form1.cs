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
            bluepen = new Pen(Color.SkyBlue, 6);
            greenpen = new Pen(Color.Green, 6);
            circleSize = new Size(15,15);

            snowBrush = new SolidBrush(Color.Gray);
            forestBrush = new SolidBrush(Color.DarkGreen);
            grasslandBrush = new SolidBrush(Color.SpringGreen);
            desertBrush = new SolidBrush(Color.Gold);
            lavaBrush = new SolidBrush(Color.Firebrick);
            volcanoBrush = new SolidBrush(Color.Fuchsia);
            oceanBrush = new SolidBrush(Color.Navy);

            this.Form1_Resize(null, null);
        }

        Bitmap bmp;
        Graphics g;
        Pen blackpen, bluepen, greenpen;
        Brush redBrush, yellowBrush;
        Brush oceanBrush, snowBrush, forestBrush, grasslandBrush, desertBrush, lavaBrush, volcanoBrush;
        Size circleSize;
        VoronoiStruct.Voronoi vmap = null;
        Random rand = new Random();
        World world = new World();


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
            //g.DrawRectangle(new Pen(Color.Orange), new Rectangle(30, 30, 10, 10));
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

        void giveelevation_assignline(int x, int y, float value) //將所有符合的點都配置elevation
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

        void calculate_polygoncenter(ref VoronoiStruct.Voronoi map) //算所有polygon的中心
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
                //System.Diagnostics.Debug.WriteLine("center x:" + x + "  y: " + y);
                x = 0; y = 0;
            }
        }

        void calelevation(ref VoronoiStruct.Voronoi map) //算所有點的elevation
        {
            float x1 = 0, y1 = 0;
            float x2 = 0, y2 = 0;
            foreach (var item1 in map.polygons)
            {
                /*x = Math.Abs(item.centerX - map.polygons[Toppolygon].centerX);
                y = Math.Abs(item.centerY - map.polygons[Toppolygon].centerY);*/
                foreach(var item2 in item1.edges)
                {
                    x1 = map.width - Math.Abs(item2.line.a.x - map.polygons[Toppolygon].centerX);
                    y1 = map.height - Math.Abs(item2.line.a.y - map.polygons[Toppolygon].centerY);
                    x2 = map.width - Math.Abs(item2.line.b.x - map.polygons[Toppolygon].centerX);
                    y2 = map.height - Math.Abs(item2.line.b.y - map.polygons[Toppolygon].centerY);
                    item2.line.a.elevation = (float)(Math.Sqrt(x1 * x1 + y1 * y1));
                    item2.line.b.elevation = (float)(Math.Sqrt(x2 * x2 + y2 * y2));
                    //System.Diagnostics.Debug.WriteLine("elevation: " + item2.line.a.elevation + "\t" + item2.line.b.elevation);
                    x1 = 0;y1 = 0;x2 = 0;y2 = 0;
                }
            }
        }

        List<VoronoiStruct.Point> riversource = new List<VoronoiStruct.Point>();
        int onstate = 0;//用到第幾個源頭
        //VoronoiStruct.Point riversource;
        List<VoronoiStruct.Point> allRiver = new List<VoronoiStruct.Point>();
        void find_river_source(ref VoronoiStruct.Voronoi map) //找出河的源頭
        {
            int n;
            int total = 0;
            bool repeat = false;
            foreach (var item in map.polygons[Toppolygon].edges)
            {
                total++;
            }
            n = rand.Next(0, total);
            foreach (var item in riversource)
            {
                if (map.polygons[Toppolygon].edges[n].line.a.x == item.x && map.polygons[Toppolygon].edges[n].line.a.y == item.y)
                    repeat = true;
            }
            if (!repeat)
            {
                map.polygons[Toppolygon].edges[n].line.isRiver = true;
                riversource.Add(map.polygons[Toppolygon].edges[n].line.a);
                onstate++;
            }
            
        }

        int river_generator(VoronoiStruct.Point pos) //產生河流
        {
            //float [,]temp;
            List<VoronoiStruct.Point> connect = new List<VoronoiStruct.Point>();
            List<VoronoiStruct.Line> line = new List<VoronoiStruct.Line>();
            float temp = 5000;
            //bool repeat = false;
            foreach (var item1 in vmap.polygons)
            {    
                foreach(var item2 in item1.edges)
                {
                    if (pos.x == item2.line.a.x && pos.y == item2.line.a.y)
                    {
                        connect.Add(item2.line.b);
                        line.Add(item2.line);
                        //allRiver.Add(item2.line.b);
                        if (item2.line.b.elevation <= temp)
                        {
                            temp = item2.line.b.elevation;
                        }
                    }
                    if (pos.x == item2.line.b.x && pos.y == item2.line.b.y)
                    {
                        connect.Add(item2.line.a);
                        line.Add(item2.line);
                        //allRiver.Add(item2.line.a);
                        if (item2.line.a.elevation <= temp)
                        {
                            temp = item2.line.a.elevation;
                        }
                    }
                }
                //connect.Clear();
                //line.Clear();
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
                if (item.a.elevation == temp)
                {
                    foreach (var item1 in vmap.polygons)
                    {
                        foreach (var item2 in item1.edges)
                        {
                            if (item.a.x == item2.line.a.x && item.a.y == item2.line.a.y
                                && item.b.x == item2.line.b.x && item.b.y == item2.line.b.y)
                            {
                                item2.line.isRiver = true;
                                allRiver.Add(item.a);
                                return river_generator(connect[i]);
                            }

                        }
                    }
                }
                else if (item.b.elevation == temp)
                {
                    foreach (var item1 in vmap.polygons)
                    {
                        foreach (var item2 in item1.edges)
                        {
                            if (item.a.x == item2.line.a.x && item.a.y == item2.line.a.y
                                && item.b.x == item2.line.b.x && item.b.y == item2.line.b.y)
                            {
                                item2.line.isRiver = true;
                                allRiver.Add(item.b);
                                return river_generator(connect[i]);
                            }

                        }
                    }
                }
            }
            //return river_generator(connect[i]);
            return 0;

        }

        void draw_river(VoronoiStruct.Voronoi map) //畫出河流
        {
            foreach (var item in map.polygons)
            {
                foreach (var item2 in item.edges)
                {
                    if (item2.line.isRiver)
                    {
                        drawLine(bluepen, item2.line);
                    }
                }
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            pictureBox1.Size = new Size(this.Size.Height - 110, this.Size.Height - 110);
            panel1.Left = pictureBox1.Right + 7;
        }

        private void savefileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string json = JsonConvert.SerializeObject(vmap, Formatting.Indented);
                System.IO.File.WriteAllText(saveFileDialog1.FileName, json);
            }
        }

        void find_sea(ref VoronoiStruct.Voronoi map) //找出所有的海
        {
            foreach(var item1 in map.polygons)
            {
                foreach(var item2 in item1.edges)
                {
                    if (item2.line.a.x == 0 || item2.line.a.x == 799) item1.isSea = true;
                    else if (item2.line.a.y == 0 || item2.line.a.y == 799) item1.isSea = true;
                    else if (item2.line.b.x == 0 || item2.line.b.x == 799) item1.isSea = true;
                    else if (item2.line.b.y == 0 || item2.line.b.y == 799) item1.isSea = true;
                }
            }
        }

        void give_moisture(ref VoronoiStruct.Voronoi map) //找出所有polygon的溼度
        {
            float x = 5000, y = 5000;
            foreach(var item1 in map.polygons)
            {
                foreach (var item3 in allRiver)
                {
                    x = (Math.Abs(item1.centerX - item3.x) <= x) ? Math.Abs(item1.centerX - item3.x) : x;
                    y = (Math.Abs(item1.centerY - item3.y) <= y) ? Math.Abs(item1.centerY - item3.y) : y;
                }
                foreach (var item4 in map.polygons)
                {
                    if (item4.isSea)
                    {
                        foreach (var item5 in item4.edges)
                        {
                            x = (Math.Abs(item1.centerX - item5.line.a.x) <= x) ? Math.Abs(item1.centerX - item5.line.a.x) : x;
                            y = (Math.Abs(item1.centerY - item5.line.a.y) <= y) ? Math.Abs(item1.centerY - item5.line.a.y) : y;
                        }
                    }
                }
                //x = (float)Math.Pow(0.95, x);
                //y = (float)Math.Pow(0.95, y);
                item1.moisture = (float)Math.Sqrt(x * x + y * y);
                item1.moisture = (float)Math.Pow(0.9, item1.moisture);
                //System.Diagnostics.Debug.WriteLine("Moisture: " + item1.moisture);
                x = 2000; y = 2000;
            }


            
        }

        private void saveCastResultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(saveFileDialog2.ShowDialog() == DialogResult.OK)
            {
                string json = JsonConvert.SerializeObject(world, Formatting.Indented);
                System.IO.File.WriteAllText(saveFileDialog2.FileName, json);
            }
        }

        void elevation_for_polygon(ref VoronoiStruct.Voronoi map) //算出所有polygon的elevation
        {
            float ele = 0;
            foreach (var item in map.polygons)
            {
                foreach (var item1 in item.edges)
                {
                    ele += item1.line.a.elevation;
                    ele += item1.line.b.elevation;
                }
                ele /= (2 * item.edges.Count);
                item.elevation = ele;
                //System.Diagnostics.Debug.WriteLine("elevation of polygon: " + ele);
                ele = 0;
            }
        }

        void ratio_elevation_of_polygon(ref VoronoiStruct.Voronoi map) //將所有polygon的elevation轉為0~1之間
        {
            float max = 0, temp = 0;
            foreach(var item in map.polygons)
            {
                max = (item.elevation >= max) ? item.elevation : max;
            }
            foreach (var item in map.polygons)
            {
                temp = (float)Math.Pow((item.elevation / max), 3);
                item.elevation = temp;
                //System.Diagnostics.Debug.WriteLine("Ratio elevation: " + item.elevation);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
            allRiver.Clear();
            riversource.Clear();
        }

        void decide_biome(ref VoronoiStruct.Voronoi map) //決定出生態系
        {
            foreach(var item in map.polygons)
            {
                if (item.elevation <= 0.3 && item.moisture <= 0.35) item.bio = VoronoiStruct.Biome.Lava;
                else if (item.elevation >= 0.3 && item.elevation <= 0.8 && item.moisture <= 0.35) item.bio = VoronoiStruct.Biome.Desert;
                else if (item.elevation >= 0.8 && item.moisture <= 0.35) item.bio = VoronoiStruct.Biome.Volcano;

                else if (item.elevation <= 0.3 && item.moisture >= 0.35 && item.moisture <= 0.55) item.bio = VoronoiStruct.Biome.Desert;
                else if (item.elevation >= 0.3 && item.elevation <= 0.8 && item.moisture >= 0.35 && item.moisture <= 0.55) item.bio = VoronoiStruct.Biome.Grassland;
                else if (item.elevation >= 0.8 && item.moisture >= 0.35 && item.moisture <= 0.55) item.bio = VoronoiStruct.Biome.Grassland;

                else if (item.elevation <= 0.5 && item.moisture >= 0.55 && item.moisture <= 0.8) item.bio = VoronoiStruct.Biome.Grassland;
                else if (item.elevation >= 0.5 && item.elevation <= 0.8 && item.moisture >= 0.55 && item.moisture <= 0.8) item.bio = VoronoiStruct.Biome.Forest;
                else if (item.elevation >= 0.8 && item.moisture >= 0.55 && item.moisture <= 0.8) item.bio = VoronoiStruct.Biome.Snow;

                else if (item.elevation <= 0.8 && item.moisture >= 0.8) item.bio = VoronoiStruct.Biome.Forest;
                else if (item.elevation >= 0.8 && item.moisture >= 0.8) item.bio = VoronoiStruct.Biome.Snow;

            }
            foreach(var item in map.polygons)
            {
                if (item.isSea) item.bio = VoronoiStruct.Biome.Ocean;
            }
        }

        void decide_coastline(ref VoronoiStruct.Voronoi map) //決定海岸線
        {
            foreach(var item1 in map.polygons)
            {
                if (item1.bio == VoronoiStruct.Biome.Ocean)
                {
                    foreach (var item2 in item1.edges)
                    {
                        if (item2.parentID == null) continue;
                        if(map.polygons[item2.parentID[0]].bio != VoronoiStruct.Biome.Ocean)
                        {
                            //map.polygons[item2.parentID[0]].bio = VoronoiStruct.Biome.Coastline;
                            map.polygons[item2.parentID[0]].bio |= VoronoiStruct.Biome.Coastline;
                            //System.Diagnostics.Debug.WriteLine("Coastline: " + map.polygons[item2.parentID[0]].focus + "  " + map.polygons[item2.parentID[0]].bio);
                        }
                        if (map.polygons[item2.parentID[1]].bio != VoronoiStruct.Biome.Ocean)
                        {
                            //map.polygons[item2.parentID[1]].bio = VoronoiStruct.Biome.Coastline;
                            map.polygons[item2.parentID[1]].bio |= VoronoiStruct.Biome.Coastline;
                            //System.Diagnostics.Debug.WriteLine("Coastline: " + map.polygons[item2.parentID[1]].focus + "  " + map.polygons[item2.parentID[1]].bio);
                        }
                    }                    
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)//Show River
        {
            foreach (var item in riversource)
            {
                System.Diagnostics.Debug.WriteLine("RiverSource X:" + item.x + "  Y: " + item.y);
            }
            
            foreach (var item in allRiver)
            {
                System.Diagnostics.Debug.WriteLine("X:" + item.x + "  Y: " + item.y);
            }
            //System.Diagnostics.Debug.WriteLine("TTT:" + vmap.polygons[7].bio);

        }

        void drawing_biome(VoronoiStruct.Voronoi map) //畫出生態系
        {
            foreach(var item in map.polygons)
            {
                switch (item.bio)
                {
                    case VoronoiStruct.Biome.Grassland:
                        drawPoint(grasslandBrush, item.focus);
                        break;
                    case VoronoiStruct.Biome.Forest:
                        drawPoint(forestBrush, item.focus);
                        break;
                    case VoronoiStruct.Biome.Desert:
                        drawPoint(desertBrush, item.focus);
                        break;
                    case VoronoiStruct.Biome.Snow:
                        drawPoint(snowBrush, item.focus);
                        break;
                    case VoronoiStruct.Biome.Lava:
                        drawPoint(lavaBrush, item.focus);
                        break;
                    case VoronoiStruct.Biome.Volcano:
                        drawPoint(volcanoBrush, item.focus);
                        break;
                    case VoronoiStruct.Biome.Ocean:
                        drawPoint(oceanBrush, item.focus);
                        break;
                    case VoronoiStruct.Biome.River:
                        drawPoint(new SolidBrush(Color.SkyBlue), item.focus);
                        break;
                    /*case VoronoiStruct.Biome.Riverbank:
                        drawPoint(new SolidBrush(Color.Pink), item.focus);
                        break;
                    case VoronoiStruct.Biome.Coastline:
                        drawPoint(new SolidBrush(Color.SaddleBrown), item.focus);
                        break;*/
                    default:
                        drawPoint(new SolidBrush(Color.Black), item.focus);
                        break;
                }
                if ((item.bio & VoronoiStruct.Biome.Coastline) != 0)
                {
                    drawPoint(new SolidBrush(Color.SaddleBrown), item.focus);
                }
                if ((item.bio & VoronoiStruct.Biome.Riverbank) != 0)
                {
                    drawPoint(new SolidBrush(Color.Pink), item.focus);
                }
                
            }
        }

        void decide_riverbank(ref VoronoiStruct.Voronoi map) //決定河岸
        {
            foreach (var item1 in map.polygons)
            {
                foreach (var item2 in item1.edges)
                {
                    if (item2.line.isRiver)
                    {
                        //System.Diagnostics.Debug.WriteLine(item2.parentID[0] +"   "+ item2.parentID[1]);
                        if (item2.parentID != null)
                        {
                            foreach (var item3 in map.polygons[item2.parentID[0]].edges)
                            {
                                if (item3.line.a.x == item2.line.b.x && item3.line.a.y == item2.line.b.y && item3.line.b.x == item2.line.a.x && item3.line.b.y == item2.line.a.y)
                                {
                                    item3.line.isRiver = true;
                                    map.polygons[item2.parentID[0]].bio |= VoronoiStruct.Biome.Riverbank;
                                    //System.Diagnostics.Debug.WriteLine("Riverbank  YES");
                                }
                            }
                            foreach (var item3 in map.polygons[item2.parentID[1]].edges)
                            {
                                if (item3.line.a.x == item2.line.b.x && item3.line.a.y == item2.line.b.y && item3.line.b.x == item2.line.a.x && item3.line.b.y == item2.line.a.y)
                                {
                                    item3.line.isRiver = true;
                                    map.polygons[item2.parentID[1]].bio |= VoronoiStruct.Biome.Riverbank;
                                    //System.Diagnostics.Debug.WriteLine("Riverbank  YES");
                                }
                            }
                        }
                        /*foreach (var item4 in map.polygons)
                        {
                            foreach (var item3 in item4.edges)
                            {
                                if (item3.line.a.x == item2.line.a.x && item3.line.a.y == item2.line.a.y && item3.line.b.x == item2.line.b.x && item3.line.b.y == item2.line.b.y)
                                {
                                    item3.line.isRiver = true;
                                    item4.bio |= VoronoiStruct.Biome.Riverbank;
                                    System.Diagnostics.Debug.WriteLine("Riverbank: " + item4.bio);
                                }
                            }
                        }*/
                        item1.bio |= VoronoiStruct.Biome.Riverbank;
                        //System.Diagnostics.Debug.WriteLine("Riverbank: "+ item1.bio);
                    }
                }
            }
        }


        int Toppolygon;
        bool isTopExist = true;
        private void elevationbutton_Click(object sender, EventArgs e)
        {
            if (vmap == null)
            {
                MessageBox.Show("No map");
                return;
            }
            pictureBox1.Image = null;
            drawVoronoi(vmap);
            drawing_biome(vmap);
            draw_river(vmap);

            int i = 0, n;
            
            bool beTop;
            while (i < 10000 && isTopExist)//找出最高的polygon
            {
                beTop = true;
                Toppolygon = -1;
                n = rand.Next(0, vmap.polygons.Count);
                /*foreach(var item in vmap.polygons[n].edges)
                {
                    if (item.line.a.x <= 300 || item.line.a.x >= 500) beTop = false;
                    else if (item.line.a.y <= 300 || item.line.a.y >= 500) beTop = false;
                    else if (item.line.b.x <= 300 || item.line.b.x >= 500) beTop = false;
                    else if (item.line.b.y <= 300 || item.line.b.y >= 500) beTop = false;
                }*/
                foreach (var item in vmap.polygons[n].edges)
                {
                    if (item.line.a.x <= 200 || item.line.a.x >=600) beTop = false;
                    else if (item.line.a.y <= 200 || item.line.a.y >= 600) beTop = false;
                    else if (item.line.b.x <= 200 || item.line.b.x >= 600) beTop = false;
                    else if (item.line.b.y <= 200 || item.line.b.y >= 600) beTop = false;
                }
                if (beTop)
                {
                    Toppolygon = n;
                    //System.Diagnostics.Debug.WriteLine("Top is: " + n);
                    //isTopExist = false;
                    break;
                }
                ++i;
                if (i == 10000) MessageBox.Show("No find corresponding polygon");
            }
            if (Toppolygon == -1) return;
            else beTop = true;
            System.Diagnostics.Debug.WriteLine("Top is: " + Toppolygon);
            /*foreach(var item in vmap.polygons[Toppolygon].edges)
            {
                giveelevation_assignline(item.line.a.x, item.line.a.y, 1000);
                giveelevation_assignline(item.line.b.x, item.line.b.y, 1000);
            }*/
            if (isTopExist)
            {
                calculate_polygoncenter(ref vmap);
                //System.Diagnostics.Debug.WriteLine(vmap.polygons[Toppolygon].centerX + "\t" + vmap.polygons[Toppolygon].centerY);
                calelevation(ref vmap);
                //System.Diagnostics.Debug.WriteLine("ONCE");
            }
            /*foreach(var item in vmap.polygons[Toppolygon].edges) //印出最高Polygon的各點高度
            {
                System.Diagnostics.Debug.WriteLine("Toppolygon elevation x: " + item.line.a.elevation + " y: " + item.line.b.elevation);
            }*/

            find_river_source(ref vmap);
            if (river_generator(riversource[riversource.Count - 1]) == -1) System.Diagnostics.Debug.WriteLine("Correct generate river");
            draw_river(vmap);

            find_sea(ref vmap);
            /*foreach (var item in vmap.polygons) //把海畫出來
            {
                if (item.isSea)
                {
                    foreach (var item2 in item.edges)
                    {
                        drawLine(greenpen, item2.line);
                    }
                }
            }*/


            give_moisture(ref vmap);


            if (isTopExist)
            {
                elevation_for_polygon(ref vmap);
                ratio_elevation_of_polygon(ref vmap);
            }

            

            decide_biome(ref vmap);

            decide_coastline(ref vmap);
            /*foreach(var item in vmap.polygons)
            {
                System.Diagnostics.Debug.WriteLine(item.bio);
            }*/
            decide_riverbank(ref vmap);

            drawing_biome(vmap);

            /*if (beTop)
            {
                isTopExist = false;
            }*/
            //System.Diagnostics.Debug.WriteLine("Here");
            pictureBox1.Invalidate();
        }

        private void openfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                readMap(openFileDialog1.FileName, out vmap);
                //if(vmap != null) MessageBox.Show("file input success");
                drawVoronoi(vmap);
                drawing_biome(vmap);
                draw_river(vmap);
            }
        }



        void cast_to_world(VoronoiStruct.Voronoi map) //將地圖投影到unit上
        {
            float x1 = 0, y1 = 0, x2 = 0, y2 = 0;
            float inner = 0, outter = 0;
            for (int i = 0; i < world.unit.GetLength(0); ++i)
            {
                for (int j = 0; j < world.unit.GetLength(1); ++j)
                {
                    world.unit[i, j] = 0;
                    foreach (var item1 in map.polygons)
                    {
                        if (item1.bio == VoronoiStruct.Biome.Ocean) continue;
                        foreach (var item2 in item1.edges)
                        {
                            if (!item2.line.isRiver) continue;
                            x1 = item2.line.a.x - i;
                            y1 = item2.line.a.y - j;
                            x2 = item2.line.b.x - i;
                            y2 = item2.line.b.y - j;
                            inner = x1 * x2 + y1 * y2;
                            outter = x1 * y2 - x2 * y2;
                            if (inner < 0 && outter == 0)
                            {
                                world.unit[i, j] = (int)VoronoiStruct.Biome.River;
                            }
                        }
                    }
                }
            }
            
            float result;
            bool inside = true;
            //bool river_so_skip = true;

            for (int i = 0; i < world.unit.GetLength(0); ++i)
            {
                for (int j = 0; j < world.unit.GetLength(1); ++j)
                {
                    if (world.unit[i, j] != 0) continue;
                    foreach (var item1 in map.polygons)
                    {
                        foreach (var item2 in item1.edges)
                        {
                            x1 = item2.line.a.x - i;
                            y1 = item2.line.a.y - j;
                            x2 = item2.line.b.x - i;
                            y2 = item2.line.b.y - j;
                            result = x1 * y2 - y1 * x2;
                            if (result < 0f)
                            {
                                inside = false;
                                break;
                            }
                        }
                        if (inside)
                        {
                            world.unit[i, j] = (int)item1.bio;
                            break;
                        }
                        inside = true;
                    }
                }
            }


        }

        //void 


        private void button1_Click(object sender, EventArgs e)
        {
            if (vmap == null)
            {
                MessageBox.Show("No map");
                return;
            }
            
            cast_to_world(vmap);
            System.Diagnostics.Debug.WriteLine("Cast Success");
        }


        private void draw_unit(int a, int b) //畫出unit
        {
            switch (world.unit[a, b])
            {
                case (int)VoronoiStruct.Biome.Grassland:
                    g.DrawRectangle(new Pen(Color.SpringGreen), new Rectangle(a, b, 1, 1));
                    break;
                case (int)VoronoiStruct.Biome.Forest:
                    g.DrawRectangle(new Pen(Color.DarkGreen), new Rectangle(a, b, 1, 1));
                    break;
                case (int)VoronoiStruct.Biome.Desert:
                    g.DrawRectangle(new Pen(Color.Gold), new Rectangle(a, b, 1, 1));
                    break;
                case (int)VoronoiStruct.Biome.Snow:
                    g.DrawRectangle(new Pen(Color.Gray), new Rectangle(a, b, 1, 1));
                    break;
                case (int)VoronoiStruct.Biome.Lava:
                    g.DrawRectangle(new Pen(Color.Firebrick), new Rectangle(a, b, 1, 1));
                    break;
                case (int)VoronoiStruct.Biome.Volcano:
                    g.DrawRectangle(new Pen(Color.Fuchsia), new Rectangle(a, b, 1, 1));
                    break;
                case (int)VoronoiStruct.Biome.Ocean:
                    g.DrawRectangle(new Pen(Color.Navy), new Rectangle(a, b, 1, 1));
                    break;
                case (int)VoronoiStruct.Biome.River:
                    g.DrawRectangle(new Pen(Color.SkyBlue), new Rectangle(a, b, 1, 1));
                    break;
                case (int)VoronoiStruct.Biome.Coastline:
                    g.DrawRectangle(new Pen(Color.SaddleBrown), new Rectangle(a, b, 1, 1));
                    break;
                case (int)VoronoiStruct.Biome.Riverbank:
                    g.DrawRectangle(new Pen(Color.Pink), new Rectangle(a, b, 1, 1));
                    break;
                default:
                    g.DrawRectangle(new Pen(Color.Black), new Rectangle(a, b, 1, 1));
                    break;
            }
            if ((world.unit[a, b] & (int)VoronoiStruct.Biome.Riverbank) != 0) g.DrawRectangle(new Pen(Color.Pink), new Rectangle(a, b, 1, 1));
            if ((world.unit[a, b] & (int)VoronoiStruct.Biome.Coastline) != 0) g.DrawRectangle(new Pen(Color.SaddleBrown), new Rectangle(a, b, 1, 1));

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (vmap == null)
                return;
            bmp = new Bitmap(vmap.width, vmap.height);
            g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            pictureBox1.Image = bmp;
            for (int i = 0; i < world.unit.GetLength(0); ++i)
            {
                for (int j = 0; j < world.unit.GetLength(1); ++j)
                {
                    draw_unit(i, j);
                }
            }
            System.Diagnostics.Debug.WriteLine("Finish Draw");
        }
    }
}
