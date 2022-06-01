using Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TankWars;


namespace View
{

    public class DrawingPanel : Panel
    {

        /// <summary>
        /// A list of the different possible colors
        /// </summary>
        private static string[] Colors =
        {
            "Blue",
            "Dark",
            "Green",
            "LightGreen",
            "Orange",
            "Purple",
            "Red",
            "Yellow"
        };

        //Here are the different images we need
        private Image[] TankImage = new Image[Colors.Length];
        private Image[] TurretImage = new Image[Colors.Length];
        private Image[] ProjectileImage = new Image[Colors.Length];
        private Image WallImage = Image.FromFile("../../../Resources/Graphics/WallSprite.png");
        private Image BackgroundImage = Image.FromFile("../../../Resources/Graphics/Background.png");


        private World theWorld;
        public DrawingPanel(World w)
        {
            DoubleBuffered = true;
            theWorld = w;

            for (int i = 0; i < Colors.Length; i++)
            {
                TankImage[i] = Image.FromFile("../../../Resources/Graphics/" + Colors[i] + "Tank.png");
                TurretImage[i] = Image.FromFile("../../../Resources/Graphics/" + Colors[i] + "Turret.png");
                ProjectileImage[i] = Image.FromFile("../../../Resources/Graphics/shot-" + Colors[i] + ".png");
            }
        }

        /// <summary>
        /// Helper method for DrawObjectWithTransform
        /// </summary>
        /// <param name="size">The world (and image) size</param>
        /// <param name="w">The worldspace coordinate</param>
        /// <returns></returns>
        private static int WorldSpaceToImageSpace(int size, double w)
        {
            return (int)w + size / 2;
        }

        // A delegate for DrawObjectWithTransform
        // Methods matching this delegate can draw whatever they want using e  
        public delegate void ObjectDrawer(object o, PaintEventArgs e);


        /// <summary>
        /// This method performs a translation and rotation to drawn an object in the world.
        /// </summary>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        /// <param name="o">The object to draw</param>
        /// <param name="worldSize">The size of one edge of the world (assuming the world is square)</param>
        /// <param name="worldX">The X coordinate of the object in world space</param>
        /// <param name="worldY">The Y coordinate of the object in world space</param>
        /// <param name="angle">The orientation of the objec, measured in degrees clockwise from "up"</param>
        /// <param name="drawer">The drawer delegate. After the transformation is applied, the delegate is invoked to draw whatever it wants</param>
        private void DrawObjectWithTransform(PaintEventArgs e, object o, int worldSize, double worldX, double worldY, double angle, ObjectDrawer drawer)
        {
            // "push" the current transform
            System.Drawing.Drawing2D.Matrix oldMatrix = e.Graphics.Transform.Clone();

            int x = WorldSpaceToImageSpace(worldSize, worldX);
            int y = WorldSpaceToImageSpace(worldSize, worldY);
            e.Graphics.TranslateTransform(x, y);
            e.Graphics.RotateTransform((float)angle);
            drawer(o, e);

            // "pop" the transform
            e.Graphics.Transform = oldMatrix;
        }

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void TankDrawer(object o, PaintEventArgs e)
        {

            Tank t = o as Tank;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            //drawing the body
            int width = 60;
            int height = 60;
            e.Graphics.DrawImage(TankImage[theWorld.GetTankColor(t.ID)], -width / 2, -height / 2, width, height);
        }

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void TurretDrawer(object o, PaintEventArgs e)
        {

            Tank t = o as Tank;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            //drawing the turret
            int width = 50;
            int height = 50;
            e.Graphics.DrawImage(TurretImage[theWorld.GetTankColor(t.ID)], -width / 2, -height / 2, width, height);
        }

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void WallDrawer(object o, PaintEventArgs e)
        {

            Wall w = o as Wall;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            //drawing the wall
            int width = 50;
            int height = 50;

            e.Graphics.DrawImage(WallImage, -width / 2, -height / 2, width, height);
        }

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void BackgroundDrawer(object o, PaintEventArgs e)
        {

            Wall w = o as Wall;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            //drawing the wall
            int size = theWorld.Size;

            e.Graphics.DrawImage(BackgroundImage, -size / 2, -size / 2, size, size);
        }

        /// <summary>
        /// Draws the projectiles
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void ProjectileDrawer(object o, PaintEventArgs e)
        {
            Projectile p = o as Projectile;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            //drawing the projectile
            int size = 30;

            e.Graphics.DrawImage(ProjectileImage[theWorld.GetTankColor(p.Owner)], -size / 2, -size / 2, size, size);
        }

        /// <summary>
        /// Draws the beams
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void BeamDrawer(object o, PaintEventArgs e)
        {
            Beam b = o as Beam;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Vector2D endPoint = b.Direction * theWorld.Size;

            Pen p = new Pen(GetRainbowColor(b.age, Constants.MaxBeamAge), 5);//maybe make beam width a constant

            e.Graphics.DrawLine(p, 0, 0, (int)endPoint.GetX(), (int)endPoint.GetY());
        }

        /// <summary>
        /// Draws powerups
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void PowerupDrawer(object o, PaintEventArgs e)
        {
            Powerup p = o as Powerup;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Pen pen1 = new Pen(Color.White, 5);
            SolidBrush b = new SolidBrush(GetRainbowColor(theWorld.GetPowerupAge(p.ID), Constants.MaxPowerupAge));

            Rectangle r1 = new Rectangle(-8, -8, 16, 16);
            e.Graphics.DrawEllipse(pen1, r1);
            e.Graphics.FillEllipse(b, r1);
        }

        /// <summary>
        /// Draws the health and name of the given tank
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void DrawTankStats(Object o, PaintEventArgs e)
        {
            Tank t = o as Tank;
            Font f = new Font("Arial", 12);
            SolidBrush b = new SolidBrush(Color.White);

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int size = 50;
            string nameplate = t.Name + ": " + t.Score;
            SizeF stringSize = e.Graphics.MeasureString(nameplate, f);
            // draw name and score
            e.Graphics.DrawString(nameplate, f, b, -(stringSize.Width / 2), size / 2 + 5);

            int yOffset = -40;
            int xOffset = -25;
            int width = 50;
            int height = 6;

            Rectangle rect;

            //draw health
            if (t.HitPoints == 3)
            {
                rect = new Rectangle(xOffset, yOffset, width, height);
                b = new SolidBrush(Color.Green);
            }
            else if (t.HitPoints == 2)
            {
                rect = new Rectangle(xOffset, yOffset, 2 * (width / 3), height);
                b = new SolidBrush(Color.Yellow);
            }
            else
            {
                rect = new Rectangle(xOffset, yOffset, width / 3, height);
                b = new SolidBrush(Color.Red);
            }

            e.Graphics.FillRectangle(b, rect);
        }

        /// <summary>
        /// Gets a color based on an age and it's maximum age
        /// </summary>
        /// <param name="age"></param>
        /// <param name="MaxAge"></param>
        /// <returns></returns>
        private Color GetRainbowColor(int age, int MaxAge)
        {
            int range = MaxAge / 5;
            Color c;
            int increment = 204 / range;


            if (age < range)
            {
                c = Color.FromArgb(255, 51 + (age % range) * increment, 51);
            }
            else if (age < range * 2)
            {
                c = Color.FromArgb(255 - (age % range) * increment, 255, 51);
            }
            else if (age < range * 3)
            {
                c = Color.FromArgb(51, 255, 51 + (age % range) * increment);
            }
            else if (age < range * 4)
            {
                c = Color.FromArgb(51, 255 - (age % range) * increment, 255);
            }
            else
            {
                c = Color.FromArgb(51 + (age % range) * increment, 51, 255);
            }

            return c;
        }

        /// <summary>
        /// Draws the death circles
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void DeathCircleDrawer(object o, PaintEventArgs e)
        {
            DeathCircle d = o as DeathCircle;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            int diameter = Constants.MaxDeathCircleSize / Constants.MaxDeathCircleAge * d.age;

            Rectangle rect = new Rectangle(-diameter/2, -diameter/2, diameter, diameter);

            Pen p = new Pen(Color.OrangeRed, 25);

            e.Graphics.DrawEllipse(p, rect);

        }

        /// <summary>
        /// Draws the death lines
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void DeathLineDrawer(object o, PaintEventArgs e)
        {
            DeathLine l = o as DeathLine;
            double radians = l.angle * Math.PI / 180.00;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Vector2D vAngle = new Vector2D(Math.Sin(radians), Math.Cos(radians));
            int offset = 8;

            Vector2D vOffset = vAngle * offset;

            int length = 20;
            Vector2D vDistance = vAngle * (offset + length);

            Pen p = new Pen(Color.White, 5);



            e.Graphics.DrawLine(p, (int)vOffset.GetX(), (int)vOffset.GetY(), (int)vDistance.GetX(), (int)vDistance.GetY());

        }

        // This method is invoked when the DrawingPanel needs to be re-drawn
        protected override void OnPaint(PaintEventArgs e)
        {
            Vector2D tankLocation = theWorld.GetTankLocation(theWorld.PlayerTankID);
            //The tank's location will return null if it doesn't exist yet.  If that's the case, don't draw anything.
            if (tankLocation == null)
                return;

            double playerX = tankLocation.GetX();
            double playerY = tankLocation.GetY();

            // calculate view/world size ratio
            double ratio = (double)Constants.DrawingPanelSize / (double)theWorld.Size;
            int halfSizeScaled = (int)(theWorld.Size / 2.0 * ratio);

            //center the world on the player
            double inverseTranslateX = -WorldSpaceToImageSpace(theWorld.Size, playerX) + halfSizeScaled;
            double inverseTranslateY = -WorldSpaceToImageSpace(theWorld.Size, playerY) + halfSizeScaled;
            e.Graphics.TranslateTransform((float)inverseTranslateX, (float)inverseTranslateY);

            //Draw all the objects in the world
            lock (theWorld)
            {
                //Background
                DrawObjectWithTransform(e, null, theWorld.Size, 0, 0, 0, BackgroundDrawer);
                //walls
                foreach (Wall w in theWorld.GetWalls())
                {
                    DrawFullWall(w, e);
                }
                //tank parts
                foreach (Tank t in theWorld.GetTanks())
                {
                    if (t.HitPoints != 0)
                    {
                        DrawObjectWithTransform(e, t, theWorld.Size, t.Location.GetX(), t.Location.GetY(), t.Orientation.ToAngle(), TankDrawer);
                        DrawObjectWithTransform(e, t, theWorld.Size, t.Location.GetX(), t.Location.GetY(), t.Aiming.ToAngle(), TurretDrawer);
                        DrawObjectWithTransform(e, t, theWorld.Size, t.Location.GetX(), t.Location.GetY(), 0, DrawTankStats);
                    }
                }
                //beams
                foreach (Beam b in theWorld.GetBeams())
                {
                    DrawObjectWithTransform(e, b, theWorld.Size, b.Origin.GetX(), b.Origin.GetY(), 0, BeamDrawer);
                }
                //projectiles
                foreach (Projectile p in theWorld.GetProjectiles())
                {
                    DrawObjectWithTransform(e, p, theWorld.Size, p.Location.GetX(), p.Location.GetY(), p.Orientation.ToAngle(), ProjectileDrawer);
                }
                //powerups
                foreach (Powerup p in theWorld.GetPowerups())
                {
                    DrawObjectWithTransform(e, p, theWorld.Size, p.Location.GetX(), p.Location.GetY(), 0, PowerupDrawer);
                }
                //explosion parts
                foreach (DeathCircle d in theWorld.GetDeathCircles())
                {
                    DrawObjectWithTransform(e, d, theWorld.Size, d.location.GetX(), d.location.GetY(), 0, DeathCircleDrawer);
                }
                foreach (DeathLine l in theWorld.GetDeathLines())
                {
                    DrawObjectWithTransform(e, l, theWorld.Size, l.origin.GetX(), l.origin.GetY(), 0, DeathLineDrawer);
                }
            }

            // Do anything that Panel (from which we inherit) needs to do
            base.OnPaint(e);
        }

        /// <summary>
        /// Takes care of drawing the full length of the wall
        /// </summary>
        /// <param name="w"></param>
        /// <param name="e"></param>
        private void DrawFullWall(Wall w, PaintEventArgs e)
        {
            //find the distance
            double xDistance = Math.Abs(w.Endpoint2.GetX() - w.Endpoint1.GetX());
            double yDistance = Math.Abs(w.Endpoint2.GetY() - w.Endpoint1.GetY());
            //this will hold the lesser endpoint
            double smaller;

            //Determine the axis
            if (xDistance == 0)
            {
                //Determine the start point
                if (w.Endpoint2.GetY() < w.Endpoint1.GetY())
                {
                    smaller = w.Endpoint2.GetY();
                }
                else
                {
                    smaller = w.Endpoint1.GetY();
                }

                //Draw the full wall
                for (int i = 0; i <= yDistance; i += 50)
                {
                    DrawObjectWithTransform(e, w, theWorld.Size, w.Endpoint1.GetX(), smaller + i, 0, WallDrawer);
                }
            }
            else
            {
                //Determine the start point
                if (w.Endpoint2.GetX() < w.Endpoint1.GetX())
                {
                    smaller = w.Endpoint2.GetX();
                }
                else
                {
                    smaller = w.Endpoint1.GetX();
                }

                //Draw the full wall
                for (int i = 0; i <= xDistance; i += 50)
                {
                    DrawObjectWithTransform(e, w, theWorld.Size, smaller + i, w.Endpoint1.GetY(), 0, WallDrawer);
                }
            }
        }
    }
}
