/* !!auto gen do not change
 
 */

using UnityEngine;
using System.Collections;

namespace Table
{
    public class TableLoad
    {
/*
        public static void LoadFromMemory()
        {
            Character.LoadFromMemory();
            Map.LoadFromMemory();

        }
*/

        public static void LoadFromResources()
        {
            Character.LoadFromResources();
            Map.LoadFromResources();

        }

        public static void LoadFromStreaming()
        {
            Character.LoadFromStreaming();
            Map.LoadFromStreaming();

        }

        public static void Clear()
        {
            Character.Clear();
            Map.Clear();

        }
    }
}