﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI.WebControls;

namespace DAL
{
    public class PlayersDAL
    {
        ConnectDatabase connectDB = new ConnectDatabase();
        public string connectionString = "Data Source=LAPTOP-5I4BGSNV\\HOANGVU;Initial Catalog=DBProject.Net;Integrated Security=True";
        public SqlConnection connection = null;

        public List<Player> LoadDataByClubID(int clubID)
        {
            List<Player> players = new List<Player>();
            using (DBProjetDataContext db = new DBProjetDataContext())
            {
                var query = from p in db.Players
                            join c in db.Clubs on p.ClubID equals c.ClubID
                            where c.ClubID == clubID
                            select new
                            {
                                playerID = p.PlayerID,
                                clubID = c.ClubID,
                                playerName = p.PlayerName,
                                image = p.Image,
                                number = p.Number,
                                dob = p.DOB,
                                country = p.Country,
                                position = p.Position
                            };

                foreach (var item in query)
                {
                    Player player = new Player();
                    player.PlayerID = item.playerID;
                    player.Image = item.image;
                    player.PlayerName = item.playerName;
                    player.ClubID = item.clubID;
                    player.Number = item.number;
                    player.Country = item.country;
                    player.DOB = item.dob;
                    player.Position = item.position;

                    players.Add(player);
                }

            }
            return players;
        }


        public bool AddData(Player player)
        {
            try
            {
                using (DBProjetDataContext db = new DBProjetDataContext())
                {
                    db.Players.InsertOnSubmit(player);
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }


        public bool DeleteData(int playerID)
        {
            try
            {
                using (DBProjetDataContext db = new DBProjetDataContext())
                {
                    var query = db.Players.Where(p => p.PlayerID == playerID).FirstOrDefault();
                    if (query != null)
                    {
                        db.Players.DeleteOnSubmit(query);
                        db.SubmitChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool DeleteAllData(int clubID)
        {
            try
            {
                using (DBProjetDataContext db = new DBProjetDataContext())
                {
                    var query = db.Players.Where(p => p.ClubID == clubID);
                    if (query != null)
                    {
                        db.Players.DeleteAllOnSubmit(query);
                        db.SubmitChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public Player LoadDataByPlayerID(int playerID)
        {
            Player player = new Player();
            using (DBProjetDataContext db = new DBProjetDataContext())
            {
                var query = db.Players.Where(p => p.PlayerID == playerID).FirstOrDefault();

                player.PlayerID = query.PlayerID;
                player.Image = query.Image;
                player.PlayerName = query.PlayerName;
                player.ClubID = player.ClubID;
                player.Number = query.Number;
                player.Country = query.Country;
                player.DOB = query.DOB;
                player.Weight = query.Weight;
                player.Height = query.Height;
                player.Salary = query.Salary;
                player.Position = query.Position;
                player.Foot = query.Foot;
            }
            return player;
        }

        public bool EditData(Player player)
        {
            try
            {
                using (DBProjetDataContext db = new DBProjetDataContext())
                {
                    int id = player.PlayerID;
                    var query = db.Players.Where(p => p.PlayerID == id).FirstOrDefault();
                    if (query != null)
                    {
                        query.PlayerName = player.PlayerName;
                        query.ClubID = player.ClubID;
                        query.Image = player.Image;
                        query.Number = player.Number;
                        query.Country = player.Country;
                        query.DOB = player.DOB;
                        query.Height = player.Height;
                        query.Weight = player.Weight;
                        query.Salary = player.Salary;
                        query.Position = player.Position;
                        query.Foot = player.Foot;

                        db.SubmitChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }


        public List<Player> SearchPlayer(string keyword, int clubID)
        {
            List<Player> players = new List<Player>();
            using (DBProjetDataContext db = new DBProjetDataContext())
            {
                try
                {
                    connectDB.OpenConnect();

                    SqlCommand cmd = connectDB.CreateCommand("usp_SearchPlayer", CommandType.StoredProcedure);
                    cmd.Parameters.AddWithValue("@Keyword", keyword);
                    cmd.Parameters.AddWithValue("@ClubID", clubID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        players = ReadDatasFromReader(reader);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    connectDB.CloseConnect();
                }
            }
            return players;
        }

        // Đọc dữ liệu từ SqlDataReader và chuyển đổi thành danh sách các đối tượng Player.
        private List<Player> ReadDatasFromReader(SqlDataReader reader)
        {
            List<Player> players = new List<Player>();
            while (reader.Read())
            {
                Player player = new Player();
                player.PlayerID = Convert.ToInt32(reader["PlayerID"]);
                player.Image = reader["Image"].ToString();
                player.PlayerName = reader["PlayerName"].ToString();
                player.ClubID = Convert.ToInt32(reader["ClubID"]);
                player.Number = Convert.ToInt32(reader["Number"]);
                player.Country = reader["Country"].ToString();
                player.DOB = Convert.ToDateTime(reader["DOB"]);
                player.Position = reader["Position"].ToString();

                players.Add(player);
            }
            return players;
        }

        

        
    }
}