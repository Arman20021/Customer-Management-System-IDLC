using System;
using System.Collections.Generic;
using System.Linq;
using CMS.BLL.DTOs.Customers;
using CMS.DAL.Entities;
using CMS.DAL.Repositories;

namespace CMS.BLL.Services;

public class UpdateCustomerService
{
    private readonly UpdateCustomerRepository updateCustomerRepository;

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

    public UpdateCustomerService(
        UpdateCustomerRepository updateCustomerRepository)
    {
        this.updateCustomerRepository =
            updateCustomerRepository;
    }

    public UpdateCustomerResponse? UpdateCustomer(
        int customerId,
        UpdateCustomerRequest request)
    {
        ValidateCustomerId(customerId);

        Customer? customer =
            updateCustomerRepository.GetCustomerForUpdate(
                customerId);

        if (customer == null)
        {
            return null;
        }

        ValidateBasicInformation(request);

        ValidateDateOfBirth(
            request.DateOfBirth);

        string status =
            GetValidStatus(request.Status);

        ValidateMobileNumbers(
            request.MobileNumbers);

        ValidateAddresses(
            request.Addresses);

        ValidateChildIds(
            customer,
            request.MobileNumbers,
            request.Addresses);

        string customerName =
            request.CustomerName.Trim();

        string emailAddress =
            request.EmailAddress.Trim();

        string nationalIdNumber =
            request.NationalIdNumber.Trim();

        CheckDuplicateEmail(
            emailAddress,
            customerId);

        CheckDuplicateNationalId(
            nationalIdNumber,
            customerId);

        List<CustomerMobile> requestedMobileNumbers =
            CreateMobileList(
                request.MobileNumbers,
                customerId);

        CheckDuplicateMobilesInDatabase(
            requestedMobileNumbers,
            customerId);

        List<CustomerAddress> requestedAddresses =
            CreateAddressList(
                request.Addresses,
                customerId);

        customer.CustomerName =
            customerName;

        customer.EmailAddress =
            emailAddress;

        customer.DateOfBirth =
            request.DateOfBirth;

        customer.NationalIdNumber =
            nationalIdNumber;

        customer.Status =
            status;

        customer.LastUpdatedDate =
            DateTime.UtcNow;

        updateCustomerRepository.UpdateCustomer(
            customer,
            requestedMobileNumbers,
            requestedAddresses);

        return new UpdateCustomerResponse
        {
            CustomerId = customer.CustomerId,

            Message =
                "Customer updated successfully."
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

    private static void ValidateBasicInformation(
        UpdateCustomerRequest request)
    {
        if (string.IsNullOrWhiteSpace(
                request.CustomerName))
        {
            throw new ArgumentException(
                "Customer name is required.");
        }

        if (string.IsNullOrWhiteSpace(
                request.EmailAddress))
        {
            throw new ArgumentException(
                "Email address is required.");
        }

        if (string.IsNullOrWhiteSpace(
                request.NationalIdNumber))
        {
            throw new ArgumentException(
                "National ID number is required.");
        }

        if (string.IsNullOrWhiteSpace(
                request.Status))
        {
            throw new ArgumentException(
                "Customer status is required.");
        }
    }

    private static void ValidateDateOfBirth(
        DateOnly? dateOfBirth)
    {
        if (dateOfBirth == null)
        {
            return;
        }

        DateOnly currentDate =
            DateOnly.FromDateTime(
                DateTime.UtcNow);

        if (dateOfBirth.Value > currentDate)
        {
            throw new ArgumentException(
                "Date of birth cannot be a future date.");
        }
    }

    private static string GetValidStatus(
        string status)
    {
        string trimmedStatus =
            status.Trim();

        if (trimmedStatus.Equals(
                "Active",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Active";
        }

        if (trimmedStatus.Equals(
                "Inactive",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Inactive";
        }

        throw new ArgumentException(
            "Invalid status. Allowed values are " +
            "Active and Inactive.");
    }

    private void CheckDuplicateEmail(
        string emailAddress,
        int customerId)
    {
        bool emailExists =
            updateCustomerRepository
                .EmailExistsForOtherCustomer(
                    emailAddress,
                    customerId);

        if (emailExists)
        {
            throw new InvalidOperationException(
                "A customer with this email address " +
                "already exists.");
        }
    }

    private void CheckDuplicateNationalId(
        string nationalIdNumber,
        int customerId)
    {
        bool nationalIdExists =
            updateCustomerRepository
                .NationalIdExistsForOtherCustomer(
                    nationalIdNumber,
                    customerId);

        if (nationalIdExists)
        {
            throw new InvalidOperationException(
                "A customer with this National ID number " +
                "already exists.");
        }
    }

    private void ValidateMobileNumbers(
        List<UpdateCustomerMobileRequest>? mobileNumbers)
    {
        if (mobileNumbers == null ||
            mobileNumbers.Count == 0)
        {
            throw new ArgumentException(
                "At least one mobile number is required.");
        }

        foreach (
            UpdateCustomerMobileRequest mobile
            in mobileNumbers)
        {
            if (string.IsNullOrWhiteSpace(
                    mobile.MobileNumber))
            {
                throw new ArgumentException(
                    "Mobile number is required.");
            }

            if (string.IsNullOrWhiteSpace(
                    mobile.MobileType))
            {
                throw new ArgumentException(
                    "Mobile type is required.");
            }

            bool validMobileType =
                allowedMobileTypes.Any(type =>
                    type.Equals(
                        mobile.MobileType.Trim(),
                        StringComparison.OrdinalIgnoreCase));

            if (!validMobileType)
            {
                throw new ArgumentException(
                    $"Invalid mobile type " +
                    $"'{mobile.MobileType}'. " +
                    "Allowed values are Primary, Alternate, " +
                    "Office, Home and Other.");
            }

            if (mobile.CustomerMobileId.HasValue &&
                mobile.CustomerMobileId.Value < 0)
            {
                throw new ArgumentException(
                    "Customer mobile ID cannot be negative.");
            }
        }

        int primaryMobileCount =
            mobileNumbers.Count(mobile =>
                mobile.MobileType
                    .Trim()
                    .Equals(
                        "Primary",
                        StringComparison.OrdinalIgnoreCase));

        if (primaryMobileCount != 1)
        {
            throw new ArgumentException(
                "Exactly one primary mobile number is required.");
        }

        bool containsDuplicateNumbers =
            mobileNumbers
                .GroupBy(
                    mobile =>
                        mobile.MobileNumber.Trim(),
                    StringComparer.OrdinalIgnoreCase)
                .Any(group =>
                    group.Count() > 1);

        if (containsDuplicateNumbers)
        {
            throw new ArgumentException(
                "The request contains duplicate mobile numbers.");
        }

        bool containsDuplicateMobileIds =
            mobileNumbers
                .Where(mobile =>
                    mobile.CustomerMobileId.HasValue &&
                    mobile.CustomerMobileId.Value > 0)
                .GroupBy(mobile =>
                    mobile.CustomerMobileId!.Value)
                .Any(group =>
                    group.Count() > 1);

        if (containsDuplicateMobileIds)
        {
            throw new ArgumentException(
                "The request contains duplicate mobile IDs.");
        }
    }

    private void ValidateAddresses(
        List<UpdateCustomerAddressRequest>? addresses)
    {
        if (addresses == null)
        {
            throw new ArgumentException(
                "Addresses are required.");
        }

        foreach (
            UpdateCustomerAddressRequest address
            in addresses)
        {
            if (string.IsNullOrWhiteSpace(
                    address.AddressType))
            {
                throw new ArgumentException(
                    "Address type is required.");
            }

            if (string.IsNullOrWhiteSpace(
                    address.AddressText))
            {
                throw new ArgumentException(
                    "Address is required.");
            }

            bool validAddressType =
                allowedAddressTypes.Any(type =>
                    type.Equals(
                        address.AddressType.Trim(),
                        StringComparison.OrdinalIgnoreCase));

            if (!validAddressType)
            {
                throw new ArgumentException(
                    $"Invalid address type " +
                    $"'{address.AddressType}'. " +
                    "Allowed values are Present, Permanent, " +
                    "Mailing, Office and Other.");
            }

            if (address.CustomerAddressId.HasValue &&
                address.CustomerAddressId.Value < 0)
            {
                throw new ArgumentException(
                    "Customer address ID cannot be negative.");
            }
        }

        bool containsDuplicateAddressIds =
            addresses
                .Where(address =>
                    address.CustomerAddressId.HasValue &&
                    address.CustomerAddressId.Value > 0)
                .GroupBy(address =>
                    address.CustomerAddressId!.Value)
                .Any(group =>
                    group.Count() > 1);

        if (containsDuplicateAddressIds)
        {
            throw new ArgumentException(
                "The request contains duplicate address IDs.");
        }
    }

    private static void ValidateChildIds(
        Customer customer,
        List<UpdateCustomerMobileRequest> mobileNumbers,
        List<UpdateCustomerAddressRequest> addresses)
    {
        HashSet<int> existingMobileIds =
            customer.CustomerMobiles
                .Select(mobile =>
                    mobile.CustomerMobileId)
                .ToHashSet();

        int? invalidMobileId =
            mobileNumbers
                .Where(mobile =>
                    mobile.CustomerMobileId.HasValue &&
                    mobile.CustomerMobileId.Value > 0)
                .Select(mobile =>
                    mobile.CustomerMobileId)
                .FirstOrDefault(id =>
                    !existingMobileIds.Contains(
                        id!.Value));

        if (invalidMobileId.HasValue)
        {
            throw new ArgumentException(
                $"Mobile ID {invalidMobileId.Value} " +
                "does not belong to customer " +
                $"{customer.CustomerId}.");
        }

        HashSet<int> existingAddressIds =
            customer.CustomerAddresses
                .Select(address =>
                    address.CustomerAddressId)
                .ToHashSet();

        int? invalidAddressId =
            addresses
                .Where(address =>
                    address.CustomerAddressId.HasValue &&
                    address.CustomerAddressId.Value > 0)
                .Select(address =>
                    address.CustomerAddressId)
                .FirstOrDefault(id =>
                    !existingAddressIds.Contains(
                        id!.Value));

        if (invalidAddressId.HasValue)
        {
            throw new ArgumentException(
                $"Address ID {invalidAddressId.Value} " +
                "does not belong to customer " +
                $"{customer.CustomerId}.");
        }
    }

    private List<CustomerMobile> CreateMobileList(
        List<UpdateCustomerMobileRequest> mobileNumbers,
        int customerId)
    {
        return mobileNumbers
            .Select(mobile =>
                new CustomerMobile
                {
                    CustomerMobileId =
                        mobile.CustomerMobileId
                            .GetValueOrDefault() > 0
                            ? mobile.CustomerMobileId.Value
                            : 0,

                    CustomerId =
                        customerId,

                    MobileNumber =
                        mobile.MobileNumber.Trim(),

                    MobileType =
                        GetValidMobileType(
                            mobile.MobileType)
                })
            .ToList();
    }

    private List<CustomerAddress> CreateAddressList(
        List<UpdateCustomerAddressRequest> addresses,
        int customerId)
    {
        return addresses
            .Select(address =>
                new CustomerAddress
                {
                    CustomerAddressId =
                        address.CustomerAddressId
                            .GetValueOrDefault() > 0
                            ? address.CustomerAddressId.Value
                            : 0,

                    CustomerId =
                        customerId,

                    AddressType =
                        GetValidAddressType(
                            address.AddressType),

                    AddressText =
                        address.AddressText.Trim()
                })
            .ToList();
    }

    private void CheckDuplicateMobilesInDatabase(
        List<CustomerMobile> mobileNumbers,
        int customerId)
    {
        List<string> requestedNumbers =
            mobileNumbers
                .Select(mobile =>
                    mobile.MobileNumber)
                .ToList();

        string? existingMobile =
            updateCustomerRepository
                .FindExistingMobileForOtherCustomer(
                    requestedNumbers,
                    customerId);

        if (existingMobile != null)
        {
            throw new InvalidOperationException(
                $"The mobile number '{existingMobile}' " +
                "already belongs to another customer.");
        }
    }

    private string GetValidMobileType(
        string mobileType)
    {
        return allowedMobileTypes.First(type =>
            type.Equals(
                mobileType.Trim(),
                StringComparison.OrdinalIgnoreCase));
    }

    private string GetValidAddressType(
        string addressType)
    {
        return allowedAddressTypes.First(type =>
            type.Equals(
                addressType.Trim(),
                StringComparison.OrdinalIgnoreCase));
    }
}