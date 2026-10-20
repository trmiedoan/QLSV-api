using System;
using System.Collections.Generic;
using QlsvApi.Models;

public partial class CreateStudentDto
{

    public string CardId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    public string Gender { get; set; } = null!;

    public int MajorId { get; set; }

    // public virtual Major Major { get; set; } = null!;

    public Student ToStudent()
    {
        return new Student
        {
            CardId = CardId,
            Name = Name,
            BirthDate = BirthDate,
            Gender = Gender,
            MajorId = MajorId
        };
    }
}
