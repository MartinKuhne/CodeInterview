using System;
using System.Collections.Generic;

namespace Rover.Lib
{
    public static class CommandParser
    {
        /// <summary>
        /// Translates character into RoverCommand
        /// </summary>
        private static readonly Dictionary<char, RoverCommand> RoverCommandMap = new Dictionary<char, RoverCommand>
        {
            { 'M', RoverCommand.Move },
            { 'L', RoverCommand.Left },
            { 'R', RoverCommand.Right }
        };

        /// <summary>
        /// Returns a position from the given input
        /// </summary>
        /// <param name="command">input string</param>
        /// <returns>Position</returns>
        /// <exception>ArgumentException, FormatException</exception>
        public static Position ParsePosition(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                throw new ArgumentException("input empty");
            }

            var tokens = command.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

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

            return result;
        }

        /// <summary>
        /// Translate a stringified command into a RoverCommand
        /// </summary>
        /// <param name="command">Stringified command</param>
        /// <returns>RoverCommand</returns>
        public static RoverCommand ParseCommand(char command)
        {
            return RoverCommandMap[command];
        }
    }
}
