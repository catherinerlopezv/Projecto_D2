using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Api.Models;
namespace Api.Services
{
    public class UserService
    {
        private readonly IMongoCollection<UsuarioI> _usuario; //Todas las operaciones se harán en base a esta variable
        public UserService(IChatDataBaseSettings settings) //Connección con la base de datos 
        {
            var client = new MongoClient(settings.ConnectionString); //Conectarse al cluster
            settings.ChatCollectionName = "UsersData";
            var database = client.GetDatabase(settings.DatabaseName); //Obtener la base
            _usuario = database.GetCollection<UsuarioI>(settings.ChatCollectionName); //Obtener la colección en este caso la que contiene los datos de los usuarios

        }
        public UsuarioI Get(string nombre) =>
            _usuario.Find<UsuarioI>(usuario => usuario.NickName == nombre).FirstOrDefault();
        public List<UsuarioI> Get() =>
            _usuario.Find(usuario => true).ToList();
        public UsuarioI Create(UsuarioI user) //Añadir nuevos usuarios al sistema
        {
            //AÑADIR EL CIFRADO DE LA CONTRASEÑA AQUÍ            
            _usuario.InsertOne(user);
            return user;

        }
        public void Update(string id, UsuarioI user) => //Actualizar registros
            _usuario.ReplaceOne(x => x.NickName == id, user);

        public void Remove(UsuarioI user) => //Eliminar usuarioi
            _usuario.DeleteOne(x => x.NickName == user.NickName);

        public void Remove(string id) => //Eliminar buscando el Id
            _usuario.DeleteOne(x => x.NickName == id);

    }
}
