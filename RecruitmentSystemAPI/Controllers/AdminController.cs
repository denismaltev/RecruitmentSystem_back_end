using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecruitmentSystemAPI.Models;
using RecruitmentSystemAPI.Repositories;
using RecruitmentSystemAPI.ViewModels;

namespace RecruitmentSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<SystemUser> _userManager;
        private readonly LabourerJobsRepo _labourerJobsRepo;

        public AdminController(UserManager<SystemUser> userManager, LabourerJobsRepo labourerJobsRepo)
        {
            _userManager = userManager;
            _labourerJobsRepo = labourerJobsRepo;
        }

        [HttpGet]
        [Route("AnnualProfitReport")]
        [Authorize(Roles = "Admin")]
        public ActionResult<IEnumerable<ChartReportVM>> AnnualProfitReport()
        {
            try
            {
                var result = _labourerJobsRepo.GetAnnualProfitReport();
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }

        [HttpGet]
        [Route("CurrentMonthExpenses")]
        [Authorize(Roles = "Admin")]
        public ActionResult<IEnumerable<ChartReportVM>> CurrentMonthExpenses()
        {
            try
            {
                var result = _labourerJobsRepo.GetCurrentMonthExpenses();
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }

        [HttpGet]
        [Route("CurrentMonthIncome")]
        [Authorize(Roles = "Admin")]
        public ActionResult<IEnumerable<ChartReportVM>> CurrentMonthIncome()
        {
            try
            {
                var result = _labourerJobsRepo.GetCurrentMonthIncome();
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }
    }
}