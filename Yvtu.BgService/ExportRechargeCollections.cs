
using ClosedXML.Excel;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.BgService
{
    class ExportRechargeCollections
    {
        private readonly IAppDbContext db;
        private readonly AppBackgroundService service;
        private readonly string fileLocation;

        public ExportRechargeCollections(IAppDbContext db, AppBackgroundService service, string fileLocation)
        {
            this.db = db;
            this.service = service;
            this.fileLocation = fileLocation;
        }

        public async Task<string> ExportAsync()
        {
            await new AppBackgroundServiceRepo(db).StartProcessingAsync(service.Id, true);
            var data = await GetDataAsync();
            if (data == null) return "nodata";
            string file = "Collections" + DateTime.Now.ToString("yyyyMMddHmmss");
            var result = ToExcelFile(data, file);
            if (result)
            {
                //new AppBackgroundServiceRepo(db)
            }
            return file + ".xlsx";
        }

        private async Task<List<RechargeCollection>> GetDataAsync()
        {
            if (service.Source.Id != "collection") return null;
            #region Parameters
            var parameters = new List<OracleParameter>();
            var whereCluase = new StringBuilder();
           
            if (service.Partner.Account > 0)
                {
                    whereCluase.Append(" WHERE pos_acc = :POSAcc");
                    var p = new OracleParameter { ParameterName = "POSAcc", OracleDbType = OracleDbType.Int32, Value = service.Partner.Account };
                    parameters.Add(p);
                }
            if (service.StartDate > DateTime.MinValue && service.StartDate != null)
                    {
                        whereCluase.Append(whereCluase.Length > 0 ? " AND createdon >= :StartDate" : " WHERE createdon >= :StartDate");
                        var p = new OracleParameter { ParameterName = "StartDate", OracleDbType = OracleDbType.Date, Value = service.StartDate };
                        parameters.Add(p);
                    }
            if (service.EndDate > DateTime.MinValue && service.EndDate != null)
                    {
                        whereCluase.Append(whereCluase.Length > 0 ? " AND createdon <= :EndDate" : " WHERE createdon <= :EndDate");
                        var p = new OracleParameter { ParameterName = "EndDate", OracleDbType = OracleDbType.Date, Value = service.EndDate.AddDays(1) };
                        parameters.Add(p);
                    }
            #endregion
            var results = await new RechargeRepo(db, null).GetCollectionsAsync(whereCluase.ToString(), parameters);
            return results;
        }

        public static DataTable GetDataTableFromObjects(List<RechargeCollection> objects)
        {
            if (objects != null && objects.Count > 0)
            {
                Type t = objects[0].GetType();
                DataTable dt = new DataTable(t.Name);
                foreach (PropertyInfo pi in t.GetProperties())
                {
                    dt.Columns.Add(new DataColumn(pi.Name));
                }
                foreach (var o in objects)
                {
                    DataRow dr = dt.NewRow();
                    foreach (DataColumn dc in dt.Columns)
                    {
                        dr[dc.ColumnName] = o.GetType().GetProperty(dc.ColumnName).GetValue(o, null);
                    }
                    dt.Rows.Add(dr);
                }
                return dt;
            }
            return null;
        }
        public  bool ToExcelFile(List<RechargeCollection> data, string filename)
        {
            bool Success = false;
            var dt = GetDataTableFromObjects(data);
            //try
            //{
            XLWorkbook wb = new XLWorkbook();

            wb.Worksheets.Add(dt, "Sheet 1");

            if (filename.Contains("."))
            {
                int IndexOfLastFullStop = filename.LastIndexOf('.');

                filename = filename.Substring(0, IndexOfLastFullStop) + ".xlsx";

            }

            filename = Path.Combine(fileLocation, filename + ".xlsx");

            wb.SaveAs(filename);

            Success = true;

            //}
            //catch (Exception ex)
            //{
            //ex.HandleException();

            //}
            return Success;
        }
    }
}
