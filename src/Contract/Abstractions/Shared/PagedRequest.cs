using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Abstractions.Shared;

public class SortOption
{
    public string Field { get; set; } = string.Empty;
    public bool IsDescending { get; set; }
}

public class PagedRequest
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    // Danh sách các cột cần sắp xếp
    public List<SortOption> SortOptions { get; set; } = new List<SortOption>();
}
