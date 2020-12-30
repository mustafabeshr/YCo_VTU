using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Yvtu.Core.Entities;
using Yvtu.Core.Queries;

namespace Yvtu.Infra.Data.Interfaces
{
    public class OpertionResult
    {
        public bool Success { get; set; }
        public int AffectedCount { get; set; }
        public string Error { get; set; }
    }

    public class ValidatePartnerResult
    {
        public Partner Partner { get; set; }
        public bool Success { get; set; }
        public string Error { get; set; }

        public ValidatePartnerResult(Partner Partner = null, bool success = false)
        {
            this.Partner = Partner;
            this.Success = success;
            this.Error = string.Empty;
        }
    }
    public interface IPartnerManager
    {
        ValidatePartnerResult Validate(string partnerId);
        Task<ValidatePartnerResult> ValidateAsync(string partnerId);
        PartBasicInfo GetPartnerBasicInfo(string partnerId);
        OpertionResult Create(Partner partner);
        Task<OpertionResult> CreateAsync(Partner partner);

        bool ChangePwd(int PartnerAcc, string PartnerId, string newPwd);
        Task<OpertionResult> ChangePwdAsync(string PartnerId, string newPwd);
        IEnumerable<Claim> GetUserClaims(Partner user);
        string GetCurrentUserId(HttpContext httpContext);
        Partner GetCurrentUser(HttpContext httpContext);
        Task SignIn(HttpContext httpContext, Partner partner, bool isPersistent = false);
        Task SignOut(HttpContext httpContext);
        bool IncreaseWrongPwdAttempts(string partnerId, bool lockAccount );
        bool PreSuccessLogin(string partnerId);
        Partner GetPartnerByAccount(int account);
        int GetCurrentUserAccount(HttpContext httpContext);
        int GetCurrentUserRole(HttpContext httpContext);
        string GetCurrentUserRoleCode(HttpContext httpContext);
        List<Partner> GetPartners(PartnerQuery param);
        bool ResetPassword(Partner partner);
        Partner GetPartnerById(string id);
        Task<OpertionResult> EditAsync(Partner oldPartner, Partner newPartner);
    }
}
