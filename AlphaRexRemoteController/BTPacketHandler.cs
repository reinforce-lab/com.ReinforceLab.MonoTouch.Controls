using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace com.ReinforceLab.iPhone.Controls.AlphaRexRemoteController
{
    internal static class BTPacketHandler
    {
        #region Variables
        const int MAX_DEVICES = 10;
        const int INQUIRY_INTERVAL = 5;

        static bool is_opened = false;
        static int state = 0;
        static List<BTDeviceInfo> device_list = new List<BTDeviceInfo>();

        public static event EventHandler DeviceListChangedEvent;
        #endregion

        #region Properties
        public static BTDeviceInfo[] DeviceList { get { return device_list.ToArray(); } }
        #endregion

        #region Privae methods
        static void invokeDeviceListChangedEvent()
        {
            if (null != DeviceListChangedEvent)
                DeviceListChangedEvent.Invoke(null, null);
        }

        static BTDeviceInfo getDeviceInfoForAddress(Byte[] addr)
        {
            foreach (var item in device_list)
            {
                if (Enumerable.SequenceEqual<byte>(item.Address, addr))
                    return item;
            }
            return null;
        }
        
        static void next()
        {
            bool found = false;
            switch (state)
            {
                case 0:
                    Debug.WriteLine("Starting inquiry scan..");
                    BTStackWrapper.bt_send_cmd(ref HCICommands.hci_inquiry, (UInt32)HCICommands.HCI_INQUIRY_LAP, (byte)INQUIRY_INTERVAL, (byte)0);
                    state = 1;
                    break;
                case 1:
                    found = false;
                    foreach (var device in device_list)
                    {
                        // remote name request
                        if (device.State == BTDeviceInfo.Status.Found)
                        {
                            found = true;
                            device.State = BTDeviceInfo.Status.RemoteNameFound;
                            Debug.WriteLine(String.Format("Get remote name of {0} ...", device.GetBtAddress()));
                            BTStackWrapper.bt_send_cmd(ref HCICommands.hci_remote_name_request, (byte[])device.Address, (byte)device.PageScanRepetitionMode, (byte)0, (UInt16)(device.ClockOffset | 0x8000));
                        }
                        if (!found)
                        {
                            Debug.WriteLine("Queried all devices, restart.");
                            state = 0;
                            next();
                        }
                    }
                    break;
                default: state = 0; break;
            }
        }
 
        [MonoTouch.MonoPInvokeCallback(typeof(BTStackWrapper.BtStackPacketCallback))]
        static void packet_handler(byte packet_type,  UInt16 channel, IntPtr packet_ptr, UInt16 size) 
        {
            Byte[] packet = new Byte[size];
            Marshal.Copy(packet_ptr, packet, (int)0, (int)size);

            int numResponses = 0;
            Byte[] addr;

            BTDeviceInfo info;
            switch (packet_type)
            {
                case HCICommands.HCI_EVENT_PACKET:
                    switch (packet[0])
                    {
                        case HCICommands.BTSTACK_EVENT_STATE:
                            // bt stack activated, get started - set local name                                            
                            if (packet[2] == (byte)HCICommands.HCI_STATE.HCI_STATE_WORKING)
                            {
                                BTStackWrapper.bt_send_cmd(ref HCICommands.hci_write_inquiry_mode, (byte)0x01); // with RSSI
                                next();
                            }
                            break;
                        case HCICommands.HCI_EVENT_COMMAND_COMPLETE:
                            if (BTStackWrapper.IsCommandCompleteEvent(packet, HCICommands.hci_write_inquiry_mode))
                            {
                                next();
                            }
                            break;
                        case HCICommands.HCI_EVENT_INQUIRY_RESULT:
                            numResponses = packet[2];
                            for (int i = 0; i < numResponses && device_list.Count < MAX_DEVICES; i++)
                            {
                                // get address
                                addr = new byte[6];
                                Array.Copy(packet, 3 + i * 6, addr, 0, addr.Length);
                                Array.Reverse(addr);

                                info = getDeviceInfoForAddress(addr);
                                if (null != info) continue;

                                info = new BTDeviceInfo(addr)
                                {                                    
                                    PageScanRepetitionMode = packet[3 + 6 * numResponses + i * 1],
                                    ClassOfDevice = BTStackWrapper.readBt24(packet, 3 + (6 + 1 + 1 + 1) * numResponses + i * 3),
                                    ClockOffset = (UInt16)(BTStackWrapper.readBt16(packet, 3 + (6 + 1 + 1 + 1 + 3) * numResponses + i * 2) & 0x7fff),
                                    Rssi = 0,
                                    State = BTDeviceInfo.Status.Found
                                };
                                device_list.Add(info);
                                invokeDeviceListChangedEvent();
                                Debug.WriteLine(String.Format("Device found: {0} with COD: 0x{1:6X}, pageScan:{2} clock offset {3}.", info.GetBtAddress(), info.ClassOfDevice, info.PageScanRepetitionMode, info.ClockOffset));
                            }
                            break;
                        case HCICommands.HCI_EVENT_INQUIRY_RESULT_WITH_RSSI:
                            numResponses = packet[2];
                            for (int i = 0; i < numResponses && device_list.Count < MAX_DEVICES; i++)
                            {
                                // get address
                                addr = new byte[6];
                                Array.Copy(packet, 3 + i * 6, addr, 0, addr.Length);
                                Array.Reverse(addr);

                                info = getDeviceInfoForAddress(addr);
                                if (null != info) continue;

                                info = new BTDeviceInfo(addr)
                                {                                
                                    PageScanRepetitionMode = packet[3 + 6 * numResponses + i * 1],
                                    ClassOfDevice = BTStackWrapper.readBt24(packet, 3 + (6 + 1 + 1 + 1) * numResponses + i * 3),
                                    ClockOffset = (UInt16)(BTStackWrapper.readBt16(packet, 3 + (6 + 1 + 1 + 1 + 3) * numResponses + i * 2) & 0x7fff),
                                    Rssi = packet[3 + numResponses * (6 + 1 + 1 + 3 + 2) + i * 1],
                                    State = BTDeviceInfo.Status.Found
                                };
                                device_list.Add(info);
                                invokeDeviceListChangedEvent();
                                Debug.WriteLine(String.Format("Device found: {0} with COD: 0x{1:6X}, pageScan:{2} clock offset {3}.", info.GetBtAddress(), info.ClassOfDevice, info.PageScanRepetitionMode, info.ClockOffset));
                            }
                            break;
                        case HCICommands.HCI_EVENT_REMOTE_NAME_REQUEST_COMPLETE:
                            // get address
                            addr = new byte[6];
                            Array.Copy(packet, 3, addr, 0, addr.Length);
                            Array.Reverse(addr);
                            info = getDeviceInfoForAddress(addr);
                            if (null != info)
                            {
                                if (packet[2] == 0)
                                {
                                    var name = System.Text.Encoding.ASCII.GetString(packet, 9, packet.Length - 9);
                                    Debug.WriteLine(String.Format("Name: {0}", name));
                                    info.DeviceName = name;
                                    info.State = BTDeviceInfo.Status.RemoteNameFound;
                                }
                                else
                                {
                                    Debug.WriteLine("Failed to get name: page timeout.");
                                }
                            }
                            next();
                            break;

                        case HCICommands.HCI_EVENT_INQUIRY_COMPLETE:
                            Debug.WriteLine("Inquiry scan done.");
                            foreach (var device in device_list)
                            {
                                if (device.State == BTDeviceInfo.Status.RemoteNameTried)
                                    device.State = BTDeviceInfo.Status.Found;
                            }
                            next();
                            break;

                        default:
                            break;
                    }
                    break;

                default:
                    break;             
            }

        }
        #endregion

        #region Public methods
        public static bool StartInquiry()
        {
            if (is_opened) return true;

            BTStackWrapper.run_loop_init(BTStackWrapper.RUN_LOOP_TYPE.RUN_LOOP_COCOA);
            int err = BTStackWrapper.bt_open();
            if (err != 0)
            {
                return false;
            }
            
            is_opened = true;

            BTStackWrapper.bt_register_packet_handler(new BTStackWrapper.BtStackPacketCallback(BTPacketHandler.packet_handler));
            BTStackWrapper.bt_send_cmd(ref HCICommands.btstack_set_power_mode, (byte)HCICommands.HCI_POWER_MODE.HCI_POWER_ON);
            BTStackWrapper.bt_send_cmd(ref HCICommands.hci_inquiry, (UInt32)HCICommands.HCI_INQUIRY_LAP, (byte)INQUIRY_INTERVAL, (byte)0);
            
            return true;
        }
        public static void StopInquiry()
        {
            if (!is_opened) return;
            
            BTStackWrapper.bt_close();
            is_opened = false;
        }
        #endregion
    }
}
