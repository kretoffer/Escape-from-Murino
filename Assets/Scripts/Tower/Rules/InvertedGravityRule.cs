
using UnityEngine;

[CreateAssetMenu(fileName = "InvertedGravityRule", menuName = "Rules/InvertedGravityRule")]
public class InvertedGravityRule : Rule
{
    private Vector3 _originalGravity;

    public override void Apply()
    {
        _originalGravity = Physics.gravity;
        Physics.gravity = -_originalGravity;
    }

    public override void Revert()
    {
        Physics.gravity = _originalGravity;
    }
}
