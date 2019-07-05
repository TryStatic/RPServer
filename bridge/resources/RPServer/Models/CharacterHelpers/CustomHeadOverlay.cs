﻿using System.Collections.Generic;
using GTANetworkAPI;

namespace RPServer.Models.CharacterHelpers
{
    internal class CustomHeadOverlay
    {
        private readonly Overlay[] _overlays;

        public CustomHeadOverlay()
        {
            _overlays = new[]
            {
                new Overlay(OverlayID.Blemishes, 255, 0, 0, 0),
                new Overlay(OverlayID.FacialHair, 255, 0, 0, 0),
                new Overlay(OverlayID.Eyebrows, 255, 0, 0, 0),
                new Overlay(OverlayID.Ageing, 255, 0, 0, 0),
                new Overlay(OverlayID.Makeup, 255, 0, 0, 0),
                new Overlay(OverlayID.Blush, 255, 0, 0, 0),
                new Overlay(OverlayID.Complexion, 255, 0, 0, 0),
                new Overlay(OverlayID.SunDamage, 255, 0, 0, 0),
                new Overlay(OverlayID.Lipstick, 255, 0, 0, 0),
                new Overlay(OverlayID.Freckles, 255, 0, 0, 0),
                new Overlay(OverlayID.ChestHair, 255, 0, 0, 0),
                new Overlay(OverlayID.BodyBlemishes, 255, 0, 0, 0),
                new Overlay(OverlayID.AdditionalBodyBlemishes, 255, 0, 0, 0)
            };
        }

        public Dictionary<int, HeadOverlay> Get()
        {
            var dict = new Dictionary<int, HeadOverlay>();
            foreach (var t in _overlays)
            {
                dict.Add(t.OverlayID, new HeadOverlay()
                {
                    Index = t.Index,
                    Opacity = t.Opacity,
                    Color = t.Color,
                    SecondaryColor = t.SecondaryColor
                });
            }

            return dict;
        }

        public void SetOverlay(OverlayID oID, byte index, float opacity, byte color, byte secColor)
        {
            _overlays[(int)oID].Index = index;
            _overlays[(int)oID].Opacity = opacity;
            _overlays[(int)oID].Color = color;
            _overlays[(int)oID].SecondaryColor = secColor;


        }

        public void ApplySingle(Client client, OverlayID oID)
        {
            client.SetHeadOverlay((int)oID, new HeadOverlay
            {
                Index = _overlays[(int)oID].Index,
                Opacity = _overlays[(int)oID].Opacity,
                Color = _overlays[(int)oID].Color,
                SecondaryColor = _overlays[(int)oID].SecondaryColor
            });
        }

        public void ApplyAll(Client client)
        {
            foreach (var overlay in _overlays)
            {
                client.SetHeadOverlay(overlay.OverlayID, new HeadOverlay
                {
                    Index = overlay.Index,
                    Opacity = overlay.Opacity,
                    Color = overlay.Color,
                    SecondaryColor = overlay.SecondaryColor
                });
            }
        }

        private struct Overlay
        {
            public byte OverlayID { get; }
            public byte Index { get; set;  }
            public float Opacity { get; set; }
            public byte Color { get; set; }
            public byte SecondaryColor { get; set; }

            public Overlay(OverlayID overlayID, byte index, float opacity, byte color, byte secondaryColor)
            {
                OverlayID = (byte)overlayID;
                Index = index;
                Opacity = opacity;
                Color = color;
                SecondaryColor = secondaryColor;
            }
        }
    }

    internal enum OverlayID
    {
        Blemishes = 0,
        FacialHair = 1,
        Eyebrows = 2,
        Ageing = 3,
        Makeup = 4,
        Blush = 5,
        Complexion = 6,
        SunDamage = 7,
        Lipstick = 8,
        Freckles = 9,
        ChestHair = 10,
        BodyBlemishes = 11,
        AdditionalBodyBlemishes = 12
    }
}
