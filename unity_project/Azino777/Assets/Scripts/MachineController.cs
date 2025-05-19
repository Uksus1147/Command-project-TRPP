using TMPro;
using UnityEngine;

public class MachineController : MonoBehaviour
{

    [SerializeField] private Slot[] slots;
    [SerializeField] private TMP_Text sumText;
    [SerializeField] private TMP_InputField betInputField;
    [SerializeField] private TMP_Text balanceText; // Текст для отображения баланса

    private int _i = 0;
    private bool _isFinished = false;

    private int _playerBalance = 1000; // Стартовый баланс

    private void Start()
    {
        UpdateBalanceUI();
    }

    // Вызывается при нажатии на кнопку "Стоп"
    public void Stop()
    {
        if (!_isFinished && _i < slots.Length)
        {
            float score = slots[_i].Stop();
            Debug.Log($"Слот {_i} выдал очки: {score}");
            _i++;

            if (_i == slots.Length)
            {
                _isFinished = true;
                CalculateResult();
            }
        }
    }

    // Вызывается при нажатии на кнопку "Перезапуск"
    public void Restart()
    {
        if (_isFinished)
        {
            int betAmount = 0;

            // Проверяем ставку
            if (!int.TryParse(betInputField.text, out betAmount) || betAmount <= 0)
            {
                Debug.LogWarning("Введите корректную ставку!");
                return;
            }

            if (_playerBalance < betAmount)
            {
                Debug.LogWarning("Недостаточно средств!");
                return;
            }

            // Списываем ставку
            _playerBalance -= betAmount;
            UpdateBalanceUI();

            // Запускаем слоты
            _isFinished = false;
            _i = 0;

            foreach (var slot in slots)
            {
                slot.StartSpinning();
            }

            if (sumText != null)
            {
                sumText.text = "";
            }

            Debug.Log("Все слоты перезапущены");
        }
    }

    private void CalculateResult()
    {
        Symbol[] symbolsOnScreen = new Symbol[slots.Length];

        for (int i = 0; i < slots.Length; i++)
        {
            Transform centerChild = slots[i].transform.GetChild(slots[i].transform.childCount - 1);
            symbolsOnScreen[i] = centerChild.GetComponent<Symbol>();
        }

        // Проверяем все символы
        bool allSame = true;
        string firstType = symbolsOnScreen[0].symbolType;

        foreach (var s in symbolsOnScreen)
        {
            if (s.symbolType != firstType)
            {
                allSame = false;
                break;
            }
        }

        int reward = 0;
        int betAmount = int.Parse(betInputField.text);

        if (allSame)
        {
            // Все символы одинаковые
            float multiplier = symbolsOnScreen[0].winMultiplier;
            reward = Mathf.RoundToInt(betAmount * multiplier);
            sumText.text = $"Джекпот! Все символы: {firstType}. Выигрыш: {reward}";
        }
        else if (symbolsOnScreen[0].symbolType == symbolsOnScreen[1].symbolType ||
                 symbolsOnScreen[1].symbolType == symbolsOnScreen[2].symbolType ||
                 symbolsOnScreen[0].symbolType == symbolsOnScreen[2].symbolType)
        {
            // Два из трёх совпали
            reward = Mathf.RoundToInt(betAmount * 0.5f); // Возвращаем половину ставки
            sumText.text = $"Частичное совпадение. Получено: {reward}";
        }
        else
        {
            // Нет совпадений
            reward = 0;
            sumText.text = " Нет совпадений. Вы проиграли ставку.";
        }

        Debug.Log($"Все слоты: {symbolsOnScreen[0]} , {symbolsOnScreen[1]} , {symbolsOnScreen[2]}");
        _playerBalance += reward;
        UpdateBalanceUI();

        //if (sumText != null)
        //{
        //    sumText.text = $"Выпало: {firstType} x{slots.Length}";
        //}

        if (_playerBalance <= 0)
        {
            GameOver();
        }
    }

    private void UpdateBalanceUI()
    {
        if (balanceText != null)
        {
            balanceText.text = $"Баланс: {_playerBalance}";
        }
    }

    private void GameOver()
    {
        Debug.Log("Игра окончена: недостаточно средств!");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
