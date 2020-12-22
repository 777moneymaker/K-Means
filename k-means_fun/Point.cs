using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace k_means_fun {
    public class Point {
        public double X { get; set; }
        public double Y { get; set; }

        public bool InCluster { get; set; }

        public Point(double x, double y, bool in_cluster = false) {
            X = x;
            Y = y;
            InCluster = in_cluster;
        }

        public Point() { }

        public override string ToString() => $"{X}, {Y}";
    }
}
