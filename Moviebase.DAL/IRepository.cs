namespace Moviebase.DAL
{
    public interface IRepository<T, in TKey> where T : class
    {
        T GetById(TKey id);
        void Delete(TKey id);
        void Insert(T entity);
        void Update(T entity);
    }
}
