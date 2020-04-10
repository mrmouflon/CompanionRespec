namespace CompanionRespec
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using TaleWorlds.Core;
    using TaleWorlds.Library;
    using TaleWorlds.SaveSystem;
    using TaleWorlds.InputSystem;
    using TaleWorlds.Localization;
    using TaleWorlds.MountAndBlade;
    using TaleWorlds.CampaignSystem;

    public class MySubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {

        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            if (game.GameType is Campaign && gameStarterObject is CampaignGameStarter campaignGameStarter)
            {
                campaignGameStarter.AddBehavior(new CompanionRespecBehavior());
            }
        }

    }
}
