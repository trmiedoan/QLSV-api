using System;
using System.Collections.Generic;

using QlsvApi.Models;

public partial class StudentListDto
{
    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalPages { get; set; }

    public int TotalCount { get; set; }

    public List<StudentDto> Students { get; set; } = null!;
}
