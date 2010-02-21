using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace com.ReinforceLab.iPhone.Controls.AlphaRexRemoteController
{
    public static class BTStackWrapper
    {
        public delegate void BtStackPacketCallback(byte packet_type, UInt16 channel, IntPtr packet_ptr, UInt16 size);

        #region btstack API
        [DllImport("BTstack", EntryPoint = "bt_open")]
        public static extern int bt_open();
        [DllImport("BTstack", EntryPoint = "bt_close")]
        public static extern int bt_close();
        
        [DllImport("BTstack", EntryPoint = "bt_register_packet_handler", CallingConvention=CallingConvention.Cdecl)]
        public static extern void bt_register_packet_handler(BtStackPacketCallback handler); //[MarshalAs(UnmanagedType.FunctionPtr)] 

        public enum RUN_LOOP_TYPE { RUN_LOOP_POSIX = 1, RUN_LOOP_COCOA = 2 };
        // init must be called before any other run_loop call
        [DllImport("BTstack", EntryPoint = "run_loop_init")]
        public static extern void run_loop_init(RUN_LOOP_TYPE type);
        [DllImport("BTstack", EntryPoint = "run_loop_init")]
        public static extern void run_loop_execute();
        #endregion

        #region btstack send_command API
        [DllImport("BTstack", EntryPoint = "bt_send_cmd", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bt_send_cmd(ref HCICommands.HCI_COMMAND cmd, byte arg1);
        [DllImport("BTstack", EntryPoint = "bt_send_cmd", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bt_send_cmd(ref HCICommands.HCI_COMMAND cmd, UInt32 arg1, byte arg2, byte arg3);
        [DllImport("BTstack", EntryPoint = "bt_send_cmd", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bt_send_cmd(ref HCICommands.HCI_COMMAND cmd, Byte[] arg1, byte arg2, byte arg3, UInt16 arg4);
        #endregion

        #region helper methods
        // helper for BT little endian format        
        public static UInt16 readBt16(byte[] buffer, int pos)
        {
            return (UInt16)((UInt16)buffer[pos] | (((UInt16)buffer[pos + 1]) << 8));
        }
        public static UInt32 readBt24(byte[] buffer, int pos)
        {
            return (UInt32)((UInt32)buffer[pos] | (((UInt32)buffer[pos + 1]) << 8) | (((UInt32)buffer[pos + 2]) << 16));
        }
        public static UInt32 readBt32(byte[] buffer, int pos)
        {
            return (UInt32)((UInt32)buffer[pos] | (((UInt32)buffer[pos + 1]) << 8) | (((UInt32)buffer[pos + 2]) << 16) | (((UInt32)buffer[pos + 3]) << 24));
        }
        // check if command complete event for given command
        public static bool IsCommandCompleteEvent(byte[] packet, HCICommands.HCI_COMMAND command)
        {
            return (packet[0] == HCICommands.HCI_EVENT_COMMAND_COMPLETE && readBt16(packet, 3) == command.OpCode);
        }
        #endregion       
    }
}
