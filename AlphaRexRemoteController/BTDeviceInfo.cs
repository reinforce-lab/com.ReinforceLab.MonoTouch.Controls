using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.ReinforceLab.iPhone.Controls.AlphaRexRemoteController
{    
    internal class BTDeviceInfo : System.ComponentModel.INotifyPropertyChanged
    {
        public enum Status { Empty = 0, Found = 1, RemoteNameTried = 2, RemoteNameFound = 3 };

        #region Variables
        String device_name;
        Byte[] address;    // length 6
        UInt16 clockOffset;
        UInt32 classOfDevice;
        Byte   pageScanRepetitionMode;
        Byte   rssi;
        Status state;
        #endregion

        #region Properties        
        public const string PN_DeviceName = "DeviceName";
        public String DeviceName
        {
            get { return device_name; }
            set { device_name = value; invokePropertyChanged(PN_DeviceName); }
        }
        public const string PN_Address = "Address";
        public Byte[] Address
        {
            get { return address; }            
        }
        public const string PN_ClockOffset = "ClockOffset";
        public UInt16 ClockOffset        
        {
            get { return clockOffset; }
            set { clockOffset = value; invokePropertyChanged(PN_ClockOffset); }
        }
        public const string PN_ClassOfDevice = "ClassOfDevice";
        public UInt32 ClassOfDevice        
        {
            get { return classOfDevice; }
            set { classOfDevice = value; invokePropertyChanged(PN_ClassOfDevice); }
        }
        public const string PN_PageScanRepetitionMode = "PageScanRepetitionMode";
        public Byte PageScanRepetitionMode
        {
            get { return pageScanRepetitionMode; }
            set { pageScanRepetitionMode = value; invokePropertyChanged(PN_PageScanRepetitionMode); }
        }
        public const string PN_Rssi = "Rssi";
        public Byte Rssi
        {
            get { return rssi; }
            set { rssi = value; invokePropertyChanged(PN_Rssi); }
        }
        public const string PN_State = "State";
        public Status State
        {
            get { return state; }
            set { state = value; invokePropertyChanged(PN_State); }
        }
        #endregion

        #region Constructor
        public BTDeviceInfo(Byte[] bt_addr)
        {
            device_name = string.Empty;
            address = new byte[6];
            bt_addr.CopyTo(address, 0);

            clockOffset = 0;
            classOfDevice = 0;
            pageScanRepetitionMode = 0;
            rssi = 0;
            state = Status.Empty;            
        }
        #endregion

        #region Public methods
        public String GetBtAddress()
        {
            return System.BitConverter.ToString(Address);
            /*
            var seg = from s in Address select s.ToString("X2");
            return String.Join(":", seg.ToArray<String>());                    
             * */
        }
        #endregion

        #region INotifyPropertyChanged メンバ
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        void invokePropertyChanged(String name)
        {
            if (null != PropertyChanged)
                PropertyChanged.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
        }
        #endregion
    }    
}
