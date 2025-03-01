using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public interface IPlayer : IDamageable
{
    void AddExp(float exp);
}

public interface IMonster : IDamageable
{ 
}

public interface IDamageable
{
    void TakeHit(float damage);
}