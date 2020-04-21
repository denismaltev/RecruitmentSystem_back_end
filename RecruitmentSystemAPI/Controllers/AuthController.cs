﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RecruitmentSystemAPI.Models;
using RecruitmentSystemAPI.Repositories;
using RecruitmentSystemAPI.ViewModels;

namespace RecruitmentSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly RecruitmentSystemContext _context;
        private SignInManager<SystemUser> _signInManager;
        private IServiceProvider _serviceProvider;
        private IConfiguration _config;

        public AuthController(RecruitmentSystemContext context, SignInManager<SystemUser> signInManager, IServiceProvider serviceProvider, IConfiguration config)
        {
            _context = context;
            _signInManager = signInManager;
            _serviceProvider = serviceProvider;
            _config = config;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult> Login([FromBody]SystemUserVM userVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(userVM.Email, userVM.Password, false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var userManager = _serviceProvider.GetRequiredService<UserManager<SystemUser>>();
                    var user = await userManager.FindByEmailAsync(userVM.Email);
                    if (user != null)
                    {
                        return BuildLoginOkResponse(user);
                    }
                }
                else if (result.IsLockedOut)
                {
                    return StatusCode(403);
                }
                else
                {
                    return Unauthorized();
                }
            }
            return BadRequest();
        }

        private ActionResult BuildLoginOkResponse(SystemUser user)
        {
            var tokenString = GenerateJSONWebToken(user);
            var userRepo = new SystemUserRepo(_context);
            var userRole = userRepo.GetUserRoleName(user.Id);
            int? profileId = null;
            if (userRole?.ToLower() == "company")
            {
                var companyRepo = new CompanyRepo(_context);
                profileId = companyRepo.GetUserCompanyId(user.Id);
            }
            else if (userRole?.ToLower() == "labourer")
            {
                var labourerRepo = new LabourerRepo(_context, null);
                profileId = labourerRepo.GetUserLabourerId(user.Id);
            }

            return Ok(new { token = tokenString, username = user.UserName, role = userRole?.ToLower(), profileId = profileId, status = "OK" });
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody]SystemUserVM userVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userRepo = new SystemUserRepo(_context);
                    var role = userRepo.GetRoleByName(userVM.RoleName);
                    if (role == null)
                    {
                        return BadRequest(new { message = $"Invalid role {userVM.RoleName}" });
                    }
                    var userManager = _serviceProvider.GetRequiredService<UserManager<SystemUser>>();
                    var result = await userManager.CreateAsync(new SystemUser { Email = userVM.Email, UserName = userVM.Email }, userVM.Password);
                    if (result.Succeeded)
                    {
                        var user = await userManager.FindByEmailAsync(userVM.Email);
                        result = await userManager.AddToRoleAsync(user, role.Name);
                        if (result.Succeeded)
                        {
                            return Ok();
                        }
                    }
                    if (result.Errors != null && result.Errors.Count() > 0)
                    {
                        return BadRequest(new { message = result.Errors.First().Description });
                    }
                }
                catch (Exception e)
                {
                    return StatusCode(500, new { message = e.Message });
                }
            }
            return BadRequest();
        }

        List<Claim> AddUserRoleClaims(List<Claim> claims, string userId)
        {
            // Get current user's roles. 
            var userRoleList = _context.UserRoles.Where(ur => ur.UserId == userId);
            var roleList = from ur in userRoleList
                           from r in _context.Roles
                           where r.Id == ur.RoleId
                           select new { r.Name };

            // Add each of the user's roles to the claims list.
            foreach (var roleItem in roleList)
            {
                claims.Add(new Claim(ClaimTypes.Role, roleItem.Name));
            }
            return claims;
        }

        string GenerateJSONWebToken(IdentityUser user)
        {
            var securityKey
                = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials
                = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti,
                            Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            claims = AddUserRoleClaims(claims, user.Id);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}