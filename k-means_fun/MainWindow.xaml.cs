﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace k_means_fun {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public HashSet<Point> Points { get; set; }
        public HashSet<Cluster> Clusters { get; set; }


        public MainWindow() {
            InitializeComponent();
            Points = new HashSet<Point>();
            Clusters = new HashSet<Cluster>();
        }

        private void FileLoadButton_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "*.csv;*.txt|*.csv;*.txt", FilterIndex = 1};
            ofd.ShowDialog();

            if(ofd.FileName != null) {
                FileLable.Content = ofd.FileName;
                    StreamReader sr = new StreamReader(ofd.FileName);
                    var lines = new List<string[]>();
                    // sr.ReadLine();
                    while(!sr.EndOfStream) {
                        string[] Line = sr.ReadLine().Split(',');
                        lines.Add(Line);
                    }
                    var points = lines.ToArray();
                    foreach(var line in points)
                        if(double.TryParse(line[0].Replace(".", ","), out double x) && double.TryParse(line[1].Replace(".", ","), out double y))
                            Points.Add(new Point(x, y));
            }
        }

        private void ComputeButton_Click(object sender, RoutedEventArgs e) {
            if(!int.TryParse(NClustersTextBox.Text, out int n_clusters)) {
                MessageBox.Show("Error while reading number of clusters", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if(Points.Count == 0) {
                MessageBox.Show("No file chosen.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if(Points.Count < n_clusters) {
                MessageBox.Show("Number of clusters less then number of points.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Clusters.Clear();
            Points = (HashSet<Point>)Points.Shuffle();

            foreach(var point in Points.GetRandomElements(n_clusters)) {
                point.InCluster = true;

                Cluster cluster = new Cluster();
                cluster.Points.Add(point);
                Clusters.Add(cluster);
            }

            foreach(var p in Points) {
                if(!p.InCluster) {
                    Cluster closest = Clusters.OrderBy(cluster => cluster.GetDistanceToCentroid(p)).First();
                    closest.Points.Add(p);
                }
            }

            StringBuilder builder = new StringBuilder();
            int i = 0;
            foreach(Cluster cluster in Clusters) {
                builder.AppendLine($"===== Cluster {++i} =====");
                int j = 0;
                foreach(var point in cluster.Points)
                    builder.AppendLine($"Point {++j}: X -> {point.X}, Y -> {point.Y}");
                builder.AppendLine();
            }
            OutputTextBox.Text = builder.ToString();

            ScatterWindow scatterWindow = new ScatterWindow(Clusters);
            scatterWindow.Show();
        }
    }

    public static class Extensions {
        public static HashSet<T> GetRandomElements<T>(this HashSet<T> set, int elementsCount) {
            return set.Count >= elementsCount ? set.OrderBy(arg => Guid.NewGuid()).Take(elementsCount).ToHashSet() : null;
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list) {
            var r = new Random((int)DateTime.Now.Ticks);
            var shuffledList = list.Select(x => new { Number = r.Next(), Item = x }).OrderBy(x => x.Number).Select(x => x.Item);
            return shuffledList.ToHashSet();
        }
    }
}
