using Examen3PM2.Controllers;
using Examen3PM2.Models;
using Microsoft.Maui.Controls;
using Plugin.Media;
using System;

namespace Examen3PM2;

public partial class actualizarnotasPage : ContentPage
{
    private Notas calificacionSeleccionada;
    public actualizarnotasPage()
	{
		InitializeComponent();
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

    public void SetCalificaciones(Notas Alum)
    {
       calificacionSeleccionada = Alum;

        descripcion_txt.Text = Alum.Descripcion;
       fechaPicker.Date = Alum.Fecha;
       
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

    private async void btnActualizar_Clicked(object sender, EventArgs e)
    {
        if (calificacionSeleccionada != null)
        {
            string descripAntigua = calificacionSeleccionada.Descripcion;
            DateTime fechaAntigua = calificacionSeleccionada.Fecha;
            byte[] antiguaphoto = calificacionSeleccionada.Photo_Record;

            calificacionSeleccionada.Descripcion = descripcion_txt.Text;
            calificacionSeleccionada.Fecha = fechaPicker.Date;
            calificacionSeleccionada.Photo_Record = image_to_array_byte();


            try
            {
                var firebaseInstance = Singleton.Instance;
   
                await firebaseInstance.UpdateData(calificacionSeleccionada.Id_Notas.ToString(), calificacionSeleccionada);

                await DisplayAlert("Éxito", "Notas actualizadas correctamente.", "OK");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                calificacionSeleccionada.Descripcion = descripAntigua;
                calificacionSeleccionada.Fecha = fechaAntigua;
                calificacionSeleccionada.Photo_Record = antiguaphoto;

                await DisplayAlert("Error", $"Error al actualizar Notas: {ex.Message}", "OK");
            }
        }
        else
        {
            await DisplayAlert("Error", "No se ha seleccionado ninguna Nota para actualizar.", "OK");
        }

    }

    private async void volverlista_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ListaNotasPage());
    }
}