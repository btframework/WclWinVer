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
unit wclWinVer;

{ Compiler directives. }

{$DEFINITIONINFO ON}
{$REFERENCEINFO ON}

{$ASSERTIONS OFF}
{$DEBUGINFO OFF}
{$LOCALSYMBOLS OFF}
{$IOCHECKS OFF}
{$OVERFLOWCHECKS OFF}
{$OPTIMIZATION OFF}

{$STACKFRAMES OFF}
{$SAFEDIVIDE OFF}
{$IFDEF VER130}
  {$ALIGN ON}
{$ELSE}
  {$ALIGN 8}
{$ENDIF}
{$RANGECHECKS OFF}
{$VARSTRINGCHECKS OFF}
{$BOOLEVAL OFF}
{$EXTENDEDSYNTAX ON}
{$TYPEDADDRESS OFF}
{$OPENSTRINGS OFF}
{$LONGSTRINGS ON}
{$WRITEABLECONST OFF}
{$MINENUMSIZE 4}

{$IFDEF BCB}
  {$ObjExportAll On}
{$ENDIF}

interface

type
  /// <summary> Windows versions. </summary>
  TwclWinVer = (
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
    verWin10 = 7);

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
/// <seealso cref="TwclWinVer" />
function wclGetWinVer(out Build: Word): TwclWinVer;
// <summary> Checks if software runs on Windows 10 IoT. </summary>
/// <returns> The function returns <c>True</c> if it runs on Windows 10 IoT.
///   Otherwise it returns <c>False</c>. </returns>
function wclIsWinIot: Boolean;

implementation

uses
  Windows, SysUtils;

function wclGetWinVer(out Build: Word): TwclWinVer;
{$J+}
// Save detected OS version so we do not need to check it each time.
const
  OsVersion: TwclWinVer = verUnknown;
  OsBuild: Word = 0;
{$J-}

var
  VerInfoSize: Cardinal;
  Dummy: Cardinal;
  VerInfo: Pointer;
  BoolRes: BOOL;
  VerValue: PVSFixedFileInfo;
  VerValueSize: Cardinal;
  Major: Word;
  Minor: Word;
  Res: LRESULT;
  Key: HKEY;
  RegVer: DWORD;
  RegValSize: DWORD;
  RegBuild: array [0..63] of Char; // Should not be longer?
begin
  if (OsVersion = verUnknown) and (OsBuild = 0) then begin
    // First, try to detect OS version using version of kernel32.dll.
    VerInfoSize := GetFileVersionInfoSize('kernel32.dll', Dummy);
    if VerInfoSize > 0 then begin
      VerInfo := GetMemory(VerInfoSize);
      if VerInfo <> nil then begin
        try
          BoolRes := GetFileVersionInfo('kernel32.dll', 0, VerInfoSize,
            VerInfo);
          if BoolRes then begin
            BoolRes := VerQueryValue(VerInfo, '\', Pointer(VerValue),
              VerValueSize);
            if BoolRes then begin
              // Decode major and minor versions.
              Major := HiWord(VerValue^.dwFileVersionMS);
              Minor := LoWord(VerValue^.dwFileVersionMS);
              OsBuild := HiWord(VerValue^.dwFileVersionLS);

              case Major of
                5: OsVersion := verWinXP;

                6: begin
                     // Few OSes have major version number 6.
                     case Minor of
                       0: OsVersion := verWinVista;
                       1: OsVersion := verWin7;
                       2: OsVersion := verWin8;
                       3: OsVersion := verWin81;
                     end;

                     // We may have problems with Win 8.1. So we must do
                     // additional checks.
                     if (OsVersion = verWin8) or (OsVersion = verWin81) then
                     begin
                       // Because Win 10 internally changes version number
                       // returned by version functions depending on application
                       // targetings we have to do some additional tests. So if
                       // Win8.1 was detected we must check registry to get
                       // exactly version number. TO do so we read reg. key
                       // appeared on Win 10 only.
                       Res := RegOpenKeyEx(HKEY_LOCAL_MACHINE,
                         'SOFTWARE\Microsoft\Windows NT\CurrentVersion', 0,
                         KEY_READ, Key);
                       if Res = ERROR_SUCCESS then begin
                         try
                           RegVer := 0;
                           RegValSize := 4;
                           Res := RegQueryValueEx(Key,
                             'CurrentMajorVersionNumber', nil, nil,
                             PBYTE(@RegVer), @RegValSize);
                           if (Res = ERROR_SUCCESS) and (RegVer = 10) then begin
                             OsVersion := verWin10;
                             // Now we have to read correct build number.
                             RegValSize := SizeOf(RegBuild);
                             ZeroMemory(@RegBuild, RegValSize);
                             Res := RegQueryValueEx(Key, 'CurrentBuildNumber',
                               nil, nil, PBYTE(@RegBuild), @RegValSize);
                             if Res = ERROR_SUCCESS then
                               OsBuild := LoWord(StrToInt(string(RegBuild)));
                           end;

                         finally
                           RegCloseKey(Key);
                         end;
                       end;
                     end;
                   end;

                10: OsVersion := verWin10; // Who knows?
              end;
            end;
          end;

        finally
          FreeMemory(VerInfo);
        end;
      end;
    end;
  end;

  Build := OsBuild;
  Result := OsVersion;
end;

function wclIsWinIot: Boolean;
const
  KEY_NAME = 'Software\Microsoft\Windows NT\CurrentVersion';
  VAL_NAME = 'ProductName';

var
  Key: HKEY;
  Val: array [0..MAX_PATH - 1] of Char;
  Size: DWORD;
  Ver: string;
begin
  Result := False;

  if RegOpenKey(HKEY_LOCAL_MACHINE, KEY_NAME, Key) = ERROR_SUCCESS then begin
    Size := SizeOf(Val);
    ZeroMemory(@Val, Size);

    if RegQueryValueEx(Key, VAL_NAME, nil, nil, PByte(@Val), @Size) = ERROR_SUCCESS then
    begin
      Ver := string(Val);
      Result := (Pos('IoT', Ver) > 0);
    end;

    RegCloseKey(Key);
  end;
end;

end.
