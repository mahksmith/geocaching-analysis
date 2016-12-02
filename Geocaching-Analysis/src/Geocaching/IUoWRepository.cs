namespace Geocaching
{
    interface IUoWRepository<T>: IRepository<T>
    {
        void Commit();
    }
}
