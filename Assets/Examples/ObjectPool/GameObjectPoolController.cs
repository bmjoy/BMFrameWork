using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GameObjectPoolController : MonoBehaviour
{
    [SerializeField] private ScriptableContainer scriptableContainerGameObjectPool = null;

    private readonly Stopwatch gameTimer = new Stopwatch();

    private long lastGameUpdateTime;

    [SerializeField] private DamageTextController damageTextController;

    public GameObject player;


    private void Awake()
    {        
        DontDestroyOnLoad(this);
        scriptableContainerGameObjectPool.Register();
        gameTimer.Start();
    }

    private void OnApplicationQuit()
    {
        gameTimer.Stop();
        scriptableContainerGameObjectPool.UnRegister();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        long elapsedTime = gameTimer.ElapsedMilliseconds;
        int gameTimeDiff = (int)(elapsedTime - lastGameUpdateTime);
        float gameTimeFloatDiff = gameTimeDiff / 1000.0f;

        lastGameUpdateTime = elapsedTime;

        scriptableContainerGameObjectPool.DoUpdate(gameTimeDiff);

        if(Input.GetMouseButtonDown(0))
        {
            Vector3 pos = player.transform.position;
            CapsuleCollider capsuleCollider = player.GetComponent<CapsuleCollider>();
            pos = capsuleCollider.center;
            pos += Vector3.up * capsuleCollider.bounds.size.y/2.0f;
            pos -= Camera.main.transform.forward * capsuleCollider.bounds.size.x;
            damageTextController.Create(pos, -100, false);
        }
        damageTextController.DoUpdate(gameTimeFloatDiff);
    }
}
