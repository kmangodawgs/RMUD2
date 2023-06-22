﻿using ItemTypes;
using Menus;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class Creature
{

    static List<string> ids = new();

    //We can't use id because of Player's _id, so we use baseId
    public string baseId = "unnamedCreature", name = "Unnamed Creature", location = "";

    public Location? Location => GetLocation();

    public bool attackable = true;
    public int health;
    public virtual int MaxHealth => 5 + Constitution;

    //Null for either means creature has no dialogue. The string is the dialogue state
    public Func<Session, DialogueMenu, Input[]>? talkInputs = null;
    public Action<Session, ClientAction, DialogueMenu>? talkHandler = null;
    public Action<Session>? talkStart = null;
    public bool HasDialogue => talkInputs != null && talkHandler != null && talkStart != null;

    //Name formatting
    public string FormattedName => Utils.Style(name, nameColor, nameBold, nameUnderline, nameItalic);
    public string nameColor = "";
    public bool nameBold, nameUnderline, nameItalic;

    public ItemHolder<Item>? mainHand, offHand;
    public virtual Weapon? Weapon => mainHand?.Item as Weapon;

    public Dictionary<AbilityScore, int> abilityScores = new()
    {
        { AbilityScore.Strength, 0 },
        { AbilityScore.Dexterity, 0 },
        { AbilityScore.Constitution, 0 },
        { AbilityScore.Agility, 0 },
        { AbilityScore.Intelligence, 0 },
        { AbilityScore.Wisdom, 0 },
        { AbilityScore.Charisma, 0 }
    };

    //Ability score methods for convenience
    public int Strength => abilityScores[AbilityScore.Strength];
    public int Dexterity => abilityScores[AbilityScore.Dexterity];
    public int Constitution => abilityScores[AbilityScore.Constitution];
    public int Agility => abilityScores[AbilityScore.Agility];
    public int Intelligence => abilityScores[AbilityScore.Intelligence];
    public int Wisdom => abilityScores[AbilityScore.Wisdom];
    public int Charisma => abilityScores[AbilityScore.Charisma];

    public virtual int DodgeThreshold => 10 + Agility;
    public virtual int MaxCarryWeight => 40 + Strength * 5;

    public Inventory inventory = new();

    public Creature(string id, string name)
    {
        Utils.OnTick += Tick;

        //If multiple creatures have the same ID, add a number to the end of the ID
        int counter = 0;
        while ((counter > 0 && ids.Contains(id + counter)) || (counter == 0 && ids.Contains(id)))
            counter++;

        if (counter > 0)
        {
            id += counter;
            name += $" {counter}";
        }

        ids.Add(id);
        baseId = id;
        this.name = name;
        //Utils.Log($"ID: {baseId}, Counter: {counter}");

        CalculateStats();
    }

    //Runs during deserialization
    public Creature()
    {
        CalculateStats();
    }

    Location? GetLocation()
    {
        return Location.Get(location);
    }

    /// <summary>
    /// Move the creature to a new location
    /// </summary>
    /// <param name="location">The ID of the new location</param>
    public void Move(string location)
    {
        Location? to = Location.Get(location);

        if (!location.Equals(""))
        {
            Location? loc = GetLocation();
            loc?.Leave(this, to);
        }

        to?.Enter(this, Location);

        if(this is Player)
        {
            Player player = (Player)this;

            player?.session?.SetMenu(new LocationMenu(player.session));
        }
    }

    protected virtual void Tick(int tickCount)
    {

    }

    public void MoveThroughRandomExit()
    {
        Location? location = Location;
        if (location != null)
        {
            if (location.exits.Count > 0)
            {
                Exit exit = location.exits[Utils.RandInt(location.exits.Count)]; //Min defaults to 0 (we made an overload)
                Move(exit.location);
            }
        }
    }

    /// <summary>
    /// Attacks using the default attack with the specified weapon
    /// </summary>
    public void Attack(Creature target, Weapon weapon)
    {
        weapon?.Attack?.execute(this, target);
    }

    public int CalculateDamage(int damage)
    {
        damage = Math.Clamp(damage, 0, health);
        return damage;
    }

    public void TakeDamage(int damage, bool calculateDamage = false)
    {
        if(calculateDamage) damage = CalculateDamage(damage);
        health -= damage;
        if (health <= 0)
            Die();
    }

    void Die()
    {
        Utils.Log($"{name} died");

        Player[] players = Location.Players;
        foreach (Player player in players)
        {
            player.session?.Log($"{FormattedName} died");
        }

        OnDie();
    }

    protected virtual void OnDie()
    {
        Location?.creatures.Remove(this);
        location = "";
        Utils.OnTick -= Tick;
    }

    //There's some stuff, like max inventory weight, where there's just not a good way auto-calculate the values, so we have to do it manually here
    public virtual void CalculateStats()
    {
        inventory.maxWeight = MaxCarryWeight;
    }

}