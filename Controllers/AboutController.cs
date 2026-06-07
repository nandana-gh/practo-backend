using Microsoft.AspNetCore.Mvc;
using PractoBackend.DTOs;
using System.Collections.Generic;

namespace PractoBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AboutController : ControllerBase
    {
        [HttpGet("testimonials")]
        public ActionResult<IEnumerable<TestimonialDto>> GetTestimonials()
        {
            var testimonials = new List<TestimonialDto>
            {
                new TestimonialDto { 
                    Quote = "For any new doctor, the most important thing is to let people know that you have started a clinic and Practice. And the best medium for that is Practo! It has been an amazing journey since I connected with Practo.", 
                    AuthorName = "Dr. Manan Vora", 
                    AuthorTitle = "Orthopedist, Mumbai",
                    AvatarUrl = "https://upload.wikimedia.org/wikipedia/commons/7/7c/Profile_avatar_placeholder_large.png"
                },
                new TestimonialDto { 
                    Quote = "Practo has completely revolutionized how patients find me. Their technology is seamless and easy to use.", 
                    AuthorName = "Dr. Anjali Kumar", 
                    AuthorTitle = "Gynecologist, Delhi",
                    AvatarUrl = "https://upload.wikimedia.org/wikipedia/commons/7/7c/Profile_avatar_placeholder_large.png"
                }
            };

            return Ok(testimonials);
        }
    }
}
