using System.Collections;
using UnityEngine;

/// <summary>
/// Randomly spawns a "!" above one or both players.
/// Press B (special button) while it shows to roll a random reward.
/// </summary>
public class SpecialEffectManager : MonoBehaviour
{
    public static SpecialEffectManager Instance { get; private set; }

    public enum RandomOutcome
    {
        DamageOpponent,
        HealSelf,
        SpeedBuff,
        RapidFireBuff
    }

    [Header("Timing")]
    public float minSpawnDelay = 8f;
    public float maxSpawnDelay = 15f;
    public float claimWindowSeconds = 4f;

    [Header("Buffs")]
    public float buffDuration = 5f;
    public float speedBuffMultiplier = 1.5f;
    public float shootBuffMultiplier = 0.5f;

    private PlayerActions _playerOne;
    private PlayerActions _playerTwo;
    private bool _playerOneEffectActive;
    private bool _playerTwoEffectActive;
    private float _nextSpawnTime;
    private GameObject _playerOneMarker;
    private GameObject _playerTwoMarker;

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
        RefreshPlayerReferences();
        ScheduleNextSpawn();
    }

    private void RefreshPlayerReferences()
    {
        PlayerActions[] players = FindObjectsOfType<PlayerActions>();
        foreach (PlayerActions player in players)
        {
            RegisterPlayer(player);
        }
    }

    private void Update()
    {
        if (GameState.Instance == null) return;
        if (GameState.Instance.gameState != GameState.GameStateEnum.InMatch) return;

        if (Time.time >= _nextSpawnTime && !_playerOneEffectActive && !_playerTwoEffectActive)
        {
            SpawnRandomEffect();
        }
    }

    public void RegisterPlayer(PlayerActions player)
    {
        if (player == null) return;

        if (player.playerCount == "2") _playerTwo = player;
        else _playerOne = player;
    }

    public void TryClaim(string playerId)
    {
        if (GameState.Instance == null) return;
        if (GameState.Instance.gameState != GameState.GameStateEnum.InMatch) return;

        if (playerId == "1")
        {
            if (!_playerOneEffectActive) return;
            ClearEffect("1");
            ApplyRandomOutcome("1");
            return;
        }

        if (playerId == "2")
        {
            if (!_playerTwoEffectActive) return;
            ClearEffect("2");
            ApplyRandomOutcome("2");
        }
    }

    private void SpawnRandomEffect()
    {
        int pick = Random.Range(0, 3);

        if (pick == 0) StartEffectOnPlayer("1");
        else if (pick == 1) StartEffectOnPlayer("2");
        else
        {
            StartEffectOnPlayer("1");
            StartEffectOnPlayer("2");
        }

        ScheduleNextSpawn();
    }

    private void StartEffectOnPlayer(string playerId)
    {
        PlayerActions player = GetPlayer(playerId);
        if (player == null) return;

        if (playerId == "1")
        {
            if (_playerOneEffectActive) return;
            _playerOneEffectActive = true;
            _playerOneMarker = CreateMarker(player.transform, "!");
            StartCoroutine(ExpireEffectAfterDelay("1", claimWindowSeconds));
        }
        else
        {
            if (_playerTwoEffectActive) return;
            _playerTwoEffectActive = true;
            _playerTwoMarker = CreateMarker(player.transform, "!");
            StartCoroutine(ExpireEffectAfterDelay("2", claimWindowSeconds));
        }
    }

    private IEnumerator ExpireEffectAfterDelay(string playerId, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (playerId == "1" && _playerOneEffectActive) ClearEffect("1");
        if (playerId == "2" && _playerTwoEffectActive) ClearEffect("2");
    }

    private void ClearEffect(string playerId)
    {
        if (playerId == "1")
        {
            _playerOneEffectActive = false;
            if (_playerOneMarker != null) Destroy(_playerOneMarker);
            _playerOneMarker = null;
        }
        else
        {
            _playerTwoEffectActive = false;
            if (_playerTwoMarker != null) Destroy(_playerTwoMarker);
            _playerTwoMarker = null;
        }
    }

    private void ApplyRandomOutcome(string playerId)
    {
        RandomOutcome outcome = RollOutcome(playerId);
        string opponentId = playerId == "1" ? "2" : "1";
        PlayerActions player = GetPlayer(playerId);
        PlayerBuffs buffs = player != null ? player.GetComponent<PlayerBuffs>() : null;

        switch (outcome)
        {
            case RandomOutcome.DamageOpponent:
                GameState.Instance.TakeDamage(opponentId);
                ShowResult(player, "STRIKE!");
                break;

            case RandomOutcome.HealSelf:
                GameState.Instance.HealPlayer(playerId, 1);
                ShowResult(player, "HEAL!");
                break;

            case RandomOutcome.SpeedBuff:
                if (buffs != null) buffs.ApplySpeedBuff(buffDuration, speedBuffMultiplier);
                ShowResult(player, "SPEED!");
                break;

            case RandomOutcome.RapidFireBuff:
                if (buffs != null) buffs.ApplyShootBuff(buffDuration, shootBuffMultiplier);
                ShowResult(player, "RAPID!");
                break;
        }
    }

    private RandomOutcome RollOutcome(string playerId)
    {
        int roll = Random.Range(0, 4);

        if (roll == (int)RandomOutcome.HealSelf && !GameState.Instance.CanHeal(playerId))
        {
            roll = Random.Range(0, 3);
            if (roll >= (int)RandomOutcome.HealSelf) roll++;
        }

        return (RandomOutcome)roll;
    }

    private void ShowResult(PlayerActions player, string message)
    {
        if (player == null) return;
        StartCoroutine(ShowBriefResult(player.transform, message));
    }

    private IEnumerator ShowBriefResult(Transform playerTransform, string message)
    {
        GameObject result = CreateMarker(playerTransform, message);
        TextMesh text = result.GetComponent<TextMesh>();
        if (text != null) text.color = Color.white;

        yield return new WaitForSeconds(1f);
        Destroy(result);
    }

    private static GameObject CreateMarker(Transform parent, string label)
    {
        GameObject marker = new GameObject("SpecialEffectMarker");
        marker.transform.SetParent(parent);
        marker.transform.localPosition = new Vector3(0f, 1.4f, 0f);

        TextMesh text = marker.AddComponent<TextMesh>();
        text.text = label;
        text.fontSize = 64;
        text.characterSize = 0.08f;
        text.anchor = TextAnchor.MiddleCenter;
        text.alignment = TextAlignment.Center;
        text.color = Color.yellow;
        text.fontStyle = FontStyle.Bold;

        return marker;
    }

    private PlayerActions GetPlayer(string playerId)
    {
        return playerId == "2" ? _playerTwo : _playerOne;
    }

    private void ScheduleNextSpawn()
    {
        _nextSpawnTime = Time.time + Random.Range(minSpawnDelay, maxSpawnDelay);
    }
}
