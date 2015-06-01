using System;

using UnityEngine;
using FluorineFx.Net;
using LitJson;


public class RtmpS2C_Interface : IRTMPS2CBase
{
    void IRTMPS2CBase.onBalanceExchange(object obj)
    {
        Debug.Log("onBalanceExchange");
    }

    void IRTMPS2CBase.onBeginGame(object obj)
    {
        Debug.Log("onBeginGame");
    }
    void IRTMPS2CBase.onEndGame(object obj)
    {
        Debug.Log("onEndGame");
    }

    void IRTMPS2CBase.OnConnect(object obj, EventArgs e)
    {
        throw new NotImplementedException();
    }

    public void onCreditExchange(object obj)
    {

        string str_json = JsonMapper.ToJson(obj);
        

        Debug.Log("onCreditExchange " + str_json);
    }

    void IRTMPS2CBase.OnDisConnect(object obj, EventArgs e)
    {
        throw new NotImplementedException();
    }


    void IRTMPS2CBase.onGetMachineList(object obj)
    {
        throw new NotImplementedException();
    }

    void IRTMPS2CBase.onLogin(object obj)
    {
        throw new NotImplementedException();
    }

    void IRTMPS2CBase.OnNetStatus(object obj, NetStatusEventArgs e)
    {
        throw new NotImplementedException();
    }

    void IRTMPS2CBase.onOnLoadInfo2(object obj)
    {
        throw new NotImplementedException();
    }

    void IRTMPS2CBase.onTakeMachine(object obj)
    {
        throw new NotImplementedException();
    }

    void IRTMPS2CBase.updateJP(object obj)
    {
        throw new NotImplementedException();
    }

    void IRTMPS2CBase.updateJPList(object obj)
    {
        throw new NotImplementedException();
    }

    void IRTMPS2CBase.updateMarquee(object obj)
    {
        throw new NotImplementedException();
    }
}
