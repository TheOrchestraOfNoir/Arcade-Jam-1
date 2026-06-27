using System.Collections;
using UnityEngine;

/// <summary>
/// Runs rhythm duels: auto or manual start, falling arrow notes, scored hits, loser takes damage.
/// </summary>
public class RhythmDuelManager : MonoBehaviour
{
    public static RhythmDuelManager Instance { get; private set; }

    public enum DuelDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    [Header("Duel length")]
    public int beatsPerDuel = 4;

    [Header("Auto duel")]
    public bool enableAutoDuels = true;
    public int autoDuelEveryBeats = 32;
    public int manualDuelCooldownBeats = 8;

    private ReadyView _readyView;
    private bool _duelActive;
    private bool _subscribedToBeat;
    private int _beatsShown;
    private int _playerOneScore;
    private int _playerTwoScore;
    private DuelDirection _currentDirection;
    private bool _playerOneScoredThisBeat;
    private bool _playerTwoScoredThisBeat;
    private int _beatsSinceLastDuel;
    private int _cooldownBeatsRemaining;
    private float _duelStartTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        _readyView = GetComponent<ReadyView>();
        StartCoroutine(SubscribeToBeatWhenReady());
    }

    private void Update()
    {
        if (!_duelActive) return;

        float maxDuelLength = beatsPerDuel * GetSecondsPerBeat() + 4f;
        if (Time.time - _duelStartTime > maxDuelLength)
        {
            EndDuel();
        }
    }

    private void OnDestroy()
    {
        UnsubscribeFromBeat();
    }

    private IEnumerator SubscribeToBeatWhenReady()
    {
        while (BeatConductor.Instance == null)
        {
            yield return null;
        }

        BeatConductor.Instance.OnBeat += HandleBeat;
        _subscribedToBeat = true;
    }

    private void UnsubscribeFromBeat()
    {
        if (!_subscribedToBeat || BeatConductor.Instance == null) return;
        BeatConductor.Instance.OnBeat -= HandleBeat;
        _subscribedToBeat = false;
    }

    public void StartDuel()
    {
        if (_duelActive) return;
        if (GameState.Instance == null) return;
        if (GameState.Instance.gameState != GameState.GameStateEnum.InMatch) return;
        if (_cooldownBeatsRemaining > 0) return;
        if (BeatConductor.Instance == null) return;

        BeginDuel();
    }

    public void ForceStop()
    {
        CancelInvoke(nameof(CloseDuelPanel));
        _duelActive = false;
        _beatsSinceLastDuel = 0;
        _cooldownBeatsRemaining = 0;

        if (_readyView != null)
        {
            _readyView.HideRhythmDuel();
        }
    }

    private void BeginDuel()
    {
        CancelInvoke(nameof(CloseDuelPanel));

        _duelActive = true;
        _beatsShown = 0;
        _playerOneScore = 0;
        _playerTwoScore = 0;
        _beatsSinceLastDuel = 0;
        _cooldownBeatsRemaining = manualDuelCooldownBeats;
        _duelStartTime = Time.time;

        PlayerDirectionInput.ResetStickMemory();
        GameState.Instance.gameState = GameState.GameStateEnum.RhythmDuel;

        if (_readyView != null)
        {
            _readyView.ShowRhythmDuel();
            _readyView.UpdateDuelScore(_playerOneScore, _playerTwoScore);
            _readyView.UpdateDuelPrompt("Get ready...");
        }
    }

    private void HandleBeat()
    {
        if (GameState.Instance == null) return;

        if (GameState.Instance.gameState == GameState.GameStateEnum.InMatch)
        {
            TrackAutoDuel();
            return;
        }

        if (!_duelActive) return;

        if (_beatsShown >= beatsPerDuel)
        {
            EndDuel();
            return;
        }

        _playerOneScoredThisBeat = false;
        _playerTwoScoredThisBeat = false;
        _currentDirection = PickRandomDirection();
        _beatsShown++;

        string arrow = DirectionToArrow(_currentDirection);
        if (_readyView != null)
        {
            _readyView.UpdateDuelPrompt(arrow);
            _readyView.SpawnDuelNote(arrow);
            _readyView.UpdateDuelScore(_playerOneScore, _playerTwoScore);
        }
    }

    private void TrackAutoDuel()
    {
        if (!enableAutoDuels) return;

        _beatsSinceLastDuel++;

        if (_cooldownBeatsRemaining > 0)
        {
            _cooldownBeatsRemaining--;
        }

        if (_beatsSinceLastDuel < autoDuelEveryBeats) return;
        if (_cooldownBeatsRemaining > 0) return;
        if (BeatConductor.Instance == null) return;

        BeginDuel();
    }

    public void TryRegisterInput(string player, int horizontal, int vertical)
    {
        if (!_duelActive) return;
        if (_beatsShown == 0) return;
        if (BeatConductor.Instance == null) return;

        BeatConductor.BeatRating timing = BeatConductor.Instance.RateInput();
        if (timing == BeatConductor.BeatRating.Miss) return;

        if (!InputMatchesDirection(horizontal, vertical, _currentDirection))
        {
            ApplyMiss(player);
            return;
        }

        if (player == "1")
        {
            if (_playerOneScoredThisBeat) return;
            _playerOneScoredThisBeat = true;
            _playerOneScore += PointsFor(timing);
            if (_readyView != null) _readyView.ShowPlayerFeedback("1", timing);
        }
        else if (player == "2")
        {
            if (_playerTwoScoredThisBeat) return;
            _playerTwoScoredThisBeat = true;
            _playerTwoScore += PointsFor(timing);
            if (_readyView != null) _readyView.ShowPlayerFeedback("2", timing);
        }
        else
        {
            return;
        }

        if (_readyView != null) _readyView.UpdateDuelScore(_playerOneScore, _playerTwoScore);
    }

    private void ApplyMiss(string player)
    {
        if (player == "1")
        {
            if (_playerOneScoredThisBeat) return;
            _playerOneScoredThisBeat = true;
            _playerOneScore -= 1;
            if (_readyView != null) _readyView.ShowPlayerFeedback("1", BeatConductor.BeatRating.Miss);
        }
        else if (player == "2")
        {
            if (_playerTwoScoredThisBeat) return;
            _playerTwoScoredThisBeat = true;
            _playerTwoScore -= 1;
            if (_readyView != null) _readyView.ShowPlayerFeedback("2", BeatConductor.BeatRating.Miss);
        }

        if (_readyView != null) _readyView.UpdateDuelScore(_playerOneScore, _playerTwoScore);
    }

    private void EndDuel()
    {
        if (!_duelActive) return;

        _duelActive = false;
        PlayerDirectionInput.ResetStickMemory();

        if (_readyView != null)
        {
            if (_playerOneScore > _playerTwoScore)
            {
                _readyView.ShowDuelResult("Player 1 wins the duel!");
                GameState.Instance.TakeDamage("2");
            }
            else if (_playerTwoScore > _playerOneScore)
            {
                _readyView.ShowDuelResult("Player 2 wins the duel!");
                GameState.Instance.TakeDamage("1");
            }
            else
            {
                _readyView.ShowDuelResult("Tie! No damage.");
            }
        }

        if (GameState.Instance.playerOneHealth > 0 && GameState.Instance.playerTwoHealth > 0)
        {
            GameState.Instance.gameState = GameState.GameStateEnum.InMatch;
            Invoke(nameof(CloseDuelPanel), 1.5f);
        }
        else if (_readyView != null)
        {
            _readyView.HideRhythmDuel();
        }
    }

    private void CloseDuelPanel()
    {
        if (GameState.Instance == null) return;
        if (GameState.Instance.gameState != GameState.GameStateEnum.InMatch) return;
        if (_readyView != null) _readyView.HideRhythmDuel();
    }

    private float GetSecondsPerBeat()
    {
        return BeatConductor.Instance != null ? BeatConductor.Instance.SecondsPerBeat : 0.5f;
    }

    private static int PointsFor(BeatConductor.BeatRating rating)
    {
        switch (rating)
        {
            case BeatConductor.BeatRating.Perfect: return 2;
            case BeatConductor.BeatRating.Good: return 1;
            case BeatConductor.BeatRating.Bad: return 0;
            default: return 0;
        }
    }

    private static DuelDirection PickRandomDirection()
    {
        return (DuelDirection)Random.Range(0, 4);
    }

    private static bool InputMatchesDirection(int horizontal, int vertical, DuelDirection direction)
    {
        switch (direction)
        {
            case DuelDirection.Up: return vertical == 1 && horizontal == 0;
            case DuelDirection.Down: return vertical == -1 && horizontal == 0;
            case DuelDirection.Left: return horizontal == -1 && vertical == 0;
            case DuelDirection.Right: return horizontal == 1 && vertical == 0;
            default: return false;
        }
    }

    private static string DirectionToArrow(DuelDirection direction)
    {
        switch (direction)
        {
            case DuelDirection.Up: return "↑";
            case DuelDirection.Down: return "↓";
            case DuelDirection.Left: return "←";
            case DuelDirection.Right: return "→";
            default: return "?";
        }
    }
}
