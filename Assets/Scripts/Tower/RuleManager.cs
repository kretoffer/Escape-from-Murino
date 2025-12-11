
using System.Collections.Generic;
using UnityEngine;

public class RuleManager : MonoBehaviour
{
    public static RuleManager Instance { get; private set; }


    [SerializeField] private List<Rule> _availableRules;
    private List<Rule> _activeRules = new List<Rule>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        
        _availableRules = new List<Rule>(Resources.LoadAll<Rule>("Rules"));
    }

    void Start()
    {
        ApplyRandomRule();
    }


    public void ApplyRandomRule()
    {
        if (_availableRules.Count == 0)
        {
            Debug.LogWarning("No available rules to apply.");
            return;
        }

        var ruleToApply = _availableRules[Random.Range(0, _availableRules.Count)];
        if (!_activeRules.Contains(ruleToApply))
        {
            ruleToApply.Apply();
            _activeRules.Add(ruleToApply);
            Debug.Log($"Applied rule: {ruleToApply}");
        }
    }

    public void RevertAllRules()
    {
        foreach (var rule in _activeRules)
        {
            rule.Revert();
            Debug.Log($"Reverted rule: {rule}");
        }
        _activeRules.Clear();
    }

    private void OnDestroy()
    {
        RevertAllRules();
    }
}
