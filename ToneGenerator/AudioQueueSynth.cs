using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using MonoTouch.AudioToolbox;
namespace com.ReinforceLab.iPhone.Controls.ToneGenerator
{
	public class AudioQueueSynth
	{
        OutputAudioQueue _audioQueue;
        readonly int _samplingRate;
        readonly int _numPacketsToRead;
        bool _isPrepared;
        double _phase;
        short[] _tmpBuf;

        public AudioQueueSynth(int samplingRate, int bufferLength)
		{
            _samplingRate = samplingRate;
            _numPacketsToRead = bufferLength;
            _isPrepared = false;
		}

        void prepareAudioQueue()
        {            
            AudioStreamBasicDescription audioFormat = new AudioStreamBasicDescription()
            {
                SampleRate = _samplingRate,
                Format = AudioFormatType.LinearPCM,
                FormatFlags = AudioFormatFlags.LinearPCMIsSignedInteger | AudioFormatFlags.IsPacked,
                FramesPerPacket = 1,
                ChannelsPerFrame = 1, // monoral
                BitsPerChannel = 16, // 16-bit
                BytesPerPacket = 2,
                BytesPerFrame = 2,
                Reserved = 0
            };
            _audioQueue = new OutputAudioQueue( audioFormat );
            _audioQueue.OutputCompleted += new EventHandler<OutputCompletedEventArgs>(_audioQueue_OutputCompleted);
            
            _tmpBuf = new short[_numPacketsToRead];
            //_numPacketsToRead  = 256;
            int bufferByteSize = _numPacketsToRead * audioFormat.BytesPerPacket;
            IntPtr bufferPtr;
            for (int index = 0; index < 3; index++)
            {
                _audioQueue.AllocateBuffer(bufferByteSize, out bufferPtr);
                outputCallback(bufferPtr);
            }
            _isPrepared = true;            
        }

        void _audioQueue_OutputCompleted(object sender, OutputCompletedEventArgs e)
        {
            //Debug.WriteLine("callback");   
            outputCallback(e.IntPtrBuffer);
        }
        void outputCallback(IntPtr bufPtr)
        {            
            int numPackets = _numPacketsToRead;
            int numBytes   = _numPacketsToRead * 2;
            double freq = 440 * 2.0 * Math.PI / _samplingRate;
            for (int i = 0; i < _tmpBuf.Length; i++)
            {                    
                double wave = Math.Sin(_phase);                    
                short sample = (short)(wave * 32767); // 16-bit integer
                _tmpBuf[i] = sample;                
                _phase += freq;                
            }
            
            unsafe { 
                fixed(short * ptr = _tmpBuf)
                {
                    OutputAudioQueue.FillAudioData(bufPtr, 0, new IntPtr((void*) ptr), 0, numBytes);
                }            
            }            
            _audioQueue.EnqueueBuffer(bufPtr, numBytes, null);
            
            /* original
            var buffer = (AudioQueueBuffer)Marshal.PtrToStructure(bufPtr, typeof(AudioQueueBuffer));            
            int numPackets = _numPacketsToRead;
            int numBytes = _numPacketsToRead * 2;
            double freq = 440 * 2.0 * Math.PI / 44100.0;            
            unsafe { 
                short *output = (short * )buffer.AudioData.ToPointer();
                double phase = _phase;
                for (int i = 0; i < numPackets; i++)
                {
                    double wave = Math.Sin(phase);
                    short sample = (short)(wave * 32767); // 16-bit integer
                    *output++ = sample;
                    phase = phase + freq;
                }
                _phase = phase;
            }
            buffer.AudioDataByteSize = (uint)numBytes;
            _audioQueue.EnqueueBuffer(bufPtr, numBytes, null);
            */
        }

        public void play()
        {
            if (!_isPrepared)
                prepareAudioQueue();
            _audioQueue.Start();
            _isPrepared = true;
        }
        public void stop(bool shouldStopImmediate)
        {
            if (null == _audioQueue)
                return;

            _audioQueue.Stop(shouldStopImmediate);
            _audioQueue.Dispose();
            _audioQueue = null;
            _isPrepared = false;
        }
	}
}

