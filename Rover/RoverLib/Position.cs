using System;

namespace Rover.Lib
{
    /// <summary>
    /// Describe a position by coordinate and direction (as per spec)
    /// </summary>
    public class Position
    {
        public Point Point { get; set; }
        public CardinalDirection Direction { get; set; }

        public Position()
        {
        }

        public Position(string stringRepresentation)
        {
            var tokens = stringRepresentation.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            Point = new Point(int.Parse(tokens[0]), int.Parse(tokens[1]));
            Direction = Enum.Parse<CardinalDirection>(tokens[2]);
        }

        public override string ToString()
        {
            return $"{Point.x} {Point.y} {Direction}";
        }
    }
}
