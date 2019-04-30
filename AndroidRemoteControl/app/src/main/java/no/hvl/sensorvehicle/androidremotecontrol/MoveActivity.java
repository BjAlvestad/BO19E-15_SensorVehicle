package no.hvl.sensorvehicle.androidremotecontrol;

import android.annotation.SuppressLint;
import android.content.Intent;
import android.graphics.Color;
import android.support.annotation.Nullable;
import android.support.v4.view.KeyEventDispatcher;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.webkit.WebSettings;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.ImageView;
import android.widget.RelativeLayout;
import android.widget.TextView;

import no.hvl.sensorvehicle.androidremotecontrol.CommunicationHelpers.Constants.ComponentType;
import no.hvl.sensorvehicle.androidremotecontrol.CommunicationHelpers.GenerateServerRequest;

public class MoveActivity extends AppCompatActivity {

    private final String TAG = "MoveActivity";
    private final int STEP = 6;

    private View iconView;

    private int rLayoutHigh;
    private int rLayoutWidth;

    private int iconHigh;
    private int iconWidth;

    @SuppressLint("ClickableViewAccessibility")
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate (savedInstanceState);
        setContentView (R.layout.activity_move);

        // Find and place icon
        iconView = findViewById (R.id.vehicleImage);
        iconHigh = iconView.getLayoutParams ().height;
        iconWidth = iconView.getLayoutParams ().width;
        Log.i ("icon", iconHigh + "   " + iconWidth);

        // Get params
        RelativeLayout relativeLayout = findViewById (R.id.layoutMove);
        rLayoutHigh = relativeLayout.getLayoutParams ().height;
        rLayoutWidth = relativeLayout.getLayoutParams ().width;
        Log.i (TAG, rLayoutHigh + "   " + rLayoutWidth);

        int rLayoutMarginLeft = relativeLayout.getLeft ();
        int rLayoutMarginTop = relativeLayout.getTop ();
        Log.i (TAG, rLayoutMarginLeft + "   " + rLayoutMarginTop);

        relativeLayout.setOnTouchListener (onHandleTouchRl ());
    }

    @Override
    protected void onStart() {
        super.onStart ();

        // Place icon in center
        ViewGroup viewGroup = findViewById (R.id.layoutMove);
        viewGroup.setBackgroundColor (Color.LTGRAY);

        RelativeLayout.LayoutParams lParams = (RelativeLayout.LayoutParams) iconView.getLayoutParams ();
        lParams.leftMargin = rLayoutWidth / 2 - iconWidth / 2;
        lParams.topMargin = rLayoutHigh / 2 - iconHigh / 2;
        iconView.setLayoutParams (lParams);

    }

    private View.OnTouchListener onHandleTouchRl() {
        return new View.OnTouchListener () {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                int x = (int) event.getX ();
                int y = (int) event.getY ();

                valueHandler (x, y);
                return true;
            }
        };
    }

    private int lastLeft = 0;
    private int lastRight = 0;

    private void valueHandler(int x, int y) {

        if (ConnectionHandler.sending) return;

        Log.i (TAG, "-------------------");
        Log.i (TAG, x + "   " + y);

        // value inside view
        if (x < 0) x = 0;
        if (x > rLayoutWidth) x = rLayoutWidth;
        if (y < 0) y = 0;
        if (y > rLayoutHigh) y = rLayoutHigh;

        Log.i (TAG, x + "   " + y);

        // define origo
        int origoX = rLayoutWidth / 2;
        int origoY = rLayoutHigh / 2;

        // value from origo
        if (x > -1) x = x - origoX;
        if (y > -1) y = y - origoY;

        // Change direction of y
        y = y * -1;

        Log.i (TAG, x + "   " + y);

        // % of width
        double percX = x / (rLayoutWidth / 2.0);
        double percY = y / (rLayoutHigh / 2.0);

        Log.i (TAG, percX + "   " + percY);

        // Set power back/forward -100 - 100
        int wheelLeft = (int) (100 * percY);
        int wheelRight = (int) (100 * percY);

        Log.i (TAG, wheelLeft + "   " + wheelRight);

        // Steering
        if (percX < 0) {
            wheelLeft = (int) (wheelLeft + (wheelLeft) * percX);
        } else if (percX > 0) {
            wheelRight = (int) (wheelRight - (wheelRight) * percX);
        }
        Log.i (TAG, wheelLeft + "   " + wheelRight);

        //
        if (newStepCheck (wheelLeft, lastLeft) || newStepCheck (wheelRight, lastRight)) {
            lastLeft = wheelLeft;
            lastRight = wheelRight;

            ConnectionHandler.sendMessage (GenerateServerRequest.setPower (wheelLeft, wheelRight), getApplicationContext());
        }
    }

    private boolean newStepCheck(int wheelNew, int wheelLast) {
        int diff = wheelLast - wheelNew;

        if (diff < -STEP || diff > STEP) {
            return true;
        } else {
            return false;
        }
    }

    public void onClickedStop(View view) {
        ConnectionHandler.sendMessage (GenerateServerRequest.setPower (0, 0), getApplicationContext());
    }
}
