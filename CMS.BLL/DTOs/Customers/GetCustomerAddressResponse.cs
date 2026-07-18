using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.BLL.DTOs.Customers;

public class GetCustomerAddressResponse
{
    public int CustomerAddressId { get; set; }

    public string AddressType { get; set; } = string.Empty;

    public string AddressText { get; set; } = string.Empty;
}
