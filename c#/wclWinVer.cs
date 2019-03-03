////////////////////////////////////////////////////////////////////////////////
//                                                                            //
//   Wireless Communication Library 7                                         //
//                                                                            //
//   Copyright (C) 2006-2019 Mike Petrichenko                                 //
//                           Soft Service Company                             //
//                           All Rights Reserved                              //
//                                                                            //
//   http://www.btframework.com                                               //
//                                                                            //
//   support@btframework.com                                                  //
//   shop@btframework.com                                                     //
//                                                                            //
// -------------------------------------------------------------------------- //
//                                                                            //
//   This file includes code from Wireless Communication Libraty.             //
//                                                                            //
// -------------------------------------------------------------------------- //
//                                                                            //
// Permission is hereby granted, free of charge, to any person obtaining a    //
// copy of this code, to use it in the Software. You have the rights          //
// to use, copy, merge, publish and distribute this code as part of your      //
// Software. You must NOT modify or sell this code. The Software that uses    //
// this code must include reference to Wireless Communication Library in its  //
// documentation and in its source code.                                      //
//                                                                            //
// The above copyright notice and this permission notice shall be included in //
// all copies or substantial portions of the Software.                        //
//                                                                            //
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR //
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,   //
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL    //
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER //
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING    //
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER        //
// DEALINGS IN THE SOFTWARE.                                                  //
//                                                                            //
////////////////////////////////////////////////////////////////////////////////
using System;
using System.Runtime.InteropServices;

namespace WCL
{
    /// <summary> Windows versions. </summary>
    public enum wclWinVer
    {
        /// <summary> Unknown Windows version. </summary>
        verUnknown = 1,
        /// <summary> Windows XP. </summary>
        verWinXP = 2,
        /// <summary> Windows Vista. </summary>
        verWinVista = 3,
        /// <summary> Windows 7. </summary>
        verWin7 = 4,
        /// <summary> Windows 8. </summary>
        verWin8 = 5,
        /// <summary> Windows 8.1. </summary>
        verWin81 = 6,
        /// <summary> Windows 10. </summary>
        verWin10 = 7
    };

    public static class wclWindowsVersion
    {
        // Save detected OS version so we do not need to check it each time.
        private static wclWinVer OsVersion = wclWinVer.verUnknown;
        private static UInt16 OsBuild = 0;

        private const UInt32 KEY_QUERY_VALUE = 0x0001;
        private const UInt32 KEY_ENUMERATE_SUB_KEYS = 0x0008;
        private const UInt32 KEY_NOTIFY = 0x0010;

        private const UInt32 STANDARD_RIGHTS_READ = 0x00020000;
        private const UInt32 SYNCHRONIZE = 0x00100000;
        private const UInt32 KEY_READ = (STANDARD_RIGHTS_READ | KEY_QUERY_VALUE |
            KEY_ENUMERATE_SUB_KEYS | KEY_NOTIFY) & (~SYNCHRONIZE);

        private static IntPtr HKEY_LOCAL_MACHINE = new IntPtr(unchecked((Int32)0x80000002));

        private const UInt32 ERROR_SUCCESS = 0;
        
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct VS_FIXEDFILEINFO
        {
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwSignature;
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwStrucVersion;
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwFileVersionMS;
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwFileVersionLS;
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwProductVersionMS;
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwProductVersionLS;
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwFileFlagsMask;
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwFileFlags;
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwFileOS;
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwFileType;
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwFileSubtype;
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwFileDateMS;
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwFileDateLS;
        };

        [DllImport("version.dll", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U4)]
        private static extern UInt32 GetFileVersionInfoSize(
            [param: MarshalAs(UnmanagedType.LPTStr), In] String lptstrFilename,
            [param: MarshalAs(UnmanagedType.U4), In, Out] ref UInt32 lpdwHandle);

        [DllImport("version.dll", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern Boolean GetFileVersionInfo(
            [param: MarshalAs(UnmanagedType.LPTStr), In] String lptstrFilename,
            [param: MarshalAs(UnmanagedType.U4), In] UInt32 dwHandle,
            [param: MarshalAs(UnmanagedType.U4), In] UInt32 dwLen,
            [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr lpData);

        [DllImport("version.dll", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern Boolean VerQueryValue(
            [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr pBlock,
            [param: MarshalAs(UnmanagedType.LPTStr), In] String lpSubBlock,
            [param: MarshalAs(UnmanagedType.SysInt), In, Out] ref IntPtr lplpBuffer,
            [param: MarshalAs(UnmanagedType.U4), In, Out] ref UInt32 puLen);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I4)]
        private static extern Int32 RegOpenKey(
            [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr hKey,
            [param: MarshalAs(UnmanagedType.LPTStr), In] String lpSubKey,
            [param: MarshalAs(UnmanagedType.SysInt), Out] out IntPtr phkResult);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I4)]
        private static extern Int32 RegOpenKeyEx(
            [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr hKey,
            [param: MarshalAs(UnmanagedType.LPTStr), In] String lpSubKey,
            [param: MarshalAs(UnmanagedType.U4), In] UInt32 ulOptions,
            [param: MarshalAs(UnmanagedType.U4), In] UInt32 samDesired,
            [param: MarshalAs(UnmanagedType.SysInt), Out] out IntPtr phkResult);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I4)]
        private static extern Int32 RegQueryValueEx(
            [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr hKey,
            [param: MarshalAs(UnmanagedType.LPTStr), In] String lpValueName,
            [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr lpReserved,
            [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr lpType,
            [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr lpData,
            [param: MarshalAs(UnmanagedType.U4), In, Out] ref UInt32 lpcbData);

        [DllImport("advapi32.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I4)]
        private static extern Int32 RegCloseKey(
            [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr hKey);

        /// <summary> Reads Windows OS version. </summary>
        /// <param name="Build"> On output contains the OS build number. </param>
        /// <returns> Returns OS version. </returns>
        /// <remarks> <para> For Win 10 <c>Build</c> can be translated to OS version
        ///   (release) using table below. </para>
        ///   <para><c> Build </c> <c> Release </c></para>
        ///   <para><c>=======</c> <c>=========</c></para>
        ///   <para><c> 10240 </c> <c>   1507  </c></para>
        ///   <para><c> 10586 </c> <c>   1511  </c></para>
        ///   <para><c> 14393 </c> <c>   1607  </c></para>
        ///   <para><c> 15063 </c> <c>   1703  </c></para>
        ///   <para><c> 16299 </c> <c>   1709  </c></para>
        ///   <para><c> 17134 </c> <c>   1803  </c></para>
        ///   <para><c> 17763 </c> <c>   1809  </c></para> </remarks>
        /// <seealso cref="wclWinVer" />
        public static wclWinVer wclGetWinVer(out UInt16 Build)
        {
            if (OsVersion == wclWinVer.verUnknown && OsBuild == 0)
            {
                // First, try to detect OS version using version of kernel32.dll.
                UInt32 Dummy = 0;
                UInt32 VerInfoSize = GetFileVersionInfoSize("kernel32.dll", ref Dummy);
                if (VerInfoSize > 0)
                {
                    IntPtr VerInfo = Marshal.AllocHGlobal((Int32)VerInfoSize);
                    if (VerInfo != IntPtr.Zero)
                    {
                        try
                        {
                            Boolean BoolRes = GetFileVersionInfo("kernel32.dll", 0, VerInfoSize, VerInfo);
                            if (BoolRes)
                            {
                                UInt32 VerValueSize = (UInt32)Marshal.SizeOf(typeof(VS_FIXEDFILEINFO));
                                IntPtr pVerValue = IntPtr.Zero;
                                BoolRes = VerQueryValue(VerInfo, "\\", ref pVerValue, ref VerValueSize);
                                if (BoolRes)
                                {
                                    // Decode major and minor versions.
                                    VS_FIXEDFILEINFO VerValue = (VS_FIXEDFILEINFO)Marshal.PtrToStructure(pVerValue,
                                        typeof(VS_FIXEDFILEINFO));

                                    UInt16 Major = (UInt16)(VerValue.dwFileVersionMS >> 16); // HIWORD
                                    UInt16 Minor = (UInt16)(VerValue.dwFileVersionMS & 0x0000FFFF); // LOWORD
                                    OsBuild = (UInt16)(VerValue.dwFileVersionLS >> 16); // HIWORD

                                    switch (Major)
                                    {
                                        case 5:
                                            OsVersion = wclWinVer.verWinXP;
                                            break;

                                        case 6:
                                            // Few OSes have major version number 6.
                                            switch (Minor)
                                            {
                                                case 0:
                                                    OsVersion = wclWinVer.verWinVista;
                                                    break;
                                                case 1:
                                                    OsVersion = wclWinVer.verWin7;
                                                    break;
                                                case 2:
                                                    OsVersion = wclWinVer.verWin8;
                                                    break;
                                                case 3:
                                                    OsVersion = wclWinVer.verWin81;
                                                    break;
                                            }

                                            // We may have problems with Win 8.1. So we must do
                                            // additional checks.
                                            if (OsVersion == wclWinVer.verWin8 || OsVersion == wclWinVer.verWin81)
                                            {
                                                // Because Win 10 internally changes version number
                                                // returned by version functions depending on application
                                                // targetings we have to do some additional tests. So if
                                                // Win8.1 was detected we must check registry to get
                                                // exactly version number. TO do so we read reg. key
                                                // appeared on Win 10 only.
                                                IntPtr Key;
                                                Int32 Res = RegOpenKeyEx(HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion",
                                                    0, KEY_READ, out Key);
                                                if (Res == ERROR_SUCCESS)
                                                {
                                                    try
                                                    {
                                                        UInt32 RegValSize = 4;
                                                        IntPtr pRegVer = Marshal.AllocHGlobal(4);
                                                        if (pRegVer != IntPtr.Zero)
                                                        {
                                                            try
                                                            {
                                                                Res = RegQueryValueEx(Key, "CurrentMajorVersionNumber", IntPtr.Zero, IntPtr.Zero,
                                                                    pRegVer, ref RegValSize);
                                                                if (Res == ERROR_SUCCESS)
                                                                {
                                                                    UInt32 RegVer = (UInt32)Marshal.ReadInt32(pRegVer);
                                                                    if (RegVer == 10)
                                                                    {
                                                                        OsVersion = wclWinVer.verWin10;
                                                                        // Now we have to read correct build number.
                                                                        RegValSize = 64 * sizeof(Char);
                                                                        IntPtr pRegBuild = Marshal.AllocHGlobal((Int32)RegValSize);
                                                                        if (pRegBuild != IntPtr.Zero)
                                                                        {
                                                                            try
                                                                            {
                                                                                Res = RegQueryValueEx(Key, "CurrentBuildNumber", IntPtr.Zero,
                                                                                    IntPtr.Zero, pRegBuild, ref RegValSize);
                                                                                if (Res == ERROR_SUCCESS)
                                                                                {
                                                                                    string RegBuild = Marshal.PtrToStringAuto(pRegBuild);
                                                                                    OsBuild = Convert.ToUInt16(RegBuild);
                                                                                }
                                                                            }
                                                                            finally
                                                                            {
                                                                                Marshal.FreeHGlobal(pRegBuild);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            finally
                                                            {
                                                                Marshal.FreeHGlobal(pRegVer);
                                                            }
                                                        }
                                                    }
                                                    finally
                                                    {
                                                        RegCloseKey(Key);
                                                    }
                                                }
                                            }
                                            break;

                                        case 10:
                                            OsVersion = wclWinVer.verWin10; // Who knows?
                                            break;
                                    }
                                }
                            }
                        }
                        finally
                        {
                            Marshal.FreeHGlobal(VerInfo);
                        }
                    }
                }
            }

            Build = OsBuild;
            return OsVersion;
        }

        /// <summary> Checks if software runs on Windows 10 IoT. </summary>
        /// <returns> The function returns <c>True</c> if it runs on Windows 10 IoT.
        ///   Otherwise it returns <c>False</c>. </returns>
        public static Boolean wclIsWinIot()
        {
            String KEY_NAME = "Software\\Microsoft\\Windows NT\\CurrentVersion";
            String VAL_NAME = "ProductName";

            Boolean Result = false;

            IntPtr Key;
            if (RegOpenKey(HKEY_LOCAL_MACHINE, KEY_NAME, out Key) == ERROR_SUCCESS)
            {
                UInt32 Size = (UInt32)(255 * sizeof(Char));
                IntPtr Val = Marshal.AllocHGlobal((int)Size);
                if (Val != IntPtr.Zero)
                {
                    if (RegQueryValueEx(Key, VAL_NAME, IntPtr.Zero, IntPtr.Zero, Val, ref Size) == ERROR_SUCCESS)
                    {
                        String Ver = Marshal.PtrToStringAuto(Val);
                        Result = (Ver.IndexOf("IoT") > -1);
                    }
                    Marshal.FreeHGlobal(Val);
                }

                RegCloseKey(Key);
            }

            return Result;
        }
    };
}
