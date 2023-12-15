using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;

namespace Examen3PM2.Models
{
    public class Singleton
    {
        private static Singleton instance = null;
        private readonly FirebaseClient firebaseClient;

        private Singleton()
        {
            firebaseClient = new FirebaseClient("https://pruebafiremaui-default-rtdb.firebaseio.com/");
        }

        public static Singleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Singleton();
                }
                return instance;
            }
        }

        public async Task CreateData(Notas data)
        {
            await firebaseClient
                .Child("NotasPAR")
                .PostAsync(data);
        }

        public async Task<List<Notas>> ReadData()
        {
            var NotasList = await firebaseClient
                .Child("NotasPAR")
                .OnceAsync<Notas>();

            return NotasList.Select(item => {
                var notas = item.Object;
                notas.Id_Notas = item.Key; // Asigna el ID de Firebase al objeto people
                return notas;
            }).ToList();
        }

        public async Task UpdateData(string key, Notas data)
        {
            await firebaseClient
                .Child("NotasPAR")
                .Child(key)
                .PutAsync(data);
        }

        public async Task DeleteData(string key)
        {
            await firebaseClient
                .Child("NotasPAR")
                .Child(key)
                .DeleteAsync();
        }

    }
}
