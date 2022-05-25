using System.Collections.Generic;
using PotentialRobot.UI.ColorPalettes;
using UnityEngine;

namespace PotentialRobot.UI.Style
{
    public class UIStyleManager
    {
        public Dictionary<IStyleReference, IStyle> Styles;
        public Dictionary<IStyleReference, List<IStyleComponent>> Components;
        public Dictionary<IStyle, List<IStyleReference>> StyleReferences;
        public delegate void StyleDelReferenceComponentsegate(IStyle style);

        private static UIStyleManager _instance;
        public static UIStyleManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UIStyleManager();
                    _instance.Initialize();
                }
                return _instance;
            }
        }

        public void Initialize()
        {
            Styles = new Dictionary<IStyleReference, IStyle>();
            Components = new Dictionary<IStyleReference, List<IStyleComponent>>();
            StyleReferences = new Dictionary<IStyle, List<IStyleReference>>();

            var references = Resources.LoadAll("", typeof(IStyleReference));
            if (Styles == null)
                return;
            foreach (IStyleReference reference in references)
            {
                Styles.Add(reference, reference.Default);
                AddToStyleReferences(reference.Default, reference);
            }
            ColorPaletteManager.Instance.OnPaletteChanged += OnPaletteChanged;
        }

        private void AddToStyleReferences(IStyle style, IStyleReference reference)
        {
            if (!StyleReferences.ContainsKey(style)) StyleReferences.Add(style, new List<IStyleReference>());
            if (!StyleReferences[style].Contains(reference)) StyleReferences[style].Add(reference);
        }

        private void RemoveStyleReference(IStyle style, IStyleReference reference)
        {
            if (StyleReferences.ContainsKey(style) && StyleReferences[style].Contains(reference))
                StyleReferences[style].Remove(reference);
        }

        public void OnValidateApplyStyle(IStyleComponent component, IStyleReference reference)
            => component.Apply(Styles[reference]);

        public void Subscribe(IStyleComponent component)
        {
            if (!Components.ContainsKey(component.Reference))
                Components.Add(component.Reference, new List<IStyleComponent>());
            if (Components[component.Reference].Contains(component))
                return;
            Components[component.Reference].Add(component);
            component.Apply(Styles[component.Reference]);
        }

        public void Unsubscribe(IStyleComponent component)
        {
            if (IsSubscribed(component))
                RemoveSubscribedComponent(component);
        }

        private bool IsSubscribed(IStyleComponent component)
        {
            return Components.ContainsKey(component.Reference)
                && Components[component.Reference].Contains(component);
        }

        private void RemoveSubscribedComponent(IStyleComponent component)
            => Components[component.Reference].Remove(component);

        public void ChangeStyle(IStyleReference reference, IStyle newStyle)
        {
            var currentStyle = Styles[reference];
            if (currentStyle == newStyle)
                return;
            Styles[reference] = newStyle;
            RemoveStyleReference(currentStyle, reference);
            AddToStyleReferences(newStyle, reference);
            UpdateStyle(reference, newStyle);
        }
        
        public void UpdateStyle(IStyle style)
        {
            if (!StyleReferences.ContainsKey(style))
                return;
            foreach (var r in StyleReferences[style])
                UpdateStyle(r, style);
        }

        public void UpdateStyle(IStyleReference reference, IStyle style)
        {
            if (!Components.ContainsKey(reference))
                return;
            var components = Components[reference];
            foreach (var c in components) c.Apply(style);
        }

        public void OnPaletteChanged(ColorPalette palette)
        {
            foreach(var reference in Styles.Keys)
            {
                var style = Styles[reference];
                var r = style as IUseColorPalette;
                if (r != null && r.Palette == palette)
                    UpdateStyle(reference, Styles[reference]);
            }
        }
    }
}
