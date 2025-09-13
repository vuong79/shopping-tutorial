using Microsoft.AspNetCore.Mvc;
using Shopping_Tutorial.Models;
using Microsoft.AspNetCore.Identity;
using Shopping_Tutorial.Models.ViewModels;
using Shopping_Tutorial.Areas.Admin.Repository;
namespace Shopping_Tutorial.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IEmailSender _emailSender;
        public AccountController(IEmailSender emailSender,UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(loginVM.Username, loginVM.Password, false, false);
                if (result.Succeeded)
                {
                    TempData["success"] = "Đăng nhập thành công.";
                    var receiver = "ny5489656@gmail.com";
                    var subject = "Đăng nhập thành công";
                    var message = "Bạn đã đăng nhập thành công vào trang Shopping_Tutorial";
                    await _emailSender.SendEmailAsync(receiver, subject, message);
                    return Redirect(loginVM.ReturnUrl ?? "/");
                }
                ModelState.AddModelError("", "Bạn nhập Username hoặc Password bị sai vui long nhập lại");
            }
            return View(loginVM);
        }
        public IActionResult Create()
        {
            return View();
        }
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

                    // Gửi username + password trực tiếp (plaintext)
                    var subject = "Thông tin tài khoản của bạn trên Shopping_Tutorial";
                    var message = $@"
                <p>Xin chào <strong>{user.Username}</strong>,</p>
                <p>Tài khoản của bạn đã được tạo thành công.</p>
                <p><strong>Tên đăng nhập:</strong> {user.Username}<br />
                   <strong>Mật khẩu:</strong> {user.Password}</p>
                <p>Vui lòng giữ kín thông tin này.</p>";

                    await _emailSender.SendEmailAsync(user.Email, subject, message);

                    return RedirectToAction("Login", "Account");
                }

                foreach (IdentityError error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }
            return View(user);
        }

        public async Task<IActionResult> Logout(string returnUrl ="/")
        {
            await _signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }
    }
}
