// Authors: Aidan Lethaby and Sean Richens, November 2019
// Powerup for CS 3500 TankWars game
// University of Utah

using Newtonsoft.Json;
using TankWars;

namespace Model
{
    /// <summary>
    /// This class represents powerups for our TankWars game
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Powerup : IHasID
    {
        /// <summary>
        /// Unique identifier of the powerup.
        /// </summary>
        [JsonProperty(PropertyName = "power")]
        public int ID { get; private set; }

        /// <summary>
        /// Location of the powerup in the world.
        /// </summary>
        [JsonProperty(PropertyName = "loc")]
        public Vector2D Location { get; set; }

        /// <summary>
        /// State of the powerup.
        /// True if powerup has been picked up.
        /// </summary>
        [JsonProperty(PropertyName = "died")]
        public bool Died { get; set; }

        /// <summary>
        /// The default constructor for the powerup class
        /// </summary>
        public Powerup()
        {
            Died = false;
        }

        /// <summary>
        /// used by the server to set the next ID of powerups
        /// </summary>
        private static int NextID = 0;

        /// <summary>
        /// Called when the server generates a new powerup
        /// </summary>
        public void SetNewID()
        {
            ID = NextID++;
        }
    }
}