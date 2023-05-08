using ApiCubosExamenFGG.Models;
using Microsoft.AspNetCore.Mvc;
using MvcApiCubosExamenFGG.Filters;
using MvcApiCubosExamenFGG.Models;
using MvcApiCubosExamenFGG.Services;

namespace MvcApiCubosExamenFGG.Controllers
{
    public class UsuariosController : Controller
    {
        private ServiceApiCubos service;
        private ServiceStorageBlobs azureservice;

        public UsuariosController(ServiceApiCubos service, ServiceStorageBlobs azureservice)
        {
            this.service = service;
            this.azureservice = azureservice;
        }

        [AuthorizeCubos]
        public async Task<IActionResult> Perfil()
        {
            string token = HttpContext.Session.GetString("TOKEN");
            UsuarioCubo user = await this.service.PerfilUsuarioAPI(token);

            List<BlobModel> archivos = await this.azureservice.GetBlobsAsync("containerusuarios");

            foreach (BlobModel b in archivos)
            {
                if (b.Nombre == user.Imagen)
                {
                    user.Imagen = b.Url;
                    break;
                }
            }

            return View(user);
        }

        public IActionResult CrearUsuario()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearUsuario(UsuarioCubo user, IFormFile imagen)
        {
            user.Imagen = imagen.FileName;
            await this.service.CreateUsuarioAPI(user);

            using (Stream stream = imagen.OpenReadStream())
            {
                await this.azureservice.UploadBlobAsync
                    ("containerusuarios", imagen.FileName, stream);
            }

            return RedirectToAction("VistaCubos", "Cubos");
        }
    }
}
