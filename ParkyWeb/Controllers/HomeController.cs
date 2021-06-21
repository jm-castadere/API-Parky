using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ParkyWeb.Models;
using ParkyWeb.Models.ViewModel;
using ParkyWeb.Repository.IRepository;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ParkyWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
       
        private readonly INationalParkRepository _parkRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly ITrailRepository _trailRepo;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="parkRepo">park</param>
        /// <param name="trailRepo">trail</param>
        /// <param name="accountRepo">account</param>
        public HomeController(ILogger<HomeController> logger, 
                                INationalParkRepository parkRepo,
                                ITrailRepository trailRepo, 
                                IAccountRepository accountRepo)
        {
            _parkRepo = parkRepo;
            _trailRepo = trailRepo;
            _logger = logger;
            _accountRepo = accountRepo;
        }

        /// <summary>
        /// View Home
        /// </summary>
        /// <returns></returns>

        public async Task<IActionResult> Index()
        {
            IndexVM listOfParksAndTrails = new IndexVM()
            {
                //get all park value
                NationalParkList = await _parkRepo.GetAllAsync(AppVariables.NationalParkAPIPath,HttpContext.Session.GetString("JWToken")),
              
                //get all trail
                TrailList = await _trailRepo.GetAllAsync(AppVariables.TrailAPIPath, HttpContext.Session.GetString("JWToken")),
            };

            return View(listOfParksAndTrails);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
        

        [HttpGet]
        public IActionResult Login()
        {
            User obj = new User();
            return View(obj);
        }

        /// <summary>
        /// Login for user
        /// </summary>
        /// <param name="obj">user parameter</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async  Task<IActionResult> Login(User obj)
        {
            //Call api to log user (url+authenticate with user parameter
            User objUser = await _accountRepo.LoginAsync(AppVariables.AccountAPIPath + "authenticate/", obj);
            //Check if User token created
            if (objUser.Token == null)
            {
                return View();
            }

            //Create identity prameter
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, objUser.Username));
            identity.AddClaim(new Claim(ClaimTypes.Role, objUser.Role));
           
            var principal = new ClaimsPrincipal(identity);
            
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            //Sauve in session token and user
            HttpContext.Session.SetString(AppVariables.JWTokenSession, objUser.Token);
            TempData[AppVariables.TempDataAlert] = "Welcome " + objUser.Username;
            
            //Redirect to Action of contoller
            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Create new user log
        /// </summary>
        /// <param name="obj">user parameter</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User obj)
        {
            bool result = await _accountRepo.RegisterAsync(AppVariables.AccountAPIPath + "register/", obj);
            if (result == false)
            {
                return View();
            }
            TempData[AppVariables.TempDataAlert] = "Registeration Successful";

            //Redirect to Action of contoller
            //Get to login view
            return RedirectToAction("Login");
        }


        /// <summary>
        /// Logout
        /// </summary>
        /// <returns></returns>
        public async  Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            //Clean session token value
            HttpContext.Session.SetString(AppVariables.JWTokenSession, "");

            //Redirect to Action of contoller
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
