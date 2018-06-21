﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public enum PhaseState
    {
        photoState,
        endState,
        switchState,
        PhotoCheckState,
        attackState,
        waitingState
    }

    const int WEEKBONUS = 10; //弱点部位へのボーナス 

    //それぞれの弱点の確立
    public struct WeekPointProbability
    {
        public int num; //弱点番号
        public int probability; //弱点の確立
    }

    public struct WeekPointData
    {
        public string name; //撮影に使った画像の名前
        public List<WeekPointProbability> datas; //弱点の情報
    }

    private List<WeekPointData> weekDatas;
    public Image m_GameClear;
    public Image m_GameOver;
    public GameObject m_endCam;
    public GameObject m_switchCam;
    public GameObject m_attackP;
    public TextController m_textcontroller;
    public GameObject m_camera;
    GameObject m_player;
    GameObject m_enemy;
    CameraController m_CC;
    public GameObject minimap;
    private int weeknumber; //敵の弱点の数字
    [SerializeField]
    private int weekcount = 0; //弱点の数
    [SerializeField]
    private int mapXsize = 0; //マップのx方向のマスの数

    private bool gameend = false;


    private PhaseState phaseState;

    bool isxrayzero = false; //射影機をすべて使い終わったか
    public int testnum = 0;

    [SerializeField] private AudioSource[] bgms;
    private int nowBGM = 0;

    [SerializeField] private GameObject[] lifeIcons;
    private int oldLife;
    private Soldier playerScript;

    [SerializeField] private float endXpos = 370;

    // Use this for initialization
    void Start () {
        m_attackP.SetActive(false);
        phaseState = PhaseState.photoState;
        //m_camera = Camera.main.gameObject;
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_enemy = GameObject.FindGameObjectWithTag("BigEnemy").transform.root.gameObject;
        //m_CC = m_camera.transform.parent.parent.GetComponent<CameraController>();
        weeknumber = Random.Range(0, weekcount);
        weeknumber = 0; //テスト用
        weekDatas = new List<WeekPointData>();
        m_GameClear.enabled = false;
        m_GameOver.enabled = false;
        playerScript = m_player.GetComponent<Soldier>();
        oldLife = playerScript.GetResidue();
        GameTextController.TextStart(0);
        UnlockManager.AllSet(true);
    }
	
	// Update is called once per frame
	void Update () {
        switch (phaseState)
        {
            case PhaseState.photoState: PhotoState(); break;
            case PhaseState.endState: EndState(); break;
            case PhaseState.switchState: SwitchState(); break;
            case PhaseState.PhotoCheckState: PhotoCheckState(); break;
            case PhaseState.attackState: AttackState(); break;
            case PhaseState.waitingState: WaitState(); break;
        }

        
    }


    private void PhotoState()
    {
        // GameClear();
        //GameOver();
        if (oldLife != playerScript.GetResidue())
        {
            oldLife = playerScript.GetResidue();
            lifeIcons[oldLife].SetActive(false);
        }
        if (playerScript.Annihilation())
        {
            Fade.FadeOut();
            ChengeWait();
            
        }
        PhaseTransition();
    }

    private void EndState()
    {
        if (BigEnemyScripts.mTransform.position.x > (mapXsize - 0) * MainStageDate.TroutLengthX)
        {
            GameTextController.TextStart(7);
            m_endCam.SetActive(false);
            m_switchCam.SetActive(true);
            phaseState = PhaseState.switchState;
        }
    }

    private void SwitchState()
    {
        //if (Fade.IsFadeOutOrIn() && Fade.IsFadeEnd())
        //{
        //    m_player.SetActive(false);
        //}
        if (!Fade.IsFadeOutOrIn() && !Fade.IsFadeEnd())
        {
            //m_attackP.SetActive(true);
            //m_attackP.transform.Find("Camera").GetComponent<AudioListener>().enabled = false;
            SetBGM(1);
            GameTextController.TextStart(8);
        }
        if (m_switchCam == null)
        {
            //m_attackP.transform.Find("Camera").GetComponent<AudioListener>().enabled = true;
            phaseState = PhaseState.PhotoCheckState;
        }
    }

    private void PhotoCheckState()
    {
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetButtonDown("WeaponChange"))
        {
            GameTextController.TextStart(9);
            phaseState = PhaseState.attackState;
        }
        if(BigEnemyScripts.mTransform.position.x >= endXpos)
        {
            ChengeWait();
            BigEnemyScripts.shootingFailure.FailureAction();
        }
    }

    public void AttackState()
    {
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetButtonDown("WeaponChange"))
        {
            phaseState = PhaseState.PhotoCheckState;
        }
        if (BigEnemyScripts.mTransform.position.x >= endXpos)
        {
            ChengeWait();
            BigEnemyScripts.shootingFailure.FailureAction();
            GameTextController.TextStart(11);
        }
    }

    public void WaitState()
    {
        if (gameend)
        {
            if (Input.anyKeyDown)
            {
                ScecnManager.SceneChange("GameStart1");
            }

        }
        else
        {
            if (BigEnemyScripts.breakEffectManager.isEnd) GameClear();
            if (Fade.IsFadeOutOrIn() && Fade.IsFadeEnd()) GameOver();
        }
        
    }


    public void GameClear()
    {
        //m_textcontroller.SetNextText(0, 0, true);
        GameTextController.TextStart(10);
        m_GameClear.enabled = true;
        Time.timeScale = 1;
        //Destroy(m_enemy);
        m_enemy.SetActive(false);
        SetBGM(2);
        //Time.timeScale = 0;
        //Debug.Log("ゲームクリア");
        gameend = true;
    }

    public void GameOver()
    {
        //m_textcontroller.SetNextText(0, 0, true);
        m_GameOver.enabled = true;
        SetBGM(3);
        //Time.timeScale = 0;
        // Debug.Log("ゲームオーバー");
        gameend = true;
    }

    void PhaseTransition()
    {
        if (isxrayzero||BigEnemyScripts.mTransform.position.x > (mapXsize - 0) * MainStageDate.TroutLengthX)
        {
            GameTextController.TextStart(15);
            m_player.SetActive(false);
            //Camera.main.gameObject.SetActive(false);
            m_camera.SetActive(false);
            PlDes();

            Vector3 enemyPos = BigEnemyScripts.mTransform.position;
            enemyPos.x = (mapXsize * MainStageDate.TroutLengthX) - (0.5f * MainStageDate.TroutLengthX);
            BigEnemyScripts.mTransform.position = enemyPos;

            minimap.SetActive(false);
            lifeIcons[0].transform.parent.gameObject.SetActive(false);
            m_endCam.SetActive(true);
            UnlockManager.AllSet(false);
            phaseState = PhaseState.endState;
        }
    }
    public void ChengeWait()
    {
        BigEnemyScripts.shootingPhaseMove.moveSpeed = 0;
        phaseState = PhaseState.waitingState;
    }

    public void XrayZero()
    {
        isxrayzero = true;
    }

    /// <summary>弱点データの保存</summary>
    public void SetWeekPhoto(string texname,List<int> weeknums)
    {
        WeekPointData result;
        result.name = texname;
        List<WeekPointProbability> list = new List<WeekPointProbability>();
        List<int> probabilitys = XrayProbability(weeknums);
        for (int i = 0; i < weeknums.Count; i++)
        {
            WeekPointProbability a;
            a.num = weeknums[i];
            a.probability = probabilitys[i];
            Debug.Log("弱点「" + a.num + "」の確立は" + a.probability + "％");
            list.Add(a);
        }
        result.datas = list;

        weekDatas.Add(result);
    }

    //弱点の確立の割り振り
    private List<int> XrayProbability(List<int> weeknums)
    {
        List<int> result = new List<int>();
        int count = 100;  //合計で100%になるように
        for (int i = 0; i < weeknums.Count; i++)
        {
            int probability = 0;
            if (weeknums[i] == weeknumber)
            {
                probability = WEEKBONUS;  //弱点部位にボーナス分％をプラス
                count -= WEEKBONUS; //そのぶん回す数減らす
            }
            result.Add(probability);
        }

        while(count > 0) //合計100%になるまで
        {
            count--;
            int plus = Random.Range(0, weeknums.Count);
            result[plus]++;
            //Debug.Log("今日は" + weeknums[plus]);
        }

        return result;
    }

    public List<WeekPointData> GetWeekPointData
    {
        get { return weekDatas; }
    }

    public bool PhotoStateNow()
    {
        return phaseState == PhaseState.photoState;
    }

    public bool PhotoCheckStateNow()
    {
        return phaseState == PhaseState.PhotoCheckState;
    }

    public bool AttackStateNow()
    {
        return phaseState == PhaseState.attackState;
    }

    public PhaseState NowState()
    {
        return phaseState;
    }

    public void PlDes()
    {
        bool test = false;
        if (!test)
        {
            BigEnemyScripts.shootingPhaseMove.ShootingPhaseSet();
            test = true;
        }
    }

    public void Damege(int i)
    {
        if (weeknumber == i)
        {
            BigEnemyScripts.breakEffectManager.ChangeType();
            //m_attackP.GetComponent<AttackPlayer>().ClearCamera();
        }
        else
        {
            BigEnemyScripts.shootingFailure.FailureAction();
            GameTextController.TextStart(11);
        }
    }

    private void SetBGM(int n)
    {
        if (nowBGM == n) return;
        foreach(var bgm in bgms)
        {
            bgm.enabled = false;
        }
        bgms[n].enabled = true;
        nowBGM = n;
    }
}
