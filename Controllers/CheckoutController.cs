using Microsoft.AspNetCore.Mvc;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Repository;
using System.Security.Claims;
using Shopping_Tutorial.Areas.Admin.Repository;

namespace Shopping_Tutorial.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IEmailSender _emailSender;

        public CheckoutController(IEmailSender emailSender, DataContext dataContext)
        {
            _dataContext = dataContext;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Checkout()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            // Tạo đơn hàng mới
            var orderCode = Guid.NewGuid().ToString();
            var orderItem = new OrderModel
            {
                OrderCode = orderCode,
                UserName = userEmail,
                Status = 1, // Đơn mới
                OrderDate = DateTime.Now
            };

            // Lấy giỏ hàng
            var cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            decimal totalAmount = 0;

            foreach (var cartItem in cartItems)
            {
                var orderDetail = new OrderDetails
                {
                    UserName = userEmail,
                    OrderCode = orderCode,
                    ProductId = cartItem.ProductId,
                    Price = cartItem.Price,
                    Quantity = cartItem.Quantity
                };

                orderItem.OrderDetails.Add(orderDetail);
                totalAmount += cartItem.Price * cartItem.Quantity;
            }

            orderItem.TotalAmount = totalAmount;

            _dataContext.Add(orderItem);
            _dataContext.SaveChanges();

            // Xóa giỏ hàng
            HttpContext.Session.Remove("Cart");

            // Gửi email cho người mua
            var subject = "Xác nhận đặt hàng thành công";
            var message = $@"
                <p>Xin chào <b>{userEmail}</b>,</p>
                <p>Bạn đã đặt hàng thành công tại <b>Shopping_Tutorial</b>.</p>
                <p><b>Mã đơn hàng:</b> {orderCode}</p>
                <p><b>Tổng tiền:</b> {totalAmount:N0} VND</p>
                <p>Chúng tôi sẽ xử lý đơn hàng và liên hệ với bạn trong thời gian sớm nhất.</p>
            ";

            await _emailSender.SendEmailAsync(userEmail, subject, message);

            TempData["success"] = "Đặt hàng thành công. Chúng tôi sẽ liên hệ với bạn trong thời gian sớm nhất.";
            return RedirectToAction("Index", "Cart");
        }
    }
}
