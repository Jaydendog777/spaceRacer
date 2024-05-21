using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Media;

namespace spaceRacer
{
    public partial class Form1 : Form
    {
        Rectangle p1 = new Rectangle(125, 350, 12, 28);
        Rectangle p2 = new Rectangle(450, 350, 12, 28);
        Rectangle top = new Rectangle(0, 0, 600, 1);
        int playerSpeed = 3;

        List<Rectangle> ballList = new List<Rectangle>();
        List<int> ballSpeeds = new List<int>();
        List<int> ballSizeX = new List<int>();
        List<int> ballSizeY = new List<int>();

        int p1Score = 0;
        int p2Score = 0;

        bool wPressed = false;
        bool sPressed = false;
        bool upPressed = false;
        bool downPressed = false;

        SolidBrush whiteBrush = new SolidBrush(Color.White);

        Random randGen = new Random();
        int randValue = 0;

        SoundPlayer kaboom = new SoundPlayer(Properties.Resources.Sonic_Boom_SoundBible_com_876321507);
        public Form1()
        {
            InitializeComponent();
        }

        public void InitializeGame()
        {
            titleLabel.Text = "";
            subtitleLabel.Text = "";
            winLabel.Text = "";
            ballList.Clear();
            ballSpeeds.Clear();
            p1.Y = 350;
            p2.Y = 350;
            p1Score = 0;
            p2Score = 0;
            p1ScoreLabel.Visible = p2ScoreLabel.Visible = true;
            p1ScoreLabel.Text = $"{p1Score}";
            p2ScoreLabel.Text = $"{p2Score}";
            p1ScoreLabel.Refresh();
            p2ScoreLabel.Refresh();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    wPressed = true;
                    break;
                case Keys.S:
                    sPressed = true;
                    break;
                case Keys.Up:
                    upPressed = true;
                    break;
                case Keys.Down:
                    downPressed = true;
                    break;
                case Keys.Escape:
                    if (gameTimer.Enabled == false)
                    {
                        Application.Exit();
                    }
                    break;
                case Keys.Space:
                    if (gameTimer.Enabled == false)
                    {
                        gameTimer.Enabled = true;
                        InitializeGame();
                    }
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    wPressed = false;
                    break;
                case Keys.S:
                    sPressed = false;
                    break;
                case Keys.Up:
                    upPressed = false;
                    break;
                case Keys.Down:
                    downPressed = false;
                    break;
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (gameTimer.Enabled == false)
            {
                titleLabel.Text = "Space Racer";
                subtitleLabel.Text = "Press SPACE to begin or ESC to exit";
            }
            else if (gameTimer.Enabled == true)
            {
                e.Graphics.FillRectangle(whiteBrush, p1);
                e.Graphics.FillRectangle(whiteBrush, p2);

                for (int i = 0; i < ballList.Count(); i++)
                {
                    e.Graphics.FillRectangle(whiteBrush, ballList[i]);
                }
            }
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            keyCheck();

            //Moves astroids
            for (int i = 0; i < ballList.Count(); i++)
            {
                int x = ballList[i].X + ballSpeeds[i];
                ballList[i] = new Rectangle(x, ballList[i].Y, ballList[i].Width, ballList[i].Height);
            }

            //Spawns astroids
            randValue = randGen.Next(0, 2);
            if (randValue == 1)
            {
                randValue = randGen.Next(0, 100);
                if (randValue < 12)
                {
                    int ballsSizeX = randGen.Next(19, 25);
                    int ballsSizeY = randGen.Next(3, 6);
                    randValue = randGen.Next(10, this.Height - 34 * 2);

                    Rectangle ball = new Rectangle(0, randValue, ballsSizeX, ballsSizeY);
                    ballList.Add(ball);
                    ballSpeeds.Add(randGen.Next(4, 10));
                }
            }
            else
            {
                randValue = randGen.Next(0, 100);
                if (randValue < 12)
                {
                    int ballsSizeX = randGen.Next(15, 35);
                    int ballsSizeY = randGen.Next(1, 8);
                    randValue = randGen.Next(10, this.Height - 34 * 2);

                    Rectangle ball = new Rectangle(this.Width, randValue, ballsSizeX, ballsSizeY);
                    ballList.Add(ball);
                    ballSpeeds.Add(randGen.Next(-10, -4));
                }
            }

            //Player 1 reaches top
            if (p1.IntersectsWith(top))
            {
                p1Score++;
                p1ScoreLabel.Text = $"{p1Score}";
                p1ScoreLabel.Refresh();
                if (p1Score <= 2)
                {
                    p1.Y = 350;
                    p1ScoreLabel.Text = $"{p1Score}";
                    p1ScoreLabel.Refresh();
                }
                else
                {
                    int win = 1;
                    gameOver(win);
                }
            }

            //Player 2 reaches top
            if (p2.IntersectsWith(top))
            {
                p2Score++;
                p2ScoreLabel.Text = $"{p2Score}";
                p2ScoreLabel.Refresh();
                if (p2Score <= 2)
                {
                    p2.Y = 350;
                    p2ScoreLabel.Text = $"{p2Score}";
                    p2ScoreLabel.Refresh();
                }
                else
                {
                    int win = 2;
                    gameOver(win);
                }
            }

            //Player hits astroid
            for (int i = 0; i < ballList.Count(); i++)
            {
                if (ballList[i].IntersectsWith(p1))
                {
                    kaboom.Play();
                    p1.Y = 350;
                    ballList.RemoveAt(i);
                    ballSpeeds.RemoveAt(i);
                    break;
                }

                else if (ballList[i].IntersectsWith(p2))
                {
                    kaboom.Play();
                    p2.Y = 350;
                    ballList.RemoveAt(i);
                    ballSpeeds.RemoveAt(i);
                    break;
                }
            }

            Refresh();
        }


        public void gameOver(int win)
        {
            InitializeGame();
            p1ScoreLabel.Visible = p2ScoreLabel.Visible = false;
            winLabel.Text = $"Player {win} wins!";
            gameTimer.Enabled = false;
        }
        public void keyCheck()
        {
            if (wPressed == true)
            {
                p1.Y -= playerSpeed;
            }
            if (sPressed == true && p1.Y < this.Height - p1.Height)
            {
                p1.Y += playerSpeed;
            }

            if (upPressed == true)
            {
                p2.Y -= playerSpeed;
            }
            if (downPressed == true && p2.Y < this.Height - p2.Height)
            {
                p2.Y += playerSpeed;
            }
        }
    }
}
