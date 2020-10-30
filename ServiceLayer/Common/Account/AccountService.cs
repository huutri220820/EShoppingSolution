﻿using DataLayer.EF;
using DataLayer.Entities;
using DataLayer.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ModelAndRequest.API;
using ModelAndRequest.Common;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Common.Account
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IConfiguration config;

        public AccountService()
        {
        }
        public AccountService(UserManager<User> userManager, SignInManager<User> signInManager, IHttpContextAccessor httpContextAccessor, IConfiguration config)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.httpContextAccessor = httpContextAccessor;
            this.config = config;
        }

        public async Task<ApiResult<object>> Login(LoginRequest loginRequest)
        {
            // tim user neu khong co tra ve thong bao loi
            var user = await userManager.FindByNameAsync(loginRequest.Username);
            if (user == null) return new ApiResult<object>(isSuccess: false, messge: "Khong tin thay user", payload: null);

            var result = await signInManager.PasswordSignInAsync(user, loginRequest.Password, loginRequest.Remember, true);
            if (!result.Succeeded)
                return new ApiResult<object>(isSuccess: false, messge: "Mat khau khong chinh xac", payload: null);

            //tao claims chua thong tin de luu vao payload cua token
            var roles = await userManager.GetRolesAsync(user);
            var claims = new[]
            {
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.GivenName,user.FullName),
                new Claim(ClaimTypes.Role, string.Join(";",roles)),
                new Claim(ClaimTypes.Name, loginRequest.Username)
            };
            var roleResult = "user";

            roleResult = roles.Contains("Admin") ? "admin" : roleResult;
            roleResult = roles.Contains("Sales") ? "sales" : roleResult;

            // ma hoa doi xung
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //tao token
            var token = new JwtSecurityToken(config["Tokens:Issuer"],config["Tokens:Issuer"],
                claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds);

            var tokenResult = new JwtSecurityTokenHandler().WriteToken(token);
            return new ApiResult<object>(isSuccess: true, messge: "Dang nhap thanh cong", payload: new { token = tokenResult, role = roleResult});
        }

        public async Task<ApiResult<bool>> Register(RegisterRequest registerRequest, bool isSale = false)
        {
            if (await userManager.FindByNameAsync(registerRequest.Username) != null)
                return new ApiResult<bool>(isSuccess: false, messge: "User name da ton tai", payload: false);
            if(await userManager.FindByEmailAsync(registerRequest.Email) != null)
                return new ApiResult<bool>(isSuccess: false, messge: "Email da ton tai", payload: false);

            var user = new User()
            {
                UserName = registerRequest.Username,
                Email = registerRequest.Email,
                Gender = registerRequest.IsMale,
                Dob = registerRequest.Dob,
                FullName = registerRequest.FullName,
                PhoneNumber = registerRequest.PhoneNumber,
                SecurityStamp=  string.Empty,
                EmailConfirmed = true
            };


            var result = await userManager.CreateAsync(user, registerRequest.Password);

            
            if(result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "User");
                return new ApiResult<bool>(isSuccess: true, messge: "Dang ki thanh cong", payload: true);
            }

            return new ApiResult<bool>(isSuccess: false, messge: string.Join("" , result.Errors.Select(er => er.Description)), payload: false);
        }

        public async Task<ApiResult<bool>> CreateSales(RegisterRequest registerRequest)
        {
            return await Register(registerRequest, true);
        }
    }
}
