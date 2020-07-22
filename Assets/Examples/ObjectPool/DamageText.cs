using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    private float disappearSpeed = 5f;
    private float disappearTimer;
    private float disappearTimeDefault = 0.5f;
    private float ySpeed = 1.25f;
    private TextMeshPro textMesh;
    private Color textColor;

    public Transform target;
    public Vector3 oldPos;
    public float flyLenth;

    public void Setup(int damage, bool friendly)
    {
        textMesh.SetText(damage.ToString());
        if (!friendly)
        {
            //textColor = textMesh.color;
            textColor = Color.yellow;
            textMesh.color = textColor;
        }
        else
        {
            textColor = Color.red;
            textMesh.color = textColor;
        }
        disappearTimer = disappearTimeDefault;
    } 

    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if(null != this.target)
    //    {
    //        Vector3 pos = this.target.position;
    //        if (null != this.target.GetComponent<BoxCollider2D>())
    //        {
    //            pos = this.target.GetComponent<BoxCollider2D>().bounds.center;
    //            pos += new Vector3(0, this.target.GetComponent<BoxCollider2D>().bounds.size.y / 2.0f, 0);
    //        }

    //        oldPos = pos;
    //    }
    //    flyLenth += ySpeed * Time.deltaTime;
    //    transform.position = oldPos + new Vector3(0, flyLenth);
    //    disappearTimer -= Time.deltaTime;
    //    if (disappearTimer <= 0)
    //    {
    //        textColor.a -= disappearSpeed * Time.deltaTime;
    //        textMesh.color = textColor;
    //        if (textColor.a <= 0)
    //        {
    //            Destroy(gameObject);
    //        }
    //    }
    //}

    public bool DoUpdate(float deltaTime)
    {
        if (null != this.target)
        {
            Vector3 pos = this.target.position;
            if (null != this.target.GetComponent<BoxCollider2D>())
            {
                pos = this.target.GetComponent<BoxCollider2D>().bounds.center;
                pos += new Vector3(0, this.target.GetComponent<BoxCollider2D>().bounds.size.y / 2.0f, 0);
            }

            oldPos = pos;
        }
        flyLenth += ySpeed * deltaTime;
        transform.position = oldPos + new Vector3(0, flyLenth);
        disappearTimer -= deltaTime;
        if (disappearTimer <= 0)
        {
            textColor.a -= disappearSpeed * deltaTime;
            textMesh.color = textColor;
            if (textColor.a <= 0)
            {
                return true;
            }
        }

        return false;
    }
}
