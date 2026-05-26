using System.Collections.Generic;
using UnityEngine;

public class PNHunt : MonoBehaviour
{
    public enum HunterState { Normal, Scanning, Hunt }

    public float scanRadius = 10f, huntSpeed = 8f, arrivalThreshold = 0.2f;
    public GameObject scrollPrefab;
    private HunterState _state = HunterState.Normal;
    private List<PNPromptBehaviour> _clouds = new List<PNPromptBehaviour>();
    private int _selectedIndex = 0;
    private PNPromptBehaviour _targetCloud;
    private Rigidbody2D _rb;
    public HunterState CurrentState => _state;
    public PNGUIController guiController;
    private Animator animator;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (_state == HunterState.Hunt)
        {
            MoveToTarget();
        }
    }

    public void EnterScanning()
    {
        _state = HunterState.Scanning;
        _selectedIndex = 0;
        Time.timeScale = 0f;
        DetectClouds();
    }

    public void ExitScanning()
    {
        Time.timeScale = 1f;
        animator.SetTrigger("BackToIdle");
        ClearHighlightsAndScrolls();
        _clouds.Clear();
        _state = HunterState.Normal;
    }

    void DetectClouds()
    {
        _clouds.Clear();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, scanRadius);
        
        foreach (var col in colliders)
        {
            PNPromptBehaviour cloud = col.GetComponent<PNPromptBehaviour>();
            if (cloud != null) _clouds.Add(cloud);
        }

        if (_clouds.Count > 0)
        {
            animator.SetTrigger("IsHunting");
            _clouds.Sort((a, b) => 
                Vector2.Distance(transform.position, a.transform.position)
                .CompareTo(Vector2.Distance(transform.position, b.transform.position)));

            foreach (var cloud in _clouds)
            {
                cloud.SpawnScroll(scrollPrefab);
            }

            _clouds[_selectedIndex].SetHighlight(true);
        }
        else
        {
            Time.timeScale = 1f;
            _state = HunterState.Normal;
        }
    }

    public void CycleTarget(int dir)
    {
        if (_clouds.Count == 0) return;
        
        _clouds[_selectedIndex].SetHighlight(false);
        _selectedIndex = (_selectedIndex + dir) % _clouds.Count;
        _clouds[_selectedIndex].SetHighlight(true);
    }

    public void EnterHunt()
    {
        if (_clouds.Count == 0 || _selectedIndex >= _clouds.Count) return;

        _targetCloud = _clouds[_selectedIndex];
        
        foreach (var cloud in _clouds)
        {
            if (cloud != _targetCloud && cloud != null) 
                cloud.DestroyScroll();
        }

        _state = HunterState.Hunt;
        Time.timeScale = 1f;
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.linearVelocity = Vector2.zero;
    }

    void MoveToTarget()
    {
        if (_targetCloud == null)
        {
            ResetPhysics();
            animator.SetTrigger("BackToIdle");
            _state = HunterState.Normal;
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, _targetCloud.transform.position, huntSpeed * Time.fixedDeltaTime);

        if (Vector2.Distance(transform.position, _targetCloud.transform.position) <= arrivalThreshold)
        {
            _targetCloud.DestroyScroll();
            Destroy(_targetCloud.gameObject);
            PlayerPrefs.SetInt("HuntedPrompts", PlayerPrefs.GetInt("HuntedPrompts") + 1);
            guiController.setPrompt();
            ResetPhysics();
            animator.SetTrigger("BackToIdle");
            _state = HunterState.Normal;
        }
    }

    public void CancelHunt()
    {
        if (_targetCloud != null) _targetCloud.DestroyScroll();
        _targetCloud = null;
        ResetPhysics();
        _state = HunterState.Normal;
    }

    void ResetPhysics()
    {
        _rb.bodyType = RigidbodyType2D.Dynamic;
    }

    void ClearHighlightsAndScrolls()
    {
        foreach (var cloud in _clouds)
        {
            if (cloud != null)
            {
                cloud.SetHighlight(false);
                cloud.DestroyScroll();
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = _state == HunterState.Scanning ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, scanRadius);
    }
}