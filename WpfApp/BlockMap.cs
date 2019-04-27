using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp
{
    public class BlockMap
    {
        public int row ;
        public int clo ;
        public Block[,] map;
        const int mmax = 39;
        const int block = 1;
        private List<Point> path =new List<Point>();

        public List<Point> getPath()
        {
            return path;
        }
        private void EmptyPath()
        {
            path.Clear();
        }
        public int this[int i,int j]
        {
            get
            {
                return map[i, j].value;
            }
            set
            {
                map[i, j].value = value;
            }
        }
        private BlockMap(int row , int clo )
        {
            this.row = row;
            this.clo = clo;
            this.map = new Block[row+block*2, clo+block*2];
            for (int i=0; i < map.GetLength(0); ++i)
            {
                for (int j=0; j < map.GetLength(1); ++j)
                {
                    map[i, j] = new Block();
                    map[i, j].point = new Point(j, i);
                }
            }
            Random rn =new Random();

            for (int i = block; i < map.GetLength(0) -1; ++i)
            {
                for (int j = block,len=map.GetLength(1); j <len/ 2 ; ++j)
                {
                    var r=rn.Next(mmax-1)+1;
                    map[i, j].value = r;
                    map[i,len-1-j].value=r;
                }
            }
        }
        public static BlockMap GetNewMap(int level)
        {
            int row = 6 + level; int clo = 6 + level;
            if (row <= 0 || clo <= 0 || (row * clo) % 2 != 0) return null;
            else
            {
                return new BlockMap(row, clo);
            }
        }
        public void Unorder()
        {
            Random random = new Random();
            int time = random.Next(30) + this.row * 2;
            for (int i=0; i<time; ++i)
            {
                int a = random.Next(this.clo)+block;
                int b = random.Next(this.row) + block;
                int c = random.Next(this.clo) + block;
                int d = random.Next(this.row) + block;
                var temp = map[c, d];
                var t = a;
                
                map[c, d] = map[a, b];
                map[a, b] = temp;
                var tt = map[c, d].point;
                map[c, d].point = map[a, b].point;
                map[a, b].point = tt;
            }
        }
        public Boolean isWin()
        {
            for (int r = 0; r < map.GetLength(0); r++)
                for (int c = 0; c < map.GetLength(1); c++)
                    if (map[r,c].value != 0)
                        return false;
            return true;
        }
        public bool wuZhe(Point point1, Point point2)
        {

            if (point1.Y != point2.Y && point1.X != point2.X)
                return false;
            // 如果两点的x坐标相等，则在水平方向上扫描
            if (point1.Y == point2.Y)
            {
                if (point1.X == point2.X - 1 || point2.X == point1.X - 1)// 列相邻
                    return true;
                for (int i = Math.Min(point1.X, point2.X) + 1; i < Math.Max(point1.X, point2.X); i++)
                    if (map[point1.Y,i].value!= 0)
                        return false;
            }
            // 如果两点的y坐标相等，则在竖直方向上扫描
            else if (point1.X == point2.X)
            {
                if (point1.Y == point2.Y - 1 || point2.Y == point1.Y - 1)// 行相邻
                    return true;
                for (int i = Math.Min(point1.Y , point2.Y) + 1; i < Math.Max(point1.Y , point2.Y); i++)
                    if (map[i,point1.X].value != 0)
                        return false;
            }
            return true;
        }
        public bool yiZhe(Point star, Point end)
        {

            // 如果属于0折的情况，直接返回FALSE
            if (star.Y == end.Y || star.X == end.X)
                return false;
            // 测试对角点1
            if (map[star.Y,end.X].value == 0)
            {
                bool test1 = wuZhe(star, new Point( end.X, star.Y));
                bool test2 = test1 ? wuZhe( new Point(end.X, star.Y), end) : test1;
                if (test1 && test2)
                {
                    path.Add(new Point(end.X, star.Y));
                    return true;
                }
            }
            // 测试对角点2
            if (map[end.Y,star.X].value == 0)
            {
                bool test1 = wuZhe(star, new Point(star.X,end.Y ) );
                bool test2 = test1 ? wuZhe( new Point(star.X, end.Y),end) : test1;
                if (test1 && test2)
                {
                    path.Add(new Point(star.X, end.Y));
                    return true;
                }
            }
            return false;
        }
        public bool erZhe(Point star, Point end)
        {
           
            //判断是否二折连接

            // 向下垂直遍历
            for (int i = star.Y + 1; i < map.GetLength(0); i++)
            {
                if (map[i,star.X ].value == 0)
                {
                    if (yiZhe(end, new Point(star.X,i)))
                    {
                        path.Add(new Point(star.X, i));
                        return true;
                    }
                }
                else
                    break;
            }
            // 向上垂直遍历
            for (int i = star.Y-1; i > -1; i--)
            {
                if (map[i,star.X].value == 0)
                {
                    if (yiZhe( new Point(star.X,i ) , end))
                    {
                        path.Add(new Point(star.X, i));
                        return true;
                    }
                }
                else
                    break;

            }
            // 向右水平遍历
            for (int i = star.X + 1; i < map.GetLength(1); i++)
            {
                if (map[star.Y,i].value == 0)
                {
                    if (yiZhe(end , new Point(i , star.Y)))
                    {
                        path.Add(new Point(i ,star.Y));
                        return true;
                    }
                }
                else
                    break;

            }
            // 向左水平遍历
            for (int i = star.X-1; i > -1; i--)
            {
                if (map[star.Y, i].value == 0)
                {
                    if (yiZhe( new Point( i, star.Y), end))
                    {
                        path.Add(new Point(i, star.Y));
                        return true;
                    }
                }
                else
                    break;
            }
            return false;

        }
        public Size GetSize()
        {
            return new Size((map.GetLength(1)) * Block.BlockWidth, (map.GetLength(0)) * Block.Blockheight);
        }

        public Block GetBlock( int x, int y) {
            return map[x, y];
        }

        internal void ClearBlock(Point point)
        {
            map[point.Y, point.X].value = 0;
        }

        internal bool LinkMatch(Point star, Point end, out List<Point> cps)
        {
            this.EmptyPath();
            cps = new List<Point>();
            if (wuZhe(star, end))
            {
                cps = this.getPath();
                return true;
            }
            else if (yiZhe(star, end))
            {
                cps = this.getPath();
                return true;
            }
            else if (erZhe(star, end))
            {
                cps = this.getPath();
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool CanGoon( out Point a,out Point b)
        {
            List<Point> cps;
            for(int i=1; i<map.GetLength(0)-1; ++i)
            {
                for (int j=1; j<map.GetLength(1)-1; ++j)
                {
                    var t = map[i, j];
                    if (t.value == 0) continue;
                    for (int q=1; q < map.GetLength(0); ++q)
                    {
                        for (int p=1; p<map.GetLength(1); ++p)
                        {
                            var tt = map[q, p];
                            if (tt.value == 0) continue;
                            if (t.value != tt.value || t==tt) continue;
                            if (LinkMatch(t.point, tt.point, out cps))
                            {
                                a = t.point;
                                b = tt.point;
                                return true;
                            }
                        }
                    }
                }
            }
            a = new Point();
            b = new Point();
            return false;
        }
    }
}
