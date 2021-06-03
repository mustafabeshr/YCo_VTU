using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Yvtu.Core.Entities;
using Yvtu.Core.Queries;
using Yvtu.Core.rpt;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Infra.Data
{
    public class MoneyTransferRepo
    {
        private readonly IAppDbContext db;
        private readonly IPartnerManager partnerManager;
        private readonly IPartnerActivityRepo partnerActivityRepo;

        public MoneyTransferRepo(IAppDbContext db, IPartnerManager partnerManager, 
            IPartnerActivityRepo partnerActivityRepo)
        {
            this.db = db;
            this.partnerManager = partnerManager;
            this.partnerActivityRepo = partnerActivityRepo;
        }

        public OpertionResult Create(MoneyTransfer transfer)
        {
            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                 new OracleParameter{ ParameterName = "v_part_id", OracleDbType = OracleDbType.Varchar2,  Value = transfer.Partner.Id },
                 new OracleParameter{ ParameterName = "v_pay_type",OracleDbType = OracleDbType.Varchar2,  Value = transfer.PayType.Id },
                 new OracleParameter{ ParameterName = "v_pay_no",OracleDbType = OracleDbType.NVarchar2,  Value = transfer.PayNo },
                 new OracleParameter{ ParameterName = "v_pay_date",OracleDbType = OracleDbType.Date,  Value = transfer.PayDate },
                 new OracleParameter{ ParameterName = "v_bank_name",OracleDbType = OracleDbType.NVarchar2,  Value = transfer.PayBank },
                 new OracleParameter{ ParameterName = "v_createdby",OracleDbType = OracleDbType.Varchar2,  Value = transfer.CreatedBy.Id },
                 new OracleParameter{ ParameterName = "v_access_channel",OracleDbType = OracleDbType.Varchar2,  Value = transfer.AccessChannel.Id },
                 new OracleParameter{ ParameterName = "v_amount",OracleDbType = OracleDbType.Decimal,  Value = transfer.Amount },
                 new OracleParameter{ ParameterName = "v_bill_no",OracleDbType = OracleDbType.Varchar2,  Value = transfer.BillNo },
                 new OracleParameter{ ParameterName = "v_request_no",OracleDbType = OracleDbType.Varchar2,  Value = transfer.RequestNo },
                 new OracleParameter{ ParameterName = "v_request_amt",OracleDbType = OracleDbType.Decimal,  Value = transfer.RequestAmount },
                 new OracleParameter{ ParameterName = "v_note",OracleDbType = OracleDbType.NVarchar2,  Value = transfer.Note },
                 new OracleParameter{ ParameterName = "v_api_trans",OracleDbType = OracleDbType.Int64,  Value = transfer.ApiTransaction }
                };
                #endregion
                db.ExecuteStoredProc("pk_financial.fn_MoneyTransfer", parameters);
                var result = int.Parse(parameters.Find(x => x.ParameterName == "retVal").Value.ToString());

                if (result > 0)
                {
                    return new OpertionResult { AffectedCount = result, Success = true, Error = string.Empty };
                }
                else
                {
                    return new OpertionResult { AffectedCount = result, Success = false, Error = string.Empty };
                }
            }
            catch (Exception ex)
            {
                return new OpertionResult { AffectedCount = -1, Success = false, Error = ex.Message };
            }
        }

        public MoneyTransfer GetSingleOrDefault(int id)
        {
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "MoneyTranferId", OracleDbType = OracleDbType.Int32,  Value = id }
            };
            var masterDataTable = this.db.GetData("Select * from MONEY_TRANSFER  where trans_id=:MoneyTranferId ", parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            DataRow row = masterDataTable.Rows[0];
            var moneyTransfer = ConvertDataRowToMoneyTransfer(row);
            return moneyTransfer;
        }

        public async Task<MoneyTransferQueryDto> MTQuery(MoneyTransferQueryDto param, int count = 200)
        {
            var WhereClause = new StringBuilder();
            var parameters = new List<OracleParameter>();
            if (param.QId > 0)
            {
                var parm = new OracleParameter { ParameterName = "TransId", OracleDbType = OracleDbType.Int32, Value = param.QId };
                WhereClause.Append(" WHERE trans_id=:TransId ");
                parameters.Add(parm);
            }
            if (!string.IsNullOrEmpty(param.QPartnerId))
            {
                if (param.QListTypeId == "all")
                {
                    if (param.QScope == "CurOpOnly")
                    {
                        WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE part_id=:PartId OR createdby=:PartId "
                        : " AND part_id=:PartId OR createdby=:PartId  ");
                        var parm = new OracleParameter { ParameterName = "PartId", OracleDbType = OracleDbType.Varchar2, Value = param.QueryUser };
                        parameters.Add(parm);
                    }
                    else if (param.QScope == "Exclusive")
                    {
                        WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) 
                        ? " WHERE (part_id=:PartId OR createdby=:PartId) AND (exists (select 1 from partner where (partner.partner_id = t.part_id or partner.partner_id = t.createdby) and partner.ref_partner = '"+ param.QueryUser + "'))"
                        : " AND (part_id=:PartId OR createdby=:PartId)  AND (exists (select 1 from partner where (partner.partner_id = t.part_id or partner.partner_id = t.createdby) and partner.ref_partner = '" + param.QueryUser + "'))");
                        var parm = new OracleParameter { ParameterName = "PartId", OracleDbType = OracleDbType.Varchar2, Value = param.QueryUser };
                        parameters.Add(parm);
                    }
                    else
                    {
                      WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE part_id=:PartId OR createdby=:PartId " 
                        : " AND part_id=:PartId OR createdby=:PartId  ");
                      var parm = new OracleParameter { ParameterName = "PartId", OracleDbType = OracleDbType.Varchar2, Value = param.QPartnerId };
                      parameters.Add(parm);
                    }
                } else if (param.QListTypeId == "debit")
                {
                    if (param.QScope == "CurOpOnly")
                    {
                        WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE part_id=:PartId "
                        : " AND part_id=:PartId  ");
                        var parm = new OracleParameter { ParameterName = "PartId", OracleDbType = OracleDbType.Varchar2, Value = param.QueryUser };
                        parameters.Add(parm);
                    }
                    else if (param.QScope == "Exclusive")
                    {
                        WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString())
                        ? " WHERE (part_id=:PartId) AND (exists (select 1 from partner where (partner.partner_id = t.part_id) and partner.ref_partner = '" + param.QueryUser + "'))"
                        : " AND (part_id=:PartId)  AND (exists (select 1 from partner where (partner.partner_id = t.part_id) and partner.ref_partner = '" + param.QueryUser + "'))");
                        var parm = new OracleParameter { ParameterName = "PartId", OracleDbType = OracleDbType.Varchar2, Value = param.QueryUser };
                        parameters.Add(parm);
                    }
                    else
                    {
                        WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE part_id=:PartId " : " AND part_id=:PartId  ");
                        var parm = new OracleParameter { ParameterName = "PartId", OracleDbType = OracleDbType.Varchar2, Value = param.QPartnerId };
                        parameters.Add(parm);

                    }
                }
                else if (param.QListTypeId == "credit")
                {
                    if (param.QScope == "CurOpOnly")
                    {
                        WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE createdby=:PartId "
                        : " AND createdby=:PartId  ");
                        var parm = new OracleParameter { ParameterName = "PartId", OracleDbType = OracleDbType.Varchar2, Value = param.QueryUser };
                        parameters.Add(parm);
                    }
                    else if (param.QScope == "Exclusive")
                    {
                        WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString())
                        ? " WHERE (part_id=:PartId) AND (exists (select 1 from partner where (partner.partner_id = t.createdby) and partner.ref_partner = '" + param.QueryUser + "'))"
                        : " AND (part_id=:PartId)  AND (exists (select 1 from partner where (partner.partner_id = t.createdby) and partner.ref_partner = '" + param.QueryUser + "'))");
                        var parm = new OracleParameter { ParameterName = "PartId", OracleDbType = OracleDbType.Varchar2, Value = param.QueryUser };
                        parameters.Add(parm);
                    }
                    else
                    {
                        WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE createdby=:PartId " : " AND createdby=:PartId  ");
                        var parm = new OracleParameter { ParameterName = "PartId", OracleDbType = OracleDbType.Varchar2, Value = param.QPartnerId };
                        parameters.Add(parm);
                    }
                }
            }
            if(param.QFromDate > DateTime.MinValue && param.QFromDate != null)
            {
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE trunc(createdon)>=:StartDate " : " AND trunc(createdon)>=:StartDate   ");
                var parm = new OracleParameter { ParameterName = "StartDate", OracleDbType = OracleDbType.Date, Value = param.QFromDate };
                parameters.Add(parm);
            }
            if (param.QToDate > DateTime.MinValue && param.QToDate != null)
            {
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE createdon<=:EndDate " : " AND createdon<=:EndDate   ");
                var parm = new OracleParameter { ParameterName = "EndDate", OracleDbType = OracleDbType.Date, Value = param.QToDate };
                parameters.Add(parm);
            }

            WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE ROWNUM <= " + count : " AND ROWNUM <= " + count);

            var masterDataTable = await this.db.GetDataAsync("Select * from v_money_transfer  " + WhereClause + " order by createdon desc", parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            var moneyTransfer = new List<MoneyTransferDetailQueryDto>();
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = ConvertDataRowToDataModel(row);
                moneyTransfer.Add(obj);
            }
            param.Results = moneyTransfer;
            return param;
        }

        public MoneyTransferQueryDto MTQueryWithPaging(MoneyTransferQueryDto param)
        {
            string WhereClause = string.Empty;
            var parameters = BuildCriteria(param, ref WhereClause);

            var strSqlStatment = new StringBuilder();
            strSqlStatment.Append("Select * from ( ");
            strSqlStatment.Append("select rownum as seq , main_data.* from ( ");
            strSqlStatment.Append("Select * from v_money_transfer  " + WhereClause + " order by createdon desc ");
            strSqlStatment.Append(") main_data ) ");
            strSqlStatment.Append($"WHERE seq > ({param.Paging.PageNo - 1}) * {param.Paging.PageSize} AND ROWNUM <= {param.Paging.PageSize}");
            var masterDataTable = this.db.GetData(strSqlStatment.ToString(), parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            var moneyTransfer = new List<MoneyTransferDetailQueryDto>();
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = ConvertDataRowToDataModel(row, true);
                moneyTransfer.Add(obj);
            }
            param.Results = moneyTransfer;
            
            return param;
        }

        public async Task<List<MoneyTransfer>> GetMoneyTransfers(int account, DateTime startDate, DateTime endDate, int count)
        {
            var WhereClause = new StringBuilder();
            var parameters = new List<OracleParameter>();
            if (account <= 0) return null;
            var accountParameter1 = new OracleParameter { ParameterName = "Account1", OracleDbType = OracleDbType.Int32, Value = account };
            var accountParameter2 = new OracleParameter { ParameterName = "Account2", OracleDbType = OracleDbType.Int32, Value = account };
            WhereClause.Append(" WHERE (part_acc=:Account1 OR creator_acc=:Account2) ");
            parameters.Add(accountParameter1);
            parameters.Add(accountParameter2);

            if (startDate > DateTime.MinValue)
            {
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE trunc(createdon)>=:StartDate " : " AND trunc(createdon)>=:StartDate   ");
                var parm = new OracleParameter { ParameterName = "StartDate", OracleDbType = OracleDbType.Date, Value = startDate };
                parameters.Add(parm);
            }
            if (endDate > DateTime.MinValue)
            {
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE createdon<=:EndDate " : " AND createdon<=:EndDate   ");
                var parm = new OracleParameter { ParameterName = "EndDate", OracleDbType = OracleDbType.Date, Value = endDate };
                parameters.Add(parm);
            }
            WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE ROWNUM <= " + count : " AND ROWNUM <= " + count);

            var masterDataTable = await this.db.GetDataAsync("Select * from v_money_transfer  " + WhereClause + " order by createdon", parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            var moneyTransfers = new List<MoneyTransfer>();
           
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = ConvertDataRowToMoneyTransfer(row);
                moneyTransfers.Add(obj);
            }
            return moneyTransfers;
        }

        private List<OracleParameter> BuildCriteria(MoneyTransferQueryDto param, ref string criteria)
        {
            var WhereClause = new StringBuilder();
            var parameters = new List<OracleParameter>();
            if (param.QId > 0)
            {
                var parm = new OracleParameter { ParameterName = "TransId", OracleDbType = OracleDbType.Int32, Value = param.QId };
                WhereClause.Append(" WHERE trans_id=:TransId ");
                parameters.Add(parm);
            }
            if (!string.IsNullOrEmpty(param.QPartnerId))
            {
                if (param.QListTypeId == "all")
                {
                    if (param.QScope == "CurOpOnly")
                    {
                        WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE (part_id=:PartId OR createdby=:PartId) "
                        : " AND (part_id=:PartId OR createdby=:PartId2)  ");
                        var parm = new OracleParameter { ParameterName = "PartId", OracleDbType = OracleDbType.Varchar2, Value = param.QueryUser };
                        var parm2 = new OracleParameter { ParameterName = "PartId2", OracleDbType = OracleDbType.Varchar2, Value = param.QueryUser };
                        parameters.Add(parm);
                        parameters.Add(parm2);
                    }
                    else if (param.QScope == "Exclusive")
                    {
                        WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString())
                        ? " WHERE (part_id=:PartId OR createdby=:PartId2) AND (exists (select 1 from partner where (partner.partner_id = t.part_id or partner.partner_id = t.createdby) and partner.ref_partner = '" + param.QueryUser + "'))"
                        : " AND (part_id=:PartId OR createdby=:PartId2)  AND (exists (select 1 from partner where (partner.partner_id = t.part_id or partner.partner_id = t.createdby) and partner.ref_partner = '" + param.QueryUser + "'))");
                        var parm = new OracleParameter { ParameterName = "PartId", OracleDbType = OracleDbType.Varchar2, Value = param.QueryUser };
                        var parm2 = new OracleParameter { ParameterName = "PartId2", OracleDbType = OracleDbType.Varchar2, Value = param.QueryUser };
                        parameters.Add(parm);
                        parameters.Add(parm2);
                    }
                    else
                    {
                        WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE (part_id=:PartId OR createdby=:PartId2) "
                          : " AND (part_id=:PartId OR createdby=:PartId2)  ");
                        var parm = new OracleParameter { ParameterName = "PartId", OracleDbType = OracleDbType.Varchar2, Value = param.QPartnerId };
                        var parm2 = new OracleParameter { ParameterName = "PartId2", OracleDbType = OracleDbType.Varchar2, Value = param.QPartnerId };
                        parameters.Add(parm);
                        parameters.Add(parm2);
                    }
                }
                else if (param.QListTypeId == "debit")
                {
                    if (param.QScope == "CurOpOnly")
                    {
                        WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE part_id=:PartId "
                        : " AND part_id=:PartId  ");
                        var parm = new OracleParameter { ParameterName = "PartId", OracleDbType = OracleDbType.Varchar2, Value = param.QueryUser };
                        parameters.Add(parm);
                    }
                    else if (param.QScope == "Exclusive")
                    {
                        WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString())
                        ? " WHERE (part_id=:PartId) AND (exists (select 1 from partner where (partner.partner_id = t.part_id) and partner.ref_partner = '" + param.QueryUser + "'))"
                        : " AND (part_id=:PartId)  AND (exists (select 1 from partner where (partner.partner_id = t.part_id) and partner.ref_partner = '" + param.QueryUser + "'))");
                        var parm = new OracleParameter { ParameterName = "PartId", OracleDbType = OracleDbType.Varchar2, Value = param.QueryUser };
                        parameters.Add(parm);
                    }
                    else
                    {
                        WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE part_id=:PartId " : " AND part_id=:PartId  ");
                        var parm = new OracleParameter { ParameterName = "PartId", OracleDbType = OracleDbType.Varchar2, Value = param.QPartnerId };
                        parameters.Add(parm);

                    }
                }
                else if (param.QListTypeId == "credit")
                {
                    if (param.QScope == "CurOpOnly")
                    {
                        WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE createdby=:PartId "
                        : " AND createdby=:PartId  ");
                        var parm = new OracleParameter { ParameterName = "PartId", OracleDbType = OracleDbType.Varchar2, Value = param.QueryUser };
                        parameters.Add(parm);
                    }
                    else if (param.QScope == "Exclusive")
                    {
                        WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString())
                        ? " WHERE (part_id=:PartId) AND (exists (select 1 from partner where (partner.partner_id = t.createdby) and partner.ref_partner = '" + param.QueryUser + "'))"
                        : " AND (part_id=:PartId)  AND (exists (select 1 from partner where (partner.partner_id = t.createdby) and partner.ref_partner = '" + param.QueryUser + "'))");
                        var parm = new OracleParameter { ParameterName = "PartId", OracleDbType = OracleDbType.Varchar2, Value = param.QueryUser };
                        parameters.Add(parm);
                    }
                    else
                    {
                        WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE createdby=:PartId " : " AND createdby=:PartId  ");
                        var parm = new OracleParameter { ParameterName = "PartId", OracleDbType = OracleDbType.Varchar2, Value = param.QPartnerId };
                        parameters.Add(parm);
                    }
                }
               
            }
            if (param.QFromDate > DateTime.MinValue && param.QFromDate != null)
            {
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE trunc(createdon)>=:StartDate " : " AND trunc(createdon)>=:StartDate   ");
                var parm = new OracleParameter { ParameterName = "StartDate", OracleDbType = OracleDbType.Date, Value = param.QFromDate };
                parameters.Add(parm);
            }
            if (param.QToDate > DateTime.MinValue && param.QToDate != null)
            {
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE trunc(createdon)<=:EndDate " : " AND trunc(createdon)<=:EndDate   ");
                var parm = new OracleParameter { ParameterName = "EndDate", OracleDbType = OracleDbType.Date, Value = param.QToDate };
                parameters.Add(parm);
            }
            criteria = WhereClause.ToString();
            return parameters;
        }

        public int GetCount(MoneyTransferQueryDto param)
        {
            string WhereClause = string.Empty;
            var parameters = BuildCriteria(param, ref WhereClause);
            var strSqlStatment = new StringBuilder();
            strSqlStatment.Append($"Select count(*) val from v_money_transfer  { WhereClause }" );
            var count = this.db.GetIntScalarValue(strSqlStatment.ToString(), parameters);
            return count;
        }

        public MoneyTransfer GetByApiTransaction(int apiTransId, int account)
        {
            string WhereClause = string.Empty;
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "apiTransId", OracleDbType = OracleDbType.Int32,  Value = apiTransId },
                 new OracleParameter{ ParameterName = "account", OracleDbType = OracleDbType.Int32,  Value = account }
            };
            var masterDataTable = this.db.GetData("Select * from MONEY_TRANSFER  where api_trans=:apiTransId and creator_acc = :account and access_channel = 'api'", parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;
            DataRow row = masterDataTable.Rows[0];
            var moneyTransfer = ConvertDataRowToMoneyTransfer(row);
            return moneyTransfer;
        }
        private MoneyTransfer ConvertDataRowToMoneyTransfer(DataRow row)
        {
            var moneyTransfer = new MoneyTransfer();
            moneyTransfer.Id = row["trans_id"] == DBNull.Value ? -1 : int.Parse(row["trans_id"].ToString());
            var partAccount = row["part_acc"] == DBNull.Value ? -1 : int.Parse(row["part_acc"].ToString());
            var creatorAccount = row["creator_acc"] == DBNull.Value ? -1 : int.Parse(row["creator_acc"].ToString());
            var partRoleId = row["part_role_id"] == DBNull.Value ? -1 : int.Parse(row["part_role_id"].ToString());
            var creatorRoleId = row["creator_role_id"] == DBNull.Value ? -1 : int.Parse(row["creator_role_id"].ToString());
            var partBalance = row["part_bal"] == DBNull.Value ? -1 : double.Parse(row["part_bal"].ToString());
            var creatorBalance = row["creator_bal"] == DBNull.Value ? 0 : double.Parse(row["creator_bal"].ToString());

            var payType = row["pay_type"] == DBNull.Value ? string.Empty : row["pay_type"].ToString();
            moneyTransfer.PayType = new CommonCodeRepo(db).GetCodesById(payType, "pay.type");
            moneyTransfer.PayNo = row["pay_no"] == DBNull.Value ? string.Empty : row["pay_no"].ToString();
            moneyTransfer.PayBank = row["bank_name"] == DBNull.Value ? string.Empty : row["bank_name"].ToString();
            moneyTransfer.PayDate = row["pay_date"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["pay_date"].ToString());
            moneyTransfer.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
            var accessChannel = row["access_channel"] == DBNull.Value ? string.Empty : row["access_channel"].ToString();
            moneyTransfer.AccessChannel = new CommonCodeRepo(db).GetCodesById(accessChannel, "access.channel");
            moneyTransfer.Amount = row["amount"] == DBNull.Value ? 0 : double.Parse(row["amount"].ToString());
            moneyTransfer.TaxPercent = row["tax_per"] == DBNull.Value ? 0 : double.Parse(row["tax_per"].ToString());
            moneyTransfer.TaxAmount = row["tax_amt"] == DBNull.Value ? 0 : double.Parse(row["tax_amt"].ToString());
            moneyTransfer.BonusPercent = row["bonus_per"] == DBNull.Value ? 0 : double.Parse(row["bonus_per"].ToString());
            moneyTransfer.BounsAmount = row["bonus_amt"] == DBNull.Value ? 0 : double.Parse(row["bonus_amt"].ToString());
            moneyTransfer.BounsTaxPercent = row["bonus_tax"] == DBNull.Value ? 0 : double.Parse(row["bonus_tax"].ToString());
            moneyTransfer.BounsTaxAmount = row["bonus_tax_amt"] == DBNull.Value ? 0 : double.Parse(row["bonus_tax_amt"].ToString());
            moneyTransfer.ReceivedAmount = row["received_amt"] == DBNull.Value ? 0 : double.Parse(row["received_amt"].ToString());
            moneyTransfer.NetAmount = row["net_amount"] == DBNull.Value ? 0 : double.Parse(row["net_amount"].ToString());
            moneyTransfer.BillNo = row["bill_no"] == DBNull.Value ? string.Empty : row["bill_no"].ToString();
            moneyTransfer.RequestNo = row["request_no"] == DBNull.Value ? string.Empty : row["request_no"].ToString();
            moneyTransfer.RequestAmount = row["request_amt"] == DBNull.Value ? 0 : double.Parse(row["request_amt"].ToString());
            moneyTransfer.Note = row["note"] == DBNull.Value ? string.Empty : row["note"].ToString();
            moneyTransfer.Adjusted = row["adjusted"] == DBNull.Value ? false : row["adjusted"].ToString() == "1" ? true : false;
            moneyTransfer.AdjustmentNo = row["adjust_id"] == DBNull.Value ? 0 : int.Parse(row["adjust_id"].ToString());
            moneyTransfer.ApiTransaction = row["api_trans"] == DBNull.Value ? 0 : long.Parse(row["api_trans"].ToString());
            moneyTransfer.FixedFactor = row["fixed_factor"] == DBNull.Value ? 0 : double.Parse(row["fixed_factor"].ToString());

            moneyTransfer.Partner = partnerManager.GetPartnerByAccount(partAccount);

            if (moneyTransfer.Partner != null)
            {
                moneyTransfer.Partner.Role = new RoleRepo(db, partnerActivityRepo).GetRole(partRoleId);
                moneyTransfer.Partner.Balance = partBalance;
                moneyTransfer.Partner.Id = row["part_id"] == DBNull.Value ? string.Empty : row["part_id"].ToString();
            }
            moneyTransfer.CreatedBy = partnerManager.GetPartnerByAccount(creatorAccount);
            if (moneyTransfer.CreatedBy != null)
            {
                moneyTransfer.CreatedBy.Role = new RoleRepo(db, partnerActivityRepo).GetRole(creatorRoleId);
                moneyTransfer.CreatedBy.Balance = creatorBalance;
                moneyTransfer.CreatedBy.Id = row["createdby"] == DBNull.Value ? string.Empty : row["createdby"].ToString();
            }
            return moneyTransfer;
        }
        private MoneyTransferDetailQueryDto ConvertDataRowToDataModel(DataRow row , bool includeSeq = false)
        {
            var dataModel = new MoneyTransferDetailQueryDto();
            if (includeSeq) dataModel.Seq = row["seq"] == DBNull.Value ? -1 : int.Parse(row["seq"].ToString());
            dataModel.Id = row["trans_id"] == DBNull.Value ? -1 : int.Parse(row["trans_id"].ToString());
            dataModel.PartnerId = row["part_id"] == DBNull.Value ? string.Empty : row["part_id"].ToString();
            dataModel.PartnerName = row["part_name"] == DBNull.Value ? string.Empty : row["part_name"].ToString();
            dataModel.PartnerRoleId = row["part_role_id"] == DBNull.Value ? -1 : int.Parse(row["part_role_id"].ToString());
            dataModel.PartnerRoleName = row["part_role_name"] == DBNull.Value ? string.Empty : row["part_role_name"].ToString();
            dataModel.PartnerAccount = row["part_acc"] == DBNull.Value ? -1 : int.Parse(row["part_acc"].ToString());
            dataModel.PartnerBalance = row["part_bal"] == DBNull.Value ? 0 : double.Parse(row["part_bal"].ToString());
            dataModel.PayTypeId = row["pay_type"] == DBNull.Value ? string.Empty : row["pay_type"].ToString();
            dataModel.PayTypeName = row["pay_type_name"] == DBNull.Value ? string.Empty : row["pay_type_name"].ToString();
            dataModel.PayNo = row["pay_no"] == DBNull.Value ? string.Empty : row["pay_no"].ToString();
            dataModel.PayDate = row["pay_date"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["pay_date"].ToString());
            dataModel.PayBank = row["bank_name"] == DBNull.Value ? string.Empty : row["bank_name"].ToString();
            dataModel.CreatorBy = row["createdby"] == DBNull.Value ? string.Empty : row["createdby"].ToString();
            dataModel.CreatorByName = row["creator_name"] == DBNull.Value ? string.Empty : row["creator_name"].ToString();
            dataModel.CreatorByRoleId = row["creator_role_id"] == DBNull.Value ? 0 : int.Parse(row["creator_role_id"].ToString());
            dataModel.CreatorByRoleName = row["creator_role_name"] == DBNull.Value ? string.Empty : row["creator_role_name"].ToString();
            dataModel.CreatorAccount = row["creator_acc"] == DBNull.Value ? -1 : int.Parse(row["creator_acc"].ToString());
            dataModel.CreatorBalance = row["creator_bal"] == DBNull.Value ? 0 : double.Parse(row["creator_bal"].ToString());
            dataModel.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
            dataModel.AccessChannelId = row["access_channel"] == DBNull.Value ? string.Empty : row["access_channel"].ToString();
            dataModel.AccessChannelName = row["access_channel_name"] == DBNull.Value ? string.Empty : row["access_channel_name"].ToString();
            dataModel.Amount = row["amount"] == DBNull.Value ? 0 : double.Parse(row["amount"].ToString());
            dataModel.TaxPercent = row["tax_per"] == DBNull.Value ? 0 : double.Parse(row["tax_per"].ToString());
            dataModel.TaxAmount = row["tax_amt"] == DBNull.Value ? 0 : double.Parse(row["tax_amt"].ToString());
            dataModel.BonusPercent = row["bonus_per"] == DBNull.Value ? 0 : double.Parse(row["bonus_per"].ToString());
            dataModel.BounsAmount = row["bonus_amt"] == DBNull.Value ? 0 : double.Parse(row["bonus_amt"].ToString());
            dataModel.BounsTaxPercent = row["bonus_tax"] == DBNull.Value ? 0 : double.Parse(row["bonus_tax"].ToString());
            dataModel.BounsTaxAmount = row["bonus_tax_amt"] == DBNull.Value ? 0 : double.Parse(row["bonus_tax_amt"].ToString());
            dataModel.ReceivedAmount = row["received_amt"] == DBNull.Value ? 0 : double.Parse(row["received_amt"].ToString());
            dataModel.NetAmount = row["net_amount"] == DBNull.Value ? 0 : double.Parse(row["net_amount"].ToString());
            dataModel.RequestAmount = row["request_amt"] == DBNull.Value ? 0 : double.Parse(row["request_amt"].ToString());
            dataModel.BillNo = row["bill_no"] == DBNull.Value ? string.Empty : row["bill_no"].ToString();
            dataModel.RequestNo = row["request_no"] == DBNull.Value ? string.Empty : row["request_no"].ToString();
            dataModel.Note = row["note"] == DBNull.Value ? string.Empty : row["note"].ToString();
            dataModel.Adjusted = row["adjusted"] == DBNull.Value ? false : row["adjusted"].ToString() == "1" ? true : false;
            dataModel.AdjustmentNo = row["adjust_id"] == DBNull.Value ? 0 : int.Parse(row["adjust_id"].ToString());
            dataModel.ApiTransaction = row["api_trans"] == DBNull.Value ? 0 : long.Parse(row["api_trans"].ToString());
            dataModel.FixedFactor = row["fixed_factor"] == DBNull.Value ? 0 : double.Parse(row["fixed_factor"].ToString());
            return dataModel;
        }
        public async Task<List<MoneyTransferRpt>> GetStatReportAsync(MoneyTransferRptQueryParam param)
        {
            #region Parameters
            param.Title = "تقرير احصائي يوضح اجمالي نقل ارصدة";
            var parameters = new List<OracleParameter>();
            var whereCluase = new StringBuilder();
            if (param != null)
            {
                if (!string.IsNullOrEmpty(param.PosId))
                {
                    whereCluase.Append(param.TransTypeId == "debit" ? " WHERE part_id = :PosId" : " WHERE createdby = :PosId");
                    var p = new OracleParameter { ParameterName = "PosId", OracleDbType = OracleDbType.Varchar2, Value = param.PosId };
                    parameters.Add(p);
                    param.Title += $"{Environment.NewLine} للرقم {param.PosId} ";
                }
                
                if (!string.IsNullOrEmpty(param.ChannelId) && param.ChannelId != "-1")
                {
                    whereCluase.Append(whereCluase.Length > 0 ? " AND access_channel = :ChannelId" : " WHERE access_channel = :ChannelId");
                    var p = new OracleParameter { ParameterName = "ChannelId", OracleDbType = OracleDbType.Varchar2, Value = param.ChannelId };
                    parameters.Add(p);
                }
                if (param.StartDate > DateTime.MinValue && param.StartDate != null)
                {
                    whereCluase.Append(whereCluase.Length > 0 ? " AND createdon >= :StartDate" : " WHERE createdon >= :StartDate");
                    var p = new OracleParameter { ParameterName = "StartDate", OracleDbType = OracleDbType.Date, Value = param.StartDate };
                    parameters.Add(p);
                }
                if (param.EndDate > DateTime.MinValue && param.EndDate != null)
                {
                    whereCluase.Append(whereCluase.Length > 0 ? " AND createdon <= :EndDate" : " WHERE createdon <= :EndDate");
                    var p = new OracleParameter { ParameterName = "EndDate", OracleDbType = OracleDbType.Date, Value = param.EndDate.AddDays(1) };
                    parameters.Add(p);
                }
            }

            #endregion

            string strSql = string.Empty;
            if (param.LevelId == "pos")
            {
                param.Title += $"{Environment.NewLine} للفترة من {param.StartDate.ToShortDateString()} الى {param.EndDate.ToShortDateString()}  " + "على مستوى نقطة البيع";
                strSql = $"select  t.access_channel,t.access_channel_name, "+ (param.TransTypeId == "debit" ? "t.part_id, t.part_name," : "t.createdby, t.creator_name,") 
                    + " count(*) cnt, sum(t.amount) amt    from v_money_transfer t " +
                    $"  {whereCluase}" +
                    $" group by t.access_channel, "+ (param.TransTypeId == "debit" ? "t.part_id, t.part_name," : "t.createdby, t.creator_name,") + " t.access_channel_name ";
            }
            else
            {
                param.Title += $"{Environment.NewLine} للفترة من {param.StartDate.ToShortDateString()} الى {param.EndDate.ToShortDateString()}  " + "على مستوى اليوم";
                strSql = $"select  t.access_channel,t.access_channel_name,to_char(createdon,'yyyy/mm/dd') d, count(*) cnt, sum(t.amount) amt    from v_money_transfer t " +
                    $" {whereCluase}" +
                    $" group by t.access_channel,t.access_channel_name, to_char(createdon,'yyyy/mm/dd')  ";
            }

            DataTable masterDataTable;
            masterDataTable = await db.GetDataAsync(strSql, parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            var results = new List<MoneyTransferRpt>();
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = new MoneyTransferRpt();
                obj.Amount = row["amt"] == DBNull.Value ? 0 : double.Parse(row["amt"].ToString());
                obj.Count = row["cnt"] == DBNull.Value ? 0 : int.Parse(row["cnt"].ToString());
                obj.Channel = row["access_channel_name"] == DBNull.Value ? string.Empty : row["access_channel_name"].ToString();
                if (param.LevelId == "pos")
                {
                    obj.Partner.Id = row[(param.TransTypeId == "debit" ? "part_id" : "createdby")] == DBNull.Value ? string.Empty : row[(param.TransTypeId == "debit" ? "part_id" : "createdby")].ToString();
                    obj.Partner.Name = row[(param.TransTypeId == "debit" ? "part_name" : "creator_name")] == DBNull.Value ? string.Empty : row[(param.TransTypeId == "debit" ? "part_name" : "creator_name")].ToString();
                }
                else if (param.LevelId == "day")
                {
                    obj.CollDay = row["d"] == DBNull.Value ? string.Empty : row["d"].ToString();
                }
                results.Add(obj);
            }
            return results;
        }

        public async Task<List<ToExcelSchema.MoneyTransfer>> GetMoneyTransferForExcelAsync(string whereClause, List<OracleParameter> parameters)
        {
            string strSql = $"select * from V_MONEY_TRANSFER {whereClause} order by trans_id";
            DataTable masterDataTable;
            masterDataTable = await db.GetDataAsync(strSql, parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            var results = new List<ToExcelSchema.MoneyTransfer>();
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = new ToExcelSchema.MoneyTransfer();
                obj.Id = row["trans_id"] == DBNull.Value ? -1 : int.Parse(row["trans_id"].ToString());
                obj.PartnerId = row["part_id"] == DBNull.Value ? string.Empty : row["part_id"].ToString();
                obj.PartnerName = row["part_name"] == DBNull.Value ? string.Empty : row["part_name"].ToString();
                obj.RoleName = row["part_role_name"] == DBNull.Value ? string.Empty : row["part_role_name"].ToString();
                obj.PartnerAccount = row["part_acc"] == DBNull.Value ? -1 : int.Parse(row["part_acc"].ToString());
                obj.PartnerBalance = row["part_bal"] == DBNull.Value ? 0 : double.Parse(row["part_bal"].ToString());
                obj.PayType = row["pay_type_name"] == DBNull.Value ? string.Empty : row["pay_type_name"].ToString();
                obj.PayNo = row["pay_no"] == DBNull.Value ? string.Empty : row["pay_no"].ToString();
                obj.PayDate = row["pay_date"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["pay_date"].ToString());
                obj.PayBank = row["bank_name"] == DBNull.Value ? string.Empty : row["bank_name"].ToString();
                obj.CreatedBy = row["createdby"] == DBNull.Value ? string.Empty : row["createdby"].ToString();
                obj.CreatedByName = row["creator_name"] == DBNull.Value ? string.Empty : row["creator_name"].ToString();
                obj.CreatedByRoleName = row["creator_role_name"] == DBNull.Value ? string.Empty : row["creator_role_name"].ToString();
                obj.CreatedByBalance = row["creator_bal"] == DBNull.Value ? 0 : double.Parse(row["creator_bal"].ToString());
                obj.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
                obj.AccessChannel = row["access_channel_name"] == DBNull.Value ? string.Empty : row["access_channel_name"].ToString();
                obj.Amount = row["amount"] == DBNull.Value ? 0 : double.Parse(row["amount"].ToString());
                obj.TaxPercent = row["tax_per"] == DBNull.Value ? 0 : double.Parse(row["tax_per"].ToString());
                obj.TaxAmount = row["tax_amt"] == DBNull.Value ? 0 : double.Parse(row["tax_amt"].ToString());
                obj.BonusPercent = row["bonus_per"] == DBNull.Value ? 0 : double.Parse(row["bonus_per"].ToString());
                obj.BounsAmount = row["bonus_amt"] == DBNull.Value ? 0 : double.Parse(row["bonus_amt"].ToString());
                obj.BounsTaxPercent = row["bonus_tax"] == DBNull.Value ? 0 : double.Parse(row["bonus_tax"].ToString());
                obj.BounsTaxAmount = row["bonus_tax_amt"] == DBNull.Value ? 0 : double.Parse(row["bonus_tax_amt"].ToString());
                obj.ReceivedAmount = row["received_amt"] == DBNull.Value ? 0 : double.Parse(row["received_amt"].ToString());
                obj.NetAmount = row["net_amount"] == DBNull.Value ? 0 : double.Parse(row["net_amount"].ToString());
                obj.RequestAmount = row["request_amt"] == DBNull.Value ? 0 : double.Parse(row["request_amt"].ToString());
                obj.BillNo = row["bill_no"] == DBNull.Value ? string.Empty : row["bill_no"].ToString();
                obj.RequestNo = row["request_no"] == DBNull.Value ? string.Empty : row["request_no"].ToString();
                obj.Note = row["note"] == DBNull.Value ? string.Empty : row["note"].ToString();
                obj.Adjusted = row["adjusted"] == DBNull.Value ? false : row["adjusted"].ToString() == "1" ? true : false;
                obj.AdjustmentNo = row["adjust_id"] == DBNull.Value ? 0 : int.Parse(row["adjust_id"].ToString());
                obj.ApiTransaction = row["api_trans"] == DBNull.Value ? 0 : long.Parse(row["api_trans"].ToString());
                obj.FixedFactor = row["fixed_factor"] == DBNull.Value ? 0 : double.Parse(row["fixed_factor"].ToString());
                results.Add(obj);
            }
            return results;
        }
    }
}
