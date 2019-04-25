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

public class ConnectionActivity extends AppCompatActivity {

    TextView textViewInfo;

    private static Socket s;
    private static ServerSocket ss;
    private static InputStreamReader isr;
    private static BufferedReader br;
    private static PrintWriter printWriter;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_connection);

        textViewInfo = findViewById (R.id.textViewInfo);
        textViewInfo.setText ("Select vehicle to connect");

       // buttonEnable (R.id.btnMoveVehicle, false);
        buttonEnable (R.id.btnSensorData, false);
    }

    @SuppressLint({"SetTextI18n", "ResourceType"})
    public void onClickedConnectVehicle1(View view) {

        buttonEnable (R.id.btnConnectVehicle1, false);
        buttonEnable (R.id.btnConnectVehicle2, false);
        buttonEnable (R.id.btnConnectDefault, false);

        EditText editText = findViewById (R.id.editTextIpUi);
        editText.setText (getResources ().getString (R.string.ip_vehicle_1));

        textViewInfo.setText ("Connecting: " + getResources ().getString (R.string.vehicle_1));

        // start connecting service

        myTask mt = new myTask ();
        mt.execute ();

    }

    @SuppressLint({"ResourceType", "SetTextI18n"})
    public void onClickedConnectVehicle2(View view) {
        buttonEnable (R.id.btnConnectVehicle1, false);
        buttonEnable (R.id.btnConnectVehicle2, false);
        buttonEnable (R.id.btnConnectDefault, false);

        EditText editText = findViewById (R.id.editTextIpUi);
        editText.setText (getResources ().getString (R.string.ip_vehicle_2));

        textViewInfo.setText ("Connecting: " + getResources ().getString (R.string.vehicle_1));

        // start connecting service
    }

    @SuppressLint("SetTextI18n")
    public void OnClickedConnectDefault(View view) {
        buttonEnable (R.id.btnConnectVehicle1, false);
        buttonEnable (R.id.btnConnectVehicle2, false);
        buttonEnable (R.id.btnConnectDefault, false);

        EditText editText = findViewById (R.id.editTextIpUi);
        String ip = String.valueOf (editText.getText ());

        textViewInfo.setText ("Connecting: " + "default");

        // start connecting service
    }

    public void onClickedMoveVehicle(View view) {
        Intent intent = new Intent (this, MoveActivity.class);
        startActivity (intent);
    }

    public void onClickedSensorData(View view) {
    }

    private void buttonEnable(int id, boolean b){
        Button btn = findViewById (id);
        btn.setEnabled (b);
    }


    class myTask extends AsyncTask<Void, Void, Void> {

        @Override
        protected Void doInBackground(Void... voids) {

            try {
                Log.i ("main", "enter background");
                s= new Socket ("158.37.76.13", 51915);
                Log.i ("main", s.toString ());



                printWriter = new PrintWriter (s.getOutputStream ());
                printWriter.write (GenerateServerRequest.setPower(-30, 30));
                printWriter.flush ();
                Log.i ("main", "datasent");

                printWriter.close ();



                s.close ();


            } catch (UnknownHostException e) {
                e.printStackTrace ();
            } catch (IOException e) {
                e.printStackTrace ();
            }
            return null;
        }
    }
}
