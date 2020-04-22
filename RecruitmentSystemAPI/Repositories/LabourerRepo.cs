using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecruitmentSystemAPI.Models;
using RecruitmentSystemAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.Repositories
{
    public class LabourerRepo
    {
        RecruitmentSystemContext _context;
        private readonly UserManager<SystemUser> _userManager;

        public LabourerRepo(RecruitmentSystemContext context, UserManager<SystemUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IQueryable<LabourerVM> GetLabourers()
        {
            return _context.Labourers.Include(l=>l.User).Select(l => new LabourerVM
            {
                Id = l.Id,
                FirstName = l.FirstName,
                LastName = l.LastName,
                PersonalId = l.PersonalId,
                Email = l.User.Email,
                City = l.City,
                Province = l.Province,
                Country = l.Country,
                Address = l.Address,
                Phone = l.Phone,
                IsActive = l.IsActive,
                Sunday = l.Availability.HasFlag(Weekdays.Sunday),
                Monday = l.Availability.HasFlag(Weekdays.Monday),
                Tuesday = l.Availability.HasFlag(Weekdays.Tuesday),
                Wednesday = l.Availability.HasFlag(Weekdays.Wednesday),
                Thursday = l.Availability.HasFlag(Weekdays.Thursday),
                Friday = l.Availability.HasFlag(Weekdays.Friday),
                Saturday = l.Availability.HasFlag(Weekdays.Saturday),
                Skills = _context.LabourerSkills.Where(ls => ls.LabourerId == l.Id)
                                                .Select(ls => new SkillsVM
                                                {
                                                    Id = ls.Id,
                                                    Name = ls.Skill.Name,
                                                    ChargeAmount = ls.Skill.ChargeAmount,
                                                    PayAmount = ls.Skill.PayAmount,
                                                    IsActive = ls.IsActive
                                                }).ToList(),
                SafetyRating = l.SafetyRating,
                QualityRating = l.QualityRating,
            });
        }

        public LabourerVM GetLabourerById(int id)
        {
            return _context.Labourers.Where(l => l.Id == id).Include(l=>l.User).Select(l => new LabourerVM
            {
                Id = l.Id,
                FirstName = l.FirstName,
                LastName = l.LastName,
                PersonalId = l.PersonalId,
                Email = l.User.Email,
                City = l.City,
                Province = l.Province,
                Country = l.Country,
                Address = l.Address,
                Phone = l.Phone,
                IsActive = l.IsActive,
                Sunday = l.Availability.HasFlag(Weekdays.Sunday),
                Monday = l.Availability.HasFlag(Weekdays.Monday),
                Tuesday = l.Availability.HasFlag(Weekdays.Tuesday),
                Wednesday = l.Availability.HasFlag(Weekdays.Wednesday),
                Thursday = l.Availability.HasFlag(Weekdays.Thursday),
                Friday = l.Availability.HasFlag(Weekdays.Friday),
                Saturday = l.Availability.HasFlag(Weekdays.Saturday),
                SafetyRating = l.SafetyRating,
                QualityRating = l.QualityRating,
                Skills = _context.LabourerSkills.Where(ls => ls.LabourerId == l.Id)
                                                .Select(ls => new SkillsVM {
                                                    Id              = ls.Id,
                                                    Name            = ls.Skill.Name,
                                                    ChargeAmount    = ls.Skill.ChargeAmount,
                                                    PayAmount       = ls.Skill.PayAmount,
                                                    IsActive        = ls.IsActive
                                                }).ToList()
            }).FirstOrDefault();
        }



        public async Task UpdateLabourer(LabourerVM labourerVM)
        {
            var labourer = _context.Labourers.Include(l=>l.LabourerSkills).FirstOrDefault(l => l.Id == labourerVM.Id);
            if (labourer == null) throw new KeyNotFoundException();
            labourer.FirstName = labourerVM.FirstName;
            labourer.LastName = labourerVM.LastName;
            labourer.PersonalId = labourerVM.PersonalId;
            //labourer.Email = labourerVM.Email;
            labourer.City = labourerVM.City;
            labourer.Province = labourerVM.Province;
            labourer.Country = labourerVM.Country;
            labourer.Address = labourerVM.Address;
            labourer.Phone = labourerVM.Phone;
            labourer.IsActive = labourerVM.IsActive;
            labourer.Availability = ConvertWeekdaysToEnum(labourerVM);


            var existingSkills = labourerVM.Skills.Where(s  => labourer.LabourerSkills.Any(ls  =>  ls.SkillId == s.Id));
            foreach(var skill in existingSkills)
            {
                var oldSkill = labourer.LabourerSkills.FirstOrDefault(s  =>  s.SkillId == skill.Id.Value);
                oldSkill.IsActive = skill.IsActive;
                _context.Update(oldSkill);
            }


            var newSkills = labourerVM.Skills.Where(s => !labourer.LabourerSkills.Any(ls => ls.SkillId == s.Id));
            foreach (var skill in newSkills)
            {
                var newSkill = new LabourerSkill
                {
                    LabourerId = labourer.Id,
                    IsActive = skill.IsActive,
                    SkillId = skill.Id.Value
                };
                _context.Add(newSkill);
            }


            await UpdateUserEmail(labourer.UserId, labourerVM.Email);

            _context.Update(labourer);
            _context.SaveChanges();
        }


        public async Task<LabourerVM> AddLabourer(LabourerVM labourerVM, string userId)
        {
            var labourer = new Labourer
            {
                UserId = userId,
                FirstName = labourerVM.FirstName,
                LastName = labourerVM.LastName,
                PersonalId = labourerVM.PersonalId,
                City = labourerVM.City,
                Province = labourerVM.Province,
                Country = labourerVM.Country,
                Address = labourerVM.Address,
                Phone = labourerVM.Phone,
                IsActive = labourerVM.IsActive,
                Availability = ConvertWeekdaysToEnum(labourerVM)
                
            };

            _context.Add(labourer);
            var labourerSkills = new List<LabourerSkill>();
            foreach(var skill in labourerVM.Skills)
            {
                var newSkill = new LabourerSkill
                {
                    IsActive = skill.IsActive,
                    Labourer = labourer,
                    SkillId = skill.Id.Value
                };
                labourerSkills.Add(newSkill);
                _context.LabourerSkills.Add(newSkill);
            }
            labourer.LabourerSkills = labourerSkills;
            await UpdateUserEmail(userId, labourerVM.Email);
            _context.SaveChanges();
            labourerVM.Id = labourer.Id;
            return labourerVM;
        }

        private async Task UpdateUserEmail(string userId, string email)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user.Email != email)
            {
                if (_context.Users.Any(u => u.Email.ToLower() == email.ToLower()))
                {
                    throw new Exception($"Email {email} is already taken");
                }
                user.Email = email;
                user.UserName = email;
            }
            await _userManager.UpdateAsync(user);
        }

        public int? GetUserLabourerId(string userId)
        {
            return _context.Labourers.Where(l => l.UserId == userId).FirstOrDefault()?.Id;
        }

        private Weekdays ConvertWeekdaysToEnum(LabourerVM labourerVM)
        {
            Weekdays weekdays = 0;
            if (labourerVM.Sunday)
                weekdays |= Weekdays.Sunday;
            if (labourerVM.Monday)
                weekdays |= Weekdays.Monday;
            if (labourerVM.Tuesday)
                weekdays |= Weekdays.Tuesday;
            if (labourerVM.Wednesday)
                weekdays |= Weekdays.Wednesday;
            if (labourerVM.Thursday)
                weekdays |= Weekdays.Thursday;
            if (labourerVM.Friday)
                weekdays |= Weekdays.Friday;
            if (labourerVM.Saturday)
                weekdays |= Weekdays.Saturday;
            return weekdays;
        }

        
    }
}