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
                string nombreArchivo = entryNombreArchivo.Text;

                if (string.IsNullOrWhiteSpace(nombreArchivo))
                {
                    await DisplayAlert("Error", "Por favor, ingresa un nombre para el archivo ZIP", "OK");
                    return;
                }

                string carpetaDestino = Path.Combine("/storage/emulated/0/Download", "Zip");

                if (!Directory.Exists(carpetaDestino))
                {
                    Directory.CreateDirectory(carpetaDestino);
                }

                string urlImagen = "https://cdn.pixabay.com/photo/2023/10/12/12/55/woman-8310751_1280.jpg";   // URL de la imagen
                string archivoZipNombre = Path.Combine(carpetaDestino, $"{nombreArchivo}.zip");

                using (HttpClient cliente = new HttpClient())
                {
                    await AgregarZip(cliente, urlImagen, carpetaDestino, archivoZipNombre);
                }

                if (File.Exists(archivoZipNombre))
                {
                    Console.WriteLine($"Archivo ZIP descargado y existe: {archivoZipNombre}");
                    await DisplayAlert("Éxito", "Archivo ZIP descargado", "OK");
                }
                else
                {
                    Console.WriteLine($"Error: El archivo ZIP no se ha creado correctamente.");
                    await DisplayAlert("Error", "No se pudo crear el archivo ZIP", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                await DisplayAlert("Error", "Se produjo un error al descargar el archivo", "OK");
            }
        }

        private async Task AgregarZip(HttpClient cliente, string url, string carpetaDestino, string archivoZipNombre)
        {
            using (Stream contenidoStream = await cliente.GetStreamAsync(url))
            {
                await AgregarArchivoAsync(contenidoStream, Path.GetFileName(url), carpetaDestino, archivoZipNombre);
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
