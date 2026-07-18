using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.DAL.Data;
using CMS.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMS.DAL.Repositories;

public class CustomerRepository
{
    private readonly CmsDbContext context;

    public CustomerRepository(CmsDbContext context)
    {
        this.context = context;
    }

    public bool EmailExists(string emailAddress)
    {
        return context.Customers
            .Any(customer =>
                customer.EmailAddress == emailAddress);
    }

    public bool NationalIdExists(string nationalIdNumber)
    {
        return context.Customers
            .Any(customer =>
                customer.NationalIdNumber == nationalIdNumber);
    }

    public string? FindExistingMobile(List<string> mobileNumbers)
    {
        return context.CustomerMobiles
            .Where(mobile =>
                mobileNumbers.Contains(mobile.MobileNumber))
            .Select(mobile => mobile.MobileNumber)
            .FirstOrDefault();
    }

    public int CreateCustomer(Customer customer)
    {
        context.Customers.Add(customer);

        context.SaveChanges();

        return customer.CustomerId;
    }
}