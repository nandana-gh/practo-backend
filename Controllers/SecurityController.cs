using Microsoft.AspNetCore.Mvc;
using PractoBackend.DTOs;
using System.Collections.Generic;

namespace PractoBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SecurityController : ControllerBase
    {
        [HttpGet("faqs")]
        public ActionResult<IEnumerable<FaqDto>> GetFaqs()
        {
            var faqs = new List<FaqDto>
            {
                new FaqDto { Question = "What is Practo's stance on Data privacy?", Answer = "We believe your health data is exactly that - yours. We implement industry-leading standards to ensure your privacy." },
                new FaqDto { Question = "Are my records secure?", Answer = "Yes, your records are encrypted with 256-bit encryption and stored securely." },
                new FaqDto { Question = "Can doctors see my personal health records?", Answer = "Only the doctors you explicitly share your records with or consult can view your health data." },
                new FaqDto { Question = "Is Practo HIPAA compliant?", Answer = "Yes, our systems are designed in compliance with HIPAA guidelines for health data security." },
                new FaqDto { Question = "How do you protect against data breaches?", Answer = "We employ continuous monitoring, regular penetration testing, and strict access controls." },
                new FaqDto { Question = "Does Practo sell patient data?", Answer = "No, Practo does not sell patient health data to third-party marketers or data brokers." },
                new FaqDto { Question = "How is my payment information secured?", Answer = "All payment transactions are processed through PCI-DSS compliant gateways." },
                new FaqDto { Question = "Can I request to delete my data?", Answer = "Yes, users can request data deletion in accordance with our privacy policy and applicable laws." },
                new FaqDto { Question = "What is ISO 27001 certification?", Answer = "ISO 27001 is a global standard for information security management systems, which Practo has achieved." },
                new FaqDto { Question = "How are communications between patient and doctor secured?", Answer = "All communications, including chat and video consultations, are end-to-end encrypted." },
                new FaqDto { Question = "Who has access to the servers where data is stored?", Answer = "Only highly restricted, authorized personnel have access to the production servers via multi-factor authentication." },
                new FaqDto { Question = "What happens in case of a server failure?", Answer = "We maintain real-time automated backups across multiple geographic zones to ensure no data is lost." },
                new FaqDto { Question = "Is the Practo Pro app secure for doctors?", Answer = "Yes, the Practo Pro app employs the same rigorous security standards as our patient applications." },
                new FaqDto { Question = "What if my doctor's phone is lost or stolen?", Answer = "Practo Pro uses token-based authentication and biometric locks. Sessions can be revoked remotely." },
                new FaqDto { Question = "Where are Practo's servers located?", Answer = "Our data is hosted on secure cloud infrastructure located in compliant data centers." }
            };

            return Ok(faqs);
        }
    }
}
