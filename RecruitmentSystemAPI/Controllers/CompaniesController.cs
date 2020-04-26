using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitmentSystemAPI.Models;
using RecruitmentSystemAPI.Repositories;
using RecruitmentSystemAPI.ViewModels;

namespace RecruitmentSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CompaniesController : ControllerBase
    {
        private readonly RecruitmentSystemContext _context;
        private readonly UserManager<SystemUser> _userManager;
        private readonly CompanyRepo _companyRepo;

        public CompaniesController(RecruitmentSystemContext context, UserManager<SystemUser> userManager, CompanyRepo companyRepo)
        {
            _context = context;
            _userManager = userManager;
            _companyRepo = companyRepo;
        }

        // GET: api/Companies
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult<IEnumerable<CompanyVM>> GetCompanies()
        {
            var result = _companyRepo.GetCompanies();
            return Ok(result);
        }

        // GET: api/Companies/5
        [HttpGet("{id}")]
        public ActionResult<Company> GetCompany(int id)
        {
            var result = _companyRepo.GetCompanyById(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // PUT: api/Companies/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Company")]
        public IActionResult PutCompany(int id, CompanyVM companyVM)
        {
            if (ModelState.IsValid)
            {
                if (id != companyVM.Id)
                {
                    return BadRequest();
                }

                try
                {
                    _companyRepo.UpdateCompany(companyVM);
                    return Ok();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_companyRepo.CompanyExists(id))
                    {
                        return NotFound();
                    }
                }
                catch (KeyNotFoundException)
                {
                    return NotFound();
                }
                catch (Exception e)
                {
                    return StatusCode(500, new { status = 500, message = e.Message });
                }

                return NoContent();
            }
            return BadRequest();
        }

        // POST: api/Companies
        [HttpPost]
        [Authorize(Roles = "Company")]
        public ActionResult<CompanyVM> PostCompany(CompanyVM companyVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = _userManager.GetUserId(User);
                    if (_companyRepo.GetUserCompanyId(userId).HasValue)
                    {
                        return BadRequest(new { message = "Company already exist" });
                    }
                    var result = _companyRepo.AddCompany(companyVM, userId);
                    return Ok(result);
                }
                catch(Exception e)
                {
                    return StatusCode(500, new { message = e.Message });
                }
            }
            return BadRequest();
        }
    }
}
