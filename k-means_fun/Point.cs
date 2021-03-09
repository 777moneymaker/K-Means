using System;

namespace k_means_fun {
    public class Point : IEquatable<Point>, ICloneable {
        public double X { get; set; }
        public double Y { get; set; }
        public Point(double x, double y) {
            X = x;
            Y = y;
        }

        public Point() { }

        public override string ToString() => $"{X}, {Y}";
        
        public bool Equals(Point other) {
            return ReferenceEquals(other, null) ? false : X == other.X && Y == other.Y;
        }

        public static bool operator ==(Point obj1, Point obj2) {
            if(ReferenceEquals(obj1, obj2)) {
                return true;
            }
            if(ReferenceEquals(obj1, null)) {
                return false;
            }
            if(ReferenceEquals(obj2, null)) {
                return false;
            }

            return obj1.Equals(obj2);
        }

        public static bool operator !=(Point obj1, Point obj2) {
            return !(obj1 == obj2);
        }

        public override bool Equals(object obj) {
            return Equals(obj as Point);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                return hashCode;
            }
        }

        public object Clone() {
            return new Point { X = this.X, Y = this.Y };
        }
    }
}
