using Microsoft.EntityFrameworkCore;
using RecruitmentSystemAPI.Models;
using RecruitmentSystemAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.Repositories
{
    public class CompanyRepo : BaseRepo
    {
        public CompanyRepo(RecruitmentSystemContext context) : base(context)
        {
        }

        public (int?, string) GetUserCompanyId(string userId)
        {
            var user = _context.CompanyUsers.Where(cu => cu.UserId == userId).Include(c => c.Company).FirstOrDefault();
            return (user?.CompanyId, user?.Company?.Name);
        }

        public IQueryable<CompanyVM> GetCompanies(int count, int page, out int totalRows, bool? orderByTopRated)
        {
            var query = _context.Companies.Select(c => new CompanyVM
            {
                Id = c.Id,
                Name = c.Name,
                City = c.City,
                Province = c.Province,
                Country = c.Country,
                Address = c.Address,
                IsActive = c.IsActive,
                Phone = c.Phone,
                Email = c.Email,
                Rating = c.Rating
            }).AsQueryable();

            totalRows = query.Count();
            if (orderByTopRated.HasValue && orderByTopRated.Value)
            {
                query = query.OrderByDescending(c => c.Rating);
            }
            else
            {
                query = query.OrderByDescending(c => c.Id);
            }
            return query.Skip(count * (page - 1)).Take(count);

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
                Phone = c.Phone,
                Email = c.Email,
                Rating = c.Rating
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
            company.Email = companyVM.Email;

            _context.Update(company);
            _context.SaveChanges();
        }

        public Dictionary<int, string> GetCompaniesDDL()
        {
            return _context.Companies.ToDictionary(c => c.Id, c => c.Name);
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
                Phone = companyVM.Phone,
                Email = companyVM.Email
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
