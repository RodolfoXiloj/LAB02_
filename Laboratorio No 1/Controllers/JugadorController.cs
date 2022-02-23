using Laboratorio_No_1.DataBase;
using Laboratorio_No_1.Models;
using ListasArtesanales;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Laboratorio_No_1.Controllers
{
    public class JugadorController : Controller
    {
        BaseDeDatos Datos = BaseDeDatos.getInstance;
        // GET: Jugador

        //-------------------------------------------------------Implementación Lista C#-------------------------------------------
        public ActionResult Index()
        {
            DateTime inicial = DateTime.Now;
            var data = Datos.Players;
            Datos.logger.WriteLog("Mostrar vista index", DateTime.Now.Subtract(inicial));
            return View(data);
        }

        // GET: Jugador/Details/5
        public ActionResult Details(int? id)
        {
            DateTime inicial = DateTime.Now;
            if (id == null)
            {
                Datos.logger.WriteLog("Jugador no encontrado en lista", DateTime.Now.Subtract(inicial));
                return HttpNotFound();
            }
            Jugador JugadorAMostrar = Datos.Players.FirstOrDefault(x => x.Id == id);
            if (JugadorAMostrar== null)
            {
                Datos.logger.WriteLog("Jugador no encontrado en lista", DateTime.Now.Subtract(inicial));
                return HttpNotFound();
            }
            var data = JugadorAMostrar;
            Datos.logger.WriteLog("Mostrar detalles Jugador" + JugadorAMostrar.Id, DateTime.Now.Subtract(inicial));
            return View(data);
        }

        #region Modificadores
        #region crear
        // GET: Jugador/Create
        public ActionResult Create()
        {
            DateTime inicial = DateTime.Now;
            Datos.logger.WriteLog("Mostrar vista Crear Jugador", DateTime.Now.Subtract(inicial));
            return View();
        }

        // POST: Jugador/Create
        [HttpPost]
        public ActionResult Create(Jugador NuevoJugador)
        {
            DateTime inicial = DateTime.Now;
            try
            {
                // TODO: Add insert logic here
                NuevoJugador.Id = ++Datos.ActualID;
                Datos.Players.AddFirst(NuevoJugador);
                Datos.logger.WriteLog("Crear Jugador", DateTime.Now.Subtract(inicial));
                return RedirectToAction("Index");
            }
            catch
            {
                Datos.logger.WriteLog("Falla en Crear Jugador", DateTime.Now.Subtract(inicial));
                return View();
            }
        }
        #endregion

        #region editar
        // GET: Jugador/Edit/5
        public ActionResult Edit(int? id)
        {
            DateTime inicial = DateTime.Now;
            if (id==null)
            {
                Datos.logger.WriteLog("Jugador no encontrado en lista", DateTime.Now.Subtract(inicial));
                return HttpNotFound();
            }
            Jugador JugadorBuscado = Datos.Players.FirstOrDefault(x => x.Id == id);
            if (JugadorBuscado == null)
            {
                Datos.logger.WriteLog("Jugador no encontrado en lista", DateTime.Now.Subtract(inicial));
                return HttpNotFound();
            }
            Datos.logger.WriteLog("Mostar vista Ediar Jugador "+JugadorBuscado.Id, DateTime.Now.Subtract(inicial));
            return View(JugadorBuscado);
        }

        // POST: Jugador/Edit/5
        [HttpPost]
        public ActionResult Edit(FormCollection Collection)
        {
            DateTime inicial = DateTime.Now;
            try
            {
                Jugador JugadorBuscado = Datos.Players.FirstOrDefault(x => x.Id == int.Parse(Collection["Id"]));
                if (JugadorBuscado == null)
                {
                    Datos.logger.WriteLog("Jugador no encontrado en lista", DateTime.Now.Subtract(inicial));
                    return HttpNotFound();
                }
                JugadorBuscado.Name = Collection["Name"];
                JugadorBuscado.LastName = Collection["LastName"];
                JugadorBuscado.position = Collection["Position"];
                JugadorBuscado.Salary = double.Parse(Collection["Salary"]);
                JugadorBuscado.Club = Collection["Club"];
                Datos.logger.WriteLog("Editar Jugador " + JugadorBuscado.Id, DateTime.Now.Subtract(inicial));
                return RedirectToAction("Index");
            }
            catch
            {
                Datos.logger.WriteLog("Falla en Editar Jugador", DateTime.Now.Subtract(inicial));
                return View();
            }
        }
        #endregion

        #region eliminar
        // GET: Jugador/Delete/5
        public ActionResult Delete(int? id)
        {
            DateTime inicial = DateTime.Now;
            if (id == null)
            {
                Datos.logger.WriteLog("Jugador no encontrado en lista", DateTime.Now.Subtract(inicial));
                return HttpNotFound();
            }
            Jugador JugadorABorrar = Datos.Players.FirstOrDefault(x => x.Id == id);
            if (JugadorABorrar == null)
            {
                Datos.logger.WriteLog("Jugador no encontrado en lista", DateTime.Now.Subtract(inicial));
                return HttpNotFound();
            }
            Datos.logger.WriteLog("Mostar vista Eliminar Jugador " + JugadorABorrar.Id, DateTime.Now.Subtract(inicial));
            return View(JugadorABorrar);
        }

        // POST: Jugador/Delete/5
        [HttpPost]
        public ActionResult Delete(int id,FormCollection collection)
        {
            DateTime inicial = DateTime.Now;
            try
            {
                // TODO: Add delete logic here
                Jugador JugadorABorrar = Datos.Players.FirstOrDefault(x => x.Id == id);
                if (JugadorABorrar == null)
                {
                    Datos.logger.WriteLog("Jugador no encontrado en lista", DateTime.Now.Subtract(inicial));
                    HttpNotFound();
                }
                Datos.Players.Remove(JugadorABorrar);
                Datos.logger.WriteLog("Eliminar Jugador" + JugadorABorrar.Id, DateTime.Now.Subtract(inicial));
                return RedirectToAction("Index");
            }
            catch
            {
                Datos.logger.WriteLog("Falla en Eliminar Jugador", DateTime.Now.Subtract(inicial));
                return View();
            }
        }
        #endregion

        #region importar
        //IMPORTAR ARCHIVO CSV

        public ActionResult Upload()
        {
            DateTime inicial = DateTime.Now;
            Datos.logger.WriteLog("Mostrar vista Importar .csv", DateTime.Now.Subtract(inicial));
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase upload)
        {
            DateTime inicial = DateTime.Now;
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    if (upload.FileName.EndsWith(".csv"))
                    {
                        Stream stream = upload.InputStream;
                        StreamReader Lector = new StreamReader(stream);
                        string Linea;
                        //Lee la primera linea, se salta a la segunda porque la primera solo es formato.
                        Linea = Lector.ReadLine();
                        Linea = Lector.ReadLine();
                        while (Linea != null)
                        {
                            try
                            {
                                //split de la linea
                                string[] Atributos = Linea.Split(',');
                                //Instacia del jugador
                                Jugador NuevoJugador = new Jugador();
                                //Asignación de atributos al objeto
                                NuevoJugador.Club = Atributos[0];
                                NuevoJugador.LastName = Atributos[1];
                                NuevoJugador.Name = Atributos[2];
                                NuevoJugador.position = Atributos[3];
                                NuevoJugador.Salary = double.Parse(Atributos[4]);
                                NuevoJugador.Id = ++Datos.ActualID;
                                //Agregar jugador a la lista.
                                Datos.Players.AddLast(NuevoJugador);
                            }
                            catch(Exception e)
                            {
                                ModelState.AddModelError("File", "Data incompatible");
                                Datos.logger.WriteLog("Falla en carga de .csv", DateTime.Now.Subtract(inicial), e);
                                return View();
                            }
                            //Leer siguiente linea
                            Linea  = Lector.ReadLine();
                        }
                        var data = Datos.Players;
                        Datos.logger.WriteLog(stream + " importado con exito", DateTime.Now.Subtract(inicial));
                        return View("Index", data);
                    }
                    else
                    {
                        ModelState.AddModelError("File", "This file format is not supported");
                        Datos.logger.WriteLog("Falla en carga de .csv", DateTime.Now.Subtract(inicial));
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("File", "Please Upload Your file");
                }
            }
            Datos.logger.WriteLog("Falla en carga de .csv", DateTime.Now.Subtract(inicial));
            return View();
        }
        #endregion

        #region eliminarCsv
        public ActionResult Deleting()
        {
            DateTime inicial = DateTime.Now;
            Datos.logger.WriteLog("Mostrar vista Borrar por .csv", DateTime.Now.Subtract(inicial));
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Deleting(HttpPostedFileBase upload)
        {
            DateTime inicial = DateTime.Now;
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    if (upload.FileName.EndsWith(".csv"))
                    {
                        Stream stream = upload.InputStream;
                        StreamReader Lector = new StreamReader(stream);
                        string Linea;
                        //Lee la primera linea, se salta a la segunda porque la primera solo es formato.
                        Linea = Lector.ReadLine();
                        Linea = Lector.ReadLine();
                        while (Linea != null)
                        {
                            try
                            {
                                //split de la linea
                                string[] Atributos = Linea.Split(',');
                                Jugador JugadorABorrar = Datos.Players.FirstOrDefault(
                                    x => x.Club == Atributos[0] &&
                                    x.LastName == Atributos[1] &&
                                    x.Name == Atributos[2] &&
                                    x.position == Atributos[3] &&
                                    x.Salary == double.Parse(Atributos[4]));
                                if (JugadorABorrar != null)
                                {
                                    Datos.Players.Remove(JugadorABorrar);
                                }
                            }
                            catch(Exception e)
                            {
                                ModelState.AddModelError("File", "Data incompatible");
                                Datos.logger.WriteLog("Falla en carga de .csv", DateTime.Now.Subtract(inicial), e);
                                return View();
                            }
                            //Leer siguiente linea
                            Linea = Lector.ReadLine();
                        }
                        var data = Datos.Players;
                        Datos.logger.WriteLog("datos en " + stream + " borrados con exito", DateTime.Now.Subtract(inicial));
                        return View("Index", data);
                    }
                    else
                    {
                        ModelState.AddModelError("File", "This file format is not supported");
                        Datos.logger.WriteLog("Falla en carga de .csv", DateTime.Now.Subtract(inicial));
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("File", "Please Upload Your file");
                }
            }
            Datos.logger.WriteLog("Falla en carga de .csv", DateTime.Now.Subtract(inicial));
            return View();
        }
        #endregion
        #endregion

        #region Búsquedas
        #region PorNombre
        public ActionResult SerchByName()
        {
            DateTime inicial = DateTime.Now;
            var data = Datos.Players;
            Datos.logger.WriteLog("Mostrar vista Busqueda", DateTime.Now.Subtract(inicial));
            return View(data);
        }

        [HttpPost]
        public ActionResult SerchByName(FormCollection Collection)
        {
            DateTime inicial = DateTime.Now;
            Datos.SearchedPlayers.Clear();
            Datos.SearchedPlayers = Datos.Players.Search(x => x.Name.ToUpper() == Collection["Name"].ToUpper());
            var data = Datos.SearchedPlayers;
            Datos.logger.WriteLog("Resultado Busqueda por Nombre " + Collection["Name"], DateTime.Now.Subtract(inicial));
            return View(data);
        }
        #endregion

        #region PorApellido
        public ActionResult SearchByLastName()
        {
            DateTime inicial = DateTime.Now;
            var data = Datos.Players;
            Datos.logger.WriteLog("Mostrar vista Busqueda", DateTime.Now.Subtract(inicial));
            return View(data);
        }

        [HttpPost]
        public ActionResult SearchByLastName(FormCollection Collection)
        {
            DateTime inicial = DateTime.Now;
            Datos.SearchedPlayers.Clear();
            Datos.SearchedPlayers = Datos.Players.Search(x => x.LastName.ToUpper() == Collection["LastName"].ToUpper());
            var data = Datos.SearchedPlayers;
            Datos.logger.WriteLog("Resultado Busqueda por Apellido " + Collection["LastName"], DateTime.Now.Subtract(inicial));
            return View(data);

        }
        #endregion

        #region PorClub
        public ActionResult SearchByClub()
        {
            DateTime inicial = DateTime.Now;
            var data = Datos.Players;
            Datos.logger.WriteLog("Mostrar vista Busqueda", DateTime.Now.Subtract(inicial));
            return View(data);
        }

        [HttpPost]
        public ActionResult SearchByClub(FormCollection Collection)
        {
            DateTime inicial = DateTime.Now;
            Datos.SearchedPlayers.Clear();
            Datos.SearchedPlayers = Datos.Players.Search(x => x.Club.ToUpper() == Collection["Club"].ToUpper());
            var data = Datos.SearchedPlayers;
            Datos.logger.WriteLog("Resultado Busqueda por Club " + Collection["Club"], DateTime.Now.Subtract(inicial));
            return View(data);
        }
        #endregion

        #region PorPosición
        public ActionResult SearchByPosition()
        {
            DateTime inicial = DateTime.Now;
            var data = Datos.Players;
            Datos.logger.WriteLog("Mostrar vista Busqueda", DateTime.Now.Subtract(inicial));
            return View(data);
        }

        [HttpPost]
        public ActionResult SearchByPosition(FormCollection Collection)
        {
            DateTime inicial = DateTime.Now;
            Datos.SearchedPlayers.Clear();
            Datos.SearchedPlayers = Datos.Players.Search(x => x.position.ToUpper() == Collection["Position"].ToUpper());
            var data = Datos.SearchedPlayers;
            Datos.logger.WriteLog("Resultado Busqueda por Posicion " + Collection["Position"], DateTime.Now.Subtract(inicial));
            return View(data);
        }
        #endregion

        #region PorSalario
        public ActionResult SearchBySalary(int Order)
        {
            DateTime inicial = DateTime.Now;
            var data = Datos.Players;
            Datos.logger.WriteLog("Mostrar vista Busqueda", DateTime.Now.Subtract(inicial));
            return View(data);
        }

        [HttpPost]
        public ActionResult SearchBySalary(int order, FormCollection Collection)
        {
            DateTime inicial = DateTime.Now;
            Datos.SearchedPlayers.Clear();
            if (order == 1)
            {             
                    try
                    {
                    Datos.SearchedPlayers = Datos.Players.Search(x => x.Salary > int.Parse(Collection["Salary"]));
                }
                    catch (Exception e)
                    {
                        Datos.logger.WriteLog(
                            "Falla en Busqueda por Salario " + Collection["Salary"],
                            DateTime.Now.Subtract(inicial),
                            e);
                        HttpNotFound();
                    }
                
            }
            else if(order == 2)
            {
                
                    try
                    {
                        Datos.SearchedPlayers = Datos.Players.Search(x => x.Salary < int.Parse(Collection["Salary"]));
                    }
                    catch (Exception e)
                    {
                        Datos.logger.WriteLog(
                            "Falla en Busqueda por Salario " + Collection["Salary"],
                            DateTime.Now.Subtract(inicial),
                            e);
                        HttpNotFound();
                    }
                
            }
            else
            {
                
                    try
                    {
                        Datos.SearchedPlayers = Datos.Players.Search(x => x.Salary == int.Parse(Collection["Salary"]));
                    }
                    catch (Exception e)
                    {
                        Datos.logger.WriteLog(
                            "Falla en Busqueda por Salario " + Collection["Salary"],
                            DateTime.Now.Subtract(inicial),
                            e);
                        HttpNotFound();
                    }
                
            }
            var data = Datos.SearchedPlayers;
            string comparacion = order == 1 ? "mayor que" : order == 2 ? "menor que" : "igual a";
            comparacion += " ";
            Datos.logger.WriteLog("Resultado Busqueda por Salario "+ comparacion + Collection["Salary"], DateTime.Now.Subtract(inicial));
            return View(Datos.SearchedPlayers);
        }
        #endregion
        #endregion
        //-------------------------------------------------------Implementación Lista Artesanal------------------------------------
        public ActionResult IndexGeneric()
        {
            DateTime inicial = DateTime.Now;
            var data = Datos.PlayersG;
            Datos.logger.WriteLog("Mostrar vista index", DateTime.Now.Subtract(inicial));
            return View(data);
        }

        public ActionResult DetailsGeneric(int? id)
        {
            DateTime inicial = DateTime.Now;
            if (id == null)
            {
                Datos.logger.WriteLog("Jugador no encontrado en lista", DateTime.Now.Subtract(inicial));
                return HttpNotFound();
            }
            Jugador JugadorAMostrar = Datos.PlayersG.FirstOrDefault(x => x.Id == id);
            if (JugadorAMostrar == null)
            {
                Datos.logger.WriteLog("Jugador no encontrado en lista", DateTime.Now.Subtract(inicial));
                return HttpNotFound();
            }
            var data = JugadorAMostrar;
            Datos.logger.WriteLog("Mostrar detalles Jugador" + JugadorAMostrar.Id, DateTime.Now.Subtract(inicial));
            return View(data);
        }

        #region Modificadores
        #region crear
        public ActionResult CreateGeneric()
        {
            DateTime inicial = DateTime.Now;
            Datos.logger.WriteLog("Mostrar vista Crear Jugador", DateTime.Now.Subtract(inicial));
            return View();
        }

        // POST: Jugador/Create
        [HttpPost]
        public ActionResult CreateGeneric(Jugador NuevoJugador)
        {
            DateTime inicial = DateTime.Now;
            try
            {
                // TODO: Add insert logic here
                NuevoJugador.Id = ++Datos.ActualIDG;
                Datos.PlayersG.Add(NuevoJugador);
                Datos.logger.WriteLog("Crear Jugador", DateTime.Now.Subtract(inicial));
                return RedirectToAction("IndexGeneric");
            }
            catch
            {
                Datos.logger.WriteLog("Falla en Crear Jugador", DateTime.Now.Subtract(inicial));
                return View();
            }
        }
        #endregion

        #region importar
        public ActionResult UploadGeneric()
        {
            DateTime inicial = DateTime.Now;
            Datos.logger.WriteLog("Mostrar vista Importar .csv", DateTime.Now.Subtract(inicial));
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadGeneric(HttpPostedFileBase upload)
        {
            DateTime inicial = DateTime.Now;
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    if (upload.FileName.EndsWith(".csv"))
                    {
                        Stream stream = upload.InputStream;
                        StreamReader Lector = new StreamReader(stream);
                        string Linea;
                        //Lee la primera linea, se salta a la segunda porque la primera solo es formato.
                        Linea = Lector.ReadLine();
                        Linea = Lector.ReadLine();
                        while (Linea != null)
                        {
                            try
                            {
                                //split de la linea
                                string[] Atributos = Linea.Split(',');
                                //Instacia del jugador
                                Jugador NuevoJugador = new Jugador();
                                //Asignación de atributos al objeto
                                NuevoJugador.Club = Atributos[0];
                                NuevoJugador.LastName = Atributos[1];
                                NuevoJugador.Name = Atributos[2];
                                NuevoJugador.position = Atributos[3];
                                NuevoJugador.Salary = double.Parse(Atributos[4]);
                                NuevoJugador.Id = ++Datos.ActualIDG;
                                //Agregar jugador a la lista.
                                Datos.PlayersG.Add(NuevoJugador);
                            }
                            catch (Exception e)
                            {
                                ModelState.AddModelError("File", "Data incompatible");
                                Datos.logger.WriteLog("Falla en carga de .csv", DateTime.Now.Subtract(inicial), e);
                                return View();
                            }
                            //Leer siguiente linea
                            Linea = Lector.ReadLine();
                        }
                        var data = Datos.PlayersG;
                        Datos.logger.WriteLog(stream + " importado con exito", DateTime.Now.Subtract(inicial));
                        return View("IndexGeneric", data);
                    }
                    else
                    {
                        ModelState.AddModelError("File", "This file format is not supported");
                        Datos.logger.WriteLog("Falla en carga de .csv", DateTime.Now.Subtract(inicial));
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("File", "Please Upload Your file");
                }
            }
            Datos.logger.WriteLog("Falla en carga de .csv", DateTime.Now.Subtract(inicial));
            return View();
        }
        #endregion

        #region editar
        public ActionResult EditGeneric(int? id)
        {
            DateTime inicial = DateTime.Now;
            if (id == null)
            {
                Datos.logger.WriteLog("Jugador no encontrado en lista", DateTime.Now.Subtract(inicial));
                return HttpNotFound();
            }

            Jugador JugadorBuscado = Datos.PlayersG.FirstOrDefault(x => x.Id == id);
            if (JugadorBuscado == null)
            {
                Datos.logger.WriteLog("Jugador no encontrado en lista", DateTime.Now.Subtract(inicial));
                return HttpNotFound();
            }
            Datos.logger.WriteLog("Mostar vista Ediar Jugador " + JugadorBuscado.Id, DateTime.Now.Subtract(inicial));
            return View(JugadorBuscado);
        }

        // POST: Jugador/Edit/5
        [HttpPost]
        public ActionResult EditGeneric(FormCollection Collection)
        {
            DateTime inicial = DateTime.Now;
            try
            {
                Jugador JugadorBuscado = Datos.PlayersG.FirstOrDefault(x => x.Id == int.Parse(Collection["Id"]));
                if (JugadorBuscado == null)
                {
                    Datos.logger.WriteLog("Jugador no encontrado en lista", DateTime.Now.Subtract(inicial));
                    return HttpNotFound();
                }
                JugadorBuscado.Name = Collection["Name"];
                JugadorBuscado.LastName = Collection["LastName"];
                JugadorBuscado.position = Collection["Position"];
                JugadorBuscado.Salary = double.Parse(Collection["Salary"]);
                JugadorBuscado.Club = Collection["Club"];
                Datos.logger.WriteLog("Editar Jugador " + JugadorBuscado.Id, DateTime.Now.Subtract(inicial));
                return RedirectToAction("IndexGeneric");
            }
            catch
            {
                Datos.logger.WriteLog("Falla en Editar Jugador", DateTime.Now.Subtract(inicial));
                return View();
            }
        }
        #endregion

        #region eliminar
        public ActionResult DeleteGeneric(int? id)
        {
            DateTime inicial = DateTime.Now;
            if (id == null)
            {
                Datos.logger.WriteLog("Jugador no encontrado en lista", DateTime.Now.Subtract(inicial));
                return HttpNotFound();
            }
            Jugador JugadorABorrar = Datos.PlayersG.FirstOrDefault(x => x.Id == id);
            if (JugadorABorrar == null)
            {
                Datos.logger.WriteLog("Jugador no encontrado en lista", DateTime.Now.Subtract(inicial));
                return HttpNotFound();
            }
            Datos.logger.WriteLog("Mostar vista Eliminar Jugador " + JugadorABorrar.Id, DateTime.Now.Subtract(inicial));
            return View(JugadorABorrar);
        }

        // POST: Jugador/Delete/5
        [HttpPost]
        public ActionResult DeleteGeneric(int id, FormCollection collection)
        {
            DateTime inicial = DateTime.Now;
            try
            {
                // TODO: Add delete logic here
                Jugador JugadorABorrar = Datos.PlayersG.FirstOrDefault(x => x.Id == id);
                if (JugadorABorrar == null)
                {
                    Datos.logger.WriteLog("Jugador no encontrado en lista", DateTime.Now.Subtract(inicial));
                    HttpNotFound();
                }
                Datos.PlayersG.Remove(JugadorABorrar);
                Datos.logger.WriteLog("Eliminar Jugador" + JugadorABorrar.Id, DateTime.Now.Subtract(inicial));
                return RedirectToAction("IndexGeneric");
            }
            catch
            {
                Datos.logger.WriteLog("Falla en Eliminar Jugador", DateTime.Now.Subtract(inicial));
                return View();
            }
        }
        #endregion

        #region eliminarCsv
        public ActionResult DeletingGeneric()
        {
            DateTime inicial = DateTime.Now;
            Datos.logger.WriteLog("Mostrar vista Borrar por .csv", DateTime.Now.Subtract(inicial));
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletingGeneric(HttpPostedFileBase upload)
        {
            DateTime inicial = DateTime.Now;
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    if (upload.FileName.EndsWith(".csv"))
                    {
                        Stream stream = upload.InputStream;
                        StreamReader Lector = new StreamReader(stream);
                        string Linea;
                        //Lee la primera linea, se salta a la segunda porque la primera solo es formato.
                        Linea = Lector.ReadLine();
                        Linea = Lector.ReadLine();
                        while (Linea != null)
                        {
                            try
                            {
                                //split de la linea
                                string[] Atributos = Linea.Split(',');
                                Jugador JugadorABorrar = Datos.PlayersG.FirstOrDefault(
                                    x => x.Club == Atributos[0] &&
                                    x.LastName == Atributos[1] &&
                                    x.Name == Atributos[2] &&
                                    x.position == Atributos[3] &&
                                    x.Salary == double.Parse(Atributos[4]));
                                if (JugadorABorrar != null)
                                {
                                    Datos.PlayersG.Remove(JugadorABorrar);
                                }
                            }
                            catch(Exception e)
                            {
                                ModelState.AddModelError("File", "Data incompatible");
                                Datos.logger.WriteLog("Falla en carga de .csv", DateTime.Now.Subtract(inicial), e);
                                return View();
                            }
                            //Leer siguiente linea
                            Linea = Lector.ReadLine();
                        }
                        var data = Datos.PlayersG;
                        Datos.logger.WriteLog("datos en " + stream + " borrados con exito", DateTime.Now.Subtract(inicial));
                        return View("IndexGeneric", data);
                    }
                    else
                    {
                        ModelState.AddModelError("File", "This file format is not supported");
                        Datos.logger.WriteLog("Falla en carga de .csv", DateTime.Now.Subtract(inicial));
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("File", "Please Upload Your file");
                }
            }
            Datos.logger.WriteLog("Falla en carga de .csv", DateTime.Now.Subtract(inicial));
            return View();
        }
        #endregion
        #endregion
        #region Búsquedas
        #region PorNombre
        public ActionResult SerchByNameGeneric()
        {
            DateTime inicial = DateTime.Now;
            var data = Datos.PlayersG;
            Datos.logger.WriteLog("Mostrar vista Busqueda", DateTime.Now.Subtract(inicial));
            return View(data);
        }

        [HttpPost]
        public ActionResult SerchByNameGeneric(FormCollection Collection)
        {
            DateTime inicial = DateTime.Now;
            Datos.SearchedPlayersG.Clear();
            Datos.SearchedPlayersG =  Datos.PlayersG.Search(x => x.Name.ToUpper() == Collection["Name"].ToUpper());
            var data = Datos.SearchedPlayersG;
            Datos.logger.WriteLog("Resultado Busqueda por Nombre " + Collection["Name"], DateTime.Now.Subtract(inicial));
            return View(data);
        }
        #endregion

        #region PorApellido
        public ActionResult SearchByLastNameGeneric()
        {
            DateTime inicial = DateTime.Now;
            var data = Datos.PlayersG;
            Datos.logger.WriteLog("Mostrar vista Busqueda", DateTime.Now.Subtract(inicial));
            return View(data);
        }

        [HttpPost]
        public ActionResult SearchByLastNameGeneric(FormCollection Collection)
        {
            DateTime inicial = DateTime.Now;
            Datos.SearchedPlayersG.Clear();
            Datos.SearchedPlayersG = Datos.PlayersG.Search(x => x.LastName.ToUpper() == Collection["LastName"].ToUpper());
            var data = Datos.SearchedPlayersG;
            Datos.logger.WriteLog("Resultado Busqueda por Apellido " + Collection["LastName"], DateTime.Now.Subtract(inicial));
            return View(data);
        }
        #endregion

        #region PorClub
        public ActionResult SearchByClubGeneric()
        {
            DateTime inicial = DateTime.Now;
            var data = Datos.PlayersG;
            Datos.logger.WriteLog("Mostrar vista Busqueda", DateTime.Now.Subtract(inicial));
            return View(data);
        }

        [HttpPost]
        public ActionResult SearchByClubGeneric(FormCollection Collection)
        {
            DateTime inicial = DateTime.Now;
            Datos.SearchedPlayersG.Clear();
            Datos.SearchedPlayersG = Datos.PlayersG.Search(x => x.Club == Collection["Club"]);
            var data = Datos.SearchedPlayersG;
            Datos.logger.WriteLog("Resultado Busqueda por Club " + Collection["Club"], DateTime.Now.Subtract(inicial));
            return View(data);
        }
        #endregion

        #region PorPosicion
        public ActionResult SearchByPositionGeneric()
        {
            DateTime inicial = DateTime.Now;
            var data = Datos.Players;
            Datos.logger.WriteLog("Mostrar vista Busqueda", DateTime.Now.Subtract(inicial));
            return View(data);
        }

        [HttpPost]
        public ActionResult SearchByPositionGeneric(FormCollection Collection)
        {
            DateTime inicial = DateTime.Now;
            Datos.SearchedPlayersG.Clear();
            Datos.SearchedPlayersG = Datos.PlayersG.Search(x => x.position.ToUpper() == Collection["Position"].ToUpper());
            var data = Datos.SearchedPlayersG;
            Datos.logger.WriteLog("Resultado Busqueda por Posicion " + Collection["Position"], DateTime.Now.Subtract(inicial));
            return View(data);
        }
        #endregion

        #region PorSalario
        public ActionResult SearchBySalaryGeneric(int Order)
        {
            DateTime inicial = DateTime.Now;
            var data = Datos.PlayersG;
            Datos.logger.WriteLog("Mostrar vista Busqueda", DateTime.Now.Subtract(inicial));
            return View(data);
        }

        [HttpPost]
        public ActionResult SearchBySalaryGeneric(int order, FormCollection Collection)
        {
            DateTime inicial = DateTime.Now;
            Datos.SearchedPlayersG.Clear();
            if (order == 1)
            {
                
                    try
                    {
                    Datos.SearchedPlayersG = Datos.PlayersG.Search(x => x.Salary > int.Parse(Collection["Salary"]));
                }
                    catch (Exception e)
                    {
                        Datos.logger.WriteLog(
                            "Falla en Busqueda por Salario " + Collection["Salary"],
                            DateTime.Now.Subtract(inicial),
                            e);
                        HttpNotFound();
                    }
                
            }
            else if (order == 2)
            {
                
                    try
                    {
                        Datos.SearchedPlayersG = Datos.PlayersG.Search(x => x.Salary < int.Parse(Collection["Salary"]));
                    }
                    catch (Exception  e)
                    {
                        Datos.logger.WriteLog(
                            "Falla en Busqueda por Salario " + Collection["Salary"],
                            DateTime.Now.Subtract(inicial),
                            e);
                        HttpNotFound();
                    }
                
            }
            else
            {
              
                    try
                    {
                        Datos.SearchedPlayersG = Datos.PlayersG.Search(x => x.Salary == int.Parse(Collection["Salary"]));
                    }
                    catch (Exception e)
                    {
                        Datos.logger.WriteLog(
                            "Falla en Busqueda por Salario " + Collection["Salary"],
                            DateTime.Now.Subtract(inicial),
                            e);
                        HttpNotFound();
                    }
                
            }
            var data = Datos.SearchedPlayersG;
            string comparacion = order == 1 ? "mayor que" : order == 2 ? "menor que" : "igual a";
            comparacion += " ";
            Datos.logger.WriteLog("Resultado Busqueda por Salario " + comparacion + Collection["Salary"], DateTime.Now.Subtract(inicial));
            return View(data);
        }
        #endregion
        #endregion
    }
}
