using practo_backend.Models;

namespace practo_backend.DTOs
{
    public class LabTestsLandingDto
    {
        public List<DiagnosticTest> TopBookedTests { get; set; } = new();
        public List<HealthCheckupPackage> PopularPackages { get; set; } = new();
        public List<HealthConcern> HealthConcerns { get; set; } = new();
        public List<VitalCheckup> RecommendedVitalCheckups { get; set; } = new();
    }
}
