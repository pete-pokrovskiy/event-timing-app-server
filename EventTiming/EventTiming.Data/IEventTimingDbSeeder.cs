using System.Threading.Tasks;

namespace EventTiming.Data
{
    public interface IEventTimingDbSeeder
    {
        Task Seed();
    }
}