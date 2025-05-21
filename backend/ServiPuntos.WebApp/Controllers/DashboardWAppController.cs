using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ServiPuntos.Core.Interfaces;


namespace ServiPuntos.Controllers
{
     // Asegura que solo usuarios autenticados puedan acceder
    public class DashboardWAppController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}