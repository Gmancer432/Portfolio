// Authors: Aidan Lethaby and Sean Richens, November 2019
// Projectile for CS 3500 TankWars game
// University of Utah

using Newtonsoft.Json;
using TankWars;

namespace Model
{
    /// <summary>
    /// This class represents tanks for our TankWars game
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Tank : IHasID
    {
        /// <summary>
        /// Unique identifier of the tank.
        /// </summary>
        [JsonProperty(PropertyName = "tank")]
        public int ID { get; set; }

        /// <summary>
        /// Location of the tank in the world.
        /// </summary>
        [JsonProperty(PropertyName = "loc")]
        public Vector2D Location { get; set; }

        /// <summary>
        /// Orientation of the tank body in the world.
        /// </summary>
        [JsonProperty(PropertyName = "bdir")]
        public Vector2D Orientation { get; set; }

        /// <summary>
        /// Orientation of the tank turret in the world.
        /// </summary>
        [JsonProperty(PropertyName = "tdir")]
        public Vector2D Aiming { get; set; }

        /// <summary>
        /// The name of the tank player.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Hit points of the tank.
        /// </summary>
        [JsonProperty(PropertyName = "hp")]
        public int HitPoints { get; set; }

        /// <summary>
        /// Score (kills) of the tank.
        /// </summary>
        [JsonProperty(PropertyName = "score")]
        public int Score { get; set; }

        /// <summary>
        /// State of the tank.
        /// True if tank has died on a certain frame.
        /// </summary>
        [JsonProperty(PropertyName = "died")]
        public bool Died { get; set; }

        /// <summary>
        /// True if tank player has disconnected from the server.
        /// </summary>
        [JsonProperty(PropertyName = "dc")]
        public bool Disconnected { get; set; }

        /// <summary>
        /// True if the tank player has just connected to the server.
        /// </summary>
        [JsonProperty(PropertyName = "join")]
        public bool Joined { get; set; }

        public int FramesUntilNextShot = 0;
        public int FramesUntilRespawn = 0;
        public int NumPowerups = 0;
        public int ShotsFired = 0;
        public int Hits = 0;

        /// <summary>
        /// The default constructor for the tank class
        /// </summary>
        public Tank()
        {
            Aiming = new Vector2D(0, -1);
            Orientation = new Vector2D(0, -1);
            Aiming = new Vector2D(0, -1);
            Location = new Vector2D(0, 0);
            HitPoints = Constants.MaxHP;
            Score = 0;
            Died = false;
            Disconnected = false;
            Joined = false;
        }

        /// <summary>
        /// Sets the tanks ID. For testing purposes!!!!!!!
        /// </summary>
        /// <param name="i">The ID</param>
        public void SetID(int i)
        {
            ID = i;
        }

        public void SetName(string name)
        {
            this.Name = name;
        }
    }
}