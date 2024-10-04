using MongoDB.Driver;
using MongoDB.Bson;
using System;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext()
    {
        // Configuração da string de conexão
        var client = new MongoClient("mongodb://localhost:27017");
        _database = client.GetDatabase("tbca"); // Nome do banco de dados
    }

    // Método para acessar a coleção "alimentos"
    public IMongoCollection<BsonDocument> GetAlimentosCollection()
    {
        return _database.GetCollection<BsonDocument>("alimentos");
    }
}
