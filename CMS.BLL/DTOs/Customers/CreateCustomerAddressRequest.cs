using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CMS.BLL.DTOs.Customers;

public class CreateCustomerAddressRequest
{
    [Required(ErrorMessage = "Address type is required.")]
    [StringLength(
        20,
        ErrorMessage = "Address type cannot exceed 20 characters.")]
    public string AddressType { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required.")]
    [StringLength(
        500,
        ErrorMessage = "Address cannot exceed 500 characters.")]
    public string AddressText { get; set; } = string.Empty;
}