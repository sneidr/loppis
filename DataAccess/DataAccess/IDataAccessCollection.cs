using DataAccess.Model;

namespace DataAccess.DataAccess
{
    public interface IDataAccessCollection
    {
        void Add(IDataAccess dataAccess);
        void RemoveSale(Sale sale);
        void WriteSale(Sale sale);
    }
}