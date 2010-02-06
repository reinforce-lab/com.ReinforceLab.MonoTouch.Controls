using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Threading;

using System.ServiceModel;
using System.ServiceModel.Web;

namespace com.ReinforceLab.MonoTouch.Controls.MjpegStreamingSample
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class MjpegStreamingService : IMjpegStreamingService
    {
        #region Variables
        const string BOUNDARY = "appmjpegserver";

        readonly IStreamingController _controller;
        readonly videoStream _videoStream;
        #endregion

        #region Constructor
        public MjpegStreamingService(IStreamingController controller)
        {
            _controller = controller;
            _videoStream = new videoStream(this);
        }
        #endregion

        #region IMjpegStreamingService メンバ
        public bool StreamingBufferStalled
        {
            get { return !_videoStream.CanBuffering(); }
        }
        public void AddImage(Byte[] img)
        {
            _videoStream.Add(img);
        }
        public System.IO.Stream MjpegStream()
        {
            var response = WebOperationContext.Current.OutgoingResponse;
            response.StatusCode = System.Net.HttpStatusCode.OK;
            response.ContentType = String.Format("multipart/x-mixed-replace;boundary={0}", BOUNDARY);

            return _videoStream;
        }
        public System.Drawing.RectangleF GetFrame()
        {
            return _controller.RegionOfInterest;
        }
        public int GetFrameRate()
        {
            return _controller.FrameRate;
        }
        public void SetFrameRate(int frameRate)
        {
            _controller.FrameRate = frameRate;
        }
        public System.Drawing.RectangleF GetRegionOfInterest()
        {
            return _controller.RegionOfInterest;
        }
        public void SetRegionOfInterest(System.Drawing.RectangleF roi)
        {
            // do nothing
        }
        #endregion

        #region Streaming class
        internal class videoStream : System.IO.Stream
        {
            #region Variables
            readonly MjpegStreamingService _service;
            readonly List<Byte[]> _imgBuffer;
            readonly AutoResetEvent _autoResetEvent;

            Byte[] _buffer;
            int _bufPosition;
            long _acmPosition = 0;
            #endregion

            #region Constructor
            public videoStream(MjpegStreamingService service)
            {
                _service = service;
                _imgBuffer = new List<byte[]>();
                _autoResetEvent = new AutoResetEvent(false);

                _buffer = new Byte[] { };
                _bufPosition = 0;
                _acmPosition = 0;
            }
            #endregion

            #region Public methods
            public void Add(Byte[] jpgImage)
            {
                lock (_imgBuffer)
                {
                    _imgBuffer.Add(jpgImage);
                    // set flag
                    _autoResetEvent.Set();
                }
            }
            public bool CanBuffering()
            {
                lock (_imgBuffer)
                {
                    return _imgBuffer.Count < 2;
                }
            }
            #endregion

            #region Private methods
            void writeString(MemoryStream ms, String text)
            {
                var buf = ASCIIEncoding.ASCII.GetBytes(text);
                ms.Write(buf, 0, buf.Length);
            }
            void fillBuffer(Byte[] data)
            {
                using (var ms = new MemoryStream())
                {
                    // boundary
                    writeString(ms, String.Format("\n--{0}\n", MjpegStreamingService.BOUNDARY));
                    // content type
                    writeString(ms, "Content-type: image/jpeg\n\n");
                    // jpeg image
                    ms.Write(data, 0, data.Length);

                    _buffer = ms.ToArray();
                    _bufPosition = 0;
                }
            }
            #endregion

            public override bool CanRead { get { return true; } }
            public override bool CanSeek { get { return false; } }
            public override bool CanWrite { get { return false; } }
            public override void Flush() { }
            public override long Length
            {
                get { return long.MaxValue; }
            }
            public override long Position
            {
                get { return _acmPosition; }
                set { }
            }
            public override int Read(byte[] buffer, int offset, int count)
            {
                if (count <= 0) return 0;

                // check buffer length
                var length = Math.Min(_buffer.Length - _bufPosition, count);
                if (length <= 0)
                {
                    // waiting flag set
                    _autoResetEvent.WaitOne();
                    lock (_imgBuffer)
                    {
                        // buffer re-fill
                        fillBuffer(_imgBuffer[0]);
                        _imgBuffer.RemoveAt(0);
                        // clear flag
                        if (_imgBuffer.Count <= 0)
                            _autoResetEvent.Reset();
                    }
                }

                // seiding buffer data
                length = Math.Min(_buffer.Length - _bufPosition, count);
                Buffer.BlockCopy(_buffer, _bufPosition, buffer, offset, length);

                _bufPosition += length;
                _acmPosition += length;

                System.Diagnostics.Debug.WriteLine(String.Format("Read(count:{0} length:{1})", count, length));

                return length;
            }
            public override long Seek(long offset, SeekOrigin origin)
            {
                return _acmPosition;
            }
            public override void SetLength(long value)
            { }
            public override void Write(byte[] buffer, int offset, int count)
            { }
        }
        #endregion
    }
}
