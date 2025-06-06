﻿using eXtensionSharp;
using Jennifer.Domain.Accounts;
using Jennifer.Infrastructure.Database;
using Jennifer.SharedKernel;
using LinqKit;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Options.Queries;

public sealed class GetsOptionQueryHandler(JenniferDbContext dbContext)
    : IQueryHandler<GetsOptionQuery, PaginatedResult<Option>>
{
    public async ValueTask<PaginatedResult<Option>> Handle(GetsOptionQuery query, CancellationToken cancellationToken)
    {
        var predicate = PredicateBuilder.New<Option>(true);
        if (query.Type.xIsNotEmpty())
        {
            predicate = predicate.And(x => x.Type == query.Type);
        }

        var queryable = dbContext.Options
            .AsNoTracking()
            .AsExpandable()
            .Where(predicate);

        var total = await queryable.CountAsync(cancellationToken: cancellationToken);
        var items = await queryable.Skip((query.PageNo - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToArrayAsync(cancellationToken: cancellationToken);
        
        return await PaginatedResult<Option>.SuccessAsync(total, items, query.PageNo, query.PageSize);
    }
}