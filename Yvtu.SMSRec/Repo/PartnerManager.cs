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
using Yvtu.Infra.Data;

namespace Yvtu.SMSRec.Repo
{
    public class PartnerManager
    {
        private readonly IRecDbContext db;

        public PartnerManager(IRecDbContext db)
        {
            this.db = db;
        }

        public bool ChangePwd(int PartnerAcc, string PartnerId,string newPwd)
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
                db.ExecuteStoredProc("pk_infra.fn_resetpassword", parameters);
                var result = int.Parse(parameters.Find(x => x.ParameterName == "retVal").Value.ToString());

                if (result > 0)
                {
                    var msg = "تم تغيير كلمة المرور الخاصبة بك الى " + newPwd;
                    new SMSRec.Repo.OutSMSRepo(db).Create(new SMSOut
                    {
                        Receiver = PartnerId,
                        Message = msg
                    });
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
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
                 new OracleParameter{ ParameterName = "v_extra",OracleDbType = OracleDbType.Varchar2,  Value = Convert.ToBase64String(salt) }
                };

                #endregion
                db.ExecuteStoredProc("pk_infra.fn_resetpassword", parameters);
                var result = int.Parse(parameters.Find(x => x.ParameterName == "retVal").Value.ToString());

                if (result > 0)
                {
                    var msg = "تم اعادة تعيين كلمة المرور الخاصة بك الى " + pass;
                    new SMSRec.Repo.OutSMSRepo(db).Create(new SMSOut
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
            catch 
            {
                return false;
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
            partner.Pwd = dataRow["pwd"] == DBNull.Value ? string.Empty : dataRow["pwd"].ToString();
            partner.Extra = dataRow["Extra"] == DBNull.Value ? string.Empty : dataRow["Extra"].ToString();
            partner.BrandName = dataRow["brandname"] == DBNull.Value ? string.Empty : dataRow["brandname"].ToString();
            partner.Balance = dataRow["balance"] == DBNull.Value ? long.MinValue : long.Parse(dataRow["balance"].ToString());
            partner.Reserved = dataRow["reserved"] == DBNull.Value ? long.MinValue : long.Parse(dataRow["reserved"].ToString());
            partner.VerificationCodeNext = dataRow["verificationcodenext"] == DBNull.Value ? false : dataRow["verificationcodenext"].ToString() == "1" ? true : false;
            partner.LockTime = dataRow["locktime"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(dataRow["locktime"].ToString());
            partner.Role.Id = dataRow["roleid"] == DBNull.Value ? int.MinValue : int.Parse(dataRow["roleid"].ToString());
            partner.Role.Name = dataRow["role_name"] == DBNull.Value ? string.Empty : dataRow["role_name"].ToString();
            partner.PersonalId.Id = dataRow["id_no"] == DBNull.Value ? string.Empty : dataRow["id_no"].ToString();
            partner.PersonalId.IdType.Id = dataRow["id_type"] == DBNull.Value ? int.MinValue : int.Parse(dataRow["id_type"].ToString());
            partner.PersonalId.IdType.Name = dataRow["id_type_name"] == DBNull.Value ? string.Empty : dataRow["id_type_name"].ToString();
            partner.PersonalId.Place = dataRow["id_place"] == DBNull.Value ? string.Empty : dataRow["id_place"].ToString();
            partner.PersonalId.Issued = dataRow["id_issued"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(dataRow["id_issued"].ToString());
            partner.Status.Id = dataRow["status"] == DBNull.Value ? int.MinValue : int.Parse(dataRow["status"].ToString());
            partner.Status.Name = dataRow["status_name"] == DBNull.Value ? string.Empty : dataRow["status_name"].ToString();
            partner.StatusOn = dataRow["statuson"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(dataRow["statuson"].ToString());
            partner.StatusBy.Id = dataRow["statusby"] == DBNull.Value ? string.Empty : dataRow["statusby"].ToString();
            partner.StatusBy.Name = dataRow["statusby_name"] == DBNull.Value ? string.Empty : dataRow["statusby_name"].ToString();
            partner.CreatedOn = dataRow["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(dataRow["createdon"].ToString());
            partner.CreatedBy.Id = dataRow["createdby"] == DBNull.Value ? string.Empty : dataRow["createdby"].ToString();
            partner.CreatedBy.Name = dataRow["createdby_name"] == DBNull.Value ? string.Empty : dataRow["createdby_name"].ToString();
            partner.Address.City.Id = dataRow["cityid"] == DBNull.Value ? int.MinValue : int.Parse(dataRow["cityid"].ToString());
            partner.Address.City.Name = dataRow["city_name"] == DBNull.Value ?string.Empty : dataRow["city_name"].ToString();
            partner.Address.District.Id = dataRow["districtid"] == DBNull.Value ? int.MinValue : int.Parse(dataRow["districtid"].ToString());
            partner.Address.District.Name = dataRow["district_name"] == DBNull.Value ? string.Empty : dataRow["district_name"].ToString();
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
            partner.RefPartner.Id = dataRow["REF_PARTNER"] == DBNull.Value ? string.Empty : dataRow["REF_PARTNER"].ToString();
            partner.RefPartner.Name = dataRow["ref_partner_name"] == DBNull.Value ? string.Empty : dataRow["ref_partner_name"].ToString();
            #endregion
            return partner;
        }
        public Partner GetActivePartner(string Id)
        {
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "partnerId", OracleDbType = OracleDbType.Varchar2,  Value = Id },
            };
            var partnerDataTable = db.GetData("select * from v_partner where partner_id=:partnerId and (status < 3)", parameters);
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
            var partnerDataTable = db.GetData("select * from v_partner where partner_acc=:partnerAccount", parameters);
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
                sql = "Update partner set WRONG_PWD_ATTEMPTS = WRONG_PWD_ATTEMPTS + 1, status = 2, statuson=sysdate, locktime = sysdate + 1/24 where partner_id = :PartnerId";
            }
            else
            {
                sql = "Update partner set WRONG_PWD_ATTEMPTS = WRONG_PWD_ATTEMPTS + 1 where partner_id = :PartnerId";
            }
            return db.ExecuteSqlCommand(sql, parameters) > 0;
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
