using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace MoviePlayer.Class
{
   public static class DllInvoke
    {
        [DllImport("kernel32.dll", EntryPoint = "LoadLibrary")]
        public static extern int LoadLibrary(
             [MarshalAs(UnmanagedType.LPStr)] string lpLibFileName);

        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress")]
        public static extern IntPtr GetProcAddress(int hModule,
            [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

        [DllImport("kernel32.dll", EntryPoint = "FreeLibrary")]
        public static extern bool FreeLibrary(int hModule);


        //#region Win API
        //[DllImport(@"D:\dll\usbjoy-alldof\vs2010\x64\Release\USBJOY_DLL.dll")]
        //public extern static IntPtr LoadLibrary(string path);

        //[DllImport(@"D:\dll\usbjoy-alldof\vs2010\x64\Release\USBJOY_DLL.dll")]
        //public extern static IntPtr GetProcAddress(IntPtr lib, string funcName);

        //[DllImport(@"D:\dll\usbjoy-alldof\vs2010\x64\Release\USBJOY_DLL.dll")]
        //public extern static bool FreeLibrary(IntPtr lib);
        //#endregion

        //public IntPtr hLib;        
        //public DllInvoke(String DLLPath)
        //{
        //    hLib = LoadLibrary(DLLPath);
        //}

        //~DllInvoke()
        //{
        //    FreeLibrary(hLib);            
        //}

        ////将要执行的函数转换为委托
        //public Delegate Invoke (string APIName,Type t)  
        //{
        //    IntPtr api = GetProcAddress(hLib, APIName);
        //    return (Delegate)Marshal.GetDelegateForFunctionPointer(api, t);
        //}


    }
}
