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
    public class JobsController : ControllerBase
    {
        private readonly RecruitmentSystemContext _context;
        private readonly UserManager<SystemUser> _userManager;

        public JobsController(RecruitmentSystemContext context, UserManager<SystemUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //GET: api/Jobs
       [HttpGet]
       [Authorize(Roles = "Company, Admin")]
        public ActionResult<IEnumerable<JobVM>> GetJobs(int? companyId)
        {
            var jobRepo = new JobRepo(_context);
            var userId = _userManager.GetUserId(User);
            if (!companyId.HasValue)
            {
                var result = jobRepo.GetCompanyJobsByUserId(userId);
                return Ok(result);
            }else
            {
                var result = jobRepo.GetJobsByCompanyId(companyId.Value);
                 return Ok(result);
            }
        }


        // GET: api/Jobs/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Company")]
        public ActionResult<JobVM> GetJob(int id)
        {
            var jobRepo = new JobRepo(_context);
            var result = jobRepo.GetJobById(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // PUT: api/Jobs/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Company")]
        public IActionResult PutJob(int id, JobVM jobVM)
        {
            if (ModelState.IsValid)
            {
                if (id != jobVM.Id)
                {
                    return BadRequest();
                }

                var jobRepo = new JobRepo(_context);
                try
                {
                    jobRepo.UpdateJob(jobVM);
                    return Ok();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!jobRepo.JobExists(id))
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

        // POST: api/Jobs
        [HttpPost]
        [Authorize(Roles = "Company")]
        public ActionResult<JobVM> PostJob([FromBody]JobVM jobVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var jobRepo = new JobRepo(_context);
                    var userId = _userManager.GetUserId(User);
                    var result = jobRepo.AddJob(jobVM, userId);
                    return Ok(result);
                }
                catch (Exception e)
                {
                    return StatusCode(500, new { message = e.Message });
                }
            }
            return BadRequest();
        }
    }
}
