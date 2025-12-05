using Microsoft.AspNetCore.Mvc;

namespace WorkspaceTenants.Controllers{

public class HomeController : Controller
{
    public IActionResult Index() => View();
    public IActionResult Login() => View();
}
}