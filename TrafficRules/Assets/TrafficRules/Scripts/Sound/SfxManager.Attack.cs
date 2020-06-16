using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SfxManager
{
    [Header("Use for AttackHIt")]
    public KAudio.KBusAudio AttackHitBus;

    [Header("Use for Attack")]
    public KAudio.KBusAudio AttackBus;

    #region Action - Attack
    public SoundComponent Play3DAttack(int idLauncher, Vector3 position, bool IsCharacter = true, bool isCrit = false)
    {
        string fileName = string.Empty;
        if (IsCharacter == false)
        {
            fileName = string.Format("SFX_Hero_{0}_Summon_Attack", idLauncher);
        }
        else
            fileName = string.Format("SFX_Hero_{0}_Attack", idLauncher);

        if (isCrit)
            fileName = "SFX_Hero_CritHit";

        SoundComponent source = LoadAndPlay(fileName, position, this.AttackBus);
        return source;
    }

    public SoundComponent Play3DBeHitted(int idLauncher, Vector3 position, bool IsCharacter = true)
    {
        string fileName = string.Empty;
        if (IsCharacter == false)
        {
            fileName = string.Format("SFX_Hero_{0}_Summon_Attack_Hit", idLauncher);
        }
        else
            fileName = string.Format("SFX_Hero_{0}_Attack_Hit", idLauncher);

        SoundComponent source = LoadAndPlay(fileName, position, this.AttackHitBus);
        return source;
    }
    #endregion
}
