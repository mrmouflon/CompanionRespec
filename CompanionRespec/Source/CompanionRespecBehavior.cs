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



namespace CompanionRespec
{
    class CompanionRespecBehavior : CampaignBehaviorBase
    {

        public override void RegisterEvents()
        {
            CampaignEvents.NewCompanionAdded.AddNonSerializedListener(this, new Action<Hero>(this.OnCompanionAdded));
            CampaignEvents.CompanionRemoved.AddNonSerializedListener(this, new Action<Hero>(this.OnCompanionRemoved));
        }
        
        public Dictionary<Hero, CampaignTime> ScatteredCompanions = new Dictionary<Hero, CampaignTime>();
        public Dictionary<Hero, CampaignTime> FiredCompanions = new Dictionary<Hero, CampaignTime>();
        
        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData<Dictionary<Hero, CampaignTime>>("ScatteredCompanions", ref this.ScatteredCompanions);
            dataStore.SyncData<Dictionary<Hero, CampaignTime>>("FiredCompanions", ref this.FiredCompanions);
        }
        
        private void OnCompanionAdded(Hero hero)
        {
            if (hero != null && hero.IsWanderer);
            {
                if (this.FiredCompanions.ContainsKey(hero) || this.ScatteredCompanions.ContainsKey(hero))
                {
                    InformationManager.DisplayMessage(new InformationMessage($"{hero.Name} cannot be demolished again."));
                    return;
                }
                this.RespecHero(hero);
            }
        }

        private void OnCompanionRemoved(Hero hero)
        {
            if (!hero.IsDead)
                {
                if (!this.FiredCompanions.ContainsKey(hero))
                {
                    this.FiredCompanions.Add(hero, CampaignTime.Now);
                    //  InformationManager.DisplayMessage(new InformationMessage($"{hero} has been fired."));
                    if (this.FiredCompanions.ContainsKey(hero))
                    {
                    //  InformationManager.DisplayMessage(new InformationMessage($"{hero} is a FiredCompanion."));
                    }
                    return;
                }
                this.FiredCompanions[hero] = CampaignTime.Now;
                //  InformationManager.DisplayMessage(new InformationMessage($"{hero} has already been fired."));
            }
        }

        private void RespecHero(Hero hero)
        {
            InformationManager.DisplayMessage(new InformationMessage($"{hero.Name} will be demolished."));
            
            InformationManager.DisplayMessage(new InformationMessage($"Clearing Perks..."));
            hero.ClearPerks();
            
            // re-initialize skills to activate perks
            foreach (SkillObject skill in DefaultSkills.GetAllSkills())
            {
                int skill_level = hero.GetSkillValue(skill);
                hero.HeroDeveloper.SetInitialSkillLevel(skill, skill_level);
            }

            int statpoints = hero.HeroDeveloper.UnspentAttributePoints;
            int focuspoints = hero.HeroDeveloper.UnspentFocusPoints;
            int focus_to_add = 0;
            int statpoints_to_add = 0;

            InformationManager.DisplayMessage(new InformationMessage($"Unspent: {statpoints} stat | {focuspoints} focus"));
            
            InformationManager.DisplayMessage(new InformationMessage($"Demolishing focus..."));

            foreach (SkillObject skill in DefaultSkills.GetAllSkills())
            {

                int focus_in_skill = hero.HeroDeveloper.GetFocus(skill);
                
                if (focus_in_skill > 0) 
                { 
                InformationManager.DisplayMessage(new InformationMessage($"{skill.Name}; {focus_in_skill}"));
                focus_to_add += focus_in_skill;
                }
            }

            InformationManager.DisplayMessage(new InformationMessage($"{focus_to_add} focus points reclaimed"));
            hero.HeroDeveloper.UnspentFocusPoints += MBMath.ClampInt(focus_to_add, 0, 999);

            hero.HeroDeveloper.ClearFocuses();

            InformationManager.DisplayMessage(new InformationMessage($"Demolishing stats..."));

            for (CharacterAttributesEnum statEnum = CharacterAttributesEnum.Vigor; statEnum < CharacterAttributesEnum.NumCharacterAttributes; statEnum++)
            {
                int attributeValue = hero.GetAttributeValue(statEnum);
                InformationManager.DisplayMessage(new InformationMessage($"{statEnum} {attributeValue} --> 0"));
                statpoints_to_add += attributeValue;
                hero.SetAttributeValue(statEnum, 0);

            }

            InformationManager.DisplayMessage(new InformationMessage($"{statpoints_to_add} stat points reclaimed"));
            

            hero.HeroDeveloper.UnspentAttributePoints += MBMath.ClampInt(statpoints_to_add, 0, 999);

            
            InformationManager.DisplayMessage(new InformationMessage($"Unspent: {hero.HeroDeveloper.UnspentAttributePoints} stat | {hero.HeroDeveloper.UnspentFocusPoints} focus"));

        }
        [SaveableField(104)]
        private List<PerkObject> _openedPerks = new List<PerkObject>();
    }
            
}
