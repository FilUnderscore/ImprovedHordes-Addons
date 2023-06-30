using HarmonyLib;
using ImprovedHordes.Core;
using ImprovedHordes.Core.Abstractions.Logging;
using ImprovedHordes.Core.World.Event;
using ImprovedHordes.Implementations.Logging;
using UnityEngine;

namespace ImprovedHordes.AirdropHordes
{
    public sealed class IHAirdropHordesMod : IModApi
    {
        private readonly Harmony harmony;

        private readonly ILoggerFactory loggerFactory;
        private readonly Core.Abstractions.Logging.ILogger logger;

        public IHAirdropHordesMod()
        {
            this.loggerFactory = new ImprovedHordesLoggerFactory();
            this.logger = this.loggerFactory.Create(typeof(IHAirdropHordesMod));

            this.harmony = new Harmony("filunderscore.improvedhordes.airdrophordes");
            this.harmony.PatchAll();
        }

        public void InitMod(Mod _modInstance)
        {
            if(!ImprovedHordesMod.TryGetInstance(out var instance))
            {
                this.logger.Error("Failed to get Improved Hordes mod instance. It seems like Improved Hordes has not initialized yet.");
                return;
            }

            instance.OnCoreInitialized += Instance_OnCoreInitialized;
        }

        private void Instance_OnCoreInitialized(object sender, ImprovedHordes.Event.ImprovedHordesCoreInitializedEvent e)
        {
            ImprovedHordesCore core = e.GetCore();
            AIDirectorAirDropComponent_SpawnSupplyCrate_Hook.WorldEventReporter = core.GetWorldEventReporter();
        }

        [HarmonyPatch(typeof(AIDirectorAirDropComponent))]
        [HarmonyPatch(nameof(AIDirectorAirDropComponent.SpawnSupplyCrate))]
        private class AIDirectorAirDropComponent_SpawnSupplyCrate_Hook
        {
            public static WorldEventReporter WorldEventReporter;

            private static void Postfix(Vector3 spawnPos)
            {
                WorldEventReporter.Report(new WorldEvent(spawnPos, 200.0f, true));
            }
        }
    }
}
