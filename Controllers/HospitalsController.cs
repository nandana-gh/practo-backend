using Microsoft.AspNetCore.Mvc;
using PractoBackend.DTOs;
using System.Collections.Generic;

namespace PractoBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HospitalsController : ControllerBase
    {
        [HttpGet("customers")]
        public ActionResult<IEnumerable<CustomerDto>> GetCustomers()
        {
            var customers = new List<CustomerDto>
            {
                new CustomerDto { Name = "NMC", LogoUrl = "nmc" },
                new CustomerDto { Name = "VPS Healthcare", LogoUrl = "vps" },
                new CustomerDto { Name = "Wockhardt Hospitals", LogoUrl = "wockhardt" },
                new CustomerDto { Name = "Tree Top", LogoUrl = "treetop" }
            };

            return Ok(customers);
        }
    }
}
