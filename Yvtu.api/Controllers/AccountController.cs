using System;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yvtu.api.Dtos;
using Yvtu.api.Errors;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data;
using Yvtu.Infra.Data.Interfaces;
using Yvtu.Infra.Services;

namespace Yvtu.api.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly IAppDbContext _db;
        private readonly IPartnerManager _partnerManager;
        private readonly ITokenService _tokenService;
        private readonly IApiDbLog _apiDbLog;

        public AccountController(IAppDbContext db,
              IPartnerManager partnerManager,
             ITokenService tokenService,
             IApiDbLog apiDbLog)
        {
            this._db = db;
            this._partnerManager = partnerManager;
            this._tokenService = tokenService;
            this._apiDbLog = apiDbLog;
        }
        [HttpPost("/api/login")]
        public ActionResult<UserDto> Login(LoginDto loginDto)
        {
            var remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;
            var partnerResult = this._partnerManager.Validate(loginDto.UserId);
            if (!partnerResult.Success) {
                var response = Unauthorized(new ApiResponse(401));
                _apiDbLog.Create(new ApiLogFile { Data = JsonSerializer.Serialize(response), Action = "login", Ip = remoteIpAddress.ToString(), 
                    Level = 0, User = loginDto.UserId });
                return response;
            }

            if (new ApiIPBlacklistRepo(_db).isBlacklisted(remoteIpAddress.ToString()))
            {
                var response = Unauthorized(new ApiResponse(401));
                _apiDbLog.Create(new ApiLogFile { Data = $" blacklist ip {JsonSerializer.Serialize(response)}", Action = "login", Ip = remoteIpAddress.ToString(), Level = 0, User = loginDto.UserId });
                return Unauthorized(new ApiResponse(401));  
            }

            if (partnerResult.Partner.IPAddress != remoteIpAddress.ToString())
            {
                new ApiIPBlacklistRepo(_db).Create(new ApiIPBlacklist { IPAddress = remoteIpAddress.ToString() });
                return Unauthorized(new ApiResponse(401));
            }

            if (partnerResult.Partner.Status.Id > 2) return Unauthorized(new ApiResponse(401, "Sorry, your account is not in correct state"));

            if (partnerResult.Partner.LockTime > DateTime.Now) return Unauthorized(new ApiResponse(401, $"Sorry, your account is suspended for {Utility.HowMuchLeftTime(partnerResult.Partner.LockTime)}"));

            byte[] salt = Convert.FromBase64String(partnerResult.Partner.Extra);
            string hash = Pbkdf2Hasher.ComputeHash(loginDto.Password, salt);

            if (partnerResult.Partner.Pwd != hash)
            {
                bool lockAccount = partnerResult.Partner.WrongPwdAttempts >= 2;
                _partnerManager.IncreaseWrongPwdAttempts(partnerResult.Partner.Id, lockAccount);
                return BadRequest(new ApiResponse(400, "Sorry, incorrect credentials"));
            }

            _partnerManager.PreSuccessLogin(partnerResult.Partner.Id);

            var user = new AppUser
            {
                 Account = partnerResult.Partner.Account,
                 Id = partnerResult.Partner.Id,
                 Name = partnerResult.Partner.Name
            };

            return new UserDto
            {
                Id = user.Id,
                Token = _tokenService.CreateToken(user),
                Account = user.Account
            };
        }


        [Authorize]
        [HttpGet("/api/balance")]
        public IActionResult BalanceQuery()
        {
            var currentUser = _partnerManager.GetPartnerById(this.HttpContext.User.Identity.Name);

            return Ok(new
            {
                resultCode = 0,
                resultDesc = "OK",
                balance = currentUser.Balance,
                reserved = currentUser.Reserved,
                queryTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
            });
        }

        [Authorize]
        [HttpPost("/api/chgSecret")]
        public IActionResult ChangePassword(ChangeSecretDto model)
        {
            var currentUser = _partnerManager.GetPartnerById(this.HttpContext.User.Identity.Name);
            if (currentUser.Status.Id > 2) return Unauthorized(new ApiResponse(-3100, "Sorry, your account is not in correct state"));
            if (currentUser.LockTime > DateTime.Now) return Unauthorized(new ApiResponse(-3101, $"Sorry, your account is suspended for {Utility.HowMuchLeftTime(currentUser.LockTime)}"));
            if (!System.Text.RegularExpressions.Regex.IsMatch(model.newSecret.ToString(), "^[0-9]{4,6}$"))
                return BadRequest(new ApiResponse(-3102, $"Sorry, new secret was invalid"));

            byte[] salt = Convert.FromBase64String(currentUser.Extra);
            string hash = Pbkdf2Hasher.ComputeHash(model.oldSecret.ToString(), salt);

            if (currentUser.Pwd != hash)
            {
                return BadRequest(new ApiResponse(-3103, "Sorry, incorrect credentials"));
            }

            var result = _partnerManager.ChangePwd(currentUser.Account, currentUser.Id, model.newSecret.ToString(), false);
            if (!result) return BadRequest(new ApiResponse(-3104, $"Sorry, change secrect was failed, please try later"));

            return Ok(new ApiResponse(0, $"Changed successfully"));
        }
    }
}
