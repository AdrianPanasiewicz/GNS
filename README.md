# GNS - Ground Station
This is repository for ground control system and data representation for Likwidator rocket project.

This repository contains the source code for the Ground Station (GNS) of the Likwidator Rocket project. The GNS is responsible for receiving, displaying, and saving real-time data from the rocket during flight.

## Features

Real-time data display: The GNS shows telemetry data from the rocket, such as altitude, speed, and other critical flight information.

Data logging: The system saves all received data for later analysis.

Mission Abort System: In case of flight anomalies, the GNS allows manual intervention to deploy emergency parachutes.

User-Friendly GUI: The application features a graphical user interface developed in C# using the .NET framework.

## Technology Stack

Programming Language: C#

Framework: .NET Framework 4.7.2.

Communication: Real-time data communication with the rocket

## Installation

1. Clone this repository:

```bash
git clone https://github.com/AdrianPanasiewicz/GNS.git
```
2. Open the project in Visual Studio or another C# IDE.
3. Build the solution and run the application.

## Usage

Launch the application from the GUI.
Connect the ground control system to the rocket via the specified communication protocol.
Monitor the real-time data.

## Contribution

The following contributed to the project:
1. Filip Sudak -  Front-end.
2. Adrian Panasiewicz - Back-end.
3. Mateusz Rowicki - USB Communication.
4. Bernard Siodłowski - LoRa communication.


We welcome contributions! Feel free to open an issue or submit a pull request.

## License

This project is licensed under the Apache License 2.0.
