using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Controllers
{
    [Authorize]
    [Route("api/v{version:apiVersion}/Users")]
    //[Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        public UsersController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        /// <summary>
        /// Login user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] AuthenticationModel model)
        {
            var user = _userRepo.Authenticate(model.Username, model.Password);
            if (user == null)
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }
            return Ok(user);
        }

        /// <summary>
        /// Register new user
        /// </summary>
        /// <param name="model">value form view</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] AuthenticationModel model)
        {
            //Check if user exist
            bool ifUserNameUnique = _userRepo.IsUniqueUser(model.Username);
            if (!ifUserNameUnique)
            {
                return BadRequest(new { message = "Username already exists" });
            }

            //add new user
            var user = _userRepo.Register(model.Username, model.Password);
            if (user == null)
            {
                return BadRequest(new { message = "Error while registering" });
            }

            return Ok();
        }
    }
}