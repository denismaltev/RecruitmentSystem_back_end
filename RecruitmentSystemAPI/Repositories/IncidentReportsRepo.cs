using Microsoft.EntityFrameworkCore;
using RecruitmentSystemAPI.Models;
using RecruitmentSystemAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.Repositories
{
    public class IncidentReportsRepo : BaseRepo
    {
        public IncidentReportsRepo(RecruitmentSystemContext context) : base(context)
        {

        }

        public (int, IEnumerable<IncidentReportVM>) GetIncidentReports(string userId, bool isAdmin, int? companyId, int count, int page, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.IncidentReports.Include(r => r.Job).ThenInclude(j => j.Company).AsQueryable();
            if (!isAdmin)
            {
                companyId = _context.CompanyUsers.FirstOrDefault(cu => cu.UserId == userId).CompanyId;
            }
            query = query.Where(r => (!companyId.HasValue || r.Job.CompanyId == companyId.Value) && (!fromDate.HasValue || r.Date.Date >= fromDate.Value.Date) && (!toDate.HasValue || r.Date.Date <= toDate.Value.Date));
            var totalRows = query.Count();
            var result = query.OrderByDescending(r => r.Date).Skip(count * (page - 1)).Take(count).Select(r => new IncidentReportVM
            {
                Id = r.Id,
                CompanyName = r.Job.Company.Name,
                JobTitle = r.Job.Title,
                Date = r.Date
            });
            return (totalRows, result);
        }

        public IncidentReportVM GetIncidentReportDetails(int id, string userId, bool isAdmin)
        {
            var query = _context.IncidentReports.Where(r => r.Id == id).Include(r => r.LabourerIncidentReports).Include(r => r.Job).ThenInclude(j => j.Company).AsQueryable();
            if (!isAdmin)
            {
                var companyId = _context.CompanyUsers.FirstOrDefault(cu => cu.UserId == userId).CompanyId;
                query = query.Where(r => r.Job.CompanyId == companyId);
            }
            var report = query.FirstOrDefault();
            var result = new IncidentReportVM
            {
                Id = report.Id,
                Date = report.Date,
                Summary = report.Summary,
                JobId = report.JobId,
                JobTitle = report.Job.Title,
                CompanyName = report.Job.Company.Name,
                Labourers = _context.Labourers.Where(l => report.LabourerIncidentReports.Any(lr => lr.LabourerId == l.Id)).Select(l => new IncidentReportLabourerVM { LabourerId = l.Id, LabourerFullName = $"{l.FirstName} {l.LastName}" }).ToList()

            };
            if (isAdmin)
            {
                report.ReviewedByAdmin = true;
                _context.Update(report);
                _context.SaveChanges();
            }
            return result;
        }

        public IncidentReport UpdateIncidentReport(IncidentReportVM incidentReportVM, string userId, bool isAdmin)
        {
            var query = _context.IncidentReports.Include(r => r.LabourerIncidentReports).Where(r => r.Id == incidentReportVM.Id);
            if (!isAdmin)
            {
                var companyId = _context.CompanyUsers.FirstOrDefault(cu => cu.UserId == userId).CompanyId;
                query = query.Include(r => r.Job).Where(r => r.Job.CompanyId == companyId);
            }
            var report = query.FirstOrDefault();
            if (report == null)
            {
                return null;
            }
            report.UpdateDate = DateTime.Now;
            report.Date = incidentReportVM.Date;
            report.JobId = incidentReportVM.JobId;
            if (isAdmin)
            {
                report.ReviewedByAdmin = true;
            }
            report.Summary = incidentReportVM.Summary;
            var newLabourers = incidentReportVM.Labourers?.Where(l => report.LabourerIncidentReports?.Any(r => r.LabourerId == l.LabourerId) == false).ToList();
            var labourersToRemove = report.LabourerIncidentReports?.Where(r => incidentReportVM.Labourers?.Any(l => l.LabourerId == r.LabourerId) == true).ToList();
            if(newLabourers != null && newLabourers.Count > 0)
            {
                var labourerIncidentReports = newLabourers.Select(l => new LabourerIncidentReport
                {
                    IncidentReportId = report.Id,
                    LabourerId = l.LabourerId
                });
                _context.LabourerIncidentReports.AddRange(labourerIncidentReports);
            }
            if(labourersToRemove != null && labourersToRemove.Count > 0)
            {
                _context.LabourerIncidentReports.RemoveRange(labourersToRemove);
            }
            _context.Update(report);
            _context.SaveChanges();
            return report;
        }

        public IncidentReportVM AddReport(IncidentReportVM incidentReportVM, string userId)
        {
            var companyId = _context.CompanyUsers.FirstOrDefault(cu => cu.UserId == userId).CompanyId;
            if(_context.Jobs.Any(j=>j.Id == incidentReportVM.JobId && j.CompanyId == companyId))
            {
                var report = new IncidentReport
                {
                    CreateDate = DateTime.Now,
                    Date = incidentReportVM.Date,
                    JobId = incidentReportVM.JobId,
                    ReviewedByAdmin = false,
                    Summary = incidentReportVM.Summary
                };
                _context.IncidentReports.Add(report);
                if(incidentReportVM.Labourers != null && incidentReportVM.Labourers.Count > 0)
                {
                    var labourerIncidentReports = incidentReportVM.Labourers.Select(l => new LabourerIncidentReport
                    {
                        IncidentReport = report,
                        LabourerId = l.LabourerId
                    });
                    _context.LabourerIncidentReports.AddRange(labourerIncidentReports);
                }
                _context.SaveChanges();
                incidentReportVM.Id = report.Id;
                return incidentReportVM;
            }
            return null;
        }
    }
}
