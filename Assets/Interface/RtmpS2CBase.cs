using UnityEngine;
using FluorineFx.Net;

public abstract class RtmpS2CBase  : MonoBehaviour
{
    public abstract void OnConnect(object obj, System.EventArgs e);
    public abstract void OnDisConnect(object obj, System.EventArgs e);
    public abstract void OnNetStatus(object obj, NetStatusEventArgs e);
    public abstract void onLogin(object obj);
    public abstract void onGetMachineList(object obj);
    public abstract void onTakeMachine(object obj);
    public abstract void onOnLoadInfo2(object obj);
    public abstract void onCreditExchange(object obj);
    public abstract void onBalanceExchange(object obj);
    public abstract void onBeginGame(object obj);
    public abstract void onEndGame(object obj);
    public abstract void updateJP(object obj);
    public abstract void updateJPList(object obj);
    public abstract void updateMarquee(object obj);
}