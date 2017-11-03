﻿using UnityEngine;
using System.Collections;

public class UIMessageBoxHelper 
{

    public delegate void ShowDelegate0(string msg);
    public delegate void ShowDelegate1(string msg, System.Action ok, System.Action cancel);
    public delegate void ShowDelegate2(string tip, string msg, System.Action ok, System.Action cancel);
    public delegate void ShowDelegate3(string msg, System.Action ok);
    public delegate void CancelDelegate();


    public static ShowDelegate0 onShow0;
    public static ShowDelegate1 onShow1;
    public static ShowDelegate2 onShow2;
    public static ShowDelegate3 onShow3;
    public static CancelDelegate onCancel;

    public static void Show(string msg)
    {
        if (onShow0 != null) onShow0(msg);
    }

    public static void Show(string msg,System.Action ok ,System.Action cancel)
    {
        if (onShow1 != null) onShow1(msg,ok,cancel);    
    }

    public static void ShowUpdate(string tip,string msg, System.Action ok , System.Action cancel )
    {
        if (onShow2 != null) onShow2(tip,msg,ok, cancel);    
    }

    public static void Show(string msg, System.Action ok)
    {
        if (onShow3 != null) onShow3(msg, ok);  
    }


    public static void Cancel()
    {
        if (onCancel != null) onCancel();
    }
}
