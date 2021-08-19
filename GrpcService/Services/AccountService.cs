using BLL.Helpers;
using DAL.Model;
using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GrpcService
{
    public class AccountService : Account.AccountBase
    {
        private readonly ILogger<AccountService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly AppSettings _appSettings;
        public AccountService(ILogger<AccountService> logger, UserManager<User> userManager,
            AppSettings appSettings)
        {
            _logger = logger;
            _userManager = userManager;
            _appSettings = appSettings;
        }

        public override async Task<RegistrationReply> Registration(RegistrationRequest request, ServerCallContext context)
        {
            User user = new User { Email = request.Email, UserName = request.UserName,
                FirstName = request.FirstName, LastName = request.LastName };
            // add new user
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                return new RegistrationReply() { Message = "Registration successful" };
            }
            else
            {
                return new RegistrationReply() { Message = result.Errors.FirstOrDefault().Code};
            }            
        }

        public override async Task<LoginReply> Login(LoginRequest request, ServerCallContext context)
        {
            User user = await _userManager.FindByNameAsync(request.EmailOrUserName);
            if(user == null)
            {
                user = await _userManager.FindByEmailAsync(request.EmailOrUserName);
            }
            //if (user == null)
            //    return Task.FromResult(new LoginReply() { });
            if (await _userManager.CheckPasswordAsync(
                user, request.Password))
            {
                //if (!user.EmailConfirmed)
                    //return BadRequest("Please, confirm e-mail");
                LoginReply loginReply = new LoginReply()
                {
                    Token = CreateJWT(GetIdentity(new List<Claim>() {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id)
                    })),
                    UserName = user.UserName
                };
                return loginReply;
            }
            else
            {
                return new LoginReply();//"Wrong password or userName "
            }            
        }

        private string CreateJWT(ClaimsIdentity claimsIdentity)
        {
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: _appSettings.Issuer,
                audience: _appSettings.Audience,
                notBefore: now,
                claims: claimsIdentity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(_appSettings.Lifetime)),
                signingCredentials: new SigningCredentials(new
                SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(_appSettings.Secret)),
                SecurityAlgorithms.HmacSha256));
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private ClaimsIdentity GetIdentity(List<Claim> claims, string authenticationType = "Token")
        {
            ClaimsIdentity claimsIdentity =
            new ClaimsIdentity(claims, authenticationType, ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }
    }
}
