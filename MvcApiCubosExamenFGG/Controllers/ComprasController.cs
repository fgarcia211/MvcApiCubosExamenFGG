using Microsoft.AspNetCore.Mvc;
using MvcApiCubosExamenFGG.Filters;
using MvcApiCubosExamenFGG.Services;

namespace MvcApiCubosExamenFGG.Controllers
{
    public class ComprasController : Controller
    {
        private ServiceApiCubos service;

        public ComprasController(ServiceApiCubos service)
        {
            this.service = service;
        }

        [AuthorizeCubos]
        public async Task<IActionResult> ComprarCubo(int idcubo)
        {
            string token = HttpContext.Session.GetString("TOKEN");

            await this.service.CreatePedido(idcubo, token);
            return RedirectToAction("ComprasUsuario");
        }

        public async Task<IActionResult> ComprasUsuario()
        {
            string token = HttpContext.Session.GetString("TOKEN");

            return View(await this.service.ComprasUsuarioAPI(token));
        }
    }
}
