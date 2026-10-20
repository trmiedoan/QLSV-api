# Information

- Database:
  - SQL Server --- SQLEXPRESS
  - Database name: QLSV2
  - Sql queries:
    - `.\CreateTables.sql`
    - `.\GenerateSamples.sql`
- `Properties/launchSettings.json` in ASP.NET Core.
  - running at: "https://localhost:7188;http://localhost:5230",

# Quick start

1. Tao database QLSV2 (SQL Server express) và chạy file `CreateTables.sql` để tạo bảng cho các model, thêm các dữ liệu cho bảng bằng cách chạy file `GenerateSamples.sql`.
2. `dotnet restore`
3. `dotnet run` để chạy dự án. (hoặc dùng `dotnet run --launch-profile https` để chạy dự án trên https)
4. Mở https://localhost:7188/swagger

---

# Brainstorming

Models:

- Student:
  - maSV
  - tenSV
  - ngaysinh
  - gioi tinh
  - khoa
- Major:
  - majorCode: CNTT, DTVT,...
  - majorName: Coong nghe thong tin, dienj tu vien thong,...

Controllers:

- StudentController
  - GetAllStudent (filter/search, sort?, pagination): GET /api/students
  - GetStudentById: GET /api/students/{id}
  - CreateStudent: POST /api/students
  - UpdateStudent: PUT /api/students/{id}
  - DeleteStudent: DELETE /api/students/{id}

# Database

- Student
  - cardId
  - Name
  - BirthDate
  - Gender
  - MajorId
- Major
  - Id
  - Name

# API

## Student

### GetAllStudent

GET /api/students

Filter:

- Name
- BirthDate
- Gender
- MajorId

Sort:

- Name
- BirthDate
- Gender
- MajorId

Pagination:

- Page
- PageSize

Response:

- List<Student>

### GetStudentById

GET /api/students/{id}

Response:

- Student

### CreateStudent

POST /api/students
Request:

- Name
- BirthDate
- Gender
- MajorId

Response:

- Student

### UpdateStudent

PUT /api/students/{id}
Request:

- Name
- BirthDate
- Gender
- MajorId

Response:

- Student

### DeleteStudent

DELETE /api/students/{id}

Response:

- Student

# HTTP Status Code

Lấy dữ liệu thành công 200 OK
Tạo sinh viên thành công 201 Created
Xóa thành công nhưng không trả body 204 No Content
Dữ liệu đầu vào không hợp lệ 400 Bad Request
Không tìm thấy sinh viên 404 Not Found
Mã sinh viên bị trùng 409 Conflict
