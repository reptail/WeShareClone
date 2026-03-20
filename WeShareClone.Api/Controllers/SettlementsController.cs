using Microsoft.AspNetCore.Mvc;
using WeShareClone.Api.Domain.Models;
using WeShareClone.Api.Domain.Repositories;
using WeShareClone.Api.Dto.Entries;
using WeShareClone.Api.Dto.Settlements;
using WeShareClone.Api.Extensions;

namespace WeShareClone.Api.Controllers;

[ApiController]
[Route("settlements")]
public class SettlementsController(
    ISettlementRepository settlementRepository,
    IEntryRepository entryRepository) : ControllerBase
{
    /// <summary>Returns all settlements.</summary>
    /// <returns>An array of all settlements.</returns>
    /// <response code="200">Settlements retrieved successfully.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<SettlementDto[]>> GetAll()
    {
        Settlement[] settlements = await settlementRepository.GetAllAsync();
        return Ok(settlements.Select(s => s.ToDto()).ToArray());
    }

    /// <summary>Returns a settlement by its ID.</summary>
    /// <param name="id">The ID of the settlement.</param>
    /// <returns>The settlement with the given ID.</returns>
    /// <response code="200">Settlement found and returned.</response>
    /// <response code="404">No settlement with the given ID exists.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SettlementDto>> GetById(int id)
    {
        Settlement? settlement = await settlementRepository.GetByIdAsync(id);
        if (settlement is null)
            return NotFound();
        return Ok(settlement.ToDto());
    }

    /// <summary>Creates a new settlement.</summary>
    /// <param name="dto">The settlement data to create.</param>
    /// <returns>The newly created settlement.</returns>
    /// <response code="201">Settlement created successfully.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<SettlementDto>> Create([FromBody] CreateSettlementDto dto)
    {
        Settlement created = await settlementRepository.CreateAsync(dto.ToDomain());
        return CreatedAtAction(
            actionName: nameof(GetById),
            routeValues: new { id = created.Id },
            value: created.ToDto()
        );
    }

    /// <summary>Updates an existing settlement.</summary>
    /// <param name="id">The ID of the settlement to update.</param>
    /// <param name="dto">The updated settlement data.</param>
    /// <returns>The updated settlement.</returns>
    /// <response code="200">Settlement updated successfully.</response>
    /// <response code="404">No settlement with the given ID exists.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SettlementDto>> Update(int id, [FromBody] UpdateSettlementDto dto)
    {
        Settlement? updated = await settlementRepository.UpdateAsync(dto.ToDomain(id));
        if (updated is null)
            return NotFound();
        return Ok(updated.ToDto());
    }

    /// <summary>Deletes a settlement by its ID.</summary>
    /// <param name="id">The ID of the settlement to delete.</param>
    /// <response code="204">Settlement deleted successfully.</response>
    /// <response code="404">No settlement with the given ID exists.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        bool deleted = await settlementRepository.DeleteAsync(id);
        if (!deleted)
            return NotFound();
        return NoContent();
    }

    /// <summary>Adds a user to a settlement.</summary>
    /// <param name="id">The ID of the settlement.</param>
    /// <param name="dto">The user to add.</param>
    /// <response code="204">User added to settlement successfully.</response>
    [HttpPost("{id:int}/users")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddUser(int id, [FromBody] AddSettlementUserDto dto)
    {
        await settlementRepository.AddUserAsync(settlementId: id, userId: dto.UserId);
        return NoContent();
    }

    /// <summary>Removes a user from a settlement.</summary>
    /// <param name="id">The ID of the settlement.</param>
    /// <param name="userId">The ID of the user to remove.</param>
    /// <response code="204">User removed from settlement successfully.</response>
    [HttpDelete("{id:int}/users/{userId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveUser(int id, int userId)
    {
        await settlementRepository.RemoveUserAsync(settlementId: id, userId: userId);
        return NoContent();
    }

    /// <summary>Returns all entries for a settlement.</summary>
    /// <param name="id">The ID of the settlement.</param>
    /// <returns>An array of entries belonging to the settlement.</returns>
    /// <response code="200">Entries retrieved successfully.</response>
    [HttpGet("{id:int}/entries")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<EntryDto[]>> GetEntries(int id)
    {
        Entry[] entries = await entryRepository.GetBySettlementIdAsync(id);
        return Ok(entries.Select(e => e.ToDto()).ToArray());
    }

    /// <summary>Returns a single entry within a settlement.</summary>
    /// <param name="id">The ID of the settlement.</param>
    /// <param name="entryId">The ID of the entry.</param>
    /// <returns>The entry with the given ID.</returns>
    /// <response code="200">Entry found and returned.</response>
    /// <response code="404">No entry with the given ID exists in this settlement.</response>
    [HttpGet("{id:int}/entries/{entryId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EntryDto>> GetEntry(int id, int entryId)
    {
        Entry? entry = await entryRepository.GetByIdAsync(entryId);
        if (entry is null || entry.SettlementId != id)
            return NotFound();
        return Ok(entry.ToDto());
    }

    /// <summary>Creates a new entry in a settlement.</summary>
    /// <param name="id">The ID of the settlement the entry belongs to.</param>
    /// <param name="dto">The entry data to create.</param>
    /// <returns>The newly created entry.</returns>
    /// <response code="201">Entry created successfully.</response>
    [HttpPost("{id:int}/entries")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<EntryDto>> CreateEntry(int id, [FromBody] CreateEntryDto dto)
    {
        Entry created = await entryRepository.CreateAsync(dto.ToDomain(id));
        return CreatedAtAction(
            actionName: nameof(GetEntry),
            routeValues: new { id = created.SettlementId, entryId = created.Id },
            value: created.ToDto()
        );
    }

    /// <summary>Updates an existing entry within a settlement.</summary>
    /// <param name="id">The ID of the settlement the entry belongs to.</param>
    /// <param name="entryId">The ID of the entry to update.</param>
    /// <param name="dto">The updated entry data.</param>
    /// <returns>The updated entry.</returns>
    /// <response code="200">Entry updated successfully.</response>
    /// <response code="404">No entry with the given ID exists in this settlement.</response>
    [HttpPut("{id:int}/entries/{entryId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EntryDto>> UpdateEntry(int id, int entryId, [FromBody] UpdateEntryDto dto)
    {
        Entry? updated = await entryRepository.UpdateAsync(dto.ToDomain(entryId, id));
        if (updated is null)
            return NotFound();
        return Ok(updated.ToDto());
    }

    /// <summary>Deletes an entry from a settlement.</summary>
    /// <param name="id">The ID of the settlement the entry belongs to.</param>
    /// <param name="entryId">The ID of the entry to delete.</param>
    /// <response code="204">Entry deleted successfully.</response>
    /// <response code="404">No entry with the given ID exists in this settlement.</response>
    [HttpDelete("{id:int}/entries/{entryId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEntry(int id, int entryId)
    {
        bool deleted = await entryRepository.DeleteAsync(entryId);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
}