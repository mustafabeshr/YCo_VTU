using Microsoft.AspNetCore.Http;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Infra.Data
{
    public class PartnerManager : IPartnerManager
    {
        private readonly IAppDbContext db;

        public PartnerManager(IAppDbContext db)
        {
            this.db = db;
        }

        public OpertionResult ChangePwd(string PartnerId, string newPwd)
        {
            throw new NotImplementedException();
        }

        public Task<OpertionResult> ChangePwdAsync(string PartnerId, string newPwd)
        {
            throw new NotImplementedException();
        }

        public async Task<OpertionResult> CreateAsync(Partner partner)
        {
            byte[] salt = Pbkdf2Hasher.GenerateRandomSalt();
            string hash = Pbkdf2Hasher.ComputeHash(partner.Pwd, salt);

            var insertSql = "insert into partner (partner_id, partner_name, brandname, balance, reserved, verificationcodenext, "+
                "  locktime, roleid, id_no, id_type, id_place, id_issued, status, statusby, "+
                "   createdby, cityid, districtid, street, zone, extra_address, pair_mobile, "+
                "  mobile, fixed, fax, email, pwd, extra) values" +
                "(:v_partner_id, :v_partner_name, :v_brandname, :v_balance, :v_reserved, :v_verificationcodenext, "+
                "  :v_locktime, :v_roleid, :v_id_no, :v_id_type, :v_id_place, :v_id_issued, :v_status, :v_statusby, " +
                "  :v_createdby, :v_cityid, :v_districtid, :v_street, :v_zone, :v_extra_address, :v_pair_mobile, " +
                "  :v_mobile, :v_fixed, :v_fax, :v_email, :v_pwd, :v_extra)";
            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "v_partner_id", OracleDbType = OracleDbType.Varchar2,  Value = partner.Id },
                 new OracleParameter{ ParameterName = "v_partner_name",OracleDbType = OracleDbType.Varchar2,  Value = partner.Name },
                 new OracleParameter{ ParameterName = "v_brandname",OracleDbType = OracleDbType.Varchar2,  Value = partner.BrandName },
                 new OracleParameter{ ParameterName = "v_balance",OracleDbType = OracleDbType.Int32,   Value = 0 },
                 new OracleParameter{ ParameterName = "v_reserved", OracleDbType = OracleDbType.Int32,  Value = 0 },
                 new OracleParameter{ ParameterName = "v_verificationcodenext", OracleDbType = OracleDbType.Single, Value = partner.VerificationCodeNext ? 1 : 0 },
                 new OracleParameter{ ParameterName = "v_locktime", OracleDbType = OracleDbType.Date, Value = null },
                 new OracleParameter{ ParameterName = "v_roleid", OracleDbType = OracleDbType.Int32, Value = partner.Role.Id },
                 new OracleParameter{ ParameterName = "v_id_no", OracleDbType = OracleDbType.Int32, Value = partner.PersonalId.Id },
                 new OracleParameter{ ParameterName = "v_id_type", OracleDbType = OracleDbType.Int32, Value = partner.PersonalId.IdType.Id },
                 new OracleParameter{ ParameterName = "v_id_place", OracleDbType = OracleDbType.Varchar2, Value = partner.PersonalId.Place },
                 new OracleParameter{ ParameterName = "v_id_issued", OracleDbType = OracleDbType.Date, Value = partner.PersonalId.Issued },
                 new OracleParameter{ ParameterName = "v_status", OracleDbType = OracleDbType.Int32, Value = partner.Status.Id },
                 new OracleParameter{ ParameterName = "v_statusby", OracleDbType = OracleDbType.Varchar2, Value = partner.StatusBy.Id },
                 new OracleParameter{ ParameterName = "v_createdby", OracleDbType = OracleDbType.Varchar2, Value = partner.CreatedBy.Id },
                 new OracleParameter{ ParameterName = "v_cityid", OracleDbType = OracleDbType.Int32, Value = partner.Address.City.Id},
                 new OracleParameter{ ParameterName = "v_districtid", OracleDbType = OracleDbType.Int32, Value = partner.Address.District.Id },
                 new OracleParameter{ ParameterName = "v_street", OracleDbType = OracleDbType.Varchar2, Value = partner.Address.Street },
                 new OracleParameter{ ParameterName = "v_zone", OracleDbType = OracleDbType.Varchar2, Value = partner.Address.Zone },
                 new OracleParameter{ ParameterName = "v_extra_address", OracleDbType = OracleDbType.Varchar2, Value = partner.Address.ExtraInfo },
                 new OracleParameter{ ParameterName = "v_pair_mobile", OracleDbType = OracleDbType.Varchar2, Value = partner.PairMobile },
                 new OracleParameter{ ParameterName = "v_mobile", OracleDbType = OracleDbType.Varchar2, Value = partner.ContactInfo.Mobile },
                 new OracleParameter{ ParameterName = "v_fixed", OracleDbType = OracleDbType.Varchar2, Value = partner.ContactInfo.Fixed },
                 new OracleParameter{ ParameterName = "v_fax", OracleDbType = OracleDbType.Varchar2, Value = partner.ContactInfo.Fax },
                 new OracleParameter{ ParameterName = "v_email", OracleDbType = OracleDbType.Varchar2, Value = partner.ContactInfo.Email },
                 new OracleParameter{ ParameterName = "v_pwd", OracleDbType = OracleDbType.Varchar2, Value = hash },
                 new OracleParameter{ ParameterName = "v_extra", OracleDbType = OracleDbType.Varchar2, Value = Convert.ToBase64String(salt)}
            };

                #endregion

                var result = await db.ExecuteSqlCommandAsync(insertSql, parameters);

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

        

        public Partner GetCurrentUser(HttpContext httpContext)
        {
            throw new NotImplementedException();
        }

        public int GetCurrentUserId(HttpContext httpContext)
        {
            throw new NotImplementedException();
        }

        public Task SignIn(HttpContext httpContext, Partner partner, bool isPersistent = false)
        {
            throw new NotImplementedException();
        }

        public Task SignOut(HttpContext httpContext)
        {
            throw new NotImplementedException();
        }

        public ValidatePartnerResult Validate(string partnerId)
        {
            throw new NotImplementedException();
        }

        public Task<ValidatePartnerResult> ValidateAsync(string partnerId)
        {
            throw new NotImplementedException();
        }

        OpertionResult IPartnerManager.Create(Partner partner)
        {
            throw new NotImplementedException();
        }
    }
}
