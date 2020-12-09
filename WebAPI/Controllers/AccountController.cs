﻿using DataLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModelAndRequest.Account;
using ServiceLayer.AccountServices;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Route("api/account/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService accountService;

        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }
        [HttpPost]
        public async Task<IActionResult> login([FromBody] LoginRequest loginRequest)
        {
            var token = await accountService.Login(loginRequest);
            return Ok(token);
        }

        [HttpPost]
        public async Task<IActionResult> register([FromBody] RegisterRequest registerRequest)
        {
            var result = await accountService.Register(registerRequest);

            return Ok(result);
        }


        [HttpGet]
        [Route("/api/account")]
        [Authorize]
        public async Task<IActionResult> GetAccount(Guid id)
        {

            var result = await accountService.GetById(id);
            return Ok(result);
        }

        [HttpDelete]
        [Authorize(policy: "Admin")]
        [Route("/api/admin/account")]
        public async Task<IActionResult> DeleteAccount(Guid id)
        {
            var result = await accountService.DeleteAccount(id);
            return Ok(result);
        }


        [HttpGet]
        [Authorize(policy: "Admin")]
        [Route("/api/admin/account/sales/all")]
        public async Task<IActionResult> getAllSales()
        {
            var result = await accountService.GetAllAccount("Sales");
            return Ok(result);
        }

        [HttpPost]
        [Authorize(policy: "Admin")]
        [Route("/api/admin/account/sales")]
        public async Task<IActionResult> CreateSales([FromBody] RegisterRequest registerRequest)
        {
            var result = await accountService.CreateSales(registerRequest);
            return Ok(result);
        }


        [HttpGet]
        [Authorize(policy: "Sales")]
        [Route("/api/admin/account/user/all")]
        public async Task<IActionResult> getAllUser()
        {
            var result = await accountService.GetAllAccount("User");
            return Ok(result);
        }
    }
}
