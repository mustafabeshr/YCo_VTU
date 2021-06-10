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
using Yvtu.Core.Queries;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Infra.Data
{
    public class PartnerManager : IPartnerManager
    {
        private readonly IAppDbContext db;
        private readonly IPartnerActivityRepo partnerActivity;

        public PartnerManager(IAppDbContext db, IPartnerActivityRepo partnerActivity)
        {
            this.db = db;
            this.partnerActivity = partnerActivity;
        }

        public bool CheckPass(Partner partner, string pwd)
        {
            //var pass = Utility.GenerateNewCode(4);
            byte[] salt = Convert.FromBase64String(partner.Extra);
            string hash = Pbkdf2Hasher.ComputeHash(pwd, salt);
            if (hash == partner.Pwd)
            {
                return true;
            }
            return false;
        }
        public bool ChangePwd(int PartnerAcc, string PartnerId,string newPwd, bool notify = true)
        {
            //var pass = Utility.GenerateNewCode(4);
            byte[] salt = Pbkdf2Hasher.GenerateRandomSalt();
            string hash = Pbkdf2Hasher.ComputeHash(newPwd, salt);
            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                 new OracleParameter{ ParameterName = "v_partner_acc", OracleDbType = OracleDbType.Int32,  Value = PartnerAcc },
                 new OracleParameter{ ParameterName = "v_pwd",OracleDbType = OracleDbType.Varchar2,  Value =  hash},
                 new OracleParameter{ ParameterName = "v_extra",OracleDbType = OracleDbType.Varchar2,  Value = Convert.ToBase64String(salt) }
                };

                #endregion
                db.ExecuteStoredProc("pk_infra.fn_ChangePassword", parameters);
                var result = int.Parse(parameters.Find(x => x.ParameterName == "retVal").Value.ToString());

                if (result > 0)
                {
                    if (notify)
                    {
                        var msg = "تم تغيير كلمة المرور الخاصبة بك الى " + newPwd;
                        new OutSMSRepo(db).Create(new SMSOut
                        {
                            Receiver = PartnerId,
                            Message = msg
                        });
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Task<OpertionResult> ChangePwdAsync(string PartnerId, string newPwd)
        {
            throw new NotImplementedException();
        }

        public async Task<OpertionResult> CreateAsync(Partner partner)
        {
            byte[] salt = Pbkdf2Hasher.GenerateRandomSalt();
            string hash = Pbkdf2Hasher.ComputeHash(partner.Pwd, salt);

            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                 new OracleParameter{ ParameterName = "v_partner_id", OracleDbType = OracleDbType.Varchar2,  Value = partner.Id },
                 new OracleParameter{ ParameterName = "v_partner_name",OracleDbType = OracleDbType.NVarchar2,  Value = partner.Name },
                 new OracleParameter{ ParameterName = "v_brandname",OracleDbType = OracleDbType.NVarchar2,  Value = partner.BrandName },
                 new OracleParameter{ ParameterName = "v_roleid", OracleDbType = OracleDbType.Int32, Value = partner.Role.Id },
                 new OracleParameter{ ParameterName = "v_id_no", OracleDbType = OracleDbType.Int32, Value = partner.PersonalId.Id },
                 new OracleParameter{ ParameterName = "v_id_type", OracleDbType = OracleDbType.Int32, Value = partner.PersonalId.IdType.Id },
                 new OracleParameter{ ParameterName = "v_id_place", OracleDbType = OracleDbType.NVarchar2, Value = partner.PersonalId.Place },
                 new OracleParameter{ ParameterName = "v_id_issued", OracleDbType = OracleDbType.Date, Value = partner.PersonalId.Issued },
                 new OracleParameter{ ParameterName = "v_createdby", OracleDbType = OracleDbType.Varchar2, Value = partner.CreatedBy.Id },
                 new OracleParameter{ ParameterName = "v_cityid", OracleDbType = OracleDbType.Int32, Value = partner.Address.City.Id},
                 new OracleParameter{ ParameterName = "v_districtid", OracleDbType = OracleDbType.Int32, Value = partner.Address.District.Id },
                 new OracleParameter{ ParameterName = "v_street", OracleDbType = OracleDbType.NVarchar2, Value = partner.Address.Street },
                 new OracleParameter{ ParameterName = "v_zone", OracleDbType = OracleDbType.NVarchar2, Value = partner.Address.Zone },
                 new OracleParameter{ ParameterName = "v_extra_address", OracleDbType = OracleDbType.NVarchar2, Value = partner.Address.ExtraInfo },
                 new OracleParameter{ ParameterName = "v_pair_mobile", OracleDbType = OracleDbType.Varchar2, Value = partner.PairMobile },
                 new OracleParameter{ ParameterName = "v_mobile", OracleDbType = OracleDbType.Varchar2, Value = partner.ContactInfo.Mobile },
                 new OracleParameter{ ParameterName = "v_fixed", OracleDbType = OracleDbType.Varchar2, Value = partner.ContactInfo.Fixed },
                 new OracleParameter{ ParameterName = "v_fax", OracleDbType = OracleDbType.Varchar2, Value = partner.ContactInfo.Fax },
                 new OracleParameter{ ParameterName = "v_email", OracleDbType = OracleDbType.Varchar2, Value = partner.ContactInfo.Email },
                 new OracleParameter{ ParameterName = "v_pwd", OracleDbType = OracleDbType.Varchar2, Value = hash },
                 new OracleParameter{ ParameterName = "v_extra", OracleDbType = OracleDbType.Varchar2, Value = Convert.ToBase64String(salt) },
                 new OracleParameter{ ParameterName = "v_ip_address", OracleDbType = OracleDbType.Varchar2, Value = partner.IPAddress},
                 new OracleParameter{ ParameterName = "v_ref_partner", OracleDbType = OracleDbType.Varchar2, Value = partner.RefPartner.Id}

            };

                #endregion
                await db.ExecuteStoredProcAsync("pk_infra.fn_createpartner", parameters);
                var result = int.Parse(parameters.Find(x => x.ParameterName == "retVal").Value.ToString());

                if (result > 0)
                {
                    var msg = "تم انشاء حساب لك بخدمة الشاحن الفوري و كلمة المرور هي  " + partner.Pwd;
                    new OutSMSRepo(db).Create(new SMSOut
                    {
                        Receiver = partner.Id,
                        Message = msg
                    });
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

        public bool ResetPassword(Partner partner)
        {
            var pass = Utility.GenerateNewCode(4);
            byte[] salt = Pbkdf2Hasher.GenerateRandomSalt();
            string hash = Pbkdf2Hasher.ComputeHash(pass, salt);

            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                 new OracleParameter{ ParameterName = "v_partner_acc", OracleDbType = OracleDbType.Varchar2,  Value = partner.Account },
                 new OracleParameter{ ParameterName = "v_pwd",OracleDbType = OracleDbType.Varchar2,  Value =  hash},
                 new OracleParameter{ ParameterName = "v_extra",OracleDbType = OracleDbType.Varchar2,  Value = Convert.ToBase64String(salt) },
                 new OracleParameter{ ParameterName = "v_createdby",OracleDbType = OracleDbType.Varchar2,  Value = partner.CreatedBy.Id }
                };

                #endregion
                db.ExecuteStoredProc("pk_infra.fn_resetpassword", parameters);
                var result = int.Parse(parameters.Find(x => x.ParameterName == "retVal").Value.ToString());

                if (result > 0)
                {
                    var msg = "تم اعادة تعيين كلمة المرور الخاصة بك الى " + pass;
                    new OutSMSRepo(db).Create(new SMSOut
                    {
                        Receiver = partner.Id,
                        Message = msg
                    });
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
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

            Claim claim = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone);

            if (claim == null)
                return "-1";

            string currentUserId = claim.Value;

            //if (!int.TryParse(claim.Value, out currentUserId))
            //    return "-1";

            return currentUserId;
        }
        public int GetCurrentUserAccount(HttpContext httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
                return -1;

            Claim claim = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (claim == null)
                return -1;

            int currentUserId = int.Parse(claim.Value);

            //if (!int.TryParse(claim.Value, out currentUserId))
            //    return "-1";

            return currentUserId;
        }
        public int GetCurrentUserRole(HttpContext httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
                return -1;

            Claim claim = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName);

            if (claim == null)
                return -1;

            int currentUserId = int.Parse(claim.Value);

            //if (!int.TryParse(claim.Value, out currentUserId))
            //    return "-1";

            return currentUserId;
        }
        public string GetCurrentUserRoleCode(HttpContext httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
                return "-1";

            Claim claim = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

            if (claim == null)
                return "-1";

            string currentUserRoleCode = claim.Value;

            //if (!int.TryParse(claim.Value, out currentUserId))
            //    return "-1";

            return currentUserRoleCode;
        }

        public Task SignIn(HttpContext httpContext, Partner partner, bool isPersistent = false)
        {
            throw new NotImplementedException();
        }

        public Task SignOut(HttpContext httpContext)
        {
            throw new NotImplementedException();
        }

        public Partner GetPartnerById(string id)
        {
            var p = GetActivePartner(id);
            return p;
        }
        public PartBasicInfo GetPartnerBasicInfo(string partnerId)
        {
            var partner = GetActivePartner(partnerId);
            if (partner == null) return null;
            var basicInfo = new PartBasicInfo();
            basicInfo.Id = partner.Id;
            basicInfo.Account = partner.Account;
            basicInfo.Name = partner.Name;
            basicInfo.RefPartnerId = partner.RefPartner.Id;
            basicInfo.Role = partner.Role;
            basicInfo.Balance = partner.Balance;
            basicInfo.Reserved = partner.Reserved;
            basicInfo.Status = partner.Status;
            basicInfo.LastLoginOn = partner.LastLoginOn ;
            return basicInfo;
        }
        public ValidatePartnerResult Validate(string partnerId)
        {
            var partner = GetActivePartner(partnerId);
            if (partner == null)
            {
                return new ValidatePartnerResult { Partner = null, Success = false, Error = "NotFound" };
            }
            else
            {
                return new ValidatePartnerResult { Partner = partner, Success = true, Error = string.Empty };
            }
        }

        private Partner ConvertDataRowToPartner(DataRow dataRow)
        {
            if (dataRow == null) return null;
            #region Convert to Partner Object
            var partner = new Partner();
            partner.Id = dataRow["partner_id"] == DBNull.Value ? string.Empty : dataRow["partner_id"].ToString();
            partner.Account = dataRow["partner_acc"] == DBNull.Value ? -1 : int.Parse(dataRow["partner_acc"].ToString());
            partner.Name = dataRow["partner_name"] == DBNull.Value ? string.Empty : dataRow["partner_name"].ToString();
            partner.BrandName = dataRow["brandname"] == DBNull.Value ? string.Empty : dataRow["brandname"].ToString();
            partner.Balance = dataRow["balance"] == DBNull.Value ? double.MinValue : double.Parse(dataRow["balance"].ToString());
            partner.Reserved = dataRow["reserved"] == DBNull.Value ? double.MinValue : double.Parse(dataRow["reserved"].ToString());
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
            partner.LastLoginOn = dataRow["last_login"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(dataRow["last_login"].ToString());

            // get sub objects
            // Role
            partner.Role = new RoleRepo(db, partnerActivity).GetRole(partner.Role.Id);
            partner.PersonalId.IdType = new IdTypeRepo(db).GetIdType(partner.PersonalId.IdType.Id);
            partner.Status = new PartnerStatusRepo(db).GetStatus(partner.Status.Id);
            partner.Address.City = new CityRepo(db).GetCity(partner.Address.City.Id);
            partner.Address.District = new DistrictRepo(db).GetDistrict(partner.Address.District.Id);

            var statusBy = dataRow["STATUSBY"] == DBNull.Value ? string.Empty : dataRow["STATUSBY"].ToString();
            var statusUserparameter = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "statusBy", OracleDbType = OracleDbType.Varchar2,  Value = statusBy },
            };
            var statusByDataTable = db.GetData("select * from partner where partner_id=:statusBy", statusUserparameter);
            if (statusByDataTable != null)
            {
                DataRow dr = statusByDataTable.Rows[0];
                partner.StatusBy.Id = dr["partner_id"] == null ? string.Empty : dr["partner_id"].ToString();
                partner.StatusBy.Name = dr["partner_name"] == null ? string.Empty : dr["partner_name"].ToString();

            }

            var createdBy = dataRow["CREATEDBY"] == DBNull.Value ? string.Empty : dataRow["CREATEDBY"].ToString();
            var createUserparameter = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "createBy", OracleDbType = OracleDbType.Varchar2,  Value = createdBy },
            };
            var createByDataTable = db.GetData("select * from partner where partner_id=:createBy", createUserparameter);
            if (createByDataTable != null)
            {
                DataRow dr = createByDataTable.Rows[0];
                partner.CreatedBy.Id = dr["partner_id"] == null ? string.Empty : dr["partner_id"].ToString();
                partner.CreatedBy.Name = dr["partner_name"] == null ? string.Empty : dr["partner_name"].ToString();

            }
            var refPartnerId = dataRow["REF_PARTNER"] == DBNull.Value ? string.Empty : dataRow["REF_PARTNER"].ToString();
            var refPartparameter = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "RefPart", OracleDbType = OracleDbType.Varchar2,  Value = refPartnerId },
            };

            var refDataTable = db.GetData("select * from partner where partner_id=:RefPart", refPartparameter);
            if (statusByDataTable != null)
            {
                DataRow dr = refDataTable.Rows[0];
                partner.RefPartner.Id = dr["partner_id"] == null ? string.Empty : dr["partner_id"].ToString();
                partner.RefPartner.Name = dr["partner_name"] == null ? string.Empty : dr["partner_name"].ToString();

            }
            /////////////////////////

            #endregion
            return partner;
        }
        private Partner GetActivePartner(string Id)
        {
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "partnerId", OracleDbType = OracleDbType.Varchar2,  Value = Id },
            };
            var partnerDataTable = db.GetData("select * from partner where partner_id=:partnerId and (status < 4)", parameters);
            if (partnerDataTable == null || partnerDataTable.Rows.Count == 0) return null;

            var partner = new Partner();
            partner = ConvertDataRowToPartner(partnerDataTable.Rows[0]);
            return partner;

        }

        public Partner GetPartnerByAccount(int account)
        {
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "partnerAccount", OracleDbType = OracleDbType.Int32,  Value = account },
            };
            var partnerDataTable = db.GetData("select * from partner where partner_acc=:partnerAccount", parameters);
            if (partnerDataTable == null) return null;

            #region Convert to Partner Object
            var partner = ConvertDataRowToPartner(partnerDataTable.Rows[0]);
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
                sql = "Update partner set WRONG_PWD_ATTEMPTS = WRONG_PWD_ATTEMPTS + 1, status = 2, statuson=sysdate, locktime = sysdate + 1/24 where partner_id = :PartnerId and status = 1";
            }
            else
            {
                sql = "Update partner set WRONG_PWD_ATTEMPTS = WRONG_PWD_ATTEMPTS + 1 where partner_id = :PartnerId and status = 1";
            }
            return db.ExecuteSqlCommand(sql, parameters) > 0;
        }

        public IEnumerable<Claim> GetUserClaims(Partner user)
        {
            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.MobilePhone, user.Id));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Account.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, user.Name));
            claims.Add(new Claim(ClaimTypes.GivenName, user.Role.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Role, user.Role.Code));
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

        public List<Partner> GetPartners(PartnerQuery param)
        {
            var WhereClause = string.Empty;
            var parameters = BuildCriteria(param, ref WhereClause);

            var dataTable = this.db.GetData("Select * from partner  " + WhereClause + " order by partner_name ", parameters);

            if (dataTable == null) return null;
            if (dataTable.Rows.Count == 0) return null;

            var partners = new List<Partner>();
            foreach (DataRow row in dataTable.Rows)
            {
                var obj = ConvertDataRowToPartner(row);
                partners.Add(obj);
            }
            return partners;
        }
        public List<Partner> GetPartnersWithPaging(PartnerQuery param)
        {
            var WhereClause = string.Empty;
            var parameters = BuildCriteria(param, ref WhereClause);

            var strSqlStatment = new StringBuilder();
            strSqlStatment.Append("Select * from ( ");
            strSqlStatment.Append("select rownum as seq , main_data.* from ( ");
            strSqlStatment.Append("Select * from partner  " + WhereClause + " order by partner_name ");
            strSqlStatment.Append(") main_data ) ");
            strSqlStatment.Append($"WHERE seq >= ({param.Paging.PageNo - 1}) * {param.Paging.PageSize} AND ROWNUM <= {param.Paging.PageSize}");
            var dataTable = this.db.GetData(strSqlStatment.ToString(), parameters);

            if (dataTable == null) return null;
            if (dataTable.Rows.Count == 0) return null;

            var partners = new List<Partner>();
            foreach (DataRow row in dataTable.Rows)
            {
                var obj = ConvertDataRowToPartner(row);
                partners.Add(obj);
            }
            return partners;
        }
        public int GetCount(PartnerQuery param)
        {
            string WhereClause = string.Empty;
            var parameters = BuildCriteria(param, ref WhereClause);
            var strSqlStatment = new StringBuilder();
            strSqlStatment.Append($"Select count(*) val from partner  { WhereClause }");
            var count = this.db.GetIntScalarValue(strSqlStatment.ToString(), parameters);
            return count;
        }
        private List<OracleParameter> BuildCriteria(PartnerQuery param, ref string criteria)
        {
            var WhereClause = new StringBuilder();
            var parameters = new List<OracleParameter>();
            if (!string.IsNullOrEmpty(param.QPartnerId))
            {
                var parm = new OracleParameter { ParameterName = "QPartnerId", OracleDbType = OracleDbType.Varchar2, Value = param.QPartnerId };
                WhereClause.Append(" WHERE partner_id=:QPartnerId ");
                parameters.Add(parm);
            }
            if (param.QAccount > 0)
            {
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE partner_acc=:QAccount " : " AND partner_acc=:QAccount  ");
                var parm = new OracleParameter { ParameterName = "QAccount", OracleDbType = OracleDbType.Int32, Value = param.QAccount };
                parameters.Add(parm);
            }
            if (!string.IsNullOrEmpty(param.QRefPartnerId))
            {
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE ref_partner=:QRefPartnerId " : " AND ref_partner=:QRefPartnerId  ");
                var parm = new OracleParameter { ParameterName = "QRefPartnerId", OracleDbType = OracleDbType.Varchar2, Value = param.QRefPartnerId };
                parameters.Add(parm);
            }
            if (param.QRoleId > 0)
            {
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE roleid=:QRoleId " : " AND roleid=:QRoleId  ");
                var parm = new OracleParameter { ParameterName = "QRoleId", OracleDbType = OracleDbType.Int32, Value = param.QRoleId };
                parameters.Add(parm);
            }
            if (param.QStatusId > 0)
            {
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE status=:QStatusId " : " AND status=:QStatusId  ");
                var parm = new OracleParameter { ParameterName = "QStatusId", OracleDbType = OracleDbType.Int32, Value = param.QStatusId };
                parameters.Add(parm);
            }

            if (!string.IsNullOrEmpty(param.QPartnerName))
            {
                //param.QPartnerName = Utility.RemoveSpecialChar(param.QPartnerName);
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE (partner_name LIKE '%' ||  :QPartnerName || '%') "
                    : " AND (partner_name LIKE '%' ||  :QPartnerName || '%') ");
                var parm = new OracleParameter { ParameterName = "QPartnerName", OracleDbType = OracleDbType.NVarchar2, Value = param.QPartnerName };
                parameters.Add(parm);
            }
            criteria = WhereClause.ToString();
            return parameters;
        }

        public async Task<OpertionResult> EditAsync(Partner oldPartner, Partner newPartner)
        {
            
            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                 new OracleParameter{ ParameterName = "v_partner_name", OracleDbType = OracleDbType.NVarchar2,  Value = newPartner.Name },
                 new OracleParameter{ ParameterName = "v_brandname",OracleDbType = OracleDbType.NVarchar2,  Value = newPartner.BrandName },
                 new OracleParameter{ ParameterName = "v_roleid",OracleDbType = OracleDbType.Int32,  Value = newPartner.Role.Id },
                 new OracleParameter{ ParameterName = "v_id_no", OracleDbType = OracleDbType.Varchar2, Value = newPartner.PersonalId.Id },
                 new OracleParameter{ ParameterName = "v_id_type", OracleDbType = OracleDbType.Int32, Value = newPartner.PersonalId.IdType.Id },
                 new OracleParameter{ ParameterName = "v_id_place", OracleDbType = OracleDbType.NVarchar2, Value = newPartner.PersonalId.Place },
                 new OracleParameter{ ParameterName = "v_id_issued", OracleDbType = OracleDbType.Date, Value = newPartner.PersonalId.Issued },
                 new OracleParameter{ ParameterName = "v_createdby", OracleDbType = OracleDbType.Varchar2, Value = newPartner.CreatedBy.Id },
                 new OracleParameter{ ParameterName = "v_cityid", OracleDbType = OracleDbType.Int32, Value = newPartner.Address.City.Id},
                 new OracleParameter{ ParameterName = "v_districtid", OracleDbType = OracleDbType.Int32, Value = newPartner.Address.District.Id },
                 new OracleParameter{ ParameterName = "v_street", OracleDbType = OracleDbType.NVarchar2, Value = newPartner.Address.Street },
                 new OracleParameter{ ParameterName = "v_zone", OracleDbType = OracleDbType.NVarchar2, Value = newPartner.Address.Zone },
                 new OracleParameter{ ParameterName = "v_extra_address", OracleDbType = OracleDbType.NVarchar2, Value = newPartner.Address.ExtraInfo },
                 new OracleParameter{ ParameterName = "v_pair_mobile", OracleDbType = OracleDbType.Varchar2, Value = newPartner.PairMobile },
                 new OracleParameter{ ParameterName = "v_mobile", OracleDbType = OracleDbType.Varchar2, Value = newPartner.ContactInfo.Mobile },
                 new OracleParameter{ ParameterName = "v_fixed", OracleDbType = OracleDbType.Varchar2, Value = newPartner.ContactInfo.Fixed },
                 new OracleParameter{ ParameterName = "v_fax", OracleDbType = OracleDbType.Varchar2, Value = newPartner.ContactInfo.Fax },
                 new OracleParameter{ ParameterName = "v_email", OracleDbType = OracleDbType.Varchar2, Value = newPartner.ContactInfo.Email },
                 new OracleParameter{ ParameterName = "v_ip_address", OracleDbType = OracleDbType.Varchar2, Value = newPartner.IPAddress},
                 new OracleParameter{ ParameterName = "v_partner_acc", OracleDbType = OracleDbType.Int32, Value = newPartner.Account},
                 new OracleParameter{ ParameterName = "v_ref_partner", OracleDbType = OracleDbType.Varchar2, Value = newPartner.RefPartner.Id}
            };

                #endregion
                await db.ExecuteStoredProcAsync("pk_infra.fn_updatepartner", parameters);
                var result = int.Parse(parameters.Find(x => x.ParameterName == "retVal").Value.ToString());

                if (result > 0)
                {
                    //newPartner = GetActivePartner(newPartner.Id);
                    //var audit = new DataAudit();
                    //audit.Activity.Id = "Partner.Edit";
                    //audit.PartnerId = newPartner.CreatedBy.Id;
                    //audit.PartnerAccount = newPartner.CreatedBy.Account;
                    //audit.Action.Id = "Update";
                    //audit.Success = true;
                    //audit.OldValue = oldPartner.ToString();
                    //audit.NewValue = newPartner.ToString();
                    //var auditResult = new DataAuditRepo(db).Create(audit);
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

        public async Task<List<IdName>> GetAccountsAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            var WhereClause = new StringBuilder();
            var parameters = new List<OracleParameter>();
            var parm = new OracleParameter { ParameterName = "PartnerId", OracleDbType = OracleDbType.Varchar2, Value = id };
            WhereClause.Append(" WHERE partner_id=:PartnerId ");
            parameters.Add(parm);
            var dataTable = await this.db.GetDataAsync("Select * from partner  " + WhereClause + " order by partner_acc desc", parameters);

            if (dataTable == null) return null;
            if (dataTable.Rows.Count == 0) return null;

            var partners = new List<IdName>();
            foreach (DataRow row in dataTable.Rows)
            {
                var account = new IdName();
                account.Id = row["partner_acc"] == DBNull.Value ? -1 : int.Parse(row["partner_acc"].ToString());
                var partnerName = row["partner_name"] == DBNull.Value ? string.Empty : row["partner_name"].ToString();
                account.Name = $"{account.Id} - {partnerName}";
                partners.Add(account);
            }
            return partners;
        }

        public async Task<int> GetUnreadUserNotifyCountAsync(string id)
        {
            return await Task.Run(() => {
                return new UserNotifyHistoryRepo(db).UserNotifyHisCount(id);
                });
            ;
        }
        public async Task<List<UserNotifyHistory>> GetUnreadUserNotifyListAsync(string id)
        {
            return await Task.Run(() => {
                return new UserNotifyHistoryRepo(db).GetUnreadListForPartner(id);
            });
            ;
        }

        public double GetBalance(int acc)
        {

            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                    new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                    new OracleParameter{ ParameterName = "v_partner_acc", OracleDbType = OracleDbType.Int32,  Value = acc }
                };

                #endregion
                db.ExecuteStoredProc("pk_utility.fn_getpartnerbalance", parameters);
                var result = double.Parse(parameters.Find(x => x.ParameterName == "retVal").Value.ToString());

                return result;
            }
            catch (Exception ex)
            {
                return -2;
            }
        }
    }
}
