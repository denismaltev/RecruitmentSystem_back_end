using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitmentSystemAPI.Models;

namespace RecruitmentSystemAPI.Controllers
{
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

        // POST: api/AdminSkills
        [HttpPost]
        public IActionResult Create([FromBody] Skill newSkill)
        {
            if ((newSkill.Name == null) ||
                (newSkill.Name == " ") ||
                (newSkill.ChargeAmount <= 0) ||
                (newSkill.PayAmount <= 0))
            {
                return BadRequest();
            }
            _context.Skills.Add(newSkill);
            _context.SaveChanges();
            return new ObjectResult(newSkill);
        }

        // PUT: api/AdminSkills/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
