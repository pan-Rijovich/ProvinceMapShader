using UnityEngine;

public class States : MonoBehaviour
{
    // Переменная для хранения текстового файла JSON
    public TextAsset statesJSON;

    // Класс для представления одного штата
    [System.Serializable]
    public class State
    {
        // Название штата
        public string name;
        // Номера провинций, входящих в данный штат
        public int[] provinceNumbers;
    }

    // Класс для представления списка штатов
    [System.Serializable]
    public class StatesList
    {
        // Массив объектов State
        public State[] states;
    }

    // Экземпляр класса StatesList для хранения данных из JSON
    public StatesList myStatesList = new StatesList();

    // Ссылка на класс Provinces
    public Provinces provincesData;

    void Start()
    {
        // Десериализация данных из JSON в объект myStatesList
        myStatesList = JsonUtility.FromJson<StatesList>(statesJSON.text);
    }

    // Метод для получения названия провинции по ее номеру
    public string GetProvinceName(int provinceNumber)
    {
        foreach (var state in myStatesList.states)
        {
            foreach (var provinceNum in state.provinceNumbers)
            {
                if (provinceNum == provinceNumber)
                {
                    return "провинция " + provinceNumber; // Просто возвращаем номер провинции для примера
                }
            }
        }
        return "Название провинции не найдено";
    }

    // Метод для получения названия области по номеру провинции
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
        return "Название области не найдено";

    }

    // Метод для получения всех провинций, входящих в данную область
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
        return new string[0]; // Возвращаем пустой массив, если область не найдена
    }

}
