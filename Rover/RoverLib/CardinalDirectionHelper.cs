using System.Collections.Generic;

namespace Rover.Lib
{
    public static class CardinalDirectionHelper
    {
        /// <summary>
        /// Identifies the new direction when rotated to the left
        /// </summary>
        public static Dictionary<CardinalDirection, CardinalDirection> mapLeftRotation = new Dictionary<CardinalDirection, CardinalDirection>
        {
            { CardinalDirection.N, CardinalDirection.W},
            { CardinalDirection.W, CardinalDirection.S},
            { CardinalDirection.S, CardinalDirection.E},
            { CardinalDirection.E, CardinalDirection.N},
        };

        /// <summary>
        /// Identifies the new direction when rotated to the right
        /// </summary>
        public static Dictionary<CardinalDirection, CardinalDirection> mapRightRotation = new Dictionary<CardinalDirection, CardinalDirection>
        {
            { CardinalDirection.N, CardinalDirection.E},
            { CardinalDirection.E, CardinalDirection.S},
            { CardinalDirection.S, CardinalDirection.W},
            { CardinalDirection.W, CardinalDirection.N}
        };

        /// <summary>
        /// Translates a directional movement into relative coordinates
        /// </summary>
        public static Dictionary<CardinalDirection, Point> mapDirectionOffset = new Dictionary<CardinalDirection, Point>
        {
            { CardinalDirection.N, new Point(0, 1)},
            { CardinalDirection.E, new Point(1, 0)},
            { CardinalDirection.S, new Point(0, -1)},
            { CardinalDirection.W, new Point(-1, 0)}
        };
    }
}
