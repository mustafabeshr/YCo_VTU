using Microsoft.AspNetCore.Http;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
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

            var insertSql = "insert into partner (partner_id, partner_name, brandname, balance, reserved, verificationcodenext, " +
                "  locktime, roleid, id_no, id_type, id_place, id_issued, status, statusby, " +
                "   createdby, cityid, districtid, street, zone, extra_address, pair_mobile, " +
                "  mobile, fixed, fax, email, pwd, extra, ip_address) values" +
                "(:v_partner_id, :v_partner_name, :v_brandname, :v_balance, :v_reserved, :v_verificationcodenext, " +
                "  :v_locktime, :v_roleid, :v_id_no, :v_id_type, :v_id_place, :v_id_issued, :v_status, :v_statusby, " +
                "  :v_createdby, :v_cityid, :v_districtid, :v_street, :v_zone, :v_extra_address, :v_pair_mobile, " +
                "  :v_mobile, :v_fixed, :v_fax, :v_email, :v_pwd, :v_extra, :v_ip_address)";
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
                 new OracleParameter{ ParameterName = "v_extra", OracleDbType = OracleDbType.Varchar2, Value = Convert.ToBase64String(salt) },
                 new OracleParameter{ ParameterName = "v_ip_address", OracleDbType = OracleDbType.Varchar2, Value = partner.IPAddress}

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

        public string GetCurrentUserId(HttpContext httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
                return "-1";

            Claim claim = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (claim == null)
                return "-1";

            string currentUserId = claim.Value;

            //if (!int.TryParse(claim.Value, out currentUserId))
            //    return "-1";

            return currentUserId;
        }

        public Task SignIn(HttpContext httpContext, Partner partner, bool isPersistent = false)
        {
            throw new NotImplementedException();
        }

        public Task SignOut(HttpContext httpContext)
        {
            throw new NotImplementedException();
        }

        public PartBasicInfo GetPartnerBasicInfo(string partnerId)
        {
            var partner = GetPartner(partnerId);
            if (partner == null) return null;
            var basicInfo = new PartBasicInfo();
            basicInfo.Id = partner.Id;
            basicInfo.Name = partner.Name;
            basicInfo.Role = partner.Role;
            basicInfo.Balance = partner.Balance;
            return basicInfo;
        }
        public ValidatePartnerResult Validate(string partnerId)
        {
            var partner = GetPartner(partnerId);
            if (partner == null)
            {
                return new ValidatePartnerResult { Partner = null, Success = false, Error = "NotFound" };
            }
            else
            {
                return new ValidatePartnerResult { Partner = partner, Success = true, Error = string.Empty };
            }

        }
        private Partner GetPartner(string Id)
        {
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "partnerId", OracleDbType = OracleDbType.Varchar2,  Value = Id },
            };
            var partnerDataTable = db.GetData("select * from partner where partner_id=:partnerId", parameters);
            if (partnerDataTable == null) return null;

            #region Convert to Partner Object
            var partner = new Partner();
            DataRow dataRow = partnerDataTable.Rows[0];
            partner.Id = dataRow["partner_id"] == DBNull.Value ? string.Empty : dataRow["partner_id"].ToString();
            partner.Name = dataRow["partner_name"] == DBNull.Value ? string.Empty : dataRow["partner_name"].ToString();
            partner.BrandName = dataRow["brandname"] == DBNull.Value ? string.Empty : dataRow["brandname"].ToString();
            partner.Balance = dataRow["balance"] == DBNull.Value ? long.MinValue : long.Parse(dataRow["balance"].ToString());
            partner.Reserved = dataRow["reserved"] == DBNull.Value ? long.MinValue : long.Parse(dataRow["reserved"].ToString());
            partner.VerificationCodeNext = dataRow["verificationcodenext"] == DBNull.Value ? false : dataRow["verificationcodenext"].ToString() == "1" ? true : false;
            partner.LockTime = dataRow["locktime"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(dataRow["locktime"].ToString());
            partner.Role.Id = dataRow["roleid"] == DBNull.Value ? int.MinValue : int.Parse(dataRow["roleid"].ToString());
            partner.PersonalId.Id = dataRow["id_no"] == DBNull.Value ? string.Empty : dataRow["id_no"].ToString();
            partner.PersonalId.IdType.Id = dataRow["id_type"] == DBNull.Value ? int.MinValue : int.Parse(dataRow["id_type"].ToString());
            partner.PersonalId.Place = dataRow["id_place"] == DBNull.Value ? string.Empty : dataRow["id_place"].ToString();
            partner.PersonalId.Issued = dataRow["id_issued"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(dataRow["id_issued"].ToString());
            partner.Status.Id = dataRow["status"] == DBNull.Value ? int.MinValue : int.Parse(dataRow["status"].ToString());
            partner.StatusOn = dataRow["statuson"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(dataRow["statuson"].ToString());
            partner.StatusBy.Id = dataRow["statusby"] == DBNull.Value ? string.Empty : dataRow["statusby"].ToString();
            partner.CreatedOn = dataRow["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(dataRow["createdon"].ToString());
            partner.CreatedBy.Id = dataRow["createdby"] == DBNull.Value ? string.Empty : dataRow["createdby"].ToString();
            partner.Address.City.Id = dataRow["cityid"] == DBNull.Value ? int.MinValue : int.Parse(dataRow["cityid"].ToString());
            partner.Address.District.Id = dataRow["districtid"] == DBNull.Value ? int.MinValue : int.Parse(dataRow["districtid"].ToString());
            partner.WrongPwdAttempts = dataRow["WRONG_PWD_ATTEMPTS"] == DBNull.Value ? int.MinValue : int.Parse(dataRow["WRONG_PWD_ATTEMPTS"].ToString());
            partner.Address.Street = dataRow["street"] == DBNull.Value ? string.Empty : dataRow["street"].ToString();
            partner.Address.Zone = dataRow["zone"] == DBNull.Value ? string.Empty : dataRow["zone"].ToString();
            partner.Address.ExtraInfo = dataRow["extra_address"] == DBNull.Value ? string.Empty : dataRow["extra_address"].ToString();
            partner.PairMobile = dataRow["pair_mobile"] == DBNull.Value ? string.Empty : dataRow["pair_mobile"].ToString();
            partner.ContactInfo.Mobile = dataRow["mobile"] == DBNull.Value ? string.Empty : dataRow["mobile"].ToString();
            partner.ContactInfo.Fixed = dataRow["fixed"] == DBNull.Value ? string.Empty : dataRow["fixed"].ToString();
            partner.ContactInfo.Fax = dataRow["fax"] == DBNull.Value ? string.Empty : dataRow["fax"].ToString();
            partner.ContactInfo.Email = dataRow["email"] == DBNull.Value ? string.Empty : dataRow["email"].ToString();
            partner.Pwd = dataRow["pwd"] == DBNull.Value ? string.Empty : dataRow["pwd"].ToString();
            partner.Extra = dataRow["extra"] == DBNull.Value ? string.Empty : dataRow["extra"].ToString();
            partner.IPAddress = dataRow["ip_address"] == DBNull.Value ? string.Empty : dataRow["ip_address"].ToString();
            // get sub objects
            // Role
            partner.Role = new RoleRepo(db).GetRole(partner.Role.Id);
            partner.PersonalId.IdType = new IdTypeRepo(db).GetIdType(partner.PersonalId.IdType.Id);
            partner.Status = new PartnerStatusRepo(db).GetStatus(partner.Status.Id);
            partner.Address.City = new CityRepo(db).GetCity(partner.Address.City.Id);
            partner.Address.District = new DistrictRepo(db).GetDistrict(partner.Address.District.Id);


            var statusUserparameter = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "statusBy", OracleDbType = OracleDbType.Varchar2,  Value = Id },
            };
            var statusByDataTable = db.GetData("select * from partner where partner_id=:statusBy", statusUserparameter);
            if (statusByDataTable != null)
            {
                DataRow dr = statusByDataTable.Rows[0];
                partner.StatusBy.Id = dr["partner_id"] == null ? string.Empty : dr["partner_id"].ToString();
                partner.StatusBy.Name = dr["partner_name"] == null ? string.Empty : dr["partner_name"].ToString();

            }

            var createUserparameter = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "createBy", OracleDbType = OracleDbType.Varchar2,  Value = Id },
            };
            var createByDataTable = db.GetData("select * from partner where partner_id=:createBy", createUserparameter);
            if (createByDataTable != null)
            {
                DataRow dr = createByDataTable.Rows[0];
                partner.CreatedBy.Id = dr["partner_id"] == null ? string.Empty : dr["partner_id"].ToString();
                partner.CreatedBy.Name = dr["partner_name"] == null ? string.Empty : dr["partner_name"].ToString();

            }
            /////////////////////////

            #endregion
            return partner;

        }

        public bool IncreaseWrongPwdAttempts(string partnerId, bool lockAccount)
        {
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "PartnerId", OracleDbType = OracleDbType.Varchar2,  Value = partnerId },
            };

            string sql = string.Empty;
            if (lockAccount)
            {
                sql = "Update partner set WRONG_PWD_ATTEMPTS = WRONG_PWD_ATTEMPTS + 1, status = 2, statuson=sysdate, locktime = sysdate + 1/24 where partner_id = :PartnerId";
            }
            else
            {
                sql = "Update partner set WRONG_PWD_ATTEMPTS = WRONG_PWD_ATTEMPTS + 1 where partner_id = :PartnerId";
            }
            return db.ExecuteSqlCommand(sql, parameters) > 0;
        }

        public IEnumerable<Claim> GetUserClaims(Partner user)
        {
            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, user.Name));
            claims.Add(new Claim(ClaimTypes.GivenName, user.Role.Id.ToString()));
            claims.AddRange(this.GetUserRoleClaims(user));
            return claims;
        }
        private IEnumerable<Claim> GetUserRoleClaims(Partner user)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Role, user.Role.Code));
            //claims.AddRange(this.GetUserPermissionClaims(role));
            return claims;
        }

        public Task<ValidatePartnerResult> ValidateAsync(string partnerId)
        {
            throw new NotImplementedException();
        }

        OpertionResult IPartnerManager.Create(Partner partner)
        {
            throw new NotImplementedException();
        }

        public bool PreSuccessLogin(string partnerId)
        {
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "PartnerId", OracleDbType = OracleDbType.Varchar2,  Value = partnerId },
            };

            string sql = "Update partner set WRONG_PWD_ATTEMPTS = 0 ,last_login = sysdate, locktime = null ,status = 1, statuson=sysdate where partner_id = :PartnerId";

            return db.ExecuteSqlCommand(sql, parameters) > 0;
        }


    }
}
