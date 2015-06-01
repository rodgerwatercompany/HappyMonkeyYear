using UnityEngine;

using System;
using FluorineFx;
using FluorineFx.Net;

public class RTMPC2S_Android : RtmpC2SBase
{
    private NetConnection _net ;
    
    private string m_SID;

    private ConnectHandler m_connecthandler;
    private DisconnectHandler m_disconnecthandler;
    private NetStatusHandler m_netstatushandler;
    
    public override void SetRtmpS2C(IRTMPS2CBase irtmps2c)
    {
        _net.Client = irtmps2c;
    }

    public override void Release()
    {
        _net.OnConnect -= m_connecthandler;
        _net.OnDisconnect -= m_disconnecthandler;
        _net.NetStatus -= m_netstatushandler;
    }
    /*
    public void OnConnect(object obj, System.EventArgs e)
    {
        Debug.Log("RtmpC2S.LoginBySid(5835);");

        Global_HEY.rtmpC2S.LoginBySID("5835");
    }

    public void OnDisConnect(object obj, System.EventArgs e)
    {
        Debug.Log("Server disconnect ... !");
    }

    public void OnNetStatus(object obj, NetStatusEventArgs e)
    {
        Debug.Log("OnNetStatus ... !");
    }
    */
    public override void Initial(string sid, IRTMPS2CBase rtmps2c)
    {
        m_SID = sid;

        if (_net == null)
        {
            //初始化 _net
            Debug.Log("初始化 _net");
            _net = new NetConnection();
        }

        m_connecthandler = new ConnectHandler(rtmps2c.OnConnect);
        m_disconnecthandler = new DisconnectHandler(rtmps2c.OnDisConnect);
        m_netstatushandler = new NetStatusHandler(rtmps2c.OnNetStatus);

        _net.ObjectEncoding = ObjectEncoding.AMF3;
        _net.Client = rtmps2c;

        _net.OnConnect += m_connecthandler;
        _net.OnDisconnect += m_disconnecthandler;
        _net.NetStatus += m_netstatushandler;

    }

    public override void Connect(string sid, string ip)
    {
        try
        {
            Debug.Log("Connect rtmp://" + ip + "/SlotMachine/service.mob");

            _net.Connect("rtmp://" + ip + "/SlotMachine/service.mob");
            //_net.Connect("rtmp://103.252.135.2:23/SlotMachine/service.mob");
        }
        catch (Exception EX)
        {
            Debug.Log("Connect Exception " + EX);
        }
    }

    public override void LoginBySID(string gamecode)
    {
        _net.Call("loginBySid", null, m_SID, gamecode);
    }

    public override void TakeMachine(string setno)
    {
        _net.Call("takeMachine", null, 0);
    }

    public override void onLoadInfo2()
    {
        _net.Call("onLoadInfo2", null);
    }

    public override void CreditExchange(string rate, string score)
    {
        _net.Call("creditExchange", null, rate, score);
    }

    public override void BeginGame(int selectline, int betperline)
    {
        _net.Call("beginGame2", null,
            m_SID,
            selectline.ToString(), betperline.ToString());
    }

    public override void EndGame(string WagersID)
    {
        _net.Call("endGame", null, m_SID, WagersID);
    }

    public override void HitFree(string WagersID, int itemID)
    {
        throw new NotImplementedException();
    }

    public override void HitBonus(int itemID)
    {
        throw new NotImplementedException();
    }

    public override void EndBonus()
    {
        throw new NotImplementedException();
    }

    public override void BalanceExchange()
    {
        _net.Call("balanceExchange", null);
    }

    public override void MachineLeave()
    {
        throw new NotImplementedException();
    }

    public override void Close()
    {
        Debug.Log("Close");
        _net.Close();
    }

}
