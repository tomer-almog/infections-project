using System.Text.Json.Serialization;

namespace UnitTests.DTOs; 

public class ExistingInfectionResponseDTO {
    [JsonPropertyName("patientIdentificationNumber")]
    public long PatientIdentificationNumber { get; set; }
    [JsonPropertyName("infectionId")]
    public Guid InfectionId { get; set; }
    [JsonPropertyName("infectionStatus")]
    public InfectionStatus InfectionStatus { get; set; }
    // More properties ...
}