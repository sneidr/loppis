namespace DataAccess.DataAccess
{
    public interface IDataAccess
    {
    }

    public class DataAccessCollection
    {
        //public void WriteSale(Sale sale);

        List<IDataAccess> DataAccess { get; set; }
    }
}