using UnityEngine;

public class States : MonoBehaviour
{
    // ���������� ��� �������� ���������� ����� JSON
    public TextAsset statesJSON;

    // ����� ��� ������������� ������ �����
    [System.Serializable]
    public class State
    {
        // �������� �����
        public string name;
        // ������ ���������, �������� � ������ ����
        public int[] provinceNumbers;
    }

    // ����� ��� ������������� ������ ������
    [System.Serializable]
    public class StatesList
    {
        // ������ �������� State
        public State[] states;
    }

    // ��������� ������ StatesList ��� �������� ������ �� JSON
    public StatesList myStatesList = new StatesList();

    // ������ �� ����� Provinces
    public Provinces provincesData;

    void Start()
    {
        // �������������� ������ �� JSON � ������ myStatesList
        myStatesList = JsonUtility.FromJson<StatesList>(statesJSON.text);
    }

    // ����� ��� ��������� �������� ��������� �� �� ������
    public string GetProvinceName(int provinceNumber)
    {
        foreach (var state in myStatesList.states)
        {
            foreach (var provinceNum in state.provinceNumbers)
            {
                if (provinceNum == provinceNumber)
                {
                    return "��������� " + provinceNumber; // ������ ���������� ����� ��������� ��� �������
                }
            }
        }
        return "�������� ��������� �� �������";
    }

    // ����� ��� ��������� �������� ������� �� ������ ���������
    public string GetStateName(int provinceNumber)
    {
        foreach (var state in myStatesList.states)
        {
            foreach (var provinceNum in state.provinceNumbers)
            {
                if (provinceNum == provinceNumber)
                {
                    return state.name;
                }
            }
        }
        return "�������� ������� �� �������";

    }

    // ����� ��� ��������� ���� ���������, �������� � ������ �������
    public string[] GetProvincesInState(string stateName)
    {
        foreach (var state in myStatesList.states)
        {
            if (state.name == stateName)
            {
                string[] provinceNames = new string[state.provinceNumbers.Length];
                for (int i = 0; i < state.provinceNumbers.Length; i++)
                {
                    provinceNames[i] = GetProvinceName(state.provinceNumbers[i]);
                }
                return provinceNames;
            }
        }
        return new string[0]; // ���������� ������ ������, ���� ������� �� �������
    }

}
