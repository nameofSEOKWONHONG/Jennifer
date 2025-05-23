﻿namespace Jennifer.Account.Session.Abstracts;

public interface IFetcher<TOut, in TIn>
{
    Task<TOut> FetchAsync(TIn input);
}