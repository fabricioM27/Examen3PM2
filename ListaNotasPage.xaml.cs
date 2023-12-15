using Firebase.Database;
using Examen3PM2.Models;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Examen3PM2;

public partial class ListaNotasPage : ContentPage
{
    public ObservableCollection<Notas> NotasList { get; set; }

	public ListaNotasPage()
	{
		InitializeComponent();

        NotasList = new ObservableCollection<Notas>();
        list.ItemsSource = NotasList;
        LoadData();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await LoadData();
    }

    public void SetNotasList(ObservableCollection<Notas> notas)
    {
        var notasOrdenadas = notas.OrderByDescending(n => n.Fecha);
        NotasList.Clear();
        foreach (var alum in notasOrdenadas)
        {
            NotasList.Add(alum);
        }
    }


    private async Task LoadData()
    {
        try
        {
            var firebaseInstance = Singleton.Instance;

            var alumnos = await firebaseInstance.ReadData();

            SetNotasList(new ObservableCollection<Notas>(alumnos));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cargar Notas: {ex.Message}", "OK");
        }
    }

    private async void list_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null)
            return;

        var selecnotas = (Notas)e.SelectedItem;

        var action = await DisplayActionSheet($"Opciones para {selecnotas.Id_Notas}", "Cancelar", null, "Editar", "Eliminar", "Ver Foto Recuerdo");

        switch (action)
        {
            case "Editar":
                var update = new actualizarnotasPage();
                update.BindingContext = selecnotas;
                update.SetCalificaciones(selecnotas);
                await Navigation.PushAsync(update);
                break;
            case "Ver Foto Recuerdo":
                    var fotosrecu = new VerfotosPage();
                    fotosrecu.BindingContext = selecnotas;
                    await Navigation.PushAsync(fotosrecu);
                break;
            case "Eliminar":
                var firebaseInstance = Singleton.Instance;
                try
                {

                    Console.WriteLine("error: " + selecnotas.Id_Notas);
                    await firebaseInstance.DeleteData(selecnotas.Id_Notas.ToString());
                    await LoadData();
                    await DisplayAlert("Éxito", "Nota eliminada correctamente.", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Error al eliminar Nota: {ex.Message}", "OK");
                }
                break;
        }

        list.SelectedItem = null;

    }
}