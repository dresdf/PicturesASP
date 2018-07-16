using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PicturesASP.Models;

namespace PicturesASP.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration conf;

        public LoginController(IConfiguration Configuration)
        {
            this.conf = Configuration;
        }
        public async Task<IActionResult> Index(string username, string password, bool rememberMe)
        {

            //TODO: search DB and find user with same username
            User user = new User { Password = password };
            
            if (user != null && VerifyHashedPassword(password, user))
            {
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, conf["user:username"]));

                var principle = new ClaimsPrincipal(identity);
                var properties = new AuthenticationProperties { IsPersistent = rememberMe };
                await HttpContext.SignInAsync(principle, properties);

                return RedirectToAction("Index", "Home");
            }
            return View();
        }




        private bool VerifyHashedPassword(string password, User user)
        {
            byte[] saltBytes = Encoding.UTF8.GetBytes(conf["user:salt"]);

            byte[] hashBytes = KeyDerivation.Pbkdf2(
                password: password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 1000,
                numBytesRequested: 256 / 8
            );

            string hashText = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
            return hashText == user.Password;
        }
    }
}