using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Areas.Admin.Repository;

namespace Shopping_Tutorial.Controllers
{
    public class OrderController : Controller
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly IEmailSender _emailSender;

        public OrderController(UserManager<AppUserModel> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(OrderModel order)
        {
            if (!ModelState.IsValid)
                return View(order);

            // Lưu order vào DB (ví dụ)
            // _context.Orders.Add(order);
            // await _context.SaveChangesAsync();

            // Lấy user hiện tại
            var user = await _userManager.GetUserAsync(User);

            if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                var subject = $"Xác nhận đơn hàng #{order.Id}";
                var message = $@"
                <p>Xin chào <strong>{user.UserName}</strong>,</p>
                <p>Cảm ơn bạn đã đặt hàng tại Shopping_Tutorial.</p>
                <p><b>Mã đơn hàng:</b> {order.Id}<br/>
                <b>Tổng tiền:</b> {order.TotalAmount:N0} VND</p>
                <p>Chúng tôi sẽ xử lý đơn hàng trong thời gian sớm nhất.</p>";

                await _emailSender.SendEmailAsync(user.Email, subject, message);
            }

            TempData["success"] = "Đặt hàng thành công!";
            return RedirectToAction("Index", "Home");
        }
    }
}
