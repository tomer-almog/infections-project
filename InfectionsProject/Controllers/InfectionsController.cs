using System.Globalization;
using System.Runtime.InteropServices;
using InfectionsProject.DTOs;
using InfectionsProject.Exceptions;
using InfectionsProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfectionsProject.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InfectionsController : Controller {
    private readonly InfectionsContext _context;
    private const string DateTimePattern = "yyyy-MM-dd'T'HH:mm:ss.FFFK";

    public InfectionsController(InfectionsContext context) {
        _context = context;
    }

    // Get infections by timestamp and optionally by infection status
    // Expected timestamp format: ISO 8601 (e.g.: 2009-06-15T13:45:30)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Infection>>> GetInfections([FromQuery] string timestamp,
        [FromQuery] [Optional] string? infectionStatus) {
        try {
            var infections = GetInfectionsFromDB(timestamp, infectionStatus);
            return Ok(infections);
        }
        catch (InvalidTimestampException) {
            return BadRequest("GET request must include valid timestamp in ISO 8601 format");
        }
        catch (InvalidInfectionStatusException) {
            return BadRequest("GET request did not include a valid InfectionStatus");
        }
        catch (Exception) {
            // Log error
            return Problem("There was an internal issue with your request", null, 500);
        }
    }

    // Update infection status by infectionId
    [HttpPut("{infectionId}")]
    public async Task<IActionResult> PutInfectionStatus(Guid infectionId, UpdateInfectionDTO updateInfectionDTO) {
        try {
            await UpdateInfectionInDB(infectionId, updateInfectionDTO);
            return NoContent();
        }
        catch (InfectionNotFoundException) {
            return NotFound();
        }
        catch (InvalidInfectionStatusException) {
            return BadRequest("PUT request must include valid InfectionStatus");
        }
        catch (DBSaveException) {
            return Problem("Unable to update infection", null, 500);
        }
        catch (Exception) {
            // Log error
            return Problem("There was an internal issue with your request", null, 500);
        }
    }

    // Create new infection
    [HttpPost]
    public async Task<ActionResult<BaseInfectionDTO>> PostInfection(NewInfectionDTO newInfectionDTO) {
        try {
            var infectionId = await SaveNewInfectionToDB(newInfectionDTO);
            return CreatedAtAction("PostInfection", new { id = infectionId }, 
                new {infectionId = infectionId.ToString()});
        }
        catch (Exception) {
            // Log error
            return Problem("There was an internal issue with your request", null, 500);
        }
    }

    // TODO: Move all business logic out of the controller into a dedicated business logic project
    private async Task<Guid> SaveNewInfectionToDB(NewInfectionDTO newInfectionDTO) {
        var infection = new Infection {
            PatientIdentificationNumber = newInfectionDTO.PatientIdentificationNumber,
            InfectionId = Guid.NewGuid(),
            InfectionStatus = InfectionStatus.Infected,
            CreatedAt = DateTime.Now
        };
        _context.Infections.Add(infection);

        await _context.SaveChangesAsync();

        return infection.InfectionId;
    }
    
    // TODO: Move all business logic out of the controller into a dedicated business logic project
    private List<Infection> GetInfectionsFromDB(string timestampDTO, string? infectionStatusDTO) {
        var isTimestampValid = DateTime.TryParseExact(timestampDTO, DateTimePattern, null,
            DateTimeStyles.None, out DateTime timestamp);
        
        if (!isTimestampValid) {
            throw new InvalidTimestampException();
        }
        
        var infections = new List<Infection>();

        if (!string.IsNullOrEmpty(infectionStatusDTO)) {
            var (isInfectionStatusValid, infectionStatus) = TryParseInfectionStatus(infectionStatusDTO);

            if (!isInfectionStatusValid) {
                throw new InvalidInfectionStatusException();
            }

            infections = _context.Infections
                .Where(infection => infection.CreatedAt >= timestamp && infection.InfectionStatus == infectionStatus)
                .ToList();

            return infections;
        }

        infections = _context.Infections
            .Where(infection => infection.CreatedAt >= timestamp)
            .ToList();
        
        return infections;
    }
    
    // TODO: Move all business logic out of the controller into a dedicated business logic project
    private async Task UpdateInfectionInDB(Guid infectionId, UpdateInfectionDTO updateInfectionDTO) {
        // Should use async method for actual DB
        var existingInfection = _context.Infections
            .FirstOrDefault(infection => infection.InfectionId == infectionId);

        if (existingInfection == null) {
            throw new InfectionNotFoundException();
        }

        var (isInfectionStatusValid, infectionStatus) = TryParseInfectionStatus(updateInfectionDTO.InfectionStatus);

        if (!isInfectionStatusValid) {
            throw new InvalidInfectionStatusException();
        }

        existingInfection.InfectionStatus = infectionStatus;
        existingInfection.UpdatedAt = DateTime.Now;

        try {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) {
            // Add logging/handle concurrency issue
            throw new DBSaveException();
        }
    }

    private (bool isInfectionStatusValid, InfectionStatus infectionStatus) TryParseInfectionStatus(
        string infectionStatusDTO) {
        var isInfectionStatusValid = Enum.TryParse(infectionStatusDTO, out InfectionStatus infectionStatus);
        return (isInfectionStatusValid, infectionStatus);
    }
}