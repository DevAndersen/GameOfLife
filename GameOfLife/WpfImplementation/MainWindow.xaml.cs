using LifeLib;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace WpfImplementation
{
    public partial class MainWindow : Window
    {
        LifeGrid lifeGrid;
        Border[,] cells;
        bool autoTick = false;
        Thread autoTickThread;

        public MainWindow()
        {
            InitializeComponent();

            int width = 32;
            int height = 32;

            for (int x = 0; x < width; x++)
            {
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int y = 0; y < height; y++)
            {
                mainGrid.RowDefinitions.Add(new RowDefinition());
            }

            lifeGrid = new LifeGrid(width, height);
            cells = new Border[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Border border = new Border();
                    border.Margin = new Thickness(1);
                    border.Background = new SolidColorBrush(Colors.LightGray);
                    border.Tag = new int[] { x, y };
                    cells[x, y] = border;

                    border.MouseLeftButtonDown += (s, e) =>
                    {
                        Border b = s as Border;
                        int[] tag = b.Tag as int[];
                        bool currentStatus = lifeGrid.GetCell(tag[0], tag[1]);
                        lifeGrid.SetCell(tag[0], tag[1], !currentStatus);
                        cells[tag[0], tag[1]].Background = !currentStatus ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.LightGray);
                    };

                    Grid.SetRow(border, x);
                    Grid.SetColumn(border, y);
                    mainGrid.Children.Add(border);
                }
            }

            lifeGrid.OnUpdate += (newGrid) =>
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            cells[x, y].Background = newGrid.GetCell(x, y) ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.LightGray);
                        });
                    }
                }
            };

            autoTickThread = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        if (autoTick)
                        {
                            lifeGrid.Update();
                        }
                        int i = 250;
                        Dispatcher.Invoke(() =>
                        {
                            i = (int)sliderAutoTickRate.Value;
                        });
                        Thread.Sleep(i);
                    }
                }
                catch
                {

                }
            });
            autoTickThread.Start();
        }

        private void btnTick_Click(object sender, RoutedEventArgs e)
        {
            lifeGrid.Update();
        }

        private void btnAuto_Click(object sender, RoutedEventArgs e)
        {
            autoTick = !autoTick;
            btnAuto.Content = "Turn auto tick " + (autoTick ? "off" : "on");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            autoTickThread.Abort();
        }

        private void sliderAutoTickRate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Dispatcher.Invoke(() =>
            {
                if (lblAutoTickRate != null)
                {
                    lblAutoTickRate.Content = ((int)sliderAutoTickRate.Value).ToString();
                }
            });
        }
    }
}