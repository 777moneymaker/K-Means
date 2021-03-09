using System;
using System.Collections.Generic;
using System.Linq;

namespace k_means_fun {
    public class Cluster : IEquatable<Cluster>, ICloneable {

        public Point Centroid { get; set; } = new Point { X = 0, Y = 0 };
        public List<Point> Points { get; set; } = new List<Point>();

        public Cluster() { 
            //Points.CollectionChanged += Points_CollectionChanged;
        }


        public void RearrangeCentroid() {
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

        bool IEquatable<Cluster>.Equals(Cluster other) {
            return this.Points.SequenceEqual(other.Points);
        }
        object ICloneable.Clone() {
            return new Cluster() {
                Centroid = Centroid.Clone() as Point,
                Points = new List<Point>(Points.Select(p => p.Clone() as Point))
            };
        }
    }
}
