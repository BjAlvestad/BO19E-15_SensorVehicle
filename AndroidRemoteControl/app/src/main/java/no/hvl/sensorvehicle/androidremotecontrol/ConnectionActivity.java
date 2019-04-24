package no.hvl.sensorvehicle.androidremotecontrol;

import android.annotation.SuppressLint;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;

public class ConnectionActivity extends AppCompatActivity {

    TextView textViewInfo;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_connection);

        textViewInfo = findViewById (R.id.textViewInfo);
        textViewInfo.setText ("Select vehicle to connect");

        buttonEnable (R.id.btnMoveVehicle, false);
        buttonEnable (R.id.btnSensorData, false);
    }

    @SuppressLint({"SetTextI18n", "ResourceType"})
    public void onClickedConnectVehicle1(View view) {

        buttonEnable (R.id.btnConnectVehicle1, false);
        buttonEnable (R.id.btnConnectVehicle2, false);
        buttonEnable (R.id.btnConnectDefault, false);

        EditText editText = findViewById (R.id.editTextIpUi);
        editText.setText (R.string.ip_vehicle_1);

        textViewInfo.setText ("Connecting: " + R.string.vehicle_1);

        // start connecting service

    }

    @SuppressLint("ResourceType")
    public void onClickedConnectVehicle2(View view) {
        buttonEnable (R.id.btnConnectVehicle1, false);
        buttonEnable (R.id.btnConnectVehicle2, false);
        buttonEnable (R.id.btnConnectDefault, false);

        EditText editText = findViewById (R.id.editTextIpUi);
        editText.setText (R.string.ip_vehicle_2);

        textViewInfo.setText ("Connecting: " + R.string.vehicle_1);

        // start connecting service
    }

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
    }

    public void onClickedSensorData(View view) {
    }

    private void buttonEnable(int id, boolean b){
        Button btn = findViewById (id);
        btn.setEnabled (b);
    }
}
