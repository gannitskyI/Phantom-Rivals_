using UnityEngine;

public class ScreenshotCapture : MonoBehaviour
{
    public string screenshotPath = "Screenshots"; // Путь для сохранения скриншотов (папка должна существовать)

    private void Update()
    {
        // Проверяем, была ли нажата клавиша S для снятия скриншота
        if (Input.GetKeyDown(KeyCode.S))
        {
            CaptureScreenshot();
        }
    }

    void CaptureScreenshot()
    {
        // Создаем имя файла скриншота на основе текущей даты и времени
        string timestamp = System.DateTime.Now.ToString("yyyyMMddHHmmss");
        string screenshotName = "Screenshot_" + timestamp + ".png";

        // Собираем полный путь к файлу скриншота
        string fullPath = System.IO.Path.Combine(Application.dataPath, screenshotPath, screenshotName);

        // Создаем папку для скриншотов, если она не существует
        System.IO.Directory.CreateDirectory(System.IO.Path.Combine(Application.dataPath, screenshotPath));

        // Захватываем текущий кадр и сохраняем его как изображение PNG
        ScreenCapture.CaptureScreenshot(fullPath);

        // Выводим сообщение в консоль о сохранении скриншота
        Debug.Log("Скриншот сохранен: " + fullPath);
    }
}