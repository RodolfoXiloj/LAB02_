using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace ListasArtesanales
{
    public class LogWriter
    {
        private string rutaLog;
        public LogWriter()
        {
            rutaLog = "log-LAB01_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt";
            try
            {
                string aux = rutaLog;
                aux = HttpContext.Current.Server.MapPath(rutaLog);
                FileStream temp = File.Create(aux);
                temp.Close();
                rutaLog = aux;
            }
            catch
            {
                rutaLog = "C:\\Users\\" + Environment.UserName + "\\" + rutaLog;
                FileStream temp = File.Create(rutaLog);
                temp.Close();
            }
        }

        public void WriteLog(string mensaje, TimeSpan tiempo)
        {
            string prevlogs = File.ReadAllText(rutaLog);
            StreamWriter escritor = new StreamWriter(rutaLog);
            escritor.Write(prevlogs);
            escritor.WriteLine(DateTime.Now.ToString("LOG yyyy/MM/dd HH:mm:ss"));
            escritor.WriteLine("Evento: " + mensaje);
            escritor.WriteLine("Duracion: " + decimal.Round(Convert.ToDecimal(tiempo.TotalMilliseconds), 3) + "ms");
            escritor.WriteLine("---- LOG END ----");
            escritor.Close();
        }

        public void WriteLog(string mensaje, TimeSpan tiempo, Exception e)
        {
            string prevlogs = File.ReadAllText(rutaLog);
            StreamWriter escritor = new StreamWriter(rutaLog);
            escritor.Write(prevlogs);
            escritor.WriteLine(DateTime.Now.ToString("LOG yyyy/MM/dd HH:mm:ss"));
            escritor.WriteLine("Evento: " + mensaje);
            escritor.WriteLine("Excepcion: " + e.Message);
            escritor.WriteLine("Duracion: " + decimal.Round(Convert.ToDecimal(tiempo.TotalMilliseconds)) + "ms", 3);
            escritor.WriteLine("---- LOG END ----");
            escritor.Close();
        }
    }
}
