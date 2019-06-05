# BO19E-15 Sensor vehicle for use by students in practical application of interdisciplinary skills 

## Table of Contents

* [About the Project](#about-the-project)
  * [Built With](#built-with)
* [Getting Started](#getting-started)
  * [Prerequisites](#prerequisites)
  * [Installation](#installation)
* [Usage](#usage)
* [Contributing](#contributing)
* [License](#license)
* [Contact](#contact)
* [Acknowledgements](#acknowledgements)

## About The Project
This project was the bachelor thesis of Bjørnar Alvestad, Benjamin Odland Skare and Erlend Lone.
Our goal was to make a learining tool for automation/electrical engineering students at Western Norway University of Applied Science. The physical car we made has ultrasonic sensors and a LiDAR for measuring distance to obstacles and it has encoders that is used to determine the speed of the car and the distance travelled. HVL SensorVehicle Simulator was made so that the students could test their control logics without having the physical car present.


### Built With
**<u>SensorVehicle-main</u>**

* MVVM framework [Prism](https://prismlibrary.github.io/docs/)



**<u>Simulator-2D</u>**

* Game developement framework [MonoGame](http://www.monogame.net/)

* Map editor [Tiled](https://www.mapeditor.org/)

  

**<u>SensorVehicle-extras</u>**

* Starting point for webcam streaming [HttpWebcamLiveStream](https://github.com/SaschaIoT/HttpWebcamLiveStream/tree/master/HttpWebcamLiveStream)

  

## Getting Started
To fully use HVL SensorVehicle Main and write your own control logic you need to:
* Clone this repository
```sh
git clone https://github.com/BjAlvestad/BO19E-15_SensorVehicle-Simplified.git
```
* Install HVL SensorVehicle Simulator from https://www.microsoft.com/en-us/p/hvl-sensorvehicle-simulator/9nbs6gn8sqlg?activetab=pivot:overviewtab
* Open SensorVehicle-main.sln and navigate to one of the StudentXX.cs-files

## Usage
![Control logic example](%5BDrawings%20and%20Documents%5D/Pictures/DriveToLargestDistance.png)
The above example shows how one can make the car drive towards the largest distance reported by the LiDAR.

To write your own control logic, navigate to the files in the picture below.
The different StudentXX.cs-files contains instruction where to write the control logic.
<img src="%5BDrawings%20and%20Documents%5D/Pictures/StudentLogicCS.png" width="251" height="197"> <img src="%5BDrawings%20and%20Documents%5D/Pictures/StudentLogicPage.png" width="473" height="177">
An overview of the different control logics from the shown .cs-files are shown in the student logic page.



<img src="%5BDrawings%20and%20Documents%5D/Pictures/Prototype1.png" width="200" height="162">    <img src="%5BDrawings%20and%20Documents%5D/Pictures/Prototype2.png" width="200" height="162">    <img src="%5BDrawings%20and%20Documents%5D/Pictures/SensorVehicle.png" width="200" height="162">
The different version of the car that was made are shown in the pictures above.
The first version was made to get the different hardware components to work.
The second version was made fully functional. The complete software was made by using this version of the sensor vehicle.
The third and final version was made even more solid than the second version. The brackets were 3D-printed and the sheet were cut with laser cutter.

**********REMOVE THIS**********************
Use this space to show useful examples of how a project can be used. Additional screenshots, code examples and demos work well in this space. You may also link to more resources.

_For more examples, please refer to the [Documentation](https://example.com)_



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
* [GitHub Emoji Cheat Sheet](https://www.webpagefx.com/tools/emoji-cheat-sheet)





[build-shield]: https://img.shields.io/badge/build-passing-brightgreen.svg?style=flat-square
[contributors-shield]: https://img.shields.io/badge/contributors-1-orange.svg?style=flat-square
[license-shield]: https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square
[license-url]: https://choosealicense.com/licenses/mit
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=flat-square&logo=linkedin&colorB=555
[linkedin-url]: https://linkedin.com/in/othneildrew
[product-screenshot]: https://raw.githubusercontent.com/othneildrew/Best-README-Template/master/screenshot.png