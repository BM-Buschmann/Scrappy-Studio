using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Scrappy_Studio.Views
{
    public sealed partial class GeneratePage : Page, INotifyPropertyChanged
    {

        public GeneratePage()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void OnDragOver(object sender, DragEventArgs e)
        {
            // Check if the dragged data contains StorageItems (files)
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
            }
            else
            {
                e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.None;
            }

            e.Handled = true;
        }

        private async void OnFileDrop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();

                // We only consider the first file dropped. You can modify this to handle multiple files if needed.
                if (items.Count > 0 && items[0] is StorageFile file)
                {

                    // Load and display the uploaded image
                    BitmapImage bitmapImage = new BitmapImage();
                    using (var fileStream = await file.OpenAsync(FileAccessMode.Read))
                    {
                        await bitmapImage.SetSourceAsync(fileStream);
                    }
                    uploadedImage.Source = bitmapImage;

                    // Update the stack panel visibility
                    uploadStackpanel.Visibility = Visibility.Collapsed;
                    uploadedStackpanel.Visibility = Visibility.Visible;

                }
            }

            e.Handled = true;
        }

        private void OnUploadIconClick(object sender, RoutedEventArgs e)
        {
            // Open file picker when the upload icon is clicked
            OpenFilePicker();
        }

        private async void OpenFilePicker()
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                // Load and display the selected image
                BitmapImage bitmapImage = new BitmapImage();
                using (var fileStream = await file.OpenAsync(FileAccessMode.Read))
                {
                    await bitmapImage.SetSourceAsync(fileStream);
                }
                uploadedImage.Source = bitmapImage;

                // Update the stack panel visibility
                uploadStackpanel.Visibility = Visibility.Collapsed;
                uploadedStackpanel.Visibility = Visibility.Visible;

            }
        }

        private void OnRemoveImageButtonClick(object sender, RoutedEventArgs e)
        {
            // Remove the image and update stack panel visibility
            uploadedImage.Source = null;
            uploadStackpanel.Visibility = Visibility.Visible;
            uploadedStackpanel.Visibility = Visibility.Collapsed;

        }

        private void OnSliderImageSizeValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            // Update the opacity of the uploadedImage based on the slider value
            double opacityValue = e.NewValue / 100.0;
            uploadedImage.Opacity = opacityValue;
        }

        private void AspectratioSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            string[] aspectRatios = { "7 : 4", "3 : 2", "4 : 3", "5 : 4", "1 : 1", "4 : 5", "3 : 4", "2 : 3", "7 : 4" };

            int sliderValue = (int)e.NewValue;

            if (sliderValue >= 0 && sliderValue < aspectRatios.Length)
            {
                AspectRatioTextBlock.Text = aspectRatios[sliderValue];
            }
        }

        private void ImageCountSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            int imageCount = (int)e.NewValue;
            ImageCountTextBlock.Text = imageCount.ToString();
        }
    }
}
