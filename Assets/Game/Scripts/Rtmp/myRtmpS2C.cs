
using System;
using FluorineFx.Net;

using LitJson;

public class myRtmpS2C 
{
    public void onBalanceExchange(object obj)
    {
        throw new NotImplementedException();
    }

    public void onBeginGame(object obj)
    {
        throw new NotImplementedException();
    }

    public void OnConnect(object obj, EventArgs e)
    {
        /*
        string str_json = JsonMapper.ToJson(obj);
        JsonData jd = JsonMapper.ToObject(str_json);
        */
        Global_HEY.rtmpC2S.LoginBySID("5835");
    }
    

    public void onCreditExchange(object obj)
    {
        throw new NotImplementedException();
    }

    public void OnDisConnect(object obj, EventArgs e)
    {
        throw new NotImplementedException();
    }    

    public void onEndGame(object obj)
    {
        throw new NotImplementedException();
    }

    public void onGetMachineList(object obj)
    {
        Global_HEY.rtmpC2S.TakeMachine(null);
    }

    public void onLogin(object obj)
    {
        string str_json = JsonMapper.ToJson(obj);
        JsonData jd = JsonMapper.ToObject(str_json);
    }

    public void OnNetStatus(object obj, NetStatusEventArgs e)
    {
    }
    
    public void onOnLoadInfo2(object obj)
    {
        throw new NotImplementedException();
    }

    public void onTakeMachine(object obj)
    {
        Global_HEY.rtmpC2S.onLoadInfo2();
    }

    public void updateJP(object obj)
    {
        throw new NotImplementedException();
    }

    public void updateJPList(object obj)
    {
        throw new NotImplementedException();
    }

    public void updateMarquee(object obj)
    {
        throw new NotImplementedException();
    }
}
