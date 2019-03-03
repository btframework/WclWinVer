## wclGetWinVer()

Detecting Windows OS version (including Win 10)

**Delphi**: `function wclGetWinVer(out Build: Word): TwclWinVer;`

**C#**: `public static wclWindowsVersion.wclWinVer wclWindowsVersion.wclGetWinVer(out UInt16 Build);`

**C++**: `wclWinVer wclGetWinVer(unsigned short& Build);`

Returns current OS version.
On output, the "Build" parameter contains the OS build number.

For Windows 10 (returning value is verWin10) the Build number can be converted to Release number using table below:

| Build | Release |
| ----- | ------- |
| 10240 |   1507  |
| 10586 |   1511  |
| 14393 |   1607  |
| 15063 |   1703  |
| 16299 |   1709  |
| 17134 |   1803  |
| 17763 |   1809  |

On Windows 10 the Release (Build) defines supported features set. MSDN when describing feature uses Build number.
But some articles usualy references to Release. So it can be useful to convert Win 10 Build to Win 10 Release.

## wclIsWinIot()

Checks if  runs on Windows 10 IoT.

**Delphi** `function wclIsWinIot: Boolean;`

**C#**: `public static Boolean wclWindowsVersion.wclIsWinIot();`

**C++**: `bool wclIsWinIot();`

Returns "True" if the OS is Windows 10 IoT. To get more detailed information about OS version (OS Build number)
use wclGetWinVer() function.
