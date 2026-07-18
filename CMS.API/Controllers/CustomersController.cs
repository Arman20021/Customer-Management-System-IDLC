using CMS.BLL.DTOs.Customers;
using CMS.BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace CMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly CustomerService customerService;

    public CustomersController(CustomerService customerService)
    {
        this.customerService = customerService;
    }

    [HttpPost]
    public IActionResult CreateCustomer(
        CreateCustomerRequest request)
    {
        try
        {
            CreateCustomerResponse response =
                customerService.CreateCustomer(request);

            return Created(
                $"/api/customers/{response.CustomerId}",
                response);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new
            {
                message = exception.Message
            });
        }
        catch (Exception)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message = "An unexpected server error occurred."
                });
        }
    }

    [HttpGet("{id:int}")]
    public IActionResult GetCustomerById(int id)
    {
        try
        {
            GetCustomerResponse? customer =
                customerService.GetCustomerById(id);

            if (customer == null)
            {
                return NotFound(new
                {
                    message =
                        $"Customer with ID {id} was not found."
                });
            }

            return Ok(customer);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
        catch (Exception)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message =
                        "An unexpected server error occurred."
                });
        }
    }
}