using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell")]
public class Spell : ScriptableObject
{
	public ShapeIdentifier shapeIdentifier;
	public SpellExecution execution;

	public bool Indentify()
	{
		return true;
	}
}
