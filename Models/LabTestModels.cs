namespace practo_backend.Models
{
    public class DiagnosticTest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string KnownAs { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal OriginalPrice { get; set; }
    }

    public class HealthCheckupPackage
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int DiscountPercent { get; set; }
        public int IncludedTestsCount { get; set; }
        public string IdealFor { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal OriginalPrice { get; set; }
    }

    public class HealthConcern
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
    }

    public class VitalCheckup
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
