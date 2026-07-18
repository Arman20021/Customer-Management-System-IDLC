using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.BLL.DTOs.CustomerDocuments;

public class UploadCustomerDocumentResponse
{
    public int CustomerDocumentId { get; set; }

    public int CustomerId { get; set; }

    public string DocumentType { get; set; } = string.Empty;

    public string OriginalFileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long FileSizeBytes { get; set; }

    public DateTime UploadedDate { get; set; }

    public string Message { get; set; } = string.Empty;
}