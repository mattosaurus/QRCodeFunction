using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.DrawingCore;
using QRCoder;
using System.Net.Http;

namespace QRCodeFunction
{
    public static class GenerateQRCode
    {
        [FunctionName("GenerateQRCode")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequest req, TraceWriter log, ExecutionContext context)
        {
            log.Info("GenerateQRCode recieved a request.");

            string data = req.Query["data"];

            if (!string.IsNullOrEmpty(data))
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);

                Bitmap qrCodeImage = qrCode.GetGraphic(20);

                log.Info("QR code image returned.");
                return new FileContentResult(ImageToByteArray(qrCodeImage), "image/jpeg");
            }
            else
            {
                log.Info("No data parameter provided.");
                return new BadRequestResult();
            }
        }

        private static byte[] ImageToByteArray(Image image)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(image, typeof(byte[]));
        }
    }
}
