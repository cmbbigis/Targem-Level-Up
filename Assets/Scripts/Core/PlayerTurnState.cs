using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;

namespace Core
{
    public class PlayerTurnState
    {
        public readonly HashSet<IEntity> ChosenEntities = new();

        public void StartTurn()
        {
            foreach (var e in ChosenEntities)
                e.IsChosen = true;
            foreach (var e in ChosenEntities)
                e.IsChosen = true;
        }
        
        public void EndTurn()
        {
            foreach (var e in ChosenEntities)
                e.IsChosen = false;
            foreach (var e in ChosenEntities)
                e.IsChosen = false;
        }
        
        [CanBeNull]
        public IEntity GetCurrent() => ChosenEntities.Count == 1 ? ChosenEntities.First() : null;
        
        public void AddChosenEntity(IEntity entity)
        {
            ChosenEntities.Add(entity);
            entity.IsChosen = true;
        }

        public void PopChosenEntity(IEntity entity)
        {
            ChosenEntities.Remove(entity);
            entity.IsChosen = false;
        }

        public void ClearChosenEntities()
        {
            foreach (var entity in ChosenEntities)
                entity.IsChosen = false;
            ChosenEntities.Clear();
        }

        public void SetChosenEntity(IEntity entity)
        {
            ClearChosenEntities();
            AddChosenEntity(entity);
        }
        
        public readonly HashSet<IEntity> HighlightedEntities = new();
        
        public void AddHighlightedEntity(IEntity entity)
        {
            HighlightedEntities.Add(entity);
            entity.IsHighlighted = true;
        }

        public void PopHighlightedEntity(IEntity entity)
        {
            HighlightedEntities.Remove(entity);
            entity.IsHighlighted = false;
        }

        public void ClearHighlightedEntity()
        {
            foreach (var entity in HighlightedEntities)
                entity.IsHighlighted = false;
            HighlightedEntities.Clear();
        }

        public void SetHighlightedEntity(IEntity entity)
        {
            ClearHighlightedEntity();
            AddHighlightedEntity(entity);
        }
        
        public void SetHighlightedEntities(IEnumerable<IEntity> entities)
        {
            ClearHighlightedEntity();
            foreach (var e in entities)
            {
                AddHighlightedEntity(e);
                e.IsHighlighted = true;
            }
        }
    }
}