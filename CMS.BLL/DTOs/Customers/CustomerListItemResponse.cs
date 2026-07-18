using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.BLL.DTOs.Customers;

public class CustomerListItemResponse
{
    public int CustomerId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public string EmailAddress { get; set; } = string.Empty;

    public DateOnly? DateOfBirth { get; set; }

    public string NationalIdNumber { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    public DateTime LastUpdatedDate { get; set; }

    public List<GetCustomerMobileResponse> MobileNumbers { get; set; }
        = new List<GetCustomerMobileResponse>();

    public List<GetCustomerAddressResponse> Addresses { get; set; }
        = new List<GetCustomerAddressResponse>();

    public List<GetCustomerDocumentResponse> Documents { get; set; }
        = new List<GetCustomerDocumentResponse>();
}