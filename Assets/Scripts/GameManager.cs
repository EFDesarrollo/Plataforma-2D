using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int score;
    private bool win;

    public int Score { get => score; set => score = value; }
    public bool Win { get => win; set => win = value; }

    public static GameManager instance;
    private void Awake()
    {
        instance = this;
        if (instance == null)
            DontDestroyOnLoad(gameObject);
        else
            Destroy(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
