using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Wsds.DAL.Identity;

namespace Wsds.WebApp.Auth
{
    public class AuthOpt
    {
        public static string Issuer { get; private set; }
        public static string Subscriber { get; private set; }
        public static string Key { get; private set; }
        public static int LifeTime { get; private set; }

        public static JwtBearerOptions InitToken(IConfigurationSection tokenSection)
        {
            Initialization(tokenSection);

            var tokenOptions = new JwtBearerOptions
            {
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = Issuer,
                    ValidateAudience = true,
                    ValidAudience = Subscriber,
                    ValidateLifetime = false,
                    IssuerSigningKey = GetSymmetricSecurityKey(),
                    ValidateIssuerSigningKey = true,
                }
            };

            return tokenOptions;
        }

        public static string GetToken(AppUser appUser, long? clientId, IEnumerable<string> roles=null)
        {
            var dateNow = DateTime.UtcNow;

            var claims = new List<Claim>
            {
                new Claim("phone", appUser.UserName.ToLower()),
                new Claim("card",appUser.Card.ToString()),
                new Claim("clientId",clientId.ToString())
            };

            if(roles!=null && roles.Any())
               claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, roles.First()));

            ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);


            var jwt = new JwtSecurityToken(
                issuer: Issuer,
                audience: Subscriber,
                notBefore: dateNow,
                claims: claimsIdentity.Claims,
                expires: dateNow.Add(TimeSpan.FromMinutes(LifeTime)),
                signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
            );

           return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
        }
        private static void Initialization(IConfigurationSection tokenSection)
        {
            Issuer = tokenSection.GetValue<string>("Publisher");
            Subscriber = tokenSection.GetValue<string>("Subscriber");
            Key = tokenSection.GetValue<string>("Key");
            LifeTime = tokenSection.GetValue<int>("LifeTime");
        }

    }
}
