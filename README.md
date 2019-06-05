# BO19E-15 Sensor vehicle for use by students in practical application of interdisciplinary skills 

## Table of Contents

* [About the Project](#about-the-project)
  * [Built With](#built-with)
* [Getting Started](#getting-started)
* [Usage](#usage)
* [Contributing](#contributing)
* [License](#license)
* [Acknowledgements](#acknowledgements)

## About The Project
This project was the bachelor thesis of Bjørnar Alvestad, Benjamin Odland Skare and Erlend Lone.
Our goal was to make a learining tool for automation/electrical engineering students at Western Norway University of Applied Science. The physical car we made has ultrasonic sensors and a LiDAR for measuring distance to obstacles and it has encoders that is used to determine the speed of the car and the distance travelled. HVL SensorVehicle Simulator was made so that the students could test their control logics without having the physical car present.


### Built With
**<u>SensorVehicle-main (UWP app, C#, XAML)</u>**
* Extension that makes it easier to create UWP apps [Windows Template Studio](https://marketplace.visualstudio.com/items?itemName=WASTeamAccount.WindowsTemplateStudio)	

* MVVM framework [Prism](https://prismlibrary.github.io/docs/)

**<u>Simulator-2D</u> (UWP app, C#, XAML)**

* Game developement framework [MonoGame](http://www.monogame.net/)

* Simple 2D camera for Monogame [Comora](https://github.com/aloisdeniel/Comora)

* Map editor [Tiled](https://www.mapeditor.org/)

* Importer for Tiled maps [TiledSharp](https://github.com/marshallward/TiledSharp)

  

**<u>SensorVehicle-extras (UWP app, C#, XAML, JavaScript, CSS, HTML)</u>**

* Starting point for webcam streaming [HttpWebcamLiveStream](https://github.com/SaschaIoT/HttpWebcamLiveStream/tree/master/HttpWebcamLiveStream)

**<u>Android app for remote control (Java - Android Studio)</u>**

**<u>Code for microcontrollers (C++ - Visual Micro)</u>**

  

## Getting Started
To fully use HVL SensorVehicle Main and write your own control logic you need to: <br>
* **<u>For writing your own control logic:</u>**
```sh
git clone https://github.com/BjAlvestad/BO19E-15_SensorVehicle-Simplified.git
```
* **<u>For access to the entire software:</u>**
```sh
git clone https://github.com/BjAlvestad/BO19E-15_SensorVehicle.git
```
* Install HVL SensorVehicle Simulator from [Microsoft Store](https://www.microsoft.com/en-us/p/hvl-sensorvehicle-simulator/9nbs6gn8sqlg?activetab=pivot:overviewtab)
* Open SensorVehicle-main.sln and navigate to one of the StudentXX.cs-files

## Usage
![Control logic example](%5BDrawings%20and%20Documents%5D/Pictures/DriveToLargestDistance.png) <br>
The above example shows how one can make the car drive towards the largest distance reported by the LiDAR.

To write your own control logic, navigate to the files in the picture below.
The different StudentXX.cs-files contains instruction where to write the control logic. <br>

<img src="%5BDrawings%20and%20Documents%5D/Pictures/StudentLogicCS.png" width="251" height="197"> <img src="%5BDrawings%20and%20Documents%5D/Pictures/StudentLogicPage.png" width="473" height="177"> <br>
An overview of the different control logics from the shown .cs-files will appear in the student logic page. <br><br>


<img src="%5BDrawings%20and%20Documents%5D/Pictures/Prototype1.png" width="200" height="162">    <img src="%5BDrawings%20and%20Documents%5D/Pictures/Prototype2.png" width="200" height="162">    <img src="%5BDrawings%20and%20Documents%5D/Pictures/SensorVehicle.PNG" width="200" height="162"> <br><br>
The different version of the car that was made are shown in the pictures above.
The first version was made to get the different hardware components to work.
The second version was made fully functional. The complete software was made by using this version of the sensor vehicle.
The third and final version was made even more solid than the second version. The brackets were 3D-printed and the sheet were cut with laser cutter.



For more information about the development process and a detailed user manual, see [Bachelor thesis document](https://github.com/BjAlvestad/BO19E-15_SensorVehicle/blob/ReadMeTestBranch/%5BDrawings%20and%20Documents%5D/Bachelor%20Thesis%20(text%20in%20Norwegian)/BO19E-15%20Bacheloroppgave.pdf) (Norwegian)



## Contributing

Contributions are what make the open source community such an amazing place to be learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request



## License

Distributed under the MIT License. See `LICENSE` for more information.



## Acknowledgements
* This README is based on [othneildrew's Best-README-Template](https://github.com/othneildrew/Best-README-Template/blob/master/README.md)