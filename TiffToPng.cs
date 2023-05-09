using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

using ImageMagick;
using System.Web;

namespace tiff_convert_demo;

public class TiffToPng
{
    private readonly ILogger _logger;

    public TiffToPng(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<TiffToPng>();
    }

    [Function("TiffToPng")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        var parms = HttpUtility.ParseQueryString(req.Url.Query);

        response.Headers.Add("Contnet-Type", "image/png");
        
        _logger.LogInformation("Converting TIFF to PNG");
        _logger.LogInformation(parms["file"]);

        // using a sample file for demo purposes
        var testFile = File.ReadAllBytes("sample.tiff");
        var result = ConvertToPng(testFile);
        
        response.WriteBytes(result);

        return response;
    }

    private byte[] ConvertToPng(byte[] tiff)
    {
        using (MemoryStream memStream = new MemoryStream(tiff))
        {
            using (MagickImage image = new MagickImage(tiff))
            {
                image.Format = MagickFormat.Png;
                image.Write(memStream);
            }

            return memStream.ToArray();
        }
    }
}

