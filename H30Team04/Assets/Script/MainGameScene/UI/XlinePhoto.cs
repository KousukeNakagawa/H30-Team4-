﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XlinePhoto : MonoBehaviour {
    [SerializeField]
    private GameObject[]m_lifes;
    [SerializeField]
    private Image[] m_wepons;
    public GameObject gm;
    GameObject XPhots;
    GameObject m_Sight;
    GameManager m_gamemanager;
    //GameObject Arrows;
    public int m_FlyerCount;
    bool m_IsChange;
    private int currentSelectStageIndex;
    AsyncOperation async;
    bool keyflag_;

    GameObject work_image;

    public GameObject photoPrefab;
    private List<GameManager.WeekPointData> xrayDatas;
    private GameObject weektextparent;
    private WeekTextManager weektexts;
    int _lifecount = 3;   //残機

    public bool test = false;

    // Use this for initialization
    void Start () {
        m_gamemanager = gm.GetComponent<GameManager>();
        keyflag_ = false;
        currentSelectStageIndex = 0;
        m_FlyerCount = 0;
        XPhots = transform.Find("XPhotos").gameObject;
        xrayDatas = new List<GameManager.WeekPointData>();
        weektextparent = transform.Find("Probabilitys").gameObject;
        weektexts = weektextparent.GetComponent<WeekTextManager>();
        //for (int i = 0; i < XPhots.transform.childCount; ++i)
        //{
        //    if (XPhots.transform.GetChild(i).name.Split('_')[0] == "XPhoto")
        //    {
        //        ++m_FlyerCount;
        //    }
        //}
        m_Sight = transform.Find("sight").gameObject;
        XPhots.SetActive(false);
        m_Sight.SetActive(false);
        weektextparent.SetActive(false);


    }
	
	// Update is called once per frame
	void Update () {
        UpdateSelect();
        ViewPhotos();
        WeponChoiceNow();
        if (Input.GetMouseButtonDown(0))
        {
            Life();
        }
	}

    void WeponChoiceNow()
    {
        if (!test)
        {
            m_wepons[1].transform.SetAsFirstSibling();
            m_wepons[0].color = new Color(1, 0, 0, 1);
            m_wepons[1].color = new Color(1, 1, 1, 0.5f);
        }
        else {
            m_wepons[0].transform.SetAsFirstSibling();
            m_wepons[1].color = new Color(1, 0, 0, 1);
            m_wepons[0].color = new Color(1, 1, 1, 0.5f);
        }
    }

    void UpdateSelect()
    {
        if (m_gamemanager.PhotoCheckStateNow())
        {
            m_Sight.SetActive(false);
            XPhots.SetActive(true);
            weektextparent.SetActive(true);
            if (m_FlyerCount == 0) return;
            const float Margin = 0.5f;
            float inputHorizontal = /*(Input.GetAxisRaw("XboxLeftHorizontal") != 0) ? Input.GetAxisRaw("XboxLeftHorizontal") :*/ Input.GetAxisRaw("Horizontal");
            if (Mathf.Abs(inputHorizontal) > Margin && !m_IsChange)
            {
                if (inputHorizontal > 0.0f)
                {
                    currentSelectStageIndex += 1;
                }
                else
                {
                    currentSelectStageIndex += (m_FlyerCount - 1);
                }
                currentSelectStageIndex = currentSelectStageIndex % m_FlyerCount;
                float l_positionX = currentSelectStageIndex * 1280;
                XPhots.GetComponent<XPhotos>().MoveTargetPositionX(-l_positionX);
                m_IsChange = true;
                weektexts.AllQuestion();
            }
            else if (Mathf.Abs(inputHorizontal) <= Margin && m_IsChange)
            {
                m_IsChange = false;
                weektexts.SetTexts(xrayDatas[currentSelectStageIndex].datas);
            }
        }
        if (m_gamemanager.AttackStateNow())
        {
            XPhots.SetActive(false);
            weektextparent.SetActive(false);
            m_Sight.SetActive(true);
        }
        else
        {
            if(m_gamemanager.GetWeekPointData.Count > m_FlyerCount)
            {
                PhotoCreate(m_gamemanager.GetWeekPointData[m_FlyerCount]);
            }
        }
    }

    void LoadPhotos()
    {

    }

    public void ViewPhotos()
    {

    }

    public void PhotoCreate(GameManager.WeekPointData data)
    {
        xrayDatas.Add(data);
        GameObject photo = Instantiate(photoPrefab);
        photo.transform.parent = XPhots.transform;
        photo.GetComponent<RectTransform>().anchoredPosition = new Vector2(1280 * m_FlyerCount, 0);
        photo.transform.Find("Photo").GetComponent<RawImage>().texture = Resources.Load("Texture/RenderTextures/XrayCamera" + data.name) as RenderTexture;
        if (m_FlyerCount == 0) weektexts.SetTexts(data.datas);
         m_FlyerCount++;
    }

    /// <summary>
    /// これを呼ぶと残機UIが減らせる
    /// </summary>
    public void Life()
    {
        m_lifes[_lifecount -1].SetActive(false);
        _lifecount--;
        if (_lifecount <= 0) m_gamemanager.GameOver();
    }

    /// <summary>
    /// 今の残機数を取得できる(0～2)
    /// </summary>
    /// <returns></returns>
    public int LifeCounter()
    {
        return _lifecount;
    }
}
