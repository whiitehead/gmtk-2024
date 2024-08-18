using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAnimator : MonoBehaviour
{
    public enum PlaybackType
    {
        PlayOnce,
        Loop,
        PingPong,
        PingPongOnce,
    }
    
    public List<Sprite> Sprites;
    public int FPS = 30;
    public PlaybackType Playback = PlaybackType.PlayOnce;
    public bool Paused = false;
    [HideInInspector] public int CurrentFrame = 0;
    
    private SpriteRenderer _spriteRenderer;
    private SpriteMask _spriteMask;
    private float _maxFrameTime; // seconds
    private float _currentFrameTime; // seconds
    private bool _isReversed;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteMask = GetComponent<SpriteMask>();
        if (_spriteMask == null && _spriteRenderer == null)
        {
            throw new Exception("Simple Animator requires either SpriteRenderer or SpriteMask");
        }
        _maxFrameTime = 1.0f / FPS;
    }
    
    public void Play() => Paused = false;
    public void Pause() => Paused = true;

    private void Update()
    {
        if (Paused)
        {
            return;
        }
        
        _currentFrameTime += Time.deltaTime;

        if (_currentFrameTime < _maxFrameTime)
        {
            return;
        }
        
        _currentFrameTime -= _maxFrameTime;

        switch (Playback)
        {
            case PlaybackType.PlayOnce:
            {
                CurrentFrame %= Sprites.Count;
                if (CurrentFrame == 0)
                {
                    Paused = true;
                }
                
                break;
            }
            case PlaybackType.Loop:
            {
                CurrentFrame %= Sprites.Count;
                
                break;
            }
            case PlaybackType.PingPong:
            {
                CurrentFrame += _isReversed ? -1 : 1;
                if (CurrentFrame == 0 || CurrentFrame == Sprites.Count - 1)
                {
                    _isReversed = !_isReversed;
                }

                break;
            }
            case PlaybackType.PingPongOnce:
            {
                CurrentFrame += _isReversed ? -1 : 1;
                if (CurrentFrame == 0)
                {
                    Paused = true;
                    _isReversed = false;
                }
                else if (CurrentFrame == Sprites.Count - 1)
                {
                    _isReversed = true;
                }

                break;
            }
        }

        if (_spriteRenderer != null)
        {
            _spriteRenderer.sprite = Sprites[CurrentFrame];
        }

        if (_spriteMask != null)
        {
            _spriteMask.sprite = Sprites[CurrentFrame];
        }
    }
}
