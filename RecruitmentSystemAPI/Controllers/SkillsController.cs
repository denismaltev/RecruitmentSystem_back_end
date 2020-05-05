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
        private readonly SkillsRepo _skillsRepo;
        public SkillsController(RecruitmentSystemContext context, SkillsRepo skillsRepo)
        {
            _skillsRepo = skillsRepo;
        }

        // GET: api/Skills
        [HttpGet]
        [Route("GetSkillsDDL")]
        public ActionResult<IEnumerable<BaseSkillsVM>> GetSkillsDDL()
        {
            var skills = _skillsRepo.GetSkillsDDL();
            return Ok(skills);
        }

        // GET: api/Skills
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult<IEnumerable<SkillsVM>> GetAll(int count = 20, int page = 1)
        {
            int totalRows;
            var result = _skillsRepo.GetSkills(count, page, out totalRows);
            return Ok(new { result, totalRows});
        }

        // GET: api/Skills/5
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {
            var skill = _skillsRepo.GetSkillById(id);
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
                    if (_skillsRepo.SkillAlreadyExists(skillVM.Name))
                    {
                        return BadRequest(new { message = $"Skill {skillVM.Name} already exists" });
                    }
                    var result = _skillsRepo.AddSkill(skillVM);
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

                try
                {
                    if (_skillsRepo.SkillAlreadyExists(skillsVM.Name, id))
                    {
                        return BadRequest(new { message = $"Skill {skillsVM.Name} already exists" });
                    }
                    _skillsRepo.UpdateSkill(skillsVM);
                    return Ok();
                }
                catch(DbUpdateConcurrencyException)
                {
                    if(!_skillsRepo.SkillExists(id))
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

