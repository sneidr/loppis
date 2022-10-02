using DataAccess.Model;

namespace DataAccess.DataAccess;

public class DataAccessCollection : IDataAccessCollection
{
    public void Add(IDataAccess dataAccess)
    {
        _dataAccess.Add(dataAccess);
    }

    public async Task WriteSale(Sale sale)
    {
        foreach (var dataAccess in _dataAccess)
        {
            await dataAccess.WriteSale(sale);
        }
    }

    public async Task RemoveSale(Sale sale)
    {
        foreach (var dataAccess in _dataAccess)
        {
            await dataAccess.RemoveSale(sale);
        }
    }

    private List<IDataAccess> _dataAccess { get; set; } = new List<IDataAccess>();
}