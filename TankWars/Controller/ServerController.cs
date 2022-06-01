using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Xml;
using Model;
using NetworkUtil;
using Newtonsoft.Json;
using TankWars;
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Controller
{
    public class ServerController
    {
        private TcpListener gameServer;
        private TcpListener webServer;
        private World world;
        private Thread worldLoop;
        private int msPerFrame = 15;
        private bool stop = false;
        private Stopwatch gameDuration = new Stopwatch();

        private Dictionary<int, Socket> connections = new Dictionary<int, Socket>();

        public void StartServer()
        {
            world = new World();

            //If unable to read the settings, don't start the server
            if (!ReadSettings())
                return;
            Console.WriteLine("Settings read successfully!");

            gameDuration.Start();
            //Start updating the world
            worldLoop = new Thread(WorldLoop);
            worldLoop.Start();

            //Start the game server
            gameServer = Networking.StartServer(RecievedClient, 11000);

            //Start the web server
            webServer = Networking.StartServer(RecievedWebClient, 80);
        }

        /// <summary>
        /// Properly update the world and send a frame correctly
        /// </summary>
        /// <param name="obj"></param>
        private void WorldLoop(object obj)
        {
            Stopwatch s = new Stopwatch();
            s.Start();

            while (!stop)
            {
                lock (world)
                {
                    world.UpdateWorld();

                    //send data to each socket
                    SendUpdatedFrame();

                    //Reset tank died flags after they are sent to ensure they are only sent one frame
                    world.ResetTankDied();

                    //Remove any disconnected tanks
                    world.RemoveDisconnectedTanks();
                }
                while (s.ElapsedMilliseconds < msPerFrame) { }
                s.Restart();
            }
        }

        private void SendUpdatedFrame()
        {
            //create message to send
            StringBuilder frame = new StringBuilder();

            foreach (Tank tank in world.GetTanks())
            {
                frame.Append(JsonConvert.SerializeObject(tank) + "\n");
            }
            foreach (Projectile projectile in world.GetProjectiles())
            {
                frame.Append(JsonConvert.SerializeObject(projectile) + "\n");
            }
            foreach (Beam beam in world.GetBeams())
            {
                frame.Append(JsonConvert.SerializeObject(beam) + "\n");
            }
            foreach (Powerup powerup in world.GetPowerups())
            {
                frame.Append(JsonConvert.SerializeObject(powerup) + "\n");
            }
            string data = frame.ToString();

            //send to each client
            lock (connections)
            {
                foreach (int i in connections.Keys)
                {
                    Socket s = connections[i];
                    if(s.Connected)
                        Networking.Send(s, data);
                }
            }

        }

        /// <summary>
        /// Called when a player first establishes a connection
        /// </summary>
        /// <param name="obj"></param>
        private void RecievedClient(SocketState obj)
        {
            if (obj.ErrorOccured)
            {
                RemoveConnection(obj.ID);
                Console.WriteLine("Client " + obj.ID + " disconnected!");
                return;
            }

            obj.OnNetworkAction = RecievedPlayerName;
            Networking.GetData(obj);
        }

        /// <summary>
        /// Called when a web server connects
        /// </summary>
        /// <param name="obj"></param>
        private void RecievedWebClient(SocketState obj)
        {
            if (obj.ErrorOccured)
            {
                return;
            }

            obj.OnNetworkAction = RecieveHTTPRequest;
            Networking.GetData(obj);
        }

        /// <summary>
        /// Called once the web server has recieved a client.
        /// </summary>
        /// <param name="obj"></param>
        private void RecieveHTTPRequest(SocketState obj)
        {
            if (obj.ErrorOccured)
            {
                return;
            }

            string[] splitString = new string[] {"\r\n"};
            string[] messages = obj.GetData().Split(splitString, StringSplitOptions.RemoveEmptyEntries);
            string regex = @"GET /games\?player=.{1,16} HTTP/1\.1";

            string basic = "GET /games";
            string extra = "?player=";
            string HTTP = " HTTP/1.1";
            if (messages[0] == basic + HTTP)
            {
                Dictionary<uint, GameModel> games = DatabaseController.RetrieveAllGames();
                string message = WebViews.GetAllGames(games);
                Networking.SendAndClose(obj.TheSocket, message);
            }
            else if (Regex.IsMatch(messages[0], regex))
            {
                //Networking.SendAndClose(obj.TheSocket, WebViews.GetHomePage(5));
                //TODO: end player specific scores

                int nameLength = messages[0].Length - (basic.Length + extra.Length + HTTP.Length);
                string name = messages[0].Substring(basic.Length + extra.Length, nameLength);
                List<SessionModel> sessions = DatabaseController.RetrievePlayerScores(name);
                string message = WebViews.GetPlayerGames(name, sessions);
                Networking.SendAndClose(obj.TheSocket, message);
            }
            else
            {
                Networking.SendAndClose(obj.TheSocket, WebViews.Get404());
            }
        }

        public void CloseServer()
        {
            stop = true;
            worldLoop.Join();
            gameDuration.Stop();
            int duration = (int)(gameDuration.ElapsedMilliseconds / 1000);

            DatabaseController.UpdateDatabase(duration, world.SaveRemainingPlayers());

            gameServer.Stop();
            webServer.Stop();
        }

        /// <summary>
        /// Called when the server has recieved the Client's side of the handshake
        /// </summary>
        /// <param name="obj"></param>
        private void RecievedPlayerName(SocketState obj)
        {
            if (obj.ErrorOccured)
            {
                RemoveConnection(obj.ID);
                Console.WriteLine("Client " + obj.ID + " disconnected!");
                return;
            }

            //Get the name from the SocketState's data
            string name = obj.GetData().Trim();
            obj.RemoveData(0, name.Length+1);

            //Make a new tank
            int ID = world.AddNewTank(obj.ID, name);

            //Start putting together the server's part of the handshake
            StringBuilder sb = new StringBuilder();
            foreach (Wall w in world.GetWalls())
            {
                sb.Append(JsonConvert.SerializeObject(w) + "\n");
            }

            //Send the handshake, alongside the walls
            Networking.Send(obj.TheSocket, ID.ToString() + "\n" + world.Size.ToString() + "\n" + sb.ToString());

            //Save the connection for later
            Console.WriteLine("Client " + ID + " connected!");
            lock (connections)
            {
                connections.Add(ID, obj.TheSocket);
            }
            
            //wait for commands
            obj.OnNetworkAction = RecieveCommand;
            Networking.GetData(obj);
        }

        /// <summary>
        /// Called when the server recieves a command from the socket
        /// </summary>
        /// <param name="obj"></param>
        private void RecieveCommand(SocketState obj)
        {
            if (obj.ErrorOccured)
            {
                RemoveConnection(obj.ID);
                Console.WriteLine("Client " + obj.ID + " disconnected!");
                return;
            }

            //retrieve the commands and save them to be processed in the World loop
            string[] messages = ProcessMessages(obj);
            foreach (string m in messages)
            {
                ControlCommand c = JsonConvert.DeserializeObject<ControlCommand>(m);
                world.UpdateCommands((int)obj.ID, c);
            }

            Networking.GetData(obj);
        }

        /// <summary>
        /// Called when a socket disconnects from the server
        /// </summary>
        /// <param name="iD"></param>
        private void RemoveConnection(long iD)
        {
            lock (connections)
            {
                connections.Remove((int)iD);
            }
            lock (world)
            {
                world.DisconnectTank((int)iD);
            }
        }

        /// <summary>
        /// Processes the messages from the SocketState's buffer.
        /// </summary>
        /// <param name="socketState">The socket state being used</param>
        /// <returns>The messages</returns>
        private string[] ProcessMessages(SocketState socketState)
        {
            string initialData = socketState.GetData();

            //Note: Split doesn't include the separator character '\n', and may include a final empty string at the end if the separator is at the end
            string[] messages = initialData.Split('\n');

            //The final string is never included, as it will either be a "" or an incomplete message
            string[] completedMessages = new string[messages.Length - 1];

            for (int i = 0; i < messages.Length - 1; i++)
            {
                //The characters to remove from the buffer are the message, plus the '\n'
                socketState.RemoveData(0, messages[i].Length + 1);
                completedMessages[i] = messages[i];
            }

            return completedMessages;
        }

        /// <summary>
        /// Set the world and server settings by reading them from the XML settings file.
        /// Returns true if reading the settings succeeded
        /// Otherwise, returns false
        /// Writes a message to the console if unsuccessful.
        /// </summary>
        private bool ReadSettings()
        {
            try
            {
                using (XmlReader reader = XmlReader.Create("../../../Resources/settings.xml"))
                {

                    Vector2D p1 = null;
                    Vector2D p2 = null;

                    while (reader.Read())
                    {
                        string currentValue = null;
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "GameSettings":
                                    if (reader.Depth != 0)
                                        throw new XmlException("GameSettings header misplaced");
                                    break;
                                case "UniverseSize":
                                    if (reader.Depth != 1)
                                        throw new XmlException("UniverseSize header misplaced");
                                    reader.Read();
                                    currentValue = reader.Value;
                                    if (Int32.TryParse(currentValue, out int size) && size > 0)
                                        world.Size = size;
                                    else
                                        throw new ArgumentException("Invalid world size");
                                    break;
                                case "MSPerFrame":
                                    if (reader.Depth != 1)
                                        throw new XmlException("MSPerFrame header misplaced");
                                    reader.Read();
                                    currentValue = reader.Value;
                                    if (Int32.TryParse(currentValue, out int ms) && ms > 0)
                                        msPerFrame = ms;
                                    else
                                        throw new ArgumentException("Invalid milliseconds per frame");
                                    break;
                                case "FramesPerShot":
                                    if (reader.Depth != 1)
                                        throw new XmlException("FramesPerShot header misplaced");
                                    reader.Read();
                                    currentValue = reader.Value;
                                    if (Int32.TryParse(currentValue, out int fps) && fps >= 0)
                                        world.FramesPerShot = fps;
                                    else
                                        throw new ArgumentException("Invalid frames per shot");
                                    break;
                                case "RespawnRate":
                                    if (reader.Depth != 1)
                                        throw new XmlException("ReaspawnRate header misplaced");
                                    reader.Read();
                                    currentValue = reader.Value;
                                    if (Int32.TryParse(currentValue, out int rr) && rr >= 0)
                                        world.RespawnRate = rr;
                                    else
                                        throw new ArgumentException("Invalid respawn rate");
                                    break;
                                case "Wall":
                                    if (reader.Depth != 1)
                                        throw new XmlException("Wall header misplaced");
                                    break;
                                case "p1":
                                    if (reader.Depth != 2)
                                        throw new XmlException("p1 header misplaced");
                                    p1 = ReadVector(reader);
                                    break;
                                case "p2":
                                    if (reader.Depth != 2)
                                        throw new XmlException("p1 header misplaced");
                                    p2 = ReadVector(reader);
                                    break;
                                case "MaxPowerups":
                                    if (reader.Depth != 1)
                                        throw new XmlException("MaxPowerups header misplaced");
                                    reader.Read();
                                    currentValue = reader.Value;
                                    if (Int32.TryParse(currentValue, out int max) && max >= 0)
                                        world.MaxPowerups = max;
                                    else
                                        throw new ArgumentException("Invalid powerup number");
                                    break;
                                default:
                                    break;
                                    
                            }
                        }
                        else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Wall")
                        {
                            Wall w = new Wall(p1, p2);
                            world.AddNewWall(w);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Setting File Error: " + e.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// A helper method for the settings reader.  Reads in a vector from the XML
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private Vector2D ReadVector(XmlReader reader)
        {
            int x = 0;
            int y = 0;

            bool xRead = false;
            bool yRead = false;

            while (reader.Read())
            {
                string currentValue = null;
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case "x":
                            if (reader.Depth != 3)
                                throw new XmlException("x header misplaced");
                            reader.Read();
                            currentValue = reader.Value;
                            if (Int32.TryParse(currentValue, out int xPoint))
                            {
                                x = xPoint;
                                xRead = true;
                            }
                            else
                                throw new ArgumentException("Invalid coordinate");
                            break;
                        case "y":
                            if (reader.Depth != 3)
                                throw new XmlException("y header misplaced");
                            reader.Read();
                            currentValue = reader.Value;
                            if (Int32.TryParse(currentValue, out int yPoint))
                            {
                                y = yPoint;
                                yRead = true;
                            }
                            else
                                throw new ArgumentException("Invalid coordinate");
                            break;
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement && (reader.Name == "p1" || reader.Name == "p2"))
                {
                    if (xRead && yRead)
                        return new Vector2D(x, y);
                }
            }
            throw new XmlException("Missing wall arguments");
        }
    }
}
