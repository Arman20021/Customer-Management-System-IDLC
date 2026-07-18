using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.DAL.Data;
using CMS.DAL.Entities;

namespace CMS.DAL.Repositories;

public class UploadCustomerDocumentRepository
{
    private readonly CmsDbContext context;

    public UploadCustomerDocumentRepository(
        CmsDbContext context)
    {
        this.context = context;
    }

    public bool CustomerExists(int customerId)
    {
        return context.Customers.Any(customer =>
            customer.CustomerId == customerId);
    }

    public int CreateDocument(
        CustomerDocument customerDocument)
    {
        context.CustomerDocuments.Add(
            customerDocument);

        context.SaveChanges();

        return customerDocument.CustomerDocumentId;
    }
}