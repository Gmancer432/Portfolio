// Authors: Aidan Lethaby and Sean Richens, November 2019
// ControlCommands for CS 3500 TankWars game
// University of Utah

using Newtonsoft.Json;
using TankWars;

namespace Model
{
    /// <summary>
    /// This class represents control commands for our TankWars game
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ControlCommand
    {

        /// <summary>
        /// Movement command.
        /// This value must either be: none, up, down, left, right.
        /// </summary>
        [JsonProperty(PropertyName = "moving")]
        public string Moving { get; set; }

        /// <summary>
        /// Fire command.
        /// This value must either be: none, main, alt.
        /// </summary>
        [JsonProperty(PropertyName = "fire")]
        public string Fire { get; set; }

        /// <summary>
        /// Private representation of direction.
        /// </summary>
        private Vector2D p_direction;

        /// <summary>
        /// Turret direction.
        /// A normalized 2d vector.
        /// </summary>
        [JsonProperty(PropertyName = "tdir")]
        public Vector2D Direction
        {
            get { return p_direction; }
            set
            {
                value.Normalize();
                p_direction = value;
            }
        }

        public ControlCommand()
        {
            Moving = "none";
            Fire = "none";
            Direction = new Vector2D(0, -1);
        }
    }
}