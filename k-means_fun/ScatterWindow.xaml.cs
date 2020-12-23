using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace k_means_fun {
    /// <summary>
    /// Interaction logic for ScatterWindow.xaml
    /// </summary>
    public partial class ScatterWindow : Window {
        public List<Cluster> Clusters { get; }
        public ScatterWindow( HashSet<Cluster> clusters) {
            InitializeComponent();
            Clusters = clusters.ToList();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            if(Clusters != null) {
                var r = new Random(314);
                ScatterPlot.Model = new PlotModel();

                for(int i = 0; i < Clusters.Count; i++) {
                    List<ScatterPoint> points = new List<ScatterPoint>();

                    Clusters[i].Points.ToList().ForEach(p => points.Add(new ScatterPoint(p.X, p.Y)));
                    ScatterSeries series = new ScatterSeries { MarkerType = MarkerType.Circle, MarkerSize = 4 };
                   
                    series.Points.AddRange(points);
                    ScatterPlot.Model.Series.Add(series);
                }
                ScatterPlot.InvalidatePlot(true);
            }
        }
    }
}
