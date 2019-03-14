using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Ruby
{
    class Loader
    {
		private static Main MainObj;
        public static void Load()
        {
			MainObj = new Main();
			MainObj.Load();
        }
		
		public static void Unload()
		{
			MainObj.Unload();
		}
    }
}
