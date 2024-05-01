using System.Runtime.InteropServices;
using HarfBuzzSharp;

namespace Monaco.Debugging.Native;

// Taken from HarfBuzzSharp's internal generated bindings
public unsafe static class HBNative
{
#if __IOS__ || __TVOS__
		private const string _harfBuzz = "@rpath/libHarfBuzzSharp.framework/libHarfBuzzSharp";
#else
    private const string _harfBuzz = "libHarfBuzzSharp";
#endif

    [DllImport(_harfBuzz, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint hb_ot_name_get_utf8(nint face, OpenTypeNameId name_id, nint language, uint* text_size, /* char */ void* text);

    [DllImport(_harfBuzz, CallingConvention = CallingConvention.Cdecl)]
    internal static extern nint hb_language_from_string([MarshalAs(UnmanagedType.LPStr)] string str, int len);
}
