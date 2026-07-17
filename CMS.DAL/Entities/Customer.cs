using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CMS.DAL.Entities;

[Index("CustomerName", Name = "IX_Customers_CustomerName")]
[Index("Status", Name = "IX_Customers_Status")]
[Index("EmailAddress", Name = "UX_Customers_EmailAddress", IsUnique = true)]
[Index("NationalIdNumber", Name = "UX_Customers_NationalIdNumber", IsUnique = true)]
public partial class Customer
{
    [Key]
    public int CustomerId { get; set; }

    [StringLength(150)]
    public string CustomerName { get; set; } = null!;

    [StringLength(255)]
    public string EmailAddress { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    [StringLength(50)]
    public string NationalIdNumber { get; set; } = null!;

    [StringLength(20)]
    public string Status { get; set; } = null!;

    [Precision(0)]
    public DateTime CreatedDate { get; set; }

    [Precision(0)]
    public DateTime LastUpdatedDate { get; set; }

    [InverseProperty("Customer")]
    public virtual ICollection<CustomerAddress> CustomerAddresses { get; set; } = new List<CustomerAddress>();

    [InverseProperty("Customer")]
    public virtual ICollection<CustomerDocument> CustomerDocuments { get; set; } = new List<CustomerDocument>();

    [InverseProperty("Customer")]
    public virtual ICollection<CustomerMobile> CustomerMobiles { get; set; } = new List<CustomerMobile>();
}
