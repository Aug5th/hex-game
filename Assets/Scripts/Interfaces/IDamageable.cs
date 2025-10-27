using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IDamageable
{
    void TakeDamage(int amount, GameObject source);
    UnityAction OnDeath { get; set; }
}
