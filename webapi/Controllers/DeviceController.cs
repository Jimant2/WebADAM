using Microsoft.AspNetCore.Mvc;
using webapi;
using webapi.Models;

namespace webapi.Controllers;

[ApiController]
[Route("[controller]")]
public class DeviceController : ControllerBase
{
    private readonly WebADAMDBRepo _repository;

    public DeviceController(WebADAMDBRepo repository)
    {
        _repository = repository;
    }

    [HttpGet (Name ="GetDevice")]
    public ActionResult<IEnumerable<Device>> GetDevice()
    {
        var data = _repository.GetAll();
        return Ok(data);
    }

    // Implement other controller actions as needed
}
