using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Oracle.ManagedDataAccess.Client;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Infra.Data
{
    public class ChangeSecretHistoryRepo
    {
        private readonly IAppDbContext _db;
        private readonly IPartnerManager _partnerManager;
        private readonly IPartnerActivityRepo _partnerActivityRepo;

        public ChangeSecretHistoryRepo(IAppDbContext db, IPartnerManager partnerManager,
            IPartnerActivityRepo partnerActivityRepo)
        {
            _db = db;
            _partnerManager = partnerManager;
            _partnerActivityRepo = partnerActivityRepo;
        }

        public OpertionResult Create(ChangeSecretHistory created)
        {
            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                 new OracleParameter{ ParameterName = "v_createdby", OracleDbType = OracleDbType.Varchar2,  Value = created.CreatedBy.Id },
                 new OracleParameter{ ParameterName = "v_createdbyacc",OracleDbType = OracleDbType.Int32,  Value = created.CreatedBy.Account },
                 new OracleParameter{ ParameterName = "v_access_channel",OracleDbType = OracleDbType.Varchar2,  Value = created.AccessChannel.Id },
                 new OracleParameter{ ParameterName = "v_old_sec_salt",OracleDbType = OracleDbType.Varchar2,  Value = created.OldSalt },
                 new OracleParameter{ ParameterName = "v_old_hash",OracleDbType = OracleDbType.Varchar2,  Value = created.OldHash },
                 new OracleParameter{ ParameterName = "v_new_salt",OracleDbType = OracleDbType.Varchar2,  Value = created.NewSalt },
                 new OracleParameter{ ParameterName = "v_new_hash",OracleDbType = OracleDbType.Varchar2,  Value = created.NewHash },
                 new OracleParameter{ ParameterName = "v_chg_type",OracleDbType = OracleDbType.Varchar2,  Value = created.ChangeType.Id },
                 new OracleParameter{ ParameterName = "v_notify_by",OracleDbType = OracleDbType.Varchar2,  Value = created.NotifyBy.Id },
                 new OracleParameter{ ParameterName = "v_partner_id",OracleDbType = OracleDbType.Varchar2,  Value = created.PartAppUser.Id },
                 new OracleParameter{ ParameterName = "v_partner_acc",OracleDbType = OracleDbType.Int32,  Value = created.PartAppUser.Account }
                };
                #endregion
                _db.ExecuteStoredProc("pk_infra.fn_Create_chgSec_his", parameters);
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
    }
}
