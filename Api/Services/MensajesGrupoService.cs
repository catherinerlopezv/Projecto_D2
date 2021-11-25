using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Api.Models;

namespace Api.Services
{
    public class MensajesGrupoService
    {
        private readonly IMongoCollection<MensajesI> _mensajes; //Todas las operaciones se harán en base a esta variable
        public MensajesGrupoService(IChatDataBaseSettings settings) //Connección con la base de datos 
        {
            var client = new MongoClient(settings.ConnectionString); //Conectarse al cluster
            settings.ChatCollectionName = "MensajesGrupos";
            var database = client.GetDatabase(settings.DatabaseName); //Obtener la base
            _mensajes = database.GetCollection<MensajesI>(settings.ChatCollectionName); //Obtener la colección en este caso la que contiene los datos de los usuarios

        }

        public MensajesI GetMensajes(string mensaje) =>
            _mensajes.Find<MensajesI>(x => x.Cuerpo == mensaje).FirstOrDefault();
        public MensajesI GetId(string mensaje) =>
            _mensajes.Find<MensajesI>(x => x.Id == mensaje).FirstOrDefault();

        public MensajesI GetFile(string name) =>
            _mensajes.Find<MensajesI>(x => x.NombreArchivo == name).FirstOrDefault();


        //ver aqui ZROETH
        public List<MensajesI> Get(string nombre, int i)
        {
            //Obtener los mensajes de cierta conversación


            return _mensajes.Find<MensajesI>(usuario =>  usuario.Visible != nombre).ToList();
        }

        public List<MensajesI> GetG(string nombre, int i)
        {
            //Obtener los mensajes de cierta conversación


            //return _mensajes.Find<MensajesI>(amigo => (amigo.Receptor[i]== nombre)).ToList();

            return _mensajes.Find<MensajesI>(usuario => (usuario.Emisor == nombre || usuario.Receptor.Contains(nombre)) && usuario.Visible != nombre).ToList();
        }


        public List<MensajesI> BuscarMensaje(string text) =>
            _mensajes.Find(msg => msg.Cuerpo.Contains(text)).ToList();

        public List<MensajesI> Get() =>
            _mensajes.Find(usuario => true).ToList();

        public MensajesI Create(MensajesI msj) //Añadir nuevos usuarios al sistema
        {
            //AÑADIR EL CIFRADO DE LA CONTRASEÑA AQUÍ            
            _mensajes.InsertOne(msj);
            
            return msj;
        }

        public void BorrarParcial(string cuerpo, MensajesI user) => //Borrar los mensajes solo para una persona
            _mensajes.ReplaceOne(x => x.Id == cuerpo, user);

        public void Remove(MensajesI mensajes) => //Eliminar buscando el emisor
            _mensajes.DeleteOne(x => x.Emisor == mensajes.Emisor);

        public void Remove(string Emisor) => //Eliminar buscando el emisor
            _mensajes.DeleteOne(x => x.Id == Emisor);
    }
}
