package no.hvl.sensorvehicle.androidremotecontrol;

import android.annotation.SuppressLint;
import android.graphics.Color;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.RelativeLayout;

public class MoveActivity extends AppCompatActivity {

    private final String TAG = "MoveActivity";

    private  RelativeLayout relativeLayout;
    private ImageView vehicleIcon;
    private View iconView;

    private int xDelta;
    private int yDelta;


    private int rLayoutMarginLeft;
    private int rLayoutMarginTop;

    private int rLayoutHigh;
    private int rLayoutWidth;

    private int iconHigh;
    private int iconWidth;

    @SuppressLint("ClickableViewAccessibility")
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate (savedInstanceState);
        setContentView (R.layout.activity_move);


        iconView = findViewById (R.id.vehicleImage);

        iconHigh = iconView.getLayoutParams ().height;
        iconWidth = iconView.getLayoutParams ().width;

        Log.i ("icon", iconHigh + "   " + iconWidth);


        relativeLayout  = findViewById (R.id.layoutMove);

        rLayoutHigh = relativeLayout.getLayoutParams ().height;
        rLayoutWidth = relativeLayout.getLayoutParams ().width;

        Log.i (TAG, rLayoutHigh + "   " + rLayoutWidth);

        rLayoutMarginLeft = relativeLayout.getLeft ();
        rLayoutMarginTop = relativeLayout.getTop ();

        Log.i (TAG, rLayoutMarginLeft + "   " + rLayoutMarginTop);




        relativeLayout.setOnTouchListener (onHandleTouchRl ());
    }

    @Override
    protected void onStart() {
        super.onStart ();

        ViewGroup viewGroup = findViewById (R.id.layoutMove);
        viewGroup.setBackgroundColor (Color.LTGRAY);

        RelativeLayout.LayoutParams lParams=(RelativeLayout.LayoutParams)iconView.getLayoutParams();
        lParams.leftMargin = rLayoutWidth/2 - iconWidth/2;
        lParams.topMargin = rLayoutHigh/2 - iconHigh/2;
        iconView.setLayoutParams (lParams);
    }

    private View.OnTouchListener onHandleTouchRl() {
        return new View.OnTouchListener () {
            @Override
            public boolean onTouch(View v, MotionEvent event) {

                 int x = (int) event.getX ();
                 int y = (int) event.getY ();
               // Log.i (TAG, x + "   " + y );

                RelativeLayout.LayoutParams lParams=(RelativeLayout.LayoutParams)iconView.getLayoutParams();
                lParams.leftMargin = x;
                lParams.topMargin = y;
                iconView.setLayoutParams (lParams);

                valueHandler(x, y);
                return true;
            }
        };
    }

    private void valueHandler(int x, int y) {

        // value inside view
        if (x<0) x=0;
        if (x>rLayoutWidth) x= rLayoutWidth;
        if (y<0) y=0;
        if (y>rLayoutHigh) y=rLayoutHigh;

        // define origo
        int origoX = rLayoutWidth/2;
        int origoY = rLayoutHigh/2;

        // value from origo
        if (x>0) x = x/2 - origoX;
        if (y>0) y = y/2 - origoY;

        Log.i (TAG, x + "   " + y);

        int wheelLeft=0;
        int wheelRight=0;




    }

}
