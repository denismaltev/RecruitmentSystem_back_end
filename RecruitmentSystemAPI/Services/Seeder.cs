using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RecruitmentSystemAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.Services
{
    public static class Seeder
    {
        public static async void Initialize(RecruitmentSystemContext context, UserManager<SystemUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!context.Users.Any())
            {
                string[] roles = new string[] { "Admin", "Company", "Labourer" };
                foreach (string role in roles)
                {
                    if (!context.Roles.Any(r => r.Name == role))
                    {
                        var identityRole = new IdentityRole<Guid>
                        {
                            Name = role,
                            NormalizedName = role.ToUpper()
                        };
                        var result = await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
                //admin
                await CreateUserWithRole(context, userManager, new SystemUser { Email = "admin@admin.com", UserName = "admin@admin.com" }, roles[0]);

                CreateSkills(context);

                //labourers
                await CreateLabourers(context, userManager, roles[2]);

                await CreateCompanies(context, userManager, roles[1]);

                await context.SaveChangesAsync();
            }
        }

        private static async Task CreateCompanies(RecruitmentSystemContext context, UserManager<SystemUser> userManager, string role)
        {
            await CreateCompany("Accurate Electric", "778-123-2222", "info@accurateelectic.com", "550 Kingsway", "Vancouver", context, userManager, role);

            await CreateCompany("Speedy Plumbing", "778-888-0900", "main@speedyplumbing.com", "743 Main Street", "Vancouver", context, userManager, role);

            await CreateCompany("Anders Glass", "778-888-2233", "info@andersglass.com", "862 William Street", "Vancouver", context, userManager, role);

            await CreateCompany("Color Your World", "778-999-3232", "color@coloryourworld.com", "255 Fraser Street", "Vancouver", context, userManager, role);

            await CreateCompany("Fantastic Flooring", "778-888-5454", "frank@fantasticflooring.com", "890 Sunset Street", "Burnaby", context, userManager, role);

            await CreateCompany("Ovation Drywall", "604-433-2222", "hello@ovationdrywall.com", "350 Elm Street", "Vancouver", context, userManager, role);

            await CreateCompany("ABC Welding", "604-555-2323", "abc@abcwelding.com", "750 Alexander Street", "Vancouver", context, userManager, role);

            await CreateCompany("Killer Carpentry", "604-250-1222", "kyle@killercarpentry.com", "5550 Harper Street", "Maple Ridge", context, userManager, role);

            await CreateCompany("Global Glaziers", "250-222-5555", "info@globalglaziers.com", "8832 Oak Street", "Langley", context, userManager, role);

            await CreateCompany("Swift Builders", "778-998-4455", "hello@swiftbuilders.com", "4356 Poplar Streety", "North Vancouver", context, userManager, role);
        }
        private static async Task CreateCompany(string name, string phone, string email, string address, string city,  RecruitmentSystemContext context, UserManager<SystemUser> userManager, string role)
        {
            var user = await CreateUserWithRole(context, userManager, new SystemUser { Email = email, UserName = email }, role);
            var company = new Company
            {
                Name = name,
                Phone = phone,
                Email = email,
                Address = address,
                City = city,
                Province = "BC",
                Country = "Canada",
                IsActive = true
            };
            context.Companies.Add(company);
            var companyUser = new CompanyUser
            {
                Company = company,
                User = user
            };
            context.CompanyUsers.Add(companyUser);
            context.SaveChanges();

            if (name == "Swift Builders")
            {
                CreateJob("Telus Building Refurbish", "The iconic Telus boot building requires a fresh coat of paint and some new drywall in their meeting rooms", "1487 27th Ave E", "Vancouver", Weekdays.Monday | Weekdays.Tuesday | Weekdays.Wednesday | Weekdays.Thursday | Weekdays.Friday | Weekdays.Saturday | Weekdays.Sunday, context, company);

                CreateJob("Skylofts on Grosvenor", "The Skylofts at Granville Square are a new project and we are looking for some electricians for the electrical portion", "1949 Comox St 305", "Vancouver", Weekdays.Monday | Weekdays.Tuesday | Weekdays.Wednesday | Weekdays.Thursday | Weekdays.Friday, context, company);
                
                CreateJob("BC Place Ceiling Repair", "The BC Place ceiling needs repairing between the joints that connect the panels", "5980 Battison St", "Vancouver", Weekdays.Monday | Weekdays.Tuesday | Weekdays.Wednesday | Weekdays.Thursday, context, company);
                
                CreateJob("Granville Gardens", "Granville Gardens is preparing for the summer season and needs a few general repairs", "5 12th Ave W", "Vancouver",  Weekdays.Friday | Weekdays.Saturday | Weekdays.Sunday, context, company);

                CreateJob("Collaboration Corner Offices", "Collaboration Corner has new cubicles and offices being installed requiring various labour", "1410 Tolmie St", "Vancouver", Weekdays.Monday | Weekdays.Tuesday | Weekdays.Wednesday | Weekdays.Thursday | Weekdays.Friday, context, company);

                CreateJob("Water Street Lofts", "This apartment block is getting a complete gut job requiring everything of all trades", "2 2536 7th Ave W", "Vancouver", Weekdays.Monday | Weekdays.Tuesday | Weekdays.Wednesday | Weekdays.Thursday | Weekdays.Friday, context, company);

                CreateJob("Gastown micro apartments", "These brand new micro apartments need new drywall installed and full electrical", "836 30th Ave E", "Vancouver", Weekdays.Monday | Weekdays.Tuesday | Weekdays.Wednesday | Weekdays.Thursday | Weekdays.Friday, context, company);

                CreateJob("Archie's Arcade Renovation", "Archie's Arcade is planning to reopen summer 2021 and requires new upgrades to satisfy building permits", "2439 7th Ave W", "Vancouver", Weekdays.Monday | Weekdays.Tuesday | Weekdays.Wednesday | Weekdays.Thursday | Weekdays.Friday, context, company);

                CreateJob("Metrotown Mall Office Units", "Metrotown Mall's office units 220 - 1650 need repairs following water damage from a leak. New roof, drywall, insulation.", "303 621 57th Ave W", "Vancouver", Weekdays.Monday | Weekdays.Tuesday | Weekdays.Wednesday | Weekdays.Thursday, context, company);

                CreateJob("Christmas Market Expansion", "The iconic Vancouver Christmas Market is expanding to include an extra 3000 square feet for 2021 requiring a variety of trades.", "2425 7th Ave W", "Vancouver", Weekdays.Monday | Weekdays.Tuesday | Weekdays.Sunday, context, company);

                CreateJob("The Rialto on Burrard", "The Rialto on Burrard needs new offices created on their top floor", "105 1335 27th St E", "North Vancouver", Weekdays.Monday | Weekdays.Tuesday | Weekdays.Wednesday | Weekdays.Thursday | Weekdays.Friday | Weekdays.Saturday | Weekdays.Sunday, context, company);

                CreateJob("Richards on Richards bathroom reno", "Richards on Richards club needs a brand new bathroom renovation. A complete gut and rebuild.", "5779 Grousewoods Cres", "North Vancouver", Weekdays.Monday | Weekdays.Tuesday | Weekdays.Wednesday | Weekdays.Thursday | Weekdays.Friday | Weekdays.Saturday | Weekdays.Sunday, context, company);

                CreateJob("Swift Builders Head Office", "Our head office at Swift Builders needs a facelift. New tiling, and cosmetic fixes.", "4148 Virginia Cres", "North Vancouver", Weekdays.Monday | Weekdays.Tuesday | Weekdays.Wednesday | Weekdays.Thursday | Weekdays.Friday | Weekdays.Saturday | Weekdays.Sunday, context, company);

                CreateJob("Science World Theatre screen repair", "Science World's iconic omnimax is in need of new panels and repair to some panels", "304 155 5th St E", "North Vancouver", Weekdays.Monday | Weekdays.Tuesday | Weekdays.Wednesday | Weekdays.Thursday | Weekdays.Friday | Weekdays.Saturday | Weekdays.Sunday, context, company);

                CreateJob("Crystal Candy franchise build", "A new location of Crystal Candy is being opened in Kitsilano needing a complete build", "796 Kennard Ave", "North Vancouver", Weekdays.Monday | Weekdays.Tuesday | Weekdays.Wednesday | Weekdays.Thursday | Weekdays.Friday | Weekdays.Saturday | Weekdays.Sunday, context, company);

                CreateJob("Art's Terrariums and Aquariums", "Due to a fire that destroyed the back room, Art's Terrariums and Aquariums need a renovation of the affected room", "411 15th St E", "North Vancouver", Weekdays.Monday | Weekdays.Tuesday | Weekdays.Wednesday | Weekdays.Thursday | Weekdays.Friday | Weekdays.Saturday | Weekdays.Sunday, context, company);

                CreateJob("Canada Embassy repairs", "The Canada Embassy has a list of miscellaneous repairs: plumbing, ceiling repair, and electrical work", "2920 Hoskins Rd", "North Vancouver", Weekdays.Monday | Weekdays.Tuesday | Weekdays.Wednesday | Weekdays.Thursday | Weekdays.Friday, context, company);

                CreateJob("Storage At The Dock expansion", "Storage At The Dock needs 30 more units created to expand a further 30,000 square feet", "5154 Ranger Ave", "North Vancouver", Weekdays.Monday | Weekdays.Tuesday | Weekdays.Wednesday | Weekdays.Thursday | Weekdays.Friday | Weekdays.Saturday | Weekdays.Sunday, context, company);

                CreateJob("The Maibu apartments", "These apartments in New Westminster require new plumbing installed, including new pipes, toilets, and sinks.", "2872 Trillium Pl", "North Vancouver", Weekdays.Monday | Weekdays.Tuesday | Weekdays.Wednesday | Weekdays.Thursday | Weekdays.Friday , context, company);

                CreateJob("Enviro BC storage lot repairs", "Enviro BC storage lots need some repairs to the inside offices and outdoor spaces", "4537 Ramsay Rd", "North Vancouver", Weekdays.Monday | Weekdays.Tuesday | Weekdays.Wednesday | Weekdays.Thursday | Weekdays.Friday , context, company);
            }

            var avgRating = context.Jobs.Where(j => j.CompanyId == company.Id).Select(j => j.Rating).ToList().DefaultIfEmpty().Average();
            company.Rating = avgRating;
            context.Update(company);
            context.SaveChanges();
        }

        private static void CreateJob(string title, string description, string address, string city, Weekdays weekdays, RecruitmentSystemContext context, Company company)
        {
            Random r = new Random();
            var startDate = new DateTime(2019, r.Next(1, 12), 1);
            var job = new Job
            {
                Title = title,
                Description = description,
                Address = address,
                City = city,
                Province = "BC",
                Country = "Canada",
                Company = company,
                CreateDate = startDate,
                UpdateDate = startDate,
                StartDate = startDate,
                EndDate = new DateTime(2020, r.Next(6, 12), 1),
                Weekdays = weekdays,
                IsActive = true
            };
            context.Jobs.Add(job);

            //add skills
            var skills = context.Skills.ToList();
            var from = r.Next(0, 14);
            var to = r.Next(from, 14);
            var count = 1;
            for (var i = from; i <= to; i++)
            {
                if (count == 6) break;
                var jobSkill = new JobSkill
                {
                    Job = job,
                    Skill = skills[i],
                    NumberOfLabourersNeeded = r.Next(1, 10)
                };
                context.JobSkills.Add(jobSkill);
                count++;
            }
            context.SaveChanges();

            //create past schedules
            AutoSchedule.MatchLabourersByDates(job, context, job.StartDate, DateTime.Now, null);

            //rate
            var labourerJobs = context.LabourerJobs.Where(lj => lj.JobId == job.Id).Include(l => l.Labourer).ToList();
            foreach(var labourerJob in labourerJobs)
            {
                labourerJob.QualityRating = r.Next(3, 5);
                labourerJob.SafetyRating = r.Next(3, 5);
                labourerJob.JobRating = r.Next(3, 5);
                context.Update(labourerJob);
            }
            context.SaveChanges();

            //calc labourer avg ratings
            foreach (var labourer in context.Labourers.ToList())
            {
                var avgQualityRating = context.LabourerJobs.Where(lj => lj.LabourerId == labourer.Id && lj.QualityRating.HasValue).Select(lj => lj.QualityRating.Value).ToList().DefaultIfEmpty().Average();
                var avgSafetyRating = context.LabourerJobs.Where(lj => lj.LabourerId == labourer.Id && lj.SafetyRating.HasValue).Select(lj => lj.SafetyRating.Value).ToList().DefaultIfEmpty().Average();
                labourer.QualityRating = (float)avgQualityRating;
                labourer.SafetyRating = (float)avgSafetyRating;
                context.Update(labourer);
            }

            //calc job avg rating
            var avgRating = context.LabourerJobs.Where(lj => lj.JobId == job.Id && lj.QualityRating.HasValue).Select(lj => lj.JobRating.Value).ToList().DefaultIfEmpty().Average();
            job.Rating = (float)avgRating;
            context.Update(job);
            context.SaveChanges();

            //TODO: incident reports

            new AutoSchedule(null).MatchLabourersForTheNearestTwoWeeks(job, context, null);
        }


        private static void CreateSkills(RecruitmentSystemContext context)
        {
            var skill = new Skill { Name = "Boilermaker", ChargeAmount = 40, PayAmount = 35, IsActive = true };
            context.Skills.Add(skill);
            skill = new Skill { Name = "Carpenter", ChargeAmount = 27, PayAmount = 21, IsActive = true };
            context.Skills.Add(skill);
            skill = new Skill { Name = "Drywall Installer", ChargeAmount = 31, PayAmount = 26, IsActive = true };
            context.Skills.Add(skill);
            skill = new Skill { Name = "Electrician", ChargeAmount = 30, PayAmount = 24, IsActive = true };
            context.Skills.Add(skill);
            skill = new Skill { Name = "Tiler", ChargeAmount = 32, PayAmount = 27, IsActive = true };
            context.Skills.Add(skill);
            skill = new Skill { Name = "Painter", ChargeAmount = 27, PayAmount = 24, IsActive = true };
            context.Skills.Add(skill);
            skill = new Skill { Name = "Plumber", ChargeAmount = 36, PayAmount = 31, IsActive = true };
            context.Skills.Add(skill);
            skill = new Skill { Name = "General", ChargeAmount = 17, PayAmount = 14, IsActive = true };
            context.Skills.Add(skill);
            skill = new Skill { Name = "Carpet Layer", ChargeAmount = 18, PayAmount = 15, IsActive = true };
            context.Skills.Add(skill);
            skill = new Skill { Name = "Builder", ChargeAmount = 22, PayAmount = 18, IsActive = true };
            context.Skills.Add(skill);
            skill = new Skill { Name = "Glazier", ChargeAmount = 26, PayAmount = 22, IsActive = true };
            context.Skills.Add(skill);
            skill = new Skill { Name = "Mason", ChargeAmount = 37, PayAmount = 32, IsActive = true };
            context.Skills.Add(skill);
            skill = new Skill { Name = "Pipefitter", ChargeAmount = 43, PayAmount = 40, IsActive = true };
            context.Skills.Add(skill);
            skill = new Skill { Name = "Welder", ChargeAmount = 22, PayAmount = 18, IsActive = true };
            context.Skills.Add(skill);
            skill = new Skill { Name = "Insulation Installer", ChargeAmount = 27, PayAmount = 23, IsActive = true };
            context.Skills.Add(skill);

            context.SaveChanges();
        }

        private static async Task CreateLabourers(RecruitmentSystemContext context, UserManager<SystemUser> userManager, string role)
        {
            await CreateLabourer("Joe", "Carpenter", "604-986-5178", "3216 Tennyson Cres", Weekdays.Monday  | Weekdays.Wednesday | Weekdays.Thursday | Weekdays.Friday , context, userManager, role);
            
            await CreateLabourer("Evgeni", "Plushenko", "604-987-3973", "122 Walter Hardwick Ave 305", Weekdays.Monday | Weekdays.Tuesday |  Weekdays.Saturday | Weekdays.Sunday, context, userManager, role);
            
            await CreateLabourer("Frank", "Capote", "604-688-5225", "3308 Ash St",  Weekdays.Thursday | Weekdays.Friday | Weekdays.Saturday | Weekdays.Sunday, context, userManager, role);
            
            await CreateLabourer("Mark", "Balkaran", "604-731-3240", "2485 Broadway W 414", Weekdays.Monday | Weekdays.Tuesday | Weekdays.Wednesday | Weekdays.Thursday | Weekdays.Friday, context, userManager, role);
            
            await CreateLabourer("Wayne", "Anderson", "778-371-1422", "275 28th Ave E", Weekdays.Monday | Weekdays.Tuesday | Weekdays.Wednesday | Weekdays.Thursday | Weekdays.Friday, context, userManager, role);
            
            await CreateLabourer("Maria", "Shears", "604-266-2522", "106 588 45th Ave W", Weekdays.Monday | Weekdays.Tuesday | Weekdays.Wednesday | Weekdays.Thursday | Weekdays.Friday | Weekdays.Saturday | Weekdays.Sunday, context, userManager, role);
            
            await CreateLabourer("Tamara", "Mallard", "604-254-9455", "563 Union St", Weekdays.Monday | Weekdays.Tuesday | Weekdays.Wednesday | Weekdays.Thursday | Weekdays.Friday, context, userManager, role);
            
            await CreateLabourer("Chris", "Black", "604-261-1809", "3007 8th Ave W", Weekdays.Monday | Weekdays.Tuesday | Weekdays.Wednesday | Weekdays.Thursday | Weekdays.Friday | Weekdays.Saturday | Weekdays.Sunday, context, userManager, role);
            
            await CreateLabourer("Harman", "Singh", "604-736-8018", "110 2255 W 8th", Weekdays.Monday | Weekdays.Tuesday | Weekdays.Thursday | Weekdays.Friday, context, userManager, role);
            
            await CreateLabourer("Jack", "Williams", "604-681-6334", "408 2260 W 10th", Weekdays.Wednesday | Weekdays.Thursday | Weekdays.Friday | Weekdays.Saturday | Weekdays.Sunday, context, userManager, role);
        }

        private static async Task CreateLabourer(string firstName, string lastName, string phone, string address, Weekdays availability, RecruitmentSystemContext context, UserManager<SystemUser> userManager, string role)
        {
            var email = firstName + "@" + lastName + ".com";
            var user = await CreateUserWithRole(context, userManager, new SystemUser { Email = email, UserName = email }, role);
            var labourer = new Labourer
            {
                FirstName = firstName,
                LastName = lastName,
                Phone = phone,
                Address = address,
                City = "Vancouver",
                Province = "BC",
                Country = "Canada",
                IsActive = true,
                PersonalId = "123456789",
                User = user,
                Availability = availability
            };
            context.Labourers.Add(labourer);

            //add skills
            var skills = context.Skills.ToList();
            Random r = new Random();
            var from = r.Next(0, 14);
            var to = r.Next(from, 14);
            var count = 1;
            for (var i = from; i <= to; i++)
            {
                if (count == 4) break;
                var labourerSkill = new LabourerSkill
                {
                    Labourer = labourer,
                    Skill = skills[i]
                };
                context.LabourerSkills.Add(labourerSkill);
                count++;
            }
            context.SaveChanges();
        }

        private static async Task<SystemUser> CreateUserWithRole(RecruitmentSystemContext context, UserManager<SystemUser> userManager, SystemUser user, string role)
        {
            if (!context.Users.Any(u => u.UserName == user.UserName))
            {
                var result = await userManager.CreateAsync(user, "P@ssw0rd");
                if (result.Succeeded)
                {
                    var systemUser = await userManager.FindByEmailAsync(user.Email);
                    result = await userManager.AddToRoleAsync(systemUser, role);
                    return systemUser;
                }
            }
            return null;
        }
    }
}
