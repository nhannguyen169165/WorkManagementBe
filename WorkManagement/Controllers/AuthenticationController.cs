﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using WorkManagement.Models;

namespace WorkManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly WorkManagementContext _context;

        public class Authentication
        {
            public string email { get; set; }
            public string password { get; set; }
        }

        public AuthenticationController(WorkManagementContext context)
        {
            _context = context;
        }

        // POST: api/Authentications
        [HttpPost]
        public IActionResult Login([FromBody] Authentication authentication)
        {

            if (authentication == null)
            {
                return BadRequest("Invalid client request");
            }

            var email = authentication.email;
            var password = authentication.password;

            var userData = (from auth in _context.Authentication
                        join user in _context.User
                        on auth.User_id equals user.Id
                        join admin in _context.Admin
                        on auth.Admin_id equals admin.Id
                        select new
                        {
                            userId = auth.User_id,
                            userName = user.Fullname,
                            userEmail = user.Email,
                            userPassword = user.Password,
                            userStatus = user.Status,
                            userRole = user.Role,
                            userColor = user.Color,
                            adminId = auth.Admin_id,
                            company = admin.Company
                        });
            var adminData = _context.Admin;
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

           

            string str = "";
            foreach (var item in userData)
            {
                if (item.userEmail == email && item.userPassword == password)
                {
                    var tokeOptions = new JwtSecurityToken(
                        issuer: "https://localhost:44320",
                        audience: "https://localhost:44320",
                        claims: new List<Claim> {
                              new Claim(ClaimTypes.Role, item.userRole)
                        },
                        expires: DateTime.Now.AddHours(12),
                        signingCredentials: signinCredentials
                    );
                    var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                    var result = new { id = item.userId, AdminId = item.adminId, name = item.userName,status = item.userStatus, role = item.userRole, color = item.userColor, Company = item.company, token = tokenString };
                    str = JsonConvert.SerializeObject(result);
                }
            }
            foreach (var item in adminData)
            {
                if (item.Email== email && item.Password == password)
                {
                    var tokeOptions = new JwtSecurityToken(
                        issuer: "https://localhost:44320",
                        audience: "https://localhost:44320",
                        claims: new List<Claim> {
                              new Claim(ClaimTypes.Role, item.Role)
                        },
                        expires: DateTime.Now.AddHours(12),
                        signingCredentials: signinCredentials
                    );
                    var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                    var result = new { id = item.Id, AdminId = item.Id, name = item.Company, status = item.Status, role = item.Role, token = tokenString };
                    str = JsonConvert.SerializeObject(result);
                }
            }
            if(str != "")
            {
                return Ok(str);
            }
            else
            {
                return Unauthorized();
            }
        }

    }
}