﻿using Items;
using ItemTypes;

namespace Creatures
{
    public static class MonsterList
    {

        public delegate Creature MonsterEntry(bool actual = true);

        public static readonly Table<MonsterEntry> MONSTERS = new(

            //Zombie
            new(1, (actual) => new SimpleMonster("zombie", "Zombie", 5, new("", "",
                new Attack[]
                {
                    new("bite", "Bite", 6, DamageType.Piercing),
                    new("punch", "Punch", 4, DamageType.Bludgeoning, atkBonus: 3),
                    new("bodyslam", "Body Slam", 4, DamageType.Bludgeoning, critThreshold: 18)
                }),
                drops: new(
                    new KeyValuePair<float, Func<ItemHolder<Item>>>(1, () => new("rottenflesh"))
                ),
                xp: 20,
                scaleTableWeight: (floor) =>
                {
                    return 1.2f - floor.temperature - floor.Depth * 0.4f;
                },
                actual: actual //Breaks if we don't have this. It's used in dungeon generation
            )),

            //Skeleton
            new(1, (actual) => new SimpleMonster("skeleton", "Skeleton", 5, ItemList.Get<Weapon>("spear"),
                drops: new(
                    new KeyValuePair<float, Func<ItemHolder<Item>>>(1, () => new("bone"))
                ), minDrops: 1, maxDrops: 3,
                xp: 25,
                scaleTableWeight: (floor) =>
                {
                    return 1.2f - floor.Depth * 0.4f;
                },
                resistances: new()
                {
                    { DamageType.Piercing, 3 },
                    { DamageType.Bludgeoning, -2 },
                },
                actual: actual //Breaks if we don't have this. It's used in dungeon generation
            )),

            //Giant Rat
            new(1, (actual) => new SimpleMonster("giantrat", "Giant Rat", 4, new("bite", "Bite", 4, DamageType.Piercing),
                drops: new(
                    new KeyValuePair<float, Func<ItemHolder<Item>>>(1, () => new("meat"))
                ),
                xp: 15,
                scaleTableWeight: (floor) =>
                {
                    return 1.5f - floor.Depth * 0.4f - Math.Abs(floor.temperature);
                },
                actual: actual //Breaks if we don't have this. It's used in dungeon generation
            )),

            //Slime
            new(1, (actual) => new SimpleMonster("slime", "Slime", 7, new("ooze", "Ooze", 4, DamageType.Poison, abilityScore: AbilityScore.Constitution),
                drops: new(
                    new KeyValuePair<float, Func<ItemHolder<Item>>>(1, () => new("slime"))
                ), minDrops: 1, maxDrops: 2,
                xp: 25,
                scaleTableWeight: (floor) =>
                {
                    return 1.2f - floor.Depth * 0.4f + floor.arcane;
                },
                resistances: new()
                {
                    { DamageType.Piercing, 4 },
                    { DamageType.Slashing, 2 },
                },
                actual: actual //Breaks if we don't have this. It's used in dungeon generation
            )),

            //Troll
            new(1, (actual) => new SimpleMonster("troll", "Troll", 15, new("punch", "Punch", 8, DamageType.Bludgeoning),
                drops: new(
                    new KeyValuePair<float, Func<ItemHolder<Item>>>(1, () => new("taintedflesh"))
                ), minDrops: 1, maxDrops: 2,
                xp: 75,
                onTick: (data) =>
                {
                    if (Utils.tickCount % 2 == 0)
                        data.self.Heal(1);
                },
                scaleTableWeight: (floor) =>
                {
                    return 0.8f + (floor.Depth - 2) * 0.4f;
                },
                resistances: new()
                {
                    { DamageType.Fire, -5 }
                },
                actual: actual //Breaks if we don't have this. It's used in dungeon generation
            )),

            //Blood Slime
            new(1, (actual) => new SimpleMonster("bloodslime", "Blood Slime", 10,
                new(new Attack[] { new("ooze", "Vampiric Ooze", 6, DamageType.Poison, dmgAbilityScore: AbilityScore.Constitution, lifeSteal: .35f) }),
                drops: new(
                    new KeyValuePair<float, Func<ItemHolder<Item>>>(1, () => new("slime")),
                    new KeyValuePair<float, Func<ItemHolder<Item>>>(0.5f, () => new("vampiricdust"))
                ), minDrops: 1, maxDrops: 2,
                xp: 50,
                scaleTableWeight: (floor) =>
                {
                    return 1.2f - Math.Abs(floor.Depth - 2) * 0.4f + floor.arcane - floor.holy;
                },
                resistances: new()
                {
                    { DamageType.Piercing, 5 },
                    { DamageType.Slashing, 3 },
                },
                constitution: 1,
                actual: actual //Breaks if we don't have this. It's used in dungeon generation
            )),

            //Twig Blight
            new(1, (actual) => new SimpleMonster("twigblight", "Twig Blight", 4, new("stab", "Stab", 6, DamageType.Piercing),
                drops: new(
                    new KeyValuePair<float, Func<ItemHolder<Item>>>(1, () => new("log"))
                ), minDrops: 1, maxDrops: 2,
                xp: 20,
                scaleTableWeight: (floor) =>
                {
                    return 1.2f - floor.Depth * 0.4f + floor.arcane;
                },
                resistances: new()
                {
                    { DamageType.Fire, -2 }
                },
                actual: actual //Breaks if we don't have this. It's used in dungeon generation
            )),

            //Oak Blight
            new(1, (actual) => new SimpleMonster("oakblight", "Oak Blight", 8, new("stab", "Stab", 6, DamageType.Piercing),
                drops: new(
                    new KeyValuePair<float, Func<ItemHolder<Item>>>(1, () => new("log"))
                ), minDrops: 2, maxDrops: 3,
                xp: 40,
                scaleTableWeight: (floor) =>
                {
                    return 0.8f - floor.Depth * 0.4f + floor.arcane;
                },
                resistances: new()
                {
                    { DamageType.Fire, -1 }
                },
                strength: 1,
                actual: actual //Breaks if we don't have this. It's used in dungeon generation
            )),

            //Fungus Zombie
            new(1, (actual) => new SimpleMonster("funguszombie", "Fungus Zombie", 12, new("punch", "Punch", 8, DamageType.Poison),
                drops: new(
                    new KeyValuePair<float, Func<ItemHolder<Item>>>(1, () => new("taintedflesh")),
                    new KeyValuePair<float, Func<ItemHolder<Item>>>(1, () => new("spore"))
                ), minDrops: 2, maxDrops: 3,
                xp: 60,
                scaleTableWeight: (floor) =>
                {
                    return 1.2f - Math.Abs(floor.Depth - 2) * 0.3f - Math.Abs(floor.temperature);
                },
                resistances: new()
                {
                    { DamageType.Fire, -1 }
                },
                strength: 3,
                dexterity: -1,
                constitution: 1,
                actual: actual //Breaks if we don't have this. It's used in dungeon generation
            )),

            //Fungus Troll
            new(100, (actual) => new SimpleMonster("fungustroll", "Fungus Troll", 25,
                new(new Attack[]
                {
                    new("slam", "Slam", 12, DamageType.Bludgeoning, critThreshold: 19),
                    new("fungusbreath", "Fungus Breath", 12, DamageType.Poison, atkBonus: 5)
                }),
                drops: new(
                    new KeyValuePair<float, Func<ItemHolder<Item>>>(1, () => new("taintedflesh")),
                    new KeyValuePair<float, Func<ItemHolder<Item>>>(1, () => new("spore"))
                ), minDrops: 2, maxDrops: 3,
                xp: 125,
                onTick: (data) =>
                {
                    if (Utils.tickCount % 2 == 0)
                        data.self.Heal(2);
                },
                scaleTableWeight: (floor) =>
                {
                    return 0.8f - Math.Abs(floor.Depth - 3) * 0.4f - Math.Abs(floor.temperature);
                },
                resistances: new()
                {
                    { DamageType.Fire, -3 }
                },
                actual: actual //Breaks if we don't have this. It's used in dungeon generation
            )),

            //Ogre
            new(1, (actual) => new SimpleMonster("ogre", "Ogre", 25,
                new(new Attack[]
                {
                    new("slam", "Slam", 12, DamageType.Bludgeoning, critThreshold: 18),
                    new("groundpound", "Ground Pound", 8, DamageType.Thunder, critThreshold: 16)
                }),
                drops: new(
                    new KeyValuePair<float, Func<ItemHolder<Item>>>(1, () => new("meat")),
                    new KeyValuePair<float, Func<ItemHolder<Item>>>(0.5f, () => new("boneclub"))
                ), minDrops: 1, maxDrops: 2,
                xp: 125,
                scaleTableWeight: (floor) =>
                {
                    return 0.8f - Math.Abs(floor.Depth - 3) * 0.4f;
                },
                actual: actual //Breaks if we don't have this. It's used in dungeon generation
            )),

            //Shadowcat
            new(1, (actual) => new SimpleMonster("shadowcat", "Shadowcat", 20,
                new(new Attack[]
                {
                    new("pounce", "Pounce", 12, DamageType.Bludgeoning, critThreshold: 19, critMult: 3),
                    new("slash", "Slash", 10, DamageType.Necrotic, critThreshold: 19)
                }),
                drops: new(
                    new KeyValuePair<float, Func<ItemHolder<Item>>>(1, () => new("meat")),
                    new KeyValuePair<float, Func<ItemHolder<Item>>>(1, () => new("shadowessence"))
                ), minDrops: 2, maxDrops: 3,
                xp: 100,
                scaleTableWeight: (floor) =>
                {
                    return 0.8f - Math.Abs(floor.Depth - 3) * 0.4f;
                },
                agility: 4,
                dexterity: 2,
                actual: actual //Breaks if we don't have this. It's used in dungeon generation
            )),

            //Zombie Slime
            new(1, (actual) => new SimpleMonster("zombieslime", "Zombie Slime", 20,
                new(new Attack[]
                {
                    new("ooze", "Ooze", 12, DamageType.Poison, critThreshold: 19, dmgAbilityScore: AbilityScore.Constitution, lifeSteal: .25f),
                    new("necroticgoop", "Necrotic Goop", 12, DamageType.Necrotic, critThreshold: 18, dmgAbilityScore: AbilityScore.Constitution,lifeSteal: .4f)
                }),
                drops: new(
                    new KeyValuePair<float, Func<ItemHolder<Item>>>(1, () => new("slime")),
                    new KeyValuePair<float, Func<ItemHolder<Item>>>(1, () => new("shadowessence"))
                ), minDrops: 2, maxDrops: 3,
                xp: 80,
                scaleTableWeight: (floor) =>
                {
                    return 0.8f - Math.Abs(floor.Depth - 3) * 0.4f + floor.arcane - floor.holy;
                },
                resistances: new()
                {
                    { DamageType.Poison, 1 },
                    { DamageType.Necrotic, 4 },
                    { DamageType.Radiant, -3 }
                },
                constitution: 5,
                actual: actual //Breaks if we don't have this. It's used in dungeon generation
            )),

            //Heavenly Defender
            new(1, (actual) => new SimpleMonster("heavenlydefender", "Heavenly Defender", 40,
                new(new Attack[]
                {
                    new("smite", "Smite", 12, DamageType.Radiant, atkBonus: 2, critThreshold: 19, dmgAbilityScore: AbilityScore.Wisdom),
                    new("longsword", "Longsword", 12, DamageType.Slashing, critThreshold: 19),
                    new("divineintervention", "Divine Intervention", 12, DamageType.Radiant, atkBonus: 3, critThreshold: 18, critMult: 3)
                }),
                drops: new(
                    new KeyValuePair<float, Func<ItemHolder<Item>>>(1, () => new("holyblood")),
                    new KeyValuePair<float, Func<ItemHolder<Item>>>(1, () => new("longsword"))
                ), minDrops: 1, maxDrops: 2,
                xp: 120,
                scaleTableWeight: (floor) =>
                {
                    return 0.8f - Math.Abs(floor.Depth - 4) * 0.4f + floor.holy;
                },
                resistances: new()
                {
                    { DamageType.Necrotic, -3 },
                    { DamageType.Fire, 2 },
                    { DamageType.Radiant, 12 },
                    { DamageType.Psychic, 1 },
                    { DamageType.Slashing, 2 },
                    { DamageType.Bludgeoning, 2 },
                    { DamageType.Piercing, 2}
                },
                strength: 4,
                dexterity: 2,
                wisdom: 4,
                agility: 3,
                actual: actual //Breaks if we don't have this. It's used in dungeon generation
            )),

            //Lesser Angel
            new(1, (actual) => new SimpleMonster("lesserangel", "Lesser Angel", 30,
                new(new Attack[]
                {
                    new("smite", "Smite", 10, DamageType.Radiant, atkBonus: 2, critThreshold: 19, dmgAbilityScore: AbilityScore.Wisdom),
                    new("mace", "Mace", 8, DamageType.Slashing, critThreshold: 18, critMult: 3),
                }),
                drops: new(
                    new KeyValuePair<float, Func<ItemHolder<Item>>>(1, () => new("holyblood")),
                    new KeyValuePair<float, Func<ItemHolder<Item>>>(1, () => new("mace"))
                ), minDrops: 1, maxDrops: 2,
                xp: 80,
                scaleTableWeight: (floor) =>
                {
                    return 0.8f - Math.Abs(floor.Depth - 4) * 0.3f + floor.holy;
                },
                resistances: new()
                {
                    { DamageType.Necrotic, -5 },
                    { DamageType.Radiant, 6 },
                    { DamageType.Fire, 1 }
                },
                strength: 3,
                dexterity: 2,
                wisdom: 3,
                agility: 2,
                actual: actual //Breaks if we don't have this. It's used in dungeon generation
            )),

            //Mutant Troll
            new(1, (actual) => new SimpleMonster("mutanttroll", "Mutant Troll", 50,
                new(new Attack[] {
                    new("punch", "Punch", 12, DamageType.Bludgeoning),
                    new("bite", "Bite", 10, DamageType.Poison, atkBonus: -2, critThreshold: 16),
                    new("bodyslam", "Bodyslam", new(6, 3), DamageType.Bludgeoning, critMult: 3)
                }),
                drops: new(
                    new KeyValuePair<float, Func<ItemHolder<Item>>>(1, () => new("taintedflesh")),
                    new KeyValuePair<float, Func<ItemHolder<Item>>>(1, () => new("aberrantcluster"))
                ), minDrops: 2, maxDrops: 3,
                xp: 150,
                onTick: (data) =>
                {
                    if (Utils.tickCount % 2 == 0)
                        data.self.Heal(3);
                },
                scaleTableWeight: (floor) =>
                {
                    return 0.8f + (floor.Depth - 5) * 0.4f + floor.arcane;
                },
                resistances: new()
                {
                    { DamageType.Poison, 4 },
                    { DamageType.Necrotic, 2 },
                    { DamageType.Radiant, -2 },
                    { DamageType.Fire, -1 }
                },
                actual: actual //Breaks if we don't have this. It's used in dungeon generation
            )),

            //Living Tentacle
            new(1, (actual) => new SimpleMonster("livingtentacle", "Living Tentacle", 30,
                new(new Attack[] {
                    new("slap", "Slap", 10, DamageType.Bludgeoning),
                    new("choke", "choke", 8, DamageType.Bludgeoning, critThreshold: 18, critMult: 5)
                }),
                drops: new(
                    new KeyValuePair<float, Func<ItemHolder<Item>>>(1, () => new("meat")),
                    new KeyValuePair<float, Func<ItemHolder<Item>>>(1, () => new("aberrantcluster"))
                ), minDrops: 2, maxDrops: 3,
                xp: 80,
                scaleTableWeight: (floor) =>
                {
                    return 0.8f + (floor.Depth - 4) * 0.3f + floor.arcane;
                },
                resistances: new()
                {
                    { DamageType.Poison, 6 },
                    { DamageType.Necrotic, 3 },
                    { DamageType.Radiant, -3 }
                },
                actual: actual //Breaks if we don't have this. It's used in dungeon generation
            ))

        );

    }
}