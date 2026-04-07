using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeShareClone.Domain.Models;
using WeShareClone.Domain.Repositories;
using WeShareClone.Dto.Entries;
using WeShareClone.Dto.Settlements;
using WeShareClone.Dto.Users;
using WeShareClone.Extensions;
using WeShareClone.Utilities;

namespace WeShareClone.Controllers;

[ApiController]
[Route("api/settlements")]
[Authorize]
public class SettlementsController(
    ISettlementRepository settlementRepository,
    IEntryRepository entryRepository,
    ISettlementDebtRepository settlementDebtRepository,
    IExchangeRateRepository exchangeRateRepository) : ControllerBase
{
    private int GetUserId()
        => int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);

    /// <summary>Returns all settlements.</summary>
    /// <returns>An array of all settlements.</returns>
    /// <response code="200">Settlements retrieved successfully.</response>
    /// <response code="403">Caller does not have the Admin role.</response>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<SettlementDto[]>> GetAllAsync()
    {
        Settlement[] settlements = await settlementRepository.GetAllAsync();
        return Ok(settlements.Select(s => s.ToDto()).ToArray());
    }

    /// <summary>Returns all settlements the current user participates in.</summary>
    /// <returns>An array of settlements the caller is a member or creator of.</returns>
    /// <response code="200">Settlements retrieved successfully.</response>
    [HttpGet("my")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<SettlementDto[]>> GetMyAsync()
    {
        Settlement[] settlements = await settlementRepository.GetByUserIdAsync(GetUserId());
        return Ok(settlements.Select(s => s.ToDto()).ToArray());
    }

    /// <summary>Returns a settlement by its ID.</summary>
    /// <param name="id">The ID of the settlement.</param>
    /// <returns>The settlement with the given ID.</returns>
    /// <response code="200">Settlement found and returned.</response>
    /// <response code="403">Caller is not a participant of this settlement.</response>
    /// <response code="404">No settlement with the given ID exists.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SettlementDto>> GetByIdAsync(int id)
    {
        Settlement? settlement = await settlementRepository.GetByIdAsync(id);
        if (settlement is null)
            return NotFound();

        if (!await settlementRepository.IsParticipantAsync(id, GetUserId()))
            return Forbid();

        return Ok(settlement.ToDto());
    }

    /// <summary>Returns the participants of a settlement.</summary>
    /// <param name="id">The ID of the settlement.</param>
    /// <returns>An array of users who are participants of the settlement.</returns>
    /// <response code="200">Participants retrieved successfully.</response>
    /// <response code="403">Caller is not a participant of this settlement.</response>
    /// <response code="404">No settlement with the given ID exists.</response>
    [HttpGet("{id:int}/participants")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto[]>> GetParticipantsAsync(int id)
    {
        if (!await settlementRepository.IsParticipantAsync(id, GetUserId()))
            return await settlementRepository.GetByIdAsync(id) is null ? NotFound() : Forbid();

        User[] participants = await settlementRepository.GetParticipantsAsync(id);
        return Ok(participants.Select(u => u.ToDto()).ToArray());
    }

    /// <summary>Creates a new settlement.</summary>
    /// <param name="dto">The settlement data to create.</param>
    /// <returns>The newly created settlement.</returns>
    /// <response code="201">Settlement created successfully.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<SettlementDto>> CreateAsync([FromBody] CreateSettlementDto dto)
    {
        Settlement created = await settlementRepository.CreateAsync(dto.ToDomain(GetUserId()));
        return Created($"api/settlements/{created.Id}", created.ToDto());
    }

    /// <summary>Updates an existing settlement.</summary>
    /// <param name="id">The ID of the settlement to update.</param>
    /// <param name="dto">The updated settlement data.</param>
    /// <returns>The updated settlement.</returns>
    /// <response code="200">Settlement updated successfully.</response>
    /// <response code="403">Caller is not the creator of this settlement.</response>
    /// <response code="404">No settlement with the given ID exists.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SettlementDto>> UpdateAsync(int id, [FromBody] UpdateSettlementDto dto)
    {
        Settlement? settlement = await settlementRepository.GetByIdAsync(id);
        if (settlement is null)
            return NotFound();

        if (settlement.CreatedBy != GetUserId())
            return Forbid();

        Settlement? updated = await settlementRepository.UpdateAsync(dto.ToDomain(id, settlement.Status));
        if (updated is null)
            return NotFound();

        return Ok(updated.ToDto());
    }

    /// <summary>Deletes a settlement by its ID.</summary>
    /// <param name="id">The ID of the settlement to delete.</param>
    /// <response code="204">Settlement deleted successfully.</response>
    /// <response code="403">Caller is not the creator of this settlement.</response>
    /// <response code="404">No settlement with the given ID exists.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        Settlement? settlement = await settlementRepository.GetByIdAsync(id);
        if (settlement is null)
            return NotFound();

        if (settlement.CreatedBy != GetUserId())
            return Forbid();

        await settlementRepository.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>Adds a user to a settlement.</summary>
    /// <param name="id">The ID of the settlement.</param>
    /// <param name="dto">The user to add.</param>
    /// <response code="204">User added to settlement successfully.</response>
    /// <response code="403">Caller is not a participant of this settlement.</response>
    /// <response code="404">No settlement with the given ID exists.</response>
    [HttpPost("{id:int}/users")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddUserAsync(int id, [FromBody] AddSettlementUserDto dto)
    {
        if (!await settlementRepository.IsParticipantAsync(id, GetUserId()))
            return await settlementRepository.GetByIdAsync(id) is null ? NotFound() : Forbid();

        await settlementRepository.AddUserAsync(settlementId: id, userId: dto.UserId);
        return NoContent();
    }

    /// <summary>Removes a user from a settlement.</summary>
    /// <param name="id">The ID of the settlement.</param>
    /// <param name="userId">The ID of the user to remove.</param>
    /// <response code="204">User removed from settlement successfully.</response>
    /// <response code="403">Caller is not a participant of this settlement.</response>
    /// <response code="404">No settlement with the given ID exists.</response>
    [HttpDelete("{id:int}/users/{userId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveUserAsync(int id, int userId)
    {
        if (!await settlementRepository.IsParticipantAsync(id, GetUserId()))
            return await settlementRepository.GetByIdAsync(id) is null ? NotFound() : Forbid();

        await settlementRepository.RemoveUserAsync(settlementId: id, userId: userId);
        return NoContent();
    }

    /// <summary>Returns all entries for a settlement.</summary>
    /// <param name="id">The ID of the settlement.</param>
    /// <returns>An array of entries belonging to the settlement.</returns>
    /// <response code="200">Entries retrieved successfully.</response>
    /// <response code="403">Caller is not a participant of this settlement.</response>
    /// <response code="404">No settlement with the given ID exists.</response>
    [HttpGet("{id:int}/entries")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EntryDto[]>> GetEntriesAsync(int id)
    {
        if (!await settlementRepository.IsParticipantAsync(id, GetUserId()))
            return await settlementRepository.GetByIdAsync(id) is null ? NotFound() : Forbid();

        Entry[] entries = await entryRepository.GetBySettlementIdAsync(id);
        return Ok(entries.Select(e => e.ToDto()).ToArray());
    }

    /// <summary>Returns a single entry within a settlement.</summary>
    /// <param name="id">The ID of the settlement.</param>
    /// <param name="entryId">The ID of the entry.</param>
    /// <returns>The entry with the given ID.</returns>
    /// <response code="200">Entry found and returned.</response>
    /// <response code="403">Caller is not a participant of this settlement.</response>
    /// <response code="404">No entry with the given ID exists in this settlement.</response>
    [HttpGet("{id:int}/entries/{entryId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EntryDto>> GetEntryAsync(int id, int entryId)
    {
        if (!await settlementRepository.IsParticipantAsync(id, GetUserId()))
            return await settlementRepository.GetByIdAsync(id) is null ? NotFound() : Forbid();

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
    /// <response code="400">One or more distribution user IDs are not participants of the settlement, or the distribution factors are inconsistent with the chosen mode.</response>
    /// <response code="403">Caller is not a participant of this settlement.</response>
    /// <response code="404">No settlement with the given ID exists.</response>
    [HttpPost("{id:int}/entries")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EntryDto>> CreateEntryAsync(int id, [FromBody] CreateEntryDto dto)
    {
        if (!await settlementRepository.IsParticipantAsync(id, GetUserId()))
            return await settlementRepository.GetByIdAsync(id) is null ? NotFound() : Forbid();

        int[] participantIds = await settlementRepository.GetParticipantIdsAsync(id);
        if (dto.Distributions.Any(d => !participantIds.Contains(d.UserId)))
            return BadRequest();

        EDistributionMode mode = Enum.Parse<EDistributionMode>(dto.DistributionMode);
        if (!IsDistributionValid(mode, dto.Distributions, dto.Value))
            return BadRequest();

        Entry created = await entryRepository.CreateAsync(dto.ToDomain(id, GetUserId()));
        return Created($"api/settlements/{created.SettlementId}/entries/{created.Id}", created.ToDto());
    }

    /// <summary>Updates an existing entry within a settlement.</summary>
    /// <param name="id">The ID of the settlement the entry belongs to.</param>
    /// <param name="entryId">The ID of the entry to update.</param>
    /// <param name="dto">The updated entry data.</param>
    /// <returns>The updated entry.</returns>
    /// <response code="200">Entry updated successfully.</response>
    /// <response code="400">One or more distribution user IDs are not participants of the settlement, or the distribution factors are inconsistent with the chosen mode.</response>
    /// <response code="403">Caller is not the creator of this entry.</response>
    /// <response code="404">No entry with the given ID exists in this settlement.</response>
    [HttpPut("{id:int}/entries/{entryId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EntryDto>> UpdateEntryAsync(int id, int entryId, [FromBody] UpdateEntryDto dto)
    {
        Entry? entry = await entryRepository.GetByIdAsync(entryId);
        if (entry is null || entry.SettlementId != id)
            return NotFound();

        if (entry.AddedBy != GetUserId())
            return Forbid();

        int[] participantIds = await settlementRepository.GetParticipantIdsAsync(id);
        if (dto.Distributions.Any(d => !participantIds.Contains(d.UserId)))
            return BadRequest();

        EDistributionMode mode = Enum.Parse<EDistributionMode>(dto.DistributionMode);
        if (!IsDistributionValid(mode, dto.Distributions, dto.Value))
            return BadRequest();

        Entry? updated = await entryRepository.UpdateAsync(dto.ToDomain(entryId, id));
        if (updated is null)
            return NotFound();

        return Ok(updated.ToDto());
    }

    /// <summary>Deletes an entry from a settlement.</summary>
    /// <param name="id">The ID of the settlement the entry belongs to.</param>
    /// <param name="entryId">The ID of the entry to delete.</param>
    /// <response code="204">Entry deleted successfully.</response>
    /// <response code="403">Caller is not the creator of this entry.</response>
    /// <response code="404">No entry with the given ID exists in this settlement.</response>
    [HttpDelete("{id:int}/entries/{entryId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEntryAsync(int id, int entryId)
    {
        Entry? entry = await entryRepository.GetByIdAsync(entryId);
        if (entry is null || entry.SettlementId != id)
            return NotFound();

        if (entry.AddedBy != GetUserId())
            return Forbid();

        await entryRepository.DeleteAsync(entryId);
        return NoContent();
    }

    /// <summary>
    /// Validates that the distribution factors are consistent with the chosen distribution mode.
    /// </summary>
    /// <param name="mode">The distribution mode to validate against.</param>
    /// <param name="distributions">The distributions to validate.</param>
    /// <param name="entryValue">The total value of the entry, used for <see cref="EDistributionMode.FixedAmount"/> validation.</param>
    /// <returns><see langword="true"/> if the factors satisfy the mode's constraints; otherwise <see langword="false"/>.</returns>
    private static bool IsDistributionValid(EDistributionMode mode, EntryDistributionDto[] distributions, decimal entryValue)
        => mode switch
        {
            // All factors must be identical
            EDistributionMode.EvenSplit   => distributions.Select(d => d.Factor).Distinct().Count() == 1,
            // Factors must sum to 1.0 (within floating-point tolerance)
            EDistributionMode.Percentage  => Math.Abs(distributions.Sum(d => d.Factor) - 1.0m) <= 0.001m,
            // Factors represent fixed amounts and must sum to the entry value (within rounding tolerance)
            EDistributionMode.FixedAmount => Math.Abs(distributions.Sum(d => d.Factor) - entryValue) <= 0.01m,
            _                             => false,
        };

    // -----------------------------------------------------------------------
    // Settlement lifecycle: Open → BeingSettled → Closed
    // -----------------------------------------------------------------------

    /// <summary>Starts settling a settlement: calculates and persists debts, transitions to BeingSettled.</summary>
    /// <param name="id">The ID of the settlement.</param>
    /// <returns>The calculated debts.</returns>
    /// <response code="200">Settlement is now in BeingSettled state; debts returned.</response>
    /// <response code="400">Settlement is not in the Open state.</response>
    /// <response code="403">Caller is not the creator of this settlement.</response>
    /// <response code="404">No settlement with the given ID exists.</response>
    [HttpPost("{id:int}/settle")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SettlementDebtDto[]>> StartSettlingAsync(int id)
    {
        Settlement? settlement = await settlementRepository.GetByIdAsync(id);
        if (settlement is null)
            return NotFound();

        if (settlement.CreatedBy != GetUserId())
            return Forbid();

        if (settlement.Status != ESettlementStatus.Open)
            return BadRequest("Settlement must be in the Open state to start settling.");

        int[] participantIds = await settlementRepository.GetParticipantIdsAsync(id);
        Entry[] entries = await entryRepository.GetBySettlementIdAsync(id);
        ExchangeRate[] rates = await exchangeRateRepository.GetLatestAsync();

        DebtCalculator.DebtPayment[] payments = DebtCalculator.Calculate(
            entries, participantIds, rates, settlement.Currency
        );

        // Persist calculated debts
        List<SettlementDebt> debts = [];
        foreach (DebtCalculator.DebtPayment payment in payments)
        {
            SettlementDebt debt = await settlementDebtRepository.CreateAsync(new SettlementDebt(
                Id: 0,
                SettlementId: id,
                FromUserId: payment.FromUserId,
                ToUserId: payment.ToUserId,
                Amount: payment.Amount,
                Currency: settlement.Currency,
                IsPaid: false,
                PaidAtUtc: null,
                CreatedAtUtc: default,
                UpdatedAtUtc: default
            ));
            debts.Add(debt);
        }

        await settlementRepository.UpdateStatusAsync(id, ESettlementStatus.BeingSettled);

        return Ok(debts.Select(d => d.ToDto()).ToArray());
    }

    /// <summary>Reverts a settlement from BeingSettled back to Open, removing all persisted debts.</summary>
    /// <param name="id">The ID of the settlement.</param>
    /// <response code="204">Settlement reverted to Open; debts deleted.</response>
    /// <response code="400">Settlement is not in the BeingSettled state.</response>
    /// <response code="403">Caller is not the creator of this settlement.</response>
    /// <response code="404">No settlement with the given ID exists.</response>
    [HttpDelete("{id:int}/settle")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevertSettlingAsync(int id)
    {
        Settlement? settlement = await settlementRepository.GetByIdAsync(id);
        if (settlement is null)
            return NotFound();

        if (settlement.CreatedBy != GetUserId())
            return Forbid();

        if (settlement.Status != ESettlementStatus.BeingSettled)
            return BadRequest("Settlement must be in the BeingSettled state to revert.");

        await settlementDebtRepository.DeleteBySettlementIdAsync(id);
        await settlementRepository.UpdateStatusAsync(id, ESettlementStatus.Open);

        return NoContent();
    }

    /// <summary>Closes a settlement. All debts must be paid first.</summary>
    /// <param name="id">The ID of the settlement.</param>
    /// <response code="204">Settlement closed successfully.</response>
    /// <response code="400">Settlement is not in BeingSettled state, or not all debts are paid.</response>
    /// <response code="403">Caller is not the creator of this settlement.</response>
    /// <response code="404">No settlement with the given ID exists.</response>
    [HttpPost("{id:int}/close")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CloseAsync(int id)
    {
        Settlement? settlement = await settlementRepository.GetByIdAsync(id);
        if (settlement is null)
            return NotFound();

        if (settlement.CreatedBy != GetUserId())
            return Forbid();

        if (settlement.Status != ESettlementStatus.BeingSettled)
            return BadRequest("Settlement must be in the BeingSettled state to close.");

        SettlementDebt[] debts = await settlementDebtRepository.GetBySettlementIdAsync(id);
        if (debts.Any(d => !d.IsPaid))
            return BadRequest("All debts must be paid before closing the settlement.");

        await settlementRepository.UpdateStatusAsync(id, ESettlementStatus.Closed);

        return NoContent();
    }

    /// <summary>Reopens a closed settlement, removing all persisted debts.</summary>
    /// <param name="id">The ID of the settlement.</param>
    /// <response code="204">Settlement reopened successfully.</response>
    /// <response code="400">Settlement is not in the Closed state.</response>
    /// <response code="403">Caller is not the creator of this settlement.</response>
    /// <response code="404">No settlement with the given ID exists.</response>
    [HttpPost("{id:int}/reopen")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReopenAsync(int id)
    {
        Settlement? settlement = await settlementRepository.GetByIdAsync(id);
        if (settlement is null)
            return NotFound();

        if (settlement.CreatedBy != GetUserId())
            return Forbid();

        if (settlement.Status != ESettlementStatus.Closed)
            return BadRequest("Settlement must be in the Closed state to reopen.");

        await settlementDebtRepository.DeleteBySettlementIdAsync(id);
        await settlementRepository.UpdateStatusAsync(id, ESettlementStatus.Open);

        return NoContent();
    }

    /// <summary>Returns all persisted debts for a settlement.</summary>
    /// <param name="id">The ID of the settlement.</param>
    /// <returns>An array of debts for the settlement.</returns>
    /// <response code="200">Debts retrieved successfully.</response>
    /// <response code="403">Caller is not a participant of this settlement.</response>
    /// <response code="404">No settlement with the given ID exists.</response>
    [HttpGet("{id:int}/debts")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SettlementDebtDto[]>> GetDebtsAsync(int id)
    {
        if (!await settlementRepository.IsParticipantAsync(id, GetUserId()))
            return await settlementRepository.GetByIdAsync(id) is null ? NotFound() : Forbid();

        SettlementDebt[] debts = await settlementDebtRepository.GetBySettlementIdAsync(id);
        return Ok(debts.Select(d => d.ToDto()).ToArray());
    }

    /// <summary>Marks a debt as paid or unpaid.</summary>
    /// <param name="id">The ID of the settlement.</param>
    /// <param name="debtId">The ID of the debt to update.</param>
    /// <param name="isPaid">Whether the debt is paid.</param>
    /// <returns>The updated debt.</returns>
    /// <response code="200">Debt updated successfully.</response>
    /// <response code="403">Caller is not a participant of this settlement.</response>
    /// <response code="404">No settlement or debt with the given ID exists.</response>
    [HttpPatch("{id:int}/debts/{debtId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SettlementDebtDto>> UpdateDebtPaidAsync(int id, int debtId, [FromBody] bool isPaid)
    {
        if (!await settlementRepository.IsParticipantAsync(id, GetUserId()))
            return await settlementRepository.GetByIdAsync(id) is null ? NotFound() : Forbid();

        SettlementDebt? updated = await settlementDebtRepository.UpdatePaidAsync(debtId, isPaid);
        if (updated is null)
            return NotFound();

        return Ok(updated.ToDto());
    }
}
