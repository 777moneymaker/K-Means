using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace k_means_fun {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public HashSet<Point> Points { get; set; }
        public List<Cluster> Clusters { get; set; }

        public MainWindow() {
            InitializeComponent();
            Points = new HashSet<Point>();
            Clusters = new List<Cluster>();
        }

        private void FileLoadButton_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "*.csv;*.txt|*.csv;*.txt", FilterIndex = 1};
            ofd.ShowDialog();

            if(ofd.FileName != string.Empty) {
                FileLable.Content = ofd.FileName;
                
                Points.Clear();
                using(StreamReader sr = new StreamReader(ofd.FileName)) {
                    var lines = new List<string[]>();
                    // sr.ReadLine();
                    while(!sr.EndOfStream) {
                        string[] Line = sr.ReadLine().Split(',');
                        lines.Add(Line);
                    }
                    
                    var points = lines.ToArray();
                    points.ToList().ForEach(line => {
                        bool x_Res = double.TryParse(line[0].Replace(".", ","), out double x);
                        bool y_Res = double.TryParse(line[1].Replace(".", ","), out double y);
                        if(x_Res && y_Res)
                            Points.Add(new Point(x, y));
                    });  
                }
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
            Points = Points.Shuffle().ToHashSet();

            Points.GetRandomElements(n_clusters).ToList().ForEach(point => {
                var cluster = new Cluster();
                cluster.Points.Add(point);
                cluster.RearrangeCentroid();
                Clusters.Add(cluster);
            });
            Points.ToList().ForEach(point => {
                if(!Clusters.Any(cluster => cluster.Points.Contains(point))) {
                    Cluster closest = Clusters.OrderBy(cluster => cluster.GetDistanceToCentroid(point)).First();
                    closest.Points.Add(point);
                }
            });
            Clusters.ForEach(cluster => cluster.RearrangeCentroid());

            List<Cluster> prevClusters;
            uint k = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            do {
                prevClusters = new List<Cluster>(Clusters.Select(c => ((ICloneable)c).Clone() as Cluster));
                
                Clusters.ForEach(c => c.Points.Clear());
                Points.ToList().ForEach(p => {
                    var closest = Clusters.OrderBy(cluster => cluster.GetDistanceToCentroid(p)).First();
                    closest.Points.Add(p);
                });
                Clusters.ForEach(c => c.RearrangeCentroid());
                k++;
            } while(!Clusters.SequenceEqual(prevClusters));
            sw.Stop();
            MessageBox.Show($"Iterations: {k}. Time: {sw.ElapsedMilliseconds:0.00}[ms]");

            uint i = 0;
            StringBuilder builder = new StringBuilder();
            Clusters.ForEach(c => {
                builder.AppendLine($"===== Cluster {++i} =====");
                uint j = 0;
                c.Points.ForEach(p => {
                    builder.AppendLine($"Point {++j}: {p}");
                });
                builder.AppendLine();
            });
            OutputTextBox.Text = builder.ToString();

            ScatterWindow scatterWindow = new ScatterWindow(Clusters.ToHashSet());
            scatterWindow.Show();
        }
    }

    public static class EnumerableExtensions {
        public static IEnumerable<T> GetRandomElements<T>(this IEnumerable<T> enumerable, int elementsCount) {
            return enumerable.Count() >= elementsCount ? enumerable.OrderBy(arg => Guid.NewGuid()).Take(elementsCount) : null;
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable) {
            var r = new Random((int)DateTime.Now.Ticks);
            var shuffledList = enumerable.Select(x => new { Number = r.Next(), Item = x }).OrderBy(x => x.Number).Select(x => x.Item);
            return shuffledList;
        }
    }
}
