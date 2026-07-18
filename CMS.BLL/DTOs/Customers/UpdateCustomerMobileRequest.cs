using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace CMS.BLL.DTOs.Customers;

public class UpdateCustomerMobileRequest
{
    public int? CustomerMobileId { get; set; }

    [Required(ErrorMessage = "Mobile number is required.")]
    [StringLength(50)]
    public string MobileNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mobile type is required.")]
    [StringLength(20)]
    public string MobileType { get; set; } = string.Empty;
}