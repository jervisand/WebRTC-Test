using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;

namespace Nagih
{
    [Serializable]
    public class CollisionEvent : UnityEvent<Collision> { }

    [Serializable]
    public class ColliderEvent : UnityEvent<Collider> { }

    [Serializable]
    public class IntEvent : UnityEvent<int> { }

    [Serializable]
    public class StringEvent : UnityEvent<string> { }
}
