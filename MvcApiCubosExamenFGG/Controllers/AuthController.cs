using ApiCubosExamenFGG.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MvcApiCubosExamenFGG.Services;
using System.Security.Claims;

namespace MvcApiCubosExamenFGG.Controllers
{
    public class AuthController : Controller
    {
        private ServiceApiCubos service;

        public AuthController(ServiceApiCubos service)
        {
            this.service = service;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string pass)
        {
            string token = await this.service.GetTokenAsync(email, pass);
            if (token == null)
            {
                ViewData["MENSAJE"] = "Usuario/Password incorrectos";
                return View();
            }
            else
            {
                HttpContext.Session.SetString("TOKEN", token);
                UsuarioCubo user = await this.service.PerfilUsuarioAPI(token);

                ClaimsIdentity identity =new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme,ClaimTypes.Name, ClaimTypes.Role);

                identity.AddClaim(new Claim(ClaimTypes.Name, email));
                identity.AddClaim(new Claim("NOMBRE", user.Nombre));

                ClaimsPrincipal userPrincipal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync (CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, new AuthenticationProperties
                {
                   ExpiresUtc = DateTime.UtcNow.AddMinutes(120)
                });

                return RedirectToAction("VistaCubos", "Cubos");
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync
                (CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("TOKEN");
            return RedirectToAction("VistaCubos", "Cubos");
        }
    }
}
