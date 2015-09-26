using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Net;
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
    public partial class MainWindow : Window
    {
        #region Constants

        const int BOTTOM_MARGIN_RECTANGLE = 200;
        const int MAX_LIVE_COUNT = 5;
        const int BLOCKS_IN_FIRST_LEVEL = 15;

        #endregion

        #region Private members

        private int _clientNumber;
        PlayerCode _playerNumber = PlayerCode.PlayerOne;

        bool _gameOver = false;
        bool _hasTheBallHitASquare = false;
        bool _isLiveTakenAway = false;
        bool _firstTime = true;

        int _blocksInLevel = BLOCKS_IN_FIRST_LEVEL;
        int _level = 1;

        int _lives = MAX_LIVE_COUNT;
        int _score = 0;

        DateTime _startTime = DateTime.Now;
        DateTime _timerStartTime = DateTime.Now;

        Ellipse _circle;

        Random _rnd = new Random();
        List<Rectangle> _squares = new List<Rectangle>();

        Rectangle _rect;
        Rectangle _rect2;

        #endregion

        #region Events

        private event Action KādsNoRekitņiemIzsaucas;

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            //this.Loaded += MainWindow_Loaded;
        }

        #region Private methods

        #region Event Handlers

        private void Rectangle_Loaded(object sender, RoutedEventArgs e)
        {
            _rect = (Rectangle)sender;

        }

        private void Rectangle_Loaded2(object sender, RoutedEventArgs e)
        {

            _rect2 = (Rectangle)sender;

        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this._clientNumber = _rnd.Next(1, 20000000);
            this.ListenToServer();

            KādsNoRekitņiemIzsaucas += FunkcijaKuraDarbosiesTIkaiJaAbiRektiņi;

            ImageBrush ib = new ImageBrush();
            ib.ImageSource = new BitmapImage(new Uri("pack://application:,,,/resources/ball.png"));


            liveCounter.Text = _lives.ToString();
            _circle = new Ellipse();
            _circle.Fill = ib;
            _circle.Width = 20;
            _circle.Height = 20;
            canvas.Children.Add(_circle);

            Canvas.SetTop(_circle, _circle.Height - 6000);
            Canvas.SetLeft(_circle, 0);
            _circle.Visibility = System.Windows.Visibility.Collapsed;

            Canvas.SetTop(notification, this.canvas.ActualHeight / 2 - notification.ActualHeight / 2);

            Canvas.SetTop(restart, this.canvas.ActualHeight / 2 - restart.ActualHeight / 2);
            Canvas.SetLeft(restart, this.canvas.ActualWidth / 2 - restart.ActualWidth / 2);


            StartFramerate();
            StartToFuckThePlayer();
            ShowTimerBrothers();
            KādsNoRekitņiemIzsaucas();
        }

        // TODO: parsaukt XAMLā
        private async void restart_Click(object sender, RoutedEventArgs e)
        {
            restart.Visibility = System.Windows.Visibility.Collapsed;
            this.ClearTargets();
            await Task.Delay(25);
            _startTime = DateTime.Now;
            _timerStartTime = DateTime.Now;
            _blocksInLevel = BLOCKS_IN_FIRST_LEVEL;
            _score = 0;
            _lives = MAX_LIVE_COUNT;
            _level = 1;
            _gameOver = false;

        }

        #endregion

        #region Timed events

        private async void ShowTimerBrothers()
        {
            while (true)
            {
                timer.Text = (30 - (int)((DateTime.Now - _timerStartTime).TotalSeconds)).ToString();

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
                TurnTimer.Text = (5 - (int)((DateTime.Now - _startTime).TotalSeconds)).ToString();
                if (5 - (int)((DateTime.Now - _startTime).TotalSeconds) == 0)
                {
                    _startTime = DateTime.Now;
                    _playerNumber = _playerNumber == PlayerCode.PlayerOne ? PlayerCode.PlayerTwo : PlayerCode.PlayerOne;
                }
                LimitPlayerMovement((byte)_playerNumber);
                await Task.Delay(10);
            }
        }

        private async void ListenToServer()
        {
            while (true)
            {
                WebClient wc = new WebClient();

                string s = wc.DownloadString("http://localhost:20160/spele/?client=" + this._clientNumber);

                if (s == "labi")
                {
                    MessageBox.Show("aaa");
                }

                await Task.Delay(40);
            }
        }

        private async void StartFramerate()
        {

            while (true)
            {
                await Task.Delay(10); // 100fps

                Canvas.SetTop(_circle, Canvas.GetTop(_circle) - 8);
                Canvas.SetLeft(_circle, Canvas.GetLeft(_circle) + 0);

                if (Canvas.GetTop(_circle) < -_circle.Height && !_hasTheBallHitASquare)
                {
                    if (_firstTime == false)
                    {
                        if (_isLiveTakenAway == false)
                        {

                            _lives--;
                            _isLiveTakenAway = true;
                            liveCounter.Text = _lives.ToString();
                            CreateNewSquare();

                            if (_lives <= 0)
                            {
                                _gameOver = true;
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

                var hitbox = new Rect(Canvas.GetLeft(_circle), Canvas.GetTop(_circle), _circle.Width, _circle.Height);

                List<Rectangle> removedSquares = new List<Rectangle>();

                foreach (Rectangle square in _squares)
                {
                    var candidateToTrash = new Rect(Canvas.GetLeft(square), Canvas.GetTop(square), square.Width, square.Height);
                    if (hitbox.IntersectsWith(candidateToTrash))
                    {

                        canvas.Children.Remove(square);
                        removedSquares.Add(square);
                        _score++;
                        scoreDisplay.Text = _score.ToString();
                        _hasTheBallHitASquare = true;

                        SoundPlayer sp = new SoundPlayer(Properties.Resources.boom);
                        sp.Play();

                        this.MižinātTrīsReizes();

                    }
                }

                foreach (var square in removedSquares)
                {
                    _squares.Remove(square);
                }
                if (_squares.Count == 0)
                {
                    if (_level != 11)
                        notification.Text = "Līmenis: " + _level.ToString();

                    else
                        notification.Text = "LOHS!!!!!!";

                    await Task.Delay(10);
                    Canvas.SetLeft(notification, this.canvas.ActualWidth / 2 - notification.ActualWidth / 2);
                    await Task.Delay(1000);
                    notification.Text = "";
                    if (_level == 2)
                    {
                        _blocksInLevel = 10;
                        _level++;
                    }
                    else if (_level == 10)
                    {
                        _blocksInLevel = 1;
                        _level++;
                    }
                    else
                    {
                        if (_level != 1)
                        {
                            _blocksInLevel--;
                        }

                        _level++;
                    }

                    StartToFuckThePlayer();

                    for (int i = 0; i < _blocksInLevel; i++)
                    {
                        CreateNewSquare();
                        await Task.Delay(100);
                    }
                }
            }

        }

        private async void StartToFuckThePlayer()
        {


            while (true)
            {
                await Task.Delay(500);

                if (_squares.Count == 0)
                    continue;

                int blockNum = _rnd.Next(0, _squares.Count - 1);
                int size = _rnd.Next(4, 35);

                _squares[blockNum].Height = size;
                _squares[blockNum].Width = size;
            }
        }

        #endregion

        private void FunkcijaKuraDarbosiesTIkaiJaAbiRektiņi()
        {
            if(_rect != null & _rect2 != null)
                LimitPlayerTurnTime();
        }

        private void CreateNewSquare()
        {
            Rectangle newRec = new Rectangle();
            newRec.Fill = Brushes.Green;
            newRec.Width = 30;
            newRec.Height = 30;

            int yPos = _rnd.Next(0, (int)this.Height - BOTTOM_MARGIN_RECTANGLE);
            Canvas.SetTop(newRec, yPos);
            int xPos = _rnd.Next(0, (int)this.Width - (int)newRec.Width);
            Canvas.SetLeft(newRec, xPos);
            canvas.Children.Add(newRec);
            _squares.Add(newRec);
        }

        private void LimitPlayerMovement(byte playerNumber)
        {
            if (playerNumber == (byte)PlayerCode.PlayerOne)
            {
                if (Canvas.GetLeft(_rect) > this.canvas.ActualWidth / 2 - _rect.Width)
                {
                    Canvas.SetLeft(_rect, this.canvas.ActualWidth / 2 - _rect.Width);
                }
                Canvas.SetLeft(_rect2, this.canvas.ActualWidth / 2);
            }
            else
            {
                if (Canvas.GetLeft(_rect2) < this.canvas.ActualWidth / 2)
                {
                    Canvas.SetLeft(_rect2, this.canvas.ActualWidth / 2 );
                }
                Canvas.SetLeft(_rect, this.canvas.ActualWidth / 2 - _rect.Width);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                Canvas.SetLeft(_rect, Canvas.GetLeft(_rect) - 10);

                if (Canvas.GetLeft(_rect) < 0)
                {
                    imageBrush.Viewport = new Rect(imageBrush.Viewport.X - 200, 0, 9000, 600);
                    //imageBrush.Viewport.Left -= 200.0;
                }
            }
            else if (e.Key == Key.Right)
            {
                WebClient wc = new WebClient();

                wc.DownloadString("http://localhost:20160/spele/?client=" + this._clientNumber + "&msg=labi");

                Canvas.SetLeft(_rect, Canvas.GetLeft(_rect) + 10);

                if (Canvas.GetLeft(_rect) + _rect.Width > this.Width)
                {
                    imageBrush.Viewport = new Rect(imageBrush.Viewport.X + 200, 0, 9000, 600);
                    //imageBrush.Viewport.Left -= 200.0;
                }
            }
            if (e.Key == Key.A)
            {
                Canvas.SetLeft(_rect2, Canvas.GetLeft(_rect2) - 10);

                if (Canvas.GetLeft(_rect2) < 0)
                {
                    imageBrush.Viewport = new Rect(imageBrush.Viewport.X - 200, 0, 9000, 600);
                    //imageBrush.Viewport.Left -= 200.0;
                }
            }
            else if (e.Key == Key.D)
            {
                Canvas.SetLeft(_rect2, Canvas.GetLeft(_rect2) + 10);

                if (Canvas.GetLeft(_rect2) + _rect2.Width > this.Width)
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
                
                if (_firstTime == true)
                {
                            
                    _circle.Visibility = System.Windows.Visibility.Visible;
                }
                if(_gameOver == false && Canvas.GetTop(_circle) < 0)
                {
                    if (_playerNumber == PlayerCode.PlayerOne)
                    {
                        Canvas.SetTop(_circle, Canvas.GetTop(_rect) - _rect.Height + _circle.Height / 2);
                        Canvas.SetLeft(_circle, Canvas.GetLeft(_rect) + _rect.Width / 2 - _circle.Width / 2);
                    }
                    else
                    {
                        Canvas.SetTop(_circle, Canvas.GetTop(_rect2) - _rect2.Height + _circle.Height / 2);
                        Canvas.SetLeft(_circle, Canvas.GetLeft(_rect2) + _rect2. Width / 2 - _circle.Width / 2);
                    }
                    _hasTheBallHitASquare = false;
                    _isLiveTakenAway = false;
                }

                _firstTime = false;

                        
            } 
        }

        // nokomentēts, jo nav API atbalsts
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

        private void ClearTargets()
        {
            foreach (var square in _squares)
            {
                canvas.Children.Remove(square);
            }
            _squares.Clear();
        }

        #endregion

        #region Enums

        private enum PlayerCode
        {
            PlayerOne = 0,
            PlayerTwo = 1
        };

        #endregion

        // TODO: japabeidz menu
        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Canvas.SetLeft((Grid)sender, (canvas.ActualWidth - ((Grid)sender).ActualWidth) / 2);
            Canvas.SetTop((Grid)sender, (canvas.ActualHeight - ((Grid)sender).ActualHeight) / 2);
        }

        private void ExitGame(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void StartNewGame(object sender, RoutedEventArgs e)
        {
            MainWindow_Loaded(null, null);
            menuGrid.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
