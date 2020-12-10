using System;

namespace Rover.Lib
{
    public class Point: IEquatable<Point>
    {
        public int x { get; private set; }
        public int y { get; private set; }

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Point operator+(Point p1, Point p2)
        {
            return new Point(p1.x + p2.x, p1.y + p2.y);
        }

        public bool Equals(Point other)
        {
            return (x == other.x) && (y == other.y);
        }

        // Boilerplate code, cf. https://docs.microsoft.com/en-us/dotnet/api/system.object.equals?view=net-5.0#System_Object_Equals_System_Object_
        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || ! this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else {
                Point p = (Point) obj;
                return (x == p.x) && (y == p.y);
            }
        }

        public override int GetHashCode()
        {
            return (x << 2) ^ y;
        }
    }
}
