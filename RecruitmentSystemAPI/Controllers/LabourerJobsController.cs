using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    public class LabourerJobsController : ControllerBase
    {
        private readonly RecruitmentSystemContext _context;
        private readonly UserManager<SystemUser> _userManager;

        public LabourerJobsController(RecruitmentSystemContext context, UserManager<SystemUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/LabourerJobs
        [HttpGet]
        [Authorize(Roles = "Labourer")]
        public ActionResult<IEnumerable<LabourerJobVM>> GetLabourerJobs(int count = 20, int page = 1, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var labourerJobsRepo = new LabourerJobsRepo(_context);
            var userId = _userManager.GetUserId(User);
            var result = labourerJobsRepo.GetLabourerJobsByUserId(userId, count, page, fromDate, toDate);
            return Ok(result);
        }
                
        // POST: api/LabourerJobs
        [HttpPost]
        [Authorize(Roles = "Labourer")] //TODO: consider to remove the function, bc there is no option to add labourer's job from client
        public ActionResult<LabourerJobVM> PostLabourerJob([FromBody]LabourerJobVM labourerJobVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var labourerJobsRepo = new LabourerJobsRepo(_context);
                    var userId = _userManager.GetUserId(User);
                    var result = labourerJobsRepo.AddLabourerJob(labourerJobVM, userId);
                    return Ok(result);
                }
                catch (Exception e)
                {
                    return StatusCode(500, new { message = e.Message });
                }
            }
            return BadRequest();
        }
        [HttpPut]
        [Route("UpdateJobRating")]
        [Authorize(Roles = "Labourer")]
        public ActionResult UpdateJobRating([FromQuery]int idToGrade, [FromQuery] int rating)
        {
            var labourerJobsRepo = new LabourerJobsRepo(_context);
            var usesrId = _userManager.GetUserId(User);
            labourerJobsRepo.UpdateJobRating(idToGrade, rating, usesrId);
            return Ok();
        }
    }
}
