using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;

namespace com.ReinforceLab.iPhone.Controls.AlphaRexRemoteController
{
    public static class HCICommands
    {
        #region hci_cmds.h
        // packet types - used in BTstack and over the H4 UART interface
        public const byte HCI_COMMAND_DATA_PACKET = 0x01;
        public const byte HCI_ACL_DATA_PACKET = 0x02;
        public const byte HCI_SCO_DATA_PACKET = 0x03;
        public const byte HCI_EVENT_PACKET = 0x04;
        // extension for client/server communication
        public const byte DAEMON_EVENT_PACKET = 0x05;
        // L2CAP data
        public const byte L2CAP_DATA_PACKET = 0x06;
        // RFCOMM data
        public const byte RFCOMM_DATA_PACKET = 0x07;
        // Events from host controller to host
        public const byte HCI_EVENT_INQUIRY_COMPLETE = 0x01;
        public const byte HCI_EVENT_INQUIRY_RESULT = 0x02;
        public const byte HCI_EVENT_CONNECTION_COMPLETE = 0x03;
        public const byte HCI_EVENT_CONNECTION_REQUEST = 0x04;
        public const byte HCI_EVENT_DISCONNECTION_COMPLETE = 0x05;
        public const byte HCI_EVENT_AUTHENTICATION_COMPLETE_EVENT = 0x06;
        public const byte HCI_EVENT_REMOTE_NAME_REQUEST_COMPLETE = 0x07;
        public const byte HCI_EVENT_ENCRIPTION_CHANGE = 0x08;
        public const byte HCI_EVENT_CHANGE_CONNECTION_LINK_KEY_COMPLETE = 0x09;
        public const byte HCI_EVENT_MASTER_LINK_KEY_COMPLETE = 0x0A;
        public const byte HCI_EVENT_READ_REMOTE_SUPPORTED_FEATURES_COMPLETE = 0x0B;
        public const byte HCI_EVENT_READ_REMOTE_VERSION_INFORMATION_COMPLETE = 0x0C;
        public const byte HCI_EVENT_QOS_SETUP_COMPLETE = 0x0D;
        public const byte HCI_EVENT_COMMAND_COMPLETE = 0x0E;
        public const byte HCI_EVENT_COMMAND_STATUS = 0x0F;
        public const byte HCI_EVENT_HARDWARE_ERROR = 0x10;
        public const byte HCI_EVENT_FLUSH_OCCURED = 0x11;
        public const byte HCI_EVENT_ROLE_CHANGE = 0x12;
        public const byte HCI_EVENT_NUMBER_OF_COMPLETED_PACKETS = 0x13;
        public const byte HCI_EVENT_MODE_CHANGE_EVENT = 0x14;
        public const byte HCI_EVENT_RETURN_LINK_KEYS = 0x15;
        public const byte HCI_EVENT_PIN_CODE_REQUEST = 0x16;
        public const byte HCI_EVENT_LINK_KEY_REQUEST = 0x17;
        public const byte HCI_EVENT_LINK_KEY_NOTIFICATION = 0x18;
        public const byte HCI_EVENT_DATA_BUFFER_OVERFLOW = 0x1A;
        public const byte HCI_EVENT_MAX_SLOTS_CHANGED = 0x1B;
        public const byte HCI_EVENT_READ_CLOCK_OFFSET_COMPLETE = 0x1C;
        public const byte HCI_EVENT_PACKET_TYPE_CHANGED = 0x1D;
        public const byte HCI_EVENT_INQUIRY_RESULT_WITH_RSSI = 0x22;
        public const byte HCI_EVENT_EXTENDED_INQUIRY_RESPONSE = 0x2F;
        public const byte HCI_EVENT_VENDOR_SPECIFIC = 0xFF;
        // last used HCI_EVENT in 2.1 is 0x3d
        // events 0x50-0x5f are used internally
        // events from BTstack for application/client lib
        public const byte BTSTACK_EVENT_STATE = 0x60;
        // data: event(8), len(8), nr hci connections
        public const byte BTSTACK_EVENT_NR_CONNECTIONS_CHANGED = 0x61;
        // data: none
        public const byte BTSTACK_EVENT_POWERON_FAILED = 0x62;
        // data: majot (8), minor (8), revision(16)
        public const byte BTSTACK_EVENT_VERSION = 0x63;
        // data: system bluetooth on/off (bool)
        public const byte BTSTACK_EVENT_SYSTEM_BLUETOOTH_ENABLED = 0x64;
        // data: event (8), len(8), status (8), address(48), handle (16), psm (16), source_cid(16), dest_cid (16) 
        public const byte L2CAP_EVENT_CHANNEL_OPENED = 0x70;
        // data: event (8), len(8), channel (16)
        public const byte L2CAP_EVENT_CHANNEL_CLOSED = 0x71;
        // data: event(8), len(8), address(48), handle (16),  psm (16), source_cid(16), dest cid(16)
        public const byte L2CAP_EVENT_INCOMING_CONNECTION = 0x72;
        // data: event(8), len(8), handle(16)
        public const byte L2CAP_EVENT_TIMEOUT_CHECK = 0x73;
        // last HCI error is 0x3d
        // l2cap errors - enumeration by the command that created them
        public const byte L2CAP_COMMAND_REJECT_REASON_COMMAND_NOT_UNDERSTOOD = 0x60;
        public const byte L2CAP_COMMAND_REJECT_REASON_SIGNALING_MTU_EXCEEDED = 0x61;
        public const byte L2CAP_COMMAND_REJECT_REASON_INVALID_CID_IN_REQUEST = 0x62;
        public const byte L2CAP_CONNECTION_RESPONSE_RESULT_SUCCESSFUL = 0x63;
        public const byte L2CAP_CONNECTION_RESPONSE_RESULT_PENDING = 0x64;
        public const byte L2CAP_CONNECTION_RESPONSE_RESULT_REFUSED_PSM = 0x65;
        public const byte L2CAP_CONNECTION_RESPONSE_RESULT_REFUSED_SECURITY = 0x66;
        public const byte L2CAP_CONNECTION_RESPONSE_RESULT_REFUSED_RESOURCES = 0x65;
        public const byte L2CAP_CONFIG_RESPONSE_RESULT_SUCCESSFUL = 0x66;
        public const byte L2CAP_CONFIG_RESPONSE_RESULT_UNACCEPTABLE_PARAMS = 0x67;
        public const byte L2CAP_CONFIG_RESPONSE_RESULT_REJECTED = 0x68;
        public const byte L2CAP_CONFIG_RESPONSE_RESULT_UNKNOWN_OPTIONS = 0x69;

        public enum HCI_POWER_MODE
        {
            HCI_POWER_OFF = 0,
            HCI_POWER_ON = 1
        } ;

        public enum HCI_STATE
        {
            HCI_STATE_OFF = 0,
            HCI_STATE_INITIALIZING = 1,
            HCI_STATE_WORKING = 2,
            HCI_STATE_HALTING = 3
        };

        //compact HCI Command packet description
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct HCI_COMMAND
        {
            public readonly UInt16 OpCode;
            public readonly String Format;

            public HCI_COMMAND(UInt16 opcode, String format)
            {
                OpCode = opcode;
                Format = format;
            }
        }

        //Default INQ Mode
        public const long HCI_INQUIRY_LAP = 0x9E8B33L;  // 0x9E8B33: General/Unlimited Inquiry Access Code (GIAC)

        // Hardware state of Bluetooth controller 
        // HCI Commands - see hci_cmds.c for info on parameters
        /*
        extern hci_cmd_t btstack_get_state;
         */
        public static HCI_COMMAND btstack_set_power_mode;
        /*
        extern hci_cmd_t btstack_set_acl_capture_mode;
        extern hci_cmd_t btstack_get_version;
        extern hci_cmd_t btstack_get_system_bluetooth_enabled;
        extern hci_cmd_t btstack_set_system_bluetooth_enabled;
         * */
        public static HCI_COMMAND hci_inquiry;
        public static HCI_COMMAND hci_inquiry_cancel;
        /*
        extern hci_cmd_t hci_accept_connection_request;
        extern hci_cmd_t hci_authentication_requested;
        extern hci_cmd_t hci_create_connection;
        extern hci_cmd_t hci_create_connection_cancel;
        extern hci_cmd_t hci_delete_stored_link_key;
        extern hci_cmd_t hci_disconnect;
        extern hci_cmd_t hci_host_buffer_size;
        extern hci_cmd_t hci_link_key_request_negative_reply;
        extern hci_cmd_t hci_link_key_request_reply;
        extern hci_cmd_t hci_pin_code_request_reply;
        extern hci_cmd_t hci_qos_setup;
        extern hci_cmd_t hci_read_bd_addr;
        extern hci_cmd_t hci_read_link_policy_settings;
        extern hci_cmd_t hci_read_link_supervision_timeout;
         * */
        public static HCI_COMMAND hci_remote_name_request;
        /*
        extern hci_cmd_t hci_remote_name_request_cancel;
        extern hci_cmd_t hci_reset;
        extern hci_cmd_t hci_set_event_mask;
        extern hci_cmd_t hci_write_authentication_enable;
        extern hci_cmd_t hci_write_class_of_device;
        extern hci_cmd_t hci_write_extended_inquiry_response;
         * */
        public static HCI_COMMAND hci_write_inquiry_mode;
        /*
        extern hci_cmd_t hci_write_link_policy_settings;
        extern hci_cmd_t hci_write_link_supervision_timeout;
        extern hci_cmd_t hci_write_local_name;
        extern hci_cmd_t hci_write_page_timeout;
        extern hci_cmd_t hci_write_scan_enable;
        extern hci_cmd_t hci_write_simple_pairing_mode;

        extern hci_cmd_t l2cap_accept_connection;
        extern hci_cmd_t l2cap_create_channel;
        extern hci_cmd_t l2cap_decline_connection;
        extern hci_cmd_t l2cap_disconnect;
        extern hci_cmd_t l2cap_register_service;
        extern hci_cmd_t l2cap_unregister_service;
         */
        #endregion

        #region hci.h
        // packet header lenghts
        public const byte HCI_CMD_DATA_PKT_HDR = 0x03;
        public const byte HCI_ACL_DATA_PKT_HDR = 0x04;
        public const byte HCI_SCO_DATA_PKT_HDR = 0x03;
        public const byte HCI_EVENT_PKT_HDR = 0x02;

        // OGFs
        public const byte OGF_LINK_CONTROL = 0x01;
        public const byte OGF_LINK_POLICY = 0x02;
        public const byte OGF_CONTROLLER_BASEBAND = 0x03;
        public const byte OGF_INFORMATIONAL_PARAMETERS = 0x04;
        public const byte OGF_BTSTACK = 0x3d;
        public const byte OGF_VENDOR = 0x3f;

        // cmds for BTstack 
        // get state: @returns HCI_STATE
        public const byte BTSTACK_GET_STATE = 0x01;
        // set power mode: @param HCI_POWER_MODE
        public const byte BTSTACK_SET_POWER_MODE = 0x02;
        // set capture mode: @param on
        public const byte BTSTACK_SET_ACL_CAPTURE_MODE = 0x03;
        // get BTstack version
        public const byte BTSTACK_GET_VERSION = 0x04;
        // get system Bluetooth state
        public const byte BTSTACK_GET_SYSTEM_BLUETOOTH_ENABLED = 0x05;
        // set system Bluetooth state
        public const byte BTSTACK_SET_SYSTEM_BLUETOOTH_ENABLED = 0x06;
        // create l2cap channel: @param bd_addr(48), psm (16)
        public const byte L2CAP_CREATE_CHANNEL = 0x20;
        // disconnect l2cap disconnect, @param channel(16), reason(8)
        public const byte L2CAP_DISCONNECT = 0x21;
        // register l2cap service: @param psm(16), mtu (16)
        public const byte L2CAP_REGISTER_SERVICE = 0x22;
        // unregister l2cap disconnect, @param psm(16)
        public const byte L2CAP_UNREGISTER_SERVICE = 0x23;
        // accept connection @param bd_addr(48), dest cid (16)
        public const byte L2CAP_ACCEPT_CONNECTION = 0x24;
        // decline l2cap disconnect,@param bd_addr(48), dest cid (16), reason(8)
        public const byte L2CAP_DECLINE_CONNECTION = 0x25;
        /*
// 
public const byte  IS_COMMAND(packet, command) (READ_BT_16(packet,0) == command.opcode)
*/
        // data: event(8)
        public const byte DAEMON_EVENT_CONNECTION_CLOSED = 0x70;
        // data: event(8), nr_connections(8)
        public const byte DAEMON_NR_CONNECTIONS_CHANGED = 0x71;

        // Connection State 
        [Flags]
        public enum hci_connection_flags 
        {
            SEND_NEGATIVE_LINK_KEY_REQUEST = 1 << 0,
            SEND_PIN_CODE_RESPONSE = 1 << 1
        };
        public enum  CONNECTION_STATE
        {    
            SENT_CREATE_CONNECTION = 1,
            RECEIVED_CONNECTION_REQUEST = 2,    
            ACCEPTED_CONNECTION_REQUEST = 3,    
            REJECTED_CONNECTION_REQUEST = 4,
            OPEN = 5,    
            SENT_DISCONNECT = 6
        };
        public enum BLUETOOTH_STATE
        {
            BLUETOOTH_OFF = 1,
            BLUETOOTH_ON = 2,
            BLUETOOTH_ACTIVE = 3
        };
/*
typedef struct {
    // linked list - assert: first field
    linked_item_t    item;
    
    // remote side
    bd_addr_t address;
    
    // module handle
    hci_con_handle_t con_handle;

    // state
    CONNECTION_STATE state;
    
    // errands
    hci_connection_flags_t flags;
    
    // timer
    timer_source_t timeout;
    struct timeval timestamp;

} hci_connection_t;

/**
 * main data structure
 *
typedef struct {
    // transport component with configuration
    hci_transport_t  * hci_transport;
    void             * config;
    
    // hardware power controller
    bt_control_t     * control;
    
    // list of existing baseband connections
    linked_list_t     connections;

    // single buffer for HCI Command assembly
    uint8_t          * hci_cmd_buffer;
    
    /* host to controller flow control *
    uint8_t  num_cmd_packets;
    uint8_t  num_acl_packets;

    /* callback to L2CAP layer *
    void (*event_packet_handler)(uint8_t *packet, uint16_t size);
    void (*acl_packet_handler)  (uint8_t *packet, uint16_t size);

    /* hci state machine *
    HCI_STATE state;
    uint8_t   substate;
    uint8_t   cmds_ready;
    
} hci_stack_t;

// create and send hci command packets based on a template and a list of parameters
uint16_t hci_create_cmd(uint8_t *hci_cmd_buffer, hci_cmd_t *cmd, ...);
uint16_t hci_create_cmd_internal(uint8_t *hci_cmd_buffer, hci_cmd_t *cmd, va_list argptr);

// set up HCI
void hci_init(hci_transport_t *transport, void *config, bt_control_t *control);

void hci_register_event_packet_handler(void (*handler)(uint8_t *packet, uint16_t size));

void hci_register_acl_packet_handler  (void (*handler)(uint8_t *packet, uint16_t size));
    
// power control
int hci_power_control(HCI_POWER_MODE mode);

/**
 * run the hci control loop once
 *
void hci_run();

// create and send hci command packets based on a template and a list of parameters
int hci_send_cmd(hci_cmd_t *cmd, ...);

// send complete CMD packet
int hci_send_cmd_packet(uint8_t *packet, int size);

// send ACL packet
int hci_send_acl_packet(uint8_t *packet, int size);

hci_connection_t * connection_for_handle(hci_con_handle_t con_handle);

// 
void hci_emit_state();
void hci_emit_connection_complete(hci_connection_t *conn);
void hci_emit_l2cap_check_timeout(hci_connection_t *conn);
void hci_emit_nr_connections_changed();
void hci_emit_hci_open_failed();
void hci_emit_btstack_version();
void hci_emit_system_bluetooth_enabled(uint8_t enabled);
 */
        #endregion

        #region Private methods
        static UInt16 Opcode(byte ogf, UInt16 ocf)
        {
            return (UInt16)(ocf | ogf << 10);
        }
        #endregion

        #region Constructor
        static HCICommands()
        {
            hci_inquiry = new HCI_COMMAND(Opcode(OGF_LINK_CONTROL, 0x01), "311"); // LAP, Inquiry length, Num_responses           
            hci_inquiry_cancel = new HCI_COMMAND( Opcode(OGF_LINK_CONTROL, 0x02), ""); // no params

            /*             
hci_cmd_t hci_create_connection = {
OPCODE(OGF_LINK_CONTROL, 0x05), "B21121"
// BD_ADDR, Packet_Type, Page_Scan_Repetition_Mode, Reserved, Clock_Offset, Allow_Role_Switch
};

hci_cmd_t hci_disconnect = {
OPCODE(OGF_LINK_CONTROL, 0x06), "H1"
// Handle, Reason: 0x05, 0x13-0x15, 0x1a, 0x29
// see Errors Codes in BT Spec Part D
};
hci_cmd_t hci_create_connection_cancel = {
OPCODE(OGF_LINK_CONTROL, 0x08), "B"
// BD_ADDR
};
hci_cmd_t hci_accept_connection_request = {
OPCODE(OGF_LINK_CONTROL, 0x09), "B1"
// BD_ADDR, Role: become master, stay slave
};
hci_cmd_t hci_link_key_request_reply = {
OPCODE(OGF_LINK_CONTROL, 0x0b), "BP"
// BD_ADDR, LINK_KEY
};
hci_cmd_t hci_link_key_request_negative_reply = {
OPCODE(OGF_LINK_CONTROL, 0x0c), "B"
// BD_ADDR
};
hci_cmd_t hci_pin_code_request_reply = {
OPCODE(OGF_LINK_CONTROL, 0x0d), "B1P"
// BD_ADDR, pin length, PIN: c-string
};
hci_cmd_t hci_authentication_requested = {
OPCODE(OGF_LINK_CONTROL, 0x11), "H"
// Handle
};
             
             * */
            
            hci_remote_name_request = new HCI_COMMAND(Opcode(OGF_LINK_CONTROL, 0x19), "B112");  // BD_ADDR, Page_Scan_Repetition_Mode, Reserved, Clock_Offset
            /*
hci_cmd_t hci_remote_name_request_cancel = {
OPCODE(OGF_LINK_CONTROL, 0x1A), "B"
// BD_ADDR
};


 //*  Link Policy Commands  
hci_cmd_t hci_qos_setup = {
    OPCODE(OGF_LINK_POLICY, 0x07), "H114444"
    // handle, flags, service_type, token rate (bytes/s), peak bandwith (bytes/s),
    // latency (us), delay_variation (us)
};
hci_cmd_t hci_read_link_policy_settings = {
    OPCODE(OGF_LINK_POLICY, 0x0c), "H"
    // handle 
};
hci_cmd_t hci_write_link_policy_settings = {
    OPCODE(OGF_LINK_POLICY, 0x0d), "H2"
    // handlee, settings
};

/**
 *  Controller & Baseband Commands 
 *
hci_cmd_t hci_set_event_mask = {
OPCODE(OGF_CONTROLLER_BASEBAND, 0x01), "44"
// event_mask lower 4 octets, higher 4 bytes
};
hci_cmd_t hci_reset = {
OPCODE(OGF_CONTROLLER_BASEBAND, 0x03), ""
// no params
};
hci_cmd_t hci_delete_stored_link_key = {
OPCODE(OGF_CONTROLLER_BASEBAND, 0x12), "B1"
// BD_ADDR, Delete_All_Flag
};
hci_cmd_t hci_write_local_name = {
OPCODE(OGF_CONTROLLER_BASEBAND, 0x13), "N"
// Local name (UTF-8, Null Terminated, max 248 octets)
};
hci_cmd_t hci_write_page_timeout = {
OPCODE(OGF_CONTROLLER_BASEBAND, 0x18), "2"
// Page_Timeout * 0.625 ms
};
hci_cmd_t hci_write_scan_enable = {
OPCODE(OGF_CONTROLLER_BASEBAND, 0x1A), "1"
// Scan_enable: no, inq, page, inq+page
};
hci_cmd_t hci_write_authentication_enable = {
OPCODE(OGF_CONTROLLER_BASEBAND, 0x20), "1"
// Authentication_Enable
};
hci_cmd_t hci_write_class_of_device = {
OPCODE(OGF_CONTROLLER_BASEBAND, 0x24), "3"
// Class of Device
};
hci_cmd_t hci_host_buffer_size = {
OPCODE(OGF_CONTROLLER_BASEBAND, 0x33), "2122"
// Host_ACL_Data_Packet_Length:, Host_Synchronous_Data_Packet_Length:, Host_Total_Num_ACL_Data_Packets:, Host_Total_Num_Synchronous_Data_Packets:
};
hci_cmd_t hci_read_link_supervision_timeout = {
OPCODE(OGF_CONTROLLER_BASEBAND, 0x36), "H"
// handle
};
hci_cmd_t hci_write_link_supervision_timeout = {
OPCODE(OGF_CONTROLLER_BASEBAND, 0x37), "H2"
// handle, Range for N: 0x0001 ? 0xFFFF Time (Range: 0.625ms ? 40.9 sec)
};
             * */
            hci_write_inquiry_mode = new HCI_COMMAND(Opcode(OGF_CONTROLLER_BASEBAND, 0x45), "1"); // Inquiry mode: 0x00 = standard, 0x01 = with RSSI, 0x02 = extended
            /*
hci_cmd_t hci_write_extended_inquiry_response = {
OPCODE(OGF_CONTROLLER_BASEBAND, 0x52), "1E"
// FEC_Required, Exstended Inquiry Response
};

hci_cmd_t hci_write_simple_pairing_mode = {
OPCODE(OGF_CONTROLLER_BASEBAND, 0x56), "1"
// mode: 0 = off, 1 = on
};

hci_cmd_t hci_read_bd_addr = {
OPCODE(OGF_INFORMATIONAL_PARAMETERS, 0x09), ""
// no params
};

// BTstack commands

hci_cmd_t btstack_get_state = {
OPCODE(OGF_BTSTACK, BTSTACK_GET_STATE), ""
// no params -> 
};
*/
        btstack_set_power_mode = new HCI_COMMAND(Opcode(OGF_BTSTACK, BTSTACK_SET_POWER_MODE), "1"); // mode: 0 = off, 1 = on
/*
hci_cmd_t btstack_set_acl_capture_mode = {
OPCODE(OGF_BTSTACK, BTSTACK_SET_ACL_CAPTURE_MODE), "1"
// mode: 0 = off, 1 = on
};

hci_cmd_t btstack_get_version = {
OPCODE(OGF_BTSTACK, BTSTACK_GET_VERSION), ""
};

hci_cmd_t btstack_get_system_bluetooth_enabled = {
OPCODE(OGF_BTSTACK, BTSTACK_GET_SYSTEM_BLUETOOTH_ENABLED), ""
};

hci_cmd_t btstack_set_system_bluetooth_enabled = {
OPCODE(OGF_BTSTACK, BTSTACK_SET_SYSTEM_BLUETOOTH_ENABLED), "1"
};

hci_cmd_t l2cap_create_channel = {
OPCODE(OGF_BTSTACK, L2CAP_CREATE_CHANNEL), "B2"
// @param bd_addr(48), psm (16)
};

hci_cmd_t l2cap_disconnect = {
OPCODE(OGF_BTSTACK, L2CAP_DISCONNECT), "21"
// @param channel(16), reason(8)
};
hci_cmd_t l2cap_register_service = {
OPCODE(OGF_BTSTACK, L2CAP_REGISTER_SERVICE), "22"
// @param psm (16), mtu (16)
};
hci_cmd_t l2cap_unregister_service = {
OPCODE(OGF_BTSTACK, L2CAP_UNREGISTER_SERVICE), "2"
// @param psm (16)
};
hci_cmd_t l2cap_accept_connection = {
OPCODE(OGF_BTSTACK, L2CAP_ACCEPT_CONNECTION), "2"
// @param source cid (16)
};
hci_cmd_t l2cap_decline_connection = {
OPCODE(OGF_BTSTACK, L2CAP_DECLINE_CONNECTION), "21"
// @param source cid (16), reason(8)
};
             */
        }
        #endregion

    }
}
