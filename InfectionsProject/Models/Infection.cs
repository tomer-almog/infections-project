namespace InfectionsProject.Models; 

public class Infection {
    public long PatientIdentificationNumber { get; set; }
    public Guid InfectionId { get; set; }
    public InfectionStatus InfectionStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}