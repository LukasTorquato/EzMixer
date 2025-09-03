<img width="1230" height="739" alt="Screenshot 2023-03-03 172318" src="https://github.com/user-attachments/assets/abd3df3d-9d8e-4877-bd96-5467a2357581" /># EzMixer - Hardware Audio Control for Windows

EzMixer is a powerful Windows application that provides tactile, hardware-based control over your application audio levels. It interfaces with a custom-built controller, powered by an Atmega microcontroller, to give you independent volume control for different applications, complete with customizable RGB lighting.

This project combines hardware and software to create a seamless experience for managing your audio, perfect for streamers, gamers, and anyone who needs fine-grained control over their sound.

Main View:
<img width="1232" height="737" alt="Screenshot 2023-03-03 162557" src="https://github.com/user-attachments/assets/a2aafdfa-50ab-46bf-a173-53a186cf5548" />

Lighting View:
<img width="1234" height="742" alt="Screenshot 2023-03-03 172229" src="https://github.com/user-attachments/assets/e6d7def6-91ae-491f-b2af-a52a4145b436" />

Configurations Tab:
<img width="1230" height="739" alt="Screenshot 2023-03-03 172318" src="https://github.com/user-attachments/assets/e5e62885-81c6-446e-8a50-68c23c70bb1b" />

## Key Features

* **Independent Volume Control**: Use physical knobs to adjust the volume of individual applications, just like a real audio mixer.
* **Application Selector**: Easily switch between applications to control their audio levels on the fly.
* **Customizable RGB Lighting**: Assign unique RGB colors to each application for visual feedback and a personalized setup.
* **WPF Application**: A modern and responsive user interface built with C# and .NET Core.
* **Hardware Integration**: Communicates with an Atmega-based controller for a true hardware-in-the-loop experience.

## Technologies Used

* **Programming Language**: C#
* **Framework**: .NET Core
* **UI**: Windows Presentation Foundation (WPF)
* **Hardware Communication**: Serial Communication
* **Microcontroller**: Atmega (compatible with Arduino)

## How It Works

EzMixer runs in the background on your Windows machine and continuously monitors for an Atmega device connected via a serial port. When the hardware is detected, the application listens for input from the physical knobs.

1.  **Hardware Input**: When you turn a knob on the controller, the Atmega device sends a signal to the EzMixer application.
2.  **Application Mapping**: The application identifies which knob was turned and which application it's mapped to.
3.  **Volume Adjustment**: EzMixer then adjusts the volume of the corresponding application in the Windows audio mixer.
4.  **RGB Feedback**: The application sends a signal back to the controller to update the RGB lighting, providing visual feedback for the selected application.

## Getting Started

To get started with EzMixer, you'll need to set up both the hardware and the software.

### Prerequisites

* A Windows machine
* Visual Studio with .NET Core development tools
* An Atmega-based microcontroller (like an Arduino)
* The necessary electronic components for the controller (potentiometers, RGB LEDs, etc.)

### Installation

1.  Clone the repository to your local machine:
    ```bash
    git clone [https://github.com/your-username/EzMixer.git](https://github.com/your-username/EzMixer.git)
    ```
2.  Open the `EzMixer.sln` file in Visual Studio.
3.  Build the solution to restore the NuGet packages and compile the application.

### Hardware Setup

You will need to build the controller circuit and flash the firmware to the Atmega device. The firmware should be programmed to send specific serial messages when a knob is turned.

### Configuration

Once the application is running, you can configure the following from the user interface:

* **Application Mapping**: Assign each knob to a specific application running on your system.
* **RGB Lighting**: Choose a unique color for each application to be displayed on the controller.

## Contributing

Contributions are welcome! If you have any ideas for new features or improvements, feel free to fork the repository, make your changes, and open a pull request.

## License

This project is licensed under the MIT License. See the `LICENSE` file for more details.
