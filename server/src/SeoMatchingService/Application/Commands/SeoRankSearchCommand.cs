using SeoMatchingService.Domain;

namespace SeoMatchingService.Application.Commands
{
    public record SeoRankSearchCommand : IRequest<IReadOnlyDictionary<string, int[]>>
    {
        public required string SearchValue { get; set; }
        public required string ComparedUrl { get; set; }
    }

    public class SeoRankSearchCommandHandler(SeoRank seoRank) : IRequestHandler<SeoRankSearchCommand, IReadOnlyDictionary<string, int[]>>
    {
        private readonly SeoRank _seoRank = seoRank;

        public Task<IReadOnlyDictionary<string, int[]>> Handle(SeoRankSearchCommand request, CancellationToken cancellationToken)
        {
            return _seoRank.SeoRankingAsync(request.SearchValue, request.ComparedUrl, cancellationToken);
        }
    }
}