// Authors: Aidan Lethaby and Sean Richens, December 2019
// Controller for CS 3500 TankWars datbase
// University of Utah


using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using TankWars;

namespace Controller
{
    class DatabaseController
    {

        private static readonly string connectionString = "server=atr.eng.utah.edu;database=cs3500_u1205811;uid=cs3500_u1205811;password=secure";

        /// <summary>
        /// Returns a dictionary of all the games in the database
        /// returns null if there was a problem
        /// </summary>
        /// <returns></returns>
        public static Dictionary<uint, GameModel> RetrieveAllGames()
        {
            //Request all games

            //This is the dictionary that holds the games
            Dictionary<uint, GameModel> games = new Dictionary<uint, GameModel>();

            // Connect to the DB
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    // Open a connection
                    conn.Open();

                    // Create a command
                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText = "select * from Games natural join PlayerStats where Games.ID = PlayerStats.ID order by Games.ID desc";

                    // Execute the command and cycle through the DataReader object
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        bool ReadLine = reader.Read();
                        while (ReadLine)
                        {
                            //Make a new game object
                            GameModel g = new GameModel((uint)reader["ID"], (uint)reader["Duration"]);
                            uint ID = g.ID;
                            games.Add(ID, g);

                            //Get all the players for that game
                            do
                            {
                                g.AddPlayer((string)reader["PlayerName"], (uint)reader["PlayerScore"], (uint)reader["PlayerAccuracy"]);

                                ReadLine = reader.Read();
                            } while (ReadLine && (uint)reader["ID"] == ID);

                            //If a new ID is found, go back and make a new game
                            //If there are no more lines to read, finish the loop
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
            }

            //Return a dictionary<ID, GameModel>
            return games;
        }

        /// <summary>
        /// Retrieves the data of all the games for one player
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static List<SessionModel> RetrievePlayerScores(string name)
        {
            //This is the list that holds the games
            List<SessionModel> sessions = new List<SessionModel>();

            // Connect to the DB
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    // Open a connection
                    conn.Open();

                    // Create a command
                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText = $"select * from Games natural join PlayerStats where Games.ID = PlayerStats.ID and PlayerStats.PlayerName = \"{name}\" order by Games.ID desc";

                    // Execute the command and cycle through the DataReader object
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //Make a new session object
                            SessionModel s = new SessionModel((uint)reader["ID"], (uint)reader["Duration"], (uint)reader["PlayerScore"], (uint)reader["PlayerAccuracy"]);
                            sessions.Add(s);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
            }

            return sessions;
        }

        public static void UpdateDatabase(int duration, List<PlayerModel> players)
        {
            // Connect to the DB
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    // Open a connection
                    conn.Open();

                    // Create a command
                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText = $"INSERT INTO `cs3500_u1205811`.`Games` (`Duration`) VALUES ('{duration}');";
                    command.ExecuteNonQuery();

                    command.CommandText = $"select last_insert_id();";
                    int gameID;
                    // Execute the command and cycle through the DataReader object
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        gameID = Convert.ToInt32(reader[0]);
                    }

                    foreach (PlayerModel p in players)
                    {
                        command.CommandText = $"INSERT IGNORE INTO `cs3500_u1205811`.`PlayerStats` (`ID`, `PlayerName`, `PlayerScore`, `PlayerAccuracy`) VALUES ('{gameID}', '{p.Name}', '{p.Score}', '{p.Accuracy}');";
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
