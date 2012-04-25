using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNAKinectCatalog
{
    public enum CameraType
    {
        Type3D,
        Type2D
    };

    public enum TextWriterEffect 
    {
        Typewriter,
        Block,
        FadeOut,
        TypeWriterWithFadeOut,
        FadeIn
    };

    public enum TextWriterFormat
    {
        Continuos, // This indicates the spriteBatch is gonna  display one string completely (No need for X & Y coords)
        Fixed // This indicates a per letter positioning
    };

    /// <summary>
    ///  TODO Change this to forward, forwardright, right, etc
    /// </summary>
    public enum MovingDirection
    {
        None, North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest
    }

    public enum ItemDiscovery
    {
        Rejected = -1, NotFound = 0, Accepted = 1
    }

    public enum CassandraMood
    {
        Mad = -3, Angry = -2, Pissed = -1, Indiferent = 0, Content = 1, Happy = 2, Extatic = 3
    };

    public enum MainMenuOptions
    {
        StartGame, Options
    };

}
