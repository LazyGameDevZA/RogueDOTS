﻿# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [0.2.4] - 2020-02-04

### Changed

* RenderWorldSystem now supports a background colour for each of the entities

## [0.2.3] - 2020-02-04

### Changed

* FOV calculation now no longer has any artifacts

## [0.2.2] - 2020-02-04

### Added

* Tests to ensure that the initial Bresenham Circle with no diagonals state fix does actually work
* Added a seperate room structure to handle the map data

### Changed

* Bresenham Circle with no diagonals initial state is now setup correctly
* Fixed the FOV calculations causing the left and bottom edge to not be rendered
* Changed the Rect structure to not be used directly in a DynamicBuffer, but rather be a dedicated Math construct

## [0.2.1] - 2020-02-04

### Added

* Bresenham Circle with no diagonals generation as the "correct" usage for FOV calculations

### Changed

* FOV calculations now use the Bresenham Circle with no diagonals to correctly fill the unobstructed FOV


## [0.2.0] - 2020-02-03

Update focuses on starting with adding basic AI. At present monsters just shout at you when you enter their viewshed.

### Added

* Initial monster AI

### Changed

* Handling ticks based on input to drive game logic
* Used a downscaled version of the font with the intent of scaling the font up for larger window sizes
* Bumped the minimum Unity version

## [0.1.0] - 2020-01-27

Basis of this version is to merge the two packages into one. The Terminal Rendering assembly definition ensures that it doesn't have be used for rendering and can potentially be replaced by another system completely.

### Added

* Tests for the toolkit functionality that serves as helpers for FOV calculation

### Changed

* Moved the FOV calculation helpers into a Toolkit assembly
* Moved the random wrapper to the Toolkit assembly

## [0.0.3] - 2020-01-27

### Added

* Map structure to tie together various aspects of map rendering
* FOV calculation
* NativeHashSet copied from [here](https://github.com/jacksondunstan/NativeCollections/blob/master/JacksonDunstanNativeCollections/NativeHashSet.cs)

## [0.0.2] - 2020-01-25

### Added

* Random map generation with basic rooms that are interconnected by tunnels
* Expanded the movement system to handle the numpad and vi keys

## [0.0.1] - 2020-01-25

Initial version release. Basic map structure and entity tracking added