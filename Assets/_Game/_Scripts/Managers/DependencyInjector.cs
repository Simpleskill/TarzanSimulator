using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimplesDev.TarzanSimulator.Types;

namespace SimplesDev.TarzanSimulator.Managers
{
   public static class DependencyInjector
   {
      [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
      public static void InjectDependencies()
      {
         List<string> dependencies = new()
         {
            GameDependencies.GAME_MANAGER
         };

         dependencies.ForEach(dependency =>
         {
            GameObject.DontDestroyOnLoad(GameObject.Instantiate(Resources.Load(dependency)));
         });
      }
   }
}