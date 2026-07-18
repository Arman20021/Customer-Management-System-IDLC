using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.BLL.DTOs.Customers;
using CMS.DAL.Entities;
using CMS.DAL.Repositories;

namespace CMS.BLL.Services;

public class CustomerService
{
    private readonly CustomerRepository customerRepository;

    private readonly string[] allowedMobileTypes =
    {
        "Primary",
        "Alternate",
        "Office",
        "Home",
        "Other"
    };

    private readonly string[] allowedAddressTypes =
    {
        "Present",
        "Permanent",
        "Mailing",
        "Office",
        "Other"
    };

    public CustomerService(CustomerRepository customerRepository)
    {
        this.customerRepository = customerRepository;
    }

    public CreateCustomerResponse CreateCustomer(
        CreateCustomerRequest request)
    {
        ValidateDateOfBirth(request.DateOfBirth);
        ValidateMobileNumbers(request.MobileNumbers);
        ValidateAddresses(request.Addresses);

        string customerName = request.CustomerName.Trim();
        string emailAddress = request.EmailAddress.Trim();
        string nationalIdNumber = request.NationalIdNumber.Trim();

        bool emailExists =
            customerRepository.EmailExists(emailAddress);

        if (emailExists)
        {
            throw new InvalidOperationException(
                "A customer with this email address already exists.");
        }

        bool nationalIdExists =
            customerRepository.NationalIdExists(nationalIdNumber);

        if (nationalIdExists)
        {
            throw new InvalidOperationException(
                "A customer with this National ID number already exists.");
        }

        List<string> requestedMobileNumbers = request.MobileNumbers
            .Select(mobile => mobile.MobileNumber.Trim())
            .ToList();

        string? existingMobile =
            customerRepository.FindExistingMobile(
                requestedMobileNumbers);

        if (existingMobile != null)
        {
            throw new InvalidOperationException(
                $"The mobile number '{existingMobile}' already exists.");
        }

        DateTime currentDate = DateTime.UtcNow;

        Customer customer = new Customer
        {
            CustomerName = customerName,
            EmailAddress = emailAddress,
            DateOfBirth = request.DateOfBirth,
            NationalIdNumber = nationalIdNumber,
            Status = "Active",
            CreatedDate = currentDate,
            LastUpdatedDate = currentDate,

            CustomerMobiles = request.MobileNumbers
                .Select(mobile => new CustomerMobile
                {
                    MobileNumber = mobile.MobileNumber.Trim(),

                    MobileType = GetValidMobileType(
                        mobile.MobileType)
                })
                .ToList(),

            CustomerAddresses = request.Addresses
                .Select(address => new CustomerAddress
                {
                    AddressType = GetValidAddressType(
                        address.AddressType),

                    AddressText = address.AddressText.Trim()
                })
                .ToList()
        };

        int customerId =
            customerRepository.CreateCustomer(customer);

        return new CreateCustomerResponse
        {
            CustomerId = customerId,
            Message = "Customer created successfully."
        };
    }

    private void ValidateDateOfBirth(DateOnly? dateOfBirth)
    {
        if (dateOfBirth == null)
        {
            return;
        }

        DateOnly currentDate =
            DateOnly.FromDateTime(DateTime.UtcNow);

        if (dateOfBirth.Value > currentDate)
        {
            throw new ArgumentException(
                "Date of birth cannot be a future date.");
        }
    }

    private void ValidateMobileNumbers(
        List<CreateCustomerMobileRequest>? mobileNumbers)
    {
        if (mobileNumbers == null || mobileNumbers.Count == 0)
        {
            throw new ArgumentException(
                "At least one mobile number is required.");
        }

        foreach (CreateCustomerMobileRequest mobile in mobileNumbers)
        {
            if (string.IsNullOrWhiteSpace(mobile.MobileNumber))
            {
                throw new ArgumentException(
                    "Mobile number is required.");
            }

            if (string.IsNullOrWhiteSpace(mobile.MobileType))
            {
                throw new ArgumentException(
                    "Mobile type is required.");
            }

            bool validMobileType = allowedMobileTypes.Any(type =>
                type.Equals(
                    mobile.MobileType.Trim(),
                    StringComparison.OrdinalIgnoreCase));

            if (!validMobileType)
            {
                throw new ArgumentException(
                    $"Invalid mobile type '{mobile.MobileType}'. " +
                    "Allowed values are Primary, Alternate, " +
                    "Office, Home and Other.");
            }
        }

        int primaryMobileCount = mobileNumbers.Count(mobile =>
            mobile.MobileType.Trim().Equals(
                "Primary",
                StringComparison.OrdinalIgnoreCase));

        if (primaryMobileCount != 1)
        {
            throw new ArgumentException(
                "Exactly one primary mobile number is required.");
        }

        bool containsDuplicateNumbers = mobileNumbers
            .GroupBy(
                mobile => mobile.MobileNumber.Trim(),
                StringComparer.OrdinalIgnoreCase)
            .Any(group => group.Count() > 1);

        if (containsDuplicateNumbers)
        {
            throw new ArgumentException(
                "The request contains duplicate mobile numbers.");
        }
    }

    private void ValidateAddresses(
        List<CreateCustomerAddressRequest>? addresses)
    {
        if (addresses == null)
        {
            return;
        }

        foreach (CreateCustomerAddressRequest address in addresses)
        {
            if (string.IsNullOrWhiteSpace(address.AddressType))
            {
                throw new ArgumentException(
                    "Address type is required.");
            }

            if (string.IsNullOrWhiteSpace(address.AddressText))
            {
                throw new ArgumentException(
                    "Address is required.");
            }

            bool validAddressType = allowedAddressTypes.Any(type =>
                type.Equals(
                    address.AddressType.Trim(),
                    StringComparison.OrdinalIgnoreCase));

            if (!validAddressType)
            {
                throw new ArgumentException(
                    $"Invalid address type '{address.AddressType}'. " +
                    "Allowed values are Present, Permanent, " +
                    "Mailing, Office and Other.");
            }
        }
    }

    private string GetValidMobileType(string mobileType)
    {
        return allowedMobileTypes.First(type =>
            type.Equals(
                mobileType.Trim(),
                StringComparison.OrdinalIgnoreCase));
    }

    private string GetValidAddressType(string addressType)
    {
        return allowedAddressTypes.First(type =>
            type.Equals(
                addressType.Trim(),
                StringComparison.OrdinalIgnoreCase));
    }
}