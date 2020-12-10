using System;
using System.Collections.Generic;
using Rover.Lib;

namespace Rover.Cmd
{
    /// <summary>
    /// Accept user input and drive rovers
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var upperRight = SetupHelper.ReadMapSize();
            // +1 difference because the spec calls for the coordinates to be zero based AND asking the user for the upper right coordinates
            var mapSize = new Point(upperRight.x + 1, upperRight.y + 1);

            var world = new World(mapSize);
            var roverData = SetupHelper.CreateRoverDataFromUserInput();
            var rovers = new List<MarsRover>();

            foreach (var iter in roverData)
            {
                var rover = new MarsRover(world, iter.Item1);
                rover.Move(iter.Item2);
                Console.WriteLine(rover.Position.ToString());
            }
        }
    }
}
