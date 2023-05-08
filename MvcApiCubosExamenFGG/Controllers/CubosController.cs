using ApiCubosExamenFGG.Models;
using Microsoft.AspNetCore.Mvc;
using MvcApiCubosExamenFGG.Models;
using MvcApiCubosExamenFGG.Services;

namespace MvcApiCubosExamenFGG.Controllers
{
    public class CubosController : Controller
    {
        private ServiceApiCubos service;
        private ServiceStorageBlobs azureservice;

        public CubosController(ServiceApiCubos service, ServiceStorageBlobs azureservice)
        {
            this.service = service;
            this.azureservice = azureservice;
        }

        public async Task<IActionResult> VistaCubos()
        {
            List<BlobModel> archivos = await this.azureservice.GetBlobsAsync("containercubos");
            List<Cubo> cubos = await this.service.GetAllCubosAPI();

            foreach (BlobModel b in archivos)
            {
                foreach(Cubo c in cubos)
                {
                    if (b.Nombre == c.Imagen)
                    {
                        c.Imagen = b.Url;
                        break;
                    }
                }
            }

            return View(cubos);
        }

        [HttpPost]
        public async Task<IActionResult> VistaCubos(string marca)
        {
            List<BlobModel> archivos = await this.azureservice.GetBlobsAsync("containercubos");
            List<Cubo> cubos = await this.service.GetCubosMarcaAPI(marca);

            foreach (BlobModel b in archivos)
            {
                foreach (Cubo c in cubos)
                {
                    if (b.Nombre == c.Imagen)
                    {
                        c.Imagen = b.Url;
                        break;
                    }
                }
            }

            return View(cubos);
        }

        public IActionResult CreateCubo()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCubo(Cubo cubo, IFormFile imagen)
        {
            cubo.Imagen = imagen.FileName;
            await this.service.CreateCuboAPI(cubo);

            using (Stream stream = imagen.OpenReadStream())
            {
                await this.azureservice.UploadBlobAsync
                    ("containercubos" , imagen.FileName, stream);
            }

            return RedirectToAction("VistaCubos", "Cubos");
        }
    }
}
