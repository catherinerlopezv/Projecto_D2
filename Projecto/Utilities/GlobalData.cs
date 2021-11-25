using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Projecto.Models;

namespace Projecto
{
  public class MyGlobalData
  {

    public  UserData ActualUser { get; set; }
    public  UserData Receptor { get; set; }
    public  List<string> ParaGrupos { get; set; }
    public  string ArchivoEntrada { get; set; }
    public  string ArchivoSalida { get; set; }

    public void obtieneSesion(ISession _session)
    {
      string jsonActualUser = _session.GetString("actualuser");
      string jsonReceptor = _session.GetString("receptor");
      string jsonParaGrupos = _session.GetString("paragrupos");
      this.ArchivoEntrada = _session.GetString("archivoentrada");
      this.ArchivoSalida = _session.GetString("archivosalida");
      if (!String.IsNullOrEmpty(jsonActualUser))
      {
        this.ActualUser = JsonConvert.DeserializeObject<UserData>(jsonActualUser);
      }
      if (!String.IsNullOrEmpty(jsonReceptor))
      {
        this.Receptor = JsonConvert.DeserializeObject<UserData>(jsonReceptor);
      }
      if (!String.IsNullOrEmpty(jsonParaGrupos))
      {
        this.ParaGrupos = JsonConvert.DeserializeObject<List<string>>(jsonParaGrupos);
      }
    }

    public void actualizaSesion(ISession _session)
    {
      string jsonActualUser = JsonConvert.SerializeObject(this.ActualUser);
      string jsonReceptor   = JsonConvert.SerializeObject(this.Receptor);
      string jsonParaGrupos = JsonConvert.SerializeObject(this.ParaGrupos);
      _session.SetString("actualuser", jsonActualUser);
      _session.SetString("receptor", jsonReceptor);
      _session.SetString("paragrupos", jsonParaGrupos);
      if (!String.IsNullOrEmpty(this.ArchivoEntrada))
      {
        _session.SetString("archivoentrada", this.ArchivoEntrada);
      }
      if (!String.IsNullOrEmpty(this.ArchivoSalida))
      {
        _session.SetString("archivosalida", this.ArchivoSalida);
      }
    }


  }

}
