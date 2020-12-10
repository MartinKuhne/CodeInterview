using System;

namespace Rover.Lib
{
    /// <summary>
    /// Implement movement for a Mars Rover object
    /// </summary>
    public class MarsRover: IDisposable
    {
        /// <summary>
        /// Our position
        /// </summary>
        public Position Position { get; private set; }

        /// <summary>
        /// Reference to a INavigation instance
        /// </summary>
        private readonly INavigation _navigation;

        /// <summary>
        /// Our vehicle ID
        /// </summary>
        private readonly Guid _vehicleId;

        private bool disposedValue;

        /// <summary>
        /// Create a new Mars Rover instance
        /// </summary>
        /// <param name="navigation">INavigation reference</param>
        /// <param name="position">Intitial position</param>
        public MarsRover(INavigation navigation, Position position)
        {
            _navigation = navigation;
            Position = position;

            _vehicleId = navigation.Register();
            _navigation.ReportLocation(_vehicleId, position.Point);
        }

        /// <summary>
        /// Execute a series of moves
        /// </summary>
        /// <param name="command">Command string</param>
        public void Move(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return;
            }

            foreach (var commandChar in command.ToCharArray())
            {
                var step = CommandParser.ParseCommand(commandChar);
                if (!Move(step))
                {
                    // if any individual move fails, abort movement
                    break;
                }
            }
        }

        /// <summary>
        /// Execute a single move
        /// </summary>
        /// <param name="roverCommand">RoverCommand</param>
        /// <returns>true if move was successful, false otherwise</returns>
        private bool Move(RoverCommand roverCommand)
        {
            switch (roverCommand)
            {
                case RoverCommand.Left:
                    Position.Direction = CardinalDirectionHelper.mapLeftRotation[Position.Direction];
                    break;
                case RoverCommand.Right:
                    Position.Direction = CardinalDirectionHelper.mapRightRotation[Position.Direction];
                    break;
                case RoverCommand.Move:
                    var offset = CardinalDirectionHelper.mapDirectionOffset[Position.Direction];
                    var destination = Position.Point + offset;
                    if (!_navigation.CanMoveTo(_vehicleId, destination, true))
                    {
                        return false;
                    }

                    // drive, then
                    Position.Point = destination;
                    _navigation.ReportLocation(_vehicleId, destination);

                    break;
            }

            return true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _navigation.DeRegister(_vehicleId);
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
