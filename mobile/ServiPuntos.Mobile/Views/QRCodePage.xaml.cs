using Microsoft.Maui.Controls;
using System;
using System.IO;
using QRCoder;

namespace ServiPuntos.Mobile.Views
{
    [QueryProperty(nameof(Code), "code")]
    public partial class QRCodePage : ContentPage
    {
        bool hasCode;
        string pageTitle;

        public bool HasCode
        {
            get => hasCode;
            set
            {
                hasCode = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsEmpty));
            }
        }

        public bool IsEmpty => !HasCode;

        public string PageTitle
        {
            get => pageTitle;
            set
            {
                pageTitle = value;
                OnPropertyChanged();
            }
        }

        public QRCodePage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        string code;
        public string Code
        {
            get => code;
            set
            {
                code = Uri.UnescapeDataString(value ?? string.Empty);
                if (!string.IsNullOrWhiteSpace(code))
                {
                    HasCode = true;
                    PageTitle = "Tu código QR";
                    GenerateQrGraphic(code);
                }
                else
                {
                    HasCode = false;
                    PageTitle = "QR no disponible";
                }
            }
        }

        void GenerateQrGraphic(string text)
        {
            try
            {
                using var gen = new QRCodeGenerator();
                var qrData = gen.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                var png = new PngByteQRCode(qrData);
                var bytes = png.GetGraphic(20);
                QrImage.Source = ImageSource.FromStream(() => new MemoryStream(bytes));
            }
            catch
            {
                _ = DisplayAlert("Error", "No se pudo generar el QR.", "OK");
            }
        }
    }
}
