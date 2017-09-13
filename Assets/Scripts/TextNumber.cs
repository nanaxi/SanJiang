using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextNumber : MonoBehaviour {
    //public Font m_Font;
    public int testNumber;

    [SerializeField]private string numberImgPath = "Resources/XXX";

    [SerializeField]
    private Sprite[] spriteNmbAry;

    private Transform m_layoutParent;
    private int m_TextNumber;

    public Transform LayoutParent
    {
        get
        {
            if (m_layoutParent == null)
            {
                m_layoutParent = transform;
            }
            return m_layoutParent;
        }

        private set
        {
            m_layoutParent = value;
        }
    }

    public int textNumber
    {
        get
        {
            return m_TextNumber;
        }

        set
        {
            m_TextNumber = value;

            DestroyTextNumber();
            UpdateTextNumber();
        }
    }

    public Sprite[] SpriteNmbAry
    {
        get
        {
            if (spriteNmbAry == null || spriteNmbAry.Length < 1)
            {
                spriteNmbAry = Resources.LoadAll<Sprite>(numberImgPath);
            }
            return spriteNmbAry;
        }

        private set
        {
            spriteNmbAry = value;
        }
    }


    // Use this for initialization
    void OnEnable()
    {
        if (LayoutParent.GetComponent<GridLayoutGroup>() == null)
        {
            Debug.LogError("GetComponent<GridLayoutGroup>() == null   __gameObject.Name=" + gameObject.name);
        }
        Debug.Log(SpriteNmbAry.Length);
    }

    void UpdateTextNumber()
    {
        string strTextNumber = textNumber.ToString();
        if (textNumber >0)
        {
            strTextNumber = "+" + strTextNumber;
        }

        for (int i = 0; i < strTextNumber.Length; i++)
        {
            Sprite spriteRes = GetSprite(strTextNumber[i].ToString());
            if (spriteRes==null)
            {
                continue;
            }
            GameObject gmIns = new GameObject();
            Image imgGet = gmIns.AddComponent<Image>();
            imgGet.sprite = spriteRes;
            gmIns.name = spriteRes.name;
            gmIns.transform.SetParent(LayoutParent);
            gmIns.transform.localScale = Vector3.one;
        }
    }


    Sprite GetSprite(string spriteName)
    {
        if (SpriteNmbAry == null || SpriteNmbAry.Length < 1)
        {
            Debug.LogError("SpriteNmbAry == null || SpriteNmbAry.Length < 1");
        }
        if (SpriteNmbAry != null)
        {
            for (int i = 0; i < SpriteNmbAry.Length; i++)
            {
                if (SpriteNmbAry[i].name == spriteName)
                {
                    return SpriteNmbAry[i];
                }
            }
        }
        
        return null;
    }

    void DestroyTextNumber()
    {
        for (int i = 0; i < LayoutParent.childCount; i++)
        {
            if (LayoutParent.GetChild(i) != null)
            {
                Destroy(LayoutParent.GetChild(i).gameObject);
            }
        }
    }

    public void ClearAll()
    {
        DestroyTextNumber();
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(256, 256, 128, 64), "Test TextNumber"))
        {
            textNumber = testNumber;
        }
    }
}
