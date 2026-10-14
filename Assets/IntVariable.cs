using UnityEngine;

[CreateAssetMenu(fileName = "IntVariable", menuName = "ScriptableObjects/IntVariable", order = 2)]
public class IntVariable : Variable<int>
{
    // PlayerPrefs key for persisting the highest value across sessions and builds
    public string highscoreKey = "highscore";
    public int previousHighestValue;

    public override void SetValue(int value)
    {
        if (value > previousHighestValue)
        {
            previousHighestValue = value;
            PlayerPrefs.SetInt(highscoreKey, previousHighestValue);
        }
        _value = value;
    }

    public void ApplyChange(int amount)
    {
        Value += amount;
    }

    public void LoadHighestValue()
    {
        previousHighestValue = PlayerPrefs.GetInt(highscoreKey, 0);
    }

    // hook this up to a "Reset Highscore" button
    public void ResetHighestValue()
    {
        previousHighestValue = 0;
        PlayerPrefs.DeleteKey(highscoreKey);
        PlayerPrefs.Save();
    }
}
