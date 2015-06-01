public abstract class RtmpC2SBase {
    
    public abstract void SetRtmpS2C(IRTMPS2CBase rtmps2c);

    public abstract void Release();

    public abstract void Initial(string sid, IRTMPS2CBase rtmps2c);

    public abstract void Connect(string sid, string ip);
    public abstract void LoginBySID(string gamecode);
    public abstract void TakeMachine(string setno);
    public abstract void onLoadInfo2();
    public abstract void CreditExchange(string rate, string score);
    public abstract void BeginGame(int selectline, int betperline);
    public abstract void EndGame(string WagersID);
    public abstract void HitFree(string WagersID, int itemID);
    public abstract void HitBonus(int itemID);
    public abstract void EndBonus();
    public abstract void BalanceExchange();
    public abstract void MachineLeave();
    public abstract void Close();
}
