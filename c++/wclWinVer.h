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
#pragma once

namespace wcl
{

/// <summary> Windows versions. </summary>
typedef enum
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
} wclWinVer;

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
wclWinVer wclGetWinVer(unsigned short& Build);

/// <summary> Checks if software runs on Windows 10 IoT. </summary>
/// <returns> The function returns <c>True</c> if it runs on Windows 10 IoT.
///   Otherwise it returns <c>False</c>. </returns>
bool wclIsWinIot();

}