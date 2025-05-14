using System.Text.Json.Serialization;

namespace UnitTests.DTOs; 

public class NewInfectionResponseDTO {
    [JsonPropertyName("infectionId")]
    public Guid InfectionId { get; set; }
}