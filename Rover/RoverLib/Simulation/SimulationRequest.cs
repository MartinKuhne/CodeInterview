using System.Collections.Generic;

namespace Rover.Lib.Simulation
{
    /// <summary>
    /// This class holds the simulation request, in the format required by the specification
    /// </summary>
    public struct SimulationRequest
    {
        /// <summary>
        /// Uppermost rightmost coordinates of the simulation
        /// </summary>
        public string MapTopRight;

        /// <summary>
        /// Initial rover positions
        /// </summary>
        public List<string> RoverLandingPositions {get; set; }

        /// <summary>
        /// Rover commands, in the same order as RoverLandingPositions
        /// </summary>
        public List<string> RoverCommands {get; set; }
    }
}
