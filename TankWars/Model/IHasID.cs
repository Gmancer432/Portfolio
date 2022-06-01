// Authors: Aidan Lethaby and Sean Richens, November 2019
// Object with IDs for CS 3500 TankWars game
// University of Utah

namespace Model
{
    /// <summary>
    /// This interface represents objects with IDs for our TankWars game
    /// This allows use of a generic method
    /// </summary>
    public interface IHasID
    {
        /// <summary>
        /// Unique identifier for the object.
        /// </summary>
        int ID { get; }
    }
}