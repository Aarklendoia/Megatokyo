using AutoMapper;
using Megatokyo.Client.Core.Models;
using Megatokyo.Client.Core.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Megatokyo.Client.ViewModels
{
    public class RantsViewModel : ObservableObject
    {
        private readonly RantsService _rantsService;
        private Rant _selected;

        public Rant Selected
        {
            get { return _selected; }
            set { SetProperty(ref _selected, value); }
        }

        public ObservableCollection<Rant> Rants { get; private set; } = new ObservableCollection<Rant>();

        public RantsViewModel()
        {
            HttpClientHandler clientHandler = new();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            HttpClient httpClient = new(clientHandler)
            {
                BaseAddress = new Uri("https://localhost:44395")
            };
            //ConfigureHttpClient(httpClient, host, apiKey);
            _rantsService = new RantsService(httpClient);
        }

        public async Task LoadDataAsync(ListDetailsViewState viewState)
        {
            Rants.Clear();

            var data = await _rantsService.GetAllRants();

            foreach (var item in data)
            {
                Rants.Add(item);
            }

            if (viewState == ListDetailsViewState.Both && Rants.Any())
            {
                Selected = Rants.First();
            }
        }
    }
}
