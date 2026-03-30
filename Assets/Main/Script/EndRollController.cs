using UnityEngine;
using UnityEngine.UI; // LayoutRebuilder���g�p���邽�߂ɕK�{
using UnityEngine.SceneManagement;

public class EndRollController : MiniGameBase
{
    [Header("Scroll Settings")]
    [SerializeField] private float scrollSpeed = 180f;
    [SerializeField] private RectTransform contentRect;
    [SerializeField] private float accelerationSmoothness = 10f; // ���炩��

    private bool isScrolling = false; // Start�O�͓������Ȃ�
    public float upSpeed = 7f;

    private float targetSpeedMultiplier = 1f; // �ڕW�{��
    private float currentSpeedMultiplier = 1f; // ���݂̔{��
    private float contentHeight; // ���C�A�E�g�v�Z��̐�����������ێ�

    public override void OnGameStart()
    {
        BGMPlay();
        if (contentRect == null) contentRect = GetComponent<RectTransform>();

        // 1. ���C�A�E�g�̋����Čv�Z�i��΂ɏ����Ȃ����Ɓj
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);

        // �������������擾���ăL���b�V������
        contentHeight = contentRect.sizeDelta.y;

        // 2. �J�n�ʒu�̏�����
        // �R���e���c�̏�[����ʂ̉��[�ɗ���ʒu����X�^�[�g������
        contentRect.anchoredPosition = new Vector2(0, -Screen.height);

        currentSpeedMultiplier = 1f;
        targetSpeedMultiplier = 1f;
        isScrolling = true;
    }

    protected override void OnActionStarted(float value)
    {
        // ���͂�������������{�����Z�b�g
        targetSpeedMultiplier = upSpeed;
    }

    protected override void OnActionCanceled(float value)
    {
        // �������瓙�{�ɖ߂�
        targetSpeedMultiplier = 1f;
    }

    void Update()
    {
        if (!isScrolling) return;

        // ���x�{�������炩�ɕω�������i�f���炵�������ł��j
        currentSpeedMultiplier = Mathf.Lerp(currentSpeedMultiplier, targetSpeedMultiplier, Time.deltaTime * accelerationSmoothness);

        // �ړ�����
        contentRect.anchoredPosition += Vector2.up * (scrollSpeed * currentSpeedMultiplier * Time.deltaTime);

        // 3. �I������F�R���e���c�́u�Ō���v����ʏ�[�����S�ɔ�������
        if (contentRect.anchoredPosition.y > contentHeight + Screen.height / 2)
        {
            isScrolling = false;
            OnEndRollComplete();
        }
    }

    private void OnEndRollComplete()
    {
        Debug.Log("�G���h���[���I��");
        // �����ɃV�[���J�ڂȂǂ��L�q
        SceneManager.LoadSceneAsync("Title");
    }
}