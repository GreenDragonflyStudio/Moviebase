using System.Collections.Generic;

namespace Moviebase.DAL
{
    public interface ISeeder<out T> where T : class
    {
        IEnumerable<T> Seed();
    }
}
