using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.BLL.DTOs.CustomerDocuments;
using CMS.DAL.Entities;
using CMS.DAL.Repositories;

namespace CMS.BLL.Services;

public class UploadCustomerDocumentService
{
    private readonly UploadCustomerDocumentRepository
        uploadCustomerDocumentRepository;

    private readonly string[] allowedDocumentTypes =
    {
        "NID",
        "TaxCertificate",
        "Photo",
        "Signature",
        "Other"
    };

    private readonly string[] allowedExtensions =
    {
        ".pdf",
        ".jpg",
        ".jpeg",
        ".png"
    };

    private const long MaximumFileSizeBytes =
        5 * 1024 * 1024;

    public UploadCustomerDocumentService(
        UploadCustomerDocumentRepository
            uploadCustomerDocumentRepository)
    {
        this.uploadCustomerDocumentRepository =
            uploadCustomerDocumentRepository;
    }

    public UploadCustomerDocumentResponse?
        UploadCustomerDocument(
            int customerId,
            string documentType,
            string originalFileName,
            string contentType,
            long fileSizeBytes,
            Stream uploadedFileStream)
    {
        ValidateCustomerId(customerId);

        bool customerExists =
            uploadCustomerDocumentRepository
                .CustomerExists(customerId);

        if (!customerExists)
        {
            return null;
        }

        string validDocumentType =
            GetValidDocumentType(documentType);

        string safeOriginalFileName =
            Path.GetFileName(originalFileName);

        ValidateFile(
            safeOriginalFileName,
            fileSizeBytes,
            uploadedFileStream);

        string extension =
            Path.GetExtension(
                safeOriginalFileName)
            .ToLowerInvariant();

        string storedFileName =
            $"{Guid.NewGuid():N}{extension}";

        string folderPath =
            Path.Combine(
                AppContext.BaseDirectory,
                "Uploads",
                "Customers",
                customerId.ToString());

        Directory.CreateDirectory(folderPath);

        string physicalFilePath =
            Path.Combine(
                folderPath,
                storedFileName);

        string relativeFilePath =
            Path.Combine(
                    "Uploads",
                    "Customers",
                    customerId.ToString(),
                    storedFileName)
                .Replace("\\", "/");

        DateTime uploadedDate =
            DateTime.UtcNow;

        try
        {
            using FileStream outputStream =
                new FileStream(
                    physicalFilePath,
                    FileMode.Create,
                    FileAccess.Write);

            uploadedFileStream.CopyTo(
                outputStream);

            CustomerDocument customerDocument =
                new CustomerDocument
                {
                    CustomerId =
                        customerId,

                    DocumentType =
                        validDocumentType,

                    OriginalFileName =
                        safeOriginalFileName,

                    StoredFileName =
                        storedFileName,

                    FilePath =
                        relativeFilePath,

                    ContentType =
                        string.IsNullOrWhiteSpace(
                            contentType)
                            ? "application/octet-stream"
                            : contentType,

                    FileSizeBytes =
                        fileSizeBytes,

                    UploadedDate =
                        uploadedDate
                };

            int customerDocumentId =
                uploadCustomerDocumentRepository
                    .CreateDocument(
                        customerDocument);

            return new UploadCustomerDocumentResponse
            {
                CustomerDocumentId =
                    customerDocumentId,

                CustomerId =
                    customerId,

                DocumentType =
                    validDocumentType,

                OriginalFileName =
                    safeOriginalFileName,

                ContentType =
                    customerDocument.ContentType
                    ?? "application/octet-stream",

                FileSizeBytes =
                    fileSizeBytes,

                UploadedDate =
                    uploadedDate,

                Message =
                    "Customer document uploaded successfully."
            };
        }
        catch
        {
            if (System.IO.File.Exists(
                    physicalFilePath))
            {
                System.IO.File.Delete(
                    physicalFilePath);
            }

            throw;
        }
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

    private string GetValidDocumentType(
        string documentType)
    {
        if (string.IsNullOrWhiteSpace(
                documentType))
        {
            throw new ArgumentException(
                "Document type is required.");
        }

        string? validDocumentType =
            allowedDocumentTypes
                .FirstOrDefault(type =>
                    type.Equals(
                        documentType.Trim(),
                        StringComparison.OrdinalIgnoreCase));

        if (validDocumentType == null)
        {
            throw new ArgumentException(
                "Invalid document type. Allowed values are " +
                "NID, TaxCertificate, Photo, Signature " +
                "and Other.");
        }

        return validDocumentType;
    }

    private void ValidateFile(
        string originalFileName,
        long fileSizeBytes,
        Stream uploadedFileStream)
    {
        if (string.IsNullOrWhiteSpace(
                originalFileName))
        {
            throw new ArgumentException(
                "File name is required.");
        }

        if (originalFileName.Length > 255)
        {
            throw new ArgumentException(
                "File name cannot exceed 255 characters.");
        }

        if (fileSizeBytes <= 0)
        {
            throw new ArgumentException(
                "The uploaded file is empty.");
        }

        if (fileSizeBytes >
            MaximumFileSizeBytes)
        {
            throw new ArgumentException(
                "The uploaded file cannot exceed 5 MB.");
        }

        if (!uploadedFileStream.CanRead)
        {
            throw new ArgumentException(
                "The uploaded file cannot be read.");
        }

        string extension =
            Path.GetExtension(
                originalFileName)
            .ToLowerInvariant();

        bool validExtension =
            allowedExtensions.Contains(
                extension);

        if (!validExtension)
        {
            throw new ArgumentException(
                "Invalid file type. Allowed file types are " +
                "PDF, JPG, JPEG and PNG.");
        }
    }
}