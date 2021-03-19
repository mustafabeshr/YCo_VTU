using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly IPartnerActivityRepo _partnerActivity;
        private readonly ITokenService _tokenService;

        public AccountController(IAppDbContext db,
              IPartnerManager partnerManager,
             IPartnerActivityRepo partnerActivity,
             ITokenService tokenService)
        {
            this._db = db;
            this._partnerManager = partnerManager;
            this._partnerActivity = partnerActivity;
            this._tokenService = tokenService;
        }
        [HttpPost("/api/login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var partnerResult = this._partnerManager.Validate(loginDto.UserId);
            if (!partnerResult.Success) return Unauthorized(new ApiResponse(401));

            var remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;

            if(partnerResult.Partner.IPAddress != remoteIpAddress.ToString()) return Unauthorized(new ApiResponse(401));

            if (partnerResult.Partner.Status.Id > 2) return Unauthorized(new ApiResponse(401, "Your account is not in correct state"));

            if (partnerResult.Partner.LockTime > DateTime.Now) return Unauthorized(new ApiResponse(401, $"Your account is suspended for {Utility.HowMuchLeftTime(partnerResult.Partner.LockTime)}"));

            byte[] salt = Convert.FromBase64String(partnerResult.Partner.Extra);
            string hash = Pbkdf2Hasher.ComputeHash(loginDto.Password, salt);

            if (partnerResult.Partner.Pwd != hash)
            {
                bool lockAccount = false;
                if (partnerResult.Partner.WrongPwdAttempts >= 2) lockAccount = true;
                _partnerManager.IncreaseWrongPwdAttempts(partnerResult.Partner.Id, lockAccount);
                return BadRequest(new ApiResponse(400, "Incorrect credentials"));
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
        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            var userId = HttpContext.User.Identity.Name;


            return Ok(userId);
        }
    }
}
