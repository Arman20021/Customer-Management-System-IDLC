using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CMS.DAL.Entities;

[Index("CustomerId", Name = "IX_CustomerAddresses_CustomerId")]
public partial class CustomerAddress
{
    [Key]
    public int CustomerAddressId { get; set; }

    public int CustomerId { get; set; }

    [StringLength(20)]
    public string AddressType { get; set; } = null!;

    [StringLength(500)]
    public string AddressText { get; set; } = null!;

    [ForeignKey("CustomerId")]
    [InverseProperty("CustomerAddresses")]
    public virtual Customer Customer { get; set; } = null!;
}
