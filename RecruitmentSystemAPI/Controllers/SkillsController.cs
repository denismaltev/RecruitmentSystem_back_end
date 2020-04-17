using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitmentSystemAPI.Models;
using RecruitmentSystemAPI.Repositories;
using RecruitmentSystemAPI.ViewModels;

namespace RecruitmentSystemAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class SkillsController : ControllerBase
    {
        private readonly RecruitmentSystemContext _context;
        public SkillsController(RecruitmentSystemContext context)
        {
            _context = context;
        }

        // GET: api/Skills
        [HttpGet]
        public ActionResult<IEnumerable<SkillsVM>> GetAll()
        {
            var skillRepo = new SkillsRepo(_context);
            var skills = skillRepo.GetSkills();
            return Ok(skills);
        }

        // GET: api/Skills/5
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {
            var skillRepo = new SkillsRepo(_context);
            var skill = skillRepo.GetSkillById(id);
            if (skill == null)
            {
                return NotFound();
            }
            return Ok(skill);
        }

        //POST: api/Skills
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult<SkillsVM> CreateSkill(SkillsVM skillVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var skillRepo = new SkillsRepo(_context);
                    if (skillRepo.SkillAlreadyExists(skillVM.Name))
                    {
                        return BadRequest(new { message = "Skill already exists" });
                    }
                    var result = skillRepo.AddSkill(skillVM);
                    return Ok(result);
                }
                catch (Exception e)
                {
                    return StatusCode(500, new { message = e.Message });
                }
            }
            return BadRequest();
        }


        //PUT: api/Skills/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult Put(int id, SkillsVM skillsVM)
        {
            if (ModelState.IsValid)
            {
                if (id != skillsVM.Id)
                {
                    return BadRequest();
                }

                var skillRepo = new SkillsRepo(_context);
                try
                {
                    skillRepo.UpdateSkill(skillsVM);
                    return Ok();
                }
                catch(DbUpdateConcurrencyException)
                {
                    if(!skillRepo.SkillExists(id))
                    {
                        return NotFound();
                    }
                }
                catch (KeyNotFoundException)
                {
                    return NotFound();
                }
                catch(Exception e)
                {
                    return StatusCode(500, new { status = 500, message = e.Message });
                }
                return NoContent();
            }
            return BadRequest();
        }
    }
}

