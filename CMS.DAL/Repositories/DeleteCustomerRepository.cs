using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CMS.DAL.Data;
using CMS.DAL.Entities;

namespace CMS.DAL.Repositories;

public class DeleteCustomerRepository
{
    private readonly CmsDbContext context;

    public DeleteCustomerRepository(
        CmsDbContext context)
    {
        this.context = context;
    }

    public bool DeleteCustomer(int customerId)
    {
        Customer? customer =
            context.Customers.FirstOrDefault(customer =>
                customer.CustomerId == customerId);

        if (customer == null)
        {
            return false;
        }

        context.Customers.Remove(customer);

        context.SaveChanges();

        return true;
    }
}