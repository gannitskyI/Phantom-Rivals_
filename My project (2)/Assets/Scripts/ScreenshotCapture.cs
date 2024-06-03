using UnityEngine;

public class ScreenshotCapture : MonoBehaviour
{
    public string screenshotPath = "Screenshots"; // ���� ��� ���������� ���������� (����� ������ ������������)

    private void Update()
    {
        // ���������, ���� �� ������ ������� S ��� ������ ���������
        if (Input.GetKeyDown(KeyCode.S))
        {
            CaptureScreenshot();
        }
    }

    void CaptureScreenshot()
    {
        // ������� ��� ����� ��������� �� ������ ������� ���� � �������
        string timestamp = System.DateTime.Now.ToString("yyyyMMddHHmmss");
        string screenshotName = "Screenshot_" + timestamp + ".png";

        // �������� ������ ���� � ����� ���������
        string fullPath = System.IO.Path.Combine(Application.dataPath, screenshotPath, screenshotName);

        // ������� ����� ��� ����������, ���� ��� �� ����������
        System.IO.Directory.CreateDirectory(System.IO.Path.Combine(Application.dataPath, screenshotPath));

        // ����������� ������� ���� � ��������� ��� ��� ����������� PNG
        ScreenCapture.CaptureScreenshot(fullPath);

        // ������� ��������� � ������� � ���������� ���������
        Debug.Log("�������� ��������: " + fullPath);
    }
}