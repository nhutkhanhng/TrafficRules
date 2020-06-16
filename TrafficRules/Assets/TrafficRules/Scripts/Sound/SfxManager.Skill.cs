using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SfxManager
{
    #region Define Name
    // Lúc ra Skill.
    public static string SKILL_CAST = "SFX_Skill_{0}_Cast";
    // Đang vận;
    public static string SKILL_CHANNELLING = "SFX_SKill_{0}_Channelling";
    // Sound dành cho dạng Intansce; Của con Puck
    public static string SKILL_FLYING = "SFX_Skill_{0}_Flying";
    // Cái này Audio lúc mà vừa bị tác động - Ví dụ như con Tiny vừa đặt tay vào ném con kia
    public static string SKILL_INFLUENCE = "SFX_Skill_{0}_Influence";

    // Cái này là cho mấy cái Sound: dạng như Skill của con Tiny hất đối thủ rơi xuống đất. ( Tiếng rơi xuống đất);
    public static string SKILL_INFLUENCED = "SFX_Skill_{0}_Influenced";
    // Sound lúc đối tượng bị trúng Skill
    public static string SKILL_BEHITTED = "SFX_Skill_{0}_BeHitted";
    #endregion
    [Header("Use for Skill")]
    public KAudio.KBusAudio SkillBus;
    public KAudio.KBusAudio SkillHit;
    public KAudio.KBusAudio SkillFlying;
    public KAudio.KBusAudio SkillCast;

    public SoundComponent Play3DSkill(string skillName, Vector3 Position)
    {
        return LoadAndPlay(skillName, Position, this.SkillBus);
    }
    public SoundComponent Play3DSkill(string skillName, Transform ObjectBeAttached = null, float DelayTime = 0f, float Duration = -1f)
    {
        Vector3 OriginPosition = Vector3.zero;
        if (ObjectBeAttached != null)
            OriginPosition = ObjectBeAttached.transform.position;

        var source = LoadAndPlay(skillName, OriginPosition, SkillBus, DelayTime, Duration);

        if (ObjectBeAttached == null || source == null)
            return source;

        if (ObjectBeAttached.gameObject != null)
            source.gameObject.transform.SetParent(ObjectBeAttached);
#if UNITY_EDITOR
        else
            Debug.LogError("Parrent is null");
#endif
        return source;
    }
    #region Action SKill 

    public SoundComponent Play3D_Skill_Cast(string SkillName, Vector3 Position)
    {
        string fileName = string.Format(SKILL_CAST, SkillName);

        var source = LoadAndPlay(fileName, Position, SkillCast);
        return source;
    }

    public SoundComponent Play3D_SKill_Channeling(string SkillName, Vector3 Position)
    {
        return LoadAndPlay(SkillName, Position, SkillCast);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="skillName"></param>
    /// <param name="Position"></param>
    /// <returns></returns>
    public SoundComponent Play3D_SKillBeHitted(string skillName, Vector3 Position)
    {
        return LoadAndPlay(skillName, Position, SkillHit);
    }

    /// <summary>
    /// Âm thanh khi mà con bị ảnh hưởng kết thúc hành động ảnh hưởng;
    /// Ví dụ như con cờ bị Tiny hất thì đây là lúc chạm đất
    /// </summary>
    /// <param name="skillName"></param>
    /// <param name="Position"></param>
    /// <returns></returns>
    public SoundComponent Play3D_Skill_Influence(string skillName, Vector3 Position)
    {
        var source = LoadAndPlay(skillName, Position, SkillHit);

        return source;
    }

    /// <summary>
    /// Audio sinh ra bởi mấy skill Intantiate một thực thể
    /// Example:
    ///     - Quả cầu của con Puck.
    ///     - ... =]].
    /// </summary>
    /// <param name="IdSkill"></param>
    /// <param name="ObjectBeAttached"></param>
    /// <returns></returns>
    public SoundComponent Play3D_Skill_Plying(int IdSkill, Transform ObjectBeAttached, float Duration, float DelayTime = 0f)
    {
        string fileName = string.Empty;
        fileName = string.Format(SKILL_FLYING, IdSkill);

        var source = LoadAndPlay(fileName, ObjectBeAttached.position, SkillFlying, DelayTime, Duration);

        if (ObjectBeAttached.gameObject != null)
            source.gameObject.transform.SetParent(ObjectBeAttached);
#if UNITY_EDITOR
        else
            Debug.LogError("Parrent is null");
#endif

        return source;
    }
    #endregion

    #region Code cũ
    public SoundComponent Play3D_UltimateCastSkill(int idLauncher, Vector3 position, bool IsCharacter = true, float time = 2f)
    {
        string fileName = string.Empty;
        if (IsCharacter == false)
        {
            fileName = string.Format("SFX_Hero_{0}_Summon_Ultimate_Casting", idLauncher);
        }
        else
            fileName = string.Format("SFX_Hero_{0}_Ultimate_Casting", idLauncher);

        return LoadAndPlay(fileName, position, this.SkillCast, 0f, time);
    }

    public SoundComponent Play3D_UltimatePerformSkill(int idLauncher, Vector3 position, bool IsCharacter = true, float time = 2f)
    {
        string fileName = string.Empty;
        if (IsCharacter == false)
        {
            fileName = string.Format("SFX_Hero_{0}_Summon_Ultimate_Perform", idLauncher);
        }
        else
            fileName = string.Format("SFX_Hero_{0}_Ultimate_Perform", idLauncher);

        return LoadAndPlay(fileName, position, this.SkillCast, 0f, time);
    }


    public SoundComponent Play3DSkillEndInfluence(int IdSkill, Vector3 position)
    {
        string fileName = string.Empty;

        fileName = string.Format("SFX_Chess_{0}_Skill_EndInfluence", IdSkill);

        return LoadAndPlay(fileName, position);
    }


    public SoundComponent Play3D_UltimateSkillHit(int idLauncher, Vector3 position, bool IsCharacter = true)
    {
        string fileName = string.Empty;
        if (IsCharacter == false)
        {
            fileName = string.Format("SFX_Hero_{0}_Summon_Skill_Hit", idLauncher);
        }
        else
            fileName = string.Format("SFX_Hero_{0}_Ultimate_Skill_Hit", idLauncher);

        return LoadAndPlay(fileName, position);
    }
    #endregion

}
