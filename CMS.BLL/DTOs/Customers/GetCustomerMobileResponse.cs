using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.BLL.DTOs.Customers;

public class GetCustomerMobileResponse
{
    public int CustomerMobileId { get; set; }

    public string MobileNumber { get; set; } = string.Empty;

    public string MobileType { get; set; } = string.Empty;
}