using System;
using System.Collections.Generic;
using System.Linq;

namespace Rover.Lib.Simulation
{
    /// <summary>
    /// Convert the simulation input into type safe data
    /// </summary>
    public static class SimulationRequestParser
    {
        public static SimulationRequestDto Parse(this SimulationRequest simulationRequest)
        {
            var tokens = simulationRequest.MapTopRight.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var result = new SimulationRequestDto
            {
                MapTopRight = new Point(int.Parse(tokens[0]), int.Parse(tokens[1])),
                RoverLandingPositions = simulationRequest.RoverLandingPositions.Select(rlp => new Position(rlp)).ToList(),
                RoverCommands = new List<List<RoverCommand>>()
            };

            foreach (var roverCommandSequence in simulationRequest.RoverCommands)
            {
                result.RoverCommands.Add(roverCommandSequence.AsRoverCommand());
            }

            return result;
        }

    }
}
