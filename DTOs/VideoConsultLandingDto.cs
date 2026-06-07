namespace practo_backend.DTOs;

public class VideoConsultLandingDto
{
    public List<SpecialtyCardDto> Specialties { get; set; } = new();
    public List<DoctorCardDto> Doctors { get; set; } = new();
    public VideoConsultStatsDto Stats { get; set; } = new();
}

public class SpecialtyCardDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public decimal StartingPrice { get; set; } // Mocked or calculated
}

public class DoctorCardDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public string ProfileImageUrl { get; set; } = string.Empty;
}

public class VideoConsultStatsDto
{
    public string HappyPatients { get; set; } = "2,00,000+";
    public string VerifiedDoctors { get; set; } = "20,000+";
    public string SpecialtiesCount { get; set; } = "25+";
    public string AppRating { get; set; } = "4.5/5";
}
