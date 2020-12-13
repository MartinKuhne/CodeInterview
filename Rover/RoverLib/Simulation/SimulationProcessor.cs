using System.Collections.Generic;

namespace Rover.Lib.Simulation
{
    /// <summary>
    /// Runs the simulation
    /// </summary>
    public static class SimulationProcessor
    {
        public static SimulationResponse Run(SimulationRequest simulationRequest)
        {
            var saneRequest = simulationRequest.Parse();
            // +1 difference because the spec calls for the coordinates to be zero based AND asking the user for the upper right coordinates
            var mapSize = new Point(saneRequest.MapTopRight.x + 1, saneRequest.MapTopRight.y + 1);
            var world = new World(mapSize);
            var simulationResponse = new SimulationResponse()
            {
                FinalCoordinates = new List<string>()
            };

            for (int iter = 0; iter < saneRequest.RoverLandingPositions.Count; iter++)
            {
                var rover = new MarsRover(world, saneRequest.RoverLandingPositions[iter]);
                rover.Move(saneRequest.RoverCommands[iter]);
                simulationResponse.FinalCoordinates.Add(rover.Position.ToString());
            }

            return simulationResponse;
        }
    }
}
