using UnityEngine;

public class Provinces : MonoBehaviour
{
    // ���������� ��� �������� ���������� ����� JSON
    public TextAsset textJSON;

    // ����� ��� ������������� ����� ���������
    [System.Serializable]
    public class Province
    {
        // ����� ���������
        public int provinceNumber;
        // ���� ��������� � ������� Color32
        public Color32 provinceColor;
    }

    // ��������������� ����� ��� �������������� ������ JSON
    [System.Serializable]
    public class ProvinceData
    {
        // ����� ���������
        public int provinceNumber;
        // ���� ��������� � ������� ������� �� 3 ����� ����� (RGB)
        public int[] provinceColor;
    }

    // ����� ��� ������������� ������ ���������
    [System.Serializable]
    public class ProvincesList
    {
        // ������ �������� ProvinceData
        public ProvinceData[] provinces;
    }

    // ��������� ������ ProvincesList ��� �������� ������ �� JSON
    public ProvincesList myProvincesList = new ProvincesList();
    // ������ �������� Province ��� �������� ������������� ������
    public Province[] provinces;

    void Start()
    {
        // �������������� ������ �� JSON � ������ myProvincesList
        myProvincesList = JsonUtility.FromJson<ProvincesList>(textJSON.text);

        // �������� ������� provinces �������� ������ ���������� ��������� � myProvincesList
        provinces = new Province[myProvincesList.provinces.Length];

        // ���� ��� �������������� ������ �� myProvincesList � ������ �������� Province
        for (int i = 0; i < myProvincesList.provinces.Length; i++)
        {
            // �������������� ������� ������ � Color32
            Color32 color = new Color32(
                (byte)myProvincesList.provinces[i].provinceColor[0], // ������� ���������
                (byte)myProvincesList.provinces[i].provinceColor[1], // ������� ���������
                (byte)myProvincesList.provinces[i].provinceColor[2], // ����� ���������
                255); // ������������ (��������� ������������)

            // �������� ������ ������� Province � ������������� ��� ���������� �� myProvincesList
            provinces[i] = new Province
            {
                provinceNumber = myProvincesList.provinces[i].provinceNumber, // ��������� ������ ���������
                provinceColor = color // ��������� ����� ���������
            };
        }
    }
}
