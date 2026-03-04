# Railway System — Possible Cargo Solver

## Problem Description

A railway system consists of several stations connected by one-way tracks.

Each station has two associated cargo types:
- Unload type — removed from the train upon arrival
- Load type — added to the train before departure

All trains start from a designated starting station carrying no cargo and may follow any valid route along the tracks.

**Goal:** For each station, determine which cargo types might be on a train when it arrives. A cargo type is considered possible if there exists at least one route from the starting station that brings it to the station.

### Rules
- When a train arrives at a station, it first unloads the consumed cargo type, then loads the provided cargo type.
- Trains can carry multiple cargo types simultaneously.
- Cargo types are abstract labels — quantity does not matter.

### Input Format
```
S T
s c_unload c_load
...
s_from s_to
...
s_0
```

- First line: `S` (number of stations) and `T` (number of tracks)
- Next `S` lines: station id, cargo type unloaded, cargo type loaded
- Next `T` lines: directed track from `s_from` to `s_to`
- Last line: starting station id

## Algorithm

The solver uses a fixed-point BFS (worklist algorithm):

1. Enqueue the starting station. Its `PossibleLoads` (arrival cargo) is empty — the train starts with no cargo.
2. Dequeue a station. Compute departure load: take the station's `PossibleLoads`, remove its `CUnload` type, add its `CLoad` type.
3. For each neighbor: if the departure load contains cargo types not yet in the neighbor's `PossibleLoads`, add them and enqueue the neighbor (if not already queued).
4. Repeat until the queue is empty — a fixed point is reached.

### Termination guarantee
Each station's `PossibleLoads` set can only grow by union operations. The total number of distinct cargo types is finite, so each station can be re-enqueued at most `C` times (where `C` is the number of unique cargo types). The algorithm always terminates.

### Complexity
- **Time:** 
1. Station Processing Complexity: $O(C^2 \times S)$ The algorithm visits every station at most $C$ times. This is because a station is only re-enqueued when its PossibleLoads set strictly grows, and this can happen at most $C$ times (the maximum number of unique cargo types).Every time a station is processed, its departureLoad is copied. This copy operation has a time complexity of $O(C)$ because the maximum capacity of the hash set is $C$. Therefore, the total complexity of processing the stations is $O(C^2 \times S)$.
2. Track Processing Complexity: $O(C^2 \times T)$ Furthermore, the algorithm processes every track at most $C$ times. This limitation comes from the fact that a track is only evaluated when its source station is processed (which, as established, happens at most $C$ times).During each track evaluation, the algorithm performs Union and Except set operations. Since the sets contain at most $C$ elements, these operations take $O(C)$ time. Given this, the total complexity of processing the tracks is $O(C^2 \times T)$.ConclusionBy summing up these two parts, we get the total time complexity:$O(C^2 \times S + C^2 \times T) = O(C^2(S + T))$.
- **Space:** $O(S \times C)$ because every station stores hashSet of possible loads (maximally containing S loads)

## Project Structure

```
CargoSolver/
├── Program.cs                  — Entry point, interactive file input loop
├── RailwaySystemBuilder.cs     — Parses input files into RailwaySystem
├── Solver.cs                   — Fixed-point BFS algorithm
├── Models/
│   ├── Station.cs              — Station data (id, unload/load types, possible loads)
│   ├── Track.cs                — Directed edge (source → destination)
│   └── RailwaySystem.cs        — Graph structure (adjacency list, station lookup)
└── Testcases/
    ├── StraightLine.txt
    ├── BifurcationAndMerge.txt
    ├── Loop.txt
    ├── DisconnectedStations.txt
    ├── SelfLoop.txt
    └── UnloadAlongRoute.txt
```

### File Responsibilities

| File | Role |
|---|---|
| `Program.cs` | Reads file path from user input (with validation), orchestrates builder → solver → output. Supports `exit` command. |
| `RailwaySystemBuilder.cs` | Parses input file using `StreamReader`. Extracts stations, tracks, and starting station. Returns an immutable `RailwaySystem`. |
| `Solver.cs` | Implements the fixed-point BFS. |
| `Station.cs` and `Track.cs`  | Basically model classes, handling objects of Stations and Tracks |
| `RailwaySystem.cs` | Builds adjacency list from station/track collections in constructor. Provides neighbor lookup and output formatting. |

## Test Cases

### 1. StraightLine.txt — Simple linear path

```
0 ──→ 1 ──→ 2
```

**Input:**
```
3 2
0 99 10
1 10 20
2 20 30
0 1
1 2
0
```

**Expected output:**
```
Station 0: Possible Loads =
Station 1: Possible Loads = 10
Station 2: Possible Loads = 20
```

---

### 2. BifurcationAndMerge.txt — Branching and merging paths

```
    ┌──→ 1 ──┐
0 ──┤        ├──→ 3
    └──→ 2 ──┘
```

**Input:**
```
4 4
0 99 1
1 99 2
2 99 3
3 99 4
0 1
0 2
1 3
2 3
0
```

**Expected output:**
```
Station 0: Possible Loads =
Station 1: Possible Loads = 1
Station 2: Possible Loads = 1
Station 3: Possible Loads = 1, 2, 3
```

---

### 3. Loop.txt — Cycle in the graph

```
0 ──→ 1 ──→ 2 ──→ 3
▲            │
└────────────┘
```

**Input:**
```
4 4
0 99 10
1 99 20
2 99 30
3 99 40
0 1
1 2
2 0
2 3
0
```

**Expected output:**
```
Station 0: Possible Loads = 10, 20, 30
Station 1: Possible Loads = 10, 20, 30
Station 2: Possible Loads = 10, 20, 30
Station 3: Possible Loads = 10, 20, 30
```

---

### 4. DisconnectedStations.txt — Unreachable stations

```
0 ──→ 1       2 ──→ 3
              ▲
            (start)
```

**Input:**
```
4 2
0 99 10
1 99 20
2 99 30
3 99 40
0 1
2 3
2
```

**Expected output:**
```
Station 0: Possible Loads =
Station 1: Possible Loads =
Station 2: Possible Loads =
Station 3: Possible Loads = 30
```

---

### 5. SelfLoop.txt — Station with a track to itself

```
0 ──→ 1 ──→ 2
      ▲  │
      └──┘
```

**Input:**
```
3 3
0 99 10
1 10 20
2 20 30
0 1
1 1
1 2
0
```

**Expected output:**
```
Station 0: Possible Loads =
Station 1: Possible Loads = 10, 20
Station 2: Possible Loads = 20
```

---

### 6. UnloadAlongRoute.txt — Cargo unloaded mid-route

```
0 ──→ 1 ──→ 2 ──→ 3
```

Each station unloads the cargo loaded by the previous one:
station 1 unloads cargo 5 (loaded at 0), station 2 unloads cargo 7 (loaded at 1),
station 3 unloads cargo 8 (loaded at 2). Every loaded cargo is eventually unloaded,
so at each station only the freshly loaded cargo is present.

**Input:**
```
4 3
0 99 5
1 5 7
2 7 8
3 8 99
0 1
1 2
2 3
0
```

**Expected output:**
```
Station 0: Possible Loads =
Station 1: Possible Loads = 5
Station 2: Possible Loads = 7
Station 3: Possible Loads = 8
```

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) (or later)

Verify the installation:
```bash
dotnet --version
```
The output should start with `9.0` (e.g. `9.0.100`).

## Building

Clone the repository and build the project:

```bash
git clone <repository-url>
cd CargoSolver
dotnet build
```

A successful build will print:
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

## Usage

### Running the program

```bash
dotnet run --project CargoSolver
```

### Interactive mode

The program runs in a loop. At each iteration it will prompt you:

```
Please enter the path to the input file:
```

1. Enter the path to a test case file (realtive or absolute), e.g.:
   ```
   Testcases/StraightLine.txt
   ```
2. The program will print the possible cargo loads for each station:
   ```
   Processing file: Testcases/StraightLine.txt
   Possible loads:
   Station 0: Possible Loads =
   Station 1: Possible Loads = 10
   Station 2: Possible Loads = 20
   ```
3. You will be prompted again for the next file.

### Exiting

Type `exit` at the file path prompt to quit the program.


