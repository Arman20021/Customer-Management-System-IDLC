using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
 

namespace CMS.BLL.DTOs.Customers;

public class CreateCustomerRequest
{
    [Required(ErrorMessage = "Customer name is required.")]
    [StringLength(
        150,
        ErrorMessage = "Customer name cannot exceed 150 characters.")]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Email address is not valid.")]
    [StringLength(
        255,
        ErrorMessage = "Email address cannot exceed 255 characters.")]
    public string EmailAddress { get; set; } = string.Empty;

    public DateOnly? DateOfBirth { get; set; }

    [Required(ErrorMessage = "National ID number is required.")]
    [StringLength(
        50,
        ErrorMessage = "National ID number cannot exceed 50 characters.")]
    public string NationalIdNumber { get; set; } = string.Empty;

    [MinLength(
        1,
        ErrorMessage = "At least one mobile number is required.")]
    public List<CreateCustomerMobileRequest> MobileNumbers { get; set; }
        = new();

    public List<CreateCustomerAddressRequest> Addresses { get; set; }
        = new();
}
