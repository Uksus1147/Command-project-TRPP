using TMPro;
using UnityEngine;

public class MachineController : MonoBehaviour
{

    [SerializeField] private Slot[] slots;
    [SerializeField] private TMP_Text sumText;
    [SerializeField] private TMP_InputField betInputField;
    [SerializeField] private TMP_Text balanceText; // ����� ��� ����������� �������

    private int _i = 0;
    private bool _isFinished = false;

    private int _playerBalance = 1000; // ��������� ������

    private void Start()
    {
        UpdateBalanceUI();
    }

    // ���������� ��� ������� �� ������ "����"
    public void Stop()
    {
        if (!_isFinished && _i < slots.Length)
        {
            float score = slots[_i].Stop();
            Debug.Log($"���� {_i} ����� ����: {score}");
            _i++;

            if (_i == slots.Length)
            {
                _isFinished = true;
                CalculateResult();
            }
        }
    }

    // ���������� ��� ������� �� ������ "����������"
    public void Restart()
    {
        if (_isFinished)
        {
            int betAmount = 0;

            // ��������� ������
            if (!int.TryParse(betInputField.text, out betAmount) || betAmount <= 0)
            {
                Debug.LogWarning("������� ���������� ������!");
                return;
            }

            if (_playerBalance < betAmount)
            {
                Debug.LogWarning("������������ �������!");
                return;
            }

            // ��������� ������
            _playerBalance -= betAmount;
            UpdateBalanceUI();

            // ��������� �����
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

            Debug.Log("��� ����� ������������");
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

        // ��������� ��� �������
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
            // ��� ������� ����������
            float multiplier = symbolsOnScreen[0].winMultiplier;
            reward = Mathf.RoundToInt(betAmount * multiplier);
            sumText.text = $"�������! ��� �������: {firstType}. �������: {reward}";
        }
        else if (symbolsOnScreen[0].symbolType == symbolsOnScreen[1].symbolType ||
                 symbolsOnScreen[1].symbolType == symbolsOnScreen[2].symbolType ||
                 symbolsOnScreen[0].symbolType == symbolsOnScreen[2].symbolType)
        {
            // ��� �� ��� �������
            reward = Mathf.RoundToInt(betAmount * 0.5f); // ���������� �������� ������
            sumText.text = $"��������� ����������. ��������: {reward}";
        }
        else
        {
            // ��� ����������
            reward = 0;
            sumText.text = " ��� ����������. �� ��������� ������.";
        }

        Debug.Log($"��� �����: {symbolsOnScreen[0]} , {symbolsOnScreen[1]} , {symbolsOnScreen[2]}");
        _playerBalance += reward;
        UpdateBalanceUI();

        //if (sumText != null)
        //{
        //    sumText.text = $"������: {firstType} x{slots.Length}";
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
            balanceText.text = $"������: {_playerBalance}";
        }
    }

    private void GameOver()
    {
        Debug.Log("���� ��������: ������������ �������!");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
