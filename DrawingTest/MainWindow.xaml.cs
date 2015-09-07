using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Text;
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

namespace DrawingTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int BOTTOM_MARGIN_RECTANGLE = 200;
        const int MAX_LIVE_COUNT = 5;
        const int BLOCKS_IN_FIRST_LEVEL = 15;

        private enum PlayerCode
        {
            PlayerOne = 0,
            PlayerTwo = 1
        };

        private event Action KādsNoRekitņiemIzsaucas;

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        bool gameOver = false;
        bool hasTheBallHitASquare = false;
        bool isLiveTakenAway = false;

        int blocksInLevel = BLOCKS_IN_FIRST_LEVEL;
        int level = 1;
        
        int lives = MAX_LIVE_COUNT;
        int score = 0;

        

        PlayerCode playerNumber = PlayerCode.PlayerOne;

        DateTime startTime = DateTime.Now;
        DateTime TimerBrosStartTime = DateTime.Now;

        Ellipse circle;


        Random rnd = new Random();
        List<Rectangle> squares = new List<Rectangle>();

        Rectangle rect;
        Rectangle rect2;

        private void Rectangle_Loaded(object sender, RoutedEventArgs e)
        {
            rect = (Rectangle)sender;
            KādsNoRekitņiemIzsaucas();

        }
        private void Rectangle_Loaded2(object sender, RoutedEventArgs e)
        {
            rect2 = (Rectangle)sender;
            KādsNoRekitņiemIzsaucas();
            
        }
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            KādsNoRekitņiemIzsaucas += FunkcijaKuraDarbosiesTIkaiJaAbiRektiņi;

            ImageBrush ib = new ImageBrush();
            ib.ImageSource = new BitmapImage(new Uri("pack://application:,,,/resources/ball.png"));


            liveCounter.Text = lives.ToString();
            circle = new Ellipse();
            circle.Fill = ib;
            circle.Width = 20;
            circle.Height = 20;
            canvas.Children.Add(circle);

            Canvas.SetTop(circle, circle.Height-6000);
            Canvas.SetLeft(circle, 0);
            circle.Visibility = System.Windows.Visibility.Collapsed;

            Canvas.SetTop(notification, this.canvas.ActualHeight / 2 - notification.ActualHeight / 2);
            
            Canvas.SetTop(restart, this.canvas.ActualHeight / 2 - restart.ActualHeight / 2);
            Canvas.SetLeft(restart, this.canvas.ActualWidth / 2 - restart.ActualWidth / 2);
            

            StartFramerate();
            StartToFuckThePlayer();
            ShowTimerBrothers();
            
        }

        private void FunkcijaKuraDarbosiesTIkaiJaAbiRektiņi()
        {
            if(rect != null & rect2 != null)
                LimitPlayerTurnTime();
        }

        private async void ShowTimerBrothers()
        {
            while (true)
            {
                timer.Text = (30 - (int)((DateTime.Now - TimerBrosStartTime).TotalSeconds)).ToString();

                if (timer.Text == "0")
                {
                    MessageBox.Show("I sexually Identify as an Attack Helicopter. Ever since I was a boy I dreamed of soaring over the oilfields dropping hot sticky loads on disgusting foreigners. People say to me that a person being a helicopter is Impossible and I'm fucking retarded but I don't care, I'm beautiful. I'm having a plastic surgeon install rotary blades, 30 mm cannons and AMG-114 Hellfire missiles on my body. From now on I want you guys to call me Apache and respect my right to kill from above and kill needlessly. If you can't accept me you're a heliphobe and need to check your vehicle privilege. Thank you for being so understanding.");
                    
                }

                await Task.Delay(500);
            }
        }
        private async void LimitPlayerTurnTime()
        {
            while (true)
            {
                TurnTimer.Text = (5 - (int)((DateTime.Now - startTime).TotalSeconds)).ToString();
                if (5 - (int)((DateTime.Now - startTime).TotalSeconds) == 0)
                {
                    startTime = DateTime.Now;
                    playerNumber = playerNumber == PlayerCode.PlayerOne ? PlayerCode.PlayerTwo : PlayerCode.PlayerOne;
                }
                LimitPlayerMovement((byte)playerNumber);
                await Task.Delay(10);
            }
        }
        void CreateNewSquare()
        {
            Rectangle newRec = new Rectangle();
            newRec.Fill = Brushes.Green;
            newRec.Width = 30;
            newRec.Height = 30;

            int yPos = rnd.Next(0, (int)this.Height - BOTTOM_MARGIN_RECTANGLE);
            Canvas.SetTop(newRec, yPos);
            int xPos = rnd.Next(0, (int)this.Width - (int)newRec.Width);
            Canvas.SetLeft(newRec, xPos);
            canvas.Children.Add(newRec);
            squares.Add(newRec);
        }

        private async void StartFramerate()
        {
            
            while (true)
            {
                await Task.Delay(10); // 100fps

                Canvas.SetTop(circle, Canvas.GetTop(circle) - 8);
                Canvas.SetLeft(circle, Canvas.GetLeft(circle) + 0);

                if (Canvas.GetTop(circle) < -circle.Height && !hasTheBallHitASquare)
                {
                    if (firstTime == false)
                    {
                        if (isLiveTakenAway == false)
                        {

                            lives--;
                            isLiveTakenAway = true;
                            liveCounter.Text = lives.ToString();
                            CreateNewSquare();

                            if (lives <= 0)
                            {
                                gameOver = true;
                                restart.Visibility = System.Windows.Visibility.Visible;
                                liveCounter.Text = "0";
                                SystemSounds.Hand.Play();
                                MessageBox.Show("Tu pakāsi");
                            }

                        }
                        else
                        {
                            continue;
                        }
                    }
                }

                var hitbox = new Rect(Canvas.GetLeft(circle), Canvas.GetTop(circle), circle.Width, circle.Height);

                List<Rectangle> removedSquares = new List<Rectangle>();
                
                foreach (Rectangle square in squares)
                {
                    var candidateToTrash = new Rect(Canvas.GetLeft(square), Canvas.GetTop(square), square.Width, square.Height);
                    if (hitbox.IntersectsWith(candidateToTrash))
                    {

                        canvas.Children.Remove(square);
                        removedSquares.Add(square);
                        score++;
                        scoreDisplay.Text = score.ToString();
                        hasTheBallHitASquare = true;

                        SoundPlayer sp = new SoundPlayer(Properties.Resources.boom);
                        sp.Play();

                        this.MižinātTrīsReizes();

                    }
                }

                foreach (var square in removedSquares)
                {
                    squares.Remove(square);
                }
                if (squares.Count == 0)
                {
                    if (level != 11)
                        notification.Text = "Līmenis: " + level.ToString();

                    else
                        notification.Text = "LOHS!!!!!!";

                    await Task.Delay(10);
                    Canvas.SetLeft(notification, this.canvas.ActualWidth / 2 - notification.ActualWidth / 2);
                    await Task.Delay(1000); 
                    notification.Text = "";
                    if (level == 2)
                    {
                        blocksInLevel = 10;
                        level++;
                    }
                    else if (level == 10)
                    {
                        blocksInLevel = 1;
                        level++;
                    }
                    else
                    {
                        if (level != 1)
                        {
                            blocksInLevel--;
                        }

                        level++;
                    }

                    StartToFuckThePlayer();
                    
                    for (int i = 0; i < blocksInLevel; i++)
                    {
                        CreateNewSquare();
                        await Task.Delay(100);
                    }
                }
            }

        }
        bool firstTime = true;

        private async void StartToFuckThePlayer() 
        {


            while (true)
            {
                await Task.Delay(500);

                if (squares.Count == 0)
                    continue;

                int blockNum = rnd.Next(0, squares.Count - 1);
                int size = rnd.Next(4, 35);

                squares[blockNum].Height = size;
                squares[blockNum].Width = size;   
            }
        }

        //[DebuggerNonUserCode]
        private void LimitPlayerMovement(byte playerNumber)
        {
            if (playerNumber == (byte)PlayerCode.PlayerOne)
            {
                if (Canvas.GetLeft(rect) > this.canvas.ActualWidth / 2 - rect.Width)
                {
                    Canvas.SetLeft(rect, this.canvas.ActualWidth / 2 - rect.Width);
                }
                Canvas.SetLeft(rect2, this.canvas.ActualWidth / 2 + rect2.Width);
            }
            else
            {
                if (Canvas.GetLeft(rect2) > this.canvas.ActualWidth / 2 + rect2.Width)
                {
                    Canvas.SetLeft(rect2, this.canvas.ActualWidth / 2 + rect2.Width);
                }
                Canvas.SetLeft(rect, this.canvas.ActualWidth / 2 - rect.Width);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                Canvas.SetLeft(rect, Canvas.GetLeft(rect) - 10);

                if (Canvas.GetLeft(rect) < 0)
                {
                    imageBrush.Viewport = new Rect(imageBrush.Viewport.X - 200, 0, 9000, 600);
                    //imageBrush.Viewport.Left -= 200.0;
                }
            }
            else if (e.Key == Key.Right)
            {
                Canvas.SetLeft(rect, Canvas.GetLeft(rect) + 10);

                if (Canvas.GetLeft(rect) + rect.Width > this.Width)
                {
                    imageBrush.Viewport = new Rect(imageBrush.Viewport.X + 200, 0, 9000, 600);
                    //imageBrush.Viewport.Left -= 200.0;
                }
            }
            if (e.Key == Key.A)
            {
                Canvas.SetLeft(rect2, Canvas.GetLeft(rect2) - 10);

                if (Canvas.GetLeft(rect2) < 0)
                {
                    imageBrush.Viewport = new Rect(imageBrush.Viewport.X - 200, 0, 9000, 600);
                    //imageBrush.Viewport.Left -= 200.0;
                }
            }
            else if (e.Key == Key.D)
            {
                Canvas.SetLeft(rect2, Canvas.GetLeft(rect2) + 10);

                if (Canvas.GetLeft(rect2) + rect2.Width > this.Width)
                {
                    imageBrush.Viewport = new Rect(imageBrush.Viewport.X + 200, 0, 9000, 600);
                    //imageBrush.Viewport.Left -= 200.0;
                }
            }
            //else if (e.Key == Key.Enter)
            //{
            //    //this.Midžināt();
            //}
            else if (e.Key == Key.Space)
            {
                
                if (firstTime == true)
                {
                            
                    circle.Visibility = System.Windows.Visibility.Visible;
                }
                if(gameOver == false && Canvas.GetTop(circle) < 0)
                {
                    if (playerNumber == PlayerCode.PlayerOne)
                    {
                        Canvas.SetTop(circle, Canvas.GetTop(rect) - rect.Height + circle.Height / 2);
                        Canvas.SetLeft(circle, Canvas.GetLeft(rect) + rect.Width / 2 - circle.Width / 2);
                    }
                    else
                    {
                        Canvas.SetTop(circle, Canvas.GetTop(rect2) - rect2.Height + circle.Height / 2);
                        Canvas.SetLeft(circle, Canvas.GetLeft(rect2) + rect2. Width / 2 - circle.Width / 2);
                    }
                    hasTheBallHitASquare = false;
                    isLiveTakenAway = false;
                }

                firstTime = false;

                        
            } 
        }
        // nokomentēts, jo man nestrādā.
        //private async void Midžināt()
        //{
        //    return;

        //    //Velleman.Kits.K8090Board b = new Velleman.Kits.K8090Board("COM3");
        //    //b.Connect();
        //    //b.SwitchRelayOn(0xC0);
        //    //b.Disconnect();

        //    await Task.Delay(180);


        //    //b.Connect();
        //    //b.SwitchRelayOff(0xC0);
        //    //b.Disconnect();
        //}
        private async void MižinātTrīsReizes()
        {
            for (int i = 2; i != 0; i--)
            {
                await Task.Delay(500);
                SoundPlayer sp = new SoundPlayer(Properties.Resources.boom);
                sp.Play();
                //Midžināt();
            }

        
        }

        private void _clearTargets()
        {
            foreach (var square in squares)
            {
                canvas.Children.Remove(square);
            }
            squares.Clear();
        }

        private async void restart_Click(object sender, RoutedEventArgs e)
        {
            restart.Visibility = System.Windows.Visibility.Collapsed;
            this._clearTargets();
            await Task.Delay(25);
            startTime = DateTime.Now;
            TimerBrosStartTime = DateTime.Now;
            blocksInLevel = BLOCKS_IN_FIRST_LEVEL;
            score = 0;
            lives = MAX_LIVE_COUNT;
            level = 1;
            gameOver = false;

        }

        
    }
}
