using System;
using System.Collections.Generic;
using Rover.Lib;

namespace Rover.Cmd
{
    /// <summary>
    /// Command line setup helpers
    /// NOTE: The specification is overly specific on how to obtain user input, and meeting that specification
    /// lead me to rather convoluted and hard to test code. In a real-world scenario, this would merit negotiating
    /// the requirements towards a functionally identical approach better supporting separation of concerns.
    /// </summary>
    internal static class SetupHelper
    {
        internal static Point ReadMapSize()
        {
            Point size = null;
            while (size == null)
            {
                Console.WriteLine("Please input upper right coordinates, two numbers separated by whitespace");
                var input = Console.ReadLine();
                try
                {
                    var tokens = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    size = new Point(int.Parse(tokens[0]), int.Parse(tokens[1]));
                    break;
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Invalid size: {exception.Message}");
                }
            }

            return size;
        }

        internal static ICollection<Tuple<Position, string>> CreateRoverDataFromUserInput()
        {
            var result = new List<Tuple<Position, string>>();
            int roverId = 1;

            while(true)
            {
                Position position = null;

                Console.WriteLine($"Please input the rover {roverId} starting position,");
                Console.WriteLine($"two numbers separated by whitespace plus one character for direction");
                Console.WriteLine($"Blank line to finish");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input) && result.Count > 0)
                {
                    return result;
                }

                try
                {
                    position = CommandParser.ParsePosition(input);
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Invalid input: {exception.Message}");
                    continue;
                }

                Console.WriteLine($"Please input rover {roverId} movement plan:");
                input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Invalid plan");
                    continue;
                }

                result.Add(new Tuple<Position, string>(position, input));
            }
        }
    }
}

