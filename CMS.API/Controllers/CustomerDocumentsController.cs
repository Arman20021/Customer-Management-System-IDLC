using Microsoft.AspNetCore.Mvc;

using CMS.API.Models.CustomerDocuments;
using CMS.BLL.DTOs.CustomerDocuments;
using CMS.BLL.Services;

namespace CMS.API.Controllers;

[ApiController]
[Route("api/customer-management/{customerId:int}/documents")]
public class CustomerDocumentsController : ControllerBase
{
    private readonly UploadCustomerDocumentService
        uploadCustomerDocumentService;

    public CustomerDocumentsController(
        UploadCustomerDocumentService
            uploadCustomerDocumentService)
    {
        this.uploadCustomerDocumentService =
            uploadCustomerDocumentService;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public IActionResult UploadCustomerDocument(
        int customerId,
        [FromForm] UploadCustomerDocumentRequest request)
    {
        try
        {
            if (request.File == null)
            {
                return BadRequest(new
                {
                    message =
                        "A document file is required."
                });
            }

            using Stream fileStream =
                request.File.OpenReadStream();

            UploadCustomerDocumentResponse? response =
                uploadCustomerDocumentService
                    .UploadCustomerDocument(
                        customerId,
                        request.DocumentType,
                        request.File.FileName,
                        request.File.ContentType,
                        request.File.Length,
                        fileStream);

            if (response == null)
            {
                return NotFound(new
                {
                    message =
                        $"Customer with ID {customerId} " +
                        "was not found."
                });
            }

            return StatusCode(
                StatusCodes.Status201Created,
                response);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                message =
                    exception.Message
            });
        }
        catch (Exception)
        {
            return StatusCode(
                StatusCodes
                    .Status500InternalServerError,
                new
                {
                    message =
                        "An unexpected server error occurred."
                });
        }
    }
}



//for document -individual