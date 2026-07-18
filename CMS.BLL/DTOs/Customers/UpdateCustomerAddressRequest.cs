using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CMS.BLL.DTOs.Customers;

public class UpdateCustomerAddressRequest
{
    public int? CustomerAddressId { get; set; }

    [Required(ErrorMessage = "Address type is required.")]
    [StringLength(20)]
    public string AddressType { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required.")]
    [StringLength(500)]
    public string AddressText { get; set; } = string.Empty;
}