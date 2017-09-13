using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>游戏中礼物
/// </summary>
public class GameGift : MonoBehaviour {

    public Image imgGift;
    public Transform moveTarget;
    public string imgPath;
    [SerializeField]
    private Sprite[] spriteAry;
    private const int maxErrorValue = 2;
    private const int moveSpeed = 128;
        
    // Use this for initialization
    void Start () {
        
    }

    IEnumerator GiftMove()
    {
        while ( gameObject.activeInHierarchy)
        {
            if (Vector3.Distance(transform.position, moveTarget.position) > maxErrorValue)
            {
                transform.position = Vector3.MoveTowards(transform.position, moveTarget.position, moveSpeed * Time.deltaTime);
            }
            yield return new WaitForEndOfFrame();
        }
        

        yield return null;
    }

    public void GiftStartMove(Transform target , string path)
    {
        moveTarget = target;
        imgPath = path;
        spriteAry = Resources.LoadAll<Sprite>(path);
        StartCoroutine(GiftMove());
    }
}

