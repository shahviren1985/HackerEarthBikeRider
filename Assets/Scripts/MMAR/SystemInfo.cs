using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMAR
{
    public class SystemInfo : MonoBehaviour
    {
        public static long SystemDateTimeinMills { get { return System.DateTime.Now.Ticks; } }
    }
}
