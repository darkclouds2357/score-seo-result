using Microsoft.EntityFrameworkCore;
using SeoMatchingService.Domain.DomainEvents;
using SeoMatchingService.Dtos;
using SeoMatchingService.Infrastructure.Entities;
using SeoMatchingService.Queries;
using System.Linq.Expressions;

namespace SeoMatchingService.Infrastructure.Query
{
    public class RelationalDatabaseQuery : IApplyQuery, ISeoRankSearchedQuery
    {
        private readonly SeoRankingDbContext _seoRankingDbContext;

        private static readonly ParameterExpression _parameter = Expression.Parameter(typeof(SeoRankSearchedEntity), "x");

        public RelationalDatabaseQuery(SeoRankingDbContext seoRankingDbContext)
        {
            this._seoRankingDbContext = seoRankingDbContext;
        }

        public async Task ApplyAsync(SeoRankSearchedEvent @event, CancellationToken cancellationToken = default)
        {
            var searchedEntities = @event.SeoSearchedResults.Select(c => new SeoRankSearchedEntity
            {
                Id = Guid.NewGuid().ToString(),
                CompareUrl = @event.ComparedUrl,
                SearchedAt = @event.CreatedAt,
                SearchEngine = c.Key,
                SeoRanks = [.. c.Value.Select(r => r.Order)],
                SearchedValue = @event.SearchedValue
            });

            await _seoRankingDbContext.SeoRankSearcheds.AddRangeAsync(searchedEntities, cancellationToken);

            await _seoRankingDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<SeoRankSearchedHistoryDto>> GetSeoRankSearchedHistoriesAsync(string searchEnigne = null, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default)
        {
            var whereExpression = BuildWhereExpression(searchEnigne, fromDate, toDate);
            var searchedResults = _seoRankingDbContext.SeoRankSearcheds.Where(whereExpression);

            var mapToDtoExpression = BuildSelectExpression();

            var result = await searchedResults.Select(mapToDtoExpression).ToListAsync(cancellationToken: cancellationToken);

            return result;
        }

        private static Expression<Func<SeoRankSearchedEntity, SeoRankSearchedHistoryDto>> BuildSelectExpression() => e => new SeoRankSearchedHistoryDto
        {
            Id = e.Id,
            CompareUrl = e.CompareUrl,
            SearchedAt = e.SearchedAt,
            SearchedValue = e.SearchedValue,
            SearchEngine = e.SearchEngine,
            SeoRanks = e.SeoRanks
        };

        private static Expression<Func<SeoRankSearchedEntity, bool>> BuildWhereExpression(string searchEnigne, DateTime? fromDate, DateTime? toDate)
        {
            List<BinaryExpression> bodyExpressions =
            [
                Expression.Equal(Expression.Constant(1), Expression.Constant(1))
            ];
            var searchEngineProperty = Expression.Property(_parameter, nameof(SeoRankSearchedEntity.SearchEngine));
            var timeProperty = Expression.Property(_parameter, nameof(SeoRankSearchedEntity.SearchedAt));
            if (!string.IsNullOrWhiteSpace(searchEnigne))
            {
                var searchConst = Expression.Constant(searchEnigne);

                var body = Expression.Equal(searchConst, searchEngineProperty);

                bodyExpressions.Add(body);
            }
            if (fromDate.HasValue)
            {
                var timeConst = Expression.Constant(fromDate.Value);
                var body = Expression.GreaterThanOrEqual(timeProperty, timeConst);
                bodyExpressions.Add(body);
            }

            if (toDate.HasValue)
            {
                var timeConst = Expression.Constant(toDate.Value);
                var body = Expression.LessThanOrEqual(timeProperty, timeConst);
                bodyExpressions.Add(body);
            }

            var alwaytrue = Expression.Equal(Expression.Constant(1), Expression.Constant(1));

            var whereBody = bodyExpressions.Aggregate(alwaytrue, Expression.And);

            var lamdaExpression = Expression.Lambda<Func<SeoRankSearchedEntity, bool>>(whereBody, _parameter);

            return lamdaExpression;
        }
    }
}