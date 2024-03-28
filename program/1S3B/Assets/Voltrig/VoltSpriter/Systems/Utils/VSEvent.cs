using UnityEngine;

namespace Voltrig.VoltSpriter
{
    /// <summary>
    /// Adapter for the Event.
    /// </summary>
    internal class VSEvent
    {
        private Event _event;

        public EventType type
        {
            get { return _event.type; }
        }

        public string commandName
        {
            get { return _event.commandName; }
        }

        public bool control
        {
            get { return _event.control; }
        }

        public bool alt
        {
            get { return _event.alt; }
        }

        public bool shift
        {
            get { return _event.shift; }
        }

        public KeyCode keyCode
        {
            get { return _event.keyCode; }
        }

        public Vector2 mousePosition
        {
            get { return _event.mousePosition; }
        }

        public int button
        {
            get { return _event.button; }
        }

        public EventModifiers modifiers
        {
            get { return _event.modifiers; }
        }

        public VSEvent(Event target)
        {
            _event = new Event(target);
        }

        public void Use()
        {
            //Debug.Log("Using the event!");
            _event.Use();
        }

        public EventType GetTypeForControl(int id)
        {
            return _event.GetTypeForControl(id);
        }
    }
}