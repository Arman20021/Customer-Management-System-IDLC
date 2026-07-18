using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.BLL.DTOs.Customers;

public class PagedCustomerResponse
{
    public List<CustomerListItemResponse> Items { get; set; }
        = new List<CustomerListItemResponse>();

    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalRecords { get; set; }

    public int TotalPages { get; set; }

    public bool HasPreviousPage { get; set; }

    public bool HasNextPage { get; set; }
}