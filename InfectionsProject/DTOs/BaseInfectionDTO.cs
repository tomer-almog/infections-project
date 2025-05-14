namespace InfectionsProject.DTOs; 

public class BaseInfectionDTO {
    public long PatientIdentificationNumber { get; set; }
    public string Name { get; set; } = null!;
}