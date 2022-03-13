using Megatokyo.Client.Core;
using Megatokyo.Client.Core.Models;
using Megatokyo.Client.Core.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Megatokyo.Client.ViewModels
{
    public class RantsViewModel : ObservableObject
    {
        private readonly IRantsApiClient _RantsApiClient;
        private Rant _selected;

        public Rant Selected
        {
            get { return _selected; }
            set { SetProperty(ref _selected, value); }
        }

        public ObservableCollection<Rant> Rants { get; private set; } = new ObservableCollection<Rant>();

        public RantsViewModel()
        {
            _RantsApiClient = RantsApiClientFactory.Create("https://localhost:44395", "");
        }

        public async Task LoadDataAsync(ListDetailsViewState viewState)
        {
            Rants.Clear();

            IEnumerable<Rant> data = await _RantsApiClient.GetAllRants();

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
