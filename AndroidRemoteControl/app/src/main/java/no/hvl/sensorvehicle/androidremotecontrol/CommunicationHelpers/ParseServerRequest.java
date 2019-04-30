package no.hvl.sensorvehicle.androidremotecontrol.CommunicationHelpers;

import android.util.Pair;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.HashMap;

import no.hvl.sensorvehicle.androidremotecontrol.CommunicationHelpers.Constants.ComponentType;
import no.hvl.sensorvehicle.androidremotecontrol.CommunicationHelpers.Constants.Key;

public class ParseServerRequest {
    public static String toCompleteMessage(String jsonStringFromServer){
        // https://www.tutlane.com/tutorial/android/android-json-parsing-with-examples
        String jsonMessage = "";

        try {
            JSONObject jObj = new JSONObject(jsonStringFromServer);

            if(jObj.has(Key.ExitConfirmation)){
                jsonMessage += "EXIT CONFIRMATION:\n" + jObj.getString(Key.ExitConfirmation) + "\n\n";
            }
            if(jObj.has(ComponentType.Wheel)) {
                jsonMessage += "WHEEL:\n" + jObj.getString(ComponentType.Wheel) + "\n\n";
            }
            if(jObj.has(ComponentType.Ultrasound)) {
                jsonMessage += "ULTRASONIC:\n" + jObj.getString(ComponentType.Ultrasound) + "\n\n";
            }
            if(jObj.has(ComponentType.Lidar)) {
                jsonMessage += "LIDAR:\n" + jObj.getString(ComponentType.Lidar) + "\n\n";
            }
            if(jObj.has(ComponentType.Encoder)) {
                jsonMessage += "ENCODER:\n" + jObj.getString(ComponentType.Encoder) + "\n\n";
            }
            if(jObj.has(Key.Error)){
                jsonMessage += "AN ERROR OCCURRED:\n" + jObj.getString(Key.Error) + "\n\n";
            }
        } catch (JSONException e) {
            jsonMessage += "***************************************************\n" +
                    "AN ERROR OCCURED WHEN ATTEMPTING TO PARSE JSON RECEIVED FROM SERVER:\n" + e.getMessage();
        }

        return jsonMessage;
    }

    public static HashMap<String, String> toKeyValuePair(String jsonStringFromServer){
        // https://www.tutlane.com/tutorial/android/android-json-parsing-with-examples
        String jsonMessage = "";
        HashMap<String, String> messages = new HashMap<>();

        try {
            JSONObject jObj = new JSONObject(jsonStringFromServer);

            if(jObj.has(Key.ExitConfirmation)){
                messages.put(Key.ExitConfirmation, jObj.getString(Key.ExitConfirmation));
            }
            if(jObj.has(ComponentType.Wheel)) {
                messages.put(ComponentType.Wheel, jObj.getString(ComponentType.Wheel));
            }
            if(jObj.has(ComponentType.Ultrasound)) {
                messages.put(ComponentType.Ultrasound, jObj.getString(ComponentType.Ultrasound));
            }
            if(jObj.has(ComponentType.Lidar)) {
                messages.put(ComponentType.Lidar, jObj.getString(ComponentType.Lidar));
            }
            if(jObj.has(ComponentType.Encoder)) {
                messages.put(ComponentType.Encoder, jObj.getString(ComponentType.Encoder));
            }
            if(jObj.has(Key.Error)){
                messages.put(Key.Error, jObj.getString(Key.Error));
            }
        } catch (JSONException e) {
            messages.put("ParsingException", e.getMessage());
        }

        return messages;
    }
}
