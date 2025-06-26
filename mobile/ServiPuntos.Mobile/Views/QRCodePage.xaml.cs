using Microsoft.Maui.Controls;
using System;
using System.IO;
using QRCoder;

namespace ServiPuntos.Mobile.Views;

[QueryProperty(nameof(Code), "code")]
public partial class QRCodePage : ContentPage
{
    public QRCodePage()
    {
        InitializeComponent();
    }

    string code;
    public string Code
    {
        get => code;
        set
        {
            code = value;
            Console.WriteLine($"[QRCodePage] Setter Code='{code}'");
            if (!string.IsNullOrWhiteSpace(code))
                GenerateQrGraphic(code);
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
            Console.WriteLine("[QRCodePage] QR generado OK");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[QRCodePage] Error al generar QR: {ex}");
            _ = DisplayAlert("Error", "No se pudo generar el QR. Revisa el log.", "OK");
        }
    }
}
