using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TapTheTiles
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        class TileDocks
        {
            public DockPanel dock;
            public Rectangle[] tiles = new Rectangle[4];
            public bool isClicked = true;

            /// <summary>
            /// Initialize the dock
            /// </summary>
            /// <param name="dock">The dock that holds the tiles</param>
            /// <param name="tiles">The titles that are in the dock</param>
            public TileDocks(DockPanel dock, params Rectangle[] tiles)
            {
                this.dock = dock;

                for (int i = 0; i < 4; i++)
                {
                    this.tiles[i] = tiles[i];
                }
            }
        }
        TileDocks[] tileDocks = new TileDocks[4];

        public static int s { get; set; }
        int score;
        DispatcherTimer animationTimer = new DispatcherTimer(DispatcherPriority.Send);
        Random randomTileSelector = new Random();

        /// <summary>
        /// Initializes the main windows
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            InitializeGameComponents();
        }

        
        /// <summary>
        /// Initalize the component - Equivalent to onRestart()
        /// </summary>
        void InitializeGameComponents()
        {
            tileDocks[0] = new TileDocks(Dock0, Tile00, Tile01, Tile02, Tile03);
            tileDocks[1] = new TileDocks(Dock1, Tile10, Tile11, Tile12, Tile13);
            tileDocks[2] = new TileDocks(Dock2, Tile20, Tile21, Tile22, Tile23);
            tileDocks[3] = new TileDocks(Dock3, Tile30, Tile31, Tile32, Tile33);

            animationTimer.Interval = TimeSpan.FromMilliseconds(10);
            animationTimer.Tick += animationTimer_Tick;
        }

        /** Main component of this Game - It is the game engine
         * 
         *  Move all the dockTiles by a specified speed
         *  If the tiles are clicked unclick it
         *  
         *  If the tile is not clicked end the game
         *  If the window is notActive then Pause the game
         */ 
        void animationTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < 4; i++)
            {
                var margin = tileDocks[i].dock.Margin;
                margin.Top = (margin.Top + (score / 20 + 2)) % 400;
                if (margin.Top > 5 && margin.Top < 10)
                {
                    if (tileDocks[i].isClicked == true)
                    {
                        tileDocks[i].isClicked = false;
                        tileDocks[i].tiles[randomTileSelector.Next() % 4].Fill = Brushes.Black;
                    }
                }
                tileDocks[i].dock.Margin = margin;
            }

            for (int i = 0; i < 4; i++)
            {
                if (tileDocks[i].dock.Margin.Top < 5 && tileDocks[i].isClicked == false) { GameOver(); }

                if(this.IsActive == false)
                {
                    animationTimer.Stop();
                    PlayOrPause.Content = "Resume";
                }
            }
        }

        /** It will set the GameOver display details
         * 
         *  Stop the timer
         *  Prevent the PlayOrPause button from being used by setting its contents to null
         *  Update the Instructions
         */ 
        void GameOver()
        {
            Instructions.Content = "Sorry!! You loose... :(";
            animationTimer.Stop();
            PlayOrPause.Content = "";
        }
        
        /** This will decide what to do when a tile is touched or clicked
         * 
         *  This will check whether the game is being played or not
         *  If so,
         *      * It will get the sender
         *      * It will check whether that tile is black or not
         *      * If so,
         *          * Updates the score
         *          * Makes the tile white
         *          * sets that the tile dock had a vaild click
         *      * Otherwise
         *          * Ends the game
         */ 
        private void Tile_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (animationTimer.IsEnabled == true)
            {
                Rectangle clickedTile = sender as Rectangle;

                if (clickedTile.Fill == Brushes.Black)
                {
                    Score.Content = "Score : " + (++score).ToString("000");
                    clickedTile.Fill = Brushes.White;
                    tileDocks[Convert.ToInt32(clickedTile.Name[4].ToString())].isClicked = true;
                }
                else
                {
                    GameOver();
                }
            }
        }

        /** Performs actions according to the buttons clicked
         * 
         */ 
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Content.ToString())
            {
                case "New Game":
                    new MainWindow().Show();
                    this.Close();
                    break;
                case "Pause":
                    (sender as Button).Content = "Resume";
                    animationTimer.Stop();
                    break;
                case "Start":
                case "Resume":
                    (sender as Button).Content = "Pause";
                    animationTimer.Start();
                    break;
            }
        }

        /** This will decide what to do when a key is pressed
         * 
         *  Checks whether the game is being played or not
         *  
         *  Gets the key pressed and converts it to number
         *  checks whether a valid tile was selected
         *  If so,
         *      * It will get the sender
         *      * It will check every tile in the tiledock to find whether that tile is black or not
         *      * If so,
         *          * Updates the score
         *          * Makes the tile white
         *          * sets that the tile dock had a vaild click
         *          * sets that a black tile was pressed
         *          * winds up this loop
         *      * Otherwise
         *          * Ends the game
         */
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (animationTimer.IsEnabled == true)
            {
                int selectedCell = -1;
                bool selectionMatch = false;

                switch (e.Key)
                {
                    case Key.NumPad1:
                    case Key.D1:
                        selectedCell = 0;
                        break;
                    case Key.NumPad2:
                    case Key.D2:
                        selectedCell = 1;
                        break;
                    case Key.NumPad3:
                    case Key.D3:
                        selectedCell = 2;
                        break;
                    case Key.NumPad4:
                    case Key.D4:
                        selectedCell = 3;
                        break;
                }

                if (selectedCell != -1)
                {
                    for (int i = 3; i >= 0; i--)
                    {
                        if (tileDocks[i].tiles[selectedCell].Fill == Brushes.Black)
                        {
                            Score.Content = "Score : " + (++score).ToString("000");
                            tileDocks[i].tiles[selectedCell].Fill = Brushes.White;
                            tileDocks[i].isClicked = true;
                            selectionMatch = true;
                            break;
                        }

                    }

                    if (!selectionMatch)
                    {
                        GameOver();
                    }
                }
            }
        }
    }
}
