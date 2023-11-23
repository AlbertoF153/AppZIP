using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppZIP
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

        }

        private async void Descargar(object sender, EventArgs e)
        {
            try
            {
                DateTime fechaActual = DateTime.Now;
                string formatoFecha = "yyyyMMdd-HHmmss";
                string nombreArchivo = $"FICHA-{fechaActual.ToString(formatoFecha)}";
                string carpetaDestino = Path.Combine("/storage/emulated/0/Download", $"FI-EVIDENCIAS-{fechaActual.ToString(formatoFecha)}");

                if (!Directory.Exists(carpetaDestino))
                {
                    Directory.CreateDirectory(carpetaDestino);
                }

                string[] urlImagen = { "https://cdn.pixabay.com/photo/2023/10/12/12/55/woman-8310751_1280.jpg", "https://cdn.pixabay.com/photo/2023/10/17/02/14/lotus-8320293_1280.jpg", "https://cdn.pixabay.com/photo/2023/10/01/13/51/monitor-lizard-8287432_1280.jpg" };   // URL de la imagen
                string archivoZipNombre = Path.Combine(carpetaDestino, $"{nombreArchivo}.zip");

                using (HttpClient cliente = new HttpClient())
                {
                    using (FileStream fs = new FileStream(archivoZipNombre, FileMode.Create))
                    {
                        using (ZipArchive archivoZip = new ZipArchive(fs, ZipArchiveMode.Create))
                        {
                            foreach (var item in urlImagen)
                            {
                                using (Stream contenidoStream = await cliente.GetStreamAsync(item))
                                {
                                    await AgregarArchivoAsync(archivoZip, contenidoStream, Path.GetFileName(item));
                                }
                            }
                        }
                    }
                }

                Console.WriteLine($"Archivo ZIP creado: {archivoZipNombre}");
                await DisplayAlert("Éxito", "Archivo ZIP creado", "OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                await DisplayAlert("Error", "Se produjo un error al descargar el archivo", "OK");
            }
        }

        private async Task AgregarZip(HttpClient cliente, string[] url, string carpetaDestino, string archivoZipNombre)
        {
            foreach (var item in url)
            {
                using (Stream contenidoStream = await cliente.GetStreamAsync(item))
                {
                    await AgregarArchivoAsync(contenidoStream, Path.GetFileName(item), carpetaDestino, archivoZipNombre);
                }

            }
            Console.WriteLine($"Archivo agregado al ZIP desde URL: {url}");
        }

        private async Task AgregarArchivoAsync(Stream contenidoStream, string nombreArchivo, string carpetaDestino, string archivoZipNombre)
        {
            using (FileStream fs = new FileStream(archivoZipNombre, FileMode.Create))
            {
                using (ZipArchive archivoZip = new ZipArchive(fs, ZipArchiveMode.Create))
                {
                    await AgregarArchivoAsync(archivoZip, contenidoStream, nombreArchivo);
                }
            }

            Console.WriteLine($"Archivo ZIP: {archivoZipNombre}");
        }

        private async Task AgregarArchivoAsync(ZipArchive archivoZip, Stream contenidoStream, string nombreArchivo)
        {
            await Task.Run(() =>
            {
                ZipArchiveEntry entrada = archivoZip.CreateEntry(nombreArchivo);
                using (Stream stream = entrada.Open())
                {
                    contenidoStream.CopyTo(stream);
                }
            });
        }
    }
}





