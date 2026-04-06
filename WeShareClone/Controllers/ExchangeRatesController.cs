using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeShareClone.Domain.Models;
using WeShareClone.Domain.Repositories;
using WeShareClone.Dto.ExchangeRates;
using WeShareClone.Extensions;
using WeShareClone.Services;

namespace WeShareClone.Controllers;

/// <summary>Provides access to Danmarks Nationalbank exchange rates.</summary>
[ApiController]
[Route("api/exchange-rates")]
[Authorize]
public class ExchangeRatesController(
    IExchangeRateRepository exchangeRateRepository,
    IExchangeRateService exchangeRateService) : ControllerBase
{
    /// <summary>
    /// Returns exchange rates for the most recent available closing date, or for a
    /// specific date when the <paramref name="date"/> query parameter is provided.
    /// </summary>
    /// <param name="date">Optional closing date in YYYY-MM-DD format.</param>
    [HttpGet]
    [ProducesResponseType<ExchangeRateDto[]>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ExchangeRateDto[]>> GetAsync([FromQuery] DateOnly? date)
    {
        ExchangeRate[] rates = date.HasValue
            ? await exchangeRateRepository.GetByDateAsync(date.Value)
            : await exchangeRateRepository.GetLatestAsync();

        if (rates.Length == 0)
            return NotFound();

        return Ok(rates.Select(r => r.ToDto()).ToArray());
    }

    /// <summary>
    /// Fetches the latest exchange rates from Danmarks Nationalbank, persists them,
    /// and returns the newly stored rates. Requires Admin role.
    /// </summary>
    [HttpPost("fetch")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType<ExchangeRateDto[]>(StatusCodes.Status200OK)]
    public async Task<ActionResult<ExchangeRateDto[]>> FetchAsync()
    {
        ExchangeRate[] rates = await exchangeRateService.FetchAndStoreAsync();
        return Ok(rates.Select(r => r.ToDto()).ToArray());
    }
}
