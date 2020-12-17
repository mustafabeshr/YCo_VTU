﻿using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Yvtu.Core.Entities;
using Yvtu.Core.Queries;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Infra.Data
{
    public class MoneyTransferRepo
    {
        private readonly IAppDbContext db;
        private readonly IPartnerManager partnerManager;
        private readonly IPartnerActivityRepo partnerActivityRepo;

        public MoneyTransferRepo(IAppDbContext db, IPartnerManager partnerManager, IPartnerActivityRepo partnerActivityRepo)
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
                 new OracleParameter{ ParameterName = "v_pay_no",OracleDbType = OracleDbType.Varchar2,  Value = transfer.PayNo },
                 new OracleParameter{ ParameterName = "v_pay_date",OracleDbType = OracleDbType.Date,  Value = transfer.PayDate },
                 new OracleParameter{ ParameterName = "v_bank_name",OracleDbType = OracleDbType.Varchar2,  Value = transfer.PayBank },
                 new OracleParameter{ ParameterName = "v_createdby",OracleDbType = OracleDbType.Varchar2,  Value = transfer.CreatedBy.Id },
                 new OracleParameter{ ParameterName = "v_access_channel",OracleDbType = OracleDbType.Varchar2,  Value = "web" },
                 new OracleParameter{ ParameterName = "v_amount",OracleDbType = OracleDbType.Decimal,  Value = transfer.Amount },
                 new OracleParameter{ ParameterName = "v_bill_no",OracleDbType = OracleDbType.Varchar2,  Value = transfer.BillNo },
                 new OracleParameter{ ParameterName = "v_request_no",OracleDbType = OracleDbType.Varchar2,  Value = transfer.RequestNo },
                 new OracleParameter{ ParameterName = "v_request_amt",OracleDbType = OracleDbType.Decimal,  Value = transfer.RequestAmount },
                 new OracleParameter{ ParameterName = "v_note",OracleDbType = OracleDbType.Varchar2,  Value = transfer.Note }
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

            var moneyTransfer = new MoneyTransfer();
            DataRow row = masterDataTable.Rows[0];
            moneyTransfer.Id = row["trans_id"] == DBNull.Value ? -1 : int.Parse(row["trans_id"].ToString());
            var partAccount = row["part_acc"] == DBNull.Value ? -1 : int.Parse(row["part_acc"].ToString());
            var creatorAccount = row["creator_acc"] == DBNull.Value ? -1 : int.Parse(row["creator_acc"].ToString());
            var partRoleId = row["part_role_id"] == DBNull.Value ? -1 : int.Parse(row["part_role_id"].ToString());
            var creatorRoleId = row["creator_role_id"] == DBNull.Value ? -1 : int.Parse(row["creator_role_id"].ToString());
            var partBalance = row["part_bal"] == DBNull.Value ? -1 : long.Parse(row["part_bal"].ToString());
            var creatorBalance = row["creator_bal"] == DBNull.Value ? 0 : long.Parse(row["creator_bal"].ToString());
            
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
            moneyTransfer.RequestAmount = row["request_amt"] == DBNull.Value ? 0 :double.Parse(row["request_amt"].ToString());
            moneyTransfer.Note = row["note"] == DBNull.Value ? string.Empty : row["note"].ToString();
            moneyTransfer.Adjusted = row["adjusted"] == DBNull.Value ? false : row["adjusted"].ToString() == "1" ? true : false;
            moneyTransfer.AdjustmentNo = row["adjust_id"] == DBNull.Value ? 0 : int.Parse(row["adjust_id"].ToString());
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

        public MoneyTransferQueryDto MTQuery(MoneyTransferQueryDto param, int count = 200)
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
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE createdon>=:StartDate " : " AND createdon>=:StartDate   ");
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

            var masterDataTable = this.db.GetData("Select * from v_money_transfer  " + WhereClause + " order by createdon desc", parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            var moneyTransfer = new List<MoneyTransferDetailQueryDto>();
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = new MoneyTransferDetailQueryDto();
                obj.Seq = row["seq"] == DBNull.Value ? -1 : int.Parse(row["seq"].ToString());
                obj.Id = row["trans_id"] == DBNull.Value ? -1 : int.Parse(row["trans_id"].ToString());
                obj.PartnerId = row["part_id"] == DBNull.Value ? string.Empty : row["part_id"].ToString();
                obj.PartnerName = row["part_name"] == DBNull.Value ? string.Empty : row["part_name"].ToString();
                obj.PartnerRoleId = row["part_role_id"] == DBNull.Value ? -1 : int.Parse(row["part_role_id"].ToString());
                obj.PartnerRoleName = row["part_role_name"] == DBNull.Value ? string.Empty : row["part_role_name"].ToString();
                obj.PartnerAccount = row["part_acc"] == DBNull.Value ? -1 : int.Parse(row["part_acc"].ToString());
                obj.PartnerBalance = row["part_bal"] == DBNull.Value ? 0 : double.Parse(row["part_bal"].ToString());
                obj.PayTypeId = row["pay_type"] == DBNull.Value ? string.Empty : row["pay_type"].ToString();
                obj.PayTypeName = row["pay_type_name"] == DBNull.Value ? string.Empty : row["pay_type_name"].ToString();
                obj.PayNo = row["pay_no"] == DBNull.Value ? string.Empty : row["pay_no"].ToString();
                obj.PayDate = row["pay_date"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["pay_date"].ToString());
                obj.PayBank = row["bank_name"] == DBNull.Value ? string.Empty : row["bank_name"].ToString();
                obj.CreatorBy = row["createdby"] == DBNull.Value ? string.Empty : row["createdby"].ToString();
                obj.CreatorByName = row["creator_name"] == DBNull.Value ? string.Empty : row["creator_name"].ToString();
                obj.CreatorByRoleId = row["creator_role_id"] == DBNull.Value ? 0 :int.Parse(row["creator_role_id"].ToString());
                obj.CreatorByRoleName = row["creator_role_name"] == DBNull.Value ? string.Empty : row["creator_role_name"].ToString();
                obj.CreatorAccount = row["creator_acc"] == DBNull.Value ? -1 : int.Parse(row["creator_acc"].ToString());
                obj.CreatorBalance = row["creator_bal"] == DBNull.Value ? 0 :double.Parse(row["creator_bal"].ToString());
                obj.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue :DateTime.Parse(row["createdon"].ToString());
                obj.AccessChannelId = row["access_channel"] == DBNull.Value ? string.Empty : row["access_channel"].ToString();
                obj.AccessChannelName = row["access_channel_name"] == DBNull.Value ? string.Empty : row["access_channel_name"].ToString();
                obj.Amount = row["amount"] == DBNull.Value ? 0 :double.Parse(row["amount"].ToString());
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
                moneyTransfer.Add(obj);
            }
            param.Results = moneyTransfer;
            return param;
        }
    }
}