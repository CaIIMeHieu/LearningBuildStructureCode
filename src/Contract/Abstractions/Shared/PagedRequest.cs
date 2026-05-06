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
    public string? Sort
    {
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                return;
            SortOptions = value.Split(',').Select( S =>
            {
                var parts = S.Trim().Split('-');
                if (parts.Length != 2)
                    return null;
                if (!parts[1].Equals("ASC", StringComparison.OrdinalIgnoreCase) &&
                    !parts[1].Equals("DESC", StringComparison.OrdinalIgnoreCase))
                    return null;
                return new SortOption
                {
                    Field = parts[0],
                    IsDescending = parts[1].Equals("DESC", StringComparison.OrdinalIgnoreCase)
                };
            })
            .Where(s => s != null)
            .Select(s => s!)
            .ToList();
        }
    }
}
