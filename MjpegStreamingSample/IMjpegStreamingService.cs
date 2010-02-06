using System;
using System.Drawing;

using System.ServiceModel;
using System.ServiceModel.Web;

namespace com.ReinforceLab.MonoTouch.Controls.MjpegStreamingSample
{
    [ServiceContract]
    public interface IMjpegStreamingService
    {
        bool StreamingBufferStalled { get; }
        void AddImage(Byte[] img);

        [OperationContract, WebGet(UriTemplate = "/")]
        System.IO.Stream MjpegStream();

        [OperationContract, WebGet(UriTemplate = "/frame")]
        RectangleF GetFrame();

        [OperationContract, WebGet(UriTemplate = "/framerate")]
        int GetFrameRate();
        [OperationContract, WebInvoke(UriTemplate = "/framerate")]
        void SetFrameRate(int frameRate);

        [OperationContract, WebGet(UriTemplate = "/roi")]
        RectangleF GetRegionOfInterest();
        [OperationContract, WebInvoke(UriTemplate = "/roi")]
        void SetRegionOfInterest(RectangleF roi);
    }
}
