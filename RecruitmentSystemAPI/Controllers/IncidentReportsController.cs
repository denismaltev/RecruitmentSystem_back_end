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
    public class IncidentReportsController : ControllerBase
    {
        private readonly IncidentReportsRepo _incidentReportsRepo;
        private readonly UserManager<SystemUser> _userManager;

        public IncidentReportsController(IncidentReportsRepo incidentReportsRepo, UserManager<SystemUser> userManager)
        {
            _incidentReportsRepo = incidentReportsRepo;
            _userManager = userManager;
        }

        // GET: api/IncidentReports
        [HttpGet]
        [Authorize(Roles = "Admin, Company")]
        public ActionResult<IEnumerable<IncidentReportVM>> GetIncidentReports(int? companyId = null, int count = 20, int page = 1, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var result = _incidentReportsRepo.GetIncidentReports(userId, User.IsInRole("Admin"), companyId, count, page, fromDate, toDate);
                return Ok(new { result = result.Item2, totalRows = result.Item1 });
            }
            catch(Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }

        // GET: api/IncidentReports/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Company")]
        public ActionResult<IncidentReportVM> GetIncidentReport(int id)
        {
            var userId = _userManager.GetUserId(User);
            var incidentReport = _incidentReportsRepo.GetIncidentReportDetails(id, userId, User.IsInRole("Admin"));

            if (incidentReport == null)
            {
                return NotFound();
            }

            return Ok(incidentReport);
        }

        // PUT: api/IncidentReports/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Company")]
        public IActionResult PutIncidentReport(int id, IncidentReportVM incidentReportVM)
        {
            if (id != incidentReportVM.Id)
            {
                return BadRequest();
            }
            try
            {
                var userId = _userManager.GetUserId(User);
                var incidentReport = _incidentReportsRepo.UpdateIncidentReport(incidentReportVM, userId, User.IsInRole("Admin"));
                if(incidentReport == null)
                {
                    return NotFound();
                }
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }

        // POST: api/IncidentReports
        [HttpPost]
        [Authorize(Roles = "Company")]
        public ActionResult<IncidentReportVM> PostIncidentReport(IncidentReportVM incidentReportVM)
        {
            var userId = _userManager.GetUserId(User);
            try
            {
                var report = _incidentReportsRepo.AddReport(incidentReportVM, userId);
                if (report == null)
                {
                    return BadRequest();
                }
                return Ok(report);
            }
            catch(Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }

        //// DELETE: api/IncidentReports/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<IncidentReport>> DeleteIncidentReport(int id)
        //{
        //    var incidentReport = await _context.IncidentReports.FindAsync(id);
        //    if (incidentReport == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.IncidentReports.Remove(incidentReport);
        //    await _context.SaveChangesAsync();

        //    return incidentReport;
        //}
    }
}
