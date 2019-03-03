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
#include "stdafx.h"
#include "Windows.h"

#include <string>
#include <tchar.h>

#include "wclWinVer.h"

#pragma comment(lib, "version.lib")
#pragma comment(lib, "advapi32.lib")

namespace wcl
{

wclWinVer wclGetWinVer(unsigned short& Build)
{
	// Save detected OS version so we do not need to check it each time.
	static wclWinVer OsVersion = verUnknown;
	static WORD OsBuild = 0;

	if (OsVersion == verUnknown && OsBuild == 0)
	{
		// First, try to detect OS version using version of kernel32.dll.
		DWORD VerInfoSize = GetFileVersionInfoSize(_T("kernel32.dll"), NULL);
		if (VerInfoSize > 0)
		{
			void* VerInfo = malloc(VerInfoSize);
			if (VerInfo != NULL)
			{
				BOOL BoolRes = GetFileVersionInfo(_T("kernel32.dll"), 0, VerInfoSize, VerInfo);
				if (BoolRes)
				{
					VS_FIXEDFILEINFO* VerValue = NULL;
					DWORD VerValueSize = 0;
					BoolRes = VerQueryValue(VerInfo, _T("\\"), (LPVOID*)&VerValue, (PUINT)&VerValueSize);
					if (BoolRes)
					{
						// Decode major and minor versions.
						WORD Major = HIWORD(VerValue->dwFileVersionMS);
						WORD Minor = LOWORD(VerValue->dwFileVersionMS);
						OsBuild = HIWORD(VerValue->dwFileVersionLS);
						
						switch (Major)
						{
							case 5:
								OsVersion = verWinXP;
								break;
							
							case 6:
								// Few OSes have major version number 6.
								switch (Minor)
								{
									case 0:
										OsVersion = verWinVista;
										break;
									case 1:
										OsVersion = verWin7;
										break;
									case 2:
										OsVersion = verWin8;
										break;
									case 3:
										OsVersion = verWin81;
										break;
								}
								
								// We may have problems with Win 8.1. So we must do
								// additional checks.
								if (OsVersion == verWin8 || OsVersion == verWin81)
								{
									// Because Win 10 internally changes version number
									// returned by version functions depending on application
									// targetings we have to do some additional tests. So if
									// Win8.1 was detected we must check registry to get
									// exactly version number. TO do so we read reg. key
									// appeared on Win 10 only.
									HKEY Key;
									LRESULT Res = RegOpenKeyEx(HKEY_LOCAL_MACHINE,
										_T("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion"), 0,
										KEY_READ, &Key);
									if (Res == ERROR_SUCCESS)
									{
										DWORD RegVer = 0;
										DWORD RegValSize = 4;
										Res = RegQueryValueEx(Key, _T("CurrentMajorVersionNumber"), NULL, NULL,
											(LPBYTE)&RegVer, &RegValSize);
										if (Res == ERROR_SUCCESS && RegVer == 10)
										{
											OsVersion = verWin10;
											// Now we have to read correct build number.
											TCHAR RegBuild[64];
											RegValSize = sizeof(RegBuild);
											ZeroMemory(&RegBuild, RegValSize);
											Res = RegQueryValueEx(Key, _T("CurrentBuildNumber"), NULL, NULL,
												(LPBYTE)&RegBuild, &RegValSize);
											if (Res == ERROR_SUCCESS)
											{
												int v = _ttoi(RegBuild);
												OsBuild = LOWORD(v);
											}
										}
										RegCloseKey(Key);
									}
								}
								break;
							
							case 10:
								OsVersion = verWin10; // Who knows?
								break;
						}
					}
				}
				free(VerInfo);
			}
		}
	}
	
	Build = OsBuild;
	return OsVersion;
}

#define KEY_NAME	_T("Software\\Microsoft\\Windows NT\\CurrentVersion")
#define VAL_NAME	_T("ProductName")

bool wclIsWinIot()
{
	bool Result = false;

	HKEY Key;
	if (RegOpenKey(HKEY_LOCAL_MACHINE, KEY_NAME, &Key) == ERROR_SUCCESS)
	{
		TCHAR Val[MAX_PATH - 1] = { 0 };
		DWORD Size = sizeof(Val);

		if (RegQueryValueEx(Key, VAL_NAME, NULL, NULL, (PBYTE)Val, &Size) == ERROR_SUCCESS)
		{
			#ifdef UNICODE
				std::wstring Ver = std::wstring(Val);
				Result = (Ver.find(_T("IoT")) != std::wstring::npos);
			#else
				std::string Ver = std::string(Val);
				Result = (Ver.find(_T("IoT")) != std::string::npos);
			#endif
			
		}

		RegCloseKey(Key);
	}

	return Result;
}

}