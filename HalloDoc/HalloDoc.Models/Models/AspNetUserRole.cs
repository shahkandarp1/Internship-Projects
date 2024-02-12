using System;
using System.Collections.Generic;

namespace HalloDoc;

public partial class AspNetUserRole
{
    public int UserId { get; set; }

    public int RoleId { get; set; }

    public int Id { get; set; }

    public virtual AspNetRole Role { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
