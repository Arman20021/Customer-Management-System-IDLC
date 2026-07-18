using CMS.BLL.DTOs.Customers;
using CMS.BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace CMS.API.Controllers;

[ApiController]
[Route("api/customer-management")]
public class CustomersController : ControllerBase
{
    private readonly CustomerService customerService;
    private readonly GetAllCustomersService getAllCustomersService;
    private readonly UpdateCustomerService updateCustomerService;

    public CustomersController(
        CustomerService customerService,
        GetAllCustomersService getAllCustomersService,
        UpdateCustomerService updateCustomerService)
    {
        this.customerService = customerService;

        this.getAllCustomersService =
            getAllCustomersService;

        this.updateCustomerService =
            updateCustomerService;
    }

    [HttpPost("create-customer")]
    public IActionResult CreateCustomer(
        CreateCustomerRequest request)
    {
        try
        {
            CreateCustomerResponse response =
                customerService.CreateCustomer(request);

            return Created(
                $"/api/customer-management/get-customer-by-id/{response.CustomerId}",
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

    [HttpGet("get-customer-by-id/{id:int}")]
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

    [HttpPost("get-all-customers")]
    public IActionResult GetAllCustomers(
    [FromQuery] GetAllCustomersRequest request)
    {
        try
        {
            PagedCustomerResponse response =
                getAllCustomersService.GetAllCustomers(request);

            return Ok(response);
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

    [HttpPut("update-customer/{id:int}")]
    public IActionResult UpdateCustomer(
    int id,
    [FromBody] UpdateCustomerRequest request)
    {
        try
        {
            UpdateCustomerResponse? response =
                updateCustomerService.UpdateCustomer(
                    id,
                    request);

            if (response == null)
            {
                return NotFound(new
                {
                    message =
                        $"Customer with ID {id} was not found."
                });
            }

            return Ok(response);
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
                    message =
                        "An unexpected server error occurred."
                });
        }
    }
}