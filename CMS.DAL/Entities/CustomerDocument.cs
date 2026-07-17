using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CMS.DAL.Entities;

[Index("CustomerId", Name = "IX_CustomerDocuments_CustomerId")]
public partial class CustomerDocument
{
    [Key]
    public int CustomerDocumentId { get; set; }

    public int CustomerId { get; set; }

    [StringLength(50)]
    public string DocumentType { get; set; } = null!;

    [StringLength(250)]
    public string OriginalFileName { get; set; } = null!;

    [StringLength(255)]
    public string StoredFileName { get; set; } = null!;

    [StringLength(500)]
    public string FilePath { get; set; } = null!;

    [StringLength(100)]
    public string ContentType { get; set; } = null!;

    public long FileSizeBytes { get; set; }

    public DateTime UploadedDate { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("CustomerDocuments")]
    public virtual Customer Customer { get; set; } = null!;
}
