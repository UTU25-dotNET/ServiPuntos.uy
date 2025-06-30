using Microsoft.Maui.Controls;
using QRCoder;
using ServiPuntos.Mobile.Models;
using System.IO;

namespace ServiPuntos.Mobile.Views
{
    public partial class CanjeItemView : ContentView
    {
        public CanjeItemView()
        {
            InitializeComponent();
        }
        
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext is CanjeSummaryDto dto && !string.IsNullOrWhiteSpace(dto.CodigoQR))
            {
                GenerateQrGraphic(dto.CodigoQR);
            }
            else
            {
                QrImage.Source = null;
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
                QrImage.Source = null;
            }
        }
    }
}
