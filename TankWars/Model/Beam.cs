// Authors: Aidan Lethaby and Sean Richens, November 2019
// Beam object for CS 3500 TankWars game
// University of Utah

using Newtonsoft.Json;
using TankWars;

namespace Model
{
    /// <summary>
    /// This class represents the beam for our TankWars game
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Beam : IHasID
    {
        /// <summary>
        /// Unique identifier of the beam.
        /// </summary>
        [JsonProperty(PropertyName = "beam")]
        public int ID { get; set; }

        /// <summary>
        /// Starting point of the beam.
        /// </summary>
        [JsonProperty(PropertyName = "org")]
        public Vector2D Origin { get; set; }

        /// <summary>
        /// Direction of the beam.
        /// </summary>
        [JsonProperty(PropertyName = "dir")]
        public Vector2D Direction { get; set; }

        /// <summary>
        /// ID of player who owns the beam.
        /// </summary>
        [JsonProperty(PropertyName = "owner")]
        public int Owner { get; set; }

        /// <summary>
        /// To keep track of how long to draw the beam.
        /// </summary>
        public int age = 0;

        /// <summary>
        /// used by the server to set the next ID of beam
        /// </summary>
        private static int NextID = 0;

        /// <summary>
        /// Called when the server generates a new beam
        /// </summary>
        public void SetNewID()
        {
            ID = NextID++;
        }
    }
}