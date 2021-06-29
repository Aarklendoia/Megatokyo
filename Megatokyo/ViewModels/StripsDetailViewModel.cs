using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Megatokyo.Core.Models;
using Megatokyo.Core.Services;
using Megatokyo.Helpers;

using Microsoft.Toolkit.Mvvm.ComponentModel;

using Windows.UI.Xaml.Navigation;

namespace Megatokyo.ViewModels
{
    public class StripsDetailViewModel : ObservableObject
    {
        private object _selectedImage;

        public object SelectedImage
        {
            get => _selectedImage;
            set
            {
                SetProperty(ref _selectedImage, value);
                ImagesNavigationHelper.UpdateImageId(StripsViewModel.StripsSelectedIdKey, ((SampleImage)SelectedImage)?.ID);
            }
        }

        public ObservableCollection<SampleImage> Source { get; } = new ObservableCollection<SampleImage>();

        public StripsDetailViewModel()
        {
        }

        public async Task LoadDataAsync()
        {
            Source.Clear();

            // Replace this with your actual data
            var data = await SampleDataService.GetImageGalleryDataAsync("ms-appx:///Assets");

            foreach (var item in data)
            {
                Source.Add(item);
            }
        }

        public void Initialize(string selectedImageID, NavigationMode navigationMode)
        {
            if (!string.IsNullOrEmpty(selectedImageID) && navigationMode == NavigationMode.New)
            {
                SelectedImage = Source.FirstOrDefault(i => i.ID == selectedImageID);
            }
            else
            {
                selectedImageID = ImagesNavigationHelper.GetImageId(StripsViewModel.StripsSelectedIdKey);
                if (!string.IsNullOrEmpty(selectedImageID))
                {
                    SelectedImage = Source.FirstOrDefault(i => i.ID == selectedImageID);
                }
            }
        }
    }
}
