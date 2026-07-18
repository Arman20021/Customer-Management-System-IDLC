using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.DAL.Data;
using CMS.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMS.DAL.Repositories;

public class GetAllCustomersRepository
{
    private readonly CmsDbContext context;

    public GetAllCustomersRepository(CmsDbContext context)
    {
        this.context = context;
    }


    public (
        List<Customer> Customers,
        int TotalRecords
    ) GetAllCustomers(
        int pageNumber,
        int pageSize,
        string? search,
        string? status,
        string sortBy,
        string sortOrder)
    {
        IQueryable<Customer> query =
            context.Customers
                .AsNoTracking()
                .Include(customer => customer.CustomerMobiles)
                .Include(customer => customer.CustomerAddresses)
                .Include(customer => customer.CustomerDocuments)
                .AsSplitQuery();


        // Search customers by name.
        if (!string.IsNullOrWhiteSpace(search))
        {
            string searchValue = search.Trim();

            query = query.Where(customer =>
                customer.CustomerName.Contains(searchValue));
        }


        // Filter customers by status.
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(customer =>
                customer.Status == status);
        }


        bool descending = sortOrder.Equals(
            "desc",
            StringComparison.OrdinalIgnoreCase);


        // Sort the customer records.
        switch (sortBy.ToLower())
        {
            case "customername":

                query = descending
                    ? query
                        .OrderByDescending(customer =>
                            customer.CustomerName)
                        .ThenByDescending(customer =>
                            customer.CustomerId)
                    : query
                        .OrderBy(customer =>
                            customer.CustomerName)
                        .ThenBy(customer =>
                            customer.CustomerId);

                break;


            case "emailaddress":

                query = descending
                    ? query
                        .OrderByDescending(customer =>
                            customer.EmailAddress)
                        .ThenByDescending(customer =>
                            customer.CustomerId)
                    : query
                        .OrderBy(customer =>
                            customer.EmailAddress)
                        .ThenBy(customer =>
                            customer.CustomerId);

                break;


            case "dateofbirth":

                query = descending
                    ? query
                        .OrderByDescending(customer =>
                            customer.DateOfBirth)
                        .ThenByDescending(customer =>
                            customer.CustomerId)
                    : query
                        .OrderBy(customer =>
                            customer.DateOfBirth)
                        .ThenBy(customer =>
                            customer.CustomerId);

                break;


            case "status":

                query = descending
                    ? query
                        .OrderByDescending(customer =>
                            customer.Status)
                        .ThenByDescending(customer =>
                            customer.CustomerId)
                    : query
                        .OrderBy(customer =>
                            customer.Status)
                        .ThenBy(customer =>
                            customer.CustomerId);

                break;


            case "createddate":

                query = descending
                    ? query
                        .OrderByDescending(customer =>
                            customer.CreatedDate)
                        .ThenByDescending(customer =>
                            customer.CustomerId)
                    : query
                        .OrderBy(customer =>
                            customer.CreatedDate)
                        .ThenBy(customer =>
                            customer.CustomerId);

                break;


            case "lastupdateddate":

                query = descending
                    ? query
                        .OrderByDescending(customer =>
                            customer.LastUpdatedDate)
                        .ThenByDescending(customer =>
                            customer.CustomerId)
                    : query
                        .OrderBy(customer =>
                            customer.LastUpdatedDate)
                        .ThenBy(customer =>
                            customer.CustomerId);

                break;


            default:

                query = descending
                    ? query.OrderByDescending(customer =>
                        customer.CustomerId)
                    : query.OrderBy(customer =>
                        customer.CustomerId);

                break;
        }


        // Count records before pagination.
        int totalRecords = query.Count();


        // Apply pagination.
        List<Customer> customers = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();


        return (customers, totalRecords);
    }
}