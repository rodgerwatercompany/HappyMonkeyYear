public struct LoginInfo_HEY
{
    public string AccountName;
    public string Domain;
    public string SID;
    public string HallID;
    public string IP;


    public LoginInfo_HEY(string _accountname,
        string _domain,
        string _sid,
        string _hallid,
        string _ip)
    {
        AccountName = _accountname;
        Domain = _domain;
        SID = _sid;
        HallID = _hallid;
        IP = _ip;
    }
}

    public struct onloadInfo
    {
        public string balance ;
        public string defaultBase;
        public string loginName;
        public string Base ;

        public onloadInfo(string _balance,string _defaultbase,string _loginname,string _base)
        {
            balance = _balance; defaultBase = _defaultbase; loginName = _loginname; Base = _base;
        }
    }

public static class Global_HEY
{

    // 儲存loading 時呼叫 onloadinfo 回傳的資料
    public static onloadInfo GL_onloadInfo;

    public static LoginInfo_HEY GL_LoginInfo;

    public static RtmpC2SBase rtmpC2S;
}