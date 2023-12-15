using Examen3PM2.Models;
using Microsoft.Maui.Controls;
using System;
using Plugin.Media;
using Plugin.Maui.Audio;




namespace Examen3PM2
{
    public partial class MainPage : ContentPage
    {
        readonly IAudioManager _audioManager;
            readonly IAudioRecorder _audioRecorder;

        public MainPage(IAudioManager audioManager)
        {
            InitializeComponent();
             _audioManager = audioManager;
             _audioRecorder = audioManager.CreateRecorder();
        }

        Plugin.Media.Abstractions.MediaFile photo_camera = null;

        public byte[] image_to_array_byte()
        {
            if (photo_camera != null)
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    Stream stream = photo_camera.GetStream();
                    stream.CopyTo(memory);
                    byte[] data = memory.ToArray();
                    return data;
                }
            }

            return null;
        }

        private async void btnguardar_Clicked(object sender, EventArgs e)
        {
            string descripcion = descripcion_txt.Text;
            DateTime fechaini = fechaPicker.Date;
            byte[] image_to_array_bytes = image_to_array_byte();

            if (string.IsNullOrWhiteSpace(descripcion))
            {
                await DisplayAlert("Error", "Por favor, completa todos los campos.", "OK");
                return;
            }

            try
            {
                var firebaseInstance = Singleton.Instance;
                Notas alumno = new Notas { Descripcion = descripcion, Fecha = fechaini, Photo_Record = image_to_array_bytes};

                await firebaseInstance.CreateData(alumno);

                await DisplayAlert("Éxito", "Datos subidos correctamente.", "OK");

                descripcion_txt.Text = string.Empty;
                
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al subir datos: {ex.Message}", "OK");
            }

        }

        private async void btn_photo_Clicked(object sender, EventArgs e)
        {
            photo_camera = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "MiAlbum",
                Name = "Foto.jpg",
                SaveToAlbum = true
            });

            if (photo_camera != null)
            {
                photo.Source = ImageSource.FromStream(() => {
                    return photo_camera.GetStream();
                });
            }

        }

        private async void VerLista_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ListaNotasPage());
        }

        private async void btnGrabarAudio_Clicked(object sender, EventArgs e)
        {
            if(await Permissions.RequestAsync<Permissions.Microphone>() != PermissionStatus.Granted){
                return;
            }

            if(!_audioRecorder.IsRecording)
                {
                await _audioRecorder.StartAsync();

            }
            else {
                var recordeAudio = await _audioRecorder.StopAsync();

                var player = AudioManager.Current.CreatePlayer(recordeAudio.GetAudioStream());
                player.Play();
            }


        }
    }
}