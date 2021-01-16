
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

        public async Task ExportAsync()
        {
            await new AppBackgroundServiceRepo(db).StartProcessingAsync(service.Id, true);
            service.ActualStartTime = DateTime.Now;
            var colldata = new List<ToExcelSchema.RechargeCollection>();
            var tradata = new List<ToExcelSchema.MoneyTransfer>();
            if (service.Source.Id == "collection")
            {
                colldata = await GetCollectionsDataAsync();
                tradata = null;
            } else if (service.Source.Id == "moneytransfer")
            {
                colldata = null;
                tradata = await GetMoneyTransferDataAsync();
            }
            if (colldata  == null && tradata == null) {
                service.Status.Id = "closed";
                service.StatusTime = DateTime.Now;
                service.RecordCount = 0;
                service.DurationInSec = DateTime.Now.Subtract(service.ActualStartTime).TotalSeconds;
                new AppBackgroundServiceRepo(db).Update(service);
            }
            else
            {
                bool result = false;
                string file = string.Empty;
                if (service.Source.Id == "collection")
                {
                    file = "Collections" + DateTime.Now.ToString("yyyyMMddHmmss");
                    result = ToExcelFile<ToExcelSchema.RechargeCollection>(colldata, file);
                }
                else if (service.Source.Id == "moneytransfer")
                {
                    file = "MoneyTransfer" + DateTime.Now.ToString("yyyyMMddHmmss");
                    result = ToExcelFile<ToExcelSchema.MoneyTransfer>(tradata, file);
                }

                if (result)
                {
                    service.FileName = file + ".xlsx";
                    service.DurationInSec = DateTime.Now.Subtract(service.ActualStartTime).TotalSeconds;
                    var fullFileName = Path.Combine(fileLocation, file + ".xlsx");
                    service.FileLocation = new DirectoryInfo(fullFileName).Parent.Name;
                    FileInfo fi = new FileInfo(fullFileName);
                    if (fi.Exists)
                    {
                        service.FileSize = fi.Length;
                    }
                    service.Status.Id = "closed";
                    service.StatusTime = DateTime.Now;
                    new AppBackgroundServiceRepo(db).Update(service);
                }else
                {
                    service.Status.Id = "error";
                    service.StatusTime = DateTime.Now;
                    service.DurationInSec = DateTime.Now.Subtract(service.ActualStartTime).TotalSeconds;
                    new AppBackgroundServiceRepo(db).Update(service);
                }
            }
        }

        private async Task<List<ToExcelSchema.RechargeCollection>> GetCollectionsDataAsync()
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
            var results = await new RechargeRepo(db, null).GetCollectionsForExcelAsync(whereCluase.ToString(), parameters);
            return results;
        }

        private async Task<List<ToExcelSchema.MoneyTransfer>> GetMoneyTransferDataAsync()
        {
            if (service.Source.Id != "moneytransfer") return null;
            #region Parameters
            var parameters = new List<OracleParameter>();
            var whereCluase = new StringBuilder();

            if (service.Partner.Account > 0)
            {
                whereCluase.Append(" WHERE (part_acc = :POSAcc OR creator_acc = :POSAcc2) ");
                var p = new OracleParameter { ParameterName = "POSAcc", OracleDbType = OracleDbType.Int32, Value = service.Partner.Account };
                var p2 = new OracleParameter { ParameterName = "POSAcc2", OracleDbType = OracleDbType.Int32, Value = service.Partner.Account };
                parameters.Add(p);
                parameters.Add(p2);
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
            var results = await new MoneyTransferRepo(db, null, null).GetMoneyTransferForExcelAsync(whereCluase.ToString(), parameters);
            return results;
        }

        public static DataTable GetDataTableFromObjects<T>(List<T> objects)
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
        public  bool ToExcelFile<T>(List<T> data, string filename)
        {
            service.RecordCount = data.Count;
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
