namespace Rover.Lib
{
    /// <summary>
    /// Describe a position by coordinate and direction (as per spec)
    /// </summary>
    public class Position
    {
        public Point Point { get; set; }
        public CardinalDirection Direction { get; set; }

        public override string ToString()
        {
            return $"{Point.x} {Point.y} {Direction}";
        }
    }
}
