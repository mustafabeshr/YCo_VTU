using Oracle.ManagedDataAccess.Client;
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
                    city.Id = row["cityid"] == DBNull.Value ? 0 : int.Parse(row["cityid"].ToString());
                    city.Name = row["cityname"] == DBNull.Value ? string.Empty : row["cityname"].ToString();
                    cities.Add(city);
                }
            }
            return cities;
        }

        public City GetCity(int id)
        {
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "cityId", OracleDbType = OracleDbType.Int32,  Value = id },
            };
            var CityDataTable = this.db.GetData("Select * from City where cityid = :cityId", parameters);
            var city = new City();
            if (CityDataTable != null)
            {
                DataRow row = CityDataTable.Rows[0];
                city.Id = row["cityid"] == DBNull.Value ? 0 : int.Parse(row["cityid"].ToString());
                city.Name = row["cityname"] == DBNull.Value ? string.Empty : row["cityname"].ToString();
                city.Order = row["cityorder"] == DBNull.Value ? byte.MinValue : byte.Parse(row["cityorder"].ToString());
            }
            return city;
        }
    }
}
