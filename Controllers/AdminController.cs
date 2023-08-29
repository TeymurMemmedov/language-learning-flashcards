using LearningCards.Context;
using LearningCards.Entities;
using LearningCards.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace LearningCards.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _databaseContext;

        private readonly IConfiguration _configuration;
        public AdminController(AppDbContext databaseContext, IConfiguration configuration)
        {
            _databaseContext = databaseContext;
            _configuration = configuration;
        }
        public IActionResult Index()
        {


            List<User> users = new List<User>();
            users = _databaseContext.Users.ToList();

            List<UserViewModel> usersViewModels = new List<UserViewModel>();
            foreach (var user in users)
            {
                usersViewModels.Add(new() { Id = user.Id, Username = user.Username, Email = user.Email });
            }
            return View(usersViewModels);


        }
    }
}
