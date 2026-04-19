using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstractions.Entities;

/*
    Phân biệt giữa field và property
        Property : phương thức public đặc biệt cung cấp cách lấy và gán giá trị cho một field trong lớp
        Field : biến thành viên lưu trữ dữ liệu
    
    Có cách cách để một property có giá trị mà ko cần public setter
    private set;
    proteded set;
    Set qua constructor
   init-only property (C# 9+) : Chỉ cho phép gán giá trị một lần duy nhất lúc khởi tạo đối tượng


    Khi EF load entity ra, nó nhớ giá trị Primary Key ban đầu để biết row là row nào để update
	var product = await _repo.FindById(oldId);
    Khi đổi id là đang thay đổi thuộc tính EF coi là Key, ef sẽ raise exception, hoặc sql sẽ raise

    invariant : tính bất biến, điều phải luôn đúng với object tại mọi thời điểm. Ví dụ: "số dư tài khoản không bao giờ âm" là một invariant.

    sai : để public setter cho ID, vi phạm id có thể bị thay đổi sau khi tạo và không được gán khi tạo 

    ORM Frameworks (Object-Relational Mapping) như Entity Framework thường yêu cầu một constructor không tham số để có thể tạo instance của entity khi load dữ liệu từ database. 
    Nếu chỉ có constructor có tham số, EF sẽ không thể tạo instance của entity.
   
*/

public abstract class DomainEntity<TKey>
{
    public virtual TKey Id { get; protected set; }

    protected DomainEntity() { }
    protected DomainEntity( TKey id )
    {
        Id = id;
    }
}
