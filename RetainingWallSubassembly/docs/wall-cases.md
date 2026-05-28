# Retaining Wall Subassembly: Wall Cases

This document details the standard structural table (Type 2 through Type 12) used in the retaining wall subassembly. 

## Case Table

When operating in **Dynamic Table** or **Preset Dropdown** modes, the following dimension rules apply. All values are stored internally in metres. The table is automatically evaluated based on the stem height ($H_1$).

| Type | a | b | c | d | e | f | g | H1 | H2 |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **2** | 0.450 | 0.450 | 1.450 | 0.700 | 0.250 | 0.350 | 0.250 | 2.000 | 0.850 |
| **3** | 0.450 | 0.450 | 1.900 | 1.000 | 0.250 | 0.450 | 0.250 | 3.000 | 1.100 |
| **4** | 0.450 | 0.500 | 2.400 | 1.300 | 0.250 | 0.500 | 0.250 | 4.000 | 1.350 |
| **5** | 0.450 | 0.700 | 2.750 | 1.500 | 0.300 | 0.650 | 0.300 | 5.000 | 1.650 |
| **6** | 0.450 | 0.800 | 3.200 | 1.700 | 0.450 | 0.800 | 0.450 | 6.000 | 1.850 |
| **7** | 0.450 | 0.900 | 3.800 | 1.900 | 0.500 | 1.000 | 0.500 | 7.000 | 2.050 |
| **8** | 0.450 | 1.000 | 3.950 | 2.100 | 0.650 | 1.200 | 0.650 | 8.000 | 2.150 |
| **9** | 0.450 | 1.100 | 4.450 | 2.150 | 0.750 | 1.300 | 0.750 | 9.000 | 2.250 |
| **10**| 0.450 | 1.150 | 4.600 | 2.250 | 0.750 | 1.350 | 0.750 | 10.000| 2.300 |
| **11**| 0.450 | 1.300 | 4.850 | 2.350 | 0.750 | 1.400 | 0.750 | 11.000| 2.400 |
| **12**| 0.450 | 1.450 | 5.250 | 2.500 | 0.800 | 1.500 | 0.800 | 12.000| 2.500 |

## Selection Workflow
- In **Dynamic Table** mode, the subassembly samples the target surface elevation to determine the actual required height $H_1$, and automatically maps to the safest structural tier in this table.
- In **Preset Dropdown** mode, you manually assign a tier via the corridor region properties.
