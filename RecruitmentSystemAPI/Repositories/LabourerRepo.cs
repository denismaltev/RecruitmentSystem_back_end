﻿using Microsoft.AspNetCore.Identity;
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
                IsActive = l.IsActive
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
                SafetyRating = l.SafetyRating,
                QualityRating = l.QualityRating,

            }).FirstOrDefault();
        }


        public async Task UpdateLabourer(LabourerVM labourerVM)
        {
            var labourer = _context.Labourers.FirstOrDefault(l => l.Id == labourerVM.Id);
            if (labourer == null) throw new KeyNotFoundException();
            labourer.FirstName = labourerVM.FirstName;
            labourer.LastName = labourerVM.LastName;
            labourer.PersonalId = labourerVM.PersonalId;
            labourer.City = labourerVM.City;
            labourer.Province = labourerVM.Province;
            labourer.Country = labourerVM.Country;
            labourer.Address = labourerVM.Address;
            labourer.Phone = labourerVM.Phone;
            labourer.IsActive = labourerVM.IsActive;

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
                SafetyRating = 0,
                QualityRating = 0,
            };
            await UpdateUserEmail(userId, labourerVM.Email);
            _context.Add(labourer);
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
    }
}