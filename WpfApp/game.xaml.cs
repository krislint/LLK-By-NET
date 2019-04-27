using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for game.xaml
    /// </summary>
    public partial class game : Page
    {
        BlockMap bmap;
        int resttime = 3;
        int tiptime = 4;
        int level;
        ResourceManager rm;
        int score;
        public static System.Windows.Forms.PictureBox picturebox;
        System.Drawing.Point? first;
        System.Timers.Timer timer1 = new System.Timers.Timer();
        
        Action<TextBox, string> updateAction;
        Action<string> gameover;
        static int counter = 1;
        int sec;
        void timerstar()
        {
            sec = 60;
            timer1.Enabled = true;
            timer1.Interval = 1000;//执行间隔时间,单位为毫秒
            timer1.AutoReset = true;
            timer1.Start();
            
        }

        private void UpdateTb(TextBox tb, string text)
        {

                sec = int.Parse(text);
                tb.Text = text;
        
        }
        // 定时器有关的函数 --自减
        private  void jianjianAsync(object sender, System.Timers.ElapsedEventArgs e)
        {
            
            //sec = int.Parse(time.Text);
            if (sec <= 0)
            {
                timer1.Stop();
                this.Dispatcher.BeginInvoke(gameover, score.ToString());
            }
                this.sec--;

             time.Dispatcher.BeginInvoke(updateAction, time, sec.ToString());
        }
       
        public game()
        {
            InitializeComponent();
            var cur = (Bitmap)Images.ResourceManager.GetObject("cursor");
            this.Cursor = MouseManager.CreateCursor(cur, 0, 0);
            this.level = Window1.level;
            // WPF 没有这个控件 只能强行new 一个 添加到winform host
            picturebox = new System.Windows.Forms.PictureBox();
            pictureHost.Child = picturebox;
            //手动添加事件处理函数
            picturebox.Paint += Repaint;
            picturebox.MouseClick += PictureBox_MouseClick;
            rm = Images.ResourceManager;
            InitMap();
            //跨线程调用的 解决方法
            updateAction = new Action<TextBox, string>(UpdateTb);
            timer1.Elapsed += new System.Timers.ElapsedEventHandler(jianjianAsync);
            gameover = new Action<string>(GameOver);
        }
        public void Repaint(Object sender, System.Windows.Forms.PaintEventArgs e)
        {
            paintmap(e.Graphics);
        }
        public void paintmap(Graphics g)
        {
            System.Drawing.Image image = null;
            for (int i=1; i<bmap.row+1; ++i)
            {
                for (int j=1; j<bmap.clo+1; ++j)
                {
                    if (bmap[i, j] > 0)
                    {
                        image = (Bitmap)rm.GetObject("_" + bmap[i, j]);
                        g.DrawImage(image, Block.BlockWidth * j, Block.Blockheight * i,31,34);
                    }
                }
            }
            if (first != null)
            {
                g.DrawImage((Bitmap)rm.GetObject("_" + bmap[first.Value.Y,first.Value.X]+ "_L2"), Block.BlockWidth * first.Value.X, Block.Blockheight * first.Value.Y,31,34);
            }
        }
        private void InitMap()
        {
            MessageBox.Show("开始", "开始");
            bmap = BlockMap.GetNewMap(level);
            System.Drawing.Size size = bmap.GetSize();
            picturebox.Width = size.Width;
            picturebox.Height=size.Height;
            pictureHost.Width = size.Width;
            pictureHost.Height = size.Height;
            picturebox.Padding = new System.Windows.Forms.Padding(0);
            picturebox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            var cur = (Bitmap)Images.ResourceManager.GetObject("cursor");
            picturebox.Cursor = SetCursor(cur, new System.Drawing.Point( 0, 0));
            bmap.Unorder();
            
            reset.Content = "重置 (" + resttime + "次)";
            reset.IsEnabled = true;
            tip.Content = "提示 (" + tiptime + "次)";
            tip.IsEnabled = true;
            timerstar();
        }
        public System.Windows.Forms.Cursor SetCursor(Bitmap cursor, System.Drawing.Point hotPoint)
        {
            int hotX = hotPoint.X;
            int hotY = hotPoint.Y;
            Bitmap myNewCursor = new Bitmap(cursor.Width * 2 - hotX, cursor.Height * 2 - hotY);
            Graphics g = Graphics.FromImage(myNewCursor);
            g.Clear(System.Drawing.Color.FromArgb(0, 0, 0, 0));
            g.DrawImage(cursor, cursor.Width - hotX, cursor.Height - hotY, cursor.Width,
            cursor.Height);
            var Cursor = new System.Windows.Forms.Cursor(myNewCursor.GetHicon());
            g.Dispose();
            myNewCursor.Dispose();
            return Cursor;
        }
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            bmap.Unorder();
            picturebox.Invalidate();
            resttime--;
            if (resttime <= 0)
            {
                reset.IsEnabled = false;
                reset.Content = "已用完！";
                return;
            }
            reset.Content = "重置 (" + resttime + "次)";
            first = null;
        }

        private void Tip_Click(object sender, RoutedEventArgs e)
        {
            System.Drawing.Point a, b;
            if (this.GetTip(out a, out b))
            {
                first = null;
                
                var g = picturebox.CreateGraphics();
                g.DrawImage((Bitmap)rm.GetObject("_" + bmap[a.Y, a.X] + "_L2"), Block.BlockWidth * a.X, Block.Blockheight * a.Y, 31, 34);
                g.DrawImage((Bitmap)rm.GetObject("_" + bmap[b.Y, b.X] + "_L2"), Block.BlockWidth * b.X, Block.Blockheight * b.Y, 31, 34);
            }
            else
            {
                MessageBox.Show("无解请重置地图");
            }
            tiptime--;
            if (tiptime <= 0)
            {
                tip.IsEnabled = false;
                tip.Content = "已用完！";
                return;
            }
            tip.Content = "提示 (" + tiptime + "次)";
        }

        private void PictureBox_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            
            int x = e.X / Block.BlockWidth;
            int y = e.Y / Block.Blockheight;
            if (x <= 0 || x > bmap.clo || y <= 0 || y > bmap.row || bmap[y, x] <= 0)
            {
                first = null;
                return;
            }
            if (first == null)
            {
                first = new System.Drawing.Point(x, y);
                picturebox.Invalidate();
                return;
            }
            else if (bmap[first.Value.Y, first.Value.X] == bmap[y, x] && (first.Value.X != x || first.Value.Y != y))
            {
                
                    System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.BlueViolet, 2);
                    Graphics g = picturebox.CreateGraphics();
                if (Link(first.Value, new System.Drawing.Point(x, y), g, pen))
                {

                    first = null;
                    if (bmap.isWin())
                    {
                        GameOver(score.ToString());
                        resttime += 2;
                        tiptime += 2;
                        level += 2;
                    }
                    else
                    {
                        System.Drawing.Point a, b;
                        if (bmap.CanGoon(out a, out b))
                        {

                        }
                        else
                        {
                            if (resttime > 0)
                            {
                                Reset_Click(null, null);
                            }
                            else
                            {
                                GameOver(score.ToString());
                            }
                        }
                    }
                }
                else
                {
                    first = new System.Drawing.Point(x, y);
                    counter = 1;
                    picturebox.Invalidate();
                }
            }
            else 
            {
                first = new System.Drawing.Point(x, y);
                picturebox.Invalidate();
                counter = 1;
                return;
            }

        }
        public class Boomclass
        {
            System.Drawing.Point point;
            Graphics g;
            ResourceManager rm; //由于忘记了内部类如何调用外部类的方法 所以再加个参数
            public  Boomclass(System.Drawing.Point point, Graphics g, ResourceManager rm)
            {
                this.point = point;
                this.g = g;
                this.rm = rm;
            }
            public void Boom()
            {
                System.Drawing.Image image;
                for (int i = 1; i <= 6; ++i)
                {
                    image = (Bitmap)rm.GetObject("B" + i);
                    g.DrawImage(image, Block.BlockWidth * point.X, Block.Blockheight * point.Y,31,34);
                    Thread.Sleep(100);
                    
                }
                game.picturebox.Invalidate();
            }
        }
            
        private bool Link(System.Drawing.Point first, System.Drawing.Point second, Graphics g, System.Drawing.Pen pen)
        {
            List<System.Drawing.Point> cps = null;
            if (bmap.LinkMatch(first, second, out cps))
            {
                // 连线操作
                System.Drawing.Point[] ps = new System.Drawing.Point[2 + cps.Count];
                ps[0] = new System.Drawing.Point(second.X * Block.BlockWidth + Block.BlockWidth / 2,
                                              second.Y * Block.Blockheight + Block.Blockheight / 2);
                int i = 1;
                while (i <= cps.Count)
                {
                    ps[i] = new System.Drawing.Point(cps[i - 1].X * Block.BlockWidth + Block.BlockWidth / 2,
                                                 cps[i - 1].Y * Block.Blockheight + Block.Blockheight / 2);
                    i++;
                }
                ps[i] = new System.Drawing.Point(first.X * Block.BlockWidth + Block.BlockWidth / 2,
                                             first.Y * Block.Blockheight + Block.Blockheight / 2);
                g.DrawLines(pen, ps);

                //连击分数翻倍
                score += counter*10;
                counter++;
                if (counter >= 5)
                {
                    counter = 5;
                }
                socre.Content = score;

                // 多线程实现 两个方块同时消除动画
                Boomclass bc1 = new Boomclass(first, picturebox.CreateGraphics(), rm);
                Thread th1 = new Thread(bc1.Boom);
                Boomclass bc2 = new Boomclass(second, picturebox.CreateGraphics(), rm);
                Thread th2 = new Thread(bc2.Boom);
                th1.Start();
                th2.Start();
                sec += 3;
                
                //这样的实现方法有点难受 但是懒得再开一个线程了
                Thread.Sleep(500);
                bmap.ClearBlock(first);
                bmap.ClearBlock(second);//消去这两个图形
                return true;
            }
            return false;

        }

        public bool GetTip(out System.Drawing.Point a,out System.Drawing.Point b)
        {
            return bmap.CanGoon(out a, out b);
        }

        private void GameOver(string content)
        {
            timer1.Stop();
            MessageBoxResult res= MessageBox.Show("游戏结束 成绩为" + content + "\n是否继续", "游戏结束", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (res == MessageBoxResult.Yes)
            {
                InitMap();
            }
            else
            {
                NavigationService.Navigate(new Uri("rank.xaml", UriKind.Relative));
            }
        }
        //多线程不会 也懒得折腾

    }
}
