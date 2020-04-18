using Microsoft.EntityFrameworkCore;
using RecruitmentSystemAPI.Models;
using RecruitmentSystemAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.Repositories
{
    public class CompanyRepo
    {
        private readonly RecruitmentSystemContext _context;
        public CompanyRepo(RecruitmentSystemContext context)
        {
            _context = context;
        }

        public int? GetUserCompanyId(string userId)
        {
            return _context.CompanyUsers.Where(cu => cu.UserId == userId).FirstOrDefault()?.CompanyId;
        }

        public IQueryable<CompanyVM> GetCompanies()
        {
            return _context.Companies.Select(c => new CompanyVM
            {
                Id = c.Id,
                Name = c.Name,
                City = c.City,
                Province = c.Province,
                Country = c.Country,
                Address = c.Address,
                IsActive = c.IsActive,
                Phone = c.Phone
            });
        }

        public CompanyVM GetCompanyById(int id)
        {
            return _context.Companies.Where(c => c.Id == id).Select(c => new CompanyVM
            {
                Id = c.Id,
                Name = c.Name,
                City = c.City,
                Province = c.Province,
                Country = c.Country,
                Address = c.Address,
                IsActive = c.IsActive,
                Phone = c.Phone
            }).FirstOrDefault();
        }

        public void UpdateCompany(CompanyVM companyVM)
        {
            var company = _context.Companies.FirstOrDefault(c => c.Id == companyVM.Id);
            if (company == null) throw new KeyNotFoundException();

            company.Name = companyVM.Name;
            company.City = companyVM.City;
            company.Province = companyVM.Province;
            company.Country = companyVM.Country;
            company.Address = companyVM.Address;
            company.Phone = companyVM.Phone;
            company.IsActive = companyVM.IsActive;

            _context.Update(company);
            _context.SaveChanges();
        }

        public bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.Id == id);
        }

        public CompanyVM AddCompany(CompanyVM companyVM, string userId)
        {
            var company = new Company
            {
                Name = companyVM.Name,
                City = companyVM.City,
                Province = companyVM.Province,
                Country = companyVM.Country,
                Address = companyVM.Address,
                IsActive = true,
                Phone = companyVM.Phone
            };
            _context.Companies.Add(company);
            _context.CompanyUsers.Add(new CompanyUser
            {
                Company = company,
                UserId = userId
            });
            _context.SaveChanges();
            companyVM.Id = company.Id;
            return companyVM;
        }
    }
}
