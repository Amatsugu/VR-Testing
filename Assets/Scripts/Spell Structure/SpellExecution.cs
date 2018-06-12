using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Spell Component/Execution")]
public abstract class SpellExecution : ScriptableObject
{
	public SpellController controller;
	public SpellStats stats;
	public SpellExecution chain;

	public abstract void Init();
}
