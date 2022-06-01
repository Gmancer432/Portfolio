// Authors: Aidan Lethaby and Sean Richens, November 2019
// Wall for CS 3500 TankWars game
// University of Utah

using Newtonsoft.Json;
using TankWars;

namespace Model
{
    /// <summary>
    /// This class represents walls for our TankWars game
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Wall : IHasID
    {
        /// <summary>
        /// Counter for wall objects created by the server.
        /// </summary>
        private static int nextID = 0;

        /// <summary>
        /// Unique identifier of the wall.
        /// </summary>
        [JsonProperty(PropertyName = "wall")]
        public int ID { get; private set; }

        /// <summary>
        /// Location of one end of the wall in the world.
        /// </summary>
        [JsonProperty(PropertyName = "p1")]
        public Vector2D Endpoint1 { get; private set; }

        /// <summary>
        /// Location of the other end of the wall in the world.
        /// </summary>
        [JsonProperty(PropertyName = "p2")]
        public Vector2D Endpoint2 { get; private set; }

        /// <summary>
        /// Constructs a wall givven the two vector point.
        /// </summary>
        /// <param name="p1">Endpoint 1</param>
        /// <param name="p2">Endpoint 2</param>
        public Wall(Vector2D p1, Vector2D p2)
        {
            Endpoint1 = p1;
            Endpoint2 = p2;
            ID = nextID++;
        }
    }
}