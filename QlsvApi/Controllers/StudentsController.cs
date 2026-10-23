using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QlsvApi.Data;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly StudentDbContext _context;



    public StudentsController(StudentDbContext context)
    {
        _context = context;
    }


    // GET: api/Students
    [HttpGet]
    public async Task<ActionResult<StudentListDto>> GetAllStudents(
         string? search = null,
    int page = 1,
    int pageSize = 10
    )
    {
        // gioi han pageSize <=100, page >0
        if (pageSize > 100)
        {
            pageSize = 100;
        }
        else if (pageSize <= 0)
        {
            pageSize = 10;
        }

        if (page <= 0)
        {
            page = 1;
        }

        // Thêm search & pagination, trả về danh sách và total page
        // var students = await _context.Students.Include(s => s.Major).ToListAsync(); // gọi ToListAsync() quá sớm, chưa phân trang, dễ load cả triệu bản ghi xuống RAM
        var students = _context.Students.AsNoTracking().Include(s => s.Major).AsQueryable(); // gọi ToListAsync() quá sớm, chưa phân trang, dễ load cả triệu bản ghi xuống RAM
        if (!string.IsNullOrWhiteSpace(search))
        {
            students = students.Where(s => s.Name.Contains(search) || s.CardId.Contains(search));
        }

        var totalCount = await students.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        if (page > totalPages)
        {
            page = totalPages;
        }
        var skip = (page - 1) * pageSize; // bo qua cac dong nam o page truoc do
        var result = await students.OrderBy(s => s.CardId).Skip(skip).Take(pageSize).ToListAsync();

        return new StudentListDto
        {
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages,
            TotalCount = totalCount,
            Students = result.Select(s => s.ToStudentDto()).ToList()
        };
    }

    // GET: api/Students/SV001
    [HttpGet("{cardId}")]
    public async Task<ActionResult<StudentDto>> GetStudentById(string cardId)
    {
        // var student = await _context.Students.FirstOrDefaultAsync(m => m.CardId == cardId); // → lúc trả về thì Major sẽ bị null vì chỉ lấy dữ liệu trong bảng Students
        var student = await _context.Students.AsNoTracking().Include(s => s.Major).FirstOrDefaultAsync(m => m.CardId == cardId);

        if (student == null)
        {
            return NotFound();
        }

        return student.ToStudentDto();
    }

    // POST: api/Students
    [HttpPost]
    public async Task<ActionResult<CreateStudentDto>> CreateStudent(CreateStudentDto student)
    {
        // check if student with the same cardId already exists 
        // mặc dù có kiểm tra ở đây nhưng trong db vẫn cần contrainst UNIQUE để kiểm tra tính unique của cardId trước khi thực hiện lệnh thêm
        if (_context.Students.Any(s => s.CardId == student.CardId))
        {
            return Conflict(); // 409 Conflict: request xung đột với trạng thái hiện tại (cardId đã tồn tại)
        }

        // check if major with the same majorId already exists: exists => add student to the major
        var major = await _context.Majors.FirstOrDefaultAsync(m => m.Id == student.MajorId);
        if (major == null)
        {
            return NotFound();
        }

        // check if Gender is valid: string "Nam" or "Nữ"
        if (student.Gender != "Nam" && student.Gender != "Nữ")
        {
            return BadRequest();
        }

        // check if BirthDate is valid: date in format "yyyy-MM-dd"
        if (!DateOnly.TryParse(student.BirthDate.ToString(), out var birthDate))
        {
            return BadRequest();
        }

        _context.Students.Add(student.ToStudent());
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetStudentById", new { cardId = student.CardId }, student);
    }

    // PUT: api/Students/{cardId}
    [HttpPut("{cardId}")]
    public async Task<ActionResult<StudentDto>> UpdateStudent(string cardId, UpdateStudentDto student)
    {
        // check if student with the same cardId already exists 
        // Change tracking: 
        var studentToUpdate = await _context.Students.FirstOrDefaultAsync(s => s.CardId == cardId);
        if (studentToUpdate == null)
        {
            return NotFound();
        }

        // check if Gender is valid: string "Nam" or "Nữ"
        if (student.Gender != "Nam" && student.Gender != "Nữ")
        {
            return BadRequest();
        }

        // check if BirthDate is valid: date in format "yyyy-MM-dd"
        if (!DateOnly.TryParse(student.BirthDate.ToString(), out var birthDate))
        {
            return BadRequest();
        }

        // check if MajorId is valid: exists => update student
        var major = await _context.Majors.FirstOrDefaultAsync(m => m.Id == student.MajorId);
        if (major == null)
        {
            return NotFound();
        }

        studentToUpdate.Name = student.Name;
        studentToUpdate.BirthDate = student.BirthDate;
        studentToUpdate.Gender = student.Gender;
        studentToUpdate.MajorId = student.MajorId;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) // Concurrency conflict
        {
            if (!StudentExists(studentToUpdate.CardId))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return studentToUpdate.ToStudentDto();
    }

    // DELETE: api/Students/{cardId}
    [HttpDelete("{cardId}")]
    public async Task<ActionResult<StudentDto>> DeleteStudent(string cardId)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s => s.CardId == cardId);
        if (student == null)
        {
            return NotFound();
        }

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    private bool StudentExists(string cardId)
    {
        return _context.Students.Any(e => e.CardId == cardId);
    }
}