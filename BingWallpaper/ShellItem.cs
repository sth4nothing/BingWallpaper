using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BingWallpaper
{
    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("b63ea76d-1f85-456f-a19c-48159efa858b")]
    public interface IShellItemArray
    {
        int BindToHandler(IntPtr pbc, ref Guid bhid, ref Guid riid, ref IntPtr ppvOut);
        int GetPropertyStore(GETPROPERTYSTOREFLAGS flags, ref Guid riid, ref IntPtr ppv);
        int GetPropertyDescriptionList(REFPROPERTYKEY keyType, ref Guid riid, ref IntPtr ppv);
        int GetAttributes(SIATTRIBFLAGS AttribFlags, int sfgaoMask, ref int psfgaoAttribs);
        int GetCount(ref int pdwNumItems);
        int GetItemAt(int dwIndex, ref IShellItem ppsi);
        int EnumItems(ref IntPtr ppenumShellItems);
    }
    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE")]
    public interface IShellItem
    {
        [PreserveSig()]
        int BindToHandler(IntPtr pbc, ref Guid bhid, ref Guid riid, ref IntPtr ppv);
        int GetParent(ref IShellItem ppsi);
        int GetDisplayName(SIGDN sigdnName, ref System.Text.StringBuilder ppszName);
        int GetAttributes(uint sfgaoMask, ref uint psfgaoAttribs);
        int Compare(IShellItem psi, uint hint, ref int piOrder);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct REFPROPERTYKEY
    {
        private Guid fmtid;
        private int pid;
        public Guid FormatId
        {
            get
            {
                return this.fmtid;
            }
        }
        public int PropertyId
        {
            get
            {
                return this.pid;
            }
        }
        public REFPROPERTYKEY(Guid formatId, int propertyId)
        {
            this.fmtid = formatId;
            this.pid = propertyId;
        }
        public static readonly REFPROPERTYKEY PKEY_DateCreated = new REFPROPERTYKEY(new Guid("B725F130-47EF-101A-A5F1-02608C9EEBAC"), 15);
    }
    public enum GETPROPERTYSTOREFLAGS
    {
        GPS_DEFAULT = 0,
        GPS_HANDLERPROPERTIESONLY = 0x1,
        GPS_READWRITE = 0x2,
        GPS_TEMPORARY = 0x4,
        GPS_FASTPROPERTIESONLY = 0x8,
        GPS_OPENSLOWITEM = 0x10,
        GPS_DELAYCREATION = 0x20,
        GPS_BESTEFFORT = 0x40,
        GPS_NO_OPLOCK = 0x80,
        GPS_PREFERQUERYPROPERTIES = 0x100,
        GPS_EXTRINSICPROPERTIES = 0x200,
        GPS_EXTRINSICPROPERTIESONLY = 0x400,
        GPS_MASK_VALID = 0x7FF
    }
    public enum SIGDN : int
    {
        SIGDN_NORMALDISPLAY = 0x0,
        SIGDN_PARENTRELATIVEPARSING = unchecked((int)0x80018001),
        SIGDN_DESKTOPABSOLUTEPARSING = unchecked((int)0x80028000),
        SIGDN_PARENTRELATIVEEDITING = unchecked((int)0x80031001),
        SIGDN_DESKTOPABSOLUTEEDITING = unchecked((int)0x8004C000),
        SIGDN_FILESYSPATH = unchecked((int)0x80058000),
        SIGDN_URL = unchecked((int)0x80068000),
        SIGDN_PARENTRELATIVEFORADDRESSBAR = unchecked((int)0x8007C001),
        SIGDN_PARENTRELATIVE = unchecked((int)0x80080001)
    }
    public enum SIATTRIBFLAGS
    {
        SIATTRIBFLAGS_AND = 0x1,
        SIATTRIBFLAGS_OR = 0x2,
        SIATTRIBFLAGS_APPCOMPAT = 0x3,
        SIATTRIBFLAGS_MASK = 0x3,
        SIATTRIBFLAGS_ALLITEMS = 0x4000
    }
}
