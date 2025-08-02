    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using Shopping_Tutorial.Models;
    using Shopping_Tutorial.Repository;

    namespace Shopping_Tutorial.Areas.Admin.Controllers
    {
        [Area("Admin")]
        [Route("Admin/Product")]
        [Authorize]

        public class ProductController : Controller
        {
            private readonly DataContext _dataContext;
            private readonly IWebHostEnvironment _webHostEnvironment;

            public ProductController(DataContext context, IWebHostEnvironment webHostEnvironment)
            {
                _dataContext = context;
                _webHostEnvironment = webHostEnvironment;
            }

            [Route("Index")]
            [HttpGet]
            public async Task<IActionResult> Index()
            {
                return View(await _dataContext.Products
                    .OrderByDescending(p => p.Id)
                    .Include(p => p.Category)
                    .Include(p => p.Brand)
                    .ToListAsync());
            }

            [Route("Create")]
            public IActionResult Create()
            {
                ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name");
                ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name");
                return View();
            }

            [Route("Create")]
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(ProductModel product)
            {
                ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
                ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);

                if (ModelState.IsValid)
                {
                    // Tạo slug từ tên sản phẩm
                    product.Slug = product.Name.Replace(" ", "-");
                    var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);
                    if (slug != null)
                    {
                        ModelState.AddModelError("", "Slug đã tồn tại, hãy thử một tên khác.");
                        return View(product);
                    }

                    // Xử lý việc tải lên hình ảnh
                    if (product.ImageUpload != null)
                    {
                        string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                        string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                        string filePath = Path.Combine(uploadsDir, imageName);

                        FileStream fs = new FileStream(filePath, FileMode.Create);
                        await product.ImageUpload.CopyToAsync(fs);
                        fs.Close();
                        product.Image = imageName;
                    }

                    _dataContext.Add(product);
                    await _dataContext.SaveChangesAsync();
                    TempData["success"] = "Thêm sản phẩm thành công.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["error"] = "Model có một vài thứ đang bị lỗi. Vui lòng kiểm tra lại dữ liệu và thử lại sau.";
                    List<string> errors = new List<string>();
                    foreach (var value in ModelState.Values)
                    {
                        foreach (var error in value.Errors)
                        {
                            errors.Add(error.ErrorMessage);
                        }
                    }
                    string errorMessage = string.Join("\n", errors);
                    return BadRequest(errorMessage);
                }
                return View(product);
            }


            [Route("Edit")]
            public async Task<IActionResult> Edit(int Id)
            {
                ProductModel product = await _dataContext.Products.FindAsync(Id);
                ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
                ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);
                return View(product);
            }
            [Route("Edit")]
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(ProductModel product)
            {
                ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
                ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);
                var existed_product = _dataContext.Products.Find(product.Id);

                if (ModelState.IsValid)
                {

                    product.Slug = product.Name.Replace(" ", "-");
                    if (product.ImageUpload != null)
                    {

                        // upload new image
                        string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                        string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                        string filePath = Path.Combine(uploadsDir, imageName);

                        // delete old image if exists
                        string oldfilePath = Path.Combine(uploadsDir, existed_product.Image);
                        try
                        {
                            if (System.IO.File.Exists(oldfilePath))
                            {
                                System.IO.File.Delete(oldfilePath); // Xóa hình ảnh cũ nếu tồn tại
                            }
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", "Không thể xóa hình ảnh sản phẩm cũ: ");
                        }

                        FileStream fs = new FileStream(filePath, FileMode.Create);
                        await product.ImageUpload.CopyToAsync(fs);
                        fs.Close();
                        existed_product.Image = imageName;
                    
                    }

                    existed_product.Name = product.Name;
                    existed_product.Description = product.Description;
                    existed_product.Price = product.Price;
                    existed_product.CategoryId = product.CategoryId;
                    existed_product.BrandId = product.BrandId;

                    _dataContext.Update(existed_product);

                    await _dataContext.SaveChangesAsync();
                    TempData["success"] = "Cập nhật sản phẩm thành công.";
                    return RedirectToAction("Index");

                }
                else
                {
                    TempData["error"] = "Model có một vài thứ đang bị lỗi. Vui lòng kiểm tra";
                    List<string> errors = new List<string>();
                    foreach (var value in ModelState.Values)
                    {
                        foreach (var error in value.Errors)
                        {
                            errors.Add(error.ErrorMessage);
                        }
                    }
                    string errorMessage = string.Join("\n", errors);
                    return BadRequest(errorMessage);
                }
                return View(product);
                }
        public async Task<IActionResult> Delete(int Id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(Id);

            if (product == null)
            {
                TempData["error"] = "Sản phẩm không tồn tại hoặc đã bị xóa.";
                return RedirectToAction("Index");
            }

            if (!string.Equals(product.Image, "1.jpg", StringComparison.OrdinalIgnoreCase))
            {
                string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                string oldfilePath = Path.Combine(uploadsDir, product.Image);

                if (System.IO.File.Exists(oldfilePath))
                {
                    try
                    {
                        System.IO.File.Delete(oldfilePath);
                    }
                    catch (Exception ex)
                    {
                        TempData["error"] = "Không thể xóa hình ảnh sản phẩm: " + ex.Message;
                        return RedirectToAction("Index");
                    }
                }
            }

            _dataContext.Products.Remove(product);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Xóa sản phẩm thành công.";
            return RedirectToAction("Index");
        }
    }
    }
