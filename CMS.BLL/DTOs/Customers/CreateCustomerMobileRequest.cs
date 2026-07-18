using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CMS.BLL.DTOs.Customers;

public class CreateCustomerMobileRequest
{
    [Required(ErrorMessage = "Mobile number is required.")]
    [StringLength(
        11,
        ErrorMessage = "Mobile number cannot exceed 50 characters.")]
    public string MobileNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mobile type is required.")]
    [StringLength(20,ErrorMessage = "Mobile type cannot exceed 20 characters.")]
    public string MobileType { get; set; } = string.Empty;
}