#r "System"
#r "System.Core"
#r "System.Xml"
#r "System.Xml.Linq"

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Win32;


string[] DeleteKeywords = new string[]{
    "Visual Studio",
    "VisualStudio"
};
string[] DeleteExcludeKeywords = new string[]{
    "Visual Studio Code",
    "VisualStudioCode"
};

bool g_EnableRealDelete = false;



/** このスクリプトに与えられたコマンドラインオプション 
 * /deleteenable
 * 
 * /deleteenable：実際に削除を実行する。これを指定しない場合、標準出力を出すだけで削除は実行しない。
 */
string[] CommandLineOptions = Environment.GetCommandLineArgs().Skip(2).ToArray();//CSScriptLauncher.exeは第1引数にスクリプトのパスを受け取る。第2引数以降が本スクリプト向けのオプションなので、2つSkipするとコマンドラインの配列になる。



TraceOut($"Start, Command={string.Join(",", CommandLineOptions)}");

var ret = Main();

return ret;


void TraceDebug(string msg){
    System.Diagnostics.Trace.WriteLine("VisualStudioRegAllDelete:" + msg);
}

void TraceOut(string msg)
{
    Console.Out.WriteLine(msg);
}

void TraceError(string msg)
{
    Console.Error.Write(msg);
}


void ParseCommandLine()
{
    for(int i = 0; i < CommandLineOptions.Length; i++)
    {
        switch(CommandLineOptions[i].TrimStart('-', '/'))
        {
            case "deleteenable":
                if (CommandLineOptions.Length <= i)
                {
                    throw new ArgumentException($"CommandLine Invalid");
                }

                g_EnableRealDelete = true;

                break;
            default:
                break;
        }
    }


}


int Main()
{
    try
    {
        TraceOut("Main Start, " + DateTime.Now.ToString());

        ParseCommandLine();


        using(var reg = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)){
            TraceOut($"VSRegAllDeleteRecursive Call Start {reg.ToString()}");
            VSRegAllDeleteRecursive(reg);
        }
        using(var reg = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)){
            TraceOut($"VSRegAllDeleteRecursive Call Start {reg.ToString()}");
            VSRegAllDeleteRecursive(reg);
        }
        using(var reg = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64)){
            TraceOut($"VSRegAllDeleteRecursive Call Start {reg.ToString()}");
            VSRegAllDeleteRecursive(reg);
        }
        using(var reg = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32)){
            TraceOut($"VSRegAllDeleteRecursive Call Start {reg.ToString()}");
            VSRegAllDeleteRecursive(reg);
        }


        TraceOut("Main End Success, " + DateTime.Now.ToString());

    }
    catch(Exception ex)
    {
        TraceError($"Fail, {DateTime.Now.ToString()}, {ex.ToString()}");
        return 11;
    }

    return 0;
}

bool ValidateIsIncludeVisualStudio(string target){
    if(DeleteKeywords.Any(item => 0 <= target.IndexOf(item)) &&
        !DeleteExcludeKeywords.Any(item => 0 <= target.IndexOf(item))){
        return true;
    }
    else{
        return false;
    }
}


void VSRegAllDeleteRecursive(RegistryKey BaseKey){

    TraceDebug($"{BaseKey.ToString()} VSRegAllDeleteRecursive Start");

    foreach(var SubName in BaseKey.GetSubKeyNames()){        
        
        if(ValidateIsIncludeVisualStudio(SubName)){
            if(g_EnableRealDelete){
                
                try{
                    BaseKey.DeleteSubKeyTree(SubName);
                }
                catch(System.Security.SecurityException ex){
                    TraceDebug($"{BaseKey.ToString()} DeleteSubKeyTree SecurityException {SubName} Skip");
                }
                catch(System.UnauthorizedAccessException ex){
                    TraceDebug($"{BaseKey.ToString()} DeleteSubKeyTree UnauthorizedAccessException {SubName} Skip");
                }
            }
            TraceOut($"{BaseKey.ToString()} DeleteSubKeyTree({SubName})(Key Name Include)");
            continue;
        }

        try{

            using(var SubKey = BaseKey.OpenSubKey(SubName)){
                VSRegAllDeleteRecursive(SubKey);
            }   
        }
        catch(System.Security.SecurityException ex){
            TraceDebug($"{BaseKey.ToString()} SecurityException {SubName} Skip");
        }

    }

    foreach(var SubName in BaseKey.GetValueNames()){
        
        if(ValidateIsIncludeVisualStudio(SubName)){
            if(g_EnableRealDelete){
                try{
                    BaseKey.DeleteValue(SubName);
                }
                catch(System.Security.SecurityException ex){
                    TraceDebug($"{BaseKey.ToString()} DeleteValue SecurityException {SubName} Skip");
                }
                catch(System.UnauthorizedAccessException ex){
                    TraceDebug($"{BaseKey.ToString()} DeleteValue UnauthorizedAccessException {SubName} Skip");
                }
            }
            TraceOut($"{BaseKey.ToString()} DeleteValue({SubName})(Value Name Include)");
            continue;
        }
        
        if(BaseKey.GetValueKind(SubName) != RegistryValueKind.String){
            continue;
        }

        {
            var value = (string)BaseKey.GetValue(SubName);

            if(ValidateIsIncludeVisualStudio(value)){
                if(g_EnableRealDelete){
                    try{
                        BaseKey.DeleteValue(SubName);
                    
                    }
                    catch(System.Security.SecurityException ex){
                        TraceDebug($"{BaseKey.ToString()} DeleteValue SecurityException {SubName} Skip");
                    }
                    catch(System.UnauthorizedAccessException ex){
                        TraceDebug($"{BaseKey.ToString()} DeleteValue UnauthorizedAccessException {SubName} Skip");
                    }
                }
                TraceOut($"{BaseKey.ToString()} DeleteValue({SubName}={value})(Value Include)");
                continue;
            }
        }        
    }

    
    TraceDebug($"{BaseKey.ToString()} VSRegAllDeleteRecursive End");
}
