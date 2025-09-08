using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping_Tutorial.Models;

namespace Shopping_Tutorial.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/User")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // ---------------------- DANH SÁCH NGƯỜI DÙNG ----------------------
        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.OrderByDescending(u => u.Id).ToListAsync();
            var userList = new List<AppUserModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.RoleNames = string.Join(", ", roles); // ghép role thành chuỗi
                userList.Add(user);
            }

            return View(userList);
        }

        // ---------------------- TẠO NGƯỜI DÙNG ----------------------
        [HttpGet]
        [Route("Create")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Id", "Name");
            return View(new AppUserModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create(AppUserModel model, string password)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Id", "Name", model.RoleId);
                return View(model);
            }

            var result = await _userManager.CreateAsync(model, password);

            if (result.Succeeded)
            {
                // Gán Role
                var role = await _roleManager.FindByIdAsync(model.RoleId);
                if (role != null)
                {
                    await _userManager.AddToRoleAsync(model, role.Name);
                }

                TempData["success"] = "Tạo người dùng thành công!";
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Id", "Name", model.RoleId);
            return View(model);
        }

        // ---------------------- SỬA NGƯỜI DÙNG ----------------------
        [HttpGet]
        [Route("Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Id", "Name", user.RoleId);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit")]
        public async Task<IActionResult> Edit(AppUserModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Id", "Name", model.RoleId);
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.Occupation = model.Occupation;
            user.RoleId = model.RoleId;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                // Cập nhật Role (nếu có)
                var currentRoles = await _userManager.GetRolesAsync(user);
                var newRole = await _roleManager.FindByIdAsync(model.RoleId);
                if (newRole != null)
                {
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    await _userManager.AddToRoleAsync(user, newRole.Name);
                }

                TempData["success"] = "Cập nhật thành công!";
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Id", "Name", model.RoleId);
            return View(model);
        }

        // ---------------------- XÓA NGƯỜI DÙNG ----------------------
        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["success"] = "Xóa thành công!";
            }
            else
            {
                TempData["error"] = "Xóa thất bại!";
            }

            return RedirectToAction("Index");
        }
    }
}
