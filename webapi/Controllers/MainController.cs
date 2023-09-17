using Microsoft.AspNetCore.Mvc;
using webapi;
using webapi.Models;

namespace webapi.Controllers;

[ApiController]
[Route("MainController")]
public class MainController : ControllerBase
{
    private readonly WebADAMDBRepo _repository;

    public MainController(WebADAMDBRepo repository)
    {
        _repository = repository;
    }

    [HttpGet ("device")]
    public ActionResult<IEnumerable<Device>> GetDevice()
    {
        var data = _repository.GetDevice();
        return Ok(data);
    }

    [HttpGet("dataSet")]
    public ActionResult<IEnumerable<DataSet>> GetDataSet()
    {
        var data = _repository.GetDataSet();
        return Ok(data);
    }

    [HttpGet("dataLoader")]
    public ActionResult<IEnumerable<DataLoader>> GetDataLoader()
    {
        var data = _repository.GetDataLoader();
        return Ok(data);
    }

    // Implement other controller actions as needed
}
