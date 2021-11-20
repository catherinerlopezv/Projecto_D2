using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;
using MongoDB.Driver;
namespace Api.Services
{
    public class ContactosService
    {
        private readonly IMongoCollection<ContactosI> _usuario; //Todas las operaciones se harán en base a esta variable
        public ContactosService(IChatDataBaseSettings settings) //Connección con la base de datos 
        {
            var client = new MongoClient(settings.ConnectionString); //Conectarse al cluster
            settings.ChatCollectionName = "Contactos";
            var database = client.GetDatabase(settings.DatabaseName); //Obtener la base
            _usuario = database.GetCollection<ContactosI>(settings.ChatCollectionName); //Obtener la colección en este caso la que contiene los datos de los usuarios

        }
        public ContactosI Get(string nombre) =>
            _usuario.Find<ContactosI>(usuario => usuario.OwnerNickName == nombre).FirstOrDefault();
        public ContactosI Create(ContactosI user) //Añadir nuevos usuarios al sistema
        {
            //AÑADIR EL CIFRADO DE LA CONTRASEÑA AQUÍ            
            _usuario.InsertOne(user);
            return user;
        }
        public void Update(string id, ContactosI user) => //Actualizar registros
           _usuario.ReplaceOne(x => x.OwnerNickName == id, user);
    }
}
