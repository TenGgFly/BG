using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BG
{
    public class memory
    {
        public static Dictionary<int, string> ItemState = new Dictionary<int, string>();

        public static void State(int id,string item)
        {
            if (ItemState.ContainsKey(id))
            {
                ItemState[id] = item;
            }
            else
            {
                ItemState.Add(id, item);
            }
        }
    }
}
