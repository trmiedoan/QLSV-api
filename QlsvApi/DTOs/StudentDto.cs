using System;
using System.Collections.Generic;

using QlsvApi.Models;

public partial class StudentDto
{
    // public int Id { get; set; }

    public string CardId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    public string Gender { get; set; } = null!;

    public int MajorId { get; set; }

    // public virtual Major Major { get; set; } = null!;
    public string? MajorCode { get; set; } = null!;

    public string? MajorName { get; set; } = null!;
}
