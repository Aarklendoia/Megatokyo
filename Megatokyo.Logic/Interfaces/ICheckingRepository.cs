﻿using Megatokyo.Domain;

namespace Megatokyo.Logic.Interfaces
{
    public interface ICheckingRepository
    {
        Task<Checking> GetAsync(int number);
        Task<Checking> CreateAsync(Checking checkingDomain);
        Task<Checking> UpdateAsync(Checking checkingDomain);
    }
}
