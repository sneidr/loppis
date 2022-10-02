using DataAccess.Model;

namespace DataAccess.DataAccess
{
    public interface IDataAccessCollection
    {
        void Add(IDataAccess dataAccess);
        Task RemoveSale(Sale sale);
        Task WriteSale(Sale sale);
    }
}