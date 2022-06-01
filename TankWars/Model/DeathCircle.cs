using System;
using System.Collections.Generic;
using System.Text;
using TankWars;

namespace Model
{
    public class DeathCircle
    {

        public int age = 0;
        public Vector2D location;

        public DeathCircle(Vector2D location)
        {
            this.location = new Vector2D(location.GetX(), location.GetY());
        }
    }
}
