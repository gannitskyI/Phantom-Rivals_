using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

public class PanelMover : MonoBehaviour
{
    private Vector3 _startPosition;
    private bool _isMoving;

    private RectTransform _rectTransform;

    private void Awake()
    {
        _startPosition = transform.position;
        _rectTransform = GetComponent<RectTransform>();
    }

    public void MoveUp()
    {
        if (_isMoving) return;
        _isMoving = true;

        float targetY = _startPosition.y + Screen.height + _rectTransform.rect.height; // ��������� ������ ������� ��� ����, ����� �� ����� ��������� �� �����
        _rectTransform.DOAnchorPosY(targetY, 1f).SetEase(Ease.OutCubic).OnComplete(() => _isMoving = false);
        // ������� ����������� ������� ����� �� �����
    }


    public void MoveDown()
    {
        if (_isMoving) return;
        _isMoving = true;

        _rectTransform.DOAnchorPosY(0, 1f).SetEase(Ease.OutCubic).OnComplete(() => _isMoving = false);
        // ������� ����������� ������� ���� � 0
    }
}
