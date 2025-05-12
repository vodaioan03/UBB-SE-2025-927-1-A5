﻿namespace DuoClassLibrary.Interfaces.Proxies
{
    public interface ICoinsServiceProxy
    {
        Task AddCoinsAsync(int userId, int amount);
        Task<bool> ApplyDailyLoginBonusAsync(int userId);
        Task<int> GetUserCoinBalanceAsync(int userId);
        Task<bool> TrySpendingCoinsAsync(int userId, int cost);
    }
}