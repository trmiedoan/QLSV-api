using System;
using System.Collections.Generic;

using QlsvApi.Models;

public partial class UpdateStudentDto
{
    // CardID can't be updated by the client so we remove it from the DTO
    //    public string CardId { get; set; } = null!;
    public string Name { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    public string Gender { get; set; } = null!;

    public int MajorId { get; set; }

    // public virtual Major Major { get; set; } = null!;
}
