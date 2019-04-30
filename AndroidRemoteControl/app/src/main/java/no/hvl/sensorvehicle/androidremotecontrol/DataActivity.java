package no.hvl.sensorvehicle.androidremotecontrol;

import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.CheckBox;

import java.util.ArrayList;
import java.util.List;

import no.hvl.sensorvehicle.androidremotecontrol.CommunicationHelpers.Constants.ComponentType;
import no.hvl.sensorvehicle.androidremotecontrol.CommunicationHelpers.Constants.Key;
import no.hvl.sensorvehicle.androidremotecontrol.CommunicationHelpers.GenerateServerRequest;

public class DataActivity extends AppCompatActivity {
    private ArrayList<String> components;
    private String componentsString;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate (savedInstanceState);
        setContentView (R.layout.activity_data);

        components = new ArrayList<>();
        components.add(ComponentType.Wheel);
        components.add(ComponentType.Ultrasound);
        componentsString = getStringFromArrayList(components);
    }


    public void onClick_getSensorData(View view) {
        ConnectionHandler.sendMessage(
                GenerateServerRequest.getSensorData(componentsString),
                getApplicationContext());
    }

    public void onClick_ComponentCheckBox(View view) {
        // Is the view now checked?
        boolean checked = ((CheckBox) view).isChecked();

        // Check which checkbox was clicked
        switch(view.getId()) {
            case R.id.checkBox_wheels:
                if (checked && !components.contains(ComponentType.Wheel)) components.add(ComponentType.Wheel);
                else components.remove(ComponentType.Wheel);
                break;
            case R.id.checkBox_ultrasound:
                if (checked && !components.contains(ComponentType.Ultrasound)) components.add(ComponentType.Ultrasound);
                else components.remove(ComponentType.Ultrasound);
                break;
            case R.id.checkBox_lidar:
                if (checked && !components.contains(ComponentType.Lidar)) components.add(ComponentType.Lidar);
                else components.remove(ComponentType.Lidar);
                break;
            case R.id.checkBox_encoder:
                if (checked && !components.contains(ComponentType.Encoder)) components.add(ComponentType.Encoder);
                else components.remove(ComponentType.Encoder);
                break;
        }
        componentsString = getStringFromArrayList(components);
    }

    private String getStringFromArrayList(ArrayList<String> list){
        String componentsString = "";
        for (String component : components) {
            componentsString += component + " ";
        }
        componentsString = componentsString.trim();

        return componentsString;
    }
}
