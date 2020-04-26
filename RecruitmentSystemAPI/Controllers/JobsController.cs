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
        private readonly JobRepo _jobRepo;

        public JobsController(RecruitmentSystemContext context, UserManager<SystemUser> userManager, JobRepo jobRepo)
        {
            _context = context;
            _userManager = userManager;
            _jobRepo = jobRepo;
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public ActionResult<IEnumerable<JobRatingVM>> GetAllCompanyJobs()
        {
            var result = _jobRepo.GetAllCompanyJobs();
            if (result == null)
            {
                return BadRequest("No matches found");
            }
            return Ok(result);
        }

        // Get all jobs for ONE company
        //GET: api/Jobs
        [HttpGet]
        [Authorize(Roles = "Company, Admin")]
        public ActionResult<IEnumerable<JobVM>> GetJobs(int? companyId, int count = 20, int page = 1, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var userId = _userManager.GetUserId(User);
            if (!companyId.HasValue)
            {
                var result = _jobRepo.GetCompanyJobsByUserId(userId);
                return Ok(result);
            }
            else
            {
                //2020 - 04 - 21T00: 00:00
                var result = _jobRepo.GetJobsByCompanyId(companyId.Value, count, page, fromDate, toDate);
                return Ok(result);
            }
        }

        // Get ONE job from ONE company
        // GET: api/Jobs/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Company, Admin")]
        public ActionResult<JobVM> GetJob(int id)
        {
            var result = _jobRepo.GetJobById(id);
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

                try
                {
                    _jobRepo.UpdateJob(jobVM);
                    return Ok();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_jobRepo.JobExists(id))
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
                    var userId = _userManager.GetUserId(User);
                    var result = _jobRepo.AddJob(jobVM, userId);
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
