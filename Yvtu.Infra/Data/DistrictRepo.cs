using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Infra.Data
{
    public class DistrictRepo
    {
        private readonly IAppDbContext db;

        public DistrictRepo(IAppDbContext db)
        {
            this.db = db;
        }

        public List<District> GetDistricts()
        {
            var districtDataTable = this.db.GetData("Select * from district  order by disorder", null);

            var districts = new List<District>();
            if (districtDataTable != null)
            {
                foreach (DataRow row in districtDataTable.Rows)
                {
                    var district = new District();
                    district.Id = row["disid"] == null ? 0 : int.Parse(row["disid"].ToString());
                    district.Name = row["disname"] == null ? string.Empty : row["disname"].ToString();
                    districts.Add(district);
                }
            }
            return districts;
        }

        public List<District> GetDistrictsByCity (int cityId)
        {
            var districtDataTable = this.db.GetData("Select * from district where cityid ="+ cityId +" order by disorder", null);

            var districts = new List<District>();
            if (districtDataTable != null)
            {
                foreach (DataRow row in districtDataTable.Rows)
                {
                    var district = new District();
                    district.Id = row["disid"] == null ? 0 : int.Parse(row["disid"].ToString());
                    district.Name = row["disname"] == null ? string.Empty : row["disname"].ToString();
                    districts.Add(district);
                }
            }
            return districts;
        }
    }
}
