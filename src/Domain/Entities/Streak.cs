using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class Streak
{
    public int Current { get; private set; }
    public int Longest { get; private set; }
    public DateOnly? LastReviewedDate { get; private set; }

    // Cho entity framework core có thể sử dụng để tạo instance của Streak khi truy vấn từ database
    protected Streak() { }
    private Streak(int current, int longest, DateOnly? lastReviewedDate)
    {
        if (current < 0)
            throw new ArgumentException("Streak current cannot be negative.");
        if (longest < current)
            throw new ArgumentException("Longest must be >= current.");
        Current = current;
        Longest = longest;
        LastReviewedDate = lastReviewedDate;
    }

    public static Streak New() => new(0, 0, null);
    public Streak Reset() => new(0, Longest, LastReviewedDate);

    // cập nhật streak xem là sẽ tiếp tục chuỗi hay là reset
    public Streak RecordReview( DateOnly reviewDate, bool hasMissedDays )
    {
        if( reviewDate == LastReviewedDate ) return this; // không cập nhật nếu ngày review trùng với ngày review cuối cùng
        var isConsecutive = !hasMissedDays && LastReviewedDate.HasValue;
        var newCurrent = isConsecutive ? Current + 1 : 0; // nếu có bỏ lỡ ngày nào thì reset current về 0, nếu không thì tăng current lên 1
        var newLongest = Math.Max(Longest, newCurrent); // cập nhật longest nếu current mới lớn hơn longest
        return new Streak(newCurrent, newLongest, reviewDate);
    }
    tại sao method này lại nằm trong Streak
}
