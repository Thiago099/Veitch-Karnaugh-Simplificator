using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Truth_table
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            numericUpDown1_ValueChanged(null, null);
        }
        List<string> generate_map(int c)
        {
            var p = 2;//number of values
            List<string> lines = new List<string>();
            for (int i = 0; i < Math.Pow(p, c); i++)
            {
                var sb = new StringBuilder();
                for (int k = 1; k < c + 1; k++)
                    sb.Append((int)(i / Math.Pow(p, k - 1) % p));
                lines.Add(sb.ToString());
            }
            for (int j = 3; j < Math.Pow(p, c); j = j * 2 + 1)
            {
                for (int i = j; i < lines.Count; i += j)
                {
                    swap(lines, i, i - 1);
                }
            }
            return lines;

        }
        void swap<T>(List<T> l, int a, int b)
        {
            T temp = l[a];
            l[a] = l[b];
            l[b] = temp;
        }
        int[,] v;
        int l;
        Button[] b;
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            var size = (int)numericUpDown1.Value;
            var map = generate_map(size);
            panel1.Controls.Clear();
            int y = 0;
            v=new int[map.Count(),map[0].Count()+1];
            b = new Button[map.Count()];
            l = map[0].Count();
            for (int i = 0; i < map.Count(); i++)
            {
                int x = 0;
                for (int j = 0; j < map[i].Count(); j++)
                {

                    var l = new Label();
                    l.Width = 20;
                    l.Height = 20;
                    v[i, j] = map[i][j] - '0';
                    l.Text = map[i][j].ToString();
                    l.Left = x+20;
                    l.Top = y+20;
                    x += 30;
                    panel1.Controls.Add(l);

                }
                var bt = new Button();
                bt.Text = "0";
                panel1.Controls.Add(bt);
                bt.Left = x+20;
                bt.Top = y+15;
                bt.Click += new System.EventHandler(this.button1_Click);
                b[i] = bt;
                y += 30;
            }
            

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Button s = (Button)sender;
            if (s.Text == "0") s.Text = "1";
            else s.Text = "0";
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            for (int i = 0; i < b.Length; i++)
            {

                v[i, l] = b[i].Text[0] - '0';
            }
            var r = v.GetLength(1) - 1;
            var l1 = r / 2;
            var l2 = r - l1;


            List<string> x = generate_map(l1);
            List<string> y = generate_map(l2);
            var map = new int[x.Count, y.Count];

            for (int i = 0; i < x.Count; i++)
            {
                for (int j = 0; j < y.Count; j++)
                {
                    for (int k = 0; k < v.GetLength(0); k++)
                    {
                        bool found = true;
                        int m = 0;
                        for (int l = 0; l < x[i].Length; l++)
                        {
                            if (x[i][l] - '0' != v[k, m])
                            {
                                found = false;
                                break;
                            }
                            m++;
                        }
                        if (!found) continue;
                        for (int l = 0; l < y[j].Length; l++)
                        {
                            if ((y[j][l] - '0') != v[k, m])
                            {
                                found = false;
                                break;
                            }
                            m++;
                        }
                        if (!found) continue;

                        map[i, j] = v[k, v.GetLength(1) - 1];
                    }
                }
            }
            int n = 0;
            var sb = new StringBuilder();

            for (int i = 0; i < l1; i++)
            {
                sb.Append((char)(n + 'A'));
                n++;
            }
            section.xv = sb.ToString();
            sb = new StringBuilder();

            Console.Write("\\");
            for (int i = 0; i < l2; i++)
            {
                sb.Append((char)(n + 'A'));
                n++;
            }
            section.yv = sb.ToString();

            List<section> groups = new List<section>();
            section.max_x = x.Count;
            section.max_y = y.Count;
            section.map = map;
            section.x = x;
            section.y = y;
            section.needed = new bool[x.Count, y.Count];
            for (int i = 0; i < x.Count; i++)
            {
                for (int j = 0; j < y.Count; j++)
                {
                    if (map[i, j] == 1) section.needed[i, j] = true;
                    else section.needed[i, j] = false;
                }
            }
            for (int i = 0; i < x.Count; i++)
            {
                for (int j = 0; j < y.Count; j++)
                {
                    if (map[i, j] == 1)
                    {
                        var c = new section(i, j, i, j);
                        groups.Add(c);
                    }
                }
            }
            groups = section.expand(groups);
            for (int i = 0; i < groups.Count();)
            {
                if (groups[i].size % 2 == 1 && groups[i].size != 1) groups.RemoveAt(i);
                else i++;
            }
            sb = new StringBuilder();

            while (true)
            {
                if(groups.Count()==0)
                {
                    break;
                }
                int max = groups[0].size;
                section maxs = groups[0];
                for (int i = 1; i < groups.Count(); i++)
                {
                    if (groups[i].size > max)
                    {
                        max = groups[i].size;
                        maxs = groups[i];
                    }
                }
                Console.WriteLine($"{maxs.y1}.{maxs.x1},{maxs.y2}.{maxs.x2}");
                var res = maxs.result();
                if(res!="")
                sb.Append(res + "+");
                maxs.set_used();
                groups.Remove(maxs);
                for (int i = 0; i < groups.Count();)
                {
                    if (!groups[i].isneeded())
                    {
                        groups.RemoveAt(i);
                    }
                    else
                        i++;
                }
                if (groups.Count() == 0) break;
            }
            if(sb.Length>0)
            textBox1.Text = sb.ToString(0, sb.Length - 1);

        }
    }
}
