using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp
{
    public class Block
    {
        public Point point;
        public int value;
        public const int BlockWidth = 31;
        public const int Blockheight = 34;
        public Block()
        {

        }
        public Block(int x, int y)
        {
            point = new Point(x, y);
        }
    }
}
