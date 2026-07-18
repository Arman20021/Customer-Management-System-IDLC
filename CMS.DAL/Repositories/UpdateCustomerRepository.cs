using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.DAL.Data;
using CMS.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMS.DAL.Repositories;

public class UpdateCustomerRepository
{
    private readonly CmsDbContext context;

    public UpdateCustomerRepository(CmsDbContext context)
    {
        this.context = context;
    }

    public Customer? GetCustomerForUpdate(int customerId)
    {
        return context.Customers
            .Include(customer => customer.CustomerMobiles)
            .Include(customer => customer.CustomerAddresses)
            .FirstOrDefault(customer =>
                customer.CustomerId == customerId);
    }

    public bool EmailExistsForOtherCustomer(
        string emailAddress,
        int customerId)
    {
        return context.Customers.Any(customer =>
            customer.EmailAddress == emailAddress
            && customer.CustomerId != customerId);
    }

    public bool NationalIdExistsForOtherCustomer(
        string nationalIdNumber,
        int customerId)
    {
        return context.Customers.Any(customer =>
            customer.NationalIdNumber == nationalIdNumber
            && customer.CustomerId != customerId);
    }

    public string? FindExistingMobileForOtherCustomer(
        List<string> mobileNumbers,
        int customerId)
    {
        return context.CustomerMobiles
            .Where(mobile =>
                mobileNumbers.Contains(mobile.MobileNumber)
                && mobile.CustomerId != customerId)
            .Select(mobile => mobile.MobileNumber)
            .FirstOrDefault();
    }

    public void UpdateCustomer(
        Customer customer,
        List<CustomerMobile> requestedMobiles,
        List<CustomerAddress> requestedAddresses)
    {
        
        foreach (
            CustomerMobile existingMobile
            in customer.CustomerMobiles.ToList())
        {
            CustomerMobile? requestedMobile =
                requestedMobiles.FirstOrDefault(mobile =>
                    mobile.CustomerMobileId ==
                    existingMobile.CustomerMobileId);

            if (requestedMobile == null)
            {
                context.CustomerMobiles.Remove(
                    existingMobile);

                continue;
            }

            existingMobile.MobileNumber =
                requestedMobile.MobileNumber;

            existingMobile.MobileType =
                requestedMobile.MobileType;
        }


 
        foreach (
            CustomerMobile requestedMobile
            in requestedMobiles.Where(mobile =>
                mobile.CustomerMobileId == 0))
        {
            customer.CustomerMobiles.Add(
                new CustomerMobile
                {
                    MobileNumber =
                        requestedMobile.MobileNumber,

                    MobileType =
                        requestedMobile.MobileType
                });
        }


        
        foreach (
            CustomerAddress existingAddress
            in customer.CustomerAddresses.ToList())
        {
            CustomerAddress? requestedAddress =
                requestedAddresses.FirstOrDefault(address =>
                    address.CustomerAddressId ==
                    existingAddress.CustomerAddressId);

            if (requestedAddress == null)
            {
                context.CustomerAddresses.Remove(
                    existingAddress);

                continue;
            }

            existingAddress.AddressType =
                requestedAddress.AddressType;

            existingAddress.AddressText =
                requestedAddress.AddressText;
        }


      
        foreach (
            CustomerAddress requestedAddress
            in requestedAddresses.Where(address =>
                address.CustomerAddressId == 0))
        {
            customer.CustomerAddresses.Add(
                new CustomerAddress
                {
                    AddressType =
                        requestedAddress.AddressType,

                    AddressText =
                        requestedAddress.AddressText
                });
        }


        context.SaveChanges();
    }
}