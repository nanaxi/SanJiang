using UnityEngine;
using System.Collections;

/// <summary>打算用这个管理声音。   该声音为单个，如果之后为多个， 那么就还需要更改
/// </summary>
public class Audio_Manage : MonoBehaviour {
    private AudioSource my_Audio_S;//当前没有播放声音的空音源
    [SerializeField]private AudioSource[] my_Audio_Array;
    private AudioSource[] playerAudop_Ary;
    private static Audio_Manage inst;
    [SerializeField]private float audio_Volume;
    public static Audio_Manage Instance// Instance
    {
        get
        {
            if (inst == null)
            {
                GameObject gm_N = new GameObject();
                gm_N.name = "Audio_Manage";
                inst = gm_N.AddComponent<Audio_Manage>();
            }
            return inst;
        }
    }

    public float BGValue
    {
        get
        {
            bGValue = my_Audio_Array[0].volume;
            return bGValue;
        }

        set
        {
            bGValue = value;
        }
    }

    [SerializeField]private AudioClip test_Audio;


    // Use this for initialization
    void Awake()
    {
        if (inst == null)
        {
            inst = this;
        }
        my_Audio_S = Get_AudioSource();
        audio_Volume = 0.5f;
        //AudioListener.volume = 0.5f;
        if (!PlayerPrefs.HasKey(Pp_Name.NxHs_AudioBgV_.ToString()))
        {
            PlayerPrefs.SetFloat(Pp_Name.NxHs_AudioBgV_.ToString(),1.0f);                
        }

        if (!PlayerPrefs.HasKey(Pp_Name.NxHs_Audio1V_.ToString()))
        {
            PlayerPrefs.SetFloat(Pp_Name.NxHs_Audio1V_.ToString(), 1.0f);
        }

        playerAudop_Ary = new AudioSource[4];

        GameObject gm_PlayerAudio = new GameObject();
        gm_PlayerAudio.name = "PlayerAudios";

        for (int i = 0; i < playerAudop_Ary.Length; i++)
        {
            GameObject gm_PAudio = new GameObject();
            gm_PAudio.name = "PlayerAudio" + (i + 1);
            gm_PAudio.transform.SetParent(gm_PlayerAudio.transform);
            playerAudop_Ary[i] = gm_PAudio.AddComponent<AudioSource>();
            playerAudop_Ary[i].loop = false;
        }

        Play_Audio("AudioClips/BGM_1");
        Init_AudioSize();
    }

    public void Init_AudioSize()
    {
        Set_AudioS_Volume(PlayerPrefs.GetFloat(Pp_Name.NxHs_Audio1V_.ToString()));
        Set_AudioLister_V(PlayerPrefs.GetFloat(Pp_Name.NxHs_AudioBgV_.ToString()));
    }

    /// <summary>4个玩家对号入座 播放声音
    /// </summary>
    public void Player_Play_Audio(int c_FangXiang, string resourcesPath) //AudioClip audioC = null)
    {
        AudioClip audioC = Resources.Load<AudioClip>(resourcesPath);
        if (audioC == null)
        {
            Debug.Log("你想播放一个空的声音？" + resourcesPath);
        }
        else
        {
            playerAudop_Ary[c_FangXiang].volume = PlayerPrefs.GetFloat(Pp_Name.NxHs_Audio1V_.ToString());
            playerAudop_Ary[c_FangXiang].clip = audioC;
            playerAudop_Ary[c_FangXiang].Play();
        }
    }
    public void PlayBg_Audio(AudioClip audioC )
    {
        if (audioC == null)
        {
            Debug.Log("你想播放一个空的声音？");
        }
        my_Audio_Array[0].clip = audioC;
        my_Audio_Array[0].Play();

    }

    /// <summary>播放 Resources 路径下的声音
    /// </summary>
    public void Play_Audio(string resourcesPath)// AudioClip audioC = null)
    {
        AudioClip audioC = Resources.Load<AudioClip>(resourcesPath);
        if (audioC == null)
        {
            Debug.Log("你想播放一个空的声音？" + resourcesPath);
            return;
        }
        //my_Audio_S.volume = audio_Volume;
        
        my_Audio_S.clip = audioC;
        my_Audio_S.Play();
        if (my_Audio_S!=my_Audio_Array[0])
        {
            AudioSource s_S = my_Audio_S;
            StartCoroutine(If_PlayAudioOver(s_S));
            //Debug.Log("AAAAA1");
        }

        if (my_Audio_S == my_Audio_Array[0])
        {
            my_Audio_S.loop = true;
        }
        my_Audio_S = Get_AudioSource();

    }

    public void Stop_Audio()
    {
        my_Audio_S.Stop();
    }

    public void Set_AudioS_Volume(float f_V)
    {
        audio_Volume = f_V;
        PlayerPrefs.SetFloat(Pp_Name.NxHs_Audio1V_.ToString(), f_V);        
        for (int i = 1; i < my_Audio_Array.Length; i++)
        {
            my_Audio_Array[i].volume = f_V;
        }
    }
    private float bGValue = 0;
    public void Set_AudioLister_V(float f_V)
    {
        PlayerPrefs.SetFloat(Pp_Name.NxHs_AudioBgV_.ToString(), f_V);
        //AudioListener.volume = f_V;
        if (my_Audio_Array[0]!=null)
        {
            my_Audio_Array[0].volume = f_V;
            if (!my_Audio_Array[0].isPlaying)
            {
                my_Audio_Array[0].Play();
            }
        }
    }


    float closeBgMiusc_Value =0;
    /// <summary>播放或者停止背景音乐
    /// </summary>
    /// <param name="b_"></param>
    public void PlayOrStop_BgAudio(bool b_)
    {
        my_Audio_Array[0].volume = b_? closeBgMiusc_Value:0;
    }

    /// <summary>获取没有播放声音的 音源
    /// </summary>
    /// <returns></returns>
    private AudioSource Get_AudioSource()
    {
        my_Audio_Array = transform.GetComponentsInChildren<AudioSource>();

        for (int i = 0; i < my_Audio_Array.Length; i++)
        {
            if (!my_Audio_Array[i].isPlaying)
            {
                my_Audio_S = my_Audio_Array[i];
                return my_Audio_S;
            }
        }
        GameObject gm_Audio = new GameObject();
        gm_Audio.name = "AudioSource_" + my_Audio_Array.Length;
        gm_Audio.transform.SetParent(transform);
        AudioSource set_V = gm_Audio.AddComponent<AudioSource>();
        set_V.loop = false; 
        set_V.volume = PlayerPrefs.GetFloat(Pp_Name.NxHs_Audio1V_.ToString());
        return Get_AudioSource();
    }

    IEnumerator If_PlayAudioOver(AudioSource audioS_)
    {
        float time_S = (audioS_.clip.length)+2;
        while (audioS_.clip!=null)
        {
            if (!audioS_.isPlaying)
            {
                audioS_.Stop();
                audioS_.clip = null;
            }
            //if (time_S > 0)
            //{
            //    time_S--;
            //}
            //else
            //{
            //    audioS_.Stop();
            //    audioS_.clip = null;
            //}
            yield return new WaitForSeconds(1.0f);
        }
        yield return null;
    }
}
