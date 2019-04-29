package no.hvl.sensorvehicle.androidremotecontrol;

import android.annotation.SuppressLint;
import android.content.Intent;
import android.os.AsyncTask;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.ServerSocket;
import java.net.Socket;
import java.net.UnknownHostException;

import no.hvl.sensorvehicle.androidremotecontrol.CommunicationHelpers.GenerateServerRequest;

import static no.hvl.sensorvehicle.androidremotecontrol.CommunicationHelpers.Constants.Address.IpVehicle1;
import static no.hvl.sensorvehicle.androidremotecontrol.CommunicationHelpers.Constants.Address.IpVehicle2;
import static no.hvl.sensorvehicle.androidremotecontrol.CommunicationHelpers.Constants.Address.Port;

public class ConnectionActivity extends AppCompatActivity {

    TextView textViewInfo;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate (savedInstanceState);
        setContentView (R.layout.activity_connection);

        textViewInfo = findViewById (R.id.textViewInfo);
        textViewInfo.setText ("Select vehicle to connect");

        buttonEnable (R.id.btnMoveVehicle, false);
        buttonEnable (R.id.btnSensorData, false);
        buttonEnable (R.id.btnDisconnect, false);
    }

    @SuppressLint({"SetTextI18n", "ResourceType"})
    public void onClickedConnectVehicle1(View view) {
        buttonEnable (R.id.btnConnectVehicle1, false);
        buttonEnable (R.id.btnConnectVehicle2, false);
        buttonEnable (R.id.btnConnectDefault, false);

        EditText editText = findViewById (R.id.editTextIpUi);
        editText.setText (IpVehicle1);

        textViewInfo.setText ("Connecting: " + getResources ().getString (R.string.vehicle_1));

        ConnectionHandler.connect (IpVehicle1, Port);

        buttonEnable (R.id.btnMoveVehicle, true);
        buttonEnable (R.id.btnSensorData, true);
        buttonEnable (R.id.btnDisconnect, true);
    }

    @SuppressLint({"ResourceType", "SetTextI18n"})
    public void onClickedConnectVehicle2(View view) {
        buttonEnable (R.id.btnConnectVehicle1, false);
        buttonEnable (R.id.btnConnectVehicle2, false);
        buttonEnable (R.id.btnConnectDefault, false);

        EditText editText = findViewById (R.id.editTextIpUi);
        editText.setText (IpVehicle2);

        textViewInfo.setText ("Connecting: " + IpVehicle2);

        ConnectionHandler.connect (IpVehicle2, Port);

        buttonEnable (R.id.btnMoveVehicle, true);
        buttonEnable (R.id.btnSensorData, true);
        buttonEnable (R.id.btnDisconnect, true);

    }

    @SuppressLint("SetTextI18n")
    public void OnClickedConnectDefault(View view) {
        buttonEnable (R.id.btnConnectVehicle1, false);
        buttonEnable (R.id.btnConnectVehicle2, false);
        buttonEnable (R.id.btnConnectDefault, false);

        EditText editText = findViewById (R.id.editTextIpUi);
        String ip = String.valueOf (editText.getText ());

        textViewInfo.setText ("Connecting: " + ip);

        ConnectionHandler.connect (ip, Port);

        buttonEnable (R.id.btnMoveVehicle, true);
        buttonEnable (R.id.btnSensorData, true);
        buttonEnable (R.id.btnDisconnect, true);
    }

    public void onClickedMoveVehicle(View view) {
        Intent intent = new Intent (this, MoveActivity.class);
        startActivity (intent);
    }

    public void onClickedSensorData(View view) {
        Intent intent = new Intent (this, DataActivity.class);
        startActivity (intent);
    }

    private void buttonEnable(int id, boolean b) {
        Button btn = findViewById (id);
        btn.setEnabled (b);
    }

    public void onClickedDisconnect(View view) {
        //ConnectionHandler.sendMessage (GenerateServerRequest.exitMessage ());

        try {
            ConnectionHandler.closeSocket ();
        } catch (IOException e) {
            e.printStackTrace ();
        }

        buttonEnable (R.id.btnConnectVehicle1, true);
        buttonEnable (R.id.btnConnectVehicle2, true);
        buttonEnable (R.id.btnConnectDefault, true);
        buttonEnable (R.id.btnMoveVehicle, false);
        buttonEnable (R.id.btnSensorData, false);
    }
}
