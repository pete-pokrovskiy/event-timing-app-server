﻿using EventTiming.Data.Repositories.ByEntity.Interfaces;
using System.Threading.Tasks;

namespace EventTiming.Data
{
    public interface IUow
    {
        IEventRepository EventRepository { get; }
        int Commit();
        Task<int> CommitAsync();
    }
}