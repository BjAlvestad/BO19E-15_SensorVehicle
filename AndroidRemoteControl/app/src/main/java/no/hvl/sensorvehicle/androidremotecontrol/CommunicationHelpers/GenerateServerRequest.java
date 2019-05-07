package no.hvl.sensorvehicle.androidremotecontrol.CommunicationHelpers;

import no.hvl.sensorvehicle.androidremotecontrol.CommunicationHelpers.Constants.ComponentType;
import no.hvl.sensorvehicle.androidremotecontrol.CommunicationHelpers.Constants.Key;
import no.hvl.sensorvehicle.androidremotecontrol.CommunicationHelpers.Constants.RequestType;

// SOME OF THE COMMANDS THAT CAN BE SENT TO SOCKET SERVER:
// Set wheel speed:     { "REQUEST_TYPE": "Command", "COMPONENT": "Wheel", "LEFT": "0", "RIGHT": "0" }
// Request Ultrasound distance:     { "REQUEST_TYPE": "Data", "COMPONENT": "Ultrasound" }
// Request Wheel and Ultrasound:     { "REQUEST_TYPE": "Data", "COMPONENT": "Wheel Ultrasound" }
public class GenerateServerRequest {

    public static String setPower(int left, int right){
        return "{ " + request(RequestType.Command) + ", " + component(ComponentType.Wheel) + ", \"LEFT\": \"" + left + "\", \"RIGHT\": \"" + right + "\" }";
    }

    public static String getSensorData(String... components){
        return "{ " + request(RequestType.Data) + ", " + component(components) + " }";
    }

    public static String stopControlLogic(){
        return "{ " + request(RequestType.Command) + ", " + component(ComponentType.StopControlLogic) + " }";
    }

    public static String restartControlLogic(){
        return "{ " + request(RequestType.Command) + ", " + component(ComponentType.RestartControlLogic) + " }";
    }

    private static String request(String Command){
        return "\"" + Key.RequestType + "\": \"" + Command + "\"";
    }

    private static String component(String... components){
        String componentsString = "";
        for (String component : components) {
            componentsString += component + " ";
        }
        componentsString = componentsString.trim();

        return "\"" + Key.Component + "\": \"" + componentsString + "\"";
    }
}
