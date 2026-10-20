using System;
using System.Collections.Generic;

namespace QlsvApi.Models;

public partial class Student
{
    public int Id { get; set; }

    public string CardId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    public string Gender { get; set; } = null!;

    public int MajorId { get; set; }

    public virtual Major Major { get; set; } = null!; // null! means that the property is required

    public StudentDto ToStudentDto()
    {
        return new StudentDto
        {
            CardId = CardId,
            Name = Name,
            BirthDate = BirthDate,
            Gender = Gender,
            MajorId = MajorId,
            MajorCode = Major.MajorCode,
            MajorName = Major.Name
        };
    }
    
}
