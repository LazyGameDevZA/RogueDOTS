# RogueDOTS

A small library that layers a nicer authoring experience on top of Unity's [Data-Oriented Technology Stack (DOTS)](https://unity.com/dots). The main aim isn't so much great performance, but rather being able to leverage the [Entities package](https://docs.unity3d.com/Packages/com.unity.entities@latest/) when building a roguelike in Unity.

At present the library is still in development and there will likely be somewhat rapid iteration towards a stable-ish V1. The project was inspired by [TheBracket's](https://github.com/thebracket) amazing tutorial on making roguelikes using Rust that can be found [here](http://bfnightly.bracketproductions.com/rustbook/chapter_0.html). At present I'm porting some of the [RLTK_RS](https://github.com/thebracket/rltk_rs) functionality to C# in Unity as I need it so as to have a coherent set of tools built on top of previous knowledge from people far smarter than myself.

## Getting started

At present getting started is slightly involved. To see how everything in this repository works you'll have to be using Unity 2019.3.0f5 or later.

1. Open the project in Unity and make sure all packages import correctly.
2. Open the scene named "PixelPerfectRenderingScene" In the embedded package [LazyGameDevZA's RogueDOTS Terminal Renderer](/Packages/za.co.lazygamedev.rogue-dots.terminal-renderer)
3. Run from the editor.

The numpad, arrow keys or vi keys will all work for navigating the player character through the world. 