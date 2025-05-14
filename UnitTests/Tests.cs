using RestSharp;
using System.Net;
using System.Text.Json;
using UnitTests.DTOs;

namespace UnitTests;

public class Tests {
    private RestClient _client;
    private const string BASE_URL = "http://localhost:5000";
    
    [SetUp]
    public void Setup() {
        _client = new RestClient(BASE_URL);
    }

    [Test]
    public async Task GetInfections_GET_ExpectsAtLeastOneInfection() {
        // Arrange
        var fetchInfectionRequest = GetCreateInfectionRequest();
        await _client.ExecuteAsync(fetchInfectionRequest);
        var fetchInfectionsRequest = new RestRequest("/api/infections?timestamp=2009-06-15T13:45:30");

        // Execute
        var fetchInfectionsResponse = await _client.ExecuteAsync(fetchInfectionsRequest);

        // Assert
        var fetchInfectionsResponseDTO = 
            JsonSerializer.Deserialize<List<ExistingInfectionResponseDTO>>(fetchInfectionsResponse.Content);

        Assert.That(fetchInfectionsResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(fetchInfectionsResponseDTO.Count, Is.GreaterThan(1));
        // Should validate every field in the DTO
    }
    
    [Test]
    public async Task GetActiveInfections_GET_ExpectsAtLeastOneInfection() {
        // Arrange
        var fetchActiveInfectionsRequest = GetCreateInfectionRequest();
        await _client.ExecuteAsync(fetchActiveInfectionsRequest);
        var fetchInfectionsRequest = 
            new RestRequest("/api/infections?timestamp=2009-06-15T13:45:30&infectionStatus=Infected");

        // Execute
        var fetchInfectionsResponse = await _client.ExecuteAsync(fetchInfectionsRequest);

        // Assert
        var fetchInfectionsResponseDTO = 
            JsonSerializer.Deserialize<List<ExistingInfectionResponseDTO>>(fetchInfectionsResponse.Content);

        Assert.That(fetchInfectionsResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(fetchInfectionsResponseDTO.Count, Is.GreaterThan(1));
        Assert.That(fetchInfectionsResponseDTO, Is.All.Matches<ExistingInfectionResponseDTO>(
            infection => infection.InfectionStatus == InfectionStatus.Infected));
        // Should validate every field in the DTO
    }
    
    [Test]
    public async Task UpdateOfInfection_PUT_ExpectsNoContent() {
        // Arrange
        var infectionId = await CreateInfectionAndGetId();
        var putUri = $"/api/infections/{infectionId}";
        
        // Execute
        var updateInfectionResponse = await UpdateInfectionAndGetResponse(putUri);

        // Assert
        Assert.That(updateInfectionResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task CreationOfInfection_POST_ExpectsCreated() {
        // Arrange
        var createInfectionRequest = GetCreateInfectionRequest();

        // Execute
        var createInfectionResponse = await _client.ExecuteAsync(createInfectionRequest);

        // Assert
        Assert.That(createInfectionResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }
    
    // Missing tests: verify non-2xx responses, verify different inputs, ...

    private static RestRequest GetCreateInfectionRequest() {
        RestRequest createInfectionRequest = new RestRequest("/api/infections", Method.Post);
        createInfectionRequest.AddBody(new NewInfectionDTO { Name = "Carolin", PatientIdentificationNumber = 123456789 });
        return createInfectionRequest;
    }

    private async Task<Guid> CreateInfectionAndGetId() {
        var createInfectionRequest = GetCreateInfectionRequest();
        var createInfectionResponse = await _client.ExecuteAsync(createInfectionRequest);
        Assert.That(createInfectionResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var createInfectionResponseDTO = JsonSerializer.Deserialize<NewInfectionResponseDTO>(createInfectionResponse.Content);

        if (createInfectionResponseDTO.InfectionId == null || createInfectionResponseDTO.InfectionId == Guid.Empty) {
            throw new Exception("Unable to parse POST response body as a valid GUID");
        }

        return createInfectionResponseDTO.InfectionId;
    }
    
    private async Task<RestResponse> UpdateInfectionAndGetResponse(string putUri) {
        var updateInfectionRequest = new RestRequest(putUri, Method.Put);
        updateInfectionRequest.AddBody(new UpdateInfectionDTO { InfectionStatus = "Healed" });
        return await _client.ExecuteAsync(updateInfectionRequest);
    }
}