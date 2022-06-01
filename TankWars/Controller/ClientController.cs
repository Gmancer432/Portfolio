// Authors: Aidan Lethaby and Sean Richens, November 2019
// Controller for CS 3500 TankWars game
// University of Utah

using System;
using System.Collections.Generic;
using System.Diagnostics;
using NetworkUtil;
using Newtonsoft.Json;
using Model;
using TankWars;

namespace Controller
{
    /// <summary>
    /// This class acts as the controller for our TankWars game (based off the MVC model)
    /// </summary>
    public class ClientController
    {
        /// <summary>
        /// Delegate method for when an error occurs.
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="errorType">The error type</param>
        public delegate void Error(string message, string errorType);
        
        /// <summary>
        /// Event for when an error occurs.
        /// </summary>
        private event Error ErrorEvent;

        /// <summary>
        /// Delegate method for when view must be invalidated.
        /// </summary>
        public delegate void OnFrame();

        /// <summary>
        /// Event for when view must be invalidated.
        /// </summary>
        private event OnFrame OnFrameEvent;

        /// <summary>
        /// Delegate method for getting player name from view.
        /// </summary>
        /// <returns>The name of the player</returns>
        public delegate string GetName();
        
        /// <summary>
        /// Method name for getting player name from view.
        /// </summary>
        private GetName GetPlayerName;

        /// <summary>
        /// World object for representing the game world.
        /// </summary>
        private World theWorld;

        /// <summary>
        /// Control command object to represent user input.
        /// </summary>
        private ControlCommand controlCommand;

        /// <summary>
        /// List holding the currently active move commands in order of most recently held down.
        /// </summary>
        private List<string> moveCommand = new List<string>();

        /// <summary>
        /// List holding the currently active fire commands in order of most recently held down.
        /// </summary>
        private List<string> fireCommand = new List<string>();

        //These are the flags that keep track of what buttons are currently being pressed.
        //If true, the key is currently pressed
        private bool up = false;
        private bool down = false;
        private bool left = false;
        private bool right = false;

        /// <summary>
        /// Flag for whether the program is closing.
        /// </summary>
        private bool close = false;

        /// <summary>
        /// Constructor for creating a new game controller with a new world.
        /// </summary>
        public ClientController()
        {
            theWorld = new World();
            controlCommand = new ControlCommand();
            moveCommand.Add("none");
            fireCommand.Add("none");
        }

        /// <summary>
        /// Connects to the server at the specified address.
        /// </summary>
        /// <param name="address">The address of the server</param>
        /// <param name="name">The name of the player</param>
        public void ConnectToServer(string address, string name)
        {
            // Check for valid name
            if(name.Length > 16)
            {
                ErrorEvent("Name must be less than 16 characters.","Naming Error");
                return;
            }

            //Set delegate for initial handshake
            Networking.ConnectToServer(InitialHandshake, address, 11000);
        }

        /// <summary>
        /// Handles initial handshake between server and client.
        /// </summary>
        /// <param name="socketState">The socket state being used</param>
        private void InitialHandshake(SocketState socketState)
        {
            // Check if an error has occured
            if(socketState.ErrorOccured)
            {
                ErrorEvent("Unable to connect!\n" + socketState.ErrorMessage,"Connection Error");
                if (socketState.TheSocket != null)
                {
                    socketState.TheSocket.Close();
                }
                return;
            }

            string name = GetPlayerName();

            //Send handshake data
            Networking.Send(socketState.TheSocket, name + "\n");

            //Change delegate to accept the startup info
            socketState.OnNetworkAction = GetStartupInfo;

            //Get the startup info
            Networking.GetData(socketState);
        }

        /// <summary>
        /// Called when a key is pressed.
        /// Update what key was most recently downed.
        /// </summary>
        /// <param name="keyValue">The key value</param>
        public void KeyDown(int keyValue)
        {
            string move = TranslateKeyToMove(keyValue);
            lock (controlCommand)
            {
                if (move != "")
                {
                    if (!CheckFlag(move))
                    {
                        moveCommand.Insert(0, move);
                        controlCommand.Moving = moveCommand[0];
                        SetFlag(move, true);
                    }
                }
            }
        }

        /// <summary>
        /// Called when a key is released.
        /// Update what key has most recently been released.
        /// </summary>
        /// <param name="keyValue">The key value</param>
        public void KeyUp(int keyValue)
        {
            string move = TranslateKeyToMove(keyValue);
            lock (controlCommand)
            {
                if (move != "")
                {
                    moveCommand.Remove(move);
                    controlCommand.Moving = moveCommand[0];
                    SetFlag(move, false);
                }
            }
        }

        /// <summary>
        /// Translates the key value into a movement command.
        /// </summary>
        /// <param name="keyValue">The key value</param>
        /// <returns>The move command</returns>
        private string TranslateKeyToMove(int keyValue)
        {
            string move = "";

            switch (keyValue)
            {
                case 'W':
                    move = "up";
                    break;
                case 'A':
                    move = "left";
                    break;
                case 'S':
                    move = "down";
                    break;
                case 'D':
                    move = "right";
                    break;
                default:
                    break;
            }
            return move;
        }

        /// <summary>
        /// Returns the value of the corresponding flag
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        private bool CheckFlag(string flag)
        {
            switch (flag)
            {
                case "up":
                    return up;
                case "down":
                    return down;
                case "left":
                    return left;
                case "right":
                    return right;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Sets the value of the corresonding flag
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="value"></param>
        private void SetFlag(string flag, bool value)
        {
            switch (flag)
            {
                case "up":
                    up = value;
                    break;
                case "down":
                    down = value;
                    break;
                case "left":
                    left = value;
                    break;
                case "right":
                    right = value;
                    break;
            }
        }

        /// <summary>
        /// Called when a mouse button is pressed.
        /// Update what button was most recently downed.
        /// </summary>
        /// <param name="buttonValue">The key value</param>
        public void MouseDown(int buttonValue)
        {
            string fire = TranslateMouseToFire(buttonValue);
            lock (controlCommand)
            {
                if (fire != "")
                {
                    fireCommand.Insert(0, fire);
                    controlCommand.Fire = fireCommand[0];
                }
            }
        }

        /// <summary>
        /// Called when a mouse button is released.
        /// Update what button has most recently been released.
        /// </summary>
        /// <param name="buttonValue">The key value</param>
        public void MouseUp(int buttonValue)
        {
            string fire = TranslateMouseToFire(buttonValue);
            lock (controlCommand)
            {
                if (fire == "alt" && fireCommand.Contains("alt"))
                {
                    return;
                }
                if (fire != "")
                {
                    fireCommand.Remove(fire);
                    controlCommand.Fire = fireCommand[0];
                }
            }
        }

        /// <summary>
        /// Translates the mouse button value into a fire command.
        /// </summary>
        /// <param name="buttonValue">The mouse button value</param>
        /// <returns>The fire command</returns>
        private string TranslateMouseToFire(int buttonValue)
        {
            string fire = "";

            if (buttonValue == Constants.LeftButton)
                fire = "main";
            else if (buttonValue == Constants.RightButton)
                fire = "alt";
            return fire;
        }

        /// <summary>
        /// Updates aiming angle.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void MouseMove(int x, int y)
        {
            lock (controlCommand)
            {
                Vector2D angle = new Vector2D(x - Constants.DrawingPanelSize / 2, y - Constants.DrawingPanelSize / 2 - Constants.OffsetHeight);
                angle.Normalize();
                controlCommand.Direction = angle;
            }
        }

        /// <summary>
        /// Handles the first server data recieved after the handshake.
        /// </summary>
        /// <param name="socketState">The socket state being used</param>
        private void GetStartupInfo(SocketState socketState)
        {
            //Check if an error has occured
            if (socketState.ErrorOccured)
            {
                ErrorEvent("Unable to connect!\n" + socketState.ErrorMessage, "Connection Error");
                socketState.TheSocket.Close();
                return;
            }

            string[] messages = ProcessMessages(socketState, true);

            //Get data if startup info not recieved
            if (messages.Length == 0)
            {
                Networking.GetData(socketState);
                return;
            }

            //Read startup info
            int ID = Int32.Parse(messages[0]);
            int worldSize = Int32.Parse(messages[1]);
            theWorld.PlayerTankID = ID;
            theWorld.Size = worldSize;

            //Change delegate to accept game updates
            socketState.OnNetworkAction = RecieveUpdates;

            //Processes any game updates recieved with startup data
            if (messages.Length > 2)
            {
                string[] remainingMessages = new string[messages.Length - 2];
                for (int i = 0; i <= remainingMessages.Length - 1; i++)
                {
                    remainingMessages[i] = messages[i + 2];
                }

                DeserializeUpdate(remainingMessages);
                OnFrameEvent();
            }

            //Get game updates
            Networking.GetData(socketState);
        }

        /// <summary>
        /// Handles remaining server data after startup data.
        /// </summary>
        /// <param name="socketState">The socket state being used</param>
        private void RecieveUpdates(SocketState socketState)
        {
            //Deserialize the messages
            string[] newMessages = ProcessMessages(socketState);

            DeserializeUpdate(newMessages);

            //draw the frame
            OnFrameEvent();

            //send an update to the server
            SendCommand(socketState);

            //if the program is closing, close the socket
            if (close)
            {
                socketState.TheSocket.Close();
                return;
            }

            //ask for more data
            Networking.GetData(socketState);
        }

        /// <summary>
        /// Sends the control commands to the server.
        /// </summary>
        /// <param name="socketState"></param>
        private void SendCommand(SocketState socketState)
        {
            lock (controlCommand)
            {
                string command = JsonConvert.SerializeObject(controlCommand);
                Networking.Send(socketState.TheSocket, command + "\n");
                //if the previous command was to fire the alt, 
                //remove that command so that it doesn't fire multiple beams in a single click
                if (fireCommand[0] == "alt")
                {
                    fireCommand.Remove("alt");
                    controlCommand.Fire = fireCommand[0];
                }
            }
        }

        /// <summary>
        /// Processes the messages from the SocketState's buffer when it is not from the intial handshake
        /// </summary>
        /// <param name="socketState">The socket state being used</param>
        /// <returns>The messages</returns>
        private string[] ProcessMessages(SocketState socketState)
        {
            return ProcessMessages(socketState, false);
        }

        /// <summary>
        /// Processes the messages from the SocketState's buffer.
        /// </summary>
        /// <param name="socketState">The socket state being used</param>
        /// <param name="initial">True if proccesing messages from the initial handshake</param>
        /// <returns>The messages</returns>
        private string[] ProcessMessages(SocketState socketState, bool initial)
        {
            string initialData = socketState.GetData();

            //Note: Split doesn't include the separator character '\n', and may include a final empty string at the end if the separator is at the end
            string[] messages = initialData.Split('\n');

            //If whole handshake message not recieved, send empty array to prompt to get more data
            if (initial && messages.Length < 3)
            {
                return new string[0];
            }

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
        /// Converts Json recieved from server into game objects.
        /// </summary>
        /// <param name="remainingMessages">The server messages</param>
        private void DeserializeUpdate(string[] remainingMessages)
        {
            List<Object> updatedObjects = new List<object>();

            foreach (string m in remainingMessages)
            {
                if (m.Contains("tank"))
                {
                    Tank t = JsonConvert.DeserializeObject<Tank>(m);
                    updatedObjects.Add(t);
                }
                else if (m.Contains("wall"))
                {
                    Wall w = JsonConvert.DeserializeObject<Wall>(m);
                    updatedObjects.Add(w);
                }
                else if (m.Contains("proj"))
                {
                    Projectile p = JsonConvert.DeserializeObject<Projectile>(m);
                    updatedObjects.Add(p);
                }
                else if (m.Contains("beam"))
                {
                    Beam b = JsonConvert.DeserializeObject<Beam>(m);
                    updatedObjects.Add(b);
                }
                else if (m.Contains("power"))
                {
                    Powerup p = JsonConvert.DeserializeObject<Powerup>(m);
                    updatedObjects.Add(p);
                }
            }

            //Send the newly converted objects to update the world
            theWorld.UpdateObjects(updatedObjects);
        }

        /// <summary>
        /// Registers error event.
        /// </summary>
        /// <param name="connectionError">The error delegate</param>
        public void RegisterHandler(Error connectionError)
        {
            ErrorEvent += connectionError;
        }

        /// <summary>
        /// Registers get name method.
        /// </summary>
        /// <param name="getName">The get name delegate</param>
        public void RegisterHandler(GetName getName)
        {
            GetPlayerName = getName;
        }

        /// <summary>
        /// Registers on frame event.
        /// </summary>
        /// <param name="onFrame">The on frame delegate</param>
        public void RegisterHandler(OnFrame onFrame)
        {
            OnFrameEvent += onFrame;
        }

        /// <summary>
        /// Returns the world.
        /// </summary>
        /// <returns>The world</returns>
        public World GetWorld()
        {
            return theWorld;
        }
        /// <summary>
        /// Closes the socket.
        /// </summary>
        public void Close()
        {
            close = true;
        }
    }
}