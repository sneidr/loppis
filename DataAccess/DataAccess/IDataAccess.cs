using DataAccess.Model;

namespace DataAccess.DataAccess;

public interface IDataAccess
{
    void WriteSale(Sale sale);
    void RemoveSale(Sale sale);
}
