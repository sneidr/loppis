using DataAccess.Model;

namespace DataAccess.DataAccess;

public interface IDataAccess
{
    Task WriteSale(Sale sale);
    Task RemoveSale(Sale sale);
}
