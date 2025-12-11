
using UnityEngine;

[CreateAssetMenu(fileName = "InvertedControlsRule", menuName = "Rules/InvertedControlsRule")]
public class InvertedControlsRule : Rule
{
    public override void Apply()
    {
        PlayerMovement.ControlsInverted = true;
    }

    public override void Revert()
    {
        PlayerMovement.ControlsInverted = false;
    }
}
