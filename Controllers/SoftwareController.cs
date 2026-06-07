using Microsoft.AspNetCore.Mvc;
using PractoBackend.DTOs;
using System.Collections.Generic;

namespace PractoBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SoftwareController : ControllerBase
    {
        [HttpGet("faqs")]
        public ActionResult<IEnumerable<FaqDto>> GetFaqs()
        {
            var faqs = new List<FaqDto>
            {
                new FaqDto { Question = "What is Practo Ray?", Answer = "Practo Ray is an award-winning clinic management software used by thousands of doctors." },
                new FaqDto { Question = "How do I print prescriptions?", Answer = "Practo Ray allows you to customize and print prescriptions effortlessly." }
            };

            return Ok(faqs);
        }
    }
}
