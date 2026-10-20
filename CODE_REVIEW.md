# BÁO CÁO REVIEW DỰ ÁN QLSV-API (ASP.NET Core & EF Core)

> **Mục tiêu dự án:** Quản lý sinh viên & ngành học (CRUD, Search, Pagination).  
> **Framework:** .NET 10, ASP.NET Core Web API, Entity Framework Core 10 (SQL Server).

---

## 1. BỘ CÂU HỎI TỰ ĐÁNH GIÁ KIẾN THỨC (KNOWLEDGE CHECK)

Hãy tự trả lời các câu hỏi sau để chắc chắn bạn đã thực sự làm chủ kiến thức và cú pháp đã sử dụng trong dự án:

### 1.1. Entity Framework Core & Tối ưu truy vấn
1. **`IQueryable` vs `IEnumerable` / `List`:**
   - Trong `GetAllStudents`, tại sao dòng `var students = await _context.Students.Include(s => s.Major).ToListAsync();` lại nguy hiểm khi bảng có hàng chục nghìn sinh viên?
   - Phép phân trang `.Skip().Take()` nên được gọi **trước** hay **sau** `.ToListAsync()`? Truy vấn SQL sinh ra ở 2 trường hợp này khác nhau như thế nào?
2. **Change Tracking & `AsNoTracking()`:**
   - Cơ chế Change Tracker trong EF Core hoạt động như thế nào?
   - Tại sao với các API chỉ đọc dữ liệu (GET), việc thêm `.AsNoTracking()` lại giúp tiết kiệm RAM và tăng tốc độ xử lý?
3. **Eager Loading & Navigation Property:**
   - Trong `UpdateStudent`, bạn dùng `_context.Students.FirstOrDefaultAsync(s => s.CardId == cardId)`. Khi gọi `studentToUpdate.ToStudentDto()`, điều gì sẽ xảy ra với `Major.MajorCode` và `Major.Name`? Tại sao?

### 1.2. Dependency Injection & Service Lifetimes
4. Trong `Program.cs`, `builder.Services.AddDbContext<StudentDbContext>(...)` đăng ký DbContext với vòng đời nào: `Transient`, `Scoped` hay `Singleton`?
   - Tại sao DbContext **tuyệt đối không** nên đăng ký dưới dạng `Singleton` trong môi trường Web đa luồng?

### 1.3. Kiến trúc DTO & Ngôn ngữ C#
5. **Tại sao không dùng trực tiếp Model `Student` làm tham số đầu vào hoặc kết quả trả về của API?**
   - Phân tích rủi ro về tấn công *Overposting* (Mass Assignment).
   - Phân tích lỗi vòng lặp tuần hoàn JSON (*Circular Reference*: `Student -> Major -> Students`).
6. Cú pháp gán `= null!;` (null-forgiving operator) ở các property DTO/Model có ý nghĩa gì đối với trình biên dịch C#? Nó có ngăn được việc dữ liệu thực tế tại runtime bị `null` hay không?

---

## 2. BẢO MẬT, EDGE CASES, BUG TIỀM ẨN & CODE THỪA

### 2.1. Lỗi logic & Edge cases nghiêm trọng

#### Bug 1: Lỗi NullReferenceException gây HTTP 500 khi cập nhật sinh viên
- **Vị trí:** `StudentsController.cs` (Action `UpdateStudent`)
- **Hiện tượng:**
  ```csharp
  var studentToUpdate = await _context.Students.FirstOrDefaultAsync(s => s.CardId == cardId);
  // ... Cập nhật các trường thông tin ...
  await _context.SaveChangesAsync();
  return studentToUpdate.ToStudentDto(); // <-- CRASH TẠI ĐÂY
  ```
- **Nguyên nhân:** Query lấy `studentToUpdate` không dùng `.Include(s => s.Major)`. Khi gọi `ToStudentDto()`, code truy cập `Major.MajorCode` và `Major.Name`, nhưng thuộc tính `Major` đang là `null`.
- **Khắc phục:** Thêm `.Include(s => s.Major)` khi query hoặc map DTO trả về mà không truy cập navigation property chưa load.

#### Bug 2: Crash server HTTP 500 do tính sai phân trang khi dữ liệu trống
- **Vị trí:** `StudentsController.cs` (Action `GetAllStudents`)
- **Hiện tượng:**
  ```csharp
  var totalCount = students.Count;
  var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

  if (page > totalPages) { page = totalPages; }
  var skip = (page - 1) * pageSize;
  students = students.Skip(skip).Take(pageSize).ToList();
  ```
- **Nguyên nhân:** Nếu danh sách rỗng (`totalCount = 0`), `totalPages = 0`. Điều kiện `page > totalPages` sẽ gán `page = 0`. Tiếp theo, `skip = (0 - 1) * pageSize = -10`. Hàm `Skip(-10)` nhận số âm sẽ quăng ngoại lệ `ArgumentOutOfRangeException` $\rightarrow$ Crash API 500.
- **Khắc phục:**
  ```csharp
  var totalPages = totalCount == 0 ? 1 : (int)Math.Ceiling((double)totalCount / pageSize);
  if (page > totalPages) page = totalPages;
  if (page < 1) page = 1;
  ```

#### Bug 3: Validation `BirthDate` không hợp lý
- **Vị trí:** `CreateStudent` và `UpdateStudent`
- **Hiện tượng:**
  ```csharp
  if (!DateOnly.TryParse(student.BirthDate.ToString(), out var birthDate))
  ```
- **Nguyên nhân:** Biến `student.BirthDate` vốn đã là kiểu `DateOnly` do Model Binder của ASP.NET Core tự parse từ JSON. Việc gọi `.ToString()` rồi `TryParse()` lại là dư thừa và dễ gây lỗi parse theo thiết lập Culture (vùng miền) của server.
- **Thiếu sót:** Chưa kiểm tra nghiệp vụ ngày sinh: ngày sinh trong tương lai (`BirthDate > DateOnly.FromDateTime(DateTime.Today)`) hoặc tuổi vượt quá giới hạn (ví dụ > 100 tuổi).

#### Bug 4: Race condition & Thiếu try-catch vi phạm Unique Constraint
- **Vị trí:** `CreateStudent`
- **Hiện tượng:**
  ```csharp
  if (_context.Students.Any(s => s.CardId == student.CardId)) // Lưu ý: Nên dùng AnyAsync
  {
      return Conflict();
  }
  ```
- **Nguyên nhân:** Nếu hai request gửi cùng 1 mã sinh viên đồng thời (concurrency), cả hai đều vượt qua lệnh `if`. Khi gọi `SaveChangesAsync()`, Database sẽ ném `DbUpdateException` do vi phạm constraint `UQ_Students_CardId`. Nếu không có `try-catch`, API sẽ trả về HTTP 500 thay vì HTTP 409 chuẩn.

---

### 2.2. Vấn đề bảo mật (Security Concerns)

1. **`TrustServerCertificate=True` trong `appsettings.json`:**
   - Thiết lập này bỏ qua việc kiểm tra chứng chỉ SSL của SQL Server, chấp nhận được khi học tập trên Localhost, nhưng lên môi trường Staging/Production sẽ tạo lỗ hổng tấn công Man-in-the-middle (MITM).
   - Không lưu tài khoản/mật khẩu database thật trong file `appsettings.json` khi đẩy lên Git. Hãy sử dụng Secret Manager (`dotnet user-secrets`) hoặc Biến môi trường (Environment Variables).
2. **Thiếu giới hạn độ dài chuỗi đầu vào (Input Validation):**
   - Database quy định `CardId` tối đa 20 ký tự, `Name` tối đa 100 ký tự.
   - Nhưng các DTO không có Validation Attribute (như `[MaxLength(20)]`, `[MaxLength(100)]`). Nếu client cố tình gửi payload chứa 50,000 ký tự, EF Core sẽ ném `DbUpdateException` (chuỗi bị cắt ngắn) làm server trả về 500.
3. **Thiếu Global Exception Handling:**
   - Dự án chưa có Middleware bắt lỗi tập trung (`UseExceptionHandler`). Khi có lỗi kết nối database hay timeout, thông tin chi tiết Stack Trace và cấu trúc hệ thống có thể bị lộ ra ngoài qua phản hồi HTTP.

---

### 2.3. Dead Code & Code rườm rà (Verbose)

1. **Dead Code trong `MajorsController.cs`:**
   - Từ dòng 31 đến dòng 104 là toàn bộ code bị comment out.
   - Hàm `private bool MajorExists(int id)` không bao giờ được gọi.
   - *Khắc phục:* Xóa bỏ toàn bộ code comment và hàm thừa để controller gọn gàng.
2. **Lạm dụng từ khóa `partial`:**
   - Các class trong thư mục `DTOs/` (`MajorDto`, `StudentDto`, `CreateStudentDto`,...) đều khai báo `public partial class`. DTO là class POCO do bạn tự định nghĩa, không phải source code do tool sinh ra, nên không cần từ khóa `partial`.
3. **Khai báo Nullable mâu thuẫn:**
   - `public string? MajorCode { get; set; } = null!;`
   - Khai báo kiểu `string?` (cho phép null) nhưng lại khởi tạo `= null!;` (bảo đảm không null). Hãy thống nhất:
     - Cho phép null: `public string? MajorCode { get; set; }`
     - Không cho phép null: `public string MajorCode { get; set; } = string.Empty;`

---

## 3. LỘ TRÌNH HỌC TẬP VÀ PHÁT TRIỂN TIẾP THEO

### Giai đoạn 1: Chuẩn hóa code & Truy vấn tối ưu (Clean Data Access)
1. **Phân trang và lọc dữ liệu trực tiếp trên Database (`IQueryable`):**
   - Không dùng `await _context.Students.ToListAsync()` rồi mới lọc `.Where()` và `.Skip().Take()` trong bộ nhớ RAM.
   - Viết truy vấn LINQ đẩy toàn bộ xuống SQL Server:
     ```csharp
     var query = _context.Students
         .AsNoTracking()
         .Include(s => s.Major)
         .AsQueryable();

     if (!string.IsNullOrWhiteSpace(search))
     {
         query = query.Where(s => s.Name.Contains(search) || s.CardId.Contains(search));
     }

     var totalCount = await query.CountAsync();
     var students = await query
         .Skip((page - 1) * pageSize)
         .Take(pageSize)
         .Select(s => s.ToStudentDto())
         .ToListAsync();
     ```
2. **Data Annotations & FluentValidation:**
   - Tìm hiểu thư viện **FluentValidation** để tách toàn bộ logic kiểm tra hợp lệ ra khỏi Controller:
     - Validate ngày sinh, định dạng mã sinh viên, giới tính hợp lệ (`Nam` / `Nữ`).
3. **EF Core Migrations (Code-First):**
   - Thay vì chạy file SQL `CreateTables.sql` thủ công, hãy học cách dùng EF Core Migration:
     ```bash
     dotnet ef migrations add InitialCreate
     dotnet ef database update
     ```

### Giai đoạn 2: Nâng cấp kiến trúc (Architecture & Best Practices)
4. **Mô hình Service Layer (Tách tầng):**
   - Controller hiện tại đang làm việc trực tiếp với DbContext và chứa cả logic kiểm tra.
   - Tạo tầng dịch vụ: `IStudentService` $\rightarrow$ `StudentService` để xử lý nghiệp vụ, giúp Controller mỏng nhẹ (chỉ nhận HTTP request $\rightarrow$ gọi service $\rightarrow$ trả HTTP status).
5. **Global Exception Handling & ProblemDetails (RFC 7807):**
   - Cấu hình Middleware xử lý lỗi toàn cục để mọi lỗi phát sinh (400, 404, 409, 500) đều trả về chuẩn JSON `ProblemDetails` đồng nhất cho Frontend dễ bắt lỗi.

### Giai đoạn 3: Tính năng nâng cao cho dự án thực tế
6. **Xác thực và phân quyền (Authentication & Authorization):**
   - Tìm hiểu ASP.NET Core Identity hoặc JWT (JSON Web Token).
   - Thêm bảng `Users` và `Roles`: Sinh viên/Khách chỉ có quyền GET; Quản trị viên (Admin) mới có quyền POST, PUT, DELETE.
7. **Viết Automated Tests:**
   - **Unit Tests:** Kiểm thử các hàm logic với `xUnit` và `Moq`.
   - **Integration Tests:** Kiểm thử endpoint API với `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory`) và In-Memory Database.
