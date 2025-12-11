
using UnityEngine;
using UnityEditor;

public class CreateRuleAssets : MonoBehaviour
{
    [MenuItem("Rules/Create Rule Assets")]
    public static void CreateAssets()
    {
        InvertedGravityRule invertedGravityRule = ScriptableObject.CreateInstance<InvertedGravityRule>();
        AssetDatabase.CreateAsset(invertedGravityRule, "Assets/Resources/Rules/InvertedGravityRule.asset");

        InvertedControlsRule invertedControlsRule = ScriptableObject.CreateInstance<InvertedControlsRule>();
        AssetDatabase.CreateAsset(invertedControlsRule, "Assets/Resources/Rules/InvertedControlsRule.asset");

        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = invertedGravityRule;
    }
}
