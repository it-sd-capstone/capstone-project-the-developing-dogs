using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class PlayerGraphics : MonoBehaviour
{
    [Header("managers")]
    [SerializeField] public GameBoard board;
    [SerializeField] public GameManager gm;
    [SerializeField] public GameBoardGraphics gbg;

    [Header("action buttons")]
    [SerializeField] GameObject drive;
    [SerializeField] GameObject fly;
    [SerializeField] GameObject station;
    [SerializeField] GameObject share;
    [SerializeField] GameObject cure;

    public bool driveState = false;
    public bool flyState = false;
    public bool buildState = false;
    public bool shareState = false;
    public bool cureState = false;

    private List<City> dests;

    void Start()
    {
        Button drivebtn = drive.GetComponent<Button>();
        Button flybtn = fly.GetComponent<Button>();
        Button stationbtn = station.GetComponent<Button>();
        Button sharebtn = share.GetComponent<Button>();
        Button curebtn = cure.GetComponent<Button>();

        drivebtn.onClick.AddListener(() => OnDriveClick());
        flybtn.onClick.AddListener(() => OnFlyClick());
        stationbtn.onClick.AddListener(() => OnStationClick());
        sharebtn.onClick.AddListener(() => OnShareClick());
        curebtn.onClick.AddListener(() => OnCureClick());


    }

    public void OnDriveClick()
    {
        Debug.Log(driveState);
        dests = gm.players[gm.currentPlayerIndex].CurrentCity.neighbors;

        driveState = driveState ? false : true;

        HighlightDests(dests);
        dests.Clear();
    }

    public void OnFlyClick()
    {
        Debug.Log("Fly");
        var hand = gm.players[gm.currentPlayerIndex].Hand;
        foreach (PlayerCard card in hand)
        {
            dests.Add(card.City);
        }

        flyState = flyState ? false : true;

        HighlightDests(dests);
        dests.Clear();
    }

    public void OnStationClick()
    {
        Debug.Log("station");

        buildState = buildState ? false : true;
        
    }

    public void OnShareClick()
    {
        Debug.Log("share");
        shareState = shareState ? false : true;
        
    }

    public void OnCureClick()
    {
        Debug.Log("cure");
        cureState = cureState ? false : true;
        
    }

    public void HighlightDests(List<City> cities)
    {
        if (flyState)
        {
            foreach (City city in cities)
            {
                gbg.Highlight(city, flyState);
            }
        }
        else if (driveState)
        {
            foreach (City city in cities)
            {
                gbg.Highlight(city, driveState);
            }
        }
        else
        {
            foreach (City city in cities)
            {
                gbg.Highlight(city, false);
            }
        }
    }
}