using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public interface IPlayer
{
    void AddExp(float exp);
}

public interface IMonster
{ 
    void ApplyDamage(float damage, IPlayer player);
}

public interface IDamageable
{
    float GetDamage();
}