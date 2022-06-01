using System;
using System.Collections.Generic;
using System.Text;
using TankWars;

namespace Model
{
    public class DeathLine
    {
        public int age = 0;
        public int angle;
        public Vector2D origin;

        public DeathLine(Vector2D location, int angle)
        {
            this.origin = new Vector2D(location.GetX(), location.GetY());
            this.angle = angle;
        }
    }
}
