using UnityEngine;

[CreateAssetMenu(menuName = "Spell Component/Spell Shape")]
public class SpellShape : ScriptableObject
{
	public ShapeMatcher shape;
	public AttributeMatcher attribute;
}