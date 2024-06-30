using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;



namespace Practice
{
    public partial class Form1 : Form
    {
        Bitmap buffer;
        Graphics graphics;
        Timer timer = new Timer();

        int FPS = 120;
        int player1y, player1x, player2y, player2x;
        int player1Score = 0, player2Score = 0;
        int ballx, bally;
        int ballRadius = 10;
        int paddleWidth = 40;
        int gateWidth = 10;
        int gateHeight = 100;
        int gatey;
        
        
        int ballSpeedX = 2, ballSpeedY = 2;
        Random random = new Random();

        public Form1()
        {
            InitializeComponent();
            pictureBox1.Width = 640;
            pictureBox1.Height = 480;
            buffer = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = Graphics.FromImage(buffer);
            pictureBox1.Image = buffer;

            timer.Interval = 1000 / FPS;
            timer.Tick += TimerCallback;
            timer.Enabled = true;

            gatey = pictureBox1.Height / 2 - gateHeight / 2;

            ResetGame();

            
        }

        private void ResetGame()
        {
            ballx = pictureBox1.Width / 2 - ballRadius;
            bally = pictureBox1.Height / 2 - ballRadius;
            ballSpeedX = random.Next(2) == 0 ? ballSpeedX : -ballSpeedX;
            ballSpeedY = random.Next(2) == 0 ? ballSpeedY : -ballSpeedY;
            player1y = pictureBox1.Height / 2;
            player2y = pictureBox1.Height / 2;
            player1x = gateWidth;
            player2x = pictureBox1.Width - paddleWidth;
            timer.Start();
        }

        private void TimerCallback(object sender, EventArgs e)
        {
            Ballxy.Text = ballx.ToString()+ " " + bally.ToString();
            Player1.Text = player1y.ToString();
            Player2.Text = player2y.ToString();
            UpdateBall();
            DrawGame();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (timer.Enabled == false) ballRadius = trackBar1.Value;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            if (timer.Enabled == false) paddleWidth = trackBar2.Value;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            if (timer.Enabled == false) gateHeight = trackBar3.Value*10;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
            trackBar1.Enabled = false;
            trackBar2.Enabled = false;
            trackBar3.Enabled = false;
            trackBar4.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ResetGame();
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            ballSpeedX = trackBar4.Value;
            ballSpeedY = trackBar4.Value;

        }

        private void UpdateBall()
        {
            ballx += ballSpeedX;
            bally += ballSpeedY;

            // Столкновение с верхней и нижней стенками
            if (bally <= 0 || bally+ballRadius*2 >= pictureBox1.Height)
                ballSpeedY = -ballSpeedY;

            // Столкновение с боковыми стенками
            if (ballx <= 0 || ballx + ballRadius*2 >= pictureBox1.Width)
            {
                ballSpeedX = -ballSpeedX;
            }

            // Столкновение с битами
            if (CheckCollision(player1y, player1x) || CheckCollision(player2y, player2x))
            {
                ballSpeedX = -ballSpeedX;
        
            }  

            // Проверка на гол
            if (GateEnter() && ballx < pictureBox1.Width / 2)
            {
                player2Score++;
                ResetGame();
            }
            if (GateEnter() && ballx > pictureBox1.Width / 2)
            {
                player1Score++;
                ResetGame();
            }
        }

        private bool CheckCollision(int paddleY, int paddleX)
        {
            return new Rectangle(ballx, bally, ballRadius*2, ballRadius*2).IntersectsWith(new Rectangle(paddleX, paddleY, paddleWidth, paddleWidth));
        }

        private bool GateEnter()
        {
            return ((ballx <= gateWidth || ballx + ballRadius >= pictureBox1.Width - gateWidth) && (bally >= gatey && bally+ballRadius*2 <= gatey+gateHeight));
        }

        private void DrawGame()
        {
            graphics.Clear(Color.White);

            //Рисуем линию раздления
            graphics.DrawLine(new Pen(Color.Black, 6), pictureBox1.Width / 2, 0, pictureBox1.Width / 2, pictureBox1.Height);

            // Рисуем биты
            graphics.FillEllipse(Brushes.Black, new Rectangle(player1x, player1y, paddleWidth, paddleWidth));
            graphics.FillEllipse(Brushes.Black, new Rectangle(player2x, player2y, paddleWidth, paddleWidth));

            // Рисуем шайбу
            graphics.FillEllipse(Brushes.Black, ballx, bally, ballRadius*2, ballRadius*2);

            // Рисуем ворота
            graphics.FillRectangle(Brushes.Gray, new Rectangle(0, gatey, gateWidth, gateHeight));
            graphics.FillRectangle(Brushes.Gray, new Rectangle(pictureBox1.Width - gateWidth, gatey, gateWidth, gateHeight));

            // Рисуем счет
            graphics.DrawString(player1Score.ToString(), new Font("Arial", 16), Brushes.Black, new Point(pictureBox1.Width / 4, 10));
            graphics.DrawString(player2Score.ToString(), new Font("Arial", 16), Brushes.Black, new Point(pictureBox1.Width * 3 / 4, 10));

            graphics.DrawRectangle(new Pen(Color.Black, 10), new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height));

            pictureBox1.Invalidate();
        }

        private void Form1KeyDown(object sender, KeyEventArgs e)
        {
            int step = 10;
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (player2y > 0) player2y -= step;
                    break;
                case Keys.Down:
                    if (player2y < pictureBox1.Height) player2y += step;
                    break;
                case Keys.Left:
                    if (player2x > pictureBox1.Width / 2) player2x -= step;
                    break;
                case Keys.Right:
                    if (player2x < pictureBox1.Width) player2x += step;
                    break;
                case Keys.Space:
                    timer.Enabled = !timer.Enabled;
                    if (timer.Enabled == false)
                    {
                        trackBar1.Enabled = true;
                        trackBar2.Enabled = true;
                        trackBar3.Enabled = true;
                        trackBar4.Enabled = true;
                        button1.Enabled = true;
                        button2.Enabled = true;
                    } else
                    {
                        trackBar1.Enabled = false;
                        trackBar2.Enabled = false;
                        trackBar3.Enabled = false;
                        trackBar4.Enabled = true;
                        button1.Enabled = false;
                        button2.Enabled = false;
                    }

                    break;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X < pictureBox1.Width / 2)
            {
                player1y = e.Y;
                player1x = e.X;
            }
        }
    }
}

