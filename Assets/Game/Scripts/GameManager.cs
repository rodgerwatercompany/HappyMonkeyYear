using UnityEngine;

using System;
using System.Collections;
using LitJson;
using FluorineFx.Net;

public struct Info_GameApplication
{
    public bool sw_autorun;
    public bool sw_freegame_waitslot;
    public bool sw_freegame_waitgui;
    public int idx_freegame;
    public int credit_endgame;

    /*  0 : Normal
        1 : Autospin
        2 : FreeGame    */
    public SM_State f_sm_state;

    public Info_GameApplication(bool pa1, SM_State state)
    {
        sw_autorun = pa1;
        f_sm_state = state;
        sw_freegame_waitslot = false;
        sw_freegame_waitgui = false;
        idx_freegame = 0;
        credit_endgame = 0;
    }
}

public struct ButtonAllowTable
{
    public bool JackPot;
    public bool Exchange;
    public bool Settings;
    public bool Maxbet;
    public bool Dollar;

    public ButtonAllowTable(bool a,bool b,bool c,bool d,bool e)
    {
        JackPot = a;
        Exchange = b;
        Settings = c;
        Maxbet = d;
        Dollar = e;
    }
}

public enum SM_State
{
    NORMAL = 0,
    AUTOSPIN = 1,
    FREEGAME = 2
}

public struct State_S2C
{
    public bool OnDisConnect;

    public bool onCreditExchange;

    public bool onBalanceExchange;

    public bool onBeginGame;

    public bool onEndGame;
}

public class GameManager :MonoBehaviour, IRTMPS2CBase,  IExchange2GM , ISlotMachine2GM , IGUIManager2GM , IBetWheel2GM ,ISetting2GM
    , IWinFreeGame2GM
{
    public UIPanel Win_SystemMessage;

    public ExchangePanel exchangePanel;

    public GUIManager guiManager;

    public SlotMachine slotmachine;

    public SoundManager_HEY soundMgr;
    public MusicManager_HEY musicMgr;

    public static GameManager Instance;

    Info_GameApplication m_GameAppInfo;
    ButtonAllowTable m_but_allowInfo;

    //JsonData m_jd_OnDisConnect;
    JsonData m_jd_onCreditExchange;
    JsonData m_jd_onBalanceExchange;
    JsonData m_jd_onBegingame;
    JsonData m_jd_onEndGame;


    object m_object_OnDisConnect;

    State_S2C m_State_rtmpS2C;
    
    void Awake()
    {
        print("Game Awake");
        string tsid = Global_HEY.GL_LoginInfo.SID;
        Global_HEY.rtmpC2S.Release();
        Global_HEY.rtmpC2S.Initial(tsid, this);
        Global_HEY.rtmpC2S.SetRtmpS2C(this);

        LogServer.Instance.print("Screen.width : " + Screen.width + " Screen.height : " + Screen.height);
        Instance = this;

        soundMgr.SetVolume(1.0f);
        musicMgr.SetVolume(1.0f);
        musicMgr.Play(0);
    }

    // Use this for initialization
    void Start () {

        m_GameAppInfo = new Info_GameApplication(false , SM_State.NORMAL);

        m_but_allowInfo = new ButtonAllowTable(false, false, false, false, false);

        m_but_allowInfo.Maxbet = true;
        m_but_allowInfo.Exchange = true;
        m_but_allowInfo.Settings = true;
        m_but_allowInfo.Dollar = true;

        Win_SystemMessage.alpha = 0.0f;

        Init();

        m_State_rtmpS2C = new State_S2C();
    }	

    void Update()
    {

        if (m_State_rtmpS2C.OnDisConnect)
        {
            m_State_rtmpS2C.OnDisConnect = false;

            try
            {
                string str_json = JsonMapper.ToJson(m_object_OnDisConnect);
                m_object_OnDisConnect = null;
                int offset = str_json.IndexOf('{');

                string event_name = str_json.Substring(0, offset);

                string event_msg = str_json.Substring(offset);

                //JsonData jd = JsonMapper.ToObject(event_msg);

                print("OnDisConnect ..." + str_json);
                /*
                if ((bool)m_jd_OnDisConnect["OnClose"])
                {
                    string domain = Global_HEY.GL_LoginInfo.Domain;
                    string accountname = Global_HEY.GL_LoginInfo.AccountName;

                    string url = "http://" + domain + "/app/WebService/view/display.php/Logout?username=" + accountname;

                    StartCoroutine(DoLoginout(url));
                }*/
            }
            catch (Exception EX)
            {
                LogServer.Instance.print("OnClose Exception " + EX);
            }
        }
        else if (m_State_rtmpS2C.onCreditExchange)
        {
            m_State_rtmpS2C.onCreditExchange = false;

            try
            {

                if ((bool)m_jd_onCreditExchange["event"])
                {
                    string balance = (m_jd_onCreditExchange["data"]["Balance"]).ToString();
                    string betBase = (m_jd_onCreditExchange["data"]["BetBase"]).ToString();

                    string credit = (m_jd_onCreditExchange["data"]["Credit"]).ToString();

                    string[] get_Int = credit.Split('.');

                    string str_credit_int = (get_Int[0]).ToString();
                    exchangePanel.OnCreditExchange(balance, betBase, str_credit_int);

                    int score_now = 0;
                    int score_bet = guiManager.GetBetScore();

                    // 設定玩家可用分數
                    score_now = Convert.ToInt32(get_Int[0]);

                    bool allowspin = false;
                    if (score_bet > 0 && score_now > 0)
                        allowspin = true;

                    // 恢復某些按鈕，設定Display可用分數
                    guiManager.OnCreateExchange(betBase, allowspin, get_Int[0]);
                }
                else
                {
                    LogServer.Instance.print("onCreditExchange event : false");
                }
            }
            catch (Exception EX)
            {
                Debug.Log("onCreditExchange Exception " + EX);
                LogServer.Instance.print("onCreditExchange Exception " + EX);
            }
        }
        else if (m_State_rtmpS2C.onBalanceExchange)
        {
            m_State_rtmpS2C.onBalanceExchange = false;

            try
            {
                guiManager.OnBalanceExchange();

                string transcredit = (m_jd_onBalanceExchange["data"]["TransCredit"]).ToString();
                string amount = (m_jd_onBalanceExchange["data"]["Amount"]).ToString();
                string balance = (m_jd_onBalanceExchange["data"]["Balance"]).ToString();

                exchangePanel.OnBalanceExchange(transcredit, amount, balance);
            }
            catch (Exception EX)
            {
                LogServer.Instance.print("onBalanceExchange Exception " + EX);
            }

        }
        else if (m_State_rtmpS2C.onBeginGame)
        {
            m_State_rtmpS2C.onBeginGame = false;

            try
            {

                print("onBeginGame 2 ");

                if ((bool)m_jd_onBegingame["event"])
                {
                    string WagersID = (m_jd_onBegingame["data"]["WagersID"]).ToString();

                    Global_HEY.rtmpC2S.EndGame(WagersID);
                }
                else
                {
                    // 錯誤訊息
                    print("錯誤訊息");
                }
            }
            catch (Exception EX)
            {
                print("onBeginGame Exception " + EX);
                LogServer.Instance.print("onBeginGame Exception " + EX);
            }
        }
        else if (m_State_rtmpS2C.onEndGame)
        {
            m_State_rtmpS2C.onEndGame = false;

            try
            {
                if (!(bool)m_jd_onEndGame["event"])
                {
                    // 錯誤資訊
                    print("錯誤訊息");
                }
                else
                {
                    // 剖析 Cards 欄位
                    string cards = (m_jd_onBegingame["data"]["Cards"]).ToString();
                    string[] tileinfo = cards.Split(',', '-');

                    string str_show = "";
                    for (int i = 0; i < tileinfo.Length; i++)
                    {
                        int num = Convert.ToInt32(tileinfo[i]);
                        tileinfo[i] = num.ToString("000");
                        str_show += tileinfo[i] + " ";
                    }
                    LogServer.Instance.print("[Debug] tileinfo " + str_show);

                    // 將資料塞入拉霸機
                    slotmachine.SetTileSpriteInfo(tileinfo);

                    // 依拉霸機的狀態選擇下一個按鈕的種類
                    if (m_GameAppInfo.f_sm_state == SM_State.AUTOSPIN)
                    {
                        slotmachine.OnClick_StartStop_Immediate();
                        // 顯示停止自動轉的按鍵
                        guiManager.AllowAutoStop();
                    }
                    else
                    {
                        slotmachine.OnClick_StartStop();

                        // 顯示 停止鍵
                        guiManager.AllowStop();
                    }

                    string[] values = (m_jd_onEndGame["data"]["Credit"]).ToString().Split('.');

                    m_GameAppInfo.credit_endgame = Convert.ToInt32(values[0]);
                }
            }
            catch (Exception EX)
            {
                print("onEndGame Exception " + EX);
                LogServer.Instance.print("onEndGame Exception " + EX);
            }
        }
    }

    void Init()
    {
        
        // 初始化開分面板
        exchangePanel.Setup(Global_HEY.GL_onloadInfo.balance,
            Global_HEY.GL_onloadInfo.defaultBase,
            Global_HEY.GL_onloadInfo.loginName,
            Global_HEY.GL_onloadInfo.Base);
            
        //rtmps2c.IRtmpS2C = this;
    }

    #region IExchange2GM
    bool IExchange2GM.OpenAllow()
    {
        if (m_but_allowInfo.Exchange)
            return true;
        else
        {
            //guiManager.
            return false;
        }
    }

    void IExchange2GM.CreateExchange(string ratio, int score)
    {
        // 關閉開分按鈕，直到開分結束恢復。
        guiManager.OnWaitCreateExchange();

        Global_HEY.rtmpC2S.CreditExchange(ratio, score.ToString());
    }

    void IExchange2GM.BalanceExchange(bool needclosegui)
    {
        if (needclosegui)
        {
            // 關閉開分按鈕
            guiManager.OnWaitCreateExchange();
        }

        Global_HEY.rtmpC2S.BalanceExchange();
    }

    void IExchange2GM.CashoutQuit()
    {
        string domain = Global_HEY.GL_LoginInfo.Domain;
        string accountname = Global_HEY.GL_LoginInfo.AccountName;

        string url = "http://" + domain + "/app/WebService/view/display.php/Logout?username=" + accountname;

        Global_HEY.rtmpC2S.Close ();
        StartCoroutine(DoLoginout(url));
    }
    // 登出 SID
    IEnumerator DoLoginout(string url)
    {
        using (WWW www = new WWW(url))
        {

            yield return www;

            if(!string.IsNullOrEmpty(www.error))
            {
                LogServer.Instance.print(www.error);
            }
            else
            {
                LogServer.Instance.print(www.text);

                Application.LoadLevel("Login_HEY");
            }
        }
    }

    #endregion

    #region ISlotMachine2GM
    void ISlotMachine2GM.OnClick_Spin()
    {
        // 關閉不允許Spin開啟的按鍵
        m_but_allowInfo.Maxbet = false;
        m_but_allowInfo.Exchange = false;
        m_but_allowInfo.Dollar = false;

        Spin();
    }

    void Spin()
    {
        int score_now = guiManager.GetNowScore();
        int betscore = guiManager.GetBetScore();

        if (score_now >= betscore)
        {
            // 計算可用分數
            score_now -= betscore;

            // Disable 4組按鍵 ， 更改可用分數顯示
            guiManager.OnClick_Spin(score_now);

            Global_HEY.rtmpC2S.BeginGame( 50, betscore/50);

            slotmachine.StartSpin();
        }
        else
        {
            if (m_GameAppInfo.f_sm_state == SM_State.AUTOSPIN)
            {
                // 結束自動轉
                m_GameAppInfo.f_sm_state = SM_State.NORMAL;
            }

            // 可用分數不足，跳出通知訊息。
            string context = "";
            string language_id = Localization.language;
            if (language_id == "TW")
                context = tw_ErrorMsg[0];
            else if (language_id == "CN")
                context = cn_ErrorMsg[0];
            else
                context = en_ErrorMsg[0];

            guiManager.ShowWindowMsg(context);

            // 復原按鍵
            guiManager.AllowSpin();

            m_but_allowInfo.Maxbet = true;
            m_but_allowInfo.Exchange = true;
            m_but_allowInfo.Dollar = true;
        }
    }
    
    void ISlotMachine2GM.OnClick_AutoSpin()
    {
        m_GameAppInfo.f_sm_state = SM_State.AUTOSPIN;

        m_GameAppInfo.sw_autorun = true;
        Spin();
    }

    void ISlotMachine2GM.OnClick_StopAutoSpin()
    {
        if(m_GameAppInfo.f_sm_state == SM_State.AUTOSPIN)
            m_GameAppInfo.f_sm_state = SM_State.NORMAL;

        m_GameAppInfo.sw_autorun = false;
    }

    void ISlotMachine2GM.OnClick_GetScore()
    {
        // 關閉動畫、結束流程。
        guiManager.OnClick_GetScore();
    }

    // slotmachine totally stop.
    void ISlotMachine2GM.OnStop()
    {
        if (m_GameAppInfo.f_sm_state == SM_State.FREEGAME)
        {
            m_GameAppInfo.sw_freegame_waitslot = false;
        }
        else
        {
            // 贏分
            bool b_win = false;
            bool b_scatter = false;
            int cnt_line = m_jd_onBegingame["data"]["Lines"].Count;
            if (cnt_line > 0)
                b_win = true;
            if (m_jd_onBegingame["data"]["Scatter"].IsObject)
            {
                b_scatter = true;
                LogServer.Instance.print("ISlotMachine2GM.OnStop b_scatter is true");
            }

            guiManager.OnStop(b_win, b_scatter, m_GameAppInfo.f_sm_state, m_jd_onBegingame["data"]);
        } 
    }

    #endregion

    #region IBetWheel2GM
    bool IBetWheel2GM.MaxBetAllow()
    {
        if (m_but_allowInfo.Maxbet)
            return true;
        else
        {
            return false;
        }
    }

    void IBetWheel2GM.UpdateBetValue(int betvalue)
    {
        int score_now = guiManager.GetNowScore();

        guiManager.UpdateBetValue(betvalue);

        if (score_now > 0)
            guiManager.AllowSpin();
    }
    #endregion

    void IGUIManager2GM.Finish_OnStop()
    {
        try
        {

            bool sw_freegame = false;
            if (m_GameAppInfo.f_sm_state != SM_State.FREEGAME)
            {
                // 檢查本次Spin有無免費遊戲
                if (m_jd_onBegingame["data"]["FreeGame"].IsObject)
                {
                    sw_freegame = true;
                    m_GameAppInfo.f_sm_state = SM_State.FREEGAME;
                }
            }

            // 如果有免費遊戲
            if (sw_freegame)
            {
                sw_freegame = false;
                // 通知玩家獲得免費遊戲以及次數
                guiManager.ShowGetFreeGame(m_jd_onBegingame["data"]["FreeGame"]["BonusInfo"].Count);

                LogServer.Instance.print("Finish_OnStop sw_freegame is true");
            }
            else
            {

                if (m_GameAppInfo.f_sm_state == SM_State.FREEGAME)
                {
                    double dou = (double)m_jd_onBegingame["data"]["FreeGame"]["BonusInfo"][m_GameAppInfo.idx_freegame]["EndCredit"];
                    int EndCredit = Convert.ToInt32(dou);
                    // 更新可用分數
                    exchangePanel.OnChangeNowScore(EndCredit);

                    m_GameAppInfo.sw_freegame_waitgui = false;
                }
                else if (m_GameAppInfo.f_sm_state == SM_State.AUTOSPIN)
                {
                    // 更新可用分數
                    exchangePanel.OnChangeNowScore(m_GameAppInfo.credit_endgame);

                    Spin();
                }
                else if (m_GameAppInfo.f_sm_state == SM_State.NORMAL)
                {
                    // 更新可用分數
                    exchangePanel.OnChangeNowScore(m_GameAppInfo.credit_endgame);

                    guiManager.AllowSpin();

                    m_but_allowInfo.Maxbet = true;
                    m_but_allowInfo.Exchange = true;
                    m_but_allowInfo.Dollar = true;

                }
            }
        }
        catch(Exception EX)
        {
            LogServer.Instance.print("Finish_OnStop Exception " + EX);
        }
    }    

    bool ISetting2GM.OpenAllow()
    {
        return m_but_allowInfo.Settings;
    }

    void IWinFreeGame2GM.OnClick_CloseWinFreeGame()
    {
        musicMgr.Play(3);
        // 執行免費遊戲
        StartCoroutine(FreeGame_Spin());
    }
    
    IEnumerator FreeGame_Spin()
    {
        int cnt_freegame = m_jd_onBegingame["data"]["FreeGame"]["BonusInfo"].Count;

        LogServer.Instance.print("cnt_freegame " + cnt_freegame);

        m_GameAppInfo.idx_freegame = 0;

        do
        {
            yield return new WaitForSeconds(1.0f);

            slotmachine.StartSpin();

            yield return new WaitForSeconds(1.0f);
            
            // 剖析 Cards 欄位
            string cards = (m_jd_onBegingame["data"]["FreeGame"]["BonusInfo"][m_GameAppInfo.idx_freegame]["Cards"]).ToString();
            string[] tileinfo = cards.Split(',', '-');

            string str_show = "";
            for (int i = 0; i < tileinfo.Length; i++)
            {
                int num = Convert.ToInt32(tileinfo[i]);
                tileinfo[i] = num.ToString("000");
                str_show += tileinfo[i] + " ";
            }
            LogServer.Instance.print("[Debug] tileinfo [" + m_GameAppInfo.idx_freegame + "] " + str_show);

            // 將資料塞入拉霸機
            slotmachine.SetTileSpriteInfo(tileinfo);

            slotmachine.OnClick_StartStop_Immediate();

            m_GameAppInfo.sw_freegame_waitslot = true;
            while(m_GameAppInfo.sw_freegame_waitslot)
            {
                yield return new WaitForEndOfFrame();
            }

            m_GameAppInfo.sw_freegame_waitgui = true;
            guiManager.OnStop_FreeGameSpinStop(m_jd_onBegingame["data"]["FreeGame"]["BonusInfo"][m_GameAppInfo.idx_freegame]);
            
            while (m_GameAppInfo.sw_freegame_waitgui)
            {
                yield return new WaitForEndOfFrame();
            }


            cnt_freegame--;
            m_GameAppInfo.idx_freegame++;            
        }
        while (cnt_freegame > 0);

        if(m_GameAppInfo.sw_autorun)
        {
            m_GameAppInfo.f_sm_state = SM_State.AUTOSPIN;
            Spin();
        }
        else
        {

            m_GameAppInfo.f_sm_state = SM_State.NORMAL;

            guiManager.AllowSpin();

            m_but_allowInfo.Maxbet = true;
            m_but_allowInfo.Exchange = true;
            m_but_allowInfo.Dollar = true;
        }


        musicMgr.Play(0);
    }

    void ISetting2GM.Logout()
    {
        string domain = Global_HEY.GL_LoginInfo.Domain;
        string accountname = Global_HEY.GL_LoginInfo.AccountName;

        string url = "http://" + domain + "/app/WebService/view/display.php/Logout?username=" + accountname;

        Global_HEY.rtmpC2S.Close();
        StartCoroutine(DoLoginout(url));
    }

    #region S2C implement

    public void OnDisConnect(object obj, EventArgs e)
    {
        try
        {
            print("OnDisConnect ...");

            m_object_OnDisConnect = obj;

            m_State_rtmpS2C.OnDisConnect = true;
        }
        catch (Exception EX)
        {
            LogServer.Instance.print("OnDisConnect Exception " + EX);
        }
    }

    public void onCreditExchange(object obj)
    {
        try
        {

            string str_json = JsonMapper.ToJson(obj);
            int offset = str_json.IndexOf('{');
            string event_name = str_json.Substring(0, offset);
            string event_msg = str_json.Substring(offset);
            m_jd_onCreditExchange = JsonMapper.ToObject(event_msg);

            m_State_rtmpS2C.onCreditExchange = true;
        }
        catch (Exception EX)
        {
            print("onCreditExchange Exception " + EX);
            LogServer.Instance.print("onCreditExchange Exception " + EX);
        }
    }

    public void onBalanceExchange(object obj)
    {
        try
        {
            
            string str_json = JsonMapper.ToJson(obj);
            int offset = str_json.IndexOf('{');
            string event_name = str_json.Substring(0, offset);
            string event_msg = str_json.Substring(offset);
            m_jd_onBalanceExchange = JsonMapper.ToObject(event_msg);

            m_State_rtmpS2C.onBalanceExchange = true;            
        }
        catch (Exception EX)
        {
            print("onBalanceExchange Exception " + EX);
            LogServer.Instance.print("onBalanceExchange Exception " + EX);
        }
    }

    public void onBeginGame(object obj)
    {
        try
        {
            print("onBeginGame ");

            string str_json = JsonMapper.ToJson(obj);
            int offset = str_json.IndexOf('{');
            string event_name = str_json.Substring(0, offset);
            string event_msg = str_json.Substring(offset);
            m_jd_onBegingame = JsonMapper.ToObject(event_msg);

            print("onBeginGame str_json " + str_json);
            LogServer.Instance.print("onBeginGame " + str_json);

            m_State_rtmpS2C.onBeginGame = true;
        }
        catch(Exception EX)
        {
            print("onBeginGame Exception " + EX);
            LogServer.Instance.print("onBeginGame Exception " + EX);
        }
    }

    public void onEndGame(object obj)
    {
        try
        {
            // 緩存資料
            string str_json = JsonMapper.ToJson(obj);
            int offset = str_json.IndexOf('{');
            string event_name = str_json.Substring(0, offset);
            string event_msg = str_json.Substring(offset);
            m_jd_onEndGame = JsonMapper.ToObject(event_msg);


            print("onEndGame str_json " + str_json);
            LogServer.Instance.print("onEndGame " + str_json);

            m_State_rtmpS2C.onEndGame = true;
        }
        catch (Exception EX)
        {
            print("onEndGame Exception " + EX);
            LogServer.Instance.print("onCreditExchange Exception " + EX);
        }
    }

    public void OnNetStatus(object obj, NetStatusEventArgs e)
    {
        print("OnNetStatus.");
    }
    public void OnConnect(object obj, EventArgs e)
    {
    }
    public void onLogin(object obj)
    {
    }
    public void onGetMachineList(object obj)
    {
    }
    public void onTakeMachine(object obj)
    {
    }
    public void onOnLoadInfo2(object obj)
    {
    }    
    public void updateJP(object obj)
    {
    }
    public void updateJPList(object obj)
    {
    }
    public void updateMarquee(object obj)
    {
    }
    #endregion

    #region Table
    private string[] en_ErrorMsg =
    {
        "NOT_ENOUGH_CREDIT",
        "DISCONNECT",
        "USER_IS_NOT_EXIST",
        "SID_IS_NOT_EXIST",
        "MACHINE_WAS_OCCUPIED",
        "CURRENCY_IS_NOT_EXIST",
        "MACHINE_IS_NOT_EXIST",
        "BETBASE_IS_ILLEGAL",
        "CREDIT_IS_ILLEGAL",
        "BALANCE_IS_NOT_ENOUGH",
        "UPDATE_BALANCE_FAILED",
        "END_WAGERS_FAILED",
        "UPDATE_CREDIT_FAILED",
        "NOT_ENOUGH_BALANCE",
        "MACHINE_IS_EMPTY",
        "MACHINE_LEAVE_FAILED",
        "DEAL_REPEATED",
        "MACHINE_IS_UNAVAILABLE",
        "FREEGAME_IS_NOT_EXIST",
        "FREEGAME_IS_ERROR",
        "WAGERS_IS_NOT_EXIST",
        "CONTENT_IS_NOT_EXIST",
        "CONTENT_UPDATE_ERROR",
        "WAGERS_AGINFO_ERROR",
        "RENT_POINT_IS_NOT_ENOUGH",
        "WAGERS_UPDATE_ERROR",
        "WAGERS_WAS_ENDED",
        "Please re-enter the game.",
        "Internet disconnection,please restart the game or check the connection status",
        "Load BonusGame resource fail",
        "Not Enough rent point.",             //30
    };

    private string[] tw_ErrorMsg =
    {
        "餘額不足",             //0
        "未成功連上Server",     //1
        "會員帳號有誤",         //2   
        "SID不存在",            //3 
        "機台已被佔",           //4
        "幣別有誤",             //5 
        "機台不存在",           //6 
        "基注有誤",             //7 
        "分數不足",             //8 
        "額度不足",             //9
        "更新額度失敗",         //10
        "結單失敗",            //11
        "更新分數失敗",        //12 
        "額度不足更換分數",    //13 
        "機台為空",           //14 
        "離開機台失敗",       //15
        "重複下單",           //16 
        "機台鎖定中",         //17 
        "免費遊戲不存在",     //18
        "免費遊戲資料有誤",   //19
        "注單不存在",        //20
        "內容不存在",        //21
        "內容更新有誤",      //22
        "注單體系資料錯誤",  //23 
        "租卡點數不足",     //24
        "注單更新失敗",     //25
        "遊戲已得分",      //26 
        "請重新登入遊戲", //27
        "網路斷線,請重新開啟遊戲或檢查連線狀態", //28
        "載入BonusGame資源包失敗",              //29
        "租卡餘額不足",                        //30

    };

    private string[] cn_ErrorMsg =
    {
        "余额不足",
        "未成功连上Server",
        "会员帐号有误",
        "SID不存在",
        "机台已被佔",
        "币别有误",
        "机台不存在",
        "基注有误",
        "分数不足",
        "额度不足",
        "更新额度失败",
        "结单失败",
        "更新分数失败",
        "额度不足更换分数",
        "机台为空",
        "离开机台失败",
        "重複下单",
        "机台锁定中",
        "免费游戏不存在",
        "免费游戏资料有误",
        "注单不存在",
        "内容不存在",
        "内容更新有误",
        "注单体系资料错误",
        "租卡点数不足",
        "注单更新失败",
        "游戏已得分",
        "请重新登入游戏",
        "网路断线,请重新开启游戏或检查连线状态",
        "载入BonusGame资源包失败",
        "租卡余额不足",                 //30

    };
    #endregion
}