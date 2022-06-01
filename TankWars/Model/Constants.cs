// Authors: Aidan Lethaby and Sean Richens, November 2019
// Constants for CS 3500 TankWars game
// University of Utah

namespace Model
{
    /// <summary>
    /// This class represents constant values for our TankWars game
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// Max hit points of a tank.
        /// </summary>
        public static readonly int MaxHP = 3;

        /// <summary>
        /// Offset height of the drawing panel.
        /// </summary>
        public static readonly int OffsetHeight = 40;

        /// <summary>
        /// Max age of a beam.
        /// </summary>
        public static readonly int MaxBeamAge = 50;

        /// <summary>
        /// Max age of a powerup (for coloring purposes)
        /// </summary>
        public static readonly int MaxPowerupAge = 150;

        /// <summary>
        /// Max age of the circles that result from tank explosions
        /// </summary>
        public static readonly int MaxDeathCircleAge = 50;

        /// <summary>
        /// max size of the circles that result from tank explosions
        /// </summary>
        public static readonly int MaxDeathCircleSize = 100;

        /// <summary>
        /// Max age of the lines that result from tank explosions
        /// </summary>
        public static readonly int MaxDeathLineAge = 20;

        /// <summary>
        /// The size of the drawing panel.
        /// </summary>
        public static readonly int DrawingPanelSize = 800;

        /// <summary>
        /// The mouse value of the left button.
        /// </summary>
        public static readonly int LeftButton = 1048576;

        /// <summary>
        /// The mouse value of the right button.
        /// </summary>
        public static readonly int RightButton = 2097152;

        /// <summary>
        /// The tank size in pixels.
        /// </summary>
        public static readonly int TankSize = 60;

        /// <summary>
        /// The wall width in pixels
        /// </summary>
        public static readonly int WallSize = 50;

        ///// <summary>
        ///// The keyvalue of the S key.
        ///// </summary>
        //public static readonly int SKey = 83;

        ///// <summary>
        ///// The keyvalue of the D key.
        ///// </summary>
        //public static readonly int DKey = 68;

        ///// <summary>
        ///// The keyvalue of the W key.
        ///// </summary>
        //public static readonly int QKey = 81;
    }
}