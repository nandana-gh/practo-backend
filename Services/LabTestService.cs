using Microsoft.EntityFrameworkCore;
using practo_backend.Data;
using practo_backend.DTOs;
using practo_backend.Models;

namespace practo_backend.Services
{
    public interface ILabTestService
    {
        Task<LabTestsLandingDto> GetLandingDataAsync();
    }

    public class LabTestService : ILabTestService
    {
        private readonly ApplicationDbContext _context;

        public LabTestService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LabTestsLandingDto> GetLandingDataAsync()
        {
            var tests = _context.DiagnosticTests.ToList();
            var packages = _context.HealthCheckupPackages.ToList();
            var concerns = _context.HealthConcerns.ToList();
            var vitals = _context.VitalCheckups.ToList();

            return new LabTestsLandingDto
            {
                TopBookedTests = tests,
                PopularPackages = packages,
                HealthConcerns = concerns,
                RecommendedVitalCheckups = vitals
            };
        }
    }
}
