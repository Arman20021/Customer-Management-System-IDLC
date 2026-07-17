using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CMS.DAL.Entities;

[Index("CustomerId", Name = "IX_CustomerMobiles_CustomerId")]
[Index("MobileNumber", Name = "UX_CustomerMobiles_MobileNumber", IsUnique = true)]
public partial class CustomerMobile
{
    [Key]
    public int CustomerMobileId { get; set; }

    public int CustomerId { get; set; }

    [StringLength(50)]
    public string MobileNumber { get; set; } = null!;

    [StringLength(20)]
    public string MobileType { get; set; } = null!;

    [ForeignKey("CustomerId")]
    [InverseProperty("CustomerMobiles")]
    public virtual Customer Customer { get; set; } = null!;
}
