using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;
using MongoDB.Driver;

namespace Api.Services
{
    public class GruposService
    {
        private readonly IMongoCollection<Grupos> _usuario; //Todas las operaciones se harán en base a esta variable
        public GruposService(IChatDataBaseSettings settings) //Connección con la base de datos 
        {
            var client = new MongoClient(settings.ConnectionString); //Conectarse al cluster
            settings.ChatCollectionName = "Grupos";
            var database = client.GetDatabase(settings.DatabaseName); //Obtener la base
            _usuario = database.GetCollection<Grupos>(settings.ChatCollectionName); //Obtener la colección en este caso la que contiene los datos de los usuarios
        }
        public Grupos Get(string nombre)
        {


            return _usuario.Find<Grupos>(usuario => usuario.Usuario == nombre).FirstOrDefault();
        }
        public List<Grupos> GetT(string nombre)
        {
            List<Grupos> algo = new List<Grupos>();
            var documentos =  _usuario.Find(_ => true).ToList();

            for (int i = 0; i < _usuario.EstimatedDocumentCount(); i++)
            {

                if ( documentos[i].Usuario != null && documentos[i].Usuario ==nombre)
                {
                    algo.Add(documentos[i]);
                }
             
            }
            return algo;
        }

        public List<Grupos> GetTG(string nombre)
        {

            List<Grupos> algo = new List<Grupos>();
            var documentos = _usuario.Find(_ => true).ToList();

            for (int i = 0; i < _usuario.EstimatedDocumentCount(); i++)
            {
                for (int j = 0; j < documentos[i].Amigos.Count; j++)
                {
                    if (documentos[i].Amigos[j] != null && documentos[i].Amigos[j]==nombre)
                    {
                        algo.Add(documentos[i]);
                    }
                }
            }
            return algo;
        }

        public Grupos GetG(string nombre)
        {
        

            return _usuario.Find<Grupos>(amigo => amigo.Amigos.Contains(nombre)).FirstOrDefault();
        }

        public Grupos Create(Grupos user) //Añadir nuevos usuarios al sistema
        {
            //AÑADIR EL CIFRADO DE LA CONTRASEÑA AQUÍ            
            _usuario.InsertOne(user);
            return user;
        }
        public void Update(string id, Grupos user) => //Actualizar registros
           _usuario.ReplaceOne(x => x.Usuario == id, user);
    }
}
