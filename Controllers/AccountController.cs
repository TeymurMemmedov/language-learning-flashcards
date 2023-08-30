
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using LearningCards.Context;
using LearningCards.Entities;
using LearningCards.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NETCore.Encrypt.Extensions;

[Authorize]
public class AccountController : Controller
{
    private readonly AppDbContext _databaseContext;

    private readonly IConfiguration _configuration;

    public AccountController(AppDbContext databaseContext, IConfiguration configuration)
    {
        _databaseContext = databaseContext;
        _configuration = configuration;
    }

    [NonAction]
    public string HashPassword(string password)
    {
        string value = _configuration.GetValue<string>("AppSettings:MD5Salt");
        password += value;
        return password.MD5();
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    public IActionResult Register(RegisterViewModel model)
    {
        RegisterViewModel model2 = model;
        if (ModelState.IsValid)
        {
            User user = new User();
            user.Email = model2.Email;
            user.Username = model2.UserName;
            user.Password = HashPassword(model2.Password);
            user.role = _databaseContext.Roles.SingleOrDefault(x => x.Name == model2.RoleName);
            user.roleId = _databaseContext.Roles.SingleOrDefault(x => x.Name == model2.RoleName).Id;
            User entity = user;
            int num = 0;
            try
            {
                _databaseContext.Users.Add(entity);
                num = _databaseContext.SaveChanges();
                return RedirectToAction("Login");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "İstifadəçi adı, yaxud e-mail artıq mövcuddur. Başqasını daxil edin");
            }
        }
        return View(model2);
    }

    [AllowAnonymous]
    public IActionResult Login()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        LoginViewModel model2 = model;
        if (base.ModelState.IsValid)
        {
            User user = new User();
            string hashedPassword = HashPassword(model2.Password);
            if (model2.UsernameOrEmail.Contains('@'))
            {
                user = _databaseContext.Users.SingleOrDefault((User x) => x.Email == model2.UsernameOrEmail && x.Password == hashedPassword);
                
            }
            else
            {
                user = _databaseContext.Users.SingleOrDefault((User x) => x.Username == model2.UsernameOrEmail && x.Password == hashedPassword);
            }
            if (user != null)
            {
                string name = _databaseContext.Roles.SingleOrDefault((Role x) => x.Id == user.roleId).Name;
                List<Claim> claims = new List<Claim>
                {
                    new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", user.Id.ToString()),
                    new Claim("Username", user.Username),
                    new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", name)
                };
                ClaimsIdentity identity = new ClaimsIdentity(claims, "Cookies");
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                base.HttpContext.SignInAsync("Cookies", principal);
                bool ısAuthenticated = base.User.Identity.IsAuthenticated;
                return RedirectToAction("Index", "Home");
            }
            base.ModelState.AddModelError("", "İstifadəçi adı yaxud şifrə səhvdir");
        }
        return View(model2);
    }

    public IActionResult Logout()
    {
        base.HttpContext.SignOutAsync("Cookies");
        return RedirectToAction("Login");
    }

    public IActionResult Profile()
    {
        base.ViewBag.Username = base.User.FindFirst("Username").Value;
        return View("Profile");
    }

    [HttpPost]
    public IActionResult ProfileChangePassword([Required(ErrorMessage ="Şifrə boş qoyula bilməz")]
        [StringLength(30, MinimumLength =8, ErrorMessage = "Şifrə minimum 8 simvoldan ibarət olmalıdır")]string Password)
    {
        if (ModelState.IsValid)
        {
            User user = _databaseContext.Users.SingleOrDefault((User x) => x.Id.ToString() == User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"));
            user.Password = HashPassword(Password);
            _databaseContext.SaveChanges();
            ViewData["result"] = "Password Changed";
            return View("Profile");
        }

        return View("Profile");
    }

    [HttpPost]
    public IActionResult ProfileChangeUsername([Required(ErrorMessage="İstifadəçi adı boş qoyula bilməz")]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "İstifadəçi adı minimum 5, maksimum 30 simvoldan ibarət olmalıdır")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9._]*$", ErrorMessage = "Invalid username format.")] string Username)
    {
        if (ModelState.IsValid)
        {
            User user = _databaseContext.Users.SingleOrDefault(x => x.Id.ToString() == User.FindFirstValue(ClaimTypes.NameIdentifier));
            string name = _databaseContext.Roles.SingleOrDefault((Role x) => x.Id == user.roleId).Name;
            user.Username = Username;
            _databaseContext.SaveChanges();
            HttpContext.SignOutAsync("Cookies");
            List<Claim> claims = new List<Claim>
                {
                    new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", user.Id.ToString()),
                    new Claim("Username", user.Username),
                    new Claim(ClaimTypes.Role,name)
                };
            ClaimsIdentity identity = new ClaimsIdentity(claims, "Cookies");
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            HttpContext.SignInAsync("Cookies", principal);
            return RedirectToAction("Profile");


        }

        return View("Profile");
    }

    [HttpGet]
    public IActionResult CreateSet()
    {
        SetViewModel model = new SetViewModel();
        return View(model);
    }

    [HttpPost]
    public IActionResult CreateSet(SetViewModel model)
    {
        if (base.ModelState.IsValid)
        {
            if (model.TermsViewModels.Count == 0)
            {
                base.ModelState.AddModelError("", "Kartsız set yaradıla bilməz.");
                return View(model);
            }
            User user = _databaseContext.Users.SingleOrDefault((User x) => x.Id.ToString() == User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"));
            int ownerUserId = int.Parse(base.User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"));
            TermSet termSet = new TermSet
            {
                Title = model.Title,
                Description = model.Description,
                isPublic = model.isPublic,
                ownerUserId = ownerUserId
            };
            _databaseContext.TermSets.Add(termSet);
            _databaseContext.SaveChanges();
            foreach (TermViewModel termsViewModel in model.TermsViewModels)
            {
                Term entity = new Term
                {
                    Name = termsViewModel.Term,
                    Definition = termsViewModel.TermDefinition,
                    TermSetId = termSet.Id
                };
                _databaseContext.Terms.Add(entity);
            }
            _databaseContext.SaveChanges();
            return RedirectToAction("GetAllSets", "Account");
        }
        return View("CreateSet", model);
    }

    public IActionResult GetAllSets()
    {
        User user = _databaseContext.Users.SingleOrDefault((User x) => x.Id.ToString() == User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"));
        List<SetViewModel> list = new List<SetViewModel>();
        List<int> list2 = new List<int>();
        List<TermSet> list3 = _databaseContext.TermSets.Where((TermSet x) => x.ownerUserId == user.Id).ToList();
        foreach (TermSet termSet in list3)
        {
            SetViewModel setViewModel = new SetViewModel
            {
                Title = termSet.Title,
                Description = termSet.Description,
                isPublic = termSet.isPublic
            };
            list2.Add(termSet.Id);
            termSet.Terms = _databaseContext.Terms.Where((Term x) => x.TermSetId == termSet.Id).ToList();
            foreach (Term term in termSet.Terms)
            {
                setViewModel.TermsViewModels.Add(new TermViewModel
                {
                    Term = term.Name,
                    TermDefinition = term.Definition
                });
            }
            list.Add(setViewModel);
        }
        base.ViewBag.ids = list2;
        return View(list);
    }

    [AllowAnonymous]
    public IActionResult GetAllPublicSets()
    {
        List<SetViewModel> list = new List<SetViewModel>();
        List<int> list2 = new List<int>();
        List<string> list3 = new List<string>();
        List<TermSet> list4 = _databaseContext.TermSets.Where((TermSet x) => x.isPublic == true).ToList();
        foreach (TermSet termSet in list4)
        {
            SetViewModel setViewModel = new SetViewModel
            {
                Title = termSet.Title,
                Description = termSet.Description,
                isPublic = termSet.isPublic
            };
            list2.Add(termSet.Id);
            list3.Add(_databaseContext.Users.SingleOrDefault((User x) => x.Id == termSet.ownerUserId).Username);
            termSet.Terms = _databaseContext.Terms.Where((Term x) => x.TermSetId == termSet.Id).ToList();
            foreach (Term term in termSet.Terms)
            {
                setViewModel.TermsViewModels.Add(new TermViewModel
                {
                    Term = term.Name,
                    TermDefinition = term.Definition
                });
            }
            list.Add(setViewModel);
        }
        base.ViewBag.ids = list2;
        base.ViewBag.ownerNames = list3;
        return View(list);
    }

    public IActionResult DeleteSet(int id)
    {
        bool flag = false;
        TermSet termSet = _databaseContext.TermSets.SingleOrDefault((TermSet x) => x.Id == id && (x.ownerUserId.ToString() == User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier") || User.FindFirstValue("http://schemas.microsoft.com/ws/2008/06/identity/claims/role") == "Admin"));
        if (termSet == null)
        {
            return NotFound();
        }
        if (_databaseContext.TermSets == null)
        {
            return Problem("Entity set  is null.");
        }
        _databaseContext.TermSets.Remove(termSet);
        _databaseContext.SaveChanges();
        return RedirectToAction("GetAllSets");
    }

    [NonAction]
    private static void Shuffle<T>(List<T> list)
    {
        Random random = new Random();
        int num = list.Count;
        while (num > 1)
        {
            num--;
            int index = random.Next(num + 1);
            T value = list[index];
            list[index] = list[num];
            list[num] = value;
        }
    }

    [AllowAnonymous]
    public IActionResult LearnSet(int id, string orderOfCards)
    {
        bool flag = false;
        TermSet termSet = _databaseContext.TermSets.SingleOrDefault((TermSet x) => x.Id == id && (x.ownerUserId.ToString() == User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier") || User.FindFirstValue("http://schemas.microsoft.com/ws/2008/06/identity/claims/role") == "Admin" || x.isPublic == true));
        if (termSet == null)
        {
            return NotFound();
        }
        if (_databaseContext.TermSets == null)
        {
            return Problem("Entity set 'MvcMovieContext.Movie'  is null.");
        }
        SetViewModel setViewModel = new SetViewModel
        {
            Title = termSet.Title,
            Description = termSet.Description,
            isPublic = termSet.isPublic
        };
        termSet.Terms = _databaseContext.Terms.Where((Term x) => x.TermSetId == termSet.Id).ToList();
        foreach (Term term in termSet.Terms)
        {
            setViewModel.TermsViewModels.Add(new TermViewModel
            {
                Term = term.Name,
                TermDefinition = term.Definition
            });
        }
        base.ViewBag.orderOfCards = orderOfCards;
        if (orderOfCards == "shuffle")
        {
            Shuffle(setViewModel.TermsViewModels);
        }
        return View(setViewModel);
    }

    public IActionResult EditSet(int id)
    {
        bool flag = false;
        TermSet termSet = _databaseContext.TermSets.SingleOrDefault((TermSet x) => x.Id == id && (x.ownerUserId.ToString() == User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier") || User.FindFirstValue("http://schemas.microsoft.com/ws/2008/06/identity/claims/role") == "Admin" || x.isPublic == true));
        if (termSet == null)
        {
            return NotFound();
        }
        if (_databaseContext.TermSets == null)
        {
            return Problem("Entity set  is null.");
        }
        SetViewModel setViewModel = new SetViewModel
        {
            Title = termSet.Title,
            Description = termSet.Description,
            isPublic = termSet.isPublic
        };
        termSet.Terms = _databaseContext.Terms.Where((Term x) => x.TermSetId == termSet.Id).ToList();
        foreach (Term term in termSet.Terms)
        {
            setViewModel.TermsViewModels.Add(new TermViewModel
            {
                Term = term.Name,
                TermDefinition = term.Definition
            });
        }
        return View(setViewModel);
    }

    [HttpPost]
    public IActionResult EditSet(int id, SetViewModel model)
    {
        if (base.ModelState.IsValid)
        {
            TermSet termSet = _databaseContext.TermSets.SingleOrDefault((TermSet x) => x.Id == id && (x.ownerUserId.ToString() == User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier") || User.FindFirstValue("http://schemas.microsoft.com/ws/2008/06/identity/claims/role") == "Admin"));
            if (termSet != null)
            {
                _databaseContext.TermSets.Remove(termSet);
                _databaseContext.SaveChanges();
                if (model.TermsViewModels.Count == 0)
                {
                    base.ModelState.AddModelError("", "Kartsız set yaradıla bilməz.");
                    return View(model);
                }
                User user = _databaseContext.Users.SingleOrDefault((User x) => x.Id.ToString() == User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"));
                int ownerUserId = int.Parse(base.User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"));
                TermSet termSet2 = new TermSet
                {
                    Title = model.Title,
                    Description = model.Description,
                    isPublic = model.isPublic,
                    ownerUserId = ownerUserId
                };
                _databaseContext.TermSets.Add(termSet2);
                _databaseContext.SaveChanges();
                foreach (TermViewModel termsViewModel in model.TermsViewModels)
                {
                    Term entity = new Term
                    {
                        Name = termsViewModel.Term,
                        Definition = termsViewModel.TermDefinition,
                        TermSetId = termSet2.Id
                    };
                    _databaseContext.Terms.Add(entity);
                }
                _databaseContext.SaveChanges();
                _databaseContext.SaveChanges();
                return RedirectToAction("GetAllSets");
            }
            return NotFound();
        }
        return View(model);
    }

    [AllowAnonymous]
    public IActionResult SearchSet(string searchedWord)
    {
        string searchedWord2 = searchedWord;
        List<SetViewModel> list = new List<SetViewModel>();
        List<int> list2 = new List<int>();
        List<string> list3 = new List<string>();
        List<TermSet> list4 = _databaseContext.TermSets.Where((TermSet x) => (x.Title.StartsWith(searchedWord2) || x.Title.Contains(searchedWord2)) && (x.ownerUserId.ToString() == User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier") || User.FindFirstValue("http://schemas.microsoft.com/ws/2008/06/identity/claims/role") == "Admin" || x.isPublic == true)).ToList();
        foreach (TermSet termSet in list4)
        {
            SetViewModel setViewModel = new SetViewModel
            {
                Title = termSet.Title,
                Description = termSet.Description,
                isPublic = termSet.isPublic
            };
            list2.Add(termSet.Id);
            list3.Add(_databaseContext.Users.SingleOrDefault((User x) => x.Id == termSet.ownerUserId).Username);
            termSet.Terms = _databaseContext.Terms.Where((Term x) => x.TermSetId == termSet.Id).ToList();
            foreach (Term term in termSet.Terms)
            {
                setViewModel.TermsViewModels.Add(new TermViewModel
                {
                    Term = term.Name,
                    TermDefinition = term.Definition
                });
            }
            list.Add(setViewModel);
        }
        base.ViewBag.ids = list2;
        base.ViewBag.ownerNames = list3;
        return View(list);
    }
}
