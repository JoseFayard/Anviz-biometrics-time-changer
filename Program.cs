using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Anviz.SDK;
using static System.Console;

namespace FechaHoraAnviz {
    class Program {
        static async Task Main (string[] args) {
            var manager = new AnvizManager ();
            var contador = 1;
            // Lista con las IP de los captahuellas
            List<string> direccionesIp = new List<string> () { "10.1.1.1", "10.1.1.2" };
            // Recorremos la lista para actualizar la hora de los captahuellas
            foreach (var ip in direccionesIp) {
                await ActualizarHora (manager, ip, contador);
                contador++;
            }
            // Cerramos la consola
            Environment.Exit (0);
        }
        // Método para cambiar la hora de los captahuellas
        private static async Task ActualizarHora (AnvizManager av_manager, string ip, int id) {
            try {
                // Nos conectamos al dispositivo a través de su IP
                using (var device = await av_manager.Connect (ip)) {

                    // Tratamos de cambiarle la hora
                    try {
                        // Tiempo actual del Servidor
                        var now = DateTime.Now;
                        // Tiempo actual del captahuellas
                        var deviceTime = await device.GetDateTime ();
                        // Realizamos una comprobación para saber si es necesario actualizar la hora del dispositivo
                        if (Math.Abs ((now - deviceTime).TotalSeconds) > 30) {
                            // Actualizamos la hora del dispositivo
                            await device.SetDateTime (now);
                            // Escribimos en un archivo TXT "LogCambioDeHoraAutomatico"
                            EscribirTXT ($"Dispositivo {id} IP: {ip} -> La hora ha sido cambiada de manera exitosa.");
                        } else {
                            // Si no es necesario cambiarle la hora lo escribimos en el archivo TXT "LogCambioDeHoraAutomatico"
                            EscribirTXT ($"Dispositivo {id} IP: {ip} -> No hubo necesidad de cambiar la hora.");
                        }
                    
                    } catch (Exception e) { // Capturamos la excepción si ocurre
                        
                        // Esto sucede cuando el dispositivo se enciende ya que en ese momento tiene un formato de hora que no es manejable y por ende salta una "Exception", con este código capturamos la excepción y le cambiamos la hora al dispositivo de forma segura.

                        // Actualizamos la hora a pesar de la excepción
                        await device.SetDateTime (DateTime.Now);
                        // Escribimos en el archivo TXT "LogCambioDeHoraAutomatico"
                        EscribirTXT ($"Dispositivo {id} IP: {ip} -> Ha ocurrido un error, aún así la hora ha sido actualizada de manera exitosa: {e.Message}");
                    }
                }
            } catch (Exception e) { // Capturamos la excepción en caso de no poder conectarnos al captahuellas
                EscribirTXT ($"Dispositivo {id} IP: {ip} -> Ha ocurrido un error: {e.Message}");
            }
        }
        // Método para escribir en un archivo TXT dentro de "Documentos" llamado "LogCambioDeHoraAutomatico"
        private static void EscribirTXT (string textoAEscribir) {
            // Tratamos de escribir en el archivo
            try {
                // Ruta de la carpeta "Documentos"
                string docPath = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
                // Creamos el archivo y escribimos en el, si ya existe solo escribimos y ya :D
                File.AppendAllText (Path.Combine (docPath, "LogCambioDeHoraAutomatico.txt"), textoAEscribir + "   " + DateTime.Now + Environment.NewLine);
            } catch (Exception e) { // Capturamos la excepción en caso de no poder escribir en el archivo TXT
                WriteLine ("Oh no! Ha ocurrido un problema: " + e.Message);
            }
        }
    }
}
