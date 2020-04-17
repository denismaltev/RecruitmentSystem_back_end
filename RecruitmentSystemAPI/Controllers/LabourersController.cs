using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class LabourersController : ControllerBase
    {
        private readonly RecruitmentSystemContext _context;
        private readonly UserManager<SystemUser> _userManager;

        public LabourersController(RecruitmentSystemContext context, UserManager<SystemUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Labourers
        [HttpGet]
        public ActionResult<IEnumerable<LabourerVM>> GetLabourers()
        {
            var labourerRepo = new LabourerRepo(_context);
            var result = labourerRepo.GetLabourers();
            return Ok(result);
        }

        // GET: api/Labourers/5
        [HttpGet("{id}")]
        public ActionResult<LabourerVM> GetLabourer(int id)
        {
            var labourerRepo = new LabourerRepo(_context);
            var result = labourerRepo.GetLabourerById(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // PUT: api/Labourers/5
        [HttpPut("{id}")]
        public IActionResult PutLabourer(int id, LabourerVM labourerVM)
        {
            if (id != labourerVM.Id)
            {
                return BadRequest();
            }
            try
            {
                var labourerRepo = new LabourerRepo(_context);
                labourerRepo.UpdateLabourer(labourerVM);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }

        }

        // POST: api/Companies
        [HttpPost]
        public ActionResult<CompanyVM> PostLabourer(LabourerVM labourerVM)
        {

            try
            {
                var labourerRepo = new LabourerRepo(_context);
                var userId = _userManager.GetUserId(User);

                if (labourerRepo.GetUserLabourerId(userId).HasValue)
                {
                    return BadRequest(new { message = "Labourer already exist" });
                }

                var result = labourerRepo.AddLabourer(labourerVM, userId);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }

        }
    }
}
