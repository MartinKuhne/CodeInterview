using System;
using System.Collections.Generic;

namespace Rover.Lib
{
    public static class SimulationRequestValidator
    {
        /// <summary>
        /// Returns a position from the given input
        /// </summary>
        /// <param name="command">input string</param>
        /// <returns>Position</returns>
        /// <exception>ArgumentException, FormatException</exception>
        public static void ValidatePosition(string position)
        {
            if (string.IsNullOrWhiteSpace(position))
            {
                throw new ArgumentException("input empty");
            }

            var tokens = position.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (tokens.Length != 3)
            {
                throw new ArgumentException("invalid number of parameters");
            }

            var result = new Position
            {
                Point = new Point(int.Parse(tokens[0]), int.Parse(tokens[1])),
                Direction = Enum.Parse<CardinalDirection>(tokens[2])
            };

            if (result.Point.x < 0 || result.Point.y < 0)
            {
                throw new ArgumentException("coordinates may not be negative");
            }
        }
    }
}
