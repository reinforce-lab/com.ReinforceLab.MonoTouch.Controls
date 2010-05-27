using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace com.ReinforceLab.iPhone.Controls.ToneGenerator
{
    public class ToneGeneratorViewController : UIViewController
    {
        #region Variables
        UIButton _playButton, _stopButton;
        UITextField _samplingFreqTextField, _bufferLengthTextField;
        UILabel _samplingFreqLabel, _bufferLengthLabel;

        AudioQueueSynth _synth;
        #endregion

        #region Constructor
        public ToneGeneratorViewController()
            : base()
        {
            initialize();
        }
        void initialize()
        {
        }
        #endregion

        #region Override methods
        public override void LoadView()
        {
            base.LoadView();

            View.BackgroundColor = UIColor.White;

            _playButton = UIButton.FromType(UIButtonType.RoundedRect);
            _playButton.Frame = new RectangleF(62, 146, 72, 37);
            _playButton.SetTitle("Play", UIControlState.Normal);
            _playButton.TouchUpInside += new EventHandler(_playButton_TouchUpInside);
            Add(_playButton);

            _stopButton = UIButton.FromType(UIButtonType.RoundedRect);
            _stopButton.Frame = new RectangleF(179, 146, 72, 37);
            _stopButton.SetTitle("Stop", UIControlState.Normal);
            _stopButton.TouchUpInside += new EventHandler(_stopButton_TouchUpInside);
            Add(_stopButton);

            _samplingFreqLabel = new UILabel(new RectangleF(42, 32, 128, 21)) { 
                Text = "Sampling freq.:"                
            };
            Add(_samplingFreqLabel);

            _samplingFreqTextField = new UITextField(new RectangleF(191, 32, 97, 31)) { 
                Text = "44100",
                KeyboardType = UIKeyboardType.NumberPad,
                BorderStyle = UITextBorderStyle.RoundedRect
            };
            Add(_samplingFreqTextField);

            _bufferLengthLabel = new UILabel(new RectangleF(42, 78, 128, 21)) { 
                Text = "Buffer Length:" 
            };
            Add(_bufferLengthLabel);

            _bufferLengthTextField = new UITextField(new RectangleF(191, 78, 97, 31)) { 
                Text = "1024" ,
                KeyboardType = UIKeyboardType.NumberPad,
                BorderStyle = UITextBorderStyle.RoundedRect
            };
            Add(_bufferLengthTextField);

            _playButton.Enabled = true;
            _stopButton.Enabled = false;
        }

        #endregion

        #region
        void _playButton_TouchUpInside(object sender, EventArgs e)
        {
            if (null != _synth)
                return;

            _synth = new AudioQueueSynth(int.Parse(_samplingFreqTextField.Text), int.Parse(_bufferLengthTextField.Text));
            _synth.play();

            _playButton.Enabled = false;
            _stopButton.Enabled = true;
        }

        void _stopButton_TouchUpInside(object sender, EventArgs e)
        {
            if (null == _synth)
                return;

            _synth.stop(true);
            _synth = null;

            _playButton.Enabled = true;
            _stopButton.Enabled = false;
        }
        #endregion
    }
}
