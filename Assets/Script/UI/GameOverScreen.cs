using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private Transform _container;
    [SerializeField] private TMP_Text _endText;
    [SerializeField] private TMP_Text _buttonText;
    [SerializeField] private Button _nextLevelButton;

    private bool _won;

    private void OnEnable()
    {
        _nextLevelButton.onClick.AddListener(OnNextLevelClicked);
    }

    private void OnDisable()
    {
        _nextLevelButton.onClick.RemoveAllListeners();
    }

    public void Initialize()
    {
        _container.localScale = Vector3.zero;
    }

    public void Show(bool isWin)
    {
        _won = isWin;
        _container.DOScale(Vector3.one, .5f).SetEase(Ease.OutBack);
        _endText.text = isWin ? "You win!" : "You lose!";
        _buttonText.text = isWin ? "Next level" : "Try again";
    }

    public void Dispose()
    {
        _container.DOKill();
    }
    
    private void OnNextLevelClicked()
    {
        GameController.Instance.Reinitialize(_won);
    }
}