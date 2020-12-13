using System;
using System.Collections.Generic;
using Rover.Lib;
using Rover.Lib.Simulation;

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
        internal static SimulationRequest InputSimulationRequest()
        {
            var simulationRequest = new SimulationRequest()
            {
                RoverCommands = new List<string>(),
                RoverLandingPositions = new List<string>()
            };

            ReadMapSize(ref simulationRequest);
            ReadRovers(ref simulationRequest);
            return simulationRequest;
        }

        internal static void ReadMapSize(ref SimulationRequest simulationRequest)
        {
            while (simulationRequest.MapTopRight == null)
            {
                Console.WriteLine("Please input upper right coordinates, two numbers separated by whitespace");
                var input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                {
                    simulationRequest.MapTopRight = input;
                }
            }
        }

        internal static void ReadRovers(ref SimulationRequest simulationRequest)
        {
            int roverId = 1;

            while(true)
            {
                Console.WriteLine($"Please input the rover {roverId} starting position,");
                Console.WriteLine($"two numbers separated by whitespace plus one character for direction");
                Console.WriteLine($"Blank line to finish");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input) && simulationRequest.RoverCommands.Count > 0)
                {
                    return;
                }

                simulationRequest.RoverLandingPositions.Add(input);

                Console.WriteLine($"Please input rover {roverId} movement plan:");
                input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Invalid plan");
                    continue;
                }

                simulationRequest.RoverCommands.Add(input);
                roverId++;
            }
        }
    }
}

