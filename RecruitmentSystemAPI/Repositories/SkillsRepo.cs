using RecruitmentSystemAPI.Models;
using RecruitmentSystemAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.Repositories
{
    public class SkillsRepo
    {
        RecruitmentSystemContext _context;
        public SkillsRepo(RecruitmentSystemContext context)
        {
            _context = context;
        }

        // GET all skills list
        public IQueryable<SkillsVM> GetSkills()
        {
            return _context.Skills.Select(s => new SkillsVM
            {
                Id = s.Id,
                Name = s.Name,
                ChargeAmount = s.ChargeAmount,
                PayAmount = s.PayAmount,
                IsActive = s.IsActive
            });
        }

        // GET one skill by id
        public SkillsVM GetSkillById(int id)
        {
            return _context.Skills.Where(s => s.Id == id).Select(s => new SkillsVM
            {
                Id = s.Id,
                Name = s.Name,
                ChargeAmount = s.ChargeAmount,
                PayAmount = s.PayAmount,
                IsActive = s.IsActive
            }).FirstOrDefault();
        }

        public void UpdateSkill(SkillsVM skillVM)
        {
            var skill = _context.Skills.FirstOrDefault(s => s.Id == skillVM.Id);
            if (skill == null) throw new KeyNotFoundException();

            skill.Name = skillVM.Name;
            skill.ChargeAmount = skillVM.ChargeAmount;
            skill.PayAmount = skillVM.PayAmount;
            skill.IsActive = skillVM.IsActive;

            _context.Update(skill);
            _context.SaveChanges();
        }

        public SkillsVM AddSkill(SkillsVM skillsVM)
        {
            var skill = new Skill
            {
                Name = skillsVM.Name,
                ChargeAmount = skillsVM.ChargeAmount,
                PayAmount = skillsVM.PayAmount,
                IsActive = skillsVM.IsActive
            };
            _context.Skills.Add(skill);
            _context.SaveChanges();
            return skillsVM;
        }

        public bool SkillAlreadyExists(string name)
        {
            return _context.Skills.Any(s => s.Name == name);
        }

        public bool SkillExists(int id)
        {
            return _context.Skills.Any(s => s.Id == id);
        }
    }
}
