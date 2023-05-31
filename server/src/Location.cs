﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class Location
{

    private static Dictionary<string, Location> locations = new()
    {
        { "intro", new IntroLocation() }
    };

    public static Location? Get(string name)
    {
        if (name != null && !name.Equals(""))
        {
            locations.TryGetValue(name, out Location? location); //We use TryGetValue to avoid looking up the key twice
            return location;
        }

        return null;
    }

    /// <summary>
    /// Adds the given location to list of locations
    /// </summary>
    public static void Add(Location location)
    {
        locations.Add(location.id, location);
    }

    //Actual class data below

    public string id = "unnamedLocation", name = "Unnamed Location", status = "The void";

    public List<Creature> creatures = new();
    public Player[] Players => creatures.Where(c => c is Player).Select(c => (Player)c).ToArray(); //.Select is used to transform each element

    public List<Exit> exits = new();

    public void Enter(Creature creature)
    {
        Utils.Log($"{creature.name} enters {name} from {creature.location}");

        creatures.Add(creature);

        creature.location = id;

        if(creature is Player)
        {
            Player player = (Player)creature;

            if (player.session == null)
            {
                Utils.Log($"Finding session for player {player._id}...");
                player.session = Session.sessions.Where(s => s.Value.playerId.Equals(player._id)).First().Value;
            }

            if(player.session != null)
                player.session.Log($"You enter {name}");
            else Utils.Log($"Player {player._id} has no session!");

            string creatureList = "Around you are:";
            foreach(Creature c in creatures)
            {
                if(c != creature) //Don't list the player
                    creatureList += $"<br>-{c.FormattedName}";
            }
            player?.session?.Log(creatureList);
        }

        OnEnter(creature);
    }

    //Maybe convert OnEnter and OnLeave to events?
    public virtual void OnEnter(Creature creature)
    {

    }

    //Leave, instead of Exit, to avoid confusion with the Exit class
    public void Leave(Creature creature)
    {
        creatures.Remove(creature);
    }

    public virtual void OnLeave(Creature creature)
    {

    }

    public virtual Input[] GetInputs(Session session, string state)
    {
        List<Input> inputs = new List<Input>();

        if(state.Equals(""))
        {
            //Dialogue
            List<Creature> dialogueCreatures = new();
            foreach (Creature creature in creatures)
                if (creature.HasDialogue) dialogueCreatures.Add(creature);
            if (dialogueCreatures.Any())
                inputs.Add(new(InputMode.Option, "talk", "Talk"));

            inputs.Add(new(InputMode.Option, "exit", "Exit"));
        }
        else
        {
            inputs.Add(new(InputMode.Option, "back", "< Back"));

            if(state.Equals("talk"))
            {
                foreach (Creature creature in creatures)
                    if (creature.HasDialogue) inputs.Add(new(InputMode.Option, creature.baseId, creature.FormattedName));
            }
            else if(state.Equals("exit"))
            {
                foreach (Exit exit in exits)
                    if(exit != null && Get(exit.location) != null)
                        inputs.Add(new(InputMode.Option, exit.location, Get(exit.location).name));
            }
        }

        return inputs.ToArray();
    }

    //We pass a ref to state so we can modify it
    public virtual void HandleInputs(Session session, ClientAction action, ref string state)
    {
        if(state.Equals(""))
        {
            if (action.action.Equals("talk"))
                state = "talk";
            else if (action.action.Equals("exit"))
                state = "exit";
        }
        else
        {
            if (action.action.Equals("back")) state = "";

            if(state.Equals("talk"))
            {
                Creature target = creatures.Where(c => c.baseId.Equals(action.action)).First();

                if(target != null)
                {
                    session.SetMenu(new Menus.DialogueMenu(target));
                }
            }
            else if(state.Equals("exit"))
            {
                if(exits.Where(e => e.location.Equals(action.action)).Any())
                {
                    session.Player.Move(action.action);
                }
            }
        }
    }

}