﻿# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

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