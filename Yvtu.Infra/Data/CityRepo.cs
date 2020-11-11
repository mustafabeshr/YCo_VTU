using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Infra.Data
{
    public class CityRepo
    {
        private readonly IAppDbContext db;

        public CityRepo(IAppDbContext db)
        {
            this.db = db;
        }

        public List<City> GetCities()
        {
            var CityDataTable = this.db.GetData("Select * from city  order by cityorder", null);

            var cities = new List<City>();
            if (CityDataTable != null)
            {
                foreach (DataRow row in CityDataTable.Rows)
                {
                    var city = new City();
                    city.Id = row["cityid"] == null ? 0 : int.Parse(row["cityid"].ToString());
                    city.Name = row["cityname"] == null ? string.Empty : row["cityname"].ToString();
                    cities.Add(city);
                }
            }
            return cities;
        }
    }
}
