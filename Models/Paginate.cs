namespace Shopping_Tutorial.Models
{
    public class Paginate
    {
        public int TotalItems { get; private set; } // Tổng số item
        public int PageSize { get; private set; } // Số item trên mỗi trang
        public int CurrentPage { get; private set; } // Trang hiện tại
        public int TotalPages { get; private set; }// Tổng số trang
        public int StartPage { get; private set; } // Trang bắt đầu hiển thị
        public int EndPage { get; private set; }// Trang kết thúc hiển thị
        public Paginate()
        {

        }
        public Paginate(int totalItems, int page, int pageSize = 10)
        {
           int totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
            int currentPage = page;
            int startPage = page - 5;
            int endPage = page + 4;
            if (startPage <= 0)
            {
                endPage -= (startPage - 1);
                startPage = 1;
            }
            if (endPage > totalPages)
            {
                endPage = totalPages;
                if (endPage > 10)
                {
                    startPage = endPage - 9;
                }
            }
            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage; 
        }
    }
}
