﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;

namespace DAL
{
    public class SeasonsDAL
    {
        RoundsDAL roundsDAL = new RoundsDAL();
        public List<Season> LoadData()
        {
            List<Season> seasons = new List<Season>();
            using (DBProjetDataContext db = new DBProjetDataContext())
            {
                var query = from ss in db.Seasons
                            orderby ss.SeasonID descending
                            select ss;

                foreach (var item in query)
                {
                    Season ss = new Season();
                    ss.SeasonID = item.SeasonID;
                    ss.SeasonName = item.SeasonName;
                    ss.StartDate = item.StartDate;
                    ss.EndDate = item.EndDate;

                    seasons.Add(ss);
                }
            }
            return seasons;
        }

        public bool AddData(Season season)
        {
            try
            {
                using (DBProjetDataContext db = new DBProjetDataContext())
                {
                    db.Seasons.InsertOnSubmit(season);
                    db.SubmitChanges();

                    // Tự động tạo rounds sau khi tạo mới 1 season
                    roundsDAL.CreateData(season.SeasonID);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public Season LoadDataByID(int seasonID)
        {
            Season season = new Season();

            using (DBProjetDataContext db = new DBProjetDataContext())
            {
                var query = db.Seasons.Where(ss => ss.SeasonID == seasonID).FirstOrDefault();
                if (query != null)
                {
                    season.SeasonID = query.SeasonID;
                    season.SeasonName = query.SeasonName;
                    season.StartDate = query.StartDate;
                    season.EndDate = query.EndDate;
                }
            }

            return season;
        }
    }
}
