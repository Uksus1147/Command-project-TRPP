using TMPro;
using UnityEngine;

public class PlayerBalance : MonoBehaviour
{
    public static PlayerBalance Instance { get; private set; }

    [SerializeField] private int startingBalance = 1000;
    private int _balance;

    [SerializeField] private TMP_Text balanceText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        _balance = startingBalance;
        UpdateBalanceUI();
    }

    // Вызывается при выигрыше или потере денег
    public bool TrySpend(int amount)
    {
        if (_balance < amount || amount < 0)
        {
            return false;
        }

        _balance -= amount;

        if (_balance <= 0)
        {
            OnGameOver();
        }

        UpdateBalanceUI();

        return true;
    }

    public void Add(int amount)
    {
        if (amount < 0) return;

        _balance += amount;
        UpdateBalanceUI();
    }

    private void UpdateBalanceUI()
    {
        if (balanceText != null)
        {
            balanceText.text = $"Баланс: {_balance}";
        }
    }

    private void OnGameOver()
    {
        Debug.Log("Игра окончена: недостаточно средств!");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
