using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransporterPlatform : PlatformEnvironment
{
    [SerializeField] private Transform _targetPoint;

    public Transform TargetPoint => _targetPoint;

    public override void Accept(IEnvironmentVisitor visitor)
    {
        visitor.Visit(this);
    }
}
