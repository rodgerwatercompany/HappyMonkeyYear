using FluorineFx.Net;

public interface IRTMPS2CBase
{
    void OnConnect(object obj, System.EventArgs e);
    void OnDisConnect(object obj, System.EventArgs e);
    void OnNetStatus(object obj, NetStatusEventArgs e);
    void onLogin(object obj);
    void onGetMachineList(object obj);
    void onTakeMachine(object obj);
    void onOnLoadInfo2(object obj);
    void onCreditExchange(object obj);
    void onBalanceExchange(object obj);
    void onBeginGame(object obj);
    void onEndGame(object obj);
    void updateJP(object obj);
    void updateJPList(object obj);
    void updateMarquee(object obj);
}
