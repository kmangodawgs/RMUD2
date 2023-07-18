﻿using ItemTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Attack
{
    public string id, name;

    public Die damage;

    public AbilityScore dmgAbilityScore, atkBonusAbilityScore;

    public Weapon? weapon;

    public int staminaCost;

    public DamageType damageType;

    int atkBonus, critThreshold;

    float critMult, lifeSteal;

    public virtual Action<Creature, Creature> execute => Execute;
    public virtual Func<Creature, List<Creature>> getTargets => GetTargets;

    public Attack(string id, string name, Die damage, DamageType damageType, int staminaCost = 2, AbilityScore? dmgAbilityScore = AbilityScore.Strength, 
        AbilityScore? atkBonusAbilityScore = AbilityScore.Dexterity, Weapon? weapon = null, int atkBonus = 0, float critMult = 2, int critThreshold = 20, float lifeSteal = 0f)
    {
        atkBonusAbilityScore ??= AbilityScore.Dexterity;
        dmgAbilityScore ??= AbilityScore.Strength;

        this.id = id;
        this.name = name;
        this.damage = damage;
        this.damageType = damageType;
        this.dmgAbilityScore = dmgAbilityScore.Value; //We use .Value because we know it's not null
        this.atkBonusAbilityScore = atkBonusAbilityScore.Value;
        this.weapon = weapon;
        this.staminaCost = staminaCost;
        this.atkBonus = atkBonus;
        this.critThreshold = critThreshold;
        this.critMult = critMult;
        this.lifeSteal = lifeSteal;
    }

    public int RollDamage(Creature attacker, Creature target)
    {
        Die die = damage.Clone();

        die.modifier += attacker.abilityScores[dmgAbilityScore];

        return die.Roll();
    }

    public int AttackBonus(Creature attacker)
    {
        return attacker.abilityScores[dmgAbilityScore] + atkBonus;
    }

    void Execute(Creature attacker, Creature target)
    {
        Utils.Log($"Executing attack {id} on {target.baseId} from {attacker.baseId}!");

        attacker.stamina -= staminaCost;

        int atkBonus = AttackBonus(attacker);
        int baseRoll = Utils.RandInt(20) + 1; //We add 1, since RandInt(20) returns a number from 0 to 19, and we want 1 to 20
        int roll = baseRoll + atkBonus;

        if (attacker is Player player)
            player.session?.Log($"{(roll - atkBonus)} + {atkBonus} = {roll} {(roll >= target.DodgeThreshold || baseRoll >= critThreshold ? "Hit!" : "Miss!")}");

        bool crit = baseRoll >= critThreshold;
        if (roll >= target.DodgeThreshold || crit)
        {
            //Hit
            //We calculate the damage so can log how much was dealt, then actually deal the damage. This ensures players who die from the attack still get the log message
            int rolledDmg = RollDamage(attacker, target);

            if(crit)
                rolledDmg = (int)Math.Round(rolledDmg * critMult);

            int damage = target.CalculateDamage(rolledDmg, damageType);
            attacker.Location?.Log($"{(crit ? "CRITICAL! " : "")}{attacker.FormattedName} hit {target.FormattedName} for {damage} " +
                $"({rolledDmg} - {target.GetDefense(damageType)}) {damageType} damage with {name}!");
            target.TakeDamage(damage, damageType, attacker);

            if(lifeSteal > 0)
            {
                int heal = (int)Math.Round(damage * lifeSteal);
                heal = attacker.Heal(heal);
                attacker.Location?.Log($"{attacker.FormattedName} healed for {heal} health from {name}!");
            }
        }
        else
        {
            //Miss
            attacker.Location?.Log($"{attacker.FormattedName} missed {target.FormattedName} with {name}!");
        }
    }

    List<Creature> GetTargets(Creature attacker)
    {
        List<Creature> targets = new();

        bool pvp = !attacker.Location?.safe ?? false; //Used to be: bool pvp = attacker.Location?.safe (dumbest line of coded I ever wrote)

        HashSet<Creature> creatures = attacker.Location?.creatures ?? new();
        IEnumerable<Creature>? attackable = creatures?.Where(c => c.attackable) ?? null;

        if (attackable != null)
        {
            if(!pvp)
                attackable = attackable.Where(c => c is not Player);

            foreach (Creature creature in attackable)
                if (creature != attacker && creature.attackable)
                    targets.Add(creature);
        }

        return targets;
    }

    public bool CanUse(Creature creature)
    {
        return creature.stamina >= staminaCost;
    }

    public virtual string Overview(Creature? creature = null, ItemHolder<Item>? item = null)
    {
        string msg = name + ":";

        msg += $" {(creature != null ? Utils.Modifier(AttackBonus(creature)) : $"+{atkBonusAbilityScore}{Utils.Modifier(atkBonus)}")} to hit.";

        Die damage = this.damage.Clone();
        damage.modifier += creature?.abilityScores[dmgAbilityScore] ?? 0;
        msg += $" Deals {damage}{(creature != null ? "" : $"+{dmgAbilityScore}")} {damageType} damage.";
        msg += $" Costs {staminaCost} stamina.";
        msg += $" Crits on a roll of {critThreshold}+ for {Math.Round(critMult, 1)}x damage.";
        
        if(lifeSteal > 0)
            msg += $" {Utils.Percent(lifeSteal)} lifesteal.";

        return msg;
    }

    public void ApplyWeapon(Weapon weapon)
    {
        this.weapon = weapon;
        id = weapon.id + "." + id;
        if(weapon.name != "") name += $" ({weapon.FormattedName})";
    }

}
