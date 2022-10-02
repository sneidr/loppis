using MongoDB.Driver;

namespace DataAccess.DataAccess;
public class MongoDbDataAccess : IDataAccess
{
    private const string connectionString = "";
    private const string databaseName = "choredb";
    private const string choreCollection = "chore_chart";
    private const string userCollection = "users";
    private const string choreHistoryCollection = "chore_history";
    
    private IMongoCollection<T> ConnectToMongo<T>(in string collection)
    {
        var settings = MongoClientSettings.FromConnectionString(connectionString);
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        var client = new MongoClient(settings);
        var db = client.GetDatabase(databaseName);
        return db.GetCollection<T>(collection);
    }

    //public async Task<List<UserModel>> GetAllUsers()
    //{
    //    var usersCollection = ConnectToMongo<UserModel>(userCollection);
    //    var results = await usersCollection.FindAsync(_ => true);
    //    return results.ToList();
    //}

    //public async Task<List<ChoreModel>> GetAllChores()
    //{
    //    var choresCollection = ConnectToMongo<ChoreModel>(choreCollection);
    //    var results = await choresCollection.FindAsync(_ => true);
    //    return results.ToList();
    //}

    //public async Task<List<ChoreModel>> GetAllChoresForAUser(UserModel user)
    //{
    //    var choresCollection = ConnectToMongo<ChoreModel>(choreCollection);
    //    var results = await choresCollection.FindAsync(cm => cm.AssignedTo.Id == user.Id);
    //    return results.ToList();
    //}

    //public Task CreateUser(UserModel user)
    //{
    //    var usersCollection = ConnectToMongo<UserModel>(userCollection);
    //    return usersCollection.InsertOneAsync(user);
    //}

    //public Task CreateChore(ChoreModel chore)
    //{
    //    var choresCollection = ConnectToMongo<ChoreModel>(choreCollection);
    //    return choresCollection.InsertOneAsync(chore);
    //}

    //public Task UpdateChore(ChoreModel chore)
    //{
    //    var choresCollection = ConnectToMongo<ChoreModel>(choreCollection);
    //    var filter = Builders<ChoreModel>.Filter.Eq("Id", chore.Id);
    //    return choresCollection.ReplaceOneAsync(filter, chore, new ReplaceOptions { IsUpsert = true });
    //}

    //public Task DeleteChore(ChoreModel chore)
    //{
    //    var choresCollection = ConnectToMongo<ChoreModel>(choreCollection);
    //    return choresCollection.DeleteOneAsync(c => c.Id == chore.Id);
    //}
}
