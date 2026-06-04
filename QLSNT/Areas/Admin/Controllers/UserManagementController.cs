using Microsoft.AspNetCore.Mvc;

namespace QLSNT.Areas.Admin.Controllers
{
    public class UserManagementController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
