using UnityEngine;
using DG.Tweening;

public class CloudMovement : MonoBehaviour
{
    public float speed = 2f; // �������� �������� ������
    public float fadeDuration = 1f; // ������������ ��������� ������
    public float leftEdgeOffset = 1f; // �������� �� ����� �������

    private float originalY; // �������� �������� Y
    private Tweener moveTween; // ������ �� ������� �������� �����������

    void Start()
    {
        // ��������� �������� �������� Y ��� ������
        originalY = transform.position.y;
    }

    void Update()
    {
        MoveCloud();
    }

    void MoveCloud()
    {
        // �������� ������� ������ � ������� �����������
        float leftEdge = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        float rightEdge = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;

        // ������� ������ ������
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        // ���������, ���� ������ ������� �� ������� ������ ������� ������, ���������� ��� � ����� �������
        if (transform.position.x > rightEdge)
        {
            FadeOutAndMove(leftEdge - leftEdgeOffset);
        }
    }

    void FadeOutAndMove(float targetX)
    {
        Renderer renderer = GetComponent<Renderer>();

        // ��������� ������� �������� �����������
        if (moveTween != null)
        {
            moveTween.Kill();
        }

        // ���������� DOTween ��� �������� ���������
        renderer.material.DOFade(0f, fadeDuration)
            .OnComplete(() => OnFadeOutComplete(targetX));
    }

    void OnFadeOutComplete(float targetX)
    {
        // ����������� �������� ���������� � �������
        Vector2 newPosition = Camera.main.ScreenToWorldPoint(new Vector3(targetX, Random.Range(0f, Screen.height), 0f));
        // ���������� �������� �������� Y
        newPosition.y = originalY;

        // ���������� DOTween ��� �������� ����������� � ������� �������������
        moveTween = transform.DOMove(newPosition, 0f)
            .OnComplete(() => OnMoveComplete());
    }

    void OnMoveComplete()
    {
        Renderer renderer = GetComponent<Renderer>();

        // ���������� DOTween ��� �������� �������������� ������������
        renderer.material.DOFade(1f, fadeDuration);
    }
}
