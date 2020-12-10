using System;

namespace Rover.Lib
{
    public interface INavigation
    {
        /// <summary>
        /// Register with ATC to obtain an ID
        /// </summary>
        public Guid Register();
        
        /// <summary>
        /// Deregister with ATC
        /// </summary>
        /// <param name="vehicleId">Vehicle ID</param>
        public void DeRegister(Guid vehicleId);
        
        /// <summary>
        /// Ask for permission to move to the specified destination
        /// </summary>
        /// <param name="vehicleId">Vehicle ID</param>
        /// <param name="destination">Destination coordinates</param>
        /// <param name="reserveLocation">Mark location as reserved (prevents other vehicles to enter the same location)</param>
        /// <returns>true if permission granted</returns>
        public bool CanMoveTo(Guid vehicleId, Point destination, bool reserveLocation);
        
        /// <summary>
        /// Report a vehicle's current location
        /// </summary>
        /// <param name="vehicleId">Vehicle ID</param>
        /// <param name="destination">Destination coordinates</param>
        public void ReportLocation(Guid vehicleId, Point destination);
    }
}
