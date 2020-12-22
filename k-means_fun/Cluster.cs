using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace k_means_fun {
    public class Cluster {

        public Point Centroid { get; set; }
        public ObservableCollection<Point> Points { get; set; }

        public Cluster() {
            Points = new ObservableCollection<Point>();
            Centroid = new Point { X = 0, Y = 0 };
            
            Points.CollectionChanged += Points_CollectionChanged;
        }

        private void Points_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            double x_sum = Points.Sum(p => p.X);
            double y_sum = Points.Sum(p => p.Y);

            Centroid = new Point {
                X = x_sum / Points.Count,
                Y = y_sum / Points.Count
            };
        }

        public double GetDistanceToCentroid(Point p) {
            if(p is null)
                throw new ArgumentNullException("Point is null");

            double xDelta = Centroid.X - p.X;
            double yDelta = Centroid.Y - p.Y;

            return Math.Sqrt(Math.Pow(xDelta, 2) + Math.Pow(yDelta, 2));
        }
    }
}
