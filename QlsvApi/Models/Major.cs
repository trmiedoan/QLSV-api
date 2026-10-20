using System;
using System.Collections.Generic;

namespace QlsvApi.Models;

public partial class Major
{
    public int Id { get; set; }

    public string MajorCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public MajorDto ToMajorDto()
    {
        return new MajorDto
        {
            Id = Id,
            MajorCode = MajorCode,
            Name = Name
        };
    }
}
