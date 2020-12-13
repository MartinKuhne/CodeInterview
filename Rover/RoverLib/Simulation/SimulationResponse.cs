using System.Collections.Generic;

namespace Rover.Lib.Simulation
{
    /// <summary>
    /// This class holds the simulation response, in the format required by the specification
    /// </summary>
    public struct SimulationResponse
    {
        /// <summary>
        /// Final rover coordinates, in the same order as the SimulationRequest provided
        /// </summary>
        public List<string> FinalCoordinates {get; set; }

        /// <summary>
        /// Error messages (if any)
        /// </summary>
        public List<string> Errors {get; set; }
    }
}