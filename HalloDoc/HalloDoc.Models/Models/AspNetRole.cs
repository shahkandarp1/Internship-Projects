using System;
using System.Collections.Generic;

namespace HalloDoc;

public partial class AspNetRole
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<AspNetUserRole> AspNetUserRoles { get; set; } = new List<AspNetUserRole>();
}
