using System.Collections.Generic;

namespace Rover.Lib.Simulation
{
    /// <summary>
    /// Simulation request, in a type safe format ready for processing
    /// </summary>
    public struct SimulationRequestDto
    {
        /// <summary>
        /// Uppermost rightmost coordinates of the simulation
        /// </summary>
        public Point MapTopRight;

        /// <summary>
        /// Initial rover positions
        /// </summary>
        public List<Position> RoverLandingPositions {get; set; }

        /// <summary>
        /// Rover commands, in the same order as RoverLandingPositions
        /// </summary>
        public List<List<RoverCommand>> RoverCommands {get; set; }
    }
}
