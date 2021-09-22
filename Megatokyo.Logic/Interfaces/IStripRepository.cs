﻿using Megatokyo.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Megatokyo.Logic.Interfaces
{
    public interface IStripRepository
    {
        Task<IEnumerable<Strip>> GetAllAsync();
        Task<IEnumerable<Strip>> GetCategoryAsync(string category);
        Task<Strip> GetAsync(int number);
        Task<Strip> CreateAsync(Strip rantDomain);
        Task<int> SaveAsync();
    }
}
