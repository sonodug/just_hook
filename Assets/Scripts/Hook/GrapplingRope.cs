using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingRope : MonoBehaviour
{
    [Header("General References:")]
    [SerializeField] private HookType _grapplingHook;
    [SerializeField] private LineRenderer _lineRenderer;

    [Header("General Settings:")]
    [SerializeField] private int _precision = 40;

    [Range(0, 20)] [SerializeField] private float _straightenLineSpeed = 5;

    [Header("Rope Animation Settings:")]
    [SerializeField] private AnimationCurve _ropeAnimationCurve;

    [Range(0.01f, 4)] [SerializeField] private float _startWaveSize = 2;

    [Header("Rope Progression:")]
    [SerializeField] private AnimationCurve _ropeProgressionCurve;

    [Range(1, 50)] [SerializeField] private float _ropeProgressionSpeed = 1;

    [SerializeField] private bool _isGrappling = true;

    private float _waveSize = 0;
    private float _moveTime = 0;
    private bool _straightLine = true;

    public bool IsGrappling => _isGrappling;

    private void Update()
    {
        _moveTime += Time.deltaTime;
        DrawRope();
    }

    private void OnEnable()
    {
        _moveTime = 0;
        _lineRenderer.positionCount = _precision;
        _waveSize = _startWaveSize;
        _straightLine = false;

        LinePointsToFirePoint();

        _lineRenderer.enabled = true;
    }

    private void OnDisable()
    {
        _lineRenderer.enabled = false;
        _isGrappling = false;
    }

    private void LinePointsToFirePoint()
    {
        for (int i = 0; i < _precision; i++)
        {
            _lineRenderer.SetPosition(i, _grapplingHook.ShotPoint.position);
        }
    }


    private void DrawRope()
    {
        if (!_straightLine)
        {
            if (Math.Round(_lineRenderer.GetPosition(_precision - 1).x, 2) == Math.Round(_grapplingHook.GrapplePoint.x, 2))
            {
                _straightLine = true;
            }
            else
            {
                DrawRopeWaves();
            }
        }
        else
        {
            if (!_isGrappling)
            {
                _grapplingHook.Grapple();
                _isGrappling = true;
            }
            if (_waveSize > 0)
            {
                _waveSize -= Time.deltaTime * _straightenLineSpeed;
                DrawRopeWaves();
            }
            else
            {
                _waveSize = 0;

                if (_lineRenderer.positionCount != 2) { _lineRenderer.positionCount = 2; }

                DrawRopeNoWaves();
            }
        }
    }

    private void DrawRopeWaves()
    {
        for (int i = 0; i < _precision; i++)
        {
            float delta = (float)i / ((float)_precision - 1f);
            Vector2 offset = Vector2.Perpendicular(_grapplingHook.GrappleDistanceVector).normalized * _ropeAnimationCurve.Evaluate(delta) * _waveSize;
            Vector2 targetPosition = Vector2.Lerp(_grapplingHook.ShotPoint.position, _grapplingHook.GrapplePoint, delta) + offset;
            Vector2 currentPosition = Vector2.Lerp(_grapplingHook.ShotPoint.position, targetPosition, _ropeProgressionCurve.Evaluate(_moveTime) * _ropeProgressionSpeed);

            _lineRenderer.SetPosition(i, currentPosition);
        }
    }

    private void DrawRopeNoWaves()
    {
        _lineRenderer.SetPosition(0, _grapplingHook.ShotPoint.position);
        _lineRenderer.SetPosition(1, _grapplingHook.GrapplePoint);
    }
}