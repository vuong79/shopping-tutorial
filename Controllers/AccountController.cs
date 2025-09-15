using Microsoft.AspNetCore.Mvc;
using Shopping_Tutorial.Models;
using Microsoft.AspNetCore.Identity;
using Shopping_Tutorial.Models.ViewModels;
using Shopping_Tutorial.Areas.Admin.Repository;

namespace Shopping_Tutorial.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly SignInManager<AppUserModel> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountController(
            IEmailSender emailSender,
            UserManager<AppUserModel> userManager,
            SignInManager<AppUserModel> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        // GET: Login
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        // POST: Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    loginVM.Username, loginVM.Password, false, false);

                if (result.Succeeded)
                {
                    TempData["success"] = "Đăng nhập thành công.";

                    // Lấy user theo Username
                    var user = await _userManager.FindByNameAsync(loginVM.Username);

                    if (user != null && !string.IsNullOrEmpty(user.Email))
                    {
                        var subject = "Đăng nhập thành công";
                        var message = $@"
                        <p>Xin chào <strong>{user.UserName}</strong>,</p>
                        <p>Bạn đã đăng nhập thành công vào hệ thống lúc {DateTime.Now}.</p>";

                        await _emailSender.SendEmailAsync(user.Email, subject, message);
                    }

                    return Redirect(loginVM.ReturnUrl ?? "/");
                }

                ModelState.AddModelError("", "Sai Username hoặc Password.");
            }
            return View(loginVM);
        }

        // GET: Create User
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create User
        [HttpPost]
        public async Task<IActionResult> Create(UserModel user)
        {
            if (ModelState.IsValid)
            {
                var newUser = new AppUserModel
                {
                    UserName = user.Username,
                    Email = user.Email
                };

                IdentityResult result = await _userManager.CreateAsync(newUser, user.Password);

                if (result.Succeeded)
                {
                    TempData["success"] = "Tạo user thành công.";

                    // Gửi mail thông tin tài khoản
                    var subject = "Thông tin tài khoản của bạn";
                    var message = $@"
                    <p>Xin chào <strong>{user.Username}</strong>,</p>
                    <p>Tài khoản của bạn đã được tạo thành công.</p>
                    <p><b>Username:</b> {user.Username}<br/>
                    <b>Password:</b> {user.Password}</p>";

                    await _emailSender.SendEmailAsync(user.Email, subject, message);

                    return RedirectToAction("Login", "Account");
                }

                foreach (IdentityError error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }
            return View(user);
        }

        // Logout
        public async Task<IActionResult> Logout(string returnUrl = "/")
        {
            await _signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }
    }
}
