using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

 

namespace CMS.BLL.DTOs.Customers;

public class GetAllCustomersRequest
{
    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "Page number must be greater than zero.")]
    public int PageNumber { get; set; } = 1;


    [Range(
        1,
        100,
        ErrorMessage = "Page size must be between 1 and 100.")]
    public int PageSize { get; set; } = 10;


    public string? Search { get; set; }


    public string? Status { get; set; }


    public string SortBy { get; set; } = "CustomerId";


    public string SortOrder { get; set; } = "asc";
}