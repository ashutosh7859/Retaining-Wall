# Retaining Wall Subassembly: Parameters

This document details all public input parameters available in the Civil 3D Properties Palette for the retaining wall subassembly.

## Core Configuration Control

| Parameter | Type | Description |
| :--- | :--- | :--- |
| **OperatingMode** | Enum | Dropdown: `DynamicTable`, `PresetDropdown`, `FullCustom`. Controls how dimensions are sourced. |
| **WallPreset** | Enum | Dropdown: `Type 2` through `Type 12`. Active only when `OperatingMode` is `PresetDropdown`. |

## Dimensions - Stem Notch & Batter

| Parameter | Type | Default | Description |
| :--- | :--- | :--- | :--- |
| **w** | Double | 0.450 m | Top stem width. |
| **x1** | Double | 0.200 m | Vertical drop to the start of the notch slope. |
| **x2** | Double | 0.150 m | Vertical drop of the notch slope. |
| **a** | Double | 0.450 m | Recess thickness at the notch bottom. |

## Dimensions - Custom Footing & Base

*(These values are only used when `OperatingMode` is `FullCustom`)*

| Parameter | Type | Default | Description |
| :--- | :--- | :--- | :--- |
| **Custom_b** | Double | 0.450 m | Stem thickness at the footing plane. |
| **Custom_c** | Double | 1.450 m | Heel projection length. |
| **Custom_d** | Double | 0.700 m | Toe projection length. |
| **Custom_e** | Double | 0.250 m | Vertical height of outer heel face. |
| **Custom_f** | Double | 0.350 m | Maximum footing depth at stem junction. |
| **Custom_g** | Double | 0.250 m | Vertical height of outer toe face. |
| **Custom_H1**| Double | 2.000 m | Stem height from ground level intercept. |
| **Custom_H2**| Double | 0.850 m | Foundation depth from GL to footing bottom. |

## Foundations - PCC Mud Mat

| Parameter | Type | Default | Description |
| :--- | :--- | :--- | :--- |
| **PccThickness** | Double | 0.150 m | Vertical thickness of lean concrete base. |
| **PccOffset** | Double | 0.150 m | Horizontal offset extension past the footing toe/heel. |

## Backfill - Filter Media

| Parameter | Type | Default | Description |
| :--- | :--- | :--- | :--- |
| **FmThickness** | Double | 0.600 m | Constant horizontal thickness of the drainage backfill zone. |
