using Data_Access_Layer.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.UserDbInitializer
{
    public class UserDbInitializer : IUserDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public UserDbInitializer(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }


        public void Initialize()
        {
            // lägg till migrations om det inte finns
            try
            {
                if (_context.Database.GetPendingMigrations().Count() > 0)
                {
                    _context.Database.Migrate(); // behöver inte köra update-database
                }
            }
            catch (Exception)
            {

                throw;
            }

            // skapa roles om de inte finns
            // Skapar upp  roller om inte Admin rollen finns
            if (!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole("Admin")).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole("Customer")).GetAwaiter().GetResult();


                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@sw.com",
                    Email = "admin@sw.com",
                    Name = "Ismail Mohamed",
                    StreetAdress = "Tjalmargatan",
                    PostTown = "Nacka",
                    City = "Stockholm"
                }, "#Sommar22").GetAwaiter().GetResult();

                ApplicationUser user = _context.ApplicationUsers.FirstOrDefault(x => x.Email == "admin@sw.com");
                _userManager.AddToRoleAsync(user, "Admin").GetAwaiter().GetResult();
            }

            return;
            
        }
    }
}
