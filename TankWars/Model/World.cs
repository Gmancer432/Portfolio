// Authors: Aidan Lethaby and Sean Richens, November 2019
// World for CS 3500 TankWars game
// University of Utah

using System;
using System.Collections.Generic;
using TankWars;
using NetworkUtil;

namespace Model
{
    /// <summary>
    /// This class represents the world for our TankWars game
    /// </summary>
    public class World
    {
        /// <summary>
        /// The dimensions of the world.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// The number of frames a shot can be fired, the cooldown between shots.
        /// </summary>
        public int FramesPerShot { get; set; }

        /// <summary>
        /// The number of frames a tank can respawn in, the cooldown after being killed.
        /// </summary>
        public int RespawnRate { get; set; }

        /// <summary>
        /// Unique identifier for the players tank.
        /// </summary>
        public int PlayerTankID { get; set; }

        /// <summary>
        /// Tanks movement speed.
        /// </summary>
        public double tankSpeed = 2.9;

        /// <summary>
        /// Projectile speed
        /// </summary>
        public double projectileSpeed = 25;

        /// <summary>
        /// List of disconnected players stats used to update the database at the end of a game.
        /// </summary>
        private List<PlayerModel> disconnectedPlayers = new List<PlayerModel>();

        /// <summary>
        /// Keeps track of the walls in the world.
        /// </summary>
        private Dictionary<int, Wall> walls = new Dictionary<int, Wall>();

        /// <summary>
        /// Keeps track of the tanks in the world.
        /// </summary>
        private Dictionary<int, Tank> tanks = new Dictionary<int, Tank>();

        /// <summary>
        /// Keeps track of the control commands sent to the server.
        /// </summary>
        private Dictionary<int, ControlCommand> commands = new Dictionary<int, ControlCommand>();

        /// <summary>
        /// Keeps track of the projectiles in the world.
        /// </summary>
        private Dictionary<int, Projectile> projectiles = new Dictionary<int, Projectile>();

        /// <summary>
        /// Keeps track of the powerups in the world.
        /// </summary>
        private Dictionary<int, Powerup> powerups = new Dictionary<int, Powerup>();

        /// <summary>
        /// Keeps track of the beams in the world.
        /// </summary>
        private Dictionary<int, Beam> beams = new Dictionary<int, Beam>();

        /// <summary>
        /// Keeps track of the colors of tanks.
        /// </summary>
        private Dictionary<int, int> tankColors = new Dictionary<int, int>();

        /// <summary>
        /// Keeps track of powerup age for color purposes.
        /// </summary>
        private Dictionary<int, int> powerupAge = new Dictionary<int, int>();

        //these are the objects that go into explosions
        private List<DeathCircle> deathCircles = new List<DeathCircle>();
        private List<DeathLine> deathLines = new List<DeathLine>();

        /// <summary>
        /// Keep track of next tank color.
        /// </summary>
        private int nextColor = 0;

        /// <summary>
        /// The max color. There are 8 unique colors.
        /// </summary>
        private readonly int maxColors = 8;

        /// <summary>
        /// Assigns the next color to tanks that newly connect.
        /// </summary>
        private void AssignColor(int ID)
        {
            tankColors.Add(ID, nextColor);
            nextColor++;
            if (nextColor >= maxColors)
                nextColor = 0;
        }

        /// <summary>
        /// Updates the states and adds objects in the world and objects from the given object list.
        /// </summary>
        /// <param name="objects">Game objects</param>
        public void UpdateObjects(List<Object> objects)
        {
            //update the ages of objects that have ages
            UpdateAges();

            //take the objects that were deserialized and update the dictionaries
            lock (this)
            {
                foreach (object o in objects)
                {
                    if (o is Wall)
                    {
                        UpdateDictionary<Wall>((Wall)o, walls);
                    }
                    else if (o is Tank)
                    {
                        //if the tank is new, assign it a color
                        if (!tanks.ContainsKey(((Tank)o).ID))
                            AssignColor(((Tank)o).ID);
                        UpdateDictionary<Tank>((Tank)o, tanks);
                        if (((Tank)o).Died)
                        {
                            MakeTankExplosion((Tank)o);
                        }
                    }
                    else if (o is Projectile)
                    {
                        UpdateDictionary<Projectile>((Projectile)o, projectiles);
                    }
                    else if (o is Powerup)
                    {
                        //if the tank is new, start keeping track of its age
                        if (!powerups.ContainsKey(((Powerup)o).ID))
                            powerupAge.Add(((Powerup)o).ID, 0);
                        UpdateDictionary<Powerup>((Powerup)o, powerups);
                    }
                    else if (o is Beam)
                    {
                        UpdateDictionary<Beam>((Beam)o, beams);
                    }
                    else if (o is Powerup)
                    {
                        UpdateDictionary<Powerup>((Powerup)o, powerups);
                    }
                }

                //remove objects that are too old or died
                RemoveDeadObjects();
            }
        }

        public List<PlayerModel> SaveRemainingPlayers()
        {
            foreach (int i in tanks.Keys)
            {
                Tank t = tanks[i];
                disconnectedPlayers.Add(new PlayerModel(t.Name, (uint)t.Score, (uint)(((double)t.Hits / (double)t.ShotsFired) * 100)));
            }

            return disconnectedPlayers;
        }

        /// <summary>
        /// Generates an explosion centered on a tank
        /// </summary>
        /// <param name="t"></param>
        private void MakeTankExplosion(Tank t)
        {
            DeathCircle d = new DeathCircle(t.Location);
            deathCircles.Add(d);

            DeathLine l;
            for (int i = 0; i < 4; i++)
            {
                l = new DeathLine(t.Location, 45 + i * 90);
                deathLines.Add(l);
            }
        }

        /// <summary>
        /// Removes objects that have died or are too old
        /// </summary>
        private void RemoveDeadObjects()
        {
            int[] beamKeys = new int[beams.Count];
            beams.Keys.CopyTo(beamKeys, 0);
            foreach (int b in beamKeys)
            {
                if (beams[b].age >= Constants.MaxBeamAge)
                    beams.Remove(b);
            }
            int[] projectileKeys = new int[projectiles.Count];
            projectiles.Keys.CopyTo(projectileKeys, 0);
            foreach (int p in projectileKeys)
            {
                if (projectiles[p].Died)
                    projectiles.Remove(p);
            }
            int[] tankKeys = new int[tanks.Count];
            tanks.Keys.CopyTo(tankKeys, 0);
            foreach (int t in tankKeys)
            {
                if (tanks[t].Disconnected)
                {
                    MakeTankExplosion(tanks[t]);
                    tanks.Remove(t);
                    tankColors.Remove(t);
                }
            }
            int[] powerupKeys = new int[powerups.Count];
            powerups.Keys.CopyTo(powerupKeys, 0);
            foreach (int p in powerupKeys)
            {
                if (powerups[p].Died)
                {
                    powerups.Remove(p);
                    powerupAge.Remove(p);
                }
            }
            DeathCircle[] circlesArray = deathCircles.ToArray();
            foreach (DeathCircle d in circlesArray)
            {
                if (d.age > Constants.MaxDeathCircleAge)
                {
                    deathCircles.Remove(d);
                }
            }
            DeathLine[] linesArray = deathLines.ToArray();
            foreach (DeathLine l in linesArray)
            {
                if (l.age > Constants.MaxDeathLineAge)
                {
                    deathLines.Remove(l);
                }
            }
        }

        public void AddNewWall(Wall w)
        {
            walls.Add(w.ID, w);
        }

        /// <summary>
        /// Updates the ages of objects that have ages
        /// </summary>
        private void UpdateAges()
        {
            foreach (int b in beams.Keys)
            {
                beams[b].age++;
            }
            foreach (int p in powerups.Keys)
            {
                powerupAge[p] = powerupAge[p] + 1;
                if (powerupAge[p] > Constants.MaxPowerupAge)
                {
                    powerupAge[p] = 0;
                }
            }
            foreach (DeathCircle d in deathCircles)
            {
                d.age++;
            }
            foreach (DeathLine l in deathLines)
            {
                l.age++;
            }
        }

        /// <summary>
        /// The generic method that updates the given dictionary with the given object
        /// </summary>
        /// <typeparam name="T">The type of dictionary</typeparam>
        /// <param name="sprite">An updated object</param>
        /// <param name="dict">The corresponding dictionary</param>
        public void UpdateDictionary<T>(T sprite, Dictionary<int, T> dict) where T : IHasID
        {
            if (dict.ContainsKey(sprite.ID))
            {
                dict.Remove(sprite.ID);
            }
            dict.Add(sprite.ID, sprite);
        }

        /// <summary>
        /// Gets a list of tanks
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tank> GetTanks()
        {
            foreach (int i in tanks.Keys)
            {
                yield return tanks[i];
            }
        }

        /// <summary>
        /// Gets a list of walls
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Wall> GetWalls()
        {
            foreach (int i in walls.Keys)
            {
                yield return walls[i];
            }
        }

        /// <summary>
        /// Gets a list of Projectiles
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Projectile> GetProjectiles()
        {
            foreach (int i in projectiles.Keys)
            {
                yield return projectiles[i];
            }
        }

        /// <summary>
        /// Gets a list of Beams
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Beam> GetBeams()
        {
            foreach (int i in beams.Keys)
            {
                yield return beams[i];
            }
        }

        /// <summary>
        /// Gets a list of Powerups
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Powerup> GetPowerups()
        {
            foreach (int i in powerups.Keys)
            {
                yield return powerups[i];
            }
        }

        /// <summary>
        /// Gets a list of DeathCircles
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DeathCircle> GetDeathCircles()
        {
            foreach (DeathCircle d in deathCircles)
            {
                yield return d;
            }
        }

        /// <summary>
        /// Gets a lsit of DeathLines
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DeathLine> GetDeathLines()
        {
            foreach (DeathLine d in deathLines)
            {
                yield return d;
            }
        }

        /// <summary>
        /// Gets the color of the given tank
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public int GetTankColor(int ID)
        {
            return tankColors[ID];
        }

        /// <summary>
        /// Gets the age of the given powerup
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public int GetPowerupAge(int ID)
        {
            return powerupAge[ID];
        }

        /// <summary>
        /// Gets the location of the given tank
        /// returns null if the tank doesn't exist yet.
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public Vector2D GetTankLocation(int ID)
        {
            Tank tank;
            tanks.TryGetValue(ID, out tank);
            if (tank == null)
                return null;
            else
                return tank.Location;
        }


        ///Server-specific code///
        public int MaxPowerups = 2;

        private int numPowerups = 0;

        private int PowerupDelay = 0;

        public int MaxPowerupDelay = 1650;
        

        /// <summary>
        /// Adds a new tank to the world when a player connects to the server
        /// </summary>
        /// <param name="ssID"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public int AddNewTank(long ssID, String name)
        {
            Tank t = new Tank();
            int ID = (int)ssID;
            t.SetID(ID);
            t.SetName(name);
            RespawnTank(t);
            lock (this)
            {
                tanks.Add(ID, t);
            }
            return ID;
        }

        /// <summary>
        /// Adds or updates a command that was sent from a client
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="c"></param>
        public void UpdateCommands(int ID, ControlCommand c)
        {
            lock (this)
            {
                if (commands.ContainsKey(ID))
                    commands.Remove(ID);
                commands.Add(ID, c);
            }
        }

        /// <summary>
        /// Processes commands and updates the world
        /// called once per frame
        /// </summary>
        public void UpdateWorld()
        {

            //update powerups (count, spawning)
            UpdatePowerups();

            //remove all beams
            beams.Clear();

            //Process commands
            foreach (int i in commands.Keys)
            {
                UpdateTank(i);
            }

            //update positions of Projectiles
            int[] projKeys = new int[projectiles.Keys.Count];
            projectiles.Keys.CopyTo(projKeys, 0);
            foreach (int i in projKeys)
            {
                UpdateProjectile(i);
            }

        }

        /// <summary>
        /// Updates powerups each frame
        /// </summary>
        private void UpdatePowerups()
        {
            //remove dead powerups
            int[] keys = new int[powerups.Count];
            powerups.Keys.CopyTo(keys, 0);
            foreach (int k in keys)
            {
                Powerup p = powerups[k];
                if (p.Died)
                    powerups.Remove(k);
            }

            //update the delay counter
            if (PowerupDelay > 0 && numPowerups < MaxPowerups)
                PowerupDelay--;

            //if needed, spawn a new powerup
            while (numPowerups < MaxPowerups && PowerupDelay <= 0)
            {
                SpawnPowerup();
            }
        }

        /// <summary>
        /// Spawns a powerup
        /// </summary>
        private void SpawnPowerup()
        {
            Powerup p = new Powerup();
            p.SetNewID();
            Random gen = new Random();

            //put the powerup in an open spot
            bool collided;
            do
            {
                collided = false;

                p.Location = new Vector2D(gen.Next(-Size / 2, Size / 2), gen.Next(-Size / 2, Size / 2));

                collided = WallPointCollision(p.Location);

                foreach (int i in tanks.Keys)
                {
                    if (!collided)
                        collided = TankPointCollision(p.Location, tanks[i]);
                }

            } while (collided);

            //reset the delay
            PowerupDelay = gen.Next(MaxPowerupDelay);

            //save the powerup
            powerups.Add(p.ID, p);
            numPowerups++;
        }

        /// <summary>
        /// updates the projectiles each frame
        /// </summary>
        /// <param name="i"></param>
        private void UpdateProjectile(int i)
        {
            Projectile p = projectiles[i];

            //remove dead projectiles and don't update them
            if (p.Died)
            {
                projectiles.Remove(i);
                return;
            }

            Vector2D newLocation = p.Location + p.Orientation * projectileSpeed;

            //Collision detection
            if (WallPointCollision(p.Location) || (p.Location.GetX() > Size/2 || p.Location.GetX() < -Size/2 || p.Location.GetY() > Size / 2 || p.Location.GetY() < -Size / 2))
            {
                p.Died = true;
            } else
            {
                //check if it hit a tank
                Tank t = TankProjCollision(p);
                if (t != null) { 
                    p.Died = true;
                    t.HitPoints--;
                    tanks[p.Owner].Hits++;
                    if (t.HitPoints <= 0)
                    {
                        TankDied(t, tanks[p.Owner]);
                    }
                }
            }

            p.Location = newLocation;
        }

        /// <summary>
        /// Called when a tank drops to 0 HP
        /// </summary>
        /// <param name="shot">The tank that died</param>
        /// <param name="shooter">The tank that landed the final shot</param>
        private void TankDied(Tank shot, Tank shooter)
        {
            shot.Died = true;
            shot.FramesUntilRespawn = RespawnRate;
            shooter.Score++;
        }

        /// <summary>
        /// Returns the tank that the projectile collided with
        /// If no tank was hit, returns null
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private Tank TankProjCollision(Projectile p)
        {
            foreach (int i in tanks.Keys)
            {
                Tank t = tanks[i];
                if(TankPointCollision(p.Location, t) && p.Owner != t.ID && t.HitPoints != 0)
                {
                    return t;
                }
            }
            return null;
        }

        /// <summary>
        /// Checks to see if a single point collides with a tank
        /// </summary>
        /// <param name="loc"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private bool TankPointCollision(Vector2D loc, Tank t)
        {
            return ((loc - t.Location).Length() < Constants.TankSize / 2);
        }

        private bool WallPointCollision(Vector2D loc)
        {
            //these represent the dimensions of the wall
            double y1;
            double y2;
            double x1;
            double x2;

            foreach (int w in walls.Keys)
            {
                //for horizontal walls
                if (walls[w].Endpoint1.GetY() == walls[w].Endpoint2.GetY())
                {
                    //Get x location
                    x2 = walls[w].Endpoint2.GetX();
                    x1 = walls[w].Endpoint1.GetX();
                    //Add a margin for the wall width
                    if (x1 > x2)
                    {
                        x1 += Constants.WallSize / 2;
                        x2 -= Constants.WallSize / 2;
                    }
                    else
                    {
                        x1 -= Constants.WallSize / 2;
                        x2 += Constants.WallSize / 2;
                    }
                    //determine the y location
                    y1 = walls[w].Endpoint1.GetY() - Constants.WallSize / 2;
                    y2 = walls[w].Endpoint1.GetY() + Constants.WallSize / 2;
                }
                //for horizontal walls
                else
                {
                    //Get y location
                    y1 = walls[w].Endpoint2.GetY();
                    y2 = walls[w].Endpoint1.GetY();
                    //Add a margin for the wall width
                    if (y1 > y2)
                    {
                        y1 += Constants.WallSize / 2;
                        y2 -= Constants.WallSize / 2;
                    }
                    else
                    {
                        y1 -= Constants.WallSize / 2;
                        y2 += Constants.WallSize / 2;
                    }
                    //determine the x location
                    x1 = walls[w].Endpoint1.GetX() - Constants.WallSize / 2;
                    x2 = walls[w].Endpoint1.GetX() + Constants.WallSize / 2;
                }

                //check if the point is inside the wall
                if ((loc.GetY() < y1 && loc.GetY() > y2) || (loc.GetY() < y2 && loc.GetY() > y1))
                {
                    if ((loc.GetX() < x1 && loc.GetX() > x2) || (loc.GetX() < x2 && loc.GetX() > x1))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// updates a tank given the ID of the Command
        /// </summary>
        /// <param name="i"></param>
        private void UpdateTank(int i)
        {
            ControlCommand c = commands[i];
            Tank t = tanks[i];

            //if the tank is dead, update respawn timer
            if (t.HitPoints <= 0 && !t.Disconnected)
            {
                t.FramesUntilRespawn--;
                if (t.FramesUntilRespawn <= 0)
                    RespawnTank(t);
                else
                    return;
            }

            //Tank orientation
            bool moving = true;
            switch (c.Moving)
            {
                case "up":
                    t.Orientation = new Vector2D(0, -1);
                    break;
                case "down":
                    t.Orientation = new Vector2D(0, 1);
                    break;
                case "left":
                    t.Orientation = new Vector2D(-1, 0);
                    break;
                case "right":
                    t.Orientation = new Vector2D(1, 0);
                    break;
                case "none":
                    moving = false;
                    break;
            }

            //Wall collision detection
            Vector2D position = t.Location + (t.Orientation * tankSpeed);
            if (moving && !WallTankCollision(position))
            {
                t.Location = position;

                //Wrap around
                if (t.Location.GetX() > Size / 2)
                    t.Location = new Vector2D(-Size / 2, t.Location.GetY());
                else if (t.Location.GetX() < -Size / 2)
                    t.Location = new Vector2D(Size / 2, t.Location.GetY());
                if (t.Location.GetY() > Size / 2)
                    t.Location = new Vector2D(t.Location.GetX(), -Size / 2);
                else if (t.Location.GetY() < -Size / 2)
                    t.Location = new Vector2D(t.Location.GetX(), Size / 2);
            }

            //Checks for powerup collision
            foreach (int k in powerups.Keys)
            {
                Powerup p = powerups[k];
                if (TankPointCollision(p.Location, t))
                {
                    numPowerups--;
                    t.NumPowerups++;
                    p.Died = true;
                }
            }


            //Update turret direction
            t.Aiming = c.Direction;

            //Check if the tank shoots a projectile
            if (t.FramesUntilNextShot > 0)
                t.FramesUntilNextShot--;

            if (c.Fire == "main" && t.FramesUntilNextShot == 0)
            {
                SpawnProjectile(t);
                t.ShotsFired++;
            }
            else if (c.Fire == "alt" && t.NumPowerups > 0)
            {
                SpawnBeam(t);
                t.NumPowerups--;
                t.ShotsFired++;
            }
        }

        /// <summary>
        /// Spawns a beam at the tank location
        /// </summary>
        /// <param name="t"></param>
        private void SpawnBeam(Tank t)
        {
            Beam b = new Beam();

            b.SetNewID();
            b.Origin = t.Location;
            b.Owner = t.ID;
            b.Direction = t.Aiming;

            beams.Add(b.ID, b);

            foreach (int i in tanks.Keys)
            {
                if (BeamCircleCollision(b.Origin, b.Direction, tanks[i].Location, Constants.TankSize/2) && tanks[i].ID != t.ID)
                {
                    tanks[i].HitPoints = 0;
                    TankDied(tanks[i], tanks[b.Owner]);
                    t.Hits++;
                }
            }   
        }

        /// <summary>
        /// Determines if a ray interescts a circle
        /// </summary>
        /// <param name="rayOrig">The origin of the ray</param>
        /// <param name="rayDir">The direction of the ray</param>
        /// <param name="center">The center of the circle</param>
        /// <param name="r">The radius of the circle</param>
        /// <returns></returns>
        public static bool BeamCircleCollision(Vector2D rayOrig, Vector2D rayDir, Vector2D center, double r)
        {
            // ray-circle intersection test
            // P: hit point
            // ray: P = O + tV
            // circle: (P-C)dot(P-C)-r^2 = 0
            // substitute to solve for t gives a quadratic equation:
            // a = VdotV
            // b = 2(O-C)dotV
            // c = (O-C)dot(O-C)-r^2
            // if the discriminant is negative, miss (no solution for P)
            // otherwise, if both roots are positive, hit

            double a = rayDir.Dot(rayDir);
            double b = ((rayOrig - center) * 2.0).Dot(rayDir);
            double c = (rayOrig - center).Dot(rayOrig - center) - r * r;

            // discriminant
            double disc = b * b - 4.0 * a * c;

            if (disc < 0.0)
                return false;

            // find the signs of the roots
            // technically we should also divide by 2a
            // but all we care about is the sign, not the magnitude
            double root1 = -b + Math.Sqrt(disc);
            double root2 = -b - Math.Sqrt(disc);

            return (root1 > 0.0 && root2 > 0.0);
        }

        /// <summary>
        /// Resets tank health and puts it in a random (and clear) location
        /// </summary>
        /// <param name="t"></param>
        private void RespawnTank(Tank t)
        {
            t.HitPoints = Constants.MaxHP;
            t.FramesUntilNextShot = 0;
            Random gen = new Random();
            bool collided;

            do
            {
                collided = false;
                t.Location = new Vector2D(gen.Next(-Size / 2, Size / 2), gen.Next(-Size / 2, Size / 2));

                if(!collided)
                    collided = WallTankCollision(t.Location);
                foreach (int i in projectiles.Keys)
                {
                    if (!collided)
                        collided = TankProjCollision(projectiles[i]) == t;
                }
                //TODO:  collision for beams/powerups
            } while (collided);
        }

        /// <summary>
        /// Spawns a new projectile, preferably not in a wall
        /// </summary>
        /// <param name="t"></param>
        private void SpawnProjectile(Tank t)
        {
            Projectile p = new Projectile(t.ID, t.Location, t.Aiming);
            lock (this)
            {
                projectiles.Add(p.ID, p);
            }
            t.FramesUntilNextShot = FramesPerShot;
        }

        /// <summary>
        /// For checking when a tank collides with a wall
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private bool WallTankCollision(Vector2D position)
        {
            Vector2D GetTankPoint(int x, int y)
            {
                return position + (new Vector2D(x, y) * (Constants.TankSize / 2));
            }

            Vector2D[] points = new Vector2D[8];
            //In order clockwise starting at top right.
            points[0] = GetTankPoint(1, -1);
            points[1] = GetTankPoint(1, 0);
            points[2] = GetTankPoint(1, 1);
            points[3] = GetTankPoint(0, 1);
            points[4] = GetTankPoint(-1, 1);
            points[5] = GetTankPoint(-1, 0);
            points[6] = GetTankPoint(-1, -1);
            points[7] = GetTankPoint(-1, 0);

            for (int i = 0; i < points.Length; i++)
            {
                if (WallPointCollision(points[i]))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Sets the disconnected flag and other things, called when a tank disconnects from the server
        /// </summary>
        /// <param name="iD"></param>
        public void DisconnectTank(int iD)
        {
            if(tanks.TryGetValue(iD, out Tank tank))
            {
                Tank t = tank;
                t.Disconnected = true;
                t.Died = true;
                t.HitPoints = 0;
            } 
        }

        /// <summary>
        /// Cleans up the tanks that have all been disconnected
        /// </summary>
        public void RemoveDisconnectedTanks()
        {
            int[] keys = new int[tanks.Keys.Count];
            tanks.Keys.CopyTo(keys, 0);
            foreach (int i in keys)
            {
                Tank t = tanks[i];
                if (t.Disconnected)
                {
                    disconnectedPlayers.Add(new PlayerModel(t.Name, (uint)t.Score, (uint)(((double)t.Hits / (double)t.ShotsFired) * 100)));
                    tanks.Remove(i);
                    commands.Remove(i);
                }
            }
        }

        /// <summary>
        /// Reset the died flag of tanks
        /// </summary>
        public void ResetTankDied()
        {
            //Reset the died flags of tanks
            foreach (int i in tanks.Keys)
            {
                tanks[i].Died = false;
            }
        }
    }
}