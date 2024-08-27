using UnityEngine;

public class WorldMapIputSystem : MonoBehaviour
{
    public Camera cam; // Камера, с которой будем работать
    public Texture2D mapTexture; // Исходная текстура карты
    public States statesData; // Скрипт с данными областей 

    private Texture2D editableTexture; // Редактируемая копия текстуры карты
    private bool hasClickedThisFrame = false; // Флаг для отслеживания нажатия мыши в текущем кадре

    void Start()
    {
        if (cam == null)
        {
            // Если камера не указана, используем основную камеру сцены
            cam = Camera.main;
        }

        // Создаем редактируемую копию текстуры карты
        editableTexture = Instantiate(mapTexture);

        // Проверяем, есть ли компонент Renderer
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.mainTexture = editableTexture; // Устанавливаем текстуру на объекте
        }
        else
        {
            Debug.LogError("Missing Renderer component on the GameObject.");
        }
    }

    void Update()
    {
        // Если уже было нажатие мыши в этом кадре, выходим
        if (hasClickedThisFrame)
            return;

        // Проверяем, была ли нажата кнопка мыши
        if (Input.GetMouseButtonDown(0))
        {
            hasClickedThisFrame = true; // Устанавливаем флаг, что было нажатие мыши в этом кадре

            // Преобразуем позицию мыши в луч
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Проверяем пересечение луча с объектами на сцене
            if (Physics.Raycast(ray, out hit))
            {
                // Преобразуем координаты пересечения в координаты текстуры
                Vector2 pixelUV = hit.textureCoord;
                pixelUV.x *= editableTexture.width;
                pixelUV.y *= editableTexture.height;

                // Получаем цвет пикселя, на который нажали
                Color clickedColor = editableTexture.GetPixel((int)pixelUV.x, (int)pixelUV.y);

                // Ищем провинцию по цвету
                int provinceNumber = GetProvinceNumberByColor(clickedColor);

                // Если провинция найдена, выводим информацию о ней
                if (provinceNumber != -1)
                {
                    string provinceName = statesData.GetProvinceName(provinceNumber);
                    string stateName = statesData.GetStateName(provinceNumber);
                    Debug.Log("Название области: " + stateName + ", " + provinceName);
                }
                else
                {
                    Debug.Log("Провинция не найдена.");
                }
            }
        }

        // Сбрасываем флаг, чтобы в следующем кадре снова могла обрабатываться новая итерация нажатия мыши
        hasClickedThisFrame = false;
    }

    // Метод для определения номера провинции по цвету
    int GetProvinceNumberByColor(Color color)
    {
        // Проходим по всем провинциям и сравниваем цвета
        foreach (var province in statesData.provincesData.provinces)
        {
            if (ColorsAreEqual(province.provinceColor, color))
            {
                return province.provinceNumber;
            }
        }
        return -1; // Возвращаем -1, если провинция не найдена
    }

    // Метод для точного сравнения цветов
    bool ColorsAreEqual(Color color1, Color color2)
    {
        return color1.r == color2.r && color1.g == color2.g && color1.b == color2.b && color1.a == color2.a;
    }
}
