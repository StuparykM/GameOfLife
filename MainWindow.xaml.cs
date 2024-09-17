using System.Diagnostics;
using System.Formats.Asn1;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GameOfLife
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Creates timer for the main window to use.
        private DispatcherTimer _timer = new();

        //bool used to determine whether an animation should "go"
        private bool _go = false;
        public MainWindow()
        {
            //Method that initializes all elements in xaml
            InitializeComponent();

            //Timer settings, using Step method as its Tick
            _timer.Tick += Step;
            _timer.Interval = TimeSpan.FromMilliseconds(100);

            //Generates a button for each cell of grid which is established by the border
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    Button cell = new Button();

                    cell.BorderThickness = new Thickness(1);

                    cell.BorderBrush = new SolidColorBrush(Colors.Black);
                    cell.Background = new SolidColorBrush(Colors.LightGray);

                    cell.Click += cellClick;

                    

                    Grid.SetRow(cell, row);
                    Grid.SetColumn(cell, col);

                    cellGrid.Children.Add(cell);
                }
            }
        }

        private void cellClick(object sender, RoutedEventArgs e)
        {
            
            //Toggles cell background
            if (SameBackground(new SolidColorBrush(Colors.LightGray), (SolidColorBrush)((Button)sender).Background))
            {
                ((Button)sender).Background = new SolidColorBrush(Colors.Yellow);   
            }
            else
            {
                ((Button)sender).Background = new SolidColorBrush(Colors.LightGray);
            }
        }
        
        private bool SameBackground(SolidColorBrush brushOne , SolidColorBrush brushTwo)
        {
            
            //Checks if two solid color brushes are the same by checking their values.
            if (brushOne.Opacity != brushTwo.Opacity)
            {
                return false;
            }

            if (brushOne.Color.A != brushTwo.Color.A)
            {
                return false;
            }

            if (brushOne.Color.R != brushTwo.Color.R)
            {
                return false;
            }

            if (brushOne.Color.G != brushTwo.Color.G)
            {
                return false;
            }

            if (brushOne.Color.B != brushTwo.Color.B)
            {
                return false;
            }

            return true;
        }

        private void Step(object? sender, EventArgs? e)
        {
            //Plays one frame

            //Snapshot of the current state of the grid
            bool[,] cellData = new bool[10,10];


            foreach (Button cell in cellGrid.Children.OfType<Button>())
            {
                int row = Grid.GetRow(cell);
                int col = Grid.GetColumn(cell);

                if (SameBackground((SolidColorBrush)cell.Background, new SolidColorBrush(Colors.Yellow)))
                {
                    cellData[row, col] = true;
                }
                else
                {
                    cellData[row, col] = false;
                }

            }

            //Counts neighbours and applies logic established in methods
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    int count = GetNeighbours(row, col, cellData);
                    Debug.WriteLine(count);
                    ImplementRules(count, row, col, cellGrid);
                }
            }



        }

        void ImplementRules(int count, int row, int col, Grid cellGrid)
        {
            //implements rules of Conway's Game of Life, using the origin cell (row, col) and the count of its live neighbours.
            Button cell = cellGrid.Children.Cast<Button>().First(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == col);
            bool isAlive = SameBackground((SolidColorBrush)cell.Background, new SolidColorBrush(Colors.Yellow));

            if (isAlive && (count < 2 || count > 3))
            {
                cell.Background = new SolidColorBrush(Colors.LightGray);
            }
            else if (!isAlive && count == 3)
            {
                cell.Background = new SolidColorBrush(Colors.Yellow);
            }
        }

        public int GetNeighbours(int row, int col, bool[,] cellData)
        {
            int count = 0;

            //Counts live neighbours
            for(int ixRow = row-1; ixRow < row+2; ixRow++)
            {
                for( int ixCol = col-1; ixCol < col+2; ixCol++)
                {
                    int nRow;
                    int nCol;
                    //If indexes go out of bounds they wrap inbounds
                    if (ixRow < 0)
                    {
                        nRow = 9;
                    }
                    else if (ixRow > 9)
                    {
                        nRow = 0;
                    }
                    else {
                        nRow = ixRow;
                    }

                    if(ixCol < 0)
                    {
                        nCol = 9;
                    }
                    else if (ixCol > 9)
                    {
                        nCol = 0;
                    }
                    else
                    {
                        nCol = ixCol;
                    }

                    //ixVariable will continue to count, nVariable allows us to keep the count inbounds. Without nVariable, will cause infinite loop because we are constantly setting ixVariable into bounds as it counts out of bounds.
                    if (!(nRow == row && nCol == col))
                    {
                        if(cellData[nRow, nCol])
                        {
                            count++;
                        }
                    }
                }
            }

            return count;
        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            //Resets Board and cell.background colors to LightGray
            foreach(Button cell in cellGrid.Children.OfType<Button>()) 
            { 
               cell.Background = new SolidColorBrush(Colors.LightGray);
            }

            //Stops animation and resets play button content
            _go = false;
            _timer.Stop();
            playButton.Content = "Play";

        }

        private void Play(object sender, RoutedEventArgs e)
        {

            //Toggles Go
               
            _go = !_go;

            //Starts or stops timer based on Go
            if(_go)
            {
                playButton.Content = "Stop";
                _timer.Start();
            }
            else
            {
                playButton.Content = "Play";
                _timer.Stop();
            }


        }
        //Buttons needs specific parameters different than what the timer needs so an additional method is created.
        private void StepButton(object sender, RoutedEventArgs e)
        {
            Step(sender, e);
        }
    }
}
