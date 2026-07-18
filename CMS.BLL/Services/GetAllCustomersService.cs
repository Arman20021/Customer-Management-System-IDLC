using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.BLL.DTOs.Customers;
using CMS.DAL.Entities;
using CMS.DAL.Repositories;

namespace CMS.BLL.Services;

public class GetAllCustomersService
{
    private readonly GetAllCustomersRepository
        getAllCustomersRepository;

    public GetAllCustomersService(
        GetAllCustomersRepository getAllCustomersRepository)
    {
        this.getAllCustomersRepository =
            getAllCustomersRepository;
    }


    public PagedCustomerResponse GetAllCustomers(
        GetAllCustomersRequest request)
    {
        string? search = null;
        string? status = null;


        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            search = request.Search.Trim();
        }


        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            status = GetValidStatus(request.Status);
        }


        string sortBy =
            GetValidSortBy(request.SortBy);

        string sortOrder =
            GetValidSortOrder(request.SortOrder);


        (
            List<Customer> customers,
            int totalRecords
        ) = getAllCustomersRepository.GetAllCustomers(
            request.PageNumber,
            request.PageSize,
            search,
            status,
            sortBy,
            sortOrder);


        List<CustomerListItemResponse> customerItems =
            customers
                .Select(customer =>
                    new CustomerListItemResponse
                    {
                        CustomerId = customer.CustomerId,

                        CustomerName = customer.CustomerName,

                        EmailAddress = customer.EmailAddress,

                        DateOfBirth = customer.DateOfBirth,

                        NationalIdNumber =
                            customer.NationalIdNumber,

                        Status = customer.Status,

                        CreatedDate = customer.CreatedDate,

                        LastUpdatedDate =
                            customer.LastUpdatedDate,

                        MobileNumbers = customer.CustomerMobiles
                            .OrderBy(mobile =>
                                mobile.CustomerMobileId)
                            .Select(mobile =>
                                new GetCustomerMobileResponse
                                {
                                    CustomerMobileId =
                                        mobile.CustomerMobileId,

                                    MobileNumber =
                                        mobile.MobileNumber,

                                    MobileType =
                                        mobile.MobileType
                                })
                            .ToList(),

                        Addresses = customer.CustomerAddresses
                            .OrderBy(address =>
                                address.CustomerAddressId)
                            .Select(address =>
                                new GetCustomerAddressResponse
                                {
                                    CustomerAddressId =
                                        address.CustomerAddressId,

                                    AddressType =
                                        address.AddressType,

                                    AddressText =
                                        address.AddressText
                                })
                            .ToList(),

                        Documents = customer.CustomerDocuments
                            .OrderBy(document =>
                                document.CustomerDocumentId)
                            .Select(document =>
                                new GetCustomerDocumentResponse
                                {
                                    CustomerDocumentId =
                                        document.CustomerDocumentId,

                                    DocumentType =
                                        document.DocumentType,

                                    OriginalFileName =
                                        document.OriginalFileName,

                                    ContentType =
                                        document.ContentType,

                                    FileSizeBytes =
                                        document.FileSizeBytes,

                                    UploadedDate =
                                        document.UploadedDate
                                })
                            .ToList()
                    })
                .ToList();


        int totalPages = (int)Math.Ceiling(
            totalRecords / (double)request.PageSize);


        return new PagedCustomerResponse
        {
            Items = customerItems,

            PageNumber = request.PageNumber,

            PageSize = request.PageSize,

            TotalRecords = totalRecords,

            TotalPages = totalPages,

            HasPreviousPage =
                request.PageNumber > 1,

            HasNextPage =
                request.PageNumber < totalPages
        };
    }


    private static string GetValidStatus(string status)
    {
        if (status.Equals(
                "Active",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Active";
        }

        if (status.Equals(
                "Inactive",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Inactive";
        }

        throw new ArgumentException(
            "Invalid status. Allowed values are Active and Inactive.");
    }


    private static string GetValidSortBy(string sortBy)
    {
        string[] allowedSortFields =
        {
            "CustomerId",
            "CustomerName",
            "EmailAddress",
            "DateOfBirth",
            "Status",
            "CreatedDate",
            "LastUpdatedDate"
        };


        string? validSortField =
            allowedSortFields.FirstOrDefault(field =>
                field.Equals(
                    sortBy,
                    StringComparison.OrdinalIgnoreCase));


        if (validSortField == null)
        {
            throw new ArgumentException(
                "Invalid sort field. Allowed values are " +
                "CustomerId, CustomerName, EmailAddress, " +
                "DateOfBirth, Status, CreatedDate and " +
                "LastUpdatedDate.");
        }


        return validSortField;
    }


    private static string GetValidSortOrder(
        string sortOrder)
    {
        if (sortOrder.Equals(
                "asc",
                StringComparison.OrdinalIgnoreCase))
        {
            return "asc";
        }


        if (sortOrder.Equals(
                "desc",
                StringComparison.OrdinalIgnoreCase))
        {
            return "desc";
        }


        throw new ArgumentException(
            "Invalid sort order. Allowed values are asc and desc.");
    }
}