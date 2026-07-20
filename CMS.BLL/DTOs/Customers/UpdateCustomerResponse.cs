using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.BLL.DTOs.Customers;

public class UpdateCustomerResponse
{
    public int CustomerId { get; set; }

    public string Message { get; set; } = string.Empty;
}



//for customer update responce-arman