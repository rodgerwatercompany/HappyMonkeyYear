using UnityEngine;

using LitJson;
using FluorineFx.Net;
using System;

public class LoadingManager : MonoBehaviour, IRTMPS2CBase
{

    public UIProgressBar progressbar;

    

    private bool sw_loadingGame;

    float progress_now;

    object m_object_onNetStatus;
    bool m_bool_onNetStatus;
    

    // Use this for initialization
    void Start () {
        try
        {
            m_bool_onNetStatus = false;

            progressbar.value = 0.0f;
            sw_loadingGame = false;

            progress_now = 0.0f;

            string tsid = Global_HEY.GL_LoginInfo.SID;
#if UNITY_ANDROID
            Global_HEY.rtmpC2S = new RTMPC2S_Android();
            Global_HEY.rtmpC2S.Initial(tsid, this);
           // Global_HEY.rtmpC2S.SetRtmpS2C(null);
#elif UNITY_IOS || UNITY_IPHONE
#endif

            this.ConnectRtmp();
        }
        catch(Exception EX)
        {
            Debug.Log("Exception " + EX);
        }
    }
	
	// Update is called once per frame
	void Update () {
	    
        if(m_bool_onNetStatus)
        {
            m_bool_onNetStatus = false;
            //string str_json = JsonMapper.ToJson(m_object_onNetStatus);

            Debug.Log("OnNetStatus " + m_object_onNetStatus.ToString());
            m_object_onNetStatus = null;
        }

        if(progressbar.value < progress_now)
        {
            progressbar.value += 0.2f * Time.deltaTime;

            if (progressbar.value >= 1.0f)
                Application.LoadLevel("Game_HEY");
        }
	}

    void ConnectRtmp()
    {
        string tsid = Global_HEY.GL_LoginInfo.SID;
        string tip = Global_HEY.GL_LoginInfo.IP;
        
        Global_HEY.rtmpC2S.Connect(tsid,tip);
    }

    public void OnConnect(object obj, EventArgs e)
    {
        try
        {
            progress_now = 0.20f;
            Debug.Log("RtmpC2S.LoginBySid(5835);");

            Global_HEY.rtmpC2S.LoginBySID("5835");
        }
        catch (Exception EX)
        {
            Debug.Log("IS2CBase.OnConnect Exception " + EX);
        }
    }

    public void OnDisConnect(object obj, EventArgs e)
    {
        Debug.Log("Server disconnect ... !");
    }

    public void OnNetStatus(object obj, NetStatusEventArgs e)
    {
        m_object_onNetStatus = obj;
        m_bool_onNetStatus = true;
    }

    public void onLogin(object obj)
    {
        progress_now = 0.40f;
        string str_json = JsonMapper.ToJson(obj);
        JsonData jd = JsonMapper.ToObject(str_json);
        Debug.Log("Login Success ! " + str_json);
    }

    public void onGetMachineList(object obj)
    {
        progress_now = 0.60f;
        Global_HEY.rtmpC2S.TakeMachine(null);
    }

    public void onTakeMachine(object obj)
    {
        Debug.Log("OnTakeMachine");
        progress_now = 0.80f;
        Global_HEY.rtmpC2S.onLoadInfo2();
    }

    public void onOnLoadInfo2(object obj)
    {
        try
        {
            string str_json = JsonMapper.ToJson(obj);
            int offset = str_json.IndexOf('{');

            string event_name = str_json.Substring(0, offset);

            string event_msg = str_json.Substring(offset);

            JsonData jd = JsonMapper.ToObject(event_msg);


            Debug.Log("onOnLoadInfo2 " + str_json);

            string str_balance = (jd["data"]["Balance"]).ToString();
            string str_dbase = (jd["data"]["DefaultBase"]).ToString();
            string str_loginname = (jd["data"]["LoginName"]).ToString();
            string str_base = (jd["data"]["Base"]).ToString();

            // 儲存資料
            Global_HEY.GL_onloadInfo = new onloadInfo(str_balance, str_dbase, str_loginname, str_base);

            progress_now = 1.0f;
        }
        catch (Exception EX)
        {
            Debug.Log("onOnLoadInfo2 Exception " + EX);
        }
    }

    public void onCreditExchange(object obj)
    {
        Debug.Log("LoadingManager onCreditExchange Exception ");
    }

    public void onBalanceExchange(object obj)
    {
        Debug.Log("LoadingManager onBalanceExchange Exception ");
    }

    public void onBeginGame(object obj)
    {
        Debug.Log("LoadingManager onBeginGame Exception ");
    }

    public void onEndGame(object obj)
    {
        Debug.Log("LoadingManager onEndGame Exception ");
    }

    public void updateJP(object obj)
    {
        Debug.Log("LoadingManager updateJP Exception ");
    }

    public void updateJPList(object obj)
    {
        Debug.Log("LoadingManager updateJPList Exception ");
    }

    public void updateMarquee(object obj)
    {
        Debug.Log("LoadingManager updateMarquee Exception ");
    }
}
