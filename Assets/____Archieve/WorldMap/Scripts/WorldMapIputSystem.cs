using UnityEngine;

public class WorldMapIputSystem : MonoBehaviour
{
    public Camera cam; // ������, � ������� ����� ��������
    public Texture2D mapTexture; // �������� �������� �����
    public States statesData; // ������ � ������� �������� 

    private Texture2D editableTexture; // ������������� ����� �������� �����
    private bool hasClickedThisFrame = false; // ���� ��� ������������ ������� ���� � ������� �����

    void Start()
    {
        if (cam == null)
        {
            // ���� ������ �� �������, ���������� �������� ������ �����
            cam = Camera.main;
        }

        // ������� ������������� ����� �������� �����
        editableTexture = Instantiate(mapTexture);

        // ���������, ���� �� ��������� Renderer
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.mainTexture = editableTexture; // ������������� �������� �� �������
        }
        else
        {
            Debug.LogError("Missing Renderer component on the GameObject.");
        }
    }

    void Update()
    {
        // ���� ��� ���� ������� ���� � ���� �����, �������
        if (hasClickedThisFrame)
            return;

        // ���������, ���� �� ������ ������ ����
        if (Input.GetMouseButtonDown(0))
        {
            hasClickedThisFrame = true; // ������������� ����, ��� ���� ������� ���� � ���� �����

            // ����������� ������� ���� � ���
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // ��������� ����������� ���� � ��������� �� �����
            if (Physics.Raycast(ray, out hit))
            {
                // ����������� ���������� ����������� � ���������� ��������
                Vector2 pixelUV = hit.textureCoord;
                pixelUV.x *= editableTexture.width;
                pixelUV.y *= editableTexture.height;

                // �������� ���� �������, �� ������� ������
                Color clickedColor = editableTexture.GetPixel((int)pixelUV.x, (int)pixelUV.y);

                // ���� ��������� �� �����
                int provinceNumber = GetProvinceNumberByColor(clickedColor);

                // ���� ��������� �������, ������� ���������� � ���
                if (provinceNumber != -1)
                {
                    string provinceName = statesData.GetProvinceName(provinceNumber);
                    string stateName = statesData.GetStateName(provinceNumber);
                    Debug.Log("�������� �������: " + stateName + ", " + provinceName);
                }
                else
                {
                    Debug.Log("��������� �� �������.");
                }
            }
        }

        // ���������� ����, ����� � ��������� ����� ����� ����� �������������� ����� �������� ������� ����
        hasClickedThisFrame = false;
    }

    // ����� ��� ����������� ������ ��������� �� �����
    int GetProvinceNumberByColor(Color color)
    {
        // �������� �� ���� ���������� � ���������� �����
        foreach (var province in statesData.provincesData.provinces)
        {
            if (ColorsAreEqual(province.provinceColor, color))
            {
                return province.provinceNumber;
            }
        }
        return -1; // ���������� -1, ���� ��������� �� �������
    }

    // ����� ��� ������� ��������� ������
    bool ColorsAreEqual(Color color1, Color color2)
    {
        return color1.r == color2.r && color1.g == color2.g && color1.b == color2.b && color1.a == color2.a;
    }
}
