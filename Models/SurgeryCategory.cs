namespace practo_backend.Models;

public class SurgeryCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<SurgeryTreatment> Treatments { get; set; } = new List<SurgeryTreatment>();
}
