using System;
using Rover.Lib.Simulation;

namespace Rover.Cmd
{
    /// <summary>
    /// Accept user input and drive rovers
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            SimulationResponse simulationResponse;
            try {
                var simulationRequest = SetupHelper.InputSimulationRequest();
                simulationResponse = SimulationProcessor.Run(simulationRequest);
            }
            catch(Exception exception)
            {
                Console.WriteLine($"ERROR: {exception.Message}");
                return;
            }

            simulationResponse.FinalCoordinates.ForEach(Console.WriteLine);
        }
    }
}
