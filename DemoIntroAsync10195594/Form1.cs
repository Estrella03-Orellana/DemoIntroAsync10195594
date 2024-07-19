using System.Diagnostics;
using System.Net.Http.Headers;

namespace DemoIntroAsync10195594
{
    public partial class Form1 : Form
    {
        HttpClient httpClient = new HttpClient();
        public Form1()
        {
            InitializeComponent();
        }

        //Peligroso: async void debe ser evitado, EXCEPTO en eventos.
        private async void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;
            //Proceso lento 
            //var Nombre = await ProcesamientoLargo();
            //MessageBox.Show($"Saludos, {Nombre}");

            var directorioActual = AppDomain.CurrentDomain.BaseDirectory;
            var destinoBaseSecuencial = Path.Combine(directorioActual, @"Imagenes\resultados-secuencial");
            var destinoBaseParalelo = Path.Combine(directorioActual, @"Imagenes\reultadoparalelo");
            PrepararEjecucion(destinoBaseParalelo, destinoBaseSecuencial);

            Console.WriteLine("Inicio");
            List<Imagen> imagenes = ObtenerImagenes();

            // Parte Secuencial
            var sw = new Stopwatch();
            sw.Start();

            foreach (var imagen in imagenes)
            {
              await ProcesarImagen(destinoBaseSecuencial, imagen);
            }
            Console.WriteLine("Secuencial  - duración en segundos: {0}",
                  sw.ElapsedMilliseconds / 1000.0);

            sw.Reset();

            sw.Start();

            var tareasEnumerable = imagenes.Select(async imagen =>
            {
                await ProcesarImagen(destinoBaseParalelo, imagen);
            });

            await Task.WhenAll(tareasEnumerable);
            Console.WriteLine("Paralelo - duracion en segundos: {0}",
                sw.ElapsedMilliseconds / 1000.0);
            //await ProcesamientoLargoA();
            //await ProcesamientoLargoB();
            //await ProcesamientoLargoC();

            //var tareas = new List<Task>()
            //{
            //   ProcesamientoLargoA(),
            //   ProcesamientoLargoB(),
            //   ProcesamientoLargoC()
            //};

            //await Task.WhenAll(tareas);
            sw.Stop();

            //var duracion = $"El programa se ejecuto en {sw.ElapsedMilliseconds / 1000.0} segundos";
            //Console.WriteLine(duracion);
            //pictureBox1.Visible = false;
        }

        private async Task ProcesarImagen(string directorio, Imagen imagen)
        {
            var respuesta = await httpClient.GetAsync(imagen.URL);
            var contenido = await respuesta.Content.ReadAsByteArrayAsync();

            Bitmap bitmap;
            using (var ms = new MemoryStream(contenido))
            {
                bitmap = new Bitmap(ms);
            }

            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            var destino = Path.Combine(directorio, imagen.Nombre);
            bitmap.Save(destino);
        }


        private static List<Imagen> ObtenerImagenes()
        { 
            var imagenes = new List<Imagen>();
            for (int i = 0; i < 7; i++)
            {
                imagenes.Add(
                    new Imagen()
                    {
                        Nombre = $"Cacicazgos {i}.png",
                        URL = "https://es.wikipedia.org/wiki/Archivo:Copia_de_Cacicazgos_de_la_Hispaniola.png"
                    });
                imagenes.Add(
                new Imagen()
                {
                    Nombre = $"Desangles {i}.jpg",
                    URL = "https://upload.wikimedia.org/wikipedia/commons/4/43/Desangles_Colon_engrillado.jpg"
                });
                imagenes.Add(
                    new Imagen()
                    { 
                        Nombre = $"Alcanzar {i}.jpg",
                        URL = "https://upload.wikimedia.org/wikipedia/commons/d/d7/Santo_Domingo_-_Alc%C3%A1zar_de_Col%C3%B3n_0777.JPG"
                    } );
            }
            return imagenes;
        }

        private void BorrarArchivoc(string directorio)
        {
            var archivos = Directory.EnumerateFiles(directorio);
            foreach (var archivo in archivos)
            {
                File.Delete(archivo);
            }
        }

        private void PrepararEjecucion(string destinoBaseParalelo, string destinoBaseSecuencial)
        {
            if (!Directory.Exists(destinoBaseParalelo))
            {
                Directory.CreateDirectory(destinoBaseParalelo);
            }
            if (!Directory.Exists(destinoBaseSecuencial))
            {
                Directory.CreateDirectory(destinoBaseSecuencial);
            }
            BorrarArchivoc(destinoBaseSecuencial);
            BorrarArchivoc (destinoBaseParalelo);
        }

        private async Task<string> ProcesamientoLargo()
        {
            await Task.Delay(5000);
            //MessageBox.Show("Ya pasaron los 3 segundos");
            return "Estrella";
        }

        private async Task ProcesamientoLargoA()
        {
            await Task.Delay(1000);
           Console.WriteLine("Proceso Largo A finalizado");
        }

        private async Task ProcesamientoLargoB()
        {
            await Task.Delay(1000);
            Console.WriteLine("Proceso Largo B finalizado");
        }

        private async Task ProcesamientoLargoC()
        {
            await Task.Delay(1000);
            Console.WriteLine("Proceso Largo C finalizado");
        }

  
    }
}
