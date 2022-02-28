using Asignment_UWP.Entity;
using Asignment_UWP.Service;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Asignment_UWP.Pages
{
    public sealed partial class RegisterPage : Windows.UI.Xaml.Controls.Page
    {
        private int checkGender;
        private string dateChanged;
        private int check = 0;
        private static string publicIDCloudinary;
        private CloudinaryDotNet.Account accountCloudinary;
        private Cloudinary cloudinary;

        public RegisterPage()
        {
            this.Loaded += RegisterPage_Loaded;
            this.InitializeComponent();
        }

        private void RegisterPage_Loaded(object sender, RoutedEventArgs e)
        {
            accountCloudinary = new CloudinaryDotNet.Account(
            "dn3bmj5ex",
            "344297185835677",
            "SanBwHJT4cGsaTibpYRpt0GzzmE"
            );
            cloudinary = new Cloudinary(accountCloudinary);
            cloudinary.Api.Secure = true;
        }

        private void HandleCheck(object sender, RoutedEventArgs e)
        {
            var check = sender as RadioButton;
            switch (check.Content)
            {
                case "Male":
                    checkGender = 1;
                    break;
                case "Fermale":
                    checkGender = 2;
                    break;
                case "Other":
                    checkGender = 3;
                    break;
            }
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog();
            var service = new UserService();
            Validate(firstName.Text, lastName.Text, email.Text, address.Text, phone.Text, password.Password);
            if (check != 0)
            {
                return;
            }
            var user = new User()
            {
                firstName = firstName.Text,
                lastName = lastName.Text,
                email = email.Text,
                phone = phone.Text,
                password = password.Password,
                address = address.Text,
                gender = checkGender,
                avatar = avatar.Text,
                birthday = dateChanged,
            };
            var result = await service.Register(user);
            if (result)
            {
                dialog.Title = "Success";
                dialog.Content = "Register Success";
                dialog.CloseButtonText = "Close";
                await dialog.ShowAsync();
                this.Frame.Navigate(typeof(Pages.LoginPage));
            }
            else
            {
                dialog.Title = "Error";
                dialog.Content = "Register Failed";
                dialog.CloseButtonText = "Close";
                await dialog.ShowAsync();
            }
        }
        private void birthday_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            var date = sender;
            dateChanged = date.Date.Value.ToString("yyyy-MM-dd");
        }
        private void Validate(string Fname, string Lname, string Email, string Address, string Phone, string Password)
        {
            if (string.IsNullOrEmpty(Fname))
            {
                checkFirstName.Text = "FirstName is required";
                check++;
            }
            if (string.IsNullOrEmpty(Lname))
            {
                checkLastName.Text = "LastName is required";
                check++;
            }
            if (string.IsNullOrEmpty(Email))
            {
                checkEmail.Text = "Email is required";
                check++;
            }
            if (string.IsNullOrEmpty(Address))
            {
                checkAddress.Text = "Address is required";
                check++;
            }
            if (string.IsNullOrEmpty(Phone))
            {
                checkPhone.Text = "Phone is required";
                check++;
            }
            if (string.IsNullOrEmpty(Password))
            {
                checkPassword.Text = "Password is required";
                check++;
            }
        }
        private void Handle_Login(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Pages.LoginPage));
        }

        private async void Button_CreateThumbnail(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                BitmapImage bitmapImage = new BitmapImage();
                IRandomAccessStream fileStream = await file.OpenReadAsync();
                await bitmapImage.SetSourceAsync(fileStream);
                RawUploadParams imageUploadParams = new RawUploadParams()
                {
                    File = new FileDescription(file.Name, await file.OpenStreamForReadAsync())
                };
                RawUploadResult result = await cloudinary.UploadAsync(imageUploadParams);
                publicIDCloudinary = result.PublicId;
                avatar.Text = result.Url.ToString();
                createThumbnail.Visibility = Visibility.Collapsed;
                deleteThumbnail.Visibility = Visibility.Visible;
            }
            else
            {
                Debug.WriteLine("Create image by avatar failed!");
            }
        }
        private async void Button_DeleteThumbnail(object sender, RoutedEventArgs e)
        {
            List<string> listPublicIdCouldinary = new List<string>();
            listPublicIdCouldinary.Add(publicIDCloudinary);
            string[] arrayPublicIdCouldinary = listPublicIdCouldinary.ToArray();
            await cloudinary.DeleteResourcesAsync(arrayPublicIdCouldinary);
            deleteThumbnail.Visibility = Visibility.Collapsed;
            createThumbnail.Visibility = Visibility.Visible;
            avatar.Text = "";
        }
    }
}
