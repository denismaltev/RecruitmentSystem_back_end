using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitmentSystemAPI.Models;
using RecruitmentSystemAPI.ViewModels;

namespace RecruitmentSystemAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminSkillsController : ControllerBase
    {
        private readonly RecruitmentSystemContext _context;
        public AdminSkillsController(RecruitmentSystemContext context)
        {
            _context = context;
        }

        // GET: api/AdminSkills
        [HttpGet]
        public IEnumerable<Skill> GetAll()
        {
            return _context.Skills.ToList();
        }

        // GET: api/AdminSkills/5
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {
            var skill = _context.Skills.FirstOrDefault(s => s.Id == id);
            if (skill == null)
            {
                return NotFound();
            }
            return Ok(skill);
        }

        //POST: api/AdminSkills
        [HttpPost]
        public IActionResult Create([FromBody] AdminSkillsVM skillsVM)
        {
            if (ModelState.IsValid)
            {
                Skill skill = new Skill();
                skill.Name = skillsVM.Name;
                skill.ChargeAmount = skillsVM.ChargeAmount;
                skill.PayAmount = skillsVM.PayAmount;
                skill.IsActive = skillsVM.IsActive;

                _context.Skills.Add(skill);
                _context.SaveChanges();
                return new ObjectResult(skill);
            }
            return BadRequest();
        }

        //PUT: api/AdminSkills/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] AdminSkillsVM skillsVM)
        {
            if (ModelState.IsValid)
            {
                var fetchedSkill = _context.Skills.Where(s => s.Id == id).FirstOrDefault();
                if (fetchedSkill == null)
                {
                    return NotFound();
                }
                fetchedSkill.Name = skillsVM.Name;
                fetchedSkill.ChargeAmount = skillsVM.ChargeAmount;
                fetchedSkill.PayAmount = skillsVM.ChargeAmount;
                fetchedSkill.IsActive = skillsVM.IsActive;
                _context.SaveChanges();
                return new ObjectResult(fetchedSkill);
            }
            return Ok();
        }


        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var fetchedSkill = _context.Skills.Where(s => s.Id == id).FirstOrDefault();
            if(fetchedSkill == null)
            {
                return NotFound();
            }
            _context.Skills.Remove(fetchedSkill);
            _context.SaveChanges();
            return Ok();
        }
    }
}

