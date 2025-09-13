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
        public CheckoutController(IEmailSender emailSender,DataContext dataContext)
        {
            _dataContext = dataContext;
            _emailSender = emailSender;
        }
        public async Task<IActionResult> Checkout()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null) 
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var ordercode = Guid.NewGuid().ToString();
                var orderItem = new OrderModel();
                orderItem.OrderCode = ordercode;
                orderItem.UserName = userEmail;
                orderItem.Status = 1;
                orderItem.CreateDate = DateTime.Now;
                _dataContext.Add(orderItem);
                _dataContext.SaveChanges();
                List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
                foreach (var cartItem in cartItems)
                {
                    var orderDetail = new OrderDetails();
                    orderDetail.UserName = userEmail;
                    orderDetail.OrderCode = ordercode;
                    orderDetail.ProductId = cartItem.ProductId; 
                    orderDetail.Price = cartItem.Price;
                    orderDetail.Quantity = cartItem.Quantity;
                    _dataContext.Add(orderDetail);
                    _dataContext.SaveChanges();
                }
                HttpContext.Session.Remove("Cart");
                var receiver = "ny5489656@gmail.com";
                var subject = "Đăt hang thanh cong";
                var message = "Bạn đã đăt hang thành công vào trang Shopping_Tutorial";
                await _emailSender.SendEmailAsync(receiver, subject, message);
                TempData["success"] = "Đặt hàng thành công. Chúng tôi sẽ liên hệ với bạn trong thời gian sớm nhất.";
                return RedirectToAction("Index", "Cart");
            }
                return View();
        }
    }
}
