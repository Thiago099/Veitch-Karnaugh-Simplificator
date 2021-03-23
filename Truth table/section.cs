using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Truth_table
{
    class section
    {
        public static int max_x;
        public static int max_y;
        public static int[,] map;
        public static bool[,] needed;
        public static List<string> x;
        public static List<string> y;
        public static string xv;
        public static string yv;
        public int height
            => y1 == y2 ? 1 : y2 > y1 ? y2 - y1 + 1 : max_y - y1 + y2 + 1;
        public int width
            => x1 == x2 ? 1 : x2 > x1 ? x2 - x1 + 1 : max_x - x1 + x2 + 1;
        public int size
                => width * height;

        public int x1;
        public int y1;
        public int x2;
        public int y2;

        public section(int x1, int y1, int x2, int y2)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
        }
        section clone => new section(x1, y1, x2, y2);
        public static int tile(int x, int max)
        {
            if (x >= max) x = x % max;
            while (x < 0) x += max;
            return x;
        }
        public void move_left()
        {
            x1 = tile(x1 - 1, max_x);
        }
        public void move_right()
        {
            x2 = tile(x2 + 1, max_x);
        }
        public void move_top()
        {
            y1 = tile(y1 - 1, max_y);
        }
        public void move_bottom()
        {
            y2 = tile(y2 + 1, max_y);
        }
        public bool can_move_left()
            => isvalid(tile(x1 - 1, max_x), y1, x2, y2);
        public bool can_move_right()
             => isvalid(x1, y1, tile(x2 + 1, max_x), y2);
        public bool can_move_top()
             => isvalid(x1, tile(y1 - 1, max_y), x2, y2);
        public bool can_move_bottom()
            => isvalid(x1, y1, x2, tile(y2 + 1, max_y));

        public static bool contains(List<section> l, section s)
        {
            foreach (var i in l)
            {
                if (i == s) return true;
            }
            return false;
        }
        public static List<section> expand(List<section> c)
        {

            var r = new List<section>();
            foreach (var i in c)
                r.Add(i);
            bool changed = false;
            foreach (var j in c)
            {
                if (j.can_move_left())
                {
                    var s = j.clone;
                    s.move_left();
                    if (!contains(r, s))
                    {
                        r.Add(s);
                        changed = true;
                    }

                }
                if (j.can_move_right())
                {
                    var s = j.clone;
                    s.move_right();
                    if (!contains(r, s))
                    {
                        r.Add(s);
                        changed = true;
                    }
                }
                if (j.can_move_top())
                {
                    var s = j.clone;
                    s.move_top();
                    if (!contains(r, s))
                    {
                        r.Add(s);
                        changed = true;
                    }

                }
                if (j.can_move_bottom())
                {
                    var s = j.clone;
                    s.move_bottom();
                    if (!contains(r, s))
                    {
                        r.Add(s);
                        changed = true;
                    }

                }
                j.can_expand = false;
            }
            if (changed) r = expand(r);
            return r;
        }
        bool can_expand = true;
        bool isvalid(int x1, int y1, int x2, int y2)
        {
            for (int i = x1; ; i = tile(i + 1, max_x))
            {
                for (int j = y1; ; j = tile(j + 1, max_y))
                {
                    if (map[i, j] != 1)
                    {
                        return false;
                    }
                    if (j == y2) break;
                }
                if (i == x2) break;
            }
            return true;
        }
        public string result()
        {
            var result = new StringBuilder();

            List<string> curs = new List<string>();
            bool[] negatedx = new bool[xv.Length];
            bool[] cx = new bool[xv.Length];
            for (int j = 0; j < cx.Length; j++)
            {
                cx[j] = true;
            }
            bool[] negatedy = new bool[yv.Length];
            bool[] cy = new bool[yv.Length];
            for (int j = 0; j < cy.Length; j++)
            {
                cy[j] = true;
            }
            for (int i = x1; ; i = tile(i + 1, max_x))
            {
                curs.Add(x[i]);
                if (i == x2) break;
            }
            for (int i = 0; i < curs.Count; i++)
            {
                for (int j = i; j < curs.Count; j++)
                {
                    for (int k = 0; k < curs[j].Length; k++)
                    {
                        if (curs[i][k] != curs[j][k]) cx[k] = false;
                        else if (curs[i][k] == '0') negatedx[k] = true;
                    }
                }
            }
            for (int i = 0; i < cx.Length; i++)
            {
                if (cx[i])
                {
                    result.Append(xv[i]);
                    if (negatedx[i]) result.Append('\'');
                }
            }
            curs = new List<string>();
            for (int i = y1; ; i = tile(i + 1, max_y))
            {
                curs.Add(y[i]);
                if (i == y2) break;
            }
            for (int i = 0; i < curs.Count; i++)
            {
                for (int j = i; j < curs.Count; j++)
                {
                    for (int k = 0; k < curs[j].Length; k++)
                    {
                        if (curs[i][k] != curs[j][k]) cy[k] = false;
                        else if (curs[i][k] == '0') negatedy[k] = true;
                    }
                }
            }
            for (int i = 0; i < cy.Length; i++)
            {
                if (cy[i])
                {
                    result.Append(yv[i]);
                    if (negatedy[i])
                        result.Append('\'');
                }
            }
            return result.ToString();
        }
        public bool isneeded()
        {
            for (int i = x1; ; i = tile(i + 1, max_x))
            {
                for (int j = y1; ; j = tile(j + 1, max_y))
                {
                    if (needed[i, j] == true) return true;
                    if (j == y2) break;
                }
                if (i == x2) break;
            }
            return false;
        }
        public void set_used()
        {
            for (int i = x1; ; i = tile(i + 1, max_x))
            {
                for (int j = y1; ; j = tile(j + 1, max_y))
                {
                    needed[i, j] = false;
                    if (j == y2) break;
                }
                if (i == x2) break;
            }
        }
        public static bool operator ==(section a, section b)
                => a.x1 == b.x1 && a.x2 == b.x2 && a.y1 == b.y1 && a.y2 == b.y2;
        public static bool operator !=(section a, section b)
                => a.x1 != b.x1 && a.x2 != b.x2 && a.y1 != b.y1 && a.y2 != b.y2;
    }

}
