using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CMS.BLL.DTOs.Customers;
using CMS.DAL.Repositories;

namespace CMS.BLL.Services;

public class DeleteCustomerService
{
    private readonly DeleteCustomerRepository
        deleteCustomerRepository;

    public DeleteCustomerService(
        DeleteCustomerRepository deleteCustomerRepository)
    {
        this.deleteCustomerRepository =
            deleteCustomerRepository;
    }

    public DeleteCustomerResponse? DeleteCustomer(
        int customerId)
    {
        ValidateCustomerId(customerId);

        bool deleted =
            deleteCustomerRepository.DeleteCustomer(
                customerId);

        if (!deleted)
        {
            return null;
        }

        return new DeleteCustomerResponse
        {
            CustomerId = customerId,

            Message =
                "Customer deleted successfully."
        };
    }

    private static void ValidateCustomerId(
        int customerId)
    {
        if (customerId <= 0)
        {
            throw new ArgumentException(
                "Customer ID must be greater than zero.");
        }
    }
}
