using Asp.Versioning;
using SeoMatchingService.Application.Commands;
using SeoMatchingService.Dtos.Requests;
using SeoMatchingService.Queries;

namespace SeoMatchingService.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v1/seo")]
    public class SeoMatchingController : ControllerBase
    {
        private readonly IMediator _commandDispatcher;
        private readonly ILogger<SeoMatchingController> _logger;
        private readonly ISeoRankSearchedQuery _query;

        public SeoMatchingController(IMediator commandSender, ILogger<SeoMatchingController> logger, ISeoRankSearchedQuery query)
        {
            this._commandDispatcher = commandSender;
            this._logger = logger;
            this._query = query;
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchAsync([FromBody] SeoSearchRequest request, CancellationToken cancellationToken = default)
        {
            // TODO this one should working with fire & forget because the crawl going with long running
            // The result will response via socket with pushed method
            var searchResult = await _commandDispatcher.Send(new SeoRankSearchCommand
            {
                ComparedUrl = request.CompareUrl,
                SearchValue = request.SearchValue
            }, cancellationToken);

            return Ok(searchResult);
        }

        [HttpPost("history")]
        public async Task<IActionResult> HistoryRanksAsync([FromQuery] string engine = null, [FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null, CancellationToken cancellationToken = default)
        {
            var result = await _query.GetSeoRankSearchedHistoriesAsync(engine, fromDate, toDate, cancellationToken);

            return Ok(result);
        }
    }
}