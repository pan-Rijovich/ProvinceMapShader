using UnityEngine;

public class Provinces : MonoBehaviour
{
    // Переменная для хранения текстового файла JSON
    public TextAsset textJSON;

    // Класс для представления одной провинции
    [System.Serializable]
    public class Province
    {
        // Номер провинции
        public int provinceNumber;
        // Цвет провинции в формате Color32
        public Color32 provinceColor;
    }

    // Вспомогательный класс для десериализации данных JSON
    [System.Serializable]
    public class ProvinceData
    {
        // Номер провинции
        public int provinceNumber;
        // Цвет провинции в формате массива из 3 целых чисел (RGB)
        public int[] provinceColor;
    }

    // Класс для представления списка провинций
    [System.Serializable]
    public class ProvincesList
    {
        // Массив объектов ProvinceData
        public ProvinceData[] provinces;
    }

    // Экземпляр класса ProvincesList для хранения данных из JSON
    public ProvincesList myProvincesList = new ProvincesList();
    // Массив объектов Province для хранения окончательных данных
    public Province[] provinces;

    void Start()
    {
        // Десериализация данных из JSON в объект myProvincesList
        myProvincesList = JsonUtility.FromJson<ProvincesList>(textJSON.text);

        // Создание массива provinces размером равным количеству провинций в myProvincesList
        provinces = new Province[myProvincesList.provinces.Length];

        // Цикл для преобразования данных из myProvincesList в массив объектов Province
        for (int i = 0; i < myProvincesList.provinces.Length; i++)
        {
            // Преобразование массива цветов в Color32
            Color32 color = new Color32(
                (byte)myProvincesList.provinces[i].provinceColor[0], // Красный компонент
                (byte)myProvincesList.provinces[i].provinceColor[1], // Зеленый компонент
                (byte)myProvincesList.provinces[i].provinceColor[2], // Синий компонент
                255); // Прозрачность (полностью непрозрачный)

            // Создание нового объекта Province и инициализация его значениями из myProvincesList
            provinces[i] = new Province
            {
                provinceNumber = myProvincesList.provinces[i].provinceNumber, // Установка номера провинции
                provinceColor = color // Установка цвета провинции
            };
        }
    }
}
