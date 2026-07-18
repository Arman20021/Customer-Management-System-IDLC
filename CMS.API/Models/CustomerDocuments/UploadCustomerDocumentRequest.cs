using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CMS.API.Models.CustomerDocuments;

public class UploadCustomerDocumentRequest
{
    [Required(ErrorMessage = "Document type is required.")]
    public string DocumentType { get; set; } = string.Empty;

    [Required(ErrorMessage = "A document file is required.")]
    public IFormFile? File { get; set; }
}