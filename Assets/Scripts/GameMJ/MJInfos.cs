using UnityEngine;
using System.Collections;


public class MJInfos : MonoBehaviour {

    public MJPositionType mjPType = MJPositionType.NULL;
    [SerializeField]
    private Mj_Sx_ mjINFO_;

    public Mj_Sx_ MjINFO_
    {
        get
        {
            return mjINFO_;
        }

        set
        {
            mjINFO_ = value;
            //Debug.Log("Look MJ Hs :" + mjINFO_.mj_SpriteName);
        }
    }

 //   // Use this for initialization
 //   void Start () {
	
	//}

}


public enum MJPositionType
{
    NULL, MJChuPai, MJMoPai, MJPaiQiang, MJShouPai,MJPengGangPai
}