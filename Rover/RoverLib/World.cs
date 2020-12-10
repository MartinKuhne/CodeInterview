using System;
using System.Collections.Generic;
using System.Linq;

namespace Rover.Lib
{
    /// <summary>
    /// World manages the coexistence of movable objects in a coordinate space
    /// following an ATC (air traffic controller) model. In this model, the pilot
    /// physically controls the vehicle, but needs to obtain permission from ATC
    /// for movement inside a controlled airspace. This is a cooperative model.
    /// </summary>
    public class World: INavigation
    {
        private readonly Dictionary<Guid, Point> Vehicles = new Dictionary<Guid, Point>();
        /// <summary>
        /// The reservation system prevents to units to simultaneously enter the same square
        /// </summary>
        private readonly Dictionary<Guid, Point> Reservations = new Dictionary<Guid, Point>();

        private readonly Point _size;

        /// <summary>
        /// Create an instance of world of the specified size
        /// </summary>
        /// <param name="width">Width (coordinates are [0, width -1]</param>
        /// <param name="height">Height (coordinates are [0, height -1]</param>
        public World(Point size)
        {
            if (size.x <= 0 || size.y <= 0)
            {
                throw new IndexOutOfRangeException();
            }

            _size = size;
        }

        /// <summary>
        /// Check if the specified coordinates are on the map
        /// </summary>
        /// <param name="destination">Coordinates</param>
        /// <returns>true if on the map, false otherwise</returns>
        internal bool CheckBounds(Point destination)
        {
            if (destination.x < 0 || destination.y < 0 || destination.x >= _size.x || destination.y >= _size.y)
            {
                return false;
            }

            return true;
        }

        ///<inheritdoc/>
        public bool CanMoveTo(Guid vehicleId, Point destination, bool reserveLocation)
        {
            if (!CheckBounds(destination)) return false;

            lock(this)
            {
                // Collision check
                if (Reservations.Values.Any(p => p.Equals(destination))
                    || Vehicles.Values.Any(p => p.Equals(destination)))
                {
                    return false;
                }

                if (reserveLocation == true)
                {
                    Reservations.Add(vehicleId, destination);
                }
            }

            return true;
        }

        ///<inheritdoc/>
        public void ReportLocation(Guid vehicleId, Point destination)
        {
            if (!CheckBounds(destination)) throw new InvalidOperationException("Out of bounds");

            lock(this)
            {
                if (Vehicles.Any(v => v.Key != vehicleId && v.Value.Equals(destination)))
                {
                    // Collision (yes the throw is safe here!)
                    throw new InvalidOperationException("Another vehicle at destination");
                }

                if (Reservations.ContainsKey(vehicleId))
                {
                    // We arrived at the location we previously requested a permit for
                    Reservations.Remove(vehicleId);
                }
            
                // This will add the vehicle if not present
                Vehicles[vehicleId] = destination;
            }
        }

        ///<inheritdoc/>
        public Guid Register()
        {
            return Guid.NewGuid();
        }

        ///<inheritdoc/>
        public void DeRegister(Guid vehicleId)
        {
            lock(this)
            {
                if(Reservations.ContainsKey(vehicleId))
                {
                    Reservations.Remove(vehicleId);
                }

                Vehicles.Remove(vehicleId);
            }
        }
    }
}
