using Microsoft.AspNetCore.Mvc;
using Shopping_Tutorial.Repository;

namespace Shopping_Tutorial.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        private readonly DataContext _dataContext;
        public UserController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
